using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using SimpleWeather.WeatherData;
using SimpleWeather.Droid.App.Controls;
using SimpleWeather.Utils;
using Android.Support.V7.App;
using Android.Views.InputMethods;
using Android.Text;
using Android.Views;
using System.Collections.Specialized;
using SimpleWeather.Droid.Utils;
using SimpleWeather.Droid.App.Helpers;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget.Helper;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using Com.Bumptech.Glide;
using Android;
using Android.Content.PM;
using Android.Runtime;
using Android.Locations;
using SimpleWeather.Controls;
using SimpleWeather.Droid.App.Widgets;
using SimpleWeather.Droid.Helpers;
using SimpleWeather.Droid.Adapters;
using SimpleWeather.Droid.App.Adapters;

namespace SimpleWeather.Droid.App
{
    public class LocationsFragment : Fragment, 
        IWeatherLoadedListener, IWeatherErrorListener,
        ActivityCompat.IOnRequestPermissionsResultCallback
    {
        private bool Loaded = false;
        private bool EditMode = false;
        private bool DataChanged = false;
        private bool HomeChanged = false;
        private bool[] ErrorCounter;

        AppCompatActivity AppCompatActivity;

        // Views
        private View mMainView;
        private RecyclerView mRecyclerView;
        private LocationPanelAdapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;
        private ItemTouchHelperCallback mITHCallback;
        private FloatingActionButton addLocationsButton;

        // Search
        private LocationSearchFragment mSearchFragment;
        private Android.Support.V7.View.ActionMode mActionMode;
        private View searchViewLayout;
        private EditText searchView;
        private TextView clearButtonView;
        private ProgressBar progressBar;
        private bool inSearchUI;

        // GPS Location
        View gpsPanelLayout;
        LocationPanel gpsPanel;
        LocationPanelViewModel gpsPanelViewModel;
        private LocationListener mLocListnr;
        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

        private ActionModeCallback mActionModeCallback = new ActionModeCallback();

        // OptionsMenu
        private IMenu optionsMenu;

        private WeatherManager wm;

        public LocationsFragment()
        {
            // Required empty public constructor
            mActionModeCallback.CreateActionMode += OnCreateActionMode;
            mActionModeCallback.DestroyActionMode += OnDestroyActionMode;

            wm = WeatherManager.GetInstance();
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            AppCompatActivity = context as AppCompatActivity;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            AppCompatActivity = null;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            AppCompatActivity = null;
        }

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            if (weather != null)
            {
                if (Settings.FollowGPS && location.locationType == LocationType.GPS)
                {
                    gpsPanelViewModel.SetWeather(weather);
                    AppCompatActivity?.RunOnUiThread(() => 
                    {
                        gpsPanel.SetWeatherBackground(gpsPanelViewModel);
                        gpsPanel.SetWeather(gpsPanelViewModel);
                    });
                }
                else
                {
                    // Update panel weather
                    LocationPanelViewModel panel = mAdapter.Dataset.First(panelVM => panelVM.LocationData.query.Equals(location.query));
                    // Just in case
                    if (panel == null)
                    {
                        panel = mAdapter.Dataset.First(panelVM => panelVM.LocationData.name.Equals(location.name) &&
                                                        panelVM.LocationData.latitude.Equals(location.latitude) &&
                                                        panelVM.LocationData.longitude.Equals(location.longitude) &&
                                                        panelVM.LocationData.tz_long.Equals(location.tz_long));
                    }
                    panel.SetWeather(weather);
                    AppCompatActivity?.RunOnUiThread(() => mAdapter.NotifyItemChanged(mAdapter.Dataset.IndexOf(panel)));
                }
            }
        }

