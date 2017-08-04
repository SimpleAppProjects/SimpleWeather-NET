using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using SimpleWeather.WeatherData;
using SimpleWeather.Droid.Controls;
using SimpleWeather.Utils;
using Android.Support.V7.App;
using Android.Views.InputMethods;
using Android.Text;
using Android.Views;
using System.Collections.Specialized;
using SimpleWeather.Droid.Utils;
using SimpleWeather.Droid.Helpers;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget.Helper;
using System.Threading.Tasks;

namespace SimpleWeather.Droid
{
    public class LocationsFragment : Fragment, IWeatherLoadedListener
    {
        private bool Loaded = false;
        private bool EditMode = false;
        private bool DataChanged = false;

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
        private ImageView clearButtonView;
        private bool inSearchUI;
        private String selected_query = string.Empty;

        // GPS Location
        View gpsPanelLayout;
        LocationPanel gpsPanel;
        LocationPanelViewModel gpsPanelViewModel;

        private ActionModeCallback mActionModeCallback = new ActionModeCallback();

        // OptionsMenu
        private IMenu optionsMenu;

        public LocationsFragment()
        {
            // Required empty public constructor
            mActionModeCallback.CreateActionMode += OnCreateActionMode;
            mActionModeCallback.DestroyActionMode += OnDestroyActionMode;
        }

        public void OnWeatherLoaded(int locationIdx, Weather weather)
        {
            if (weather != null)
            {
                if (locationIdx == App.HomeIdx && Settings.FollowGPS)
                {
                    gpsPanelViewModel.SetWeather(weather);
                    gpsPanel.SetWeather(gpsPanelViewModel);
                }
                else
                {
                    // Update panel weather
                    int index = Settings.FollowGPS ? locationIdx - 1 : locationIdx;
                    LocationPanelViewModel panel = mAdapter.Dataset[index];
                    panel.SetWeather(weather);
                    mAdapter.NotifyItemChanged(index);
                }
            }
        }

        private bool OnCreateActionMode(Android.Support.V7.View.ActionMode mode, IMenu menu)
        {
            if (searchViewLayout == null)
                searchViewLayout = Activity.LayoutInflater.Inflate(Resource.Layout.search_action_bar, null);

            mode.CustomView = searchViewLayout;
            EnterSearchUi();
            return true;
        }

        private void OnDestroyActionMode(Android.Support.V7.View.ActionMode mode)
        {
            ExitSearchUi();
            mActionMode = null;
        }

