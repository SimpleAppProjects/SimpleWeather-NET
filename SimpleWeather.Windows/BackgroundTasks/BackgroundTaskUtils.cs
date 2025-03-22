using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;
using BackgroundTaskBuilder = Microsoft.Windows.ApplicationModel.Background.BackgroundTaskBuilder;

namespace SimpleWeather.NET.BackgroundTasks
{
    internal static class BackgroundTaskUtils
    {
        public static void SetTaskEntryPoint(this BackgroundTaskBuilder backgroundTaskBuilder)
        {
            backgroundTaskBuilder.SetTaskEntryPointClsid(typeof(InProcBackgroundTask).GUID);
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
