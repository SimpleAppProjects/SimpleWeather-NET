using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using SimpleWeather.Controls;

namespace SimpleWeather.Droid.Controls
{
    public class HourlyForecastItem : LinearLayout
    {
        private Context mContext;
        private View viewLayout;
        private TextView forecastDate;
        private TextView forecastIcon;
        private TextView forecastCondition;
        private TextView forecastTempHi;
        private TextView forecastPoPIcon;
        private TextView forecastPoP;
        private TextView forecastWindDirection;
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
            mContext = context;
            LayoutInflater inflater = LayoutInflater.From(context);
            viewLayout = inflater.Inflate(Resource.Layout.weather_hrforecast_panel, this);

            forecastDate = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_date);
            forecastIcon = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_icon);
            forecastCondition = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_condition);
            forecastTempHi = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_temphi);
            forecastPoPIcon = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_pop_icon);
            forecastPoP = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_pop);
            forecastWindDirection = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_wind_dir);
            forecastWindSpeed = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_wind);
        }

        public void SetForecast(HourlyForecastItemViewModel forecastView)
        {
            forecastDate.Text = forecastView.Date;
            forecastIcon.Text = forecastView.WeatherIcon;
            forecastCondition.Text = forecastView.Condition;
            forecastTempHi.Text = forecastView.HiTemp;

            if (SimpleWeather.Utils.Settings.API.Equals(WeatherData.WeatherAPI.OpenWeatherMap) ||
                SimpleWeather.Utils.Settings.API.Equals(WeatherData.WeatherAPI.MetNo))
                forecastPoPIcon.Text = mContext.GetString(Resource.String.wi_cloudy);
            else
                forecastPoPIcon.Text = mContext.GetString(Resource.String.wi_raindrop);

            forecastPoP.Text = forecastView.PoP;
            forecastWindDirection.Rotation = forecastView.WindDirection;
            forecastWindSpeed.Text = forecastView.WindSpeed;
        }
    }
}