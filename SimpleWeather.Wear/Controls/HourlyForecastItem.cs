using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using SimpleWeather.Controls;

namespace SimpleWeather.Droid.Wear.Controls
{
    public class HourlyForecastItem : LinearLayout
    {
        private View viewLayout;
        private TextView forecastDate;
        private TextView forecastIcon;
        private TextView forecastTempHi;

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
            forecastIcon = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_icon);
            forecastTempHi = viewLayout.FindViewById<TextView>(Resource.Id.hrforecast_temphi);
        }

        public void SetForecast(HourlyForecastItemViewModel forecastView)
        {
            forecastDate.Text = forecastView.Date;
            forecastIcon.Text = forecastView.WeatherIcon;
            forecastTempHi.Text = forecastView.HiTemp;
        }
    }
}