        public void OnWeatherError(WeatherException wEx)
        {
            switch (wEx.ErrorStatus)
            {
                case WeatherUtils.ErrorStatus.NetworkError:
                case WeatherUtils.ErrorStatus.NoWeather:
                    // Show error message and prompt to refresh
                    // Only warn once
                    if (!ErrorCounter[(int)wEx.ErrorStatus])
                    {
                        Snackbar snackBar = Snackbar.Make(mMainView, wEx.Message, Snackbar.LengthLong);
                        snackBar.SetAction(Resource.String.action_retry, async (View v) =>
                        {
                            await RefreshLocations();
                        });
                        snackBar.Show();
                        ErrorCounter[(int)wEx.ErrorStatus] = true;
                    }
                    break;
                default:
                    // Show error message
                    // Only warn once
                    if (!ErrorCounter[(int)wEx.ErrorStatus])
                    {
                        Snackbar.Make(mMainView, wEx.Message, Snackbar.LengthLong).Show();
                        ErrorCounter[(int)wEx.ErrorStatus] = true;
                    }
                    break;
            }
        }

        private bool OnCreateActionMode(Android.Support.V7.View.ActionMode mode, IMenu menu)
        {
            if (searchViewLayout == null)
                searchViewLayout = AppCompatActivity.LayoutInflater.Inflate(Resource.Layout.search_action_bar, null);

            mode.CustomView = searchViewLayout;
            EnterSearchUi();
            // Hide FAB in actionmode
            if (addLocationsButton != null)
                addLocationsButton.Visibility = ViewStates.Gone;
            return true;
        }

        private void OnDestroyActionMode(Android.Support.V7.View.ActionMode mode)
        {
            ExitSearchUi();
            if (addLocationsButton != null)
                addLocationsButton.Visibility = ViewStates.Visible;
            mActionMode = null;
        }

        private void OnPanelClick(object sender, EventArgs e)
        {
            View view = sender as View;
            if (view == null && e != null)
                view = (e as RecyclerClickEventArgs).View;

            if (view != null && view.Enabled)
            {
                var locData = (LocationData)view.Tag;

                if (locData.Equals(Settings.HomeData))
                {
                    // Pop all since we're going home
                    AppCompatActivity.SupportFragmentManager.PopBackStack(null, (int)Android.App.PopBackStackFlags.Inclusive);
                }
                else
                {
                    // Navigate to WeatherNowFragment
                    Fragment fragment = WeatherNowFragment.NewInstance(locData);
                    AppCompatActivity.SupportFragmentManager.BeginTransaction()
                            .Add(Resource.Id.fragment_container, fragment, null)
                            .Hide(this)
                            .AddToBackStack(null)
                            .Commit();
                }
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            Loaded = true;

            // Get ActionMode state
            if (savedInstanceState != null && savedInstanceState.GetBoolean("SearchUI", false))
            {
                inSearchUI = true;

                // Restart ActionMode
                mActionMode = AppCompatActivity.StartSupportActionMode(mActionModeCallback);
            }

            int max = Enum.GetValues(typeof(WeatherUtils.ErrorStatus)).Cast<int>().Max();
            ErrorCounter = new bool[max];

            mLocListnr = new LocationListener();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.fragment_locations, container, false);
            mMainView = view;
            view.FindViewById(Resource.Id.search_fragment_container).Click += delegate
            {
                ExitSearchUi();
            };
            view.FocusableInTouchMode = true;
            view.RequestFocus();
            view.KeyPress += (sender, e) =>
            {
                if (e.KeyCode == Keycode.Back && EditMode)
                {
                    ToggleEditMode();
                    e.Handled = true;
                }
                else
                    e.Handled = false;
            };

            // Setup ActionBar
            HasOptionsMenu = true;

            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.locations_container);

            addLocationsButton = view.FindViewById<FloatingActionButton>(Resource.Id.locations_add);
            addLocationsButton.Click += delegate
            {
                mActionMode = AppCompatActivity.StartSupportActionMode(mActionModeCallback);
            };

            gpsPanelLayout = view.FindViewById(Resource.Id.gps_follow_layout);
            gpsPanel = view.FindViewById<LocationPanel>(Resource.Id.gps_panel);
            gpsPanel.Click += OnPanelClick;

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            mRecyclerView.HasFixedSize = false;

