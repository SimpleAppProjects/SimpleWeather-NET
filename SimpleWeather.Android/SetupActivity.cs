using SimpleWeather.Utils;
using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views.InputMethods;
using Android.Content;
using Android.Support.V4.App;
using SimpleWeather.Droid.App.Helpers;
using Android.Support.V4.Content;
using Android.Locations;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using Android;
using Android.Content.PM;
using Android.Runtime;
using SimpleWeather.Droid.App.Controls;
using SimpleWeather.Droid.Utils;
using System.Collections.Specialized;
using System.Threading;
using Android.Graphics;
using Android.Appwidget;
using SimpleWeather.Droid.App.Widgets;
using SimpleWeather.Droid.Helpers;

namespace SimpleWeather.Droid.App
{
    [Android.App.Activity(
        Theme = "@style/SetupTheme",
        WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustPan)]
    public class SetupActivity : AppCompatActivity, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        private LocationSearchFragment mSearchFragment;
        private Android.Support.V7.View.ActionMode mActionMode;
        private View searchViewLayout;
        private View searchViewContainer;
        private Spinner apiSpinner;
        private EditText keyEntry;
        private EditText searchView;
        private TextView clearButtonView;
        private bool inSearchUI;

        private Button gpsFollowButton;
        private ProgressBar progressBar;
        private Location mLocation;
        private LocationListener mLocListnr;
        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;
        private CancellationTokenSource cts;

        private ActionModeCallback mActionModeCallback = new ActionModeCallback();
        private WeatherData.WeatherManager wm;

        // Widget id for ConfigurationActivity
        private int mAppWidgetId = AppWidgetManager.InvalidAppwidgetId;

        private bool OnCreateActionMode(Android.Support.V7.View.ActionMode mode, IMenu menu)
        {
            if (searchViewLayout == null)
                searchViewLayout = LayoutInflater.Inflate(Resource.Layout.search_action_bar, null);

            mode.CustomView = searchViewLayout;
            EnterSearchUi();
            return true;
        }

