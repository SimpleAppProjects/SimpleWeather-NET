using System;
using Android.Content;
using Android.OS;
using Android.Content.Res;
using Android.Support.V7.App;
using Android.Views;
using Android.Support.V4.App;
using Android.Preferences;
using Android.Webkit;
using Android.Util;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Runtime;
using Android.Widget;

namespace SimpleWeather.Droid
{
    [Android.App.Activity(Label = "@string/title_activity_settings", Theme = "@style/SettingsTheme")]
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
                    base.OnBackPressed();
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
            private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

            // Preference Keys
            private static string KEY_ABOUTAPP = "key_aboutapp";
            private static string KEY_FOLLOWGPS = "key_followgps";

            // Preferences
            private SwitchPreference followGps;

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

                followGps = (SwitchPreference)FindPreference(KEY_FOLLOWGPS);
                followGps.PreferenceChange += (object sender, Preference.PreferenceChangeEventArgs e) =>
                {
                    SwitchPreference pref = e.Preference as SwitchPreference;
                    if ((bool)e.NewValue)
                    {
                        if (ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                            ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                        {
                            RequestPermissions(new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
                                    PERMISSION_LOCATION_REQUEST_CODE);
                            return;
                        }
                        else
                        {
                            App.Preferences.Edit().PutBoolean("HomeChanged", true).Apply();
                        }
                    }
                };
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
                                App.Preferences.Edit().PutBoolean("HomeChanged", true).Apply();
                            }
                            else
                            {
                                // permission denied, boo! Disable the
                                // functionality that depends on this permission.
                                Toast.MakeText(Activity, "Location access denied", ToastLength.Short).Show();
                                followGps.Checked = false;
                                Console.WriteLine(SimpleWeather.Utils.Settings.FollowGPS);
                            }
                            return;
                        }
                }
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

                var packageInfo = Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0);
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