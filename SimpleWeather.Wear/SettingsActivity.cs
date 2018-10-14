using System;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Util;
using Android;
using Android.Content.PM;
using Android.Runtime;
using Android.Widget;
using SimpleWeather.Utils;
using Android.Text.Style;
using Android.Graphics;
using Android.Text;
using System.Text;
using Android.Support.Wearable.Activity;
using Android.App;
using Android.Preferences;
using System.IO;
using Google.Android.Wearable.Intent;
using Android.Support.Wear.Widget;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.Support.V4.Content;
using SimpleWeather.Droid.Wear.Helpers;

namespace SimpleWeather.Droid.Wear
{
    [Activity(Theme = "@style/WearSettingsTheme")]
    public class SettingsActivity : WearableActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Display the fragment as the main content.
            Fragment fragment = FragmentManager.FindFragmentById(Android.Resource.Id.Content);

            // Check if fragment exists
            if (fragment == null)
            {
                FragmentManager.BeginTransaction()
                        .Replace(Android.Resource.Id.Content, new SettingsFragment())
                        .Commit();
            }
        }

        public override void OnBackPressed()
        {
            string KEY_API = "API";

            if (FragmentManager.FindFragmentById(Android.Resource.Id.Content) is SettingsFragment fragment)
            {
                ListPreference keyPref = fragment.FindPreference(KEY_API) as ListPreference;
                if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && WeatherData.WeatherManager.IsKeyRequired(keyPref.Value))
                {
                    // Set keyentrypref color to red
                    Toast.MakeText(this, Resource.String.message_enter_apikey, ToastLength.Long).Show();
                    if (fragment.View is SwipeDismissFrameLayout dismissLayout)
                    {
                        //dismissLayout.Reset();
                    }
                    return;
                }
            }

            base.OnBackPressed();
        }

        public class SettingsFragment : SwipeDismissPreferenceFragment
        {
            private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

            // Preference Keys
            private const string KEY_ABOUTAPP = "key_aboutapp";
            private const string KEY_FOLLOWGPS = "key_followgps";
            private const string KEY_API = "API";
            private const string KEY_APIKEY = "API_KEY";
            private const string KEY_USECELSIUS = "key_usecelsius";
            private const string KEY_DATASYNC = "key_datasync";
            private const string KEY_CONNSTATUS = "key_connectionstatus";
            private const string KEY_APIREGISTER = "key_apiregister";
            private const string KEY_USEPERSONALKEY = "key_usepersonalkey";

            private const string CATEGORY_API = "category_api";

            // Preferences
            private SwitchPreference followGps;
            private ListPreference providerPref;
            private SwitchPreference personalKeyPref;
            private KeyEntryPreference keyEntry;
            private ListPreference syncPreference;
            private Preference connStatusPref;
            private Preference registerPref;

            private PreferenceCategory apiCategory;

            private ConnectionStatusReceiver connStatusReceiver;

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_general);

                apiCategory = (PreferenceCategory)FindPreference(CATEGORY_API);

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
                        if (Activity.CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                            Activity.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                        {
                            RequestPermissions(new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
                                    PERMISSION_LOCATION_REQUEST_CODE);
                            return;
                        }
                    }
                };

                keyEntry = (KeyEntryPreference)FindPreference(KEY_APIKEY);
                keyEntry.BeforeDialogCreated += (object sender, EventArgs e) =>
                {
                    keyEntry?.UpdateAPI(providerPref.Value);
                };

                keyEntry.PositiveButtonClick += async (sender, e) =>
                {
                    String key = keyEntry.APIKey;

                    String API = providerPref.Value;
                    if (await WeatherData.WeatherManager.IsKeyValid(key, API))
                    {
                        Settings.API_KEY = key;
                        Settings.API = API;

                        Settings.KeyVerified = true;
                        UpdateKeySummary();

                        keyEntry.Dialog.Dismiss();
                    }
                };

                personalKeyPref = (SwitchPreference)FindPreference(KEY_USEPERSONALKEY);
                personalKeyPref.PreferenceChange += (object sender, Preference.PreferenceChangeEventArgs e) =>
                {
                    var pref = e.Preference as SwitchPreference;
                    if ((bool)e.NewValue)
                    {
                        if (apiCategory.FindPreference(KEY_APIKEY) == null)
                            apiCategory.AddPreference(keyEntry);
                        if (apiCategory.FindPreference(KEY_APIREGISTER) == null)
                            apiCategory.AddPreference(registerPref);
                        keyEntry.Enabled = true;
                    }
                    else
                    {
                        var selectedWProv = WeatherData.WeatherManager.GetProvider(providerPref.Value);

                        if (!String.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
                        {
                            // We're using our own (verified) keys
                            Settings.KeyVerified = true;
                            Settings.API = providerPref.Value;
                        }

                        keyEntry.Enabled = false;
                        apiCategory.RemovePreference(keyEntry);
                        apiCategory.RemovePreference(registerPref);
                    }
                };

                var providers = WeatherData.WeatherAPI.APIs;
                providerPref = (ListPreference)FindPreference(KEY_API);
                providerPref.SetEntries(providers.Select(provider => provider.Display).ToArray());
                providerPref.SetEntryValues(providers.Select(provider => provider.Value).ToArray());
                providerPref.Persistent = false;
                providerPref.PreferenceChange += (object sender, Preference.PreferenceChangeEventArgs e) =>
                {
                    var pref = e.Preference as ListPreference;
                    var selectedWProv = WeatherData.WeatherManager.GetProvider(e.NewValue.ToString());

                    if (selectedWProv.KeyRequired)
                    {
                        if (String.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
                        {
                            personalKeyPref.Checked = Settings.UsePersonalKey = true;
                            personalKeyPref.Enabled = false;
                            keyEntry.Enabled = false;
                            apiCategory.RemovePreference(keyEntry);
                            apiCategory.RemovePreference(registerPref);
                        }
                        else
                        {
                            personalKeyPref.Enabled = true;
                        }

                        if (!Settings.UsePersonalKey)
                        {
                            // We're using our own (verified) keys
                            Settings.KeyVerified = true;
                            keyEntry.Enabled = false;
                            apiCategory.RemovePreference(keyEntry);
                            apiCategory.RemovePreference(registerPref);
                        }
                        else
                        {
                            keyEntry.Enabled = true;

                            if (apiCategory.FindPreference(KEY_APIKEY) == null)
                                apiCategory.AddPreference(keyEntry);
                            if (apiCategory.FindPreference(KEY_APIREGISTER) == null)
                                apiCategory.AddPreference(registerPref);
                        }

                        if (apiCategory.FindPreference(KEY_USEPERSONALKEY) == null)
                            apiCategory.AddPreference(personalKeyPref);

                        // Reset to old value if not verified
                        if (!Settings.KeyVerified)
                            Settings.API = pref.Value;
                        else
                            Settings.API = e.NewValue.ToString();

                        var providerEntry = providers.Find(provider => provider.Value == e.NewValue.ToString());
                        UpdateKeySummary(providerEntry.Display);
                        UpdateRegisterLink(providerEntry.Value);
                    }
                    else
                    {
                        Settings.KeyVerified = false;
                        keyEntry.Enabled = false;
                        personalKeyPref.Enabled = false;

                        Settings.API = e.NewValue.ToString();
                        // Clear API KEY entry to avoid issues
                        Settings.API_KEY = String.Empty;

                        apiCategory.RemovePreference(personalKeyPref);
                        apiCategory.RemovePreference(keyEntry);
                        apiCategory.RemovePreference(registerPref);
                        UpdateKeySummary();
                        UpdateRegisterLink();
                    }
                };

                registerPref = FindPreference(KEY_APIREGISTER);
                registerPref.PreferenceClick += RegisterPref_PreferenceClick;

                // Set key as verified if API Key is req for API and its set
                if (WeatherData.WeatherManager.GetInstance().KeyRequired)
                {
                    keyEntry.Enabled = true;

                    if (!String.IsNullOrWhiteSpace(Settings.API_KEY) && !Settings.KeyVerified)
                        Settings.KeyVerified = true;

                    if (String.IsNullOrWhiteSpace(WeatherData.WeatherManager.GetInstance().GetAPIKey()))
                    {
                        personalKeyPref.Checked = Settings.UsePersonalKey = true;
                        personalKeyPref.Enabled = false;
                        keyEntry.Enabled = false;
                        apiCategory.RemovePreference(keyEntry);
                        apiCategory.RemovePreference(registerPref);
                    }
                    else
                    {
                        personalKeyPref.Enabled = true;
                    }

                    if (!Settings.UsePersonalKey)
                    {
                        // We're using our own (verified) keys
                        Settings.KeyVerified = true;
                        keyEntry.Enabled = false;
                        apiCategory.RemovePreference(keyEntry);
                        apiCategory.RemovePreference(registerPref);
                    }
                }
                else
                {
                    keyEntry.Enabled = false;
                    personalKeyPref.Enabled = false;
                    apiCategory.RemovePreference(personalKeyPref);
                    apiCategory.RemovePreference(keyEntry);
                    apiCategory.RemovePreference(registerPref);
                    Settings.KeyVerified = false;
                }

                UpdateKeySummary();
                UpdateRegisterLink();

                syncPreference = (ListPreference)FindPreference(KEY_DATASYNC);
                syncPreference.PreferenceChange += (object sender, Preference.PreferenceChangeEventArgs e) =>
                {
                    int newValue = int.Parse(e.NewValue.ToString());

                    ListPreference pref = e.Preference as ListPreference;
                    pref.Summary = pref.GetEntries()[newValue];

                    EnableSyncedSettings((WearableDataSync)newValue == WearableDataSync.Off);
                };
                syncPreference.Summary = syncPreference.GetEntries()[int.Parse(syncPreference.Value)];
                EnableSyncedSettings(Settings.DataSync == WearableDataSync.Off);

                connStatusPref = FindPreference(KEY_CONNSTATUS);
                connStatusReceiver = new ConnectionStatusReceiver();
                connStatusReceiver.ConnectionStatusChanged += (status) =>
                {
                    switch (status)
                    {
                        case WearConnectionStatus.Disconnected:
                            connStatusPref.Summary = GetString(Resource.String.status_disconnected);
                            connStatusPref.PreferenceClick -= ConnStatusPref_PreferenceClick;
                            break;
                        case WearConnectionStatus.Connecting:
                            connStatusPref.Summary = GetString(Resource.String.status_connecting);
                            connStatusPref.PreferenceClick -= ConnStatusPref_PreferenceClick;
                            break;
                        case WearConnectionStatus.AppNotInstalled:
                            connStatusPref.Summary = GetString(Resource.String.status_notinstalled);
                            connStatusPref.PreferenceClick += ConnStatusPref_PreferenceClick;
                            break;
                        case WearConnectionStatus.Connected:
                            connStatusPref.Summary = GetString(Resource.String.status_connected);
                            connStatusPref.PreferenceClick -= ConnStatusPref_PreferenceClick;
                            break;
                        default:
                            break;
                    }
                };
            }

            private void EnableSyncedSettings(bool enable)
            {
                FindPreference(KEY_USECELSIUS).Enabled = enable;
                followGps.Enabled = enable;
                apiCategory.Enabled = enable;
            }

            private void ConnStatusPref_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
            {
                var intentAndroid = new Intent(Intent.ActionView)
                    .AddCategory(Intent.CategoryBrowsable)
                    .SetData(WearableHelper.PlayStoreURI);

                RemoteIntent.StartRemoteActivity(Activity, intentAndroid,
                    new ConfirmationResultReceiver(Activity));
            }

            private void RegisterPref_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
            {
                var intentAndroid = new Intent(e.Preference.Intent)
                    .AddCategory(Intent.CategoryBrowsable);

                RemoteIntent.StartRemoteActivity(Activity, intentAndroid,
                    new ConfirmationResultReceiver(Activity));
            }

            private void UpdateKeySummary()
            {
                UpdateKeySummary(providerPref.Entry);
            }

            private void UpdateKeySummary(string providerAPI)
            {
                if (!String.IsNullOrWhiteSpace(Settings.API_KEY))
                {
                    var keyVerified = Settings.KeyVerified;

                    ForegroundColorSpan colorSpan = new ForegroundColorSpan(keyVerified ?
                        Color.Green : Color.Red);
                    ISpannable summary = new SpannableString(keyVerified ?
                        GetString(Resource.String.message_keyverified) : GetString(Resource.String.message_keyinvalid));
                    summary.SetSpan(colorSpan, 0, summary.Length(), 0);
                    keyEntry.SummaryFormatted = summary;
                }
                else
                {
                    keyEntry.Summary = Activity.GetString(Resource.String.pref_summary_apikey, providerAPI);
                }
            }

            private void UpdateRegisterLink()
            {
                UpdateRegisterLink(providerPref.Value);
            }

            private void UpdateRegisterLink(string providerAPI)
            {
                registerPref.Intent = new Intent(Intent.ActionView)
                    .SetData(Android.Net.Uri.Parse(
                        WeatherData.WeatherAPI.APIs.First(prov => prov.Value == providerAPI).APIRegisterURL));
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
                                // Reset home location data
                                //Settings.SaveLastGPSLocData(new WeatherData.LocationData());
                            }
                            else
                            {
                                // permission denied, boo! Disable the
                                // functionality that depends on this permission.
                                Toast.MakeText(Activity, Resource.String.error_location_denied, ToastLength.Short).Show();
                                followGps.Checked = false;
                                Console.WriteLine(Settings.FollowGPS);
                            }
                            return;
                        }
                    default:
                        break;
                }
            }

            public override void OnResume()
            {
                base.OnResume();

                LocalBroadcastManager.GetInstance(Activity)
                    .RegisterReceiver(connStatusReceiver, new IntentFilter(WearableDataListenerService.ACTION_UPDATECONNECTIONSTATUS));
                Activity.StartService(new Intent(Activity, typeof(WearableDataListenerService))
                    .SetAction(WearableDataListenerService.ACTION_UPDATECONNECTIONSTATUS));
            }

            public override void OnPause()
            {
                LocalBroadcastManager.GetInstance(Activity)
                    .UnregisterReceiver(connStatusReceiver);

                base.OnPause();
            }
        }

        internal class ConnectionStatusReceiver : BroadcastReceiver
        {
            public event Action<WearConnectionStatus> ConnectionStatusChanged;

            public override void OnReceive(Context context, Intent intent)
            {
                if (WearableDataListenerService.ACTION_UPDATECONNECTIONSTATUS.Equals(intent?.Action))
                {
                    var connStatus = (WearConnectionStatus)intent.GetIntExtra(WearableDataListenerService.EXTRA_CONNECTIONSTATUS, 0);
                    ConnectionStatusChanged?.Invoke(connStatus);
                }
            }
        }

        public class KeyEntryPreference : EditTextPreference
        {
            public EventHandler BeforeDialogCreated { get; set; }
            public EventHandler PositiveButtonClick { get; set; }
            public EventHandler NegativeButtonClick { get; set; }

            private String CurrentAPI { get; set; }
            public String APIKey { get; private set; }

            private EditText keyEntry;
            private EditText keyEntry2;

            public KeyEntryPreference(Context context)
                : base(context)
            {
                CurrentAPI = Settings.API;
            }

            public KeyEntryPreference(Context context, IAttributeSet attrs)
                : base(context, attrs)
            {
                CurrentAPI = Settings.API;
            }

            public KeyEntryPreference(Context context, IAttributeSet attrs, int defStyleAttr)
                : base(context, attrs, defStyleAttr)
            {
                CurrentAPI = Settings.API;
            }

            public KeyEntryPreference(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
                : base(context, attrs, defStyleAttr, defStyleRes)
            {
                CurrentAPI = Settings.API;
            }

            public void UpdateAPI(String CurrentAPI)
            {
                this.CurrentAPI = CurrentAPI;
            }

            protected override View OnCreateDialogView()
            {
                BeforeDialogCreated?.Invoke(this, EventArgs.Empty);

                if (CurrentAPI == WeatherData.WeatherAPI.Here)
                {
                    LayoutInflater inflater = LayoutInflater.From(Context);
                    return inflater.Inflate(Resource.Layout.layout_keyentry2_dialog, null);
                }
                else
                {
                    return base.OnCreateDialogView();
                }
            }

            protected override void OnBindDialogView(View view)
            {
                base.OnBindDialogView(view);

                keyEntry = view.FindViewById<EditText>(Android.Resource.Id.Edit);
                keyEntry.TextChanged += EditText_TextChanged;

                if (CurrentAPI == WeatherData.WeatherAPI.Here)
                {
                    keyEntry2 = view.FindViewById<EditText>(Resource.Id.keyEntry2);
                    keyEntry2.TextChanged += EditText_TextChanged;
                }
            }

            private void EditText_TextChanged(object sender, TextChangedEventArgs e)
            {
                if (CurrentAPI == WeatherData.WeatherAPI.Here)
                {
                    string app_id = keyEntry?.Text;
                    string app_code = keyEntry2?.Text;

                    APIKey = String.Format("{0};{1}", app_id, app_code);
                }
                else
                    APIKey = e.Text.ToString();
            }

            protected override void ShowDialog(Bundle state)
            {
                base.ShowDialog(state);

                AlertDialog alertdialog = Dialog as AlertDialog;

                View posButton = alertdialog.GetButton((int)DialogButtonType.Positive);
                View negButton = alertdialog.GetButton((int)DialogButtonType.Negative);
                posButton.Click += PositiveButtonClick;
                if (NegativeButtonClick == null)
                    negButton.Click += delegate { alertdialog.Dismiss(); };
                else
                    negButton.Click += NegativeButtonClick;

                APIKey = Settings.API_KEY;

                if (CurrentAPI == WeatherData.WeatherAPI.Here)
                {
                    string app_id = String.Empty;
                    string app_code = String.Empty;

                    if (!String.IsNullOrWhiteSpace(APIKey))
                    {
                        var keyArr = APIKey.Split(';');
                        app_id = keyArr.First();
                        app_code = keyArr.Last();
                    }

                    keyEntry.Text = app_id;
                    keyEntry2.Text = app_code;
                }
            }
        }

        public class AboutAppFragment : SwipeDismissPreferenceFragment
        {
            // Preference Keys
            private static string KEY_ABOUTCREDITS = "key_aboutcredits";
            private static string KEY_ABOUTOSLIBS = "key_aboutoslibs";
            private static string KEY_ABOUTVERSION = "key_aboutversion";

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_aboutapp);

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
        }

        public class CreditsFragment : SwipeDismissPreferenceFragment
        {
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_credits);
            }

            public override bool OnPreferenceTreeClick(PreferenceScreen preferenceScreen, Preference preference)
            {
                if (preference != null && preference.Intent != null)
                {
                    RemoteIntent.StartRemoteActivity(Activity, preference.Intent
                        .SetAction(Intent.ActionView)
                        .AddCategory(Intent.CategoryBrowsable),
                        null);

                    // Show open on phone animation
                    new Android.Support.Wearable.Views.ConfirmationOverlay()
                        .SetType(Android.Support.Wearable.Views.ConfirmationOverlay.OpenOnPhoneAnimation)
                        .SetMessage(Activity.GetString(Resource.String.message_openedonphone))
                        .ShowAbove(View);

                    return true;
                }

                return base.OnPreferenceTreeClick(preferenceScreen, preference);
            }
        }

        public class OSSCreditsFragment : SwipeDismissPreferenceFragment
        {
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_oslibs);
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

                TextView textview = view.FindViewById<TextView>(Resource.Id.textview);

                // Load html to string
                String text = String.Empty;
                using (StreamReader sReader = new StreamReader(Context.Assets.Open("credits/licenses.txt")))
                {
                    text = sReader.ReadToEnd();
                }

                textview.Text = text;
            }
        }
    }
}