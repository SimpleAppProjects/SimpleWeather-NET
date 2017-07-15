
using Android.Content;
using Android.OS;
using Android.Content.Res;
using Android.Support.V7.App;
using Android.Views;
using Android.Support.V4.App;
using Android.Preferences;
using Android.Webkit;
using Android.Util;

namespace SimpleWeather.Droid
{
    [Android.App.Activity(Label = "@string/title_activity_settings", Theme = "@style/SettingsTheme",
        ParentActivity = typeof(MainActivity))]
    public class SettingsActivity : AppCompatPreferenceActivity
    {
        /**
         * Helper method to determine if the device has an extra-large screen. For
         * example, 10" tablets are extra-large.
         */
        private static bool IsXLargeTablet(Context context)
        {
            return (context.Resources.Configuration.ScreenLayout
                    & ScreenLayout.SizeMask) >= ScreenLayout.SizeXlarge;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetupActionBar();

            // Display the fragment as the main content.
            Android.App.Fragment fragment = FragmentManager.FindFragmentById(Android.Resource.Id.Content);

            // Check if fragment exists
            if (fragment == null)
            {
                FragmentManager.BeginTransaction()
                        .Replace(Android.Resource.Id.Content, new SettingsFragment())
                        .Commit();
            }
        }

        /**
         * Set up the {@link android.app.ActionBar}, if the API is available.
         */
        private void SetupActionBar()
        {
            ActionBar actionBar = SupportActionBar;
            if (actionBar != null)
            {
                // Show the Up button in the action bar.
                actionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Android.Resource.Id.Home)
            {
                if (!base.OnMenuItemSelected(featureId, item))
                {
                    Intent intent = NavUtils.GetParentActivityIntent(this);
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    NavUtils.NavigateUpTo(this, intent);
                }
                return true;
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        /**
         * {@inheritDoc}
         */
        public override bool OnIsMultiPane()
        {
            return IsXLargeTablet(this);
        }

        /**
         * This method stops fragment injection in malicious applications.
         * Make sure to deny any unknown fragments here.
         */
        protected override bool IsValidFragment(string fragmentName)
        {
            return Java.Lang.Class.FromType(typeof(PreferenceFragment)).Name.Equals(fragmentName)
                || Java.Lang.Class.FromType(typeof(SettingsFragment)).Name.Equals(fragmentName)
                || Java.Lang.Class.FromType(typeof(AboutAppFragment)).Name.Equals(fragmentName)
                || Java.Lang.Class.FromType(typeof(CreditsFragment)).Name.Equals(fragmentName);
        }

        public class SettingsFragment : PreferenceFragment
        {
            // Preference Keys
            private static string KEY_ABOUTAPP = "key_aboutapp";

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_general);
                SetHasOptionsMenu(false);

                FindPreference(KEY_ABOUTAPP).PreferenceClick += (object sender, Preference.PreferenceClickEventArgs e) =>
                {
                    // Display the fragment as the main content.
                    FragmentManager.BeginTransaction()
                            .Replace(Android.Resource.Id.Content, new AboutAppFragment())
                            .AddToBackStack(null)
                            .Commit();
                };
            }

            public override void OnResume()
            {
                base.OnResume();

                // Title
                AppCompatPreferenceActivity activity = (AppCompatPreferenceActivity)Activity;
                activity.SupportActionBar.Title = GetString(Resource.String.title_activity_settings);
            }
        }

        public class AboutAppFragment : PreferenceFragment
        {
            // Preference Keys
            private static string KEY_ABOUTCREDITS = "key_aboutcredits";
            private static string KEY_ABOUTOSLIBS = "key_aboutoslibs";
            private static string KEY_ABOUTVERSION = "key_aboutversion";

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_aboutapp);
                SetHasOptionsMenu(false);

                FindPreference(KEY_ABOUTCREDITS).PreferenceClick += (object sender, Preference.PreferenceClickEventArgs e) =>
                {
                    // Display the fragment as the main content.
                    FragmentManager.BeginTransaction()
                            .Replace(Android.Resource.Id.Content, new CreditsFragment())
                            .AddToBackStack(null)
                            .Commit();
                };
                FindPreference(KEY_ABOUTOSLIBS).PreferenceClick += (object sender, Preference.PreferenceClickEventArgs e) =>
                {
                    // Display the fragment as the main content.
                    FragmentManager.BeginTransaction()
                            .Replace(Android.Resource.Id.Content, new OSSCreditsFragment())
                            .AddToBackStack(null)
                            .Commit();
                };

                var packageInfo = Context.PackageManager.GetPackageInfo(Context.PackageName, 0);
                FindPreference(KEY_ABOUTVERSION).Summary = string.Format("v{0}", packageInfo.VersionName);
            }

            public override void OnResume()
            {
                base.OnResume();

                // Title
                AppCompatPreferenceActivity activity = (AppCompatPreferenceActivity)Activity;
                activity.SupportActionBar.Title = GetString(Resource.String.pref_title_about);
            }
        }

        public class CreditsFragment : PreferenceFragment
        {
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_credits);
                SetHasOptionsMenu(false);
            }

            public override void OnResume()
            {
                base.OnResume();

                // Title
                AppCompatPreferenceActivity activity = (AppCompatPreferenceActivity)Activity;
                activity.SupportActionBar.Title = GetString(Resource.String.pref_title_credits);
            }
        }

        public class OSSCreditsFragment : PreferenceFragment
        {
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_oslibs);
                SetHasOptionsMenu(false);
            }

            public override void OnResume()
            {
                base.OnResume();

                // Title
                AppCompatPreferenceActivity activity = (AppCompatPreferenceActivity)Activity;
                activity.SupportActionBar.Title = GetString(Resource.String.pref_title_oslibs);
            }
        }

        public class OSSCreditsPreference : Preference
        {
            public OSSCreditsPreference(Context context)
                : base(context)
            {
            }

            public OSSCreditsPreference(Context context, IAttributeSet attrs)
                : base(context, attrs)
            {
            }

            public OSSCreditsPreference(Context context, IAttributeSet attrs, int defStyleAttr)
                : base(context, attrs, defStyleAttr)
            {
            }

            public OSSCreditsPreference(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
                : base(context, attrs, defStyleAttr, defStyleRes)
            {
            }

            protected override void OnBindView(View view)
            {
                base.OnBindView(view);

                WebView webview = view.FindViewById<WebView>(Resource.Id.webview);
                webview.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
                webview.LoadUrl("file:///android_asset/credits/licenses.html");
            }
        }
    }
}