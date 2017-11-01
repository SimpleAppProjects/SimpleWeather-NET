using System;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using SimpleWeather.WeatherData;
using SimpleWeather.Droid.Controls;
using Android.Support.V7.App;
using SimpleWeather.Controls;
using SimpleWeather.Droid.Utils;
using SimpleWeather.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Support.V4.Widget;
using Android.Support.V4.Content;
using Android.Content.Res;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using SimpleWeather.Droid.Helpers;
using Android.Runtime;
using Android.Content.PM;
using Android;
using Android.Locations;
using Android.Graphics;
using Com.Bumptech.Glide;

namespace SimpleWeather.Droid
{
    public class WeatherNowFragment : Fragment, IWeatherLoadedListener, IWeatherErrorListener,
        ActivityCompat.IOnRequestPermissionsResultCallback
    {
        private Context context;
        private LocationData location = null;
        private bool loaded = false;
        private int BGAlpha = 255;

        WeatherDataLoader wLoader = null;
        WeatherNowViewModel weatherView = null;

        AppCompatActivity AppCompatActivity;

        // Views
        private SwipeRefreshLayout refreshLayout;
        private NestedScrollView mainView;
        private ImageView bgImageView;
        // Condition
        private TextView locationName;
        private TextView updateTime;
        private TextView weatherIcon;
        private TextView weatherCondition;
        private TextView weatherTemp;
        // Details
        private View detailsPanel;
        private TextView humidity;
        private TextView pressureState;
        private TextView pressure;
        private TextView visiblity;
        private TextView feelslike;
        private TextView windDirection;
        private TextView windSpeed;
        private TextView sunrise;
        private TextView sunset;
        // Forecast
        private LinearLayout forecastPanel;
        private LinearLayout forecastView;
        // Additional Details
        private Switch forecastSwitch;
        private HorizontalScrollView forecastScrollView;
        private ViewPager txtForecastView;
        private LinearLayout hrforecastPanel;
        private LinearLayout hrforecastView;
        private LinearLayout precipitationPanel;
        private TextView chance;
        private TextView qpfRain;
        private TextView qpfSnow;
        // Nav Header View
        private View navheader;
        private TextView navLocation;
        private TextView navWeatherTemp;
        // Weather Credit
        private TextView weatherCredit;

        // GPS location
        private Android.Locations.Location mLocation;
        private LocationListener mLocListnr;
        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

        public void OnWeatherLoaded(LocationData location, Weather weather)
        {
            if (weather != null)
            {
                weatherView.UpdateView(weather);
                SetView(weatherView);

                if (Settings.HomeData.Equals(location))
                {
                    // Update widgets if they haven't been already
                    if (TimeSpan.FromTicks(DateTime.Now.Ticks - Settings.UpdateTime.Ticks).TotalMinutes > Settings.RefreshInterval)
                    {
                        context.StartService(new Intent(context, typeof(Widgets.WeatherWidgetService))
                            .SetAction(Widgets.WeatherWidgetService.ACTION_UPDATEWEATHER));
                    }

                    // Update ongoing notification if its not showing
                    if (Settings.OnGoingNotification && !Notifications.WeatherNotificationBuilder.IsShowing)
                    {
                        context.StartService(new Intent(context, typeof(Widgets.WeatherWidgetService))
                            .SetAction(Widgets.WeatherWidgetService.ACTION_REFRESHNOTIFICATION));
                    }
                }
            }

            Activity.RunOnUiThread(() => refreshLayout.Refreshing = false);
        }

        public void OnWeatherError(WeatherException wEx)
        {
            switch (wEx.ErrorStatus)
            {
                case WeatherUtils.ErrorStatus.NetworkError:
                case WeatherUtils.ErrorStatus.NoWeather:
                    // Show error message and prompt to refresh
                    Snackbar snackBar = Snackbar.Make(mainView, wEx.Message, Snackbar.LengthLong);
                    snackBar.SetAction(Resource.String.action_retry, async (View v) =>
                    {
                        await RefreshWeather(false);
                    });
                    snackBar.Show();
                    break;
                default:
                    // Show error message
                    Snackbar.Make(mainView, wEx.Message, Snackbar.LengthLong).Show();
                    break;
            }
        }

        public WeatherNowFragment()
        {
            // Required empty public constructor
            weatherView = new WeatherNowViewModel();
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
                args.PutString("data", Task.Run(() => JSONParser.Serializer(data, typeof(LocationData))).Result);
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
            AppCompatActivity = Activity as AppCompatActivity;

            // Create your fragment here
            if (Arguments != null)
            {
                location = Task.Run(() => JSONParser.DeserializerAsync<LocationData>(Arguments.GetString("data"))).Result;

                if (location != null && wLoader == null)
                    wLoader = new WeatherDataLoader(this, this, location);
            }

            if (savedInstanceState != null)
            {
                BGAlpha = savedInstanceState.GetInt("alpha", 255);
            }

            context = Activity.ApplicationContext;
            mLocListnr = new LocationListener();
            mLocListnr.LocationChanged += async (Android.Locations.Location location) =>
            {
                if (Settings.FollowGPS && await UpdateLocation())
                {
                    // Setup loader from updated location
                    wLoader = new WeatherDataLoader(this, this, this.location);

                    await RefreshWeather(false);
                }
            };
            loaded = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.fragment_weather_now, container, false);

            // Setup Actionbar
            HasOptionsMenu = false;

            refreshLayout = (SwipeRefreshLayout)view;
            mainView = view.FindViewById<NestedScrollView>(Resource.Id.fragment_weather_now);
            mainView.ScrollChange += ScrollView_ScrollChange;
            bgImageView = view.FindViewById<ImageView>(Resource.Id.image_view);
            // Condition
            locationName = view.FindViewById<TextView>(Resource.Id.label_location_name);
            updateTime = view.FindViewById<TextView>(Resource.Id.label_updatetime);
            weatherIcon = view.FindViewById<TextView>(Resource.Id.weather_icon);
            weatherCondition = view.FindViewById<TextView>(Resource.Id.weather_condition);
            weatherTemp = view.FindViewById<TextView>(Resource.Id.weather_temp);
            // Details
            detailsPanel = view.FindViewById(Resource.Id.details_panel);
            humidity = view.FindViewById<TextView>(Resource.Id.humidity);
            pressureState = view.FindViewById<TextView>(Resource.Id.pressure_state);
            pressure = view.FindViewById<TextView>(Resource.Id.pressure);
            visiblity = view.FindViewById<TextView>(Resource.Id.visibility_val);
            feelslike = view.FindViewById<TextView>(Resource.Id.feelslike);
            windDirection = view.FindViewById<TextView>(Resource.Id.wind_direction);
            windSpeed = view.FindViewById<TextView>(Resource.Id.wind_speed);
            sunrise = view.FindViewById<TextView>(Resource.Id.sunrise_time);
            sunset = view.FindViewById<TextView>(Resource.Id.sunset_time);
            // Forecast
            forecastPanel = view.FindViewById<LinearLayout>(Resource.Id.forecast_panel);
            forecastPanel.Visibility = ViewStates.Invisible;
            forecastView = view.FindViewById<LinearLayout>(Resource.Id.forecast_view);
            // Additional Details
            forecastSwitch = view.FindViewById<Switch>(Resource.Id.forecast_switch);
            forecastSwitch.CheckedChange += ForecastSwitch_CheckedChange;
            forecastSwitch.Visibility = ViewStates.Gone;
            forecastScrollView = view.FindViewById<HorizontalScrollView>(Resource.Id.forecast_scrollview);
            txtForecastView = view.FindViewById<ViewPager>(Resource.Id.txt_forecast_viewpgr);
            txtForecastView.Adapter = new TextForecastPagerAdapter(this.Activity, new List<TextForecastItemViewModel>());
            txtForecastView.Visibility = ViewStates.Gone;
            hrforecastPanel = view.FindViewById<LinearLayout>(Resource.Id.hourly_forecast_panel);
            hrforecastPanel.Visibility = ViewStates.Gone;
            hrforecastView = view.FindViewById<LinearLayout>(Resource.Id.hourly_forecast_view);
            precipitationPanel = view.FindViewById<LinearLayout>(Resource.Id.precipitation_card);
            precipitationPanel.Visibility = ViewStates.Gone;
            chance = view.FindViewById<TextView>(Resource.Id.chance_val);
            qpfRain = view.FindViewById<TextView>(Resource.Id.qpf_rain_val);
            qpfSnow = view.FindViewById<TextView>(Resource.Id.qpf_snow_val);

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

            // Nav Header View
            navheader = Activity.FindViewById<NavigationView>(Resource.Id.nav_view).GetHeaderView(0);
            navLocation = navheader.FindViewById<TextView>(Resource.Id.nav_location);
            navWeatherTemp = navheader.FindViewById<TextView>(Resource.Id.nav_weathertemp);

            weatherCredit = view.FindViewById<TextView>(Resource.Id.weather_credit);

            loaded = true;
            refreshLayout.Refreshing = true;

            return view;
        }

