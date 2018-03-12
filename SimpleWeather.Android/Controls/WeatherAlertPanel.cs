using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using SimpleWeather.Controls;
using Android.Support.V7.Widget;
using Android.Graphics;

namespace SimpleWeather.Droid.App.Controls
{
    public class WeatherAlertPanel : RelativeLayout
    {
        private View viewLayout;
        private AppCompatImageView alertIcon;
        private TextView alertTitle;
        private TextView postDate;
        private CardView headerCard;
        private CardView bodyCard;
        private TextView expandIcon;
        private TextView bodyTextView;

        private bool Expanded = false;

        public WeatherAlertPanel(Context context) :
            base(context)
        {
            Initialize(context);
        }

        public WeatherAlertPanel(Context context, WeatherAlertViewModel alertView) :
            base(context)
        {
            Initialize(context);
            SetAlert(alertView);
        }

        public WeatherAlertPanel(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context);
        }

        public WeatherAlertPanel(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }

        public WeatherAlertPanel(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) :
            base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            LayoutInflater inflater = LayoutInflater.From(context);
            viewLayout = inflater.Inflate(Resource.Layout.weather_alert_panel, this);

            alertIcon = viewLayout.FindViewById<AppCompatImageView>(Resource.Id.alert_icon);
            alertTitle = viewLayout.FindViewById<TextView>(Resource.Id.alert_title);
            postDate = viewLayout.FindViewById<TextView>(Resource.Id.post_date);
            headerCard = viewLayout.FindViewById<CardView>(Resource.Id.header_card);
            bodyCard = viewLayout.FindViewById<CardView>(Resource.Id.body_card);
            expandIcon = viewLayout.FindViewById<TextView>(Resource.Id.expand_icon);
            bodyTextView = viewLayout.FindViewById<TextView>(Resource.Id.body_textview);

            bodyCard.Visibility = ViewStates.Gone;
            headerCard.Click += (sender, e) =>
            {
                Expanded = !Expanded;

                expandIcon.Text = App.Context.GetString(
                    Expanded ?
                    Resource.String.materialicon_expand_less :
                    Resource.String.materialicon_expand_more);
                bodyCard.Visibility = Expanded ? ViewStates.Visible : ViewStates.Gone;
            };
        }

        public void SetAlert(WeatherAlertViewModel alertView)
        {
            headerCard.SetCardBackgroundColor(GetColorFromAlertSeverity(alertView.AlertSeverity));
            alertIcon.SetImageResource(GetDrawableFromAlertType(alertView.AlertType));
            alertTitle.Text = alertView.Title;
            postDate.Text = alertView.PostDate;
            bodyTextView.Text = string.Format("{0}\n{1}\n{2}", alertView.ExpireDate, alertView.Message, alertView.Attribution);
        }

        private int GetDrawableFromAlertType(WeatherData.WeatherAlertType type)
        {
            int drawable = -1;

            switch (type)
            {
                case WeatherData.WeatherAlertType.DenseFog:
                    drawable = Resource.Drawable.fog;
                    break;
                case WeatherData.WeatherAlertType.Fire:
                    drawable = Resource.Drawable.fire;
                    break;
                case WeatherData.WeatherAlertType.FloodWarning:
                case WeatherData.WeatherAlertType.FloodWatch:
                    drawable = Resource.Drawable.flood;
                    break;
                case WeatherData.WeatherAlertType.Heat:
                    drawable = Resource.Drawable.hot;
                    break;
                case WeatherData.WeatherAlertType.HighWind:
                    drawable = Resource.Drawable.strong_wind;
                    break;
                case WeatherData.WeatherAlertType.HurricaneLocalStatement:
                case WeatherData.WeatherAlertType.HurricaneWindWarning:
                    drawable = Resource.Drawable.hurricane;
                    break;
                case WeatherData.WeatherAlertType.SevereThunderstormWarning:
                case WeatherData.WeatherAlertType.SevereThunderstormWatch:
                    drawable = Resource.Drawable.thunderstorm;
                    break;
                case WeatherData.WeatherAlertType.TornadoWarning:
                case WeatherData.WeatherAlertType.TornadoWatch:
                    drawable = Resource.Drawable.tornado;
                    break;
                case WeatherData.WeatherAlertType.Volcano:
                    drawable = Resource.Drawable.volcano;
                    break;
                case WeatherData.WeatherAlertType.WinterWeather:
                    drawable = Resource.Drawable.snowflake_cold;
                    break;
                case WeatherData.WeatherAlertType.DenseSmoke:
                    drawable = Resource.Drawable.smoke;
                    break;
                case WeatherData.WeatherAlertType.DustAdvisory:
                    drawable = Resource.Drawable.dust;
                    break;
                case WeatherData.WeatherAlertType.EarthquakeWarning:
                    drawable = Resource.Drawable.earthquake;
                    break;
                case WeatherData.WeatherAlertType.GaleWarning:
                    drawable = Resource.Drawable.gale_warning;
                    break;
                case WeatherData.WeatherAlertType.SmallCraft:
                    drawable = Resource.Drawable.small_craft_advisory;
                    break;
                case WeatherData.WeatherAlertType.StormWarning:
                    drawable = Resource.Drawable.storm_warning;
                    break;
                case WeatherData.WeatherAlertType.TsunamiWarning:
                case WeatherData.WeatherAlertType.TsunamiWatch:
                    drawable = Resource.Drawable.tsunami;
                    break;
                case WeatherData.WeatherAlertType.SevereWeather:
                case WeatherData.WeatherAlertType.SpecialWeatherAlert:
                default:
                    drawable = Resource.Drawable.ic_error_white;
                    break;
            }

            return drawable;
        }

        private Color GetColorFromAlertSeverity(WeatherData.WeatherAlertSeverity severity)
        {
            Color color = Color.Orange;

            switch (severity)
            {
                case WeatherData.WeatherAlertSeverity.Severe:
                    color = Color.OrangeRed;
                    break;
                case WeatherData.WeatherAlertSeverity.Extreme:
                    color = Color.Red;
                    break;
                case WeatherData.WeatherAlertSeverity.Moderate:
                default:
                    color = Color.Orange;
                    break;
            }

            return color;
        }
    }
}