            // use a linear layout manager
            mLayoutManager = new LinearLayoutManager(AppCompatActivity);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // specify an adapter (see also next example)
            mAdapter = new LocationPanelAdapter(Glide.With(this), new List<LocationPanelViewModel>());
            mAdapter.ItemClick += OnPanelClick;
            mAdapter.ItemLongClick += OnPanelLongClick;
            mAdapter.CollectionChanged += LocationPanels_CollectionChanged;
            mRecyclerView.SetAdapter(mAdapter);
            new ItemTouchHelper(mITHCallback = new ItemTouchHelperCallback(mAdapter))
                .AttachToRecyclerView(mRecyclerView);

            // Turn off by default
            mITHCallback.SetLongPressDragEnabled(false);
            mITHCallback.SetItemViewSwipeEnabled(false);

            Loaded = true;

            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            // Inflate the menu; this adds items to the action bar if it is present.
            optionsMenu = menu;
            menu.Clear();
            inflater.Inflate(Resource.Menu.locations, menu);

            bool onlyHomeIsLeft = (mAdapter.ItemCount == 1);
            IMenuItem editMenuBtn = optionsMenu.FindItem(Resource.Id.action_editmode);
            if (editMenuBtn != null)
                editMenuBtn.SetVisible(!onlyHomeIsLeft);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Handle action bar item clicks here. The action bar will
            // automatically handle clicks on the Home/Up button, so long
            // as you specify a parent AppCompatActivity in AndroidManifest.xml.
            int id = item.ItemId;

            //noinspection SimplifiableIfStatement
            if (id == Resource.Id.action_editmode)
            {
                ToggleEditMode();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private async Task Resume()
        {
            // Update view on resume
            // ex. If temperature unit changed
            AppCompatActivity?.SupportActionBar.SetBackgroundDrawable(
                new ColorDrawable(new Color(ContextCompat.GetColor(AppCompatActivity, Resource.Color.colorPrimary))));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                AppCompatActivity?.Window.SetStatusBarColor(
                    new Color(ContextCompat.GetColor(AppCompatActivity, Resource.Color.colorPrimaryDark)));
            }

            if (Settings.FollowGPS)
            {
                gpsPanelLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                gpsPanelViewModel = null;
                gpsPanelLayout.Visibility = ViewStates.Gone;
            }

            if (mAdapter.ItemCount == 0 || Settings.FollowGPS && gpsPanelViewModel == null)
            {
                // New instance; Get locations and load up weather data
                await LoadLocations();
            }
            else if (!Loaded)
            {
                // Refresh view
                await RefreshLocations();
                Loaded = true;
            }
        }

        public override async void OnResume()
        {
            base.OnResume();

            // Don't resume if fragment is hidden
            if (this.IsHidden)
                return;
            else
                await Resume();

            // Title
            if (AppCompatActivity != null)
                AppCompatActivity.SupportActionBar.Title = GetString(Resource.String.label_nav_locations);
        }

        public override void OnPause()
        {
            base.OnPause();
            Loaded = false;

            // Reset error counter
            Array.Clear(ErrorCounter, 0, ErrorCounter.Length);
        }

        public override async void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (!hidden && this.IsVisible)
            {
                await Resume();
            }
            else if (hidden)
            {
                Loaded = false;
                // Reset error counter
                Array.Clear(ErrorCounter, 0, ErrorCounter.Length);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            // Save ActionMode state
            outState.PutBoolean("SearchUI", inSearchUI);

            if (inSearchUI)
                ExitSearchUi();

            base.OnSaveInstanceState(outState);
        }

        private async Task LoadLocations()
        {
            // Load up saved locations
            var locations = await Settings.GetFavorites();
            mAdapter.RemoveAll();

            // Setup saved favorite locations
            await LoadGPSPanel();
            foreach (LocationData location in locations)
            {
                LocationPanelViewModel panel = new LocationPanelViewModel()
                {
                    LocationData = location
                };

                mAdapter.Add(panel);
            }

            foreach (LocationData location in locations)
            {
                await Task.Run(async () =>
                {
                    var wLoader = new WeatherDataLoader(this, this, location);
                    await wLoader.LoadWeatherData(false);
                });
            }
        }

