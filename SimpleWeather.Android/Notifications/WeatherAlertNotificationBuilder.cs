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

namespace SimpleWeather.Droid.App.Notifications
{
#if DEBUG
    [BroadcastReceiver(Name = "com.thewizrd.simpleweather_debug.WeatherNotificationBroadcastReceiver")]
#else
    [BroadcastReceiver(Name = "com.thewizrd.simpleweather.WeatherNotificationBroadcastReceiver")]
#endif
    public class WeatherNotificationBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Relay intent to WeatherAlertNotificationService
            intent.SetClass(context, Java.Lang.Class.FromType(typeof(WeatherAlertNotificationService)));
            WeatherAlertNotificationService.EnqueueWork(context, intent);
        }
    }

    // Simple service to keep track of posted alert notifications
#if DEBUG
    [Service(Name = "com.thewizrd.simpleweather_debug.WeatherAlertNotificationService")]
#else
    [Service(Name = "com.thewizrd.simpleweather.WeatherAlertNotificationService")]
#endif
    public class WeatherAlertNotificationService : JobIntentService
    {
        private static string TAG = "WeatherAlertNotificationService";

        public const string ACTION_CANCELNOTIFICATION = "SimpleWeather.Droid.action.CANCEL_NOTIFICATION";
        public const string ACTION_CANCELALLNOTIFICATIONS = "SimpleWeather.Droid.action.CANCEL_ALL_NOTIFICATIONS";
        public const string EXTRA_NOTIFICATIONID = "SimpleWeather.Droid.extra.NOTIFICATION_ID";

        private const int JOB_ID = 1001;

        private static Dictionary<int, String> mNotifications;

        static WeatherAlertNotificationService()
        {
            if (mNotifications == null)
                mNotifications = new Dictionary<int, string>();
        }

        public static void EnqueueWork(Context context, Intent work)
        {
            EnqueueWork(context,
                Java.Lang.Class.FromType(typeof(WeatherAlertNotificationService)),
                JOB_ID, work);
        }

        public override void OnCreate()
        {
            base.OnCreate();

            if (mNotifications == null)
                mNotifications = new Dictionary<int, string>();
        }

        public static void AddNotication(int notID, String title)
        {
            if (mNotifications == null)
                mNotifications = new Dictionary<int, string>();

            mNotifications.TryAdd(notID, title);
        }

        public static int GetNotificationsCount()
        {
            if (mNotifications == null)
                return 0;
            else
                return mNotifications.Count;
        }

        public static IEnumerable<KeyValuePair<int, String>> GetNotifications()
        {
            return mNotifications?.AsEnumerable();
        }

        protected override void OnHandleWork(Intent intent)
        {
            if (ACTION_CANCELNOTIFICATION.Equals(intent.Action))
            {
                int id = intent.GetIntExtra(EXTRA_NOTIFICATIONID, -2);
                if (id >= 0 && mNotifications.Count > 0)
                {
                    mNotifications.Remove(id);
                }
            }
            else if (ACTION_CANCELALLNOTIFICATIONS.Equals(intent.Action))
            {
                mNotifications.Clear();
            }
        }
    }

    public static class WeatherAlertNotificationBuilder
    {
        // Sets an ID for the notification
        private const string TAG = "SimpleWeather.WeatherAlerts";
        private const int PERSISTENT_NOT_ID = 0;
        private const string NOT_CHANNEL_ID = "SimpleWeather.weatheralerts";
        private const int MIN_GROUPCOUNT = 3;
        private const int SUMMARY_ID = 0;

        public static async Task CreateNotifications(LocationData location, List<WeatherAlert> alerts)
        {
            // Gets an instance of the NotificationManager service
            NotificationManagerCompat mNotifyMgr = NotificationManagerCompat.From(App.Context);
            InitChannel();

            // Create click intent
            // Start WeatherNow Activity with weather data
            Intent intent = new Intent(App.Context, typeof(MainActivity))
                .SetAction(Widgets.WeatherWidgetService.ACTION_SHOWALERTS)
                .PutExtra("data", location.ToJson())
                .PutExtra(Widgets.WeatherWidgetService.ACTION_SHOWALERTS, true)
                .SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
            PendingIntent clickPendingIntent = PendingIntent.GetActivity(App.Context, 0, intent, 0);

            // Build update
            foreach (WeatherAlert alert in alerts)
            {
                var alertVM = new WeatherAlertViewModel(alert);

                var iconBmp = ImageUtils.TintBitmap(
                    await BitmapFactory.DecodeResourceAsync(App.Context.Resources, GetDrawableFromAlertType(alertVM.AlertType)),
                    Color.Black);

                var title = String.Format("{0} - {1}", alertVM.Title, location.name);

                NotificationCompat.Builder mBuilder =
                    new NotificationCompat.Builder(App.Context, NOT_CHANNEL_ID)
                    .SetSmallIcon(Resource.Drawable.ic_error_white)
                    .SetLargeIcon(iconBmp)
                    .SetContentTitle(title)
                    .SetContentText(alertVM.ExpireDate)
                    .SetStyle(new NotificationCompat.BigTextStyle().BigText(alertVM.ExpireDate))
                    .SetContentIntent(clickPendingIntent)
                    .SetOnlyAlertOnce(true)
                    .SetAutoCancel(true)
                    .SetPriority(NotificationCompat.PriorityDefault) as NotificationCompat.Builder;

                if(Build.VERSION.SdkInt < BuildVersionCodes.N)
                {
                    // Tell service to remove stored notification
                    mBuilder.SetDeleteIntent(GetDeleteNotificationIntent((int)alertVM.AlertType));
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N ||
                    WeatherAlertNotificationService.GetNotificationsCount() > MIN_GROUPCOUNT)
                {
                    mBuilder.SetGroup(TAG);
                }

                // Builds the notification and issues it.
                // Tag: location.query; id: weather alert type
                if (Build.VERSION.SdkInt < BuildVersionCodes.N)
                    WeatherAlertNotificationService.AddNotication((int)alertVM.AlertType, title);
                mNotifyMgr.Notify(location.query, (int)alertVM.AlertType, mBuilder.Build());
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N ||
                WeatherAlertNotificationService.GetNotificationsCount() > MIN_GROUPCOUNT)
            {
                // Notification inboxStyle for grouped notifications
                var inboxStyle = new NotificationCompat.InboxStyle();

                if (Build.VERSION.SdkInt < BuildVersionCodes.N)
                {
                    // Add active notification titles to summary notification
                    foreach (var notif in WeatherAlertNotificationService.GetNotifications())
                    {
                        mNotifyMgr.Cancel(location.query, notif.Key);
                        inboxStyle.AddLine(notif.Value);
                    }

                    inboxStyle.SetBigContentTitle(App.Context.GetString(Resource.String.title_fragment_alerts));
                    inboxStyle.SetSummaryText(location.name);
                }
                else
                {
                    inboxStyle.SetSummaryText(App.Context.GetString(Resource.String.title_fragment_alerts));
                }

                NotificationCompat.Builder mSummaryBuilder =
                    new NotificationCompat.Builder(App.Context, NOT_CHANNEL_ID)
                    .SetSmallIcon(Resource.Drawable.ic_error_white)
                    .SetContentTitle(App.Context.GetString(Resource.String.title_fragment_alerts))
                    .SetContentText(location.name)
                    .SetStyle(inboxStyle)
                    .SetGroup(TAG)
                    .SetGroupSummary(true)
                    .SetContentIntent(clickPendingIntent)
                    .SetOnlyAlertOnce(true)
                    .SetAutoCancel(true) as NotificationCompat.Builder;

                if (Build.VERSION.SdkInt < BuildVersionCodes.N)
                    mSummaryBuilder.SetDeleteIntent(GetDeleteAllNotificationsIntent());

                // Builds the summary notification and issues it.
                mNotifyMgr.Notify(location.query, SUMMARY_ID, mSummaryBuilder.Build());
            }
        }

        private static PendingIntent GetDeleteNotificationIntent(int notId)
        {
            Intent intent = new Intent(App.Context, typeof(WeatherNotificationBroadcastReceiver))
                .SetAction(WeatherAlertNotificationService.ACTION_CANCELNOTIFICATION)
                .PutExtra(WeatherAlertNotificationService.EXTRA_NOTIFICATIONID, notId);

            // Use notification id as unique request code
            return PendingIntent.GetBroadcast(App.Context, notId, intent, PendingIntentFlags.UpdateCurrent);
        }

        private static PendingIntent GetDeleteAllNotificationsIntent()
        {
            Intent intent = new Intent(App.Context, typeof(WeatherNotificationBroadcastReceiver))
                .SetAction(WeatherAlertNotificationService.ACTION_CANCELALLNOTIFICATIONS);

            return PendingIntent.GetBroadcast(App.Context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        private static void InitChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationManager mNotifyMgr = (NotificationManager)App.Context.GetSystemService(App.NotificationService);
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