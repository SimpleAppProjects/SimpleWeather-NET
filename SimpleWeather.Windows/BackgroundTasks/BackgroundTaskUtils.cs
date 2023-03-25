using SimpleWeather.BackgroundTasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;

namespace SimpleWeather.NET.BackgroundTasks
{
    internal static class BackgroundTaskUtils
    {
        public static bool IsWin32ComTaskPresent()
        {
            return ApiInformation.IsMethodPresent("Windows.ApplicationModel.Background.BackgroundTaskBuilder", "SetTaskEntryPointClsid");
        }

        public static void SetTaskEntryPoint(this BackgroundTaskBuilder backgroundTaskBuilder)
        {
            /** Win32 Bg Task: only supports a limited amount of triggers
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041, 0) && IsWin32ComTaskPresent())
            {
                backgroundTaskBuilder.SetTaskEntryPointClsid(typeof(InProcBackgroundTask).GUID);
            }
            */
            backgroundTaskBuilder.TaskEntryPoint = BackgroundTask.TASK_ENTRY_POINT;
        }

        public static BackgroundTaskBuilder CreateTask(string taskName)
        {
            var task = new BackgroundTaskBuilder()
            {
                Name = taskName
            };
            task.SetTaskEntryPoint();
            return task;
        }

        public static BackgroundTaskBuilder Condition(this BackgroundTaskBuilder builder, IBackgroundCondition condition)
        {
            builder.AddCondition(condition);
            return builder;
        }

        public static BackgroundTaskBuilder Trigger(this BackgroundTaskBuilder builder, IBackgroundTrigger trigger)
        {
            builder.SetTrigger(trigger);
            return builder;
        }
    }
}
