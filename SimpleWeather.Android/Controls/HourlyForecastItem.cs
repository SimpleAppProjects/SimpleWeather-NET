using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using SimpleWeather.Controls;

namespace SimpleWeather.Droid.Controls
{
    public class HourlyForecastItem : LinearLayout
    {
        private View viewLayout;
        private TextView forecastDate;
        private WeatherIcon forecastIcon;
        private TextView forecastCondition;
        private TextView forecastTempHi;
        private TextView forecastPoP;
        private WeatherIcon forecastWindDirection;
        private TextView forecastWindSpeed;

        public HourlyForecastItem(Context context) :
            base(context)
        {
            Initialize(context);
        }

        public HourlyForecastItem(Context context, HourlyForecastItemViewModel forecastView) :
            base(context)
        {
            Initialize(context);
            SetForecast(forecastView);
        }

        public HourlyForecastItem(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context);
        }

        public HourlyForecastItem(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }

        public HourlyForecastItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) :
            base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            LayoutInflater inflater = LayoutInflater.From(context);
            viewLayout = inflater.Inflate(Resource.Layout.weather_hrforecast_panel, this);

            forecastDate = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_date);
            forecastIcon = viewLayout.FindViewById<WeatherIcon>(Resource.Id.hrforecast_icon);
            forecastCondition = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_condition);
            forecastTempHi = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_temphi);
            forecastPoP = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_pop);
            forecastWindDirection = viewLayout.FindViewById<WeatherIcon>(Resource.Id.hrforecast_wind_dir);
            forecastWindSpeed = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_wind);
        }

        public void SetForecast(HourlyForecastItemViewModel forecastView)
        {
            forecastDate.Text = forecastView.Date;
            forecastIcon.Text = forecastView.WeatherIcon;
            forecastCondition.Text = forecastView.Condition;
            forecastTempHi.Text = forecastView.HiTemp;
            forecastPoP.Text = forecastView.PoP;
            forecastWindDirection.Rotation = forecastView.WindDirection;
            forecastWindSpeed.Text = forecastView.WindSpeed;
        }
    }
}