        private void ScrollView_ScrollChange(object sender, NestedScrollView.ScrollChangeEventArgs e)
        {
            if (bgImageView != null)
            {
                int alpha = 255 - (int)(255 * 1.25 * e.ScrollY / (e.V.GetChildAt(0).Height - e.V.Height));
                bgImageView.ImageAlpha = (alpha >= 0) ? BGAlpha = alpha : BGAlpha = 0;
            }
        }

        private static bool IsLargeTablet(Context context)
        {
            return (context.Resources.Configuration.ScreenLayout
                    & ScreenLayout.SizeMask) >= ScreenLayout.SizeLarge;
        }

        private void AdjustDetailsLayout()
        {
            if (IsLargeTablet(Activity))
            {
                mainView.Post(() =>
                {
                    if (this.View == null)
                        return;

                    Android.Support.V7.Widget.GridLayout panel = (Android.Support.V7.Widget.GridLayout)detailsPanel;

                    // Minimum width for ea. card
                    int minWidth = 600;
                    // Size of the view
                    int viewWidth = (int)(this.View.Width - panel.PaddingEnd - panel.PaddingStart);
                    // Available columns based on min card width
                    int availColumns = (viewWidth / minWidth) == 0 ? 1 : viewWidth / minWidth;
                    // Maximum columns to use
                    int maxColumns = (availColumns > panel.ChildCount) ? panel.ChildCount : availColumns;

                    int freeSpace = viewWidth - (minWidth * maxColumns);
                    // Increase card width to fill available space
                    int itemWidth = minWidth + (freeSpace / maxColumns);

                    // Adjust GridLayout
                    // Start
                    int currCol = 0;
                    int currRow = 0;
                    for(int i = 0; i < panel.ChildCount; i++)
                    {
                        View view = panel.GetChildAt(i);

                        var layoutParams = new Android.Support.V7.Widget.GridLayout.LayoutParams(
                            Android.Support.V7.Widget.GridLayout.InvokeSpec(currRow, 1.0f),
                            Android.Support.V7.Widget.GridLayout.InvokeSpec(currCol, 1.0f));
                        layoutParams.Width = 0;
                        ViewCompat.SetPaddingRelative(view, 20, 0, 20, 0);
                        view.LayoutParameters = layoutParams;
                        if (currCol == maxColumns - 1)
                        {
                            currCol = 0;
                            currRow++;
                        }
                        else
                            currCol++;
                    }
                    panel.RowCount = GridLayout.Undefined;
                    panel.ColumnCount = maxColumns;
                });
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            menu.Clear();
        }

        private async void Resume()
        {
            /* Update view on resume
             * ex. If temperature unit changed
             */

            LocationData homeData = Settings.HomeData;

            // Did home change?
            bool homeChanged = false;
            if (location != null && FragmentManager.BackStackEntryCount == 0)
            {
                if (location.Equals(homeData) && Tag != null)
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
                // Reset if source is different
                if (weatherView.WeatherSource != Settings.API)
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
                        int ttl = int.Parse(weather.ttl);
                        TimeSpan span = DateTime.Now - weather.update_time;
                        if (span.TotalMinutes > ttl)
                            await RefreshWeather(false);
                        else
                        {
                            weatherView.UpdateView(wLoader.GetWeather());
                            SetView(weatherView);
                            loaded = true;
                        }
                    }
                }
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            // Don't resume if fragment is hidden
            if (this.IsHidden)
                return;
            else
                Resume();

            // Title
            AppCompatActivity.SupportActionBar.Title = GetString(Resource.String.title_activity_weather_now);
        }

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (!hidden && weatherView != null && this.IsVisible)
            {
                UpdateNavHeader(weatherView);
                Resume();
            }
            else if (hidden)
                loaded = false;
        }

        public override void OnPause()
        {
            base.OnPause();
            loaded = false;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            // Save data
            outState.PutInt("alpha", BGAlpha);

            base.OnSaveInstanceState(outState);
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
                location = Settings.LocationData.First();
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

        private void ForecastSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            forecastSwitch.Text = e.IsChecked ? 
                Activity.GetString(Resource.String.switch_details) : Activity.GetString(Resource.String.switch_daily);
            forecastScrollView.Visibility = e.IsChecked ? ViewStates.Gone : ViewStates.Visible;
            txtForecastView.Visibility = e.IsChecked ? ViewStates.Visible : ViewStates.Gone;
        }

        private void SetView(WeatherNowViewModel weatherView)
        {
            Activity.RunOnUiThread(() =>
            {
                // Background
                refreshLayout.Background = new ColorDrawable(weatherView.PendingBackground);
                bgImageView.ImageAlpha = BGAlpha;
                Glide.With(this)
                     .Load(weatherView.Background)
                     .CenterCrop()
                     .Into(bgImageView);

                // Actionbar & StatusBar
                AppCompatActivity.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(weatherView.PendingBackground));
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    AppCompatActivity.Window.SetStatusBarColor(Color.Argb(255,
                    (int)(weatherView.PendingBackground.R * 0.75), (int)(weatherView.PendingBackground.G * 0.75), (int)(weatherView.PendingBackground.B * 0.75)));
                }

                // Location
                locationName.Text = weatherView.Location;

                // Date Updated
                updateTime.Text = weatherView.UpdateDate;

                // Update Current Condition
                weatherTemp.Text = weatherView.CurTemp;
                weatherCondition.Text = weatherView.CurCondition;
                weatherIcon.Text = weatherView.WeatherIcon;

                // WeatherDetails
                // Astronomy
                sunrise.Text = weatherView.Sunrise;
                sunset.Text = weatherView.Sunset;

                // Wind
                feelslike.Text = weatherView.WindChill;
                windSpeed.Text = weatherView.WindSpeed;
                windDirection.Rotation = weatherView.WindDirection;

                // Atmosphere
                humidity.Text = weatherView.Humidity;
                pressure.Text = weatherView.Pressure;

                pressureState.Visibility = weatherView.RisingVisiblity;
                pressureState.Text = weatherView.RisingIcon;

                visiblity.Text = weatherView._Visibility;

                // Add UI elements
                forecastView.RemoveAllViews();
                foreach (ForecastItemViewModel forecast in weatherView.Forecasts)
                {
                    forecastView.AddView(new ForecastItem(Activity, forecast));
                }
                forecastPanel.Visibility = ViewStates.Visible;

                // Additional Details
                hrforecastView.RemoveAllViews();
                if (weatherView.WUExtras.HourlyForecast.Count >= 1)
                {
                    foreach (HourlyForecastItemViewModel hrforecast in weatherView.WUExtras.HourlyForecast)
                    {
                        hrforecastView.AddView(new HourlyForecastItem(Activity, hrforecast));
                    }
                    hrforecastPanel.Visibility = ViewStates.Visible;
                    mainView.FindViewById(Resource.Id.hourly_space).Visibility = ViewStates.Visible;
                }
                else
                {
                    hrforecastPanel.Visibility = ViewStates.Gone;
                    mainView.FindViewById(Resource.Id.hourly_space).Visibility = ViewStates.Gone;
                }

                if (weatherView.WUExtras.TextForecast.Count >= 1)
                {
                    forecastSwitch.Visibility = ViewStates.Visible;
                    (txtForecastView.Adapter as TextForecastPagerAdapter).UpdateDataset(weatherView.WUExtras.TextForecast.ToList());
                }
                else
                    forecastSwitch.Visibility = ViewStates.Gone;

                if (!String.IsNullOrWhiteSpace(weatherView.WUExtras.Chance))
                {
                    chance.Text = weatherView.WUExtras.Chance;
                    qpfRain.Text = weatherView.WUExtras.Qpf_Rain;
                    qpfSnow.Text = weatherView.WUExtras.Qpf_Snow;

                    precipitationPanel.Visibility = ViewStates.Visible;

                    if (IsLargeTablet(Activity))
                    {
                        // Add back panel if not present
                        Android.Support.V7.Widget.GridLayout panel = (Android.Support.V7.Widget.GridLayout)detailsPanel;
                        int childIdx = panel.IndexOfChild(panel.FindViewById(Resource.Id.precipitation_card));
                        if (childIdx < 0)
                            panel.AddView(precipitationPanel, 0);
                    }
                }
                else
                {
                    if (IsLargeTablet(Activity))
                    {
                        Android.Support.V7.Widget.GridLayout panel = (Android.Support.V7.Widget.GridLayout)detailsPanel;
                        panel.RemoveView(panel.FindViewById(Resource.Id.precipitation_card));
                    }
                    else
                        precipitationPanel.Visibility = ViewStates.Gone;
                }

                // Fix DetailsLayout
                AdjustDetailsLayout();

                // Nav Header View
                UpdateNavHeader(weatherView);

                weatherCredit.Text = weatherView.WeatherCredit;
            });
        }

        private void UpdateNavHeader(WeatherNowViewModel weatherView)
        {
            navheader.Background = new ColorDrawable(weatherView.PendingBackground);
            navLocation.Text = weatherView.Location;
            navWeatherTemp.Text = weatherView.CurTemp;
        }

        private async Task<bool> UpdateLocation()
        {
            bool locationChanged = false;

            if (Settings.FollowGPS && (location == null || location.locationType == LocationType.GPS))
            {
                if (Activity != null && ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                    ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
                            PERMISSION_LOCATION_REQUEST_CODE);
                    return false;
                }

                LocationManager locMan = (LocationManager)Activity.GetSystemService(Context.LocationService);
                bool isGPSEnabled = locMan.IsProviderEnabled(LocationManager.GpsProvider);
                bool isNetEnabled = locMan.IsProviderEnabled(LocationManager.NetworkProvider);

                Android.Locations.Location location = null;

                if (isGPSEnabled || isNetEnabled)
                {
                    Criteria locCriteria = new Criteria() { Accuracy = Accuracy.Coarse, CostAllowed = false, PowerRequirement = Power.Low };
                    string provider = locMan.GetBestProvider(locCriteria, true);
                    location = locMan.GetLastKnownLocation(provider);

                    if (location == null)
                        locMan.RequestSingleUpdate(provider, mLocListnr, null);
                    else
                    {
                        LocationData lastGPSLocData = await Settings.GetLastGPSLocData();

                        // Check previous location difference
                        if (lastGPSLocData.query != null &&
                            mLocation != null && ConversionMethods.CalculateGeopositionDistance(mLocation, location) < 2500)
                        {
                            return false;
                        }

                        if (lastGPSLocData.query != null &&
                            Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
                            location.Latitude, location.Longitude)) < 2500)
                        {
                            return false;
                        }

                        string selected_query = string.Empty;

                        await Task.Run(async () =>
                        {
                            LocationQueryViewModel view = await GeopositionQuery.GetLocation(location);

                            if (!String.IsNullOrEmpty(view.LocationQuery))
                                selected_query = view.LocationQuery;
                            else
                                selected_query = string.Empty;
                        });

                        if (String.IsNullOrWhiteSpace(selected_query))
                        {
                            // Stop since there is no valid query
                            return false;
                        }

                        // Save location as last known
                        lastGPSLocData.SetData(selected_query, location);
                        Settings.SaveLastGPSLocData();

                        this.location = lastGPSLocData;
                        mLocation = location;
                        locationChanged = true;
                    }
                }
                else
                {
                    Toast.MakeText(Activity, Resource.String.error_retrieve_location, ToastLength.Short).Show();
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