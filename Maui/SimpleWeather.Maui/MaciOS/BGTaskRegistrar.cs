#if __IOS__
using BackgroundTasks;
using SimpleWeather.Extras.BackgroundTasks;
using SimpleWeather.Maui.BackgroundTasks;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Maui
{
    public static class BGTaskRegistrar
    {
        public static void RegisterBGTasks(this AppDelegate appDelegate)
        {
            WidgetUpdaterTask.RegisterTask();
            WeatherUpdaterTask.RegisterTask();
            DailyNotificationTask.RegisterTask();
            RemoteConfigUpdateTask.RegisterTask();
            PremiumStatusTask.RegisterTask();
            AppUpdaterTask.RegisterTask();

            BGTaskScheduler.Shared.GetPending((tks) =>
            {
                tks.ForEach(t =>
                {
                    Logger.WriteLine(LoggerLevel.Debug, $"{t.Identifier}: task registered");
                });
            });
        }
    }
}
#endif