        private void OnPanelClick(object sender, EventArgs e)
        {
            View view = sender as View;
            if (view == null && e != null)
                view = (e as RecyclerClickEventArgs).View;

            if (view.Enabled)
            {
                LocationPanel v = (LocationPanel)view;
                Pair<int, string> pair = (Pair<int, string>)view.Tag;

                Fragment fragment = WeatherNowFragment.NewInstance(pair);
                // Navigate to WeatherNowFragment
                if (pair.Key == App.HomeIdx)
                {
                    AppCompatActivity.SupportFragmentManager.BeginTransaction().Replace(
                        Resource.Id.fragment_container, fragment).Commit();

                    // Pop all since we're going home
                    AppCompatActivity.SupportFragmentManager.PopBackStack(null, (int)Android.App.PopBackStackFlags.Inclusive);
                }
                else
                {
                    AppCompatActivity.SupportFragmentManager.BeginTransaction().Add(
                        Resource.Id.fragment_container, fragment).Hide(this).AddToBackStack(null).Commit();
                }
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            AppCompatActivity = Activity as AppCompatActivity;
            Loaded = true;

            // Get ActionMode state
            if (savedInstanceState != null && savedInstanceState.GetBoolean("SearchUI", false))
            {
                inSearchUI = true;

                // Restart ActionMode
                mActionMode = AppCompatActivity.StartSupportActionMode(mActionModeCallback);
            }
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
            mLayoutManager = new LinearLayoutManager(Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // specify an adapter (see also next example)
            mAdapter = new LocationPanelAdapter(new List<LocationPanelViewModel>());
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

            IMenuItem editMenuBtn = optionsMenu.FindItem(Resource.Id.action_editmode);
            if (editMenuBtn != null)
                editMenuBtn.SetVisible(!(!Settings.FollowGPS && mAdapter.ItemCount == 1 || Settings.FollowGPS && mAdapter.ItemCount == 0));
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

        public override async void OnResume()
        {
            base.OnResume();

            // Don't resume if fragment is hidden
            if (this.IsHidden)
                return;

            // Update view on resume
            // ex. If temperature unit changed
            if (Settings.FollowGPS)
            {
                gpsPanelLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                gpsPanelViewModel = null;
                gpsPanelLayout.Visibility = ViewStates.Gone;
            }

            if (!Settings.FollowGPS && mAdapter.ItemCount == 0 ||
                Settings.FollowGPS && gpsPanelViewModel == null)
            {
                // New instance; Get locations and load up weather data
                await LoadLocations();
            }
            else if (!Loaded)
            {
                // Refresh view
                RefreshLocations();
                Loaded = true;
            }

            // Title
            AppCompatActivity.SupportActionBar.Title = GetString(Resource.String.label_nav_locations);
        }

        public override void OnPause()
        {
            base.OnPause();
            Loaded = false;
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
            WeatherDataLoader wLoader = null;

            List<String> locations = await Settings.GetLocations();
            mAdapter.RemoveAll();

            foreach (String location in locations)
            {
                int index = locations.IndexOf(location);

                LocationPanelViewModel panel = new LocationPanelViewModel()
                {
                    Pair = new Pair<int, string>(index, location)
                };

                // Set home
                if (index == App.HomeIdx && !Settings.FollowGPS)
                    panel.IsHome = true;
                else
                    panel.IsHome = false;

                if (index == App.HomeIdx && Settings.FollowGPS)
                {
                    panel.IsHome = true;
                    gpsPanelViewModel = panel;
                }
                else
                    mAdapter.Add(Settings.FollowGPS ? index - 1 : index, panel);

                wLoader = new WeatherDataLoader(this, location, index);
                await wLoader.LoadWeatherData(false);
            }
        }

        private async void RefreshLocations()
        {
            // Reload all panels if needed
            List<string> locations = await Settings.GetLocations();
            bool reload = !Settings.FollowGPS && locations.Count != mAdapter.ItemCount ||
                Settings.FollowGPS && (gpsPanelViewModel == null || locations.Count - 1 != mAdapter.ItemCount);

            // Reload if weather source differs
            if ((gpsPanelViewModel != null && gpsPanelViewModel.WeatherSource != Settings.API) ||
                (mAdapter.ItemCount >= 1 && mAdapter.Dataset[0].WeatherSource != Settings.API))
                reload = true;

            // Reload if panel queries dont match
            if (!reload && (gpsPanelViewModel != null && locations[App.HomeIdx] != gpsPanelViewModel.Pair.Value))
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
                    dataset.Append(gpsPanelViewModel);

                foreach (LocationPanelViewModel view in dataset)
                {
                    WeatherDataLoader wLoader =
                        new WeatherDataLoader(this, view.Pair.Value, view.Pair.Key);
                    await wLoader.LoadWeatherData(false);
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
                int index = 0;

                if (!String.IsNullOrEmpty(adapter.Dataset[e.Position].LocationQuery))
                    selected_query = adapter.Dataset[e.Position].LocationQuery;
                else
                    selected_query = string.Empty;

                if (String.IsNullOrWhiteSpace(selected_query))
                {
                    // Stop since there is no valid query
                    return;
                }

                // Show loading dialog
                LoadingDialog progDialog = new LoadingDialog(Activity);
                progDialog.Show();

                OrderedDictionary weatherData = await Settings.GetWeatherData();

                index = weatherData.Keys.Count;

                // Check if location already exists
                if (weatherData.Contains(selected_query))
                {
                    // Hide dialog
                    progDialog.Dismiss();
                    ExitSearchUi();
                    return;
                }

                Weather weather = null;

                weather = await WeatherLoaderTask.GetWeather(selected_query);

                if (weather == null)
                {
                    // Hide dialog
                    progDialog.Dismiss();
                    return;
                }

                // Save coords to List
                weatherData.Add(selected_query, weather);

                // Save data
                Settings.SaveWeatherData();

                LocationPanelViewModel panel = new LocationPanelViewModel(weather)
                {
                    Pair = new Pair<int, string>(index, selected_query)
                };

                // Set properties if necessary
                if (EditMode)
                    panel.EditMode = true;

                mAdapter.Add(Settings.FollowGPS ? index - 1 : index, panel);

                // Hide dialog
                progDialog.Dismiss();
                ExitSearchUi();
            });
            searchFragment.UserVisibleHint = false;
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

            HideInputMethod(Activity.CurrentFocus);
            searchView.ClearFocus();
            mActionMode.Finish();
            mMainView.RequestFocus();
            inSearchUI = false;
        }

        private void ShowInputMethod(View view)
        {
            InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(
                    Context.InputMethodService);
            imm.ToggleSoftInput(0, HideSoftInputFlags.NotAlways);
        }

        private void HideInputMethod(View view)
        {
            InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(
                    Context.InputMethodService);
            if (imm != null && view != null)
            {
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }

        private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (LocationPanelViewModel panelView in mAdapter.Dataset)
            {
                int index = mAdapter.Dataset.IndexOf(panelView);

                if (Settings.FollowGPS) index++;

                panelView.Pair = new Pair<int, string>(index, panelView.Pair.Value);
            }

            // Flag that data has changed
            if (EditMode && (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Move))
                DataChanged = true;

            // Cancel edit Mode
            // TODO: cleanup code; it looks ugly
            if (EditMode && (!Settings.FollowGPS && mAdapter.ItemCount == 1 || Settings.FollowGPS && mAdapter.ItemCount == 0))
                ToggleEditMode();

            // Disable EditMode if only single location
            if (optionsMenu != null)
            {
                IMenuItem editMenuBtn = optionsMenu.FindItem(Resource.Id.action_editmode);
                if (editMenuBtn != null)
                    editMenuBtn.SetVisible(!(!Settings.FollowGPS && mAdapter.ItemCount == 1 || Settings.FollowGPS && mAdapter.ItemCount == 0));
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
                mAdapter.ItemClick -= OnPanelClick;
                mAdapter.ItemLongClick -= OnPanelLongClick;
            }
            else
            {
                // Register events
                mAdapter.ItemClick += OnPanelClick;
                mAdapter.ItemLongClick += OnPanelLongClick;
            }

            foreach (LocationPanelViewModel view in mAdapter.Dataset)
            {
                view.EditMode = EditMode;
                mAdapter.NotifyItemChanged(mAdapter.Dataset.IndexOf(view));
            }

            if (!EditMode && DataChanged) Settings.SaveWeatherData();
            DataChanged = false;
        }
    }
}