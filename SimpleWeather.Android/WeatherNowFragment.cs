using System;

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
using Com.Nostra13.Universalimageloader.Core;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Support.V4.Widget;
using Android.Support.V4.Content;
using Android.Content.Res;

namespace SimpleWeather.Droid
{
    public class WeatherNowFragment : Fragment, IWeatherLoadedListener
    {
        private Context context;
        private Pair<int, string> pair;
        private bool loaded = false;

        WeatherDataLoader wLoader = null;
        WeatherNowViewModel weatherView = null;

        // Views
        private SwipeRefreshLayout refreshLayout;
        private View mainView;
        // Condition
        private TextView locationName;
        private TextView updateTime;
        private WeatherIcon weatherIcon;
        private TextView weatherCondition;
        private WeatherIcon weatherTemp;
        // Details
        private View detailsPanel;
        private TextView humidity;
        private WeatherIcon pressureState;
        private TextView pressure;
        private TextView visiblity;
        private TextView feelslike;
        private WeatherIcon windDirection;
        private TextView windSpeed;
        private TextView sunrise;
        private TextView sunset;
        // Forecast
        private LinearLayout forecastPanel;
        private LinearLayout forecastView;

        private ImageLoader loader = ImageLoader.Instance;

        private String fahrenheit = App.Context.GetString(Resource.String.wi_fahrenheit);
        private String celsius = App.Context.GetString(Resource.String.wi_celsius);

        public void OnWeatherLoaded(int locationIdx, Weather weather)
        {
            if (weather != null)
            {
                weatherView.UpdateView(weather);
                SetView(weatherView);
            }

            Activity.RunOnUiThread(() => refreshLayout.Refreshing = false);
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
        public static WeatherNowFragment NewInstance(Pair<int, string> pair)
        {
            WeatherNowFragment fragment = new WeatherNowFragment();
            if (pair != null)
            {
                Bundle args = new Bundle();
                args.PutString("pair", Task.Run(() => JSONParser.Serializer(pair, typeof(Pair<int, string>))).Result);
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
                pair = Task.Run(() => JSONParser.DeserializerAsync<Pair<int, string>>(Arguments.GetString("pair"))).Result;

                if (pair != null && wLoader == null)
                    wLoader = new WeatherDataLoader(this, pair.Value, pair.Key);
            }

            context = Activity.ApplicationContext;
            loaded = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.fragment_weather_now, container, false);

            // Setup Actionbar
            HasOptionsMenu = false;

            refreshLayout = (SwipeRefreshLayout)view;
            mainView = view.FindViewById(Resource.Id.fragment_weather_now);
            // Condition
            locationName = (TextView)view.FindViewById(Resource.Id.label_location_name);
            updateTime = (TextView)view.FindViewById(Resource.Id.label_updatetime);
            weatherIcon = (WeatherIcon)view.FindViewById(Resource.Id.weather_icon);
            weatherCondition = (TextView)view.FindViewById(Resource.Id.weather_condition);
            weatherTemp = (WeatherIcon)view.FindViewById(Resource.Id.weather_temp);
            // Details
            detailsPanel = view.FindViewById(Resource.Id.details_panel);
            humidity = (TextView)view.FindViewById(Resource.Id.humidity);
            pressureState = (WeatherIcon)view.FindViewById(Resource.Id.pressure_state);
            pressure = (TextView)view.FindViewById(Resource.Id.pressure);
            visiblity = (TextView)view.FindViewById(Resource.Id.visibility_val);
            feelslike = (TextView)view.FindViewById(Resource.Id.feelslike);
            windDirection = (WeatherIcon)view.FindViewById(Resource.Id.wind_direction);
            windSpeed = (TextView)view.FindViewById(Resource.Id.wind_speed);
            sunrise = (TextView)view.FindViewById(Resource.Id.sunrise_time);
            sunset = (TextView)view.FindViewById(Resource.Id.sunset_time);
            // Forecast
            forecastPanel = (LinearLayout)view.FindViewById(Resource.Id.forecast_panel);
            forecastPanel.Visibility = ViewStates.Invisible;
            forecastView = (LinearLayout)view.FindViewById(Resource.Id.forecast_view);

            // SwipeRefresh
            refreshLayout.SetColorSchemeColors(ContextCompat.GetColor(Activity, Resource.Color.colorPrimary));
            refreshLayout.Refresh += delegate { Task.Run(() => RefreshWeather(true)); };

            // Fix DetailsLayout
            AdjustDetailsLayout();

            loaded = true;
            refreshLayout.Refreshing = true;
            Task.Run(Restore);

            return view;
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
                    Android.Support.V7.Widget.GridLayout panel = (Android.Support.V7.Widget.GridLayout)detailsPanel;

                    // Minimum width for ea. card
                    int minWidth = 600;
                    // Size of the view
                    int viewWidth = (int)(this.View.Width - panel.PaddingRight - panel.PaddingLeft);
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
                        view.LayoutParameters = new Android.Support.V7.Widget.GridLayout.LayoutParams(
                            Android.Support.V7.Widget.GridLayout.InvokeSpec(currRow, 1.0f),
                            Android.Support.V7.Widget.GridLayout.InvokeSpec(currCol, 1.0f));
                        view.LayoutParameters.Width = 0;
                        view.SetPaddingRelative(20, 0, 20, 0);
                        if (currCol == maxColumns - 1)
                        {
                            currCol = 0;
                            currRow++;
                        }
                        else
                            currCol++;
                    }
                    panel.RowCount = currRow + 1;
                    panel.ColumnCount = maxColumns;
                });
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            menu.Clear();
        }

        public override void OnResume()
        {
            base.OnResume();

            // Update view on resume
            // ex. If temperature unit changed
            if (wLoader != null && !loaded)
            {
                if (wLoader.GetWeather() != null)
                {
                    weatherView.UpdateView(wLoader.GetWeather());
                    SetView(weatherView);
                    loaded = true;
                }
            }

            // Title
            AppCompatActivity activity = (AppCompatActivity)Activity;
            activity.SupportActionBar.Title = GetString(Resource.String.title_activity_weather_now);
        }

        public override void OnPause()
        {
            base.OnPause();
            loaded = false;
        }

        private async Task Restore()
        {
            if (wLoader == null)
            {
                // Weather was loaded before. Lets load it up...
                List<string> locations = await Settings.GetLocations();
                string local = locations[App.HomeIdx];

                wLoader = new WeatherDataLoader(this, local, App.HomeIdx);
            }

            // Load up weather data
            await RefreshWeather(false);
        }

        private async Task RefreshWeather(bool forceRefresh)
        {
            await wLoader.LoadWeatherData(forceRefresh);
        }

        private void SetView(WeatherNowViewModel weatherView)
        {
            // Background
            mainView.Post(() => 
            {
                View.Background = new ColorDrawable(weatherView.PendingBackground);
                loader.DisplayImage(weatherView.Background, new CustomViewAware(mainView), ImageUtils.CenterCropConfig(View.Width, View.Height));
            });

            Activity.RunOnUiThread(() =>
            {
                forecastView.Background = ContextCompat.GetDrawable(Activity, weatherView.PanelBackground);
                detailsPanel.Background = ContextCompat.GetDrawable(Activity, weatherView.PanelBackground);
            
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
                if (forecastPanel.Visibility != ViewStates.Visible)
                    forecastPanel.Visibility = ViewStates.Visible;
            });
        }
    }
}