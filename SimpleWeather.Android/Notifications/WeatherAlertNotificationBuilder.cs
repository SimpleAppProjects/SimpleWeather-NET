using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using SimpleWeather.Droid.Utils;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using Android.Text.Format;
using Android.OS;
using Android.Service.Notification;
using System.Collections.Generic;
using Android.Graphics;
using System.Threading.Tasks;
using SimpleWeather.Controls;

namespace SimpleWeather.Droid.Notifications
{
    public static class WeatherAlertNotificationBuilder
    {
        // Sets an ID for the notification
        private const string TAG = "SimpleWeather.WeatherAlerts";
        private const int PERSISTENT_NOT_ID = 0;
        private const string NOT_CHANNEL_ID = "SimpleWeather.weatheralerts";

        public static async Task CreateNotifications(LocationData location, List<WeatherAlert> alerts)
        {
            // Gets an instance of the NotificationManager service
            NotificationManager mNotifyMgr = (NotificationManager)App.Context.GetSystemService(App.NotificationService);
            InitChannel(mNotifyMgr);

            // Build update
            foreach(WeatherAlert alert in alerts)
            {
                var alertVM = new WeatherAlertViewModel(alert);

                var iconBmp = ImageUtils.TintBitmap(
                    await BitmapFactory.DecodeResourceAsync(App.Context.Resources, GetDrawableFromAlertType(alertVM.AlertType)),
                    Color.Black);

                NotificationCompat.Builder mBuilder =
                    new NotificationCompat.Builder(App.Context, NOT_CHANNEL_ID)
                    .SetSmallIcon(Resource.Drawable.ic_error_white)
                    .SetLargeIcon(iconBmp)
                    .SetContentTitle(String.Format("{0} - {1}", alertVM.Title, location.name))
                    .SetContentText(alertVM.ExpireDate)
                    .SetStyle(new NotificationCompat.BigTextStyle().BigText(alertVM.ExpireDate))
                    .SetGroup(TAG)
                    .SetOnlyAlertOnce(true)
                    .SetAutoCancel(true)
                    .SetPriority(NotificationCompat.PriorityDefault) as NotificationCompat.Builder;

                // Start WeatherNow Activity with weather data
                Intent intent = new Intent(App.Context, typeof(MainActivity))
                    .SetAction(Widgets.WeatherWidgetService.ACTION_SHOWALERTS)
                    .PutExtra("data", location.ToJson())
                    .PutExtra(Widgets.WeatherWidgetService.ACTION_SHOWALERTS, true)
                    .SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                PendingIntent clickPendingIntent = PendingIntent.GetActivity(App.Context, 0, intent, 0);
                mBuilder.SetContentIntent(clickPendingIntent);

                // Builds the notification and issues it.
                mNotifyMgr.Notify(TAG, alert.GetHashCode(), mBuilder.Build());
            }
        }

        private static void InitChannel(NotificationManager mNotifyMgr)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel mChannel = mNotifyMgr.GetNotificationChannel(NOT_CHANNEL_ID);

                if (mChannel == null)
                {
                    String notchannel_name = App.Context.Resources.GetString(Resource.String.not_channel_name_alerts);
                    String notchannel_desc = App.Context.Resources.GetString(Resource.String.not_channel_desc_alerts);

                    mChannel = new NotificationChannel(NOT_CHANNEL_ID, notchannel_name, NotificationImportance.Default)
                    {
                        Description = notchannel_desc
                    };
                    // Configure the notification channel.
                    mChannel.SetShowBadge(true);
                    mChannel.EnableLights(true);
                    mChannel.EnableVibration(false);
                    mNotifyMgr.CreateNotificationChannel(mChannel);
                }
            }
        }

        private static int GetDrawableFromAlertType(WeatherAlertType type)
        {
            int iconRes = Resource.Drawable.ic_logo;
            switch (type)
            {
                case WeatherAlertType.DenseFog:
                    iconRes = Resource.Drawable.fog;
                    break;
                case WeatherAlertType.Fire:
                    iconRes = Resource.Drawable.fire;
                    break;
                case WeatherAlertType.FloodWarning:
                case WeatherAlertType.FloodWatch:
                    iconRes = Resource.Drawable.flood;
                    break;
                case WeatherAlertType.Heat:
                    iconRes = Resource.Drawable.hot;
                    break;
                case WeatherAlertType.HighWind:
                    iconRes = Resource.Drawable.strong_wind;
                    break;
                case WeatherAlertType.HurricaneLocalStatement:
                case WeatherAlertType.HurricaneWindWarning:
                    iconRes = Resource.Drawable.hurricane;
                    break;
                case WeatherAlertType.SevereThunderstormWarning:
                case WeatherAlertType.SevereThunderstormWatch:
                    iconRes = Resource.Drawable.thunderstorm;
                    break;
                case WeatherAlertType.TornadoWarning:
                case WeatherAlertType.TornadoWatch:
                    iconRes = Resource.Drawable.tornado;
                    break;
                case WeatherAlertType.Volcano:
                    iconRes = Resource.Drawable.volcano;
                    break;
                case WeatherAlertType.WinterWeather:
                    iconRes = Resource.Drawable.snowflake_cold;
                    break;
                case WeatherAlertType.SevereWeather:
                case WeatherAlertType.SpecialWeatherAlert:
                default:
                    iconRes = Resource.Drawable.ic_error_white;
                    break;
            }

            return iconRes;
        }
    }
}