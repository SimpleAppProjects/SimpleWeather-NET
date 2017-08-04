using System;
using System.Linq;
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
using SimpleWeather.Utils;
using Android.Text.Style;
using Android.Graphics;
using Android.Text;

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
                    OnBackPressed();
                }
                return true;
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        public override void OnBackPressed()
        {
            string KEY_API = "API";

            if (FragmentManager.FindFragmentById(Android.Resource.Id.Content) is SettingsFragment fragment)
            {
                ListPreference keyPref = fragment.FindPreference(KEY_API) as ListPreference;
                if (String.IsNullOrWhiteSpace(Settings.API_KEY) && keyPref.Value == Settings.API_WUnderground)
                {
                    // Set keyentrypref color to red
                    Toast.MakeText(this, "Please enter an API Key", ToastLength.Long).Show();
                    return;
                }
            }

            base.OnBackPressed();
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
            private const string KEY_ABOUTAPP = "key_aboutapp";
            private const string KEY_FOLLOWGPS = "key_followgps";
            private const string KEY_API = "API";
            private const string KEY_APIKEY = "API_KEY";
            private const string KEY_APIKEY_VERIFIED = "API_KEY_VERIFIED";
            private const string KEY_HOMECHANGED = "HomeChanged";

            // Preferences
            private SwitchPreference followGps;
            private ListPreference providerPref;
            private KeyEntryPreference keyEntry;
            private ISharedPreferences wuSharedPrefs;
            private bool keyVerified { get { return IsKeyVerfied(); } set { SetKeyVerified(value); } }

            private bool IsKeyVerfied()
            {
                return wuSharedPrefs.GetBoolean(KEY_APIKEY_VERIFIED, false);
            }

            private void SetKeyVerified(bool value)
            {
                wuSharedPrefs.Edit().PutBoolean(KEY_APIKEY_VERIFIED, value).Apply();
            }

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_general);
                SetHasOptionsMenu(false);
                wuSharedPrefs = Activity.GetSharedPreferences(Settings.API_WUnderground, FileCreationMode.Private);

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
                            App.Preferences.Edit().PutBoolean(KEY_HOMECHANGED, true).Apply();
                        }
                    }
                };

                keyEntry = (KeyEntryPreference)FindPreference(KEY_APIKEY);
                keyEntry.PositiveButtonClick += async delegate
                {
                    String key = keyEntry.EditText.Text;

                    if (!String.IsNullOrWhiteSpace(key))
                    {
                        if (await WeatherUnderground.KeyCheckQuery.IsValid(key))
                        {
                            Settings.API_KEY = key;
                            Settings.API = Settings.API_WUnderground;

                            keyVerified = true;
                            UpdateKeySummary();

                            keyEntry.Dialog.Dismiss();
                        }
                    }
                    else
                        Toast.MakeText(this.Activity, "Please enter an API Key", ToastLength.Short).Show();
                };

                providerPref = (ListPreference)FindPreference(KEY_API);
                providerPref.Summary = providerPref.Entry;
                providerPref.Persistent = false;
                providerPref.PreferenceChange += (object sender, Preference.PreferenceChangeEventArgs e) => 
                {
                    ListPreference pref = e.Preference as ListPreference;

                    if (e.NewValue.ToString() == Settings.API_WUnderground)
                    {
                        keyEntry.Enabled = true;

                        if (!String.IsNullOrWhiteSpace(Settings.API_KEY) && !keyVerified)
                            keyVerified = true;

                        // Reset to old value if not verified
                        if (!keyVerified)
                            Settings.API = pref.Value;
                        else
                            Settings.API = Settings.API_WUnderground;
                    }
                    else if (e.NewValue.ToString() == Settings.API_Yahoo)
                    {
                        keyVerified = false;
                        wuSharedPrefs.Edit().Remove(KEY_APIKEY_VERIFIED).Apply();
                        keyEntry.Enabled = false;

                        Settings.API = Settings.API_Yahoo;
                    }

                    int idx = pref.GetEntryValues().ToList().IndexOf(e.NewValue.ToString());
                    pref.Summary = pref.GetEntries()[idx];
                    UpdateKeySummary();
                };

                if (Settings.API == Settings.API_WUnderground)
                {
                    keyEntry.Enabled = true;

                    if (!String.IsNullOrWhiteSpace(Settings.API_KEY) && !keyVerified)
                        keyVerified = true;
                }
                else
                {
                    keyEntry.Enabled = false;
                    wuSharedPrefs.Edit().Remove(KEY_APIKEY_VERIFIED).Apply();
                }

                UpdateKeySummary();

            }

            private void UpdateKeySummary()
            {
                if (wuSharedPrefs.Contains(KEY_APIKEY_VERIFIED))
                {
                    ForegroundColorSpan colorSpan = new ForegroundColorSpan(keyVerified ?
                        Color.Green : Color.Red);
                    ISpannable summary = new SpannableString(keyVerified ?
                        "Key Verified" : "Key Invalid");
                    summary.SetSpan(colorSpan, 0, summary.Length(), 0);
                    keyEntry.SummaryFormatted = summary;
                }
                else
                {
                    keyEntry.Summary = Activity.GetString(Resource.String.pref_summary_apikey);
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
                                App.Preferences.Edit().PutBoolean(KEY_HOMECHANGED, true).Apply();
                            }
                            else
                            {
                                // permission denied, boo! Disable the
                                // functionality that depends on this permission.
                                Toast.MakeText(Activity, "Location access denied", ToastLength.Short).Show();
                                followGps.Checked = false;
                                Console.WriteLine(Settings.FollowGPS);
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

        public class KeyEntryPreference : EditTextPreference
        {
            public EventHandler PositiveButtonClick { get; set; }
            public EventHandler NegativeButtonClick { get; set; }

            public KeyEntryPreference(Context context) :
                base(context)
            {
            }

            public KeyEntryPreference(Context context, IAttributeSet attrs) :
                base(context, attrs)
            {
            }

            public KeyEntryPreference(Context context, IAttributeSet attrs, int defStyle) :
                base(context, attrs, defStyle)
            {
            }

            public KeyEntryPreference(Context context, IAttributeSet attrs, int defStyle, int defStyleRes) :
                base(context, attrs, defStyle, defStyleRes)
            {
            }

            protected override void ShowDialog(Bundle state)
            {
                base.ShowDialog(state);

                this.EditText.Text = Settings.API_KEY;

                Android.App.AlertDialog dialog = Dialog as Android.App.AlertDialog;
                View posButton = dialog.GetButton((int)DialogButtonType.Positive);
                View negButton = dialog.GetButton((int)DialogButtonType.Negative);
                posButton.Click += PositiveButtonClick;
                if (NegativeButtonClick == null)
                    negButton.Click += delegate { dialog.Dismiss(); };
                else
                    negButton.Click += NegativeButtonClick;
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