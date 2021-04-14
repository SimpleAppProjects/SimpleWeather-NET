using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using SimpleWeather.Utils;
using SimpleWeather.UWP.WNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.UI.Notifications;

namespace SimpleWeather.UWP.BackgroundTasks
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Safety", "UWP001:Platform-specific", Justification = "<Pending>")]
    public sealed class AppUpdaterTask : IBackgroundTask
    {
        private const string taskName = nameof(AppUpdaterTask);
        private StoreContext context = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            await Task.Run(async () =>
            {
                // Run update logic
                Logger.WriteLine(LoggerLevel.Debug, "{0}: running app update check task", taskName);

                await CheckForUpdates();
            });

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private async Task CheckForUpdates()
        {
            if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext") &&
                ApiInformation.IsApiContractPresent("Windows.Services.Store.StoreContext", 4))
            {
                await DownloadAndInstallAllUpdatesInBackgroundAsync();
            }
        }


        private async Task DownloadAndInstallAllUpdatesInBackgroundAsync()
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }

            // Get the updates that are available.
            IReadOnlyList<StorePackageUpdate> storePackageUpdates = null;

            try
            {
                storePackageUpdates = await context.GetAppAndOptionalStorePackageUpdatesAsync();
            }
            catch (FileNotFoundException)
            {
                // Debug: store context does not exist
            }

            if (storePackageUpdates?.Count > 0)
            {
                bool isMandatory = storePackageUpdates.Any(u => u.Mandatory);

                if (!context.CanSilentlyDownloadStorePackageUpdates)
                {
                    // Send toast notification to user
                    if (isMandatory)
                    {
                        CreateUpdateToast();
                    }
                    return;
                }

                // Start the silent downloads and wait for the downloads to complete.
                StorePackageUpdateResult downloadResult =
                    await context.TrySilentDownloadStorePackageUpdatesAsync(storePackageUpdates);

                switch (downloadResult.OverallState)
                {
                    case StorePackageUpdateState.Completed:
                        // The download has completed successfully. At this point, confirm whether your app
                        // can restart now and then install the updates (for example, you might only install
                        // packages silently if your app has been idle for a certain period of time). The
                        // IsNowAGoodTimeToRestartApp method is not implemented in this example, you should
                        // implement it as needed for your own app.
                        if (App.IsInBackground)
                        {
                            await InstallUpdate(storePackageUpdates);
                        }
                        else
                        {
                            // Retry/reschedule the installation later. The RetryInstallLater method is not  
                            // implemented in this example, you should implement it as needed for your own app.
                            if (isMandatory)
                            {
                                CreateUpdateToast();
                                FeatureSettings.IsUpdateAvailable = true;
                            }
                            return;
                        }
                        break;
                    // If the user cancelled the download or you can't perform the download for some other
                    // reason (for example, Wi-Fi might have been turned off and the device is now on
                    // a metered network) try again later. The RetryDownloadAndInstallLater method is not  
                    // implemented in this example, you should implement it as needed for your own app.
                    case StorePackageUpdateState.Canceled:
                    case StorePackageUpdateState.ErrorLowBattery:
                    case StorePackageUpdateState.ErrorWiFiRecommended:
                    case StorePackageUpdateState.ErrorWiFiRequired:
                    case StorePackageUpdateState.OtherError:
                        // Retry when this task runs again
                        if (isMandatory)
                        {
                            FeatureSettings.IsUpdateAvailable = true;
                        }
                        return;
                    default:
                        break;
                }
            }
        }

        private async Task InstallUpdate(IReadOnlyList<StorePackageUpdate> storePackageUpdates)
        {
            bool isMandatory = storePackageUpdates.Any(u => u.Mandatory);

            AnalyticsLogger.LogEvent(taskName + ": InstallUpdate",
                new Dictionary<string, string>()
                {
                    { "isMandatory", isMandatory.ToString() }
                });

            // Start the silent installation of the packages. Because the packages have already
            // been downloaded in the previous method, the following line of code just installs
            // the downloaded packages.
            StorePackageUpdateResult downloadResult =
                await context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);

            switch (downloadResult.OverallState)
            {
                // If the user cancelled the installation or you can't perform the installation  
                // for some other reason, try again later. The RetryInstallLater method is not  
                // implemented in this example, you should implement it as needed for your own app.
                case StorePackageUpdateState.Canceled:
                case StorePackageUpdateState.ErrorLowBattery:
                case StorePackageUpdateState.OtherError:
                    // Retry when this task runs again
                    if (isMandatory)
                    {
                        FeatureSettings.IsUpdateAvailable = true;
                    }
                    return;
                default:
                    break;
            }
        }

        private static void CreateUpdateToast()
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = App.ResLoader.GetString("UpdateAvailablePrompt")
                            }
                        }
                    }
                },
                Audio = new ToastAudio()
                {
                    Silent = true
                },
                Launch = new QueryString()
                {
                    { "action", "check-updates" }
                }
                .ToString()
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml())
            {
                Tag = toastContent.GetHashCode().ToInvariantString()
            };

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static async Task RegisterBackgroundTask(bool reregister = false)
        {
            var taskRegistration = GetTaskRegistration();

            if (taskRegistration != null)
            {
                if (reregister)
                {
                    // Unregister any previous exising background task
                    taskRegistration.Unregister(true);
                }
                else
                {
                    return;
                }
            }

            // Request access
            var backgroundAccessStatus = BackgroundAccessStatus.Unspecified;

            try
            {
                BackgroundExecutionManager.RemoveAccess();
                backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            }
            catch (UnauthorizedAccessException)
            {
                // An access denied exception may be thrown if two requests are issued at the same time
                // For this specific sample, that could be if the user double clicks "Request access"
            }

            // If allowed
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                // Register a task for each trigger
                var taskBuilder = new BackgroundTaskBuilder() { Name = taskName };
                taskBuilder.SetTrigger(new TimeTrigger(1440, false)); // Daily
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh));

                try
                {
                    taskBuilder.Register();
                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2147942583)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "{0}: background task already registered", taskName);
                    }
                    else
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error registering background task", taskName);
                    }
                }
            }
        }

        public static void UnregisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }
        }

        private static IBackgroundTaskRegistration GetTaskRegistration()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return task.Value;
                }
            }

            return null;
        }
    }
}