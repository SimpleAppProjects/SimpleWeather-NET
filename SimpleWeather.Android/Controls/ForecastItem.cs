using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using SimpleWeather.Controls;
using Android.Support.Constraints;

namespace SimpleWeather.Droid.App.Controls
{
    public class ForecastItem : ConstraintLayout
    {
        private View viewLayout;
        private TextView forecastDate;
        private TextView forecastIcon;
        private TextView forecastCondition;
        private TextView forecastTempHi;
        private TextView forecastTempLo;

        public ForecastItem(Context context) :
            base(context)
        {
            Initialize(context);
        }

        public ForecastItem(Context context, ForecastItemViewModel forecastView) :
            base(context)
        {
            Initialize(context);
            SetForecast(forecastView);
        }

        public ForecastItem(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context);
        }

        public ForecastItem(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            LayoutInflater inflater = LayoutInflater.From(context);
            viewLayout = inflater.Inflate(Resource.Layout.weather_forecast_panel, this);

            forecastDate = viewLayout.FindViewById<TextView>(Resource.Id.forecast_date);
            forecastIcon = viewLayout.FindViewById<TextView>(Resource.Id.forecast_icon);
            forecastCondition = viewLayout.FindViewById<TextView>(Resource.Id.forecast_condition);
            forecastTempHi = viewLayout.FindViewById<TextView>(Resource.Id.forecast_temphi);
            forecastTempLo = viewLayout.FindViewById<TextView>(Resource.Id.forecast_templo);
        }

        public void SetForecast(ForecastItemViewModel forecastView)
        {
            forecastDate.Text = forecastView.Date;
            forecastIcon.Text = forecastView.WeatherIcon;
            forecastCondition.Text = forecastView.Condition;
            forecastTempHi.Text = forecastView.HiTemp;
            forecastTempLo.Text = forecastView.LoTemp;
        }
    }
}