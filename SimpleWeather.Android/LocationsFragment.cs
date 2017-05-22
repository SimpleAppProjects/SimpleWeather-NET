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

namespace SimpleWeather.Droid
{
    public class LocationsFragment : Fragment, WeatherLoadedListener
    {
        private Context context;

        // Views
        private LocationPanel HomePanel;
        private RecyclerView mRecyclerView;
        private LocationPanelAdapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;
        private Button addLocationsButton;

        // Search
        private LocationSearchFragment mSearchFragment;
        private Android.Support.V7.View.ActionMode mActionMode;
        private View searchViewLayout;
        private EditText searchView;
        private ImageView backButtonView;
        private ImageView clearButtonView;
        private ImageView locationButtonView;
        private bool inSearchUI;
        private String selected_query = string.Empty;

        private ActionModeCallback mActionModeCallback = new ActionModeCallback();

        public LocationsFragment()
        {
            // Required empty public constructor
            mActionModeCallback.CreateActionMode += onCreateActionMode;
            mActionModeCallback.DestroyActionMode += onDestroyActionMode;
        }

        public void onWeatherLoaded(int locationIdx, Weather weather)
        {
            if (weather != null)
            {
                // Home Panel
                if (locationIdx == App.HomeIdx)
                {
                    // TODO: make lpv local
                    // OR Make HomePanel a recyclerview
                    HomePanel.SetWeather(new LocationPanelView(weather) { Pair = (Pair<int, string>)HomePanel.Tag});
                }
                // Others
                else
                {
                    LocationPanelView panel = mAdapter.Dataset[locationIdx - 1];
                    panel.setWeather(weather);
                    mAdapter.NotifyDataSetChanged();
                }
            }
        }

        private bool onCreateActionMode(Android.Support.V7.View.ActionMode mode, IMenu menu)
        {
            if (searchViewLayout == null)
            {
                searchViewLayout = Activity.LayoutInflater.Inflate(Resource.Layout.search_action_bar, null);
                searchViewLayout.SetPadding(0, 0, 16, 0); // l, t, r, b
            }
            mode.CustomView = searchViewLayout;
            enterSearchUi();
            return true;
        }

        private void onDestroyActionMode(Android.Support.V7.View.ActionMode mode)
        {
            exitSearchUi();
            mActionMode = null;
        }

        private void onPanelClick(object sender, EventArgs e)
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
                AppCompatActivity activity = (AppCompatActivity)Activity;
                if (pair.Key == App.HomeIdx)
                {
                    activity.SupportFragmentManager.BeginTransaction().Replace(
                        Resource.Id.fragment_container, fragment).Commit();

                    // Pop all since we're going home
                    activity.SupportFragmentManager.PopBackStack(null, (int)Android.App.PopBackStackFlags.Inclusive);
                }
                else
                {
                    activity.SupportFragmentManager.BeginTransaction().Add(
                        Resource.Id.fragment_container, fragment).AddToBackStack(null).Commit();
                }
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            context = Activity.ApplicationContext;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.fragment_locations, container, false);
            view.FindViewById(Resource.Id.search_fragment_container).Click += delegate
            {
                exitSearchUi();
            };

            HomePanel = (LocationPanel)view.FindViewById(Resource.Id.home_panel);
            HomePanel.ContextMenuCreated += (object sender, View.CreateContextMenuEventArgs e) =>
            {
                View v = sender as View;
                e.Menu.Add(Menu.None, v.Id, 0, "Change Favorite Location");
            };
            HomePanel.Click += onPanelClick;

            // Other Locations
            mRecyclerView = (RecyclerView)view.FindViewById(Resource.Id.other_location_container);

            addLocationsButton = (Button)view.FindViewById(Resource.Id.other_location_add);
            addLocationsButton.Click += delegate
            {
                AppCompatActivity activity = (AppCompatActivity)Activity;
                mActionMode = activity.StartSupportActionMode(mActionModeCallback);
            };

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            mRecyclerView.HasFixedSize = false;

            // use a linear layout manager
            mLayoutManager = new LinearLayoutManager(Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // specify an adapter (see also next example)
            mAdapter = new LocationPanelAdapter(new List<LocationPanelView>());
            mAdapter.ItemClick += onPanelClick;
            mRecyclerView.SetAdapter(mAdapter);

            view.Post(() =>
            {
                LoadLocations();
            });

            return view;
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            // Home Panel
            if (item.ItemId == Resource.Id.home_panel)
            {
                AppCompatActivity activity = (AppCompatActivity)Activity;
                mActionMode = activity.StartSupportActionMode(mActionModeCallback);
                mActionMode.Tag = Resource.Id.home_panel;
            }
            else if (item.TitleFormatted.ToString() == "Delete Location")
            {
                // Remove location from list
                OrderedDictionary weatherData = Settings.getWeatherData().ConfigureAwait(false).GetAwaiter().GetResult();
                weatherData.RemoveAt(item.ItemId - 1);
                Settings.saveWeatherData(weatherData);

                // Remove panel
                mAdapter.Remove(item.ItemId);
            }
            return base.OnContextItemSelected(item);
        }

        public override void OnResume()
        {
            base.OnResume();

            // Update view on resume
            // ex. If temperature unit changed
            // TODO: do this

            // Title
            AppCompatActivity activity = (AppCompatActivity)Activity;
            activity.SupportActionBar.Title = GetString(Resource.String.label_nav_locations);
        }

        private async void LoadLocations()
        {
            WeatherDataLoader wLoader = null;

            List<String> locations = await Settings.getLocations();

            foreach (String location in locations)
            {
                int index = locations.IndexOf(location);

                if (index == App.HomeIdx)
                {
                    // Nothing
                    HomePanel.Tag = new Pair<int, string>(index, location);
                }
                else
                {
                    LocationPanelView panel = new LocationPanelView();
                    panel.Pair = new Pair<int, string>(index, location);
                    mAdapter.Add(index - 1, panel);
                }

                wLoader = new WeatherDataLoader(this, location, index);
                await wLoader.loadWeatherData(false);
            }
        }

        public override void OnAttachFragment(Fragment childFragment)
        {
            if ((childFragment as LocationSearchFragment) != null)
            {
                mSearchFragment = (LocationSearchFragment)childFragment;
                setupSearchUi();
            }
        }

        private void enterSearchUi()
        {
            inSearchUI = true;
            if (mSearchFragment == null)
            {
                addSearchFragment();
                return;
            }
            mSearchFragment.UserVisibleHint = true;
            mSearchFragment.Activity.Window.SetSoftInputMode(SoftInput.AdjustPan | SoftInput.AdjustResize);
            FragmentTransaction transaction = ChildFragmentManager
                    .BeginTransaction();
            transaction.Show(mSearchFragment);
            transaction.CommitAllowingStateLoss();
            ChildFragmentManager.ExecutePendingTransactions();
            setupSearchUi();
        }

        private void setupSearchUi()
        {
            if (searchView == null)
            {
                prepareSearchView();
            }
            searchView.RequestFocus();
        }

        private void addSearchFragment()
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

                OrderedDictionary weatherData = await Settings.getWeatherData();

                if (mActionMode.Tag != null && (int)mActionMode.Tag == Resource.Id.home_panel)
                    index = App.HomeIdx;
                else
                    index = weatherData.Keys.Count;

                // Check if location already exists
                if (index == App.HomeIdx)
                {
                    if (weatherData.Keys.Cast<string>().First().Equals(selected_query))
                    {
                        exitSearchUi();
                        return;
                    }
                }
                else if (weatherData.Contains(selected_query))
                {
                    exitSearchUi();
                    return;
                }

                Weather weather = null;

                weather = await WeatherLoaderTask.getWeather(selected_query);

                if (weather == null)
                    return;

                // Save coords to List
                if (mActionMode.Tag != null && (int)mActionMode.Tag == Resource.Id.home_panel)
                {
                    weatherData.RemoveAt(0);
                    weatherData.Insert(0, selected_query, weather);
                }
                else
                {
                    weatherData.Add(selected_query, weather);
                }
                // Save data
                Settings.saveWeatherData(weatherData);

                if (index == App.HomeIdx)
                {
                    // ProgressBar
                    HomePanel.ShowLoading(true);
                    HomePanel.SetWeather(new LocationPanelView(weather) { Pair = new Pair<int, string>(index, selected_query) });
                }
                else
                {
                    // (TODO:) NOTE: panel number could be wrong since we're adding
                    LocationPanelView panel = new LocationPanelView(weather);
                    panel.Pair = new Pair<int, string>(index, selected_query);
                    mAdapter.Add(index - 1, panel);
                }

                exitSearchUi();
            });
            searchFragment.UserVisibleHint = false;
            ft.Add(Resource.Id.search_fragment_container, searchFragment);
            ft.CommitAllowingStateLoss();
        }

        private void prepareSearchView()
        {
            backButtonView = (ImageView)searchViewLayout
                    .FindViewById(Resource.Id.search_back_button);
            backButtonView.Visibility = ViewStates.Gone;
            searchView = (EditText)searchViewLayout
                    .FindViewById(Resource.Id.search_view);
            clearButtonView = (ImageView)searchViewLayout.FindViewById(Resource.Id.search_close_button);
            locationButtonView = (ImageView)searchViewLayout
                    .FindViewById(Resource.Id.search_location_button);
            clearButtonView.Click += delegate { searchView.Text = String.Empty; };
            searchView.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                if (mSearchFragment != null)
                {
                    clearButtonView.Visibility = String.IsNullOrEmpty(e.Text.ToString()) ? ViewStates.Gone : ViewStates.Visible;
                    mSearchFragment.fetchLocations(e.Text.ToString());
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
                View v = sender as View;

                if (e.ActionId == ImeAction.Search)
                {
                    if (mSearchFragment != null)
                    {
                        mSearchFragment.fetchLocations(selected_query);
                        HideInputMethod(v);
                    }
                    e.Handled = true;
                }
                e.Handled = false;
            };
            locationButtonView.Click += delegate { mSearchFragment.fetchGeoLocation(); };
        }

        private void exitSearchUi()
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

        internal class ActionModeCallback : Java.Lang.Object, Android.Support.V7.View.ActionMode.ICallback
        {
            public Func<Android.Support.V7.View.ActionMode, IMenu, bool> CreateActionMode;
            public Action<Android.Support.V7.View.ActionMode> DestroyActionMode;

            // Called when the action mode is created; startActionMode() was called
            public bool OnCreateActionMode(Android.Support.V7.View.ActionMode mode, IMenu menu)
            {
                if (CreateActionMode != null)
                    return CreateActionMode.Invoke(mode, menu);
                else
                    return false;
            }

            // Called each time the action mode is shown. Always called after onCreateActionMode, but
            // may be called multiple times if the mode is invalidated.
            public bool OnPrepareActionMode(Android.Support.V7.View.ActionMode mode, IMenu menu)
            {
                return false; // Return false if nothing is done
            }

            // Called when the user selects a contextual menu item
            public bool OnActionItemClicked(Android.Support.V7.View.ActionMode mode, IMenuItem item)
            {
                return false; // Return false if nothing is done
            }

            // Called when the user exits the action mode
            public void OnDestroyActionMode(Android.Support.V7.View.ActionMode mode)
            {
                if (DestroyActionMode != null)
                    DestroyActionMode.Invoke(mode);
            }
        }
    }
}