        private async Task LoadGPSPanel()
        {
            // Setup gps panel
            if (Settings.FollowGPS)
            {
                gpsPanelLayout.Visibility = ViewStates.Visible;
                var locData = await Settings.GetLastGPSLocData();

                if (locData == null || locData.query == null)
                {
                    locData = await UpdateLocation();
                }

                if (locData != null && locData.query != null)
                {
                    LocationPanelViewModel panel = new LocationPanelViewModel()
                    {
                        LocationData = locData
                    };
                    gpsPanelViewModel = panel;

                    await Task.Run(async () =>
                    {
                        var wLoader = new WeatherDataLoader(this, this, locData);
                        await wLoader.LoadWeatherData(false);
                    });
                }
            }
        }

        private async Task RefreshLocations()
        {
            // Reload all panels if needed
            var locations = await Settings.GetLocationData();
            var homeData = await Settings.GetLastGPSLocData();
            bool reload = (locations.Count != mAdapter.ItemCount || Settings.FollowGPS && gpsPanelViewModel == null);

            // Reload if weather source differs
            if ((gpsPanelViewModel != null && gpsPanelViewModel.WeatherSource != Settings.API) ||
                (mAdapter.ItemCount >= 1 && mAdapter.Dataset[0].WeatherSource != Settings.API))
                reload = true;

            // Reload if panel queries dont match
            if (!reload && (gpsPanelViewModel != null && homeData.query != gpsPanelViewModel.LocationData.query))
                reload = true;

            if (reload)
            {
                mAdapter.RemoveAll();
                await LoadLocations();
            }
            else
            {
                List<LocationPanelViewModel> dataset = mAdapter.Dataset;
                if (gpsPanelViewModel != null)
                    dataset.Add(gpsPanelViewModel);

                foreach (LocationPanelViewModel view in dataset)
                {
                    await Task.Run(async () =>
                    {
                        var wLoader =
                            new WeatherDataLoader(this, this, view.LocationData);
                        await wLoader.LoadWeatherData(false);
                    });
                }
            }
        }

