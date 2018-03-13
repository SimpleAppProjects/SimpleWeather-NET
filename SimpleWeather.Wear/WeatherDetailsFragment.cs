using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.Wear.Widget;
using Android.Util;
using Android.Widget;
using Android.Views;
using SimpleWeather.Controls;
using SimpleWeather.Droid.Adapters;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using Android.Support.Wearable.Input;
using Android.Support.V4.Widget;

namespace SimpleWeather.Droid.Wear
{
    public class WeatherDetailsFragment : SwipeDismissFragment
    {
        private WeatherNowViewModel weatherView = null;

        // Details
        private NestedScrollView scrollView;
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
        private RelativeLayout precipitationPanel;
        private TextView chanceLabel;
        private TextView chance;
        private TextView qpfRain;
        private TextView qpfSnow;
        private TextView cloudinessLabel;
        private TextView cloudiness;
        private TextView weatherCredit;

        public static WeatherDetailsFragment NewInstance(WeatherNowViewModel weatherViewModel)
        {
            WeatherDetailsFragment fragment = new WeatherDetailsFragment();
            if (weatherViewModel != null)
            {
                fragment.weatherView = weatherViewModel;
            }
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View outerView = base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.fragment_weather_details, outerView as ViewGroup, true);
            outerView.FocusableInTouchMode = true;
            outerView.GenericMotion += (sender, e) =>
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

            scrollView = view.FindViewById<NestedScrollView>(Resource.Id.scrollView);
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

            // Additional Details
            precipitationPanel = view.FindViewById<RelativeLayout>(Resource.Id.precipitation_card);
            precipitationPanel.Visibility = ViewStates.Gone;
            chanceLabel = view.FindViewById<TextView>(Resource.Id.chance_label);
            chance = view.FindViewById<TextView>(Resource.Id.chance_val);
            cloudinessLabel = view.FindViewById<TextView>(Resource.Id.cloudiness_label);
            cloudiness = view.FindViewById<TextView>(Resource.Id.cloudiness);
            qpfRain = view.FindViewById<TextView>(Resource.Id.qpf_rain_val);
            qpfSnow = view.FindViewById<TextView>(Resource.Id.qpf_snow_val);

            // Cloudiness only supported by OWM
            cloudinessLabel.Visibility = ViewStates.Gone;
            cloudiness.Visibility = ViewStates.Gone;

            weatherCredit = view.FindViewById<TextView>(Resource.Id.weather_credit);

            return outerView;
        }

        public override void OnResume()
        {
            base.OnResume();

            // Don't resume if fragment is hidden
            if (this.IsHidden)
                return;
            else
                Initialize();
        }

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (!hidden && this.IsVisible)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if (weatherView != null)
            {
                View?.SetBackgroundColor(weatherView.PendingBackground);
                View?.RequestFocus();

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

                if (!String.IsNullOrWhiteSpace(weatherView.Extras.Chance))
                {
                    chance.Text = cloudiness.Text = weatherView.Extras.Chance;
                    qpfRain.Text = weatherView.Extras.Qpf_Rain;
                    qpfSnow.Text = weatherView.Extras.Qpf_Snow;

                    if (!Settings.API.Equals(WeatherAPI.MetNo))
                    {
                        precipitationPanel.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        precipitationPanel.Visibility = ViewStates.Gone;
                    }

                    if (Settings.API.Equals(WeatherAPI.OpenWeatherMap) || Settings.API.Equals(WeatherAPI.MetNo))
                    {
                        chanceLabel.Visibility = ViewStates.Gone;
                        chance.Visibility = ViewStates.Gone;

                        cloudinessLabel.Visibility = ViewStates.Visible;
                        cloudiness.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        chanceLabel.Visibility = ViewStates.Visible;
                        chance.Visibility = ViewStates.Visible;

                        cloudinessLabel.Visibility = ViewStates.Gone;
                        cloudiness.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    precipitationPanel.Visibility = ViewStates.Gone;

                    cloudinessLabel.Visibility = ViewStates.Gone;
                    cloudiness.Visibility = ViewStates.Gone;
                }

                weatherCredit.Text = weatherView.WeatherCredit;
            }
        }
    }
}