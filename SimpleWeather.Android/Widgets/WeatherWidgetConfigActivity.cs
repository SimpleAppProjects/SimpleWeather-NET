using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimpleWeather.Utils;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SimpleWeather.Controls;
using System.Collections.ObjectModel;
using SimpleWeather.WeatherData;
using Android.Support.V7.App;
using SimpleWeather.Droid.App.Helpers;
using SimpleWeather.Droid.Helpers;
using SimpleWeather.Droid.Adapters;
using SimpleWeather.Droid.App.Controls;
using System.Threading.Tasks;
using Android.Text;
using Android.Views.InputMethods;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Locations;
using System.Threading;

namespace SimpleWeather.Droid.App.Widgets
{
#if DEBUG
    [Activity(Name = "com.thewizrd.simpleweather_debug.WeatherWidgetConfigActivity",
#else
    [Activity(Name = "com.thewizrd.simpleweather.WeatherWidgetConfigActivity",
#endif
        Label = "Widget Preferences", Theme = "@style/AppTheme",
        WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustPan)]
    [IntentFilter(new string[]
    {
        AppWidgetManager.ActionAppwidgetConfigure
    })]
    public class WeatherWidgetConfigActivity : AppCompatActivity, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        // Widget id for ConfigurationActivity
        private int mAppWidgetId = AppWidgetManager.InvalidAppwidgetId;

        // Location Search
        private ReadOnlyCollection<LocationData> Favorites;
        private LocationQueryViewModel query_vm = null;
        private LocationQueryViewModel gpsQuery_vm = null;

        private Android.Locations.Location mLocation;
        private LocationListener mLocListnr;
        private CancellationTokenSource cts;

        private LocationSearchFragment mSearchFragment;
        private Android.Support.V7.View.ActionMode mActionMode;
        private View searchViewLayout;
        private EditText searchView;
        private TextView clearButtonView;
        private ProgressBar progressBar;
        private ActionModeCallback mActionModeCallback = new ActionModeCallback();
        private bool inSearchUI;

        // Weather
        private WeatherManager wm;

        // Views
        private Spinner locSpinner;
        private ArrayAdapter<ComboBoxItem> locAdapter;
        private TextView locSummary;
        private Spinner refreshSpinner;
        private AppBarLayout appBarLayout;
        private CollapsingToolbarLayout collapsingToolbar;
        private ComboBoxItem selectedItem;

        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;
        private const int SETUP_REQUEST_CODE = 10;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the result to CANCELED.  This will cause the widget host to cancel
            // out of the widget placement if they press the back button.
            SetResult(Result.Canceled, new Intent().PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId));

            // Find the widget id from the intent.
            if (Intent != null && Intent.Extras != null)
            {
                mAppWidgetId = Intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId);
            }

            if (mAppWidgetId == AppWidgetManager.InvalidAppwidgetId)
            {
                // If they gave us an intent without the widget id, just bail.
                Finish();
            }

            SetContentView(Resource.Layout.activity_widget_setup);

            wm = WeatherManager.GetInstance();

            // Setup location spinner
            locSpinner = FindViewById<Spinner>(Resource.Id.location_pref_spinner);
            locSummary = FindViewById<TextView>(Resource.Id.location_pref_summary);

            var comboList = new List<ComboBoxItem>()
            {
                new ComboBoxItem(GetString(Resource.String.pref_item_gpslocation), "GPS"),
                new ComboBoxItem(GetString(Resource.String.label_btn_add_location), "Search")
            };
            var favs = await Settings.GetFavorites();
            Favorites = new ReadOnlyCollection<LocationData>(favs);
            foreach (LocationData location in Favorites)
            {
                comboList.Insert(comboList.Count - 1, new ComboBoxItem(location.name, location.query));
            }

            locAdapter = new ArrayAdapter<ComboBoxItem>(
                this, Android.Resource.Layout.SimpleSpinnerItem,
                comboList);
            locAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            locAdapter.SetNotifyOnChange(true);
            locSpinner.Adapter = locAdapter;

            FindViewById(Resource.Id.location_pref).Click += (object sender, EventArgs e) =>
            {
                locSpinner.PerformClick();
            };
            locSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                CtsCancel();

