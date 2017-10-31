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

namespace SimpleWeather.Droid.Notifications
{
    public static class WeatherNotificationBuilder
    {
        // Sets an ID for the notification
        private const int PERSISTENT_NOT_ID = 0;
        private static Notification mNotification;
        public static bool IsShowing = false;

        public static void UpdateNotification(Weather weather)
        {
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
                WeatherUtils.GetWeatherIconResource(weather.condition.icon));

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
            Intent refreshClickIntent = new Intent(App.Context, typeof(Widgets.WeatherWidgetService))
                .SetAction(Widgets.WeatherWidgetService.ACTION_UPDATEWEATHER);
            PendingIntent prgPendingIntent = PendingIntent.GetService(App.Context, 0, refreshClickIntent, 0);
            updateViews.SetOnClickPendingIntent(Resource.Id.refresh_button, prgPendingIntent);

            int level = int.Parse(temp.Replace("º", ""));
            int resId = level < 0 ? Resource.Drawable.notification_temp_neg : Resource.Drawable.notification_temp_pos;

            NotificationCompat.Builder mBuilder =
                new NotificationCompat.Builder(App.Context)
                .SetContent(updateViews)
                .SetPriority(NotificationCompat.PriorityLow)
                .SetOngoing(true) as NotificationCompat.Builder;

            if (Settings.NotificationIcon == Settings.TEMPERATURE_ICON)
                mBuilder.SetSmallIcon(resId, Math.Abs(level));
            else if (Settings.NotificationIcon == Settings.CONDITION_ICON)
                mBuilder.SetSmallIcon(WeatherUtils.GetWeatherIconResource(weather.condition.icon));

            Intent onClickIntent = new Intent(App.Context, typeof(MainActivity));
            PendingIntent clickPendingIntent = PendingIntent.GetActivity(App.Context, 0, onClickIntent, 0);
            mBuilder.SetContentIntent(clickPendingIntent);

            // Gets an instance of the NotificationManager service
            NotificationManager mNotifyMgr = (NotificationManager)App.Context.GetSystemService(App.NotificationService);
            // Builds the notification and issues it.
            mNotification = mBuilder.Build();
            mNotifyMgr.Notify(PERSISTENT_NOT_ID, mNotification);
            IsShowing = true;
        }

        public static void ShowRefresh()
        {
            if (mNotification == null)
            {
                NotificationCompat.Builder mBuilder =
                    new NotificationCompat.Builder(App.Context)
                    .SetSmallIcon(Resource.Drawable.ic_logo)
                    .SetPriority(NotificationCompat.PriorityLow)
                    .SetOngoing(true) as NotificationCompat.Builder;

                mNotification = mBuilder.Build();
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

            // Gets an instance of the NotificationManager service
            NotificationManager mNotifyMgr = (NotificationManager)App.Context.GetSystemService(App.NotificationService);
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