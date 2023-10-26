#if __IOS__
using BackgroundTasks;
using Foundation;
using SimpleWeather.Extras.BackgroundTasks;
using SimpleWeather.Maui.BackgroundTasks;
using SimpleWeather.Utils;
#if DEBUG
using ObjCRuntime;
using System.Runtime.InteropServices;
#endif

namespace SimpleWeather.Maui
{
    public static class BGTaskRegistrar
    {
#if DEBUG
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);
#endif

        public static void RegisterBGTasks()
        {
            WidgetUpdaterTask.RegisterTask();
            WeatherUpdaterTask.RegisterTask();
            DailyNotificationTask.RegisterTask();
            RemoteConfigUpdateTask.RegisterTask();
            PremiumStatusTask.RegisterTask();
            AppUpdaterTask.RegisterTask();
        }

        public static void ScheduleBGTasks()
        {
            WidgetUpdaterTask.ScheduleTask();
            WeatherUpdaterTask.ScheduleTask();
            DailyNotificationTask.ScheduleTask();
            RemoteConfigUpdateTask.ScheduleTask();
            PremiumStatusTask.ScheduleTask();
            AppUpdaterTask.ScheduleTask();

#if DEBUG
            BGTaskScheduler.Shared.GetPending((tks) =>
            {
                tks.ForEach(t =>
                {
                    Logger.WriteLine(LoggerLevel.Debug, $"{t.Identifier}: task registered; EarliestBeginDate - {t.EarliestBeginDate}");
                });
            });

            var t = new Thread(async () =>
            {
                Thread.Sleep(TimeSpan.FromMinutes(0.5));

                var tks = await BGTaskScheduler.Shared.GetPendingAsync();

                tks.ForEach(t =>
                {
                    Logger.WriteLine(LoggerLevel.Debug, $"{t.Identifier}: task registered; EarliestBeginDate - {t.EarliestBeginDate}");
                });

                TestLaunchTask();
            })
            {
                Name = "Test BG Task"
            };
            t.Start();
#endif
        }

#if DEBUG
        private static void TestLaunchTask()
        {
            using var taskId = new NSString(WidgetUpdaterTask.TASK_ID);
            var method = new Selector("_simulateLaunchForTaskWithIdentifier:");
            void_objc_msgSend_IntPtr(BGTaskScheduler.Shared.Handle, method.Handle, taskId.Handle);
        }
#endif
    }
}
#endif