                if (locSpinner.SelectedItem is ComboBoxItem item)
                {
                    locSummary.Text = item.Display;

                    if (item.Value.Equals("Search"))
                        mActionMode = StartSupportActionMode(mActionModeCallback);
                    else
                        selectedItem = item;
                }
                else
                    selectedItem = null;
                query_vm = null;
            };
            locSpinner.SetSelection(0);

            // Setup interval spinner
            refreshSpinner = FindViewById<Spinner>(Resource.Id.interval_pref_spinner);
            var refreshSummary = FindViewById<TextView>(Resource.Id.interval_pref_summary);
            var refreshList = new List<ComboBoxItem>();
            var refreshEntries = this.Resources.GetStringArray(Resource.Array.refreshinterval_entries);
            var refreshValues = this.Resources.GetStringArray(Resource.Array.refreshinterval_values);
            for (int i = 0; i < refreshEntries.Length; i++)
            {
                refreshList.Add(new ComboBoxItem(refreshEntries[i], refreshValues[i]));
            }
            var refreshAdapter = new ArrayAdapter<ComboBoxItem>(
                this, Android.Resource.Layout.SimpleSpinnerItem,
                refreshList);
            refreshAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            refreshSpinner.Adapter = refreshAdapter;
            FindViewById(Resource.Id.interval_pref).Click += (object sender, EventArgs e) =>
            {
                refreshSpinner.PerformClick();
            };
            refreshSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                if (refreshSpinner.SelectedItem is ComboBoxItem item)
                {
                    refreshSummary.Text = item.Display;
                }
            };
            refreshSpinner.SetSelection(
                refreshList.FindIndex(item => item.Value.Equals(Settings.RefreshInterval.ToString())));

            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);

            mActionModeCallback.CreateActionMode += OnCreateActionMode;
            mActionModeCallback.DestroyActionMode += OnDestroyActionMode;

            FindViewById(Resource.Id.search_fragment_container).Click += delegate
            {
                ExitSearchUi();
            };

            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.app_bar);
            collapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);

            cts = new CancellationTokenSource();

            // Location Listener
            mLocListnr = new LocationListener();
            mLocListnr.LocationChanged += async (Android.Locations.Location location) =>
            {
                mLocation = location;
                await FetchGeoLocation();
            };

            if (!Settings.WeatherLoaded)
            {
                Toast.MakeText(this, GetString(Resource.String.prompt_setup_app_first), ToastLength.Short).Show();

                Intent intent = new Intent(this, typeof(SetupActivity))
                    .SetAction(AppWidgetManager.ActionAppwidgetConfigure)
                    .PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId);
                StartActivityForResult(intent, SETUP_REQUEST_CODE);
            }
        }

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

        private void EnterSearchUi()
        {
            inSearchUI = true;

            // Unset scroll flag
            var toolbarParams = collapsingToolbar.LayoutParameters as AppBarLayout.LayoutParams;
            toolbarParams.ScrollFlags &= ~AppBarLayout.LayoutParams.ScrollFlagScroll;

            if (mSearchFragment == null)
            {
                AddSearchFragment();
                return;
            }
            mSearchFragment.UserVisibleHint = true;
            var transaction = SupportFragmentManager
                    .BeginTransaction();
            transaction.Show(mSearchFragment);
            transaction.CommitAllowingStateLoss();
            SupportFragmentManager.ExecutePendingTransactions();
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


        private void ShowLoading(bool show)
        {
            if (mSearchFragment == null)
                return;

            progressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;

            if (show || (!show && String.IsNullOrEmpty(searchView.Text)))
                clearButtonView.Visibility = ViewStates.Gone;
            else
                clearButtonView.Visibility = ViewStates.Visible;
        }

        public override void OnAttachFragment(Android.Support.V4.App.Fragment fragment)
        {
            if (fragment is LocationSearchFragment)
            {
                mSearchFragment = (LocationSearchFragment)fragment;
                SetupSearchUi();
            }
        }

        private void AddSearchFragment()
        {
            if (mSearchFragment != null)
                return;

            var ft = SupportFragmentManager.BeginTransaction();
            LocationSearchFragment searchFragment = new LocationSearchFragment();
            searchFragment.SetClickListener((object sender, RecyclerClickEventArgs e) =>
            {
                LocationQueryAdapter adapter = sender as LocationQueryAdapter;
                LocationQuery v = (LocationQuery)e.View;

                if (!String.IsNullOrEmpty(adapter.Dataset[e.Position].LocationQuery))
                    query_vm = adapter.Dataset[e.Position];
                else
                    query_vm = null;

                if (String.IsNullOrWhiteSpace(query_vm.LocationQuery))
                {
                    // Stop since there is no valid query
                    query_vm = null;
                    return;
                }

                // Cancel other tasks
                mSearchFragment.CtsCancel();

                ShowLoading(true);

                if (mSearchFragment.CtsCancelRequested())
                {
                    ShowLoading(false);
                    query_vm = null;
                    return;
                }

                // Check if location already exists
                if (Favorites.FirstOrDefault(l => l.query == query_vm.LocationQuery) is LocationData loc)
                {
                    ShowLoading(false);
                    ExitSearchUi();

                    // Set selection
                    query_vm = null;
                    locSpinner.SetSelection(
                        locAdapter.GetPosition(new ComboBoxItem(loc.name, loc.query)));
                    return;
                }

                if (mSearchFragment.CtsCancelRequested())
                {
                    ShowLoading(false);
                    query_vm = null;
                    return;
                }

                // We got our data so disable controls just in case
                adapter.Dataset.Clear();
                adapter.NotifyDataSetChanged();
                searchFragment.View.FindViewById<RecyclerView>(Resource.Id.recycler_view).Enabled = false;

                // Save data
                var item = new ComboBoxItem(query_vm.LocationName, query_vm.LocationQuery);
                var idx = locAdapter.Count - 1;
                locAdapter.Insert(item, idx);
                locSpinner.SetSelection(idx);
                locSummary.Text = item.Display;

                // Hide dialog
                ShowLoading(false);
                ExitSearchUi();
            });
            searchFragment.UserVisibleHint = false;
            ft.Add(Resource.Id.search_fragment_container, searchFragment);
            ft.CommitAllowingStateLoss();
        }

        private void PrepareSearchView()
        {
            searchView = searchViewLayout.FindViewById<EditText>(Resource.Id.search_view);
            clearButtonView = searchViewLayout.FindViewById<TextView>(Resource.Id.search_close_button);
            progressBar = searchViewLayout.FindViewById<ProgressBar>(Resource.Id.search_progressBar);
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

        private void ExitSearchUi()
        {
            searchView.Text = String.Empty;

            if (mSearchFragment != null)
            {
                mSearchFragment.UserVisibleHint = false;

                var transaction = SupportFragmentManager
                        .BeginTransaction();
                transaction.Remove(mSearchFragment);
                mSearchFragment = null;
                transaction.CommitAllowingStateLoss();
            }

            HideInputMethod(CurrentFocus);
            searchView?.ClearFocus();
            mActionMode?.Finish();
            //locSpinner?.Parent?.RequestFocus();
            inSearchUI = false;

            // Reset to last selected item
            if (query_vm == null && selectedItem != null)
                locSpinner.SetSelection(locAdapter.GetPosition(selectedItem));

            // Set scroll flag
            var toolbarParams = collapsingToolbar.LayoutParameters as AppBarLayout.LayoutParams;
            toolbarParams.ScrollFlags &= AppBarLayout.LayoutParams.ScrollFlagScroll;
        }

        private void ShowInputMethod(View view)
        {
            InputMethodManager imm = GetSystemService(
                    InputMethodService) as InputMethodManager;
            imm?.ToggleSoftInput(0, HideSoftInputFlags.NotAlways);
        }

        private void HideInputMethod(View view)
        {
            InputMethodManager imm = GetSystemService(
                    InputMethodService) as InputMethodManager;
            if (imm != null && view != null)
            {
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == SETUP_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    // Get result data
                    var dataJson = data?.GetStringExtra("data");

                    if (!String.IsNullOrWhiteSpace(dataJson))
                    {
                        var locData = LocationData.FromJson(
                            new Newtonsoft.Json.JsonTextReader(
                                new System.IO.StringReader(dataJson)));

                        if (locData.locationType == LocationType.Search)
                        {
                            // Add location to adapter and select it
                            Favorites = new ReadOnlyCollection<LocationData>(Favorites.Append(locData).ToList());
                            var item = new ComboBoxItem(locData.name, locData.query);
                            var idx = locAdapter.Count - 1;
                            locAdapter.Insert(item, idx);
                            locSpinner.SetSelection(idx);
                        }
                        else
                        {
                            // GPS; set to first selection
                            locSpinner.SetSelection(0);
                        }
                    }
                }
            }
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
                SetResult(Result.Canceled, new Intent().PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId));
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Inflate the menu; this adds items to the action bar if it is present.
            menu.Clear();
            MenuInflater.Inflate(Resource.Menu.menu_widgetsetup, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                // Respond to the action bar's Up/Home button
                case Android.Resource.Id.Home:
                    if (inSearchUI)
                    {
                        // We should let the user go back to usual screens with tabs.
                        ExitSearchUi();
                    }
                    else
                    {
                        SetResult(Result.Canceled, new Intent().PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId));
                        Finish();
                    }
                    return true;
                case Resource.Id.action_done:
                    PrepareWidget();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private async void PrepareWidget()
        {
            // Update Settings
            if (refreshSpinner.SelectedItem is ComboBoxItem refreshItem)
            {
                if (int.TryParse(refreshItem.Value, out int refreshValue))
                    Settings.RefreshInterval = refreshValue;
            }

            // Get location data
            if (locSpinner.SelectedItem is ComboBoxItem locationItem)
            {
                LocationData locData = null;

                switch (locationItem.Value)
                {
                    case "GPS":
                        Settings.FollowGPS = true;

                        if (gpsQuery_vm == null || mLocation == null)
                        {
                            await FetchGeoLocation();
                        }
                        else
                        {
                            locData = new LocationData(gpsQuery_vm, mLocation);
                            Settings.SaveLastGPSLocData(locData);

                            // Save locdata for widget
                            WidgetUtils.SaveLocationData(mAppWidgetId, locData);
                            WidgetUtils.AddWidgetId(locData.query, mAppWidgetId);
                        }
                        break;
                    default:
                        // Get location data
                        if (locSpinner.SelectedItem is ComboBoxItem item)
                        {
                            locData = Favorites.FirstOrDefault(loc => loc.query.Equals(item.Value));

                            if (locData == null && query_vm != null)
                            {
                                locData = new LocationData(query_vm);

                                // Add location to favs
                                await Settings.AddLocation(locData);
                            }
                            else if (locData == null)
                            {
                                SetResult(Result.Canceled, new Intent().PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId));
                                Finish();
                                return;
                            }

                            // Save locdata for widget
                            WidgetUtils.SaveLocationData(mAppWidgetId, locData);
                            WidgetUtils.AddWidgetId(locData.query, mAppWidgetId);
                        }
                        break;
                }

                // Trigger widget service to update widget
                WeatherWidgetService.EnqueueWork(this,
                    new Intent(this, typeof(WeatherWidgetService))
                    .SetAction(WeatherWidgetService.ACTION_REFRESHWIDGET)
                    .PutExtra(WeatherWidgetProvider.EXTRA_WIDGET_IDS, new int[] { mAppWidgetId }));

                // Create return intent
                Intent resultValue = new Intent();
                resultValue.PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId);
                SetResult(Result.Ok, resultValue);
                Finish();
            }
            else
            {
                SetResult(Result.Canceled, new Intent().PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId));
                Finish();
            }
        }

        private void CtsCancel()
        {
            cts.Cancel();
            cts = new CancellationTokenSource();
        }

        private bool CtsCancelRequested()
        {
            return (bool)cts?.IsCancellationRequested;
        }

        private async Task FetchGeoLocation()
        {
            if (cts.IsCancellationRequested)
            {
                return;
            }

            if (mLocation != null)
            {
                LocationQueryViewModel view = null;

                // Cancel other tasks
                CtsCancel();

                if (cts.IsCancellationRequested)
                {
                    return;
                }

                await Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested)
                    {
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
                    return;
                }

                if (CtsCancelRequested())
                {
                    return;
                }

                // Set gps location data
                gpsQuery_vm = view;

                // We got our location data, so setup the widget
                PrepareWidget();
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

            Android.Locations.Location location = null;

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
                            Toast.MakeText(this, Resource.String.error_location_denied, ToastLength.Short).Show();
                        }
                        return;
                    }
            }
        }
    }
}