        private async Task<LocationData> UpdateLocation()
        {
            LocationData locationData = null;

            if (Settings.FollowGPS)
            {
                if (AppCompatActivity != null && ContextCompat.CheckSelfPermission(AppCompatActivity, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                    ContextCompat.CheckSelfPermission(AppCompatActivity, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(AppCompatActivity, new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
                            PERMISSION_LOCATION_REQUEST_CODE);
                    return null;
                }

                LocationManager locMan = (LocationManager)AppCompatActivity.GetSystemService(Context.LocationService);
                bool isGPSEnabled = locMan.IsProviderEnabled(LocationManager.GpsProvider);
                bool isNetEnabled = locMan.IsProviderEnabled(LocationManager.NetworkProvider);

                Android.Locations.Location location = null;

                if (isGPSEnabled || isNetEnabled)
                {
                    Criteria locCriteria = new Criteria() { Accuracy = Accuracy.Coarse, CostAllowed = false, PowerRequirement = Power.Low };
                    string provider = locMan.GetBestProvider(locCriteria, true);
                    location = locMan.GetLastKnownLocation(provider);

                    if (location == null)
                        locMan.RequestSingleUpdate(provider, mLocListnr, null);
                    else
                    {
                        LocationQueryViewModel view = null;

                        await Task.Run(async () =>
                        {
                            view = await wm.GetLocation(location);

                            if (String.IsNullOrEmpty(view.LocationQuery))
                                view = new LocationQueryViewModel();
                        });

                        if (String.IsNullOrWhiteSpace(view.LocationQuery))
                        {
                            // Stop since there is no valid query
                            gpsPanelViewModel = null;
                            gpsPanelLayout.Visibility = ViewStates.Gone;
                            return null;
                        }

                        // Save location as last known
                        locationData = new LocationData(view, location);
                    }
                }
                else
                {
                    Toast.MakeText(AppCompatActivity, Resource.String.error_retrieve_location, ToastLength.Short).Show();
                    gpsPanelViewModel = null;
                    gpsPanelLayout.Visibility = ViewStates.Gone;
                }
            }

            return locationData;
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
                            var locData = await UpdateLocation();
                            if (locData != null)
                            {
                                Settings.SaveLastGPSLocData(locData);
                                await LoadGPSPanel();

                                App.Context.StartService(
                                    new Intent(App.Context, typeof(WearableDataListenerService))
                                        .SetAction(WearableDataListenerService.ACTION_SENDLOCATIONUPDATE));
                            }
                            else
                            {
                                gpsPanelViewModel = null;
                                gpsPanelLayout.Visibility = ViewStates.Gone;
                            }
                        }
                        else
                        {
                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
                            Settings.FollowGPS = false;
                            gpsPanelViewModel = null;
                            gpsPanelLayout.Visibility = ViewStates.Gone;
                            Toast.MakeText(AppCompatActivity, Resource.String.error_location_denied, ToastLength.Short).Show();
                        }
                        return;
                    }
            }
        }

        public override void OnAttachFragment(Fragment childFragment)
        {
            if (childFragment is LocationSearchFragment)
            {
                mSearchFragment = childFragment as LocationSearchFragment;

                if (inSearchUI)
                    SetupSearchUi();
            }
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
            FragmentTransaction transaction = ChildFragmentManager
                    .BeginTransaction();
            transaction.Show(mSearchFragment);
            transaction.CommitAllowingStateLoss();
            ChildFragmentManager.ExecutePendingTransactions();
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

        private void AddSearchFragment()
        {
            if (mSearchFragment != null)
                return;

            FragmentTransaction ft = ChildFragmentManager.BeginTransaction();
            LocationSearchFragment searchFragment = new LocationSearchFragment();
            searchFragment.SetClickListener(async (object sender, RecyclerClickEventArgs e) =>
            {
                LocationQueryAdapter adapter = sender as LocationQueryAdapter;
                LocationQuery v = (LocationQuery)e.View;
                LocationQueryViewModel query_vm = null;

                if (!String.IsNullOrEmpty(adapter.Dataset[e.Position].LocationQuery))
                    query_vm = adapter.Dataset[e.Position];
                else
                    query_vm = new LocationQueryViewModel();

                if (String.IsNullOrWhiteSpace(query_vm.LocationQuery))
                {
                    // Stop since there is no valid query
                    return;
                }

                // Cancel other tasks
                mSearchFragment.CtsCancel();

                ShowLoading(true);

                if (mSearchFragment.CtsCancelRequested())
                {
                    ShowLoading(false);
                    return;
                }

                // Check if location already exists
                var locData = await Settings.GetLocationData();
                if (locData.Exists(l => l.query == query_vm.LocationQuery))
                {
                    ShowLoading(false);
                    ExitSearchUi();
                    return;
                }

                if (mSearchFragment.CtsCancelRequested())
                {
                    ShowLoading(false);
                    return;
                }

                var location = new LocationData(query_vm);
                Weather weather = await Settings.GetWeatherData(location.query);
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
                    ShowLoading(false);
                    return;
                }

                // We got our data so disable controls just in case
                mAdapter.Dataset.Clear();
                mAdapter.NotifyDataSetChanged();
                searchFragment.View.FindViewById<RecyclerView>(Resource.Id.recycler_view).Enabled = false;

                // Save data
                await Settings.AddLocation(location);
                if (wm.SupportsAlerts && weather.weather_alerts != null)
                    await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
                await Settings.SaveWeatherData(weather);

                LocationPanelViewModel panel = new LocationPanelViewModel(weather)
                {
                    LocationData = location
                };

                // Set properties if necessary
                if (EditMode) panel.EditMode = true;

                int index = mAdapter.Dataset.Count;
                mAdapter.Add(panel);

                // Update shortcuts
                await Task.Run(() => Shortcuts.ShortcutCreator.UpdateShortcuts());

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

                FragmentTransaction transaction = ChildFragmentManager
                        .BeginTransaction();
                transaction.Remove(mSearchFragment);
                mSearchFragment = null;
                transaction.CommitAllowingStateLoss();
            }

            HideInputMethod(AppCompatActivity?.CurrentFocus);
            searchView?.ClearFocus();
            mActionMode?.Finish();
            mMainView?.RequestFocus();
            inSearchUI = false;
        }

        private void ShowInputMethod(View view)
        {
            InputMethodManager imm = AppCompatActivity?.GetSystemService(
                    Context.InputMethodService) as InputMethodManager;
            imm?.ToggleSoftInput(0, HideSoftInputFlags.NotAlways);
        }

        private void HideInputMethod(View view)
        {
            InputMethodManager imm = AppCompatActivity.GetSystemService(
                    Context.InputMethodService) as InputMethodManager;
            if (imm != null && view != null)
            {
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }

        private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool dataMoved = (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Move);
            bool onlyHomeIsLeft = (mAdapter.ItemCount == 1);

            // Flag that data has changed
            if (EditMode && dataMoved)
                DataChanged = true;

            if (EditMode && (e.NewStartingIndex == App.HomeIdx || e.OldStartingIndex == App.HomeIdx))
                HomeChanged = true;

            // Cancel edit Mode
            if (EditMode && onlyHomeIsLeft)
                ToggleEditMode();

            // Disable EditMode if only single location
            if (optionsMenu != null)
            {
                IMenuItem editMenuBtn = optionsMenu.FindItem(Resource.Id.action_editmode);
                if (editMenuBtn != null)
                    editMenuBtn.SetVisible(!onlyHomeIsLeft);
            }
        }

        private void OnPanelLongClick(object sender, RecyclerClickEventArgs e)
        {
            if (!EditMode && mAdapter.ItemCount > 1) ToggleEditMode();
        }

        private void ToggleEditMode()
        {
            // Toggle EditMode
            EditMode = !EditMode;

            IMenuItem editMenuBtn = optionsMenu.FindItem(Resource.Id.action_editmode);
            // Change EditMode button drwble
            editMenuBtn.SetIcon(EditMode ? Resource.Drawable.ic_done_white_24dp : Resource.Drawable.ic_mode_edit_white_24dp);
            // Change EditMode button label
            editMenuBtn.SetTitle(EditMode ? GetString(Resource.String.abc_action_mode_done) : GetString(Resource.String.action_editmode));

            // Set Drag & Swipe ability
            mITHCallback.SetLongPressDragEnabled(EditMode);
            mITHCallback.SetItemViewSwipeEnabled(EditMode);

            if (EditMode)
            {
                // Unregister events
                gpsPanel.Click -= OnPanelClick;
                mAdapter.ItemClick -= OnPanelClick;
                mAdapter.ItemLongClick -= OnPanelLongClick;
            }
            else
            {
                // Register events
                gpsPanel.Click += OnPanelClick;
                mAdapter.ItemClick += OnPanelClick;
                mAdapter.ItemLongClick += OnPanelLongClick;
            }

            foreach (LocationPanelViewModel view in mAdapter.Dataset)
            {
                view.EditMode = EditMode;
                mAdapter.NotifyItemChanged(mAdapter.Dataset.IndexOf(view));

                if (!EditMode && DataChanged)
                {
                    string query = view.LocationData.query;
                    int pos = mAdapter.Dataset.IndexOf(view);
                    Task.Factory.StartNew(() => Settings.MoveLocation(query, pos));
                }
            }

            if (!EditMode && HomeChanged)
            {
                WeatherWidgetService.EnqueueWork(AppCompatActivity, new Intent(AppCompatActivity, typeof(WeatherWidgetService))
                        .SetAction(WeatherWidgetService.ACTION_UPDATEWEATHER));

                App.Context.StartService(
                    new Intent(App.Context, typeof(WearableDataListenerService))
                        .SetAction(WearableDataListenerService.ACTION_SENDLOCATIONUPDATE));
                App.Context.StartService(
                    new Intent(App.Context, typeof(WearableDataListenerService))
                        .SetAction(WearableDataListenerService.ACTION_SENDWEATHERUPDATE));
            }

            DataChanged = false;
            HomeChanged = false;
        }
    }
}