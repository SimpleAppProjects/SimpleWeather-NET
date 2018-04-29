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
using Android.Util;

namespace SimpleWeather.Droid.App.Notifications
{
    public static class WeatherNotificationBuilder
    {
        // Sets an ID for the notification
        private const int PERSISTENT_NOT_ID = 0;
        private const string NOT_CHANNEL_ID = "SimpleWeather.ongoingweather";

        private static Notification mNotification;
        public static bool IsShowing = false;

        public static void UpdateNotification(Weather weather)
        {
            // Gets an instance of the NotificationManager service
            NotificationManager mNotifyMgr = (NotificationManager)App.Context.GetSystemService(App.NotificationService);
            InitChannel(mNotifyMgr);

            var wm = WeatherManager.GetInstance();

            // Build update
            RemoteViews updateViews = new RemoteViews(App.Context.PackageName, Resource.Layout.notification_layout);

            string temp = (Settings.IsFahrenheit ?
                    Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            string condition = weather.condition.weather;
            string hitemp = (Settings.IsFahrenheit ?
                    weather.forecast[0].high_f : weather.forecast[0].high_c) + "º";
            string lotemp = (Settings.IsFahrenheit ?
                    weather.forecast[0].low_f : weather.forecast[0].low_c) + "º";

            // Weather icon
            updateViews.SetImageViewResource(Resource.Id.weather_icon, 
                wm.GetWeatherIconResource(weather.condition.icon));

            // Location Name
            updateViews.SetTextViewText(Resource.Id.location_name, weather.location.name);

            // Condition text
            updateViews.SetTextViewText(Resource.Id.condition_weather, string.Format("{0} - {1}", temp, condition));

            // Details
            updateViews.SetTextViewText(Resource.Id.condition_details,
                string.Format("{0} / {1}", hitemp, lotemp));

            // Update Time
            string timeformat = DateTime.Now.ToString("h:mm tt");

            if (DateFormat.Is24HourFormat(App.Context))
                timeformat = DateTime.Now.ToString("HH:mm");

            updateViews.SetTextViewText(Resource.Id.update_time, timeformat);

            // Progress bar
            updateViews.SetViewVisibility(Resource.Id.refresh_button, ViewStates.Visible);
            updateViews.SetViewVisibility(Resource.Id.refresh_progress, ViewStates.Gone);
            Intent refreshClickIntent = new Intent(App.Context, typeof(Widgets.WeatherWidgetBroadcastReceiver))
                .SetAction(Widgets.WeatherWidgetService.ACTION_REFRESHNOTIFICATION);
            PendingIntent prgPendingIntent = PendingIntent.GetBroadcast(App.Context, 0, refreshClickIntent, 0);
            updateViews.SetOnClickPendingIntent(Resource.Id.refresh_button, prgPendingIntent);

            if (!int.TryParse(temp.Replace("º", ""), out int level))
                level = 0;

            int resId = level < 0 ? Resource.Drawable.notification_temp_neg : Resource.Drawable.notification_temp_pos;

            NotificationCompat.Builder mBuilder =
                new NotificationCompat.Builder(App.Context, NOT_CHANNEL_ID)
                .SetContent(updateViews)
                .SetPriority(NotificationCompat.PriorityLow)
                .SetOngoing(true) as NotificationCompat.Builder;

            if (Settings.NotificationIcon == Settings.TEMPERATURE_ICON)
                mBuilder.SetSmallIcon(resId, Math.Abs(level));
            else if (Settings.NotificationIcon == Settings.CONDITION_ICON)
                mBuilder.SetSmallIcon(wm.GetWeatherIconResource(weather.condition.icon));

            Intent onClickIntent = new Intent(App.Context, typeof(MainActivity))
                .SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
            PendingIntent clickPendingIntent = PendingIntent.GetActivity(App.Context, 0, onClickIntent, 0);
            mBuilder.SetContentIntent(clickPendingIntent);

            // Builds the notification and issues it.
            mNotification = mBuilder.Build();
            mNotifyMgr.Notify(PERSISTENT_NOT_ID, mNotification);
            IsShowing = true;
        }

        private static void InitChannel(NotificationManager mNotifyMgr)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel mChannel = mNotifyMgr.GetNotificationChannel(NOT_CHANNEL_ID);

                if (mChannel == null)
                {
                    String notchannel_name = App.Context.Resources.GetString(Resource.String.not_channel_name_weather);
                    String notchannel_desc = App.Context.Resources.GetString(Resource.String.not_channel_desc_weather);

                    mChannel = new NotificationChannel(NOT_CHANNEL_ID, notchannel_name, NotificationImportance.Low)
                    {
                        Description = notchannel_desc
                    };
                    // Configure the notification channel.
                    mChannel.SetShowBadge(true);
                    mChannel.EnableLights(false);
                    mChannel.EnableVibration(false);
                    mNotifyMgr.CreateNotificationChannel(mChannel);
                }
            }
        }

        public static void ShowRefresh()
        {
            // Gets an instance of the NotificationManager service
            NotificationManager mNotifyMgr = (NotificationManager)App.Context.GetSystemService(App.NotificationService);
            InitChannel(mNotifyMgr);

            if (mNotification == null)
            {
                try
                {
                    StatusBarNotification statNot = null;
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    {
                        var statNots = mNotifyMgr.GetActiveNotifications();
                        if (statNots?.Length > 0)
                            statNot = statNots.FirstOrDefault(not => not.Id == PERSISTENT_NOT_ID);
                    }

                    if (statNot != null && statNot.Notification != null)
                        mNotification = statNot.Notification;
                    else
                    {
                        NotificationCompat.Builder mBuilder =
                            new NotificationCompat.Builder(App.Context, NOT_CHANNEL_ID)
                            .SetSmallIcon(Resource.Drawable.ic_logo)
                            .SetPriority(NotificationCompat.PriorityLow)
                            .SetOngoing(true) as NotificationCompat.Builder;

                        mNotification = mBuilder.Build();
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("WeatherNotificationBuilder", ex.StackTrace);
                }
            }

            // Build update
            RemoteViews updateViews = null;

            if (mNotification.ContentView == null)
                updateViews = new RemoteViews(App.Context.PackageName, Resource.Layout.notification_layout);
            else
                updateViews = mNotification.ContentView;

            updateViews.SetViewVisibility(Resource.Id.refresh_button, ViewStates.Gone);
            updateViews.SetViewVisibility(Resource.Id.refresh_progress, ViewStates.Visible);

            mNotification.ContentView = updateViews;

            // Builds the notification and issues it.
            mNotifyMgr.Notify(PERSISTENT_NOT_ID, mNotification);
            IsShowing = true;
        }

        public static void RemoveNotification()
        {
            NotificationManager mNotifyMgr = (NotificationManager)App.Context.GetSystemService(App.NotificationService);
            mNotifyMgr.Cancel(PERSISTENT_NOT_ID);
            IsShowing = false;
        }
    }
}