        private void OnDestroyActionMode(Android.Support.V7.View.ActionMode mode)
        {
            ExitSearchUi();
            mActionMode = null;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Check if this activity was started from adding a new widget
            if (Intent != null && AppWidgetManager.ActionAppwidgetConfigure.Equals(Intent.Action))
            {
                mAppWidgetId = Intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId);

                if (Settings.WeatherLoaded || mAppWidgetId == AppWidgetManager.InvalidAppwidgetId)
                {
                    // This shouldn't happen, but just in case
                    SetResult(Android.App.Result.Ok);
                    Finish();
                    // Return if we're finished
                    return;
                }

                if (mAppWidgetId != AppWidgetManager.InvalidAppwidgetId)
                    // Set the result to CANCELED.  This will cause the widget host to cancel
                    // out of the widget placement if they press the back button.
                    SetResult(Android.App.Result.Canceled, new Intent().PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId));
            }

            SetContentView(Resource.Layout.activity_setup);

            cts = new CancellationTokenSource();

            mActionModeCallback.CreateActionMode += OnCreateActionMode;
            mActionModeCallback.DestroyActionMode += OnDestroyActionMode;

            // Get ActionMode state
            if (savedInstanceState != null && savedInstanceState.GetBoolean("SearchUI", false))
            {
                inSearchUI = true;

                // Restart ActionMode
                mActionMode = StartSupportActionMode(mActionModeCallback);
            }

            wm = WeatherData.WeatherManager.GetInstance();

            searchViewContainer = FindViewById(Resource.Id.search_view_container);
            apiSpinner = FindViewById<Spinner>(Resource.Id.api_spinner);
            keyEntry = FindViewById<EditText>(Resource.Id.key_entry);
            gpsFollowButton = FindViewById<Button>(Resource.Id.gps_follow);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            progressBar.Visibility = ViewStates.Gone;

            // Setup spinner
            var spinnerAdapter = new ArrayAdapter<ComboBoxItem>(
                this, Android.Resource.Layout.SimpleSpinnerItem,
                WeatherData.WeatherAPI.APIs.ToArray());
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            apiSpinner.Adapter = spinnerAdapter;

            // NOTE: Bug: Explicitly set tintmode on Lollipop devices
            if (Build.VERSION.SdkInt == BuildVersionCodes.Lollipop)
                progressBar.IndeterminateTintMode = PorterDuff.Mode.SrcIn;

            /* Event Listeners */
            apiSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                Spinner spinner = sender as Spinner;
                Settings.API = (spinner.SelectedItem as ComboBoxItem).Value;
                wm.UpdateAPI();

                if (wm.KeyRequired)
                    FindViewById(Resource.Id.key_entry_box).Visibility = ViewStates.Visible;
                else
                {
                    FindViewById(Resource.Id.key_entry_box).Visibility = ViewStates.Invisible;
                    Settings.API_KEY = keyEntry.Text = String.Empty;
                }
            };

            keyEntry.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                View v = sender as View;

                if (e.HasFocus)
                {
                    ShowInputMethod(v.FindFocus());
                }
                else
                {
                    HideInputMethod(v);
                }
            };

            keyEntry.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                if (!String.IsNullOrWhiteSpace(e.Text.ToString()))
                {
                    Settings.API_KEY = e.Text.ToString();
                }
            };

            FindViewById(Resource.Id.activity_setup).Click += delegate
            {
                keyEntry.ClearFocus();
            };

            searchViewContainer.Click += delegate
            {
                mActionMode = StartSupportActionMode(mActionModeCallback);
            };

            FindViewById(Resource.Id.search_fragment_container).Click += delegate
            {
                ExitSearchUi();
            };

            gpsFollowButton.Click += async delegate
            {
                await FetchGeoLocation();
            };

            // Location Listener
            mLocListnr = new LocationListener();
            mLocListnr.LocationChanged += async (Location location) =>
            {
                mLocation = location;
                await FetchGeoLocation();
            };

            // Reset focus
            FindViewById(Resource.Id.activity_setup).RequestFocus();

            // Set WUnderground as default API
            apiSpinner.SetSelection(0);

            // Load API key
            if (Settings.API_KEY != null)
            {
                keyEntry.Text = Settings.API_KEY;
            }
        }

        private void EnableControls(bool enable)
        {
            searchViewContainer.Enabled = enable;
            apiSpinner.Enabled = enable;
            keyEntry.Enabled = enable;
            gpsFollowButton.Enabled = enable;
            progressBar.Visibility = enable ? ViewStates.Gone : ViewStates.Visible;
        }

        public async Task FetchGeoLocation()
        {
            gpsFollowButton.Enabled = false;

            if (mLocation != null)
            {
                LocationQueryViewModel view = null;

                // Cancel other tasks
                cts.Cancel();
                cts = new CancellationTokenSource();

                if (cts.IsCancellationRequested)
                {
                    EnableControls(true);
                    return;
                }

                // Show loading bar
                progressBar.Visibility = ViewStates.Visible;

                await Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested)
                    {
                        EnableControls(true);
                        return;
                    }

                    // Get geo location
                    view = await wm.GetLocation(mLocation);

                    if (String.IsNullOrEmpty(view.LocationQuery))
                        view = new LocationQueryViewModel();
                });

                if (String.IsNullOrWhiteSpace(view.LocationQuery))
                {
                    // Stop since there is no valid query
                    EnableControls(true);
                    return;
                }

                if (String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
                {
                    Toast.MakeText(this.ApplicationContext, Resource.String.werror_invalidkey, ToastLength.Short).Show();
                    EnableControls(true);
                    return;
                }

                if (cts.IsCancellationRequested)
                {
                    EnableControls(true);
                    return;
                }

                // Get Weather Data
                var location = new WeatherData.LocationData(view, mLocation);
                WeatherData.Weather weather = await Settings.GetWeatherData(location.query);
                if (weather == null)
                {
                    try
                    {
                        weather = await wm.GetWeather(location);
                    }
                    catch (WeatherException wEx)
                    {
                        weather = null;
                        Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
                    }
                }

                if (weather == null)
                {
                    EnableControls(true);
                    return;
                }

                // We got our data so disable controls just in case
                EnableControls(false);

                // Save weather data
                Settings.SaveLastGPSLocData(location);
                await Settings.DeleteLocations();
                await Settings.AddLocation(new WeatherData.LocationData(view));
                if (wm.SupportsAlerts && weather.weather_alerts != null)
                    await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
                await Settings.SaveWeatherData(weather);

                Settings.FollowGPS = true;
                Settings.WeatherLoaded = true;

                if (mAppWidgetId == AppWidgetManager.InvalidAppwidgetId)
                {
                    // Start WeatherNow Activity with weather data
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.PutExtra("data", location.ToJson());

                    StartActivity(intent);
                    FinishAffinity();
                }
                else
                {
                    // Trigger widget service to update widget
                    WeatherWidgetService.EnqueueWork(this,
                        new Intent(this, typeof(WeatherWidgetService))
                        .SetAction(WeatherWidgetService.ACTION_REFRESHWIDGET)
                        .PutExtra(AppWidgetManager.ExtraAppwidgetIds, new int[] { mAppWidgetId }));

                    // Create return intent
                    Intent resultValue = new Intent();
                    resultValue.PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId);
                    SetResult(Android.App.Result.Ok, resultValue);
                    Finish();
                }
            }
            else
            {
                await UpdateLocation();
            }
        }

        private async Task UpdateLocation()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
                        PERMISSION_LOCATION_REQUEST_CODE);
                return;
            }

            LocationManager locMan = (LocationManager)GetSystemService(Context.LocationService);
            bool isGPSEnabled = locMan.IsProviderEnabled(LocationManager.GpsProvider);
            bool isNetEnabled = locMan.IsProviderEnabled(LocationManager.NetworkProvider);

            Location location = null;

            if (isGPSEnabled)
            {
                location = locMan.GetLastKnownLocation(LocationManager.GpsProvider);

                if (location == null)
                    location = locMan.GetLastKnownLocation(LocationManager.NetworkProvider);

                if (location == null)
                    locMan.RequestSingleUpdate(LocationManager.GpsProvider, mLocListnr, null);
                else
                {
                    mLocation = location;
                    await FetchGeoLocation();
                }
            }
            else if (isNetEnabled)
            {
                location = locMan.GetLastKnownLocation(LocationManager.NetworkProvider);

                if (location == null)
                    locMan.RequestSingleUpdate(LocationManager.NetworkProvider, mLocListnr, null);
                else
                {
                    mLocation = location;
                    await FetchGeoLocation();
                }
            }
            else
            {
                EnableControls(true);
                Toast.MakeText(this, Resource.String.error_retrieve_location, ToastLength.Short).Show();
            }
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case PERMISSION_LOCATION_REQUEST_CODE:
                    {
                        // If request is cancelled, the result arrays are empty.
                        if (grantResults.Length > 0
                                && grantResults[0] == Permission.Granted)
                        {
                            // permission was granted, yay!
                            // Do the task you need to do.
                            await FetchGeoLocation();
                        }
                        else
                        {
                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
                            EnableControls(true);
                            Toast.MakeText(this, Resource.String.error_location_denied, ToastLength.Short).Show();
                        }
                        return;
                    }
            }
        }

        public override void OnAttachFragment(Fragment fragment)
        {
            if (fragment is LocationSearchFragment)
            {
                mSearchFragment = (LocationSearchFragment)fragment;
                SetupSearchUi();
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            // Save ActionMode state
            outState.PutBoolean("SearchUI", inSearchUI);

            if (inSearchUI)
                ExitSearchUi();

            base.OnSaveInstanceState(outState);
        }

        private void EnterSearchUi()
        {
            inSearchUI = true;
            if (mSearchFragment == null)
            {
                AddSearchFragment();
                return;
            }
            mSearchFragment.UserVisibleHint = true;
            FragmentTransaction transaction = SupportFragmentManager
                    .BeginTransaction();
            transaction.Show(mSearchFragment);
            transaction.CommitAllowingStateLoss();
            FragmentManager.ExecutePendingTransactions();
            SetupSearchUi();
        }

        private void SetupSearchUi()
        {
            if (searchView == null)
            {
                PrepareSearchView();
            }
            searchView.RequestFocus();
        }

        private void AddSearchFragment()
        {
            if (mSearchFragment != null)
            {
                return;
            }
            FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
            Fragment searchFragment = new LocationSearchFragment()
            {
                UserVisibleHint = false
            };
            
            // Add AppWidgetId to fragment args
            if (mAppWidgetId != AppWidgetManager.InvalidAppwidgetId)
            {
                Bundle args = new Bundle();
                args.PutInt(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId);
                searchFragment.Arguments = args;
            }

            ft.Add(Resource.Id.search_fragment_container, searchFragment);
            ft.CommitAllowingStateLoss();
        }

        private void PrepareSearchView()
        {
            searchView = searchViewLayout.FindViewById<EditText>(Resource.Id.search_view);
            clearButtonView = searchViewLayout.FindViewById<TextView>(Resource.Id.search_close_button);
            clearButtonView.Click += delegate { searchView.Text = String.Empty; };
            searchView.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                if (mSearchFragment != null)
                {
                    // Cancel pending searches
                    cts.Cancel();
                    cts = new CancellationTokenSource();

                    clearButtonView.Visibility = String.IsNullOrEmpty(e.Text.ToString()) ? ViewStates.Gone : ViewStates.Visible;
                    mSearchFragment.FetchLocations(e.Text.ToString());

                    // If we're using searchfragment
                    // make sure gps feature is off
                    Settings.FollowGPS = false;
                }
            };
            clearButtonView.Visibility = ViewStates.Gone;
            searchView.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                View v = sender as View;

                if (e.HasFocus)
                    ShowInputMethod(v.FindFocus());
                else
                    HideInputMethod(v);
            };
            searchView.EditorAction += (object sender, TextView.EditorActionEventArgs e) =>
            {
                EditText v = sender as EditText;
                if (e.ActionId == ImeAction.Search)
                {
                    if (mSearchFragment != null)
                    {
                        mSearchFragment.FetchLocations(v.Text);
                        HideInputMethod(v);

                        // If we're using searchfragment
                        // make sure gps feature is off
                        Settings.FollowGPS = false;
                    }
                    e.Handled = true;
                }
                e.Handled = false;
            };
        }

        public override void OnBackPressed()
        {
            if (inSearchUI)
            {
                // We should let the user go back to usual screens with tabs.
                ExitSearchUi();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private void ExitSearchUi()
        {
            searchView.Text = String.Empty;

            if (mSearchFragment != null)
            {
                mSearchFragment.UserVisibleHint = false;

                FragmentTransaction transaction = SupportFragmentManager
                        .BeginTransaction();
                transaction.Remove(mSearchFragment);
                mSearchFragment = null;
                transaction.CommitAllowingStateLoss();
            }

            HideInputMethod(CurrentFocus);
            searchView.ClearFocus();
            mActionMode.Finish();
            inSearchUI = false;
        }

        private void ShowInputMethod(View view)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(
                    Context.InputMethodService);
            imm.ToggleSoftInput(0, HideSoftInputFlags.NotAlways);
        }

        private void HideInputMethod(View view)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(
                    Context.InputMethodService);
            if (imm != null && view != null)
            {
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }
    }
}