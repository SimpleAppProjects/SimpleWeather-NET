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
        private ApplicationTrigger AppTrigger = null;

        public BackgroundTaskHandler()
        {
            AppTrigger = new ApplicationTrigger();
        }

        public async Task RequestAppTrigger()
        {
            if (AppTrigger == null)
                AppTrigger = new ApplicationTrigger();

            // Request access
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            // If allowed
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                await AppTrigger.RequestAsync();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("BackgroundTaskHandler: Can't trigger ApplicationTrigger, background access not allowed");
            }
        }

        public async Task RegisterBackgroundTask()
        {
            // Unregister any previous exising background task
            UnregisterBackgroundTask();

            // Request access
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            // If allowed
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                // Register a task for each trigger
                var tb1 = new BackgroundTaskBuilder() { Name = taskName };
                tb1.SetTrigger(new TimeTrigger((uint)Utils.Settings.RefreshInterval, false));
                var tb2 = new BackgroundTaskBuilder() { Name = taskName };
                tb2.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                var tb3 = new BackgroundTaskBuilder() { Name = taskName };
                tb3.SetTrigger(AppTrigger);

                tb1.Register();
                tb2.Register();
                tb3.Register();
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
