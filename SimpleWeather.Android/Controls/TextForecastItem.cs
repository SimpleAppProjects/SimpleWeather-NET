using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using SimpleWeather.Controls;

namespace SimpleWeather.Droid.Controls
{
    public class TextForecastItem : LinearLayout
    {
        private View viewLayout;
        private WeatherIcon forecastIcon;
        private TextView forecastPoP;
        private TextView forecastText;

        public TextForecastItem(Context context) :
            base(context)
        {
            Initialize(context);
        }

        public TextForecastItem(Context context, TextForecastItemViewModel forecastView) :
            base(context)
        {
            Initialize(context);
            SetForecast(forecastView);
        }

        public TextForecastItem(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context);
        }

        public TextForecastItem(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }

        public TextForecastItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) :
            base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            LayoutInflater inflater = LayoutInflater.From(context);
            viewLayout = inflater.Inflate(Resource.Layout.txt_forecast_panel, this);

            forecastIcon = (WeatherIcon)viewLayout.FindViewById(Resource.Id.txt_forecasticon);
            forecastPoP = (TextView)viewLayout.FindViewById(Resource.Id.txtforecast_pop);
            forecastText = (TextView)viewLayout.FindViewById(Resource.Id.txt_fcttext);
        }

        public void SetForecast(TextForecastItemViewModel forecastView)
        {
            forecastIcon.Text = forecastView.WeatherIcon;
            forecastPoP.Text = forecastView.PoP;
            forecastText.Text = forecastView.FctText;
        }
    }
}