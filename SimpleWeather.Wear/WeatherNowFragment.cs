using System;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SimpleWeather.WeatherData;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Support.V4.Widget;
using Android.Support.V4.Content;
using Android.Runtime;
using Android.Content.PM;
using Android;
using Android.Locations;
using Android.App;
using Android.Gms.Location;
using Android.Support.Wearable.Input;
using SimpleWeather.Droid.Wear.Helpers;
using SimpleWeather.Droid.Wear.Wearable;

namespace SimpleWeather.Droid.Wear
{
    public class WeatherNowFragment : Fragment,
        IWeatherLoadedListener, IWeatherErrorListener,
        ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private LocationData location = null;
        private bool loaded = false;

        private WeatherManager wm;
        private WeatherDataLoader wLoader = null;
        private WeatherNowViewModel weatherView = null;

        // Views
        private SwipeRefreshLayout refreshLayout;
        private NestedScrollView scrollView;
        // Condition
        private TextView locationName;
        private TextView updateTime;
        private TextView weatherIcon;
        private TextView weatherCondition;
        private TextView weatherTemp;
        // Weather Credit
        private TextView weatherCredit;

        private IWeatherViewLoadedListener mCallback;

        // GPS location
        private FusedLocationProviderClient mFusedLocationClient;
        private Android.Locations.Location mLocation;
        private LocationCallback mLocCallback;
        private Droid.Helpers.LocationListener mLocListnr;
        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

        // Data
        private LocalBroadcastReceiver dataReceiver;
        private bool receiverRegistered = false;
        // Timer for timing out of operations
        private System.Timers.Timer timer;

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            Activity?.RunOnUiThread(() =>
            {
                if (weather?.IsValid() == true)
                {
                    wm.UpdateWeather(weather);
                    weatherView.UpdateView(weather);
                    SetView(weatherView);
                    mCallback?.OnWeatherViewUpdated(weatherView);

                    // Update complications if they haven't been already
                    WeatherComplicationIntentService.EnqueueWork(Activity,
                        new Intent(Activity, typeof(WeatherComplicationIntentService))
                            .SetAction(WeatherComplicationIntentService.ACTION_UPDATECOMPLICATIONS));

                    if (!loaded)
                    {
                        TimeSpan span = DateTimeOffset.Now - weather.update_time;
                        if (span.TotalMinutes > Settings.DefaultInterval)
                        {
                            // send request to refresh data on connected device
                            Activity?.StartService(new Intent(Activity, typeof(WearableDataListenerService))
                                .SetAction(WearableDataListenerService.ACTION_REQUESTWEATHERUPDATE)
                                .PutExtra(WearableDataListenerService.EXTRA_FORCEUPDATE, true));
                        }

                        loaded = true;
                    }
                }

                refreshLayout.Refreshing = false;
            });
        }

        public void OnWeatherError(WeatherException wEx)
        {
            if (wEx != null)
            {
                // Show error message
                Activity?.RunOnUiThread(() =>
                {
                    Toast.MakeText(Activity, wEx.Message, ToastLength.Long).Show();
                });
            }
        }

        public WeatherNowFragment()
        {
            // Required empty public constructor
            weatherView = new WeatherNowViewModel();
            wm = WeatherManager.GetInstance();
        }

        /**
         * Use this factory method to create a new instance of
         * this fragment using the provided parameters.
         *
         * @param query Weather query.
         * @param index Location index.
         * @return A new instance of fragment WeatherNowFragment.
         */
        public static WeatherNowFragment NewInstance(LocationData data)
        {
            var fragment = new WeatherNowFragment();
            if (data != null)
            {
                Bundle args = new Bundle();
                args.PutString("data", data.ToJson());
                fragment.Arguments = args;
            }
            return fragment;
        }

        public static WeatherNowFragment NewInstance(Bundle args)
        {
            var fragment = new WeatherNowFragment()
            {
                Arguments = args
            };
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            if (Arguments != null)
            {
                using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(
                        new System.IO.StringReader(Arguments.GetString("data", null))))
                {
                    location = LocationData.FromJson(jsonTextReader);
                }

                if (location != null && wLoader == null)
                    wLoader = new WeatherDataLoader(location, this, this);
            }

            if (WearableHelper.IsGooglePlayServicesInstalled && !WearableHelper.HasGPS)
            {
                mFusedLocationClient = new FusedLocationProviderClient(Activity);
                mLocCallback = new LocationCallback();
                mLocCallback.LocationResult += async (sender, e) =>
                {
                    mLocation = e.Result.LastLocation;

                    if (Settings.FollowGPS && await UpdateLocation())
                    {
                        // Setup loader from updated location
                        wLoader = new WeatherDataLoader(this.location, this, this);

                        await RefreshWeather(false);
                    }

                    await mFusedLocationClient.RemoveLocationUpdatesAsync(mLocCallback);
                };
            }
            else
            {
                mLocListnr = new Droid.Helpers.LocationListener();
                mLocListnr.LocationChanged += async (Android.Locations.Location location) =>
                {
                    if (Settings.FollowGPS && await UpdateLocation())
                    {
                        // Setup loader from updated location
                        wLoader = new WeatherDataLoader(this.location, this, this);

                        await RefreshWeather(false);
                    }
                };
            }

            dataReceiver = new LocalBroadcastReceiver();
            dataReceiver.BroadcastReceived += async (context, intent) =>
            {
                if (WearableHelper.LocationPath.Equals(intent?.Action) ||
                    WearableHelper.WeatherPath.Equals(intent?.Action))
                {
                    if (WearableHelper.WeatherPath.Equals(intent.Action) ||
                        (!loaded && location != null))
                    {
                        if (timer.Enabled)
                            timer.Stop();

                        // We got all our data; now load the weather
                        wLoader = new WeatherDataLoader(location, this, this);
                        await wLoader.ForceLoadSavedWeatherData();
                    }

                    if (WearableHelper.LocationPath.Equals(intent.Action))
                    {
                        // We got the location data
                        location = Settings.HomeData;
                        loaded = false;
                    }
                }
                else if (WearableHelper.ErrorPath.Equals(intent?.Action))
                {
                    // An error occurred; cancel the sync operation
                    await CancelDataSync();
                }
                else if (WearableHelper.IsSetupPath.Equals(intent?.Action))
                {
                    if (Settings.DataSync != WearableDataSync.Off)
                    {
                        bool isDeviceSetup = intent.GetBooleanExtra(WearableDataListenerService.EXTRA_DEVICESETUPSTATUS, false);
                        var connStatus = (WearConnectionStatus)intent.GetIntExtra(WearableDataListenerService.EXTRA_CONNECTIONSTATUS, 0);

                        if (isDeviceSetup &&
                            connStatus == WearConnectionStatus.Connected)
                        {
                            // Device is setup and connected; proceed with sync
                            Activity?.StartService(new Intent(Activity, typeof(WearableDataListenerService))
                                .SetAction(WearableDataListenerService.ACTION_REQUESTLOCATIONUPDATE));
                            Activity?.StartService(new Intent(Activity, typeof(WearableDataListenerService))
                                .SetAction(WearableDataListenerService.ACTION_REQUESTWEATHERUPDATE));

                            ResetTimer();
                        }
                        else
                        {
                            // Device is not connected; cancel sync
                            await CancelDataSync();
                        }
                    }
                }
            };

            loaded = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.fragment_weather_now, container, false);
            view.FocusableInTouchMode = true;
            view.GenericMotion += (sender, e) =>
            {
                if (e.Event.Action == MotionEventActions.Scroll && RotaryEncoder.IsFromRotaryEncoder(e.Event))
                {
                    // Don't forget the negation here
                    float delta = -RotaryEncoder.GetRotaryAxisValue(e.Event) * RotaryEncoder.GetScaledScrollFactor(Activity);

                    // Swap these axes if you want to do horizontal scrolling instead
                    scrollView.ScrollBy(0, (int)Math.Round(delta));

                    e.Handled = true;
                }

                e.Handled = false;
            };

            refreshLayout = (SwipeRefreshLayout)view;
            scrollView = view.FindViewById<NestedScrollView>(Resource.Id.fragment_weather_now);
            // Condition
            locationName = view.FindViewById<TextView>(Resource.Id.label_location_name);
            updateTime = view.FindViewById<TextView>(Resource.Id.label_updatetime);
            weatherIcon = view.FindViewById<TextView>(Resource.Id.weather_icon);
            weatherCondition = view.FindViewById<TextView>(Resource.Id.weather_condition);
            weatherTemp = view.FindViewById<TextView>(Resource.Id.weather_temp);

            // SwipeRefresh
            refreshLayout.SetColorSchemeColors(ContextCompat.GetColor(Activity, Resource.Color.colorPrimary));
            refreshLayout.Refresh += delegate
            {
                Task.Run(async () =>
                {
                    if (Settings.FollowGPS && await UpdateLocation())
                        // Setup loader from updated location
                        wLoader = new WeatherDataLoader(this.location, this, this);

                    await RefreshWeather(true);
                });
            };

            weatherCredit = view.FindViewById<TextView>(Resource.Id.weather_credit);

            loaded = true;
            refreshLayout.Refreshing = true;

            timer = new System.Timers.Timer(30000); // 30sec
            timer.Elapsed += async (sender, e) =>
            {
                // We hit the interval
                // Data syncing is taking a long time to setup
                // Stop and load saved data
                await CancelDataSync();
            };

            return view;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            mCallback = (IWeatherViewLoadedListener)context;
            App.Preferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnDetach()
        {
            base.OnDetach();

            mCallback = null;
            App.Preferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnResume()
        {
            base.OnResume();

            // Don't resume if fragment is hidden
            if (!this.IsHidden)
            {
                Task.Run(async () =>
                {
                    // Use normal resume if sync is off
                    if (Settings.DataSync == WearableDataSync.Off)
                        await Resume();
                    else
                        await DataSyncResume();
                });
            }
        }

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (!hidden && weatherView != null && this.IsVisible)
            {
                Task.Run(async () =>
                {
                    // Use normal resume if sync is off
                    if (Settings.DataSync == WearableDataSync.Off)
                        await Resume();
                    else
                        await DataSyncResume();
                });
            }
            else if (hidden)
            {
                loaded = false;
            }
        }

        public override void OnPause()
        {
            base.OnPause();

            if (receiverRegistered)
            {
                LocalBroadcastManager.GetInstance(Activity)
                    .UnregisterReceiver(dataReceiver);
                receiverRegistered = false;
            }

            if (timer.Enabled)
                timer.Stop();

            loaded = false;
        }

        private async Task Restore()
        {
            bool forceRefresh = false;

            // GPS Follow location
            if (Settings.FollowGPS && (location == null || location.locationType == LocationType.GPS))
            {
                LocationData locData = await Settings.GetLastGPSLocData();

                if (locData == null)
                {
                    // Update location if not setup
                    await UpdateLocation();
                    wLoader = new WeatherDataLoader(location, this, this);
                    forceRefresh = true;
                }
                else
                {
                    // Reset locdata if source is different
                    if (locData.source != Settings.API)
                        Settings.SaveHomeData(new LocationData());

                    if (await UpdateLocation())
                    {
                        // Setup loader from updated location
                        wLoader = new WeatherDataLoader(location, this, this);
                        forceRefresh = true;
                    }
                    else
                    {
                        // Setup loader saved location data
                        location = locData;
                        wLoader = new WeatherDataLoader(location, this, this);
                    }
                }
            }
            else if (wLoader == null)
            {
                // Weather was loaded before. Lets load it up...
                location = Settings.HomeData;
                wLoader = new WeatherDataLoader(location, this, this);
            }

            // Load up weather data
            await RefreshWeather(forceRefresh);
        }

        private async Task Resume()
        {
            /* Update view on resume
             * ex. If temperature unit changed
             */
            // New Page = loaded - true
            // Navigating back to frag = !loaded - false
            if (loaded || wLoader == null)
            {
                await Restore();
                loaded = true;
            }
            else if (wLoader != null && !loaded)
            {
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                // Reset if source || locale is different
                if (weatherView.WeatherSource != Settings.API ||
                    wm.SupportsWeatherLocale && weatherView.WeatherLocale != locale)
                {
                    await Restore();
                    loaded = true;
                }
                else if (wLoader.GetWeather()?.IsValid() == true)
                {
                    var weather = wLoader.GetWeather();

                    // Update weather if needed on resume
                    if (Settings.FollowGPS && await UpdateLocation())
                    {
                        // Setup loader from updated location
                        wLoader = new WeatherDataLoader(this.location, this, this);
                        await RefreshWeather(false);
                        loaded = true;
                    }
                    else
                    {
                        // Check weather data expiration
                        TimeSpan span = DateTimeOffset.Now - weather.update_time;
                        if (span.TotalMinutes > Settings.DefaultInterval)
                            await RefreshWeather(false);
                        else
                        {
                            weatherView.UpdateView(wLoader.GetWeather());
                            SetView(weatherView);
                            mCallback?.OnWeatherViewUpdated(weatherView);
                            loaded = true;
                        }
                    }
                }
            }
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                return;

            switch(key)
            {
                case "key_datasync":
                    // If data sync settings changes,
                    // reset so we can properly reload
                    wLoader = null;
                    location = null;
                    break;
                default:
                    break;
            }
        }

        private void DataSyncRestore()
        {
            // Send request to service to get weather data
            Activity?.RunOnUiThread(() => refreshLayout.Refreshing = true);
            Activity?.StartService(new Intent(Activity, typeof(WearableDataListenerService))
                .SetAction(WearableDataListenerService.ACTION_REQUESTSETUPSTATUS));

            // Start timeout timer
            ResetTimer();
        }

        private async Task DataSyncResume()
        {
            if (!receiverRegistered)
            {
                IntentFilter filter = new IntentFilter();
                filter.AddAction(WearableHelper.SettingsPath);
                filter.AddAction(WearableHelper.LocationPath);
                filter.AddAction(WearableHelper.WeatherPath);
                filter.AddAction(WearableHelper.IsSetupPath);

                LocalBroadcastManager.GetInstance(Activity)
                    .RegisterReceiver(dataReceiver, filter);
                receiverRegistered = true;
            }

            /* Update view on resume
             * ex. If temperature unit changed
             */
            // New Page = loaded - true
            // Navigating back to frag = !loaded - false
            if (loaded || wLoader == null)
            {
                DataSyncRestore();
            }
            else if (wLoader != null && !loaded)
            {
                if (wLoader.GetWeather()?.IsValid() == true)
                {
                    Weather weather = wLoader.GetWeather();
                    /*
                        DateTime < 0 - This instance is earlier than value.
                        DateTime == 0 - This instance is the same as value.
                        DateTime > 0 - This instance is later than value.
                    */
                    if (Settings.UpdateTime.CompareTo(weather.update_time.UtcDateTime) > 0)
                    {
                        // Data was updated while we we're away; loaded it up
                        if (location == null || !location.Equals(Settings.HomeData))
                            location = Settings.HomeData;

                        Activity?.RunOnUiThread(() => refreshLayout.Refreshing = true);

                        wLoader = new WeatherDataLoader(this.location, this, this);
                        await wLoader.ForceLoadSavedWeatherData();
                    }
                    else
                    {
                        // Check weather data expiration
                        TimeSpan span = DateTimeOffset.Now - weather.update_time;
                        if (span.TotalMinutes > Settings.DefaultInterval)
                        {
                            // send request to refresh data on connected device
                            Activity?.StartService(new Intent(Activity, typeof(WearableDataListenerService))
                                .SetAction(WearableDataListenerService.ACTION_REQUESTWEATHERUPDATE)
                                .PutExtra(WearableDataListenerService.EXTRA_FORCEUPDATE, true));
                        }

                        weatherView.UpdateView(wLoader.GetWeather());
                        SetView(weatherView);
                        mCallback?.OnWeatherViewUpdated(weatherView);
                        loaded = true;
                    }
                }
                else
                {
                    // Data is null; restore
                    DataSyncRestore();
                }
            }
        }

        private async Task CancelDataSync()
        {
            if (timer.Enabled)
                timer.Stop();

            if (Settings.DataSync == WearableDataSync.DeviceOnly)
            {
                if (location == null && Settings.HomeData != null)
                {
                    // Load whatever we have available
                    location = Settings.HomeData;
                    wLoader = new WeatherDataLoader(location, this, this);
                    await wLoader.ForceLoadSavedWeatherData();
                }
                else
                {
                    Activity?.RunOnUiThread(() =>
                    {
                        Toast.MakeText(Activity, GetString(Resource.String.werror_noweather), ToastLength.Long).Show();
                    });
                }
            }
        }

        private void ResetTimer()
        {
            if (timer.Enabled)
                timer.Stop();
            timer.Start();
        }

        private async Task RefreshWeather(bool forceRefresh)
        {
            Activity?.RunOnUiThread(() => refreshLayout.Refreshing = true);
            if (Settings.DataSync == WearableDataSync.Off)
                await wLoader.LoadWeatherData(forceRefresh);
            else
                await DataSyncResume();
        }

        private void SetView(WeatherNowViewModel weatherView)
        {
            Activity?.RunOnUiThread(() =>
            {
                // Background
                refreshLayout.Background = new ColorDrawable(weatherView.PendingBackground);

                // Location
                locationName.Text = weatherView.Location;

                // Date Updated
                updateTime.Text = weatherView.UpdateDate;

                // Update Current Condition
                weatherTemp.Text = weatherView.CurTemp;
                weatherCondition.Text = weatherView.CurCondition;
                weatherIcon.Text = weatherView.WeatherIcon;

                weatherCredit.Text = weatherView.WeatherCredit;
            });
        }

        private async Task<bool> UpdateLocation()
        {
            bool locationChanged = false;

            if (Settings.FollowGPS && (location == null || location.locationType == LocationType.GPS))
            {
                if (Activity != null && ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(new String[] { Manifest.Permission.AccessFineLocation },
                            PERMISSION_LOCATION_REQUEST_CODE);
                    return false;
                }

                Android.Locations.Location location = null;

                if (WearableHelper.IsGooglePlayServicesInstalled && !WearableHelper.HasGPS)
                {
                    location = await mFusedLocationClient.GetLastLocationAsync();

                    if (location == null)
                    {
                        var mLocationRequest = new LocationRequest();
                        mLocationRequest.SetInterval(10000);
                        mLocationRequest.SetFastestInterval(1000);
                        mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
                        await mFusedLocationClient.RequestLocationUpdatesAsync(mLocationRequest, mLocCallback, null);
                        await mFusedLocationClient.FlushLocationsAsync();
                    }
                }
                else
                {
                    LocationManager locMan = Activity?.GetSystemService(Context.LocationService) as LocationManager;
                    bool isGPSEnabled = (bool)locMan?.IsProviderEnabled(LocationManager.GpsProvider);
                    bool isNetEnabled = (bool)locMan?.IsProviderEnabled(LocationManager.NetworkProvider);

                    if (isGPSEnabled || isNetEnabled)
                    {
                        Criteria locCriteria = new Criteria() { Accuracy = Accuracy.Coarse, CostAllowed = false, PowerRequirement = Power.Low };
                        string provider = locMan.GetBestProvider(locCriteria, true);
                        location = locMan.GetLastKnownLocation(provider);

                        if (location == null)
                            locMan.RequestSingleUpdate(provider, mLocListnr, null);
                    }
                    else
                    {
                        Toast.MakeText(Activity, Resource.String.error_retrieve_location, ToastLength.Short).Show();
                    }
                }

                if (location != null)
                {
                    LocationData lastGPSLocData = await Settings.GetLastGPSLocData();

                    // Check previous location difference
                    if (lastGPSLocData.query != null
                        && mLocation != null && ConversionMethods.CalculateGeopositionDistance(mLocation, location) < 1600)
                    {
                        return false;
                    }

                    if (lastGPSLocData.query != null
                        && Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
                        location.Latitude, location.Longitude)) < 1600)
                    {
                        return false;
                    }

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
                        return false;
                    }

                    // Save location as last known
                    lastGPSLocData.SetData(view, location);
                    Settings.SaveHomeData(lastGPSLocData);

                    this.location = lastGPSLocData;
                    mLocation = location;
                    locationChanged = true;
                }
            }

            return locationChanged;
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
                            //FetchGeoLocation();
                            Task.Run(UpdateLocation);
                        }
                        else
                        {
                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
                            Settings.FollowGPS = false;
                            Toast.MakeText(Activity, Resource.String.error_location_denied, ToastLength.Short).Show();
                        }
                        return;
                    }
                default:
                    break;
            }
        }
    }
}
