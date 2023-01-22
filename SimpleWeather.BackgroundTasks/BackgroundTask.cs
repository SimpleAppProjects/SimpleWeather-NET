// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using Windows.ApplicationModel.Background;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.BackgroundTasks
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public const int E_ACCESSDENIED = unchecked((int)0x80070005);
        public const string TASK_ENTRY_POINT = "SimpleWeather.BackgroundTasks.BackgroundTask";

        public BackgroundTask() { }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = null;

            try
            {
                deferral = taskInstance.GetDeferral();

                var backgroundTaskClient = new BackgroundTaskClient();
                backgroundTaskClient.Run(taskInstance);
            }
            catch (Exception e) when (e.HResult == E_ACCESSDENIED)
            {
                // Access Denied happens in Connected Standby,
                // where BG tasks can run but the centennial process cannot.
                // Ignore these errors.
            }
            finally
            {
                if (deferral != null)
                {
                    deferral.Complete();
                }
            }
        }
    }
}
