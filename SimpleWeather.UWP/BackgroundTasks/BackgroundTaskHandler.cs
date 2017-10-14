using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.UWP.BackgroundTasks
{
    public class BackgroundTaskHandler
    {
        private const string taskName = "WeatherUpdateBackgroundTask";
        public ApplicationTrigger AppTrigger = null;

        public BackgroundTaskHandler()
        {
            AppTrigger = new ApplicationTrigger();
        }

        public async void RegisterBackgroundTask()
        {
            // Unregister any previous exising background task
            UnregisterBackgroundTask();

            // Request access
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            // If allowed
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.SetTrigger(new TimeTrigger((uint)Utils.Settings.RefreshInterval, false));
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                taskBuilder.SetTrigger(AppTrigger);
                var registration = taskBuilder.Register();
            }
        }

        public void UnregisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }
        }
    }
}
