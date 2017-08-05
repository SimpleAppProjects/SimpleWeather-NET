using SimpleWeather.Utils;
using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views.InputMethods;
using Android.Content;
using Android.Support.V4.App;
using SimpleWeather.Droid.Helpers;
using Android.Support.V4.Content;
using Android.Locations;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using Android;
using Android.Content.PM;
using Android.Runtime;
using SimpleWeather.Droid.Controls;
using SimpleWeather.Droid.Utils;
using System.Collections.Specialized;

namespace SimpleWeather.Droid
{
    [Android.App.Activity(Theme = "@style/SetupTheme",
        WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustPan)]
    public class SetupActivity : AppCompatActivity
    {
        private LocationSearchFragment mSearchFragment;
        private Android.Support.V7.View.ActionMode mActionMode;
        private View searchViewLayout;
        private Spinner apiSpinner;
        private EditText keyEntry;
        private EditText searchView;
        private ImageView clearButtonView;
        private bool inSearchUI;

        private Button gpsFollowButton;
        private Location mLocation;
        private LocationListener mLocListnr;
        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

        private ActionModeCallback mActionModeCallback = new ActionModeCallback();

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
            SetContentView(Resource.Layout.activity_setup);

            mActionModeCallback.CreateActionMode += OnCreateActionMode;
            mActionModeCallback.DestroyActionMode += OnDestroyActionMode;

            // Get ActionMode state
            if (savedInstanceState != null && savedInstanceState.GetBoolean("SearchUI", false))
            {
                inSearchUI = true;

                // Restart ActionMode
                mActionMode = StartSupportActionMode(mActionModeCallback);
            }

            apiSpinner = FindViewById<Spinner>(Resource.Id.api_spinner);
            keyEntry = FindViewById<EditText>(Resource.Id.key_entry);
            gpsFollowButton = FindViewById<Button>(Resource.Id.gps_follow);

            /* Event Listeners */
            apiSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                if (e.Position == 0)
                {
                    Settings.API = Settings.API_WUnderground;
                    FindViewById(Resource.Id.key_entry_box).Visibility = ViewStates.Visible;
                }
                else
                {
                    Settings.API = Settings.API_Yahoo;
                    FindViewById(Resource.Id.key_entry_box).Visibility = ViewStates.Invisible;
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

            FindViewById(Resource.Id.search_view_container).Click += delegate
            {
                mActionMode = StartSupportActionMode(mActionModeCallback);
            };

            FindViewById(Resource.Id.search_fragment_container).Click += delegate
            {
                ExitSearchUi();
            };

            gpsFollowButton.Click += delegate
            {
                FetchGeoLocation();
            };

            // Location Listener
            mLocListnr = new LocationListener();
            mLocListnr.LocationChanged += (Location location) =>
            {
                mLocation = location;
                FetchGeoLocation();
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

        public async void FetchGeoLocation()
        {
            if (mLocation != null)
            {
                String selected_query = String.Empty;
                
                // Show loading dialog
                LoadingDialog progDialog = new LoadingDialog(this);
                progDialog.Show();

                await Task.Run(async () =>
                {
                     // Get geo location
                     LocationQueryViewModel view = await WeatherData.GeopositionQuery.GetLocation(mLocation);

                     if (!String.IsNullOrEmpty(view.LocationQuery))
                         selected_query = view.LocationQuery;
                     else
                         selected_query = string.Empty;
                });

                if (String.IsNullOrWhiteSpace(selected_query))
                {
                    // Stop since there is no valid query
                    progDialog.Dismiss();
                    return;
                }

                if (String.IsNullOrWhiteSpace(Settings.API_KEY) && Settings.API == Settings.API_WUnderground)
                {
                    Toast.MakeText(this.ApplicationContext, Resource.String.werror_invalidkey, ToastLength.Short).Show();
                    // Hide dialog
                    progDialog.Dismiss();
                    return;
                }

                Pair<int, string> pair;

                // Get Weather Data
                OrderedDictionary weatherData = await Settings.GetWeatherData();

                WeatherData.Weather weather = await WeatherData.WeatherLoaderTask.GetWeather(selected_query);

                if (weather == null)
                {
                    // Hide dialog
                    progDialog.Dismiss();
                    return;
                }

                // Save weather data
                if (weatherData.Contains(selected_query))
                    weatherData[selected_query] = weather;
                else
                    weatherData.Add(selected_query, weather);
                Settings.SaveWeatherData();

                pair = new Pair<int, string>(App.HomeIdx, selected_query);

                // Start WeatherNow Activity with weather data
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("pair", await JSONParser.SerializerAsync(pair, typeof(Pair<int, string>)));

                Settings.FollowGPS = true;
                Settings.WeatherLoaded = true;
                // Hide dialog
                progDialog.Dismiss();

                StartActivity(intent);
                FinishAffinity();
            }
            else
            {
                UpdateLocation();
            }
        }

        private void UpdateLocation()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                RequestPermissions(new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
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
                    FetchGeoLocation();
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
                    FetchGeoLocation();
                }
            }
            else
            {
                Toast.MakeText(this, Resource.String.error_retrieve_location, ToastLength.Short).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
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
                            FetchGeoLocation();
                        }
                        else
                        {
                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
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
            ft.Add(Resource.Id.search_fragment_container, searchFragment);
            ft.CommitAllowingStateLoss();
        }

        private void PrepareSearchView()
        {
            searchView = searchViewLayout.FindViewById<EditText>(Resource.Id.search_view);
            clearButtonView = searchViewLayout.FindViewById<ImageView>(Resource.Id.search_close_button);
            clearButtonView.Click += delegate { searchView.Text = String.Empty; };
            searchView.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                if (mSearchFragment != null)
                {
                    clearButtonView.Visibility = String.IsNullOrEmpty(e.Text.ToString()) ? ViewStates.Gone : ViewStates.Visible;
                    mSearchFragment.FetchLocations(e.Text.ToString());
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