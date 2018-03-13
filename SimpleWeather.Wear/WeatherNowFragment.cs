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

namespace SimpleWeather.Droid.Wear
{
    public class WeatherNowFragment : Fragment, IWeatherLoadedListener, IWeatherErrorListener
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

        Helpers.IWeatherViewLoadedListener mCallback;

        // GPS location
        private FusedLocationProviderClient mFusedLocationClient;
        private Android.Locations.Location mLocation;
        private LocationCallback mLocCallback;
        private Droid.Helpers.LocationListener mLocListnr;
        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            if (weather != null)
            {
                wm.UpdateWeather(weather);
                weatherView.UpdateView(weather);
                SetView(weatherView);
                mCallback?.OnWeatherViewUpdated(weatherView);
            }

            Activity?.RunOnUiThread(() => refreshLayout.Refreshing = false);
        }

        public void OnWeatherError(WeatherException wEx)
        {
            if (wEx != null)
            {
                // Show error message
                Toast.MakeText(Activity, wEx.Message, ToastLength.Long).Show();
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
            WeatherNowFragment fragment = new WeatherNowFragment();
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
            WeatherNowFragment fragment = new WeatherNowFragment()
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
                location = LocationData.FromJson(
                    new Newtonsoft.Json.JsonTextReader(
                        new System.IO.StringReader(Arguments.GetString("data", null))));

                if (location != null && wLoader == null)
                    wLoader = new WeatherDataLoader(this, this, location);
            }

            if (App.IsGooglePlayServicesInstalled && !App.HasGPS)
            {
                mFusedLocationClient = new FusedLocationProviderClient(Activity);
                mLocCallback = new LocationCallback();
                mLocCallback.LocationResult += async (sender, e) =>
                {
                    mLocation = e.Result.LastLocation;

                    if (Settings.FollowGPS && await UpdateLocation())
                    {
                        // Setup loader from updated location
                        wLoader = new WeatherDataLoader(this, this, this.location);

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
                        wLoader = new WeatherDataLoader(this, this, this.location);

                        await RefreshWeather(false);
                    }
                };
            }
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
                        wLoader = new WeatherDataLoader(this, this, this.location);

                    await RefreshWeather(true);
                });
            };

            weatherCredit = view.FindViewById<TextView>(Resource.Id.weather_credit);

            loaded = true;
            refreshLayout.Refreshing = true;

            return view;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            mCallback = (Helpers.IWeatherViewLoadedListener)context;
        }

        public override void OnDetach()
        {
            base.OnDetach();

            mCallback = null;
        }

        private async Task Resume()
        {
            /* Update view on resume
             * ex. If temperature unit changed
             */

            LocationData homeData = Settings.HomeData;

            // Did home change?
            bool homeChanged = false;
            if (location != null && FragmentManager.BackStackEntryCount == 0)
            {
                if (!location.Equals(homeData) && Tag == "home")
                {
                    location = homeData;
                    wLoader = null;
                    homeChanged = true;
                }
            }

            // New Page = loaded - true
            // Navigating back to frag = !loaded - false
            if (loaded || homeChanged || wLoader == null)
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
                else if (wLoader.GetWeather() != null)
                {
                    Weather weather = wLoader.GetWeather();

                    // Update weather if needed on resume
                    if (Settings.FollowGPS && await UpdateLocation())
                    {
                        // Setup loader from updated location
                        wLoader = new WeatherDataLoader(this, this, this.location);
                        await RefreshWeather(false);
                        loaded = true;
                    }
                    else
                    {
                        // Check weather data expiration
                        if (!int.TryParse(weather.ttl, out int ttl))
                            ttl = 60;
                        TimeSpan span = DateTimeOffset.Now - weather.update_time;
                        if (span.TotalMinutes > ttl)
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

        public override async void OnResume()
        {
            base.OnResume();

            // Don't resume if fragment is hidden
            if (this.IsHidden)
                return;
            else
                await Resume();
        }

        public override async void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (!hidden && weatherView != null && this.IsVisible)
                await Resume();
            else if (hidden)
                loaded = false;
        }

        public override void OnPause()
        {
            base.OnPause();
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
                    wLoader = new WeatherDataLoader(this, this, location);
                    forceRefresh = true;
                }
                else
                {
                    // Reset locdata if source is different
                    if (locData.source != Settings.API)
                        Settings.SaveLastGPSLocData(new LocationData());

                    if (await UpdateLocation())
                    {
                        // Setup loader from updated location
                        wLoader = new WeatherDataLoader(this, this, location);
                        forceRefresh = true;
                    }
                    else
                    {
                        // Setup loader saved location data
                        location = locData;
                        wLoader = new WeatherDataLoader(this, this, location);
                    }
                }
            }
            else if (wLoader == null)
            {
                // Weather was loaded before. Lets load it up...
                location = Settings.HomeData;
                wLoader = new WeatherDataLoader(this, this, location);
            }

            // Load up weather data
            await RefreshWeather(forceRefresh);
        }

        private async Task RefreshWeather(bool forceRefresh)
        {
            refreshLayout.Refreshing = true;
            await wLoader.LoadWeatherData(forceRefresh);
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

                if (App.IsGooglePlayServicesInstalled && !App.HasGPS)
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
                    if (lastGPSLocData.query != null &&
                        mLocation != null && ConversionMethods.CalculateGeopositionDistance(mLocation, location) < 1600)
                    {
                        return false;
                    }

                    if (lastGPSLocData.query != null &&
                        Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
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
                    Settings.SaveLastGPSLocData();

                    this.location = lastGPSLocData;
                    mLocation = location;
                    locationChanged = true;
                }
            }

            return locationChanged;
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
                            //FetchGeoLocation();
                            await UpdateLocation();
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
            }
        }
    }
}