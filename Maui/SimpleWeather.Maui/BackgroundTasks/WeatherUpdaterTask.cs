#if __IOS__ || __MACCATALYST__
using BackgroundTasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.BackgroundTasks;
using SimpleWeather.Common.Location;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.Notifications;
using SimpleWeather.Maui.WeatherAlerts;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System.Collections.ObjectModel;
using UIKit;

namespace SimpleWeather.Maui.BackgroundTasks
{
    public class WeatherUpdaterTask : IBackgroundTask
    {
        private const string taskName = nameof(WeatherUpdaterTask);
        public const string TASK_ID = $"SimpleWeather.{taskName}";
        private readonly CancellationTokenSource cts = new();

        public bool IsCancelled => cts.IsCancellationRequested;

        public event EventHandler TaskCompleted;

        private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private WeatherUpdaterTask() { }

        public void Cancel()
        {
            cts.Cancel();
        }

        public async Task Run()
        {
            try
            {
                Logger.WriteLine(LoggerLevel.Debug, $"{taskName}: Run...");

                var locationChanged = false;

                this.RunCatching(async () =>
                {
                    var remoteConfigService = Ioc.Default.GetService<IRemoteConfigService>();
                    await remoteConfigService.CheckConfig();
                });

                if (SettingsManager.WeatherLoaded)
                {
                    if (SettingsManager.FollowGPS)
                    {
                        Logger.WriteLine(LoggerLevel.Debug, $"{taskName}: Updating location...");

                        try
                        {
                            var result = await UpdateLocation();

                            switch (result)
                            {
                                case LocationResult.Changed ret:
                                    locationChanged = true;
                                    await SettingsManager.SaveLastGPSLocData(ret.Data);
                                    break;
                                default:
                                    // no-op
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLine(LoggerLevel.Error, e);
                        }
                    }

                    if (cts.IsCancellationRequested) return;

                    Logger.WriteLine(LoggerLevel.Debug, $"{taskName}: Refreshing weather for tiles...");

                    // Refresh weather for tiles
                    await PreloadWeather();

                    Logger.WriteLine(LoggerLevel.Debug, $"{taskName}: Getting weather data...");
                    // Retrieve weather data.
                    var weather = await GetWeather();

                    if (cts.IsCancellationRequested) return;

                    // TODO: update widgets

                    if (SettingsManager.PoPChanceNotificationEnabled)
                    {
                        await PoPNotificationCreator.CreateNotification(await SettingsManager.GetHomeData());
                    }

                    // TODO: update shortcuts

                    if (weather != null)
                    {
                        // Post alerts if setting is on
                        if (SettingsManager.ShowAlerts && wm.SupportsAlerts)
                        {
                            await WeatherAlertHandler.PostAlerts(await SettingsManager.GetHomeData(), weather.weather_alerts);
                        }

                        // Set update time
                        SettingsManager.UpdateTime = DateTime.Now;

                        if (locationChanged)
                        {
                            SharedModule.Instance.RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE,
                                new Dictionary<string, object>
                                {
                                    { CommonActions.EXTRA_FORCEUPDATE, false }
                                });
                        }
                    }
                }

                Logger.WriteLine(LoggerLevel.Debug, $"{taskName}: End of run...");
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, $"{taskName}: exception occurred...");
            }
            finally
            {
                TaskCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <exception cref="WeatherException">Ignore.</exception>
        private async Task<Weather> GetWeather()
        {
            Logger.WriteLine(LoggerLevel.Info, $"{taskName}: Getting weather data for home...");

            Weather weather;

            try
            {
                cts.Token.ThrowIfCancellationRequested();

                weather = await new WeatherDataLoader(await SettingsManager.GetHomeData())
                        .LoadWeatherData(new WeatherRequest.Builder()
                            .ForceRefresh(false)
                            .LoadAlerts()
                            .LoadForecasts()
                            .Build()
                        );
            }
            catch (OperationCanceledException cancelEx)
            {
                Logger.WriteLine(LoggerLevel.Info, cancelEx, $"{taskName}: GetWeather cancelled");
                return null;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, $"{taskName}: GetWeather error");
                return null;
            }

            return weather;
        }

        /// <exception cref="WeatherException">Ignore.</exception>
        private async Task PreloadWeather()
        {
            var locations = await SettingsManager.GetFavorites() ?? new Collection<LocationData.LocationData>();

            Logger.WriteLine(LoggerLevel.Info, $"{taskName}: Preloading weather data for favorites...");

            foreach (var location in locations)
            {
                // WidgetUtils.Exists(location.query)
                if (false)
                {
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        await new WeatherDataLoader(location)
                            .LoadWeatherData(new WeatherRequest.Builder()
                                .ForceRefresh(false)
                                .LoadAlerts()
                                .Build()
                            );
                    }
                    catch (OperationCanceledException cancelEx)
                    {
                        Logger.WriteLine(LoggerLevel.Info, cancelEx, $"{taskName}: PreloadWeather cancelled");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, $"{taskName}: PreloadWeather error");
                    }
                }
            }
        }

        private async Task<LocationResult> UpdateLocation()
        {
            var locationProvider = new LocationProvider();

            if (SettingsManager.FollowGPS)
            {
                if (await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() != PermissionStatus.Granted)
                {
                    return new LocationResult.PermissionDenied();
                }

                var lastLocation = await SettingsManager.GetLastGPSLocData();
                return await locationProvider.GetLatestLocationData(lastLocation);
            }

            return new LocationResult.NotChanged(null);
        }

        public static void RegisterTask()
        {
            BGTaskScheduler.Shared.Register(TASK_ID, null, (task) =>
            {
                HandleTaskRegistration(task as BGProcessingTask);
            });
        }

        public static void UpdateWeather()
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Requesting to start work immediately", taskName);

            try
            {
                var cts = new CancellationTokenSource();
                var id = UIKit.UIApplication.SharedApplication.BeginBackgroundTask(cts.Cancel);

                if (id != UIApplication.BackgroundTaskInvalid)
                {
                    Task.Run(new WeatherUpdaterTask().Run, cts.Token).ContinueWith(t =>
                    {
                        UIApplication.SharedApplication.EndBackgroundTask(id);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e);
            }
        }

        public static void CancelPendingTasks()
        {
            BGTaskScheduler.Shared.Cancel(TASK_ID);
        }

        private static void HandleTaskRegistration(BGProcessingTask task)
        {
            // Schedule a new refresh task.
            ScheduleTask();

            var taskOp = new WeatherUpdaterTask();

            task.ExpirationHandler = taskOp.Cancel;

            taskOp.TaskCompleted += (s, e) =>
            {
                task.SetTaskCompleted(!taskOp.IsCancelled);
            };

            Task.Run(taskOp.Run);
        }

        public static void ScheduleTask()
        {
            var settingsMgr = Ioc.Default.GetService<SettingsManager>();

            var request = new BGProcessingTaskRequest(TASK_ID)
            {
                EarliestBeginDate = Foundation.NSDate.FromTimeIntervalSinceNow(TimeSpan.FromMinutes(settingsMgr.RefreshInterval).TotalSeconds)
            };

            try
            {
                BGTaskScheduler.Shared.Submit(request, out var error);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, $"{taskName}: Unable to register task");
            }
        }
    }
}
#endif
