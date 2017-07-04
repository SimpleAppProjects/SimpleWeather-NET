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
        private ImageView locationButtonView;
        private bool inSearchUI;

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
                mActionMode = StartSupportActionMode(mActionModeCallback);
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
            locationButtonView.Click += delegate { mSearchFragment.FetchGeoLocation(); };
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