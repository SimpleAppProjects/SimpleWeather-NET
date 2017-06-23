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
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Graphics;

namespace SimpleWeather.Droid
{
    [Android.App.Activity(Name = "SimpleWeather.Droid.SetupActivity",
        Theme = "@style/SetupTheme", WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustResize)]
    public class SetupActivity : AppCompatActivity
    {
        private LocationSearchFragment mSearchFragment;
        private Spinner apiSpinner;
        private EditText keyEntry;
        private EditText searchView;
        private ImageView backButtonView;
        private ImageView clearButtonView;
        private ImageView locationButtonView;
        private bool inSearchUI;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_setup);

            // Setup Actionbar
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.Elevation = 0;

            apiSpinner = (Spinner)FindViewById(Resource.Id.api_spinner);
            keyEntry = (EditText)FindViewById(Resource.Id.key_entry);

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
                EnterSearchUi();
            };

            FindViewById(Resource.Id.search_fragment_container).Click += delegate
            {
                ExitSearchUi();
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

        public override void OnAttachFragment(Fragment fragment)
        {
            if ((fragment as LocationSearchFragment) != null)
            {
                mSearchFragment = (LocationSearchFragment)fragment;
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
            mSearchFragment.Activity.Window.SetSoftInputMode(SoftInput.AdjustPan | SoftInput.AdjustResize);
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
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetBackgroundDrawable(null);
            SupportActionBar.Elevation = 5;
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
            View searchViewLayout = LayoutInflater.Inflate(
                    Resource.Layout.search_action_bar, null);
            backButtonView = (ImageView)searchViewLayout
                    .FindViewById(Resource.Id.search_back_button);
            backButtonView.Click += delegate
            {
                ExitSearchUi();
            };
            searchView = (EditText)searchViewLayout
                    .FindViewById(Resource.Id.search_view);
            clearButtonView = (ImageView)searchViewLayout.FindViewById(Resource.Id.search_close_button);
            locationButtonView = (ImageView)searchViewLayout
                    .FindViewById(Resource.Id.search_location_button);
            clearButtonView.Click += delegate
            {
                searchView.Text = String.Empty;
            };

            searchView.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                if (mSearchFragment != null)
                {
                    clearButtonView.Visibility = String.IsNullOrEmpty(e.Text.ToString()) ? ViewStates.Gone : ViewStates.Visible;
                    mSearchFragment.FetchLocations(e.Text.ToString());
                }
            };
            clearButtonView.Visibility = ViewStates.Gone;
            searchView.FocusChange += (object s, View.FocusChangeEventArgs e) =>
            {
                View v = s as View;
                if (e.HasFocus)
                {
                    ShowInputMethod(v.FindFocus());
                }
                else
                {
                    HideInputMethod(v);
                }
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
            locationButtonView.Click += delegate
            {
                mSearchFragment.FetchGeoLocation();
            };

            SupportActionBar.SetCustomView(
                    searchViewLayout,
                    new ActionBar.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent));
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
            if (mSearchFragment != null)
            {
                mSearchFragment.UserVisibleHint = false;

                FragmentTransaction transaction = SupportFragmentManager
                        .BeginTransaction();
                transaction.Remove(mSearchFragment);
                mSearchFragment = null;
                transaction.CommitAllowingStateLoss();
            }

            // We want to hide SearchView and show Tabs. Also focus on previously
            // selected one.
            SupportActionBar.SetDisplayShowCustomEnabled(false);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(new Color(ContextCompat.GetColor(this, Resource.Color.colorPrimary))));
            SupportActionBar.Elevation = 0;
            HideInputMethod(CurrentFocus);
            InvalidateOptionsMenu();
            searchView.ClearFocus();
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