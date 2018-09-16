using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SimpleWeather.Droid.App.Widgets;
using SimpleWeather.Droid.Helpers;
using SimpleWeather.Utils;

namespace SimpleWeather.Droid.App
{
    [Service(Enabled = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    [IntentFilter(new string[]
    {
        DataApi.ActionDataChanged,
        MessageApi.ActionMessageReceived,
        CapabilityApi.ActionCapabilityChanged
    },
        DataScheme = "wear", DataHost = "*")]
    public class WearableDataListenerService : WearableListenerService,
        GoogleApiClient.IOnConnectionFailedListener,
        GoogleApiClient.IConnectionCallbacks
    {
        private const String TAG = nameof(WearableDataListenerService);

        // Actions
        public const String ACTION_SENDSETTINGSUPDATE = "SimpleWeather.Droid.action.SEND_SETTINGS_UPDATE";
        public const String ACTION_SENDLOCATIONUPDATE = "SimpleWeather.Droid.action.SEND_LOCATION_UPDATE";
        public const String ACTION_SENDWEATHERUPDATE = "SimpleWeather.Droid.action.SEND_WEATHER_UPDATE";

        private GoogleApiClient mGoogleApiClient;
        private ICollection<INode> mWearNodesWithApp;
        private ICollection<INode> mAllConnectedNodes;
        private bool Loaded = false;

        private const int JOB_ID = 1002;

        public static void EnqueueWork(Context context, Intent work)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var jb = new Android.App.Job.JobInfo.Builder(JOB_ID,
                    new ComponentName(context, Java.Lang.Class.FromType(typeof(WearableDataListenerService))));
                jb.SetOverrideDeadline(0);

                var js = (Android.App.Job.JobScheduler)context.ApplicationContext
                    .GetSystemService(Context.JobSchedulerService);
                js.Enqueue(jb.Build(), new Android.App.Job.JobWorkItem(work));
            }
            else
            {
                context.StartService(work);
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();

            if (mGoogleApiClient == null)
            {
                mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .AddApi(WearableClass.API)
                    .AddConnectionCallbacks(this)
                    .AddOnConnectionFailedListener(this)
                    .Build();
            }

            if (!mGoogleApiClient.IsConnected)
                mGoogleApiClient.Connect();

            var oldHandler = Java.Lang.Thread.DefaultUncaughtExceptionHandler;

            Java.Lang.Thread.DefaultUncaughtExceptionHandler =
                new UncaughtExceptionHandler((thread, throwable) =>
                {
                    Logger.WriteLine(LoggerLevel.Error, throwable, "SimpleWeather: {0}: UncaughtException", TAG);

                    if (oldHandler != null)
                    {
                        oldHandler.UncaughtException(thread, throwable);
                    }
                    else
                    {
                        Java.Lang.JavaSystem.Exit(2);
                    }
                });
        }

        public void OnConnected(Bundle connectionHint)
        {
            Task.Run(async () =>
            {
                await WearableClass.CapabilityApi.AddCapabilityListenerAsync(
                    mGoogleApiClient,
                    this,
                    WearableHelper.CAPABILITY_WEAR_APP);

                mWearNodesWithApp = await FindWearDevicesWithApp();
                mAllConnectedNodes = await FindAllWearDevices();

                Loaded = true;
            });
        }

        public override void OnDestroy()
        {
            Task.Run(async () =>
            {
                if ((mGoogleApiClient != null) && mGoogleApiClient.IsConnected)
                {
                    await WearableClass.CapabilityApi.RemoveCapabilityListenerAsync(
                        mGoogleApiClient,
                        this,
                        WearableHelper.CAPABILITY_WEAR_APP);

                    mGoogleApiClient.Disconnect();
                }
                Loaded = false;
            });

            base.OnDestroy();
        }

        public void OnConnectionSuspended(int cause)
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: onConnectionSuspended(): connection to location client suspended: " + cause, TAG);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: onConnectionFailed(): " + result, TAG);
        }

        public override void OnDataChanged(DataEventBuffer dataEvents)
        {
            base.OnDataChanged(dataEvents);
        }

        public override void OnMessageReceived(IMessageEvent messageEvent)
        {
            if (messageEvent.Path.Equals(WearableHelper.StartActivityPath))
            {
                Intent startIntent = new Intent(this, typeof(LaunchActivity))
                    .SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(startIntent);
            }
            else if (messageEvent.Path.Equals(WearableHelper.SettingsPath))
            {
                Task.Run(async () => await CreateSettingsDataRequest(true));
            }
            else if (messageEvent.Path.Equals(WearableHelper.LocationPath))
            {
                Task.Run(async () => await CreateLocationDataRequest(true));
            }
            else if (messageEvent.Path.Equals(WearableHelper.WeatherPath))
            {
                var data = messageEvent.GetData();
                bool force = false;
                if (data != null && data.Length > 0)
                    force = BitConverter.ToBoolean(data, 0);

                if (!force)
                {
                    Task.Run(async () => await CreateWeatherDataRequest(true));
                }
                else
                {
                    // Refresh weather data
                    WeatherWidgetService.EnqueueWork(this, new Intent(this, typeof(WeatherWidgetService))
                        .SetAction(WeatherWidgetService.ACTION_UPDATEWEATHER));
                }
            }
            else if (messageEvent.Path.Equals(WearableHelper.IsSetupPath))
            {
                SendSetupStatus(messageEvent.SourceNodeId);
            }
        }

        public override void OnCapabilityChanged(ICapabilityInfo capabilityInfo)
        {
            Task.Run(async () =>
            {
                mWearNodesWithApp = capabilityInfo.Nodes;
                mAllConnectedNodes = await FindAllWearDevices();
            });
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            // Create requests if nodes exist with app support
            if (ACTION_SENDSETTINGSUPDATE.Equals(intent?.Action))
            {
                Task.Run(async () => await CreateSettingsDataRequest(true));
                return StartCommandResult.NotSticky;
            }
            else if (ACTION_SENDLOCATIONUPDATE.Equals(intent?.Action))
            {
                Task.Run(async () => await CreateLocationDataRequest(true));
                return StartCommandResult.NotSticky;
            }
            else if (ACTION_SENDWEATHERUPDATE.Equals(intent?.Action))
            {
                Task.Run(async () => await CreateWeatherDataRequest(true));
                return StartCommandResult.NotSticky;
            }

            return base.OnStartCommand(intent, flags, startId);
        }

        private async Task<ICollection<INode>> FindWearDevicesWithApp()
        {
            var getCapabilityResult = await WearableClass.CapabilityApi.GetCapabilityAsync(
                mGoogleApiClient,
                WearableHelper.CAPABILITY_WEAR_APP,
                CapabilityApi.FilterAll);

            if (getCapabilityResult != null && getCapabilityResult.Status.IsSuccess)
            {
                var capabilityInfo = getCapabilityResult.Capability;
                return capabilityInfo.Nodes;
            }

            return null;
        }

        private async Task<ICollection<INode>> FindAllWearDevices()
        {
            var getConnectedNodesResult = await WearableClass.NodeApi
                .GetConnectedNodesAsync(mGoogleApiClient);

            if (getConnectedNodesResult != null && getConnectedNodesResult.Status.IsSuccess)
            {
                return getConnectedNodesResult.Nodes;
            }

            return null;
        }

        private async Task CreateSettingsDataRequest(bool urgent)
        {
            // Don't send anything unless we're setup
            if (!Settings.WeatherLoaded)
                return;

            if (mWearNodesWithApp == null)
            {
                // Create requests if nodes exist with app support
                mWearNodesWithApp = await FindWearDevicesWithApp();

                if (mWearNodesWithApp == null || mWearNodesWithApp.Count == 0)
                    return;
            }

            var mapRequest = PutDataMapRequest.Create(WearableHelper.SettingsPath);
            mapRequest.DataMap.PutString("API", Settings.API);
            mapRequest.DataMap.PutString("API_KEY", Settings.API_KEY);
            mapRequest.DataMap.PutBoolean("KeyVerified", Settings.KeyVerified);
            mapRequest.DataMap.PutBoolean("FollowGPS", Settings.FollowGPS);
            mapRequest.DataMap.PutLong("update_time", DateTime.UtcNow.Ticks);
            var request = mapRequest.AsPutDataRequest();
            if (urgent) request.SetUrgent();
            await WearableClass.DataApi.PutDataItem(mGoogleApiClient, request);

            Logger.WriteLine(LoggerLevel.Info, "{0}: CreateSettingsDataRequest(): urgent: {1}", TAG, urgent.ToString());
        }

        private async Task CreateLocationDataRequest(bool urgent)
        {
            // Don't send anything unless we're setup
            if (!Settings.WeatherLoaded)
                return;

            if (mWearNodesWithApp == null)
            {
                // Create requests if nodes exist with app support
                mWearNodesWithApp = await FindWearDevicesWithApp();

                if (mWearNodesWithApp == null || mWearNodesWithApp.Count == 0)
                    return;
            }

            PutDataMapRequest mapRequest = PutDataMapRequest.Create(WearableHelper.LocationPath);
            var homeData = Settings.HomeData;
            mapRequest.DataMap.PutString("locationData", homeData?.ToJson());
            mapRequest.DataMap.PutLong("update_time", DateTime.UtcNow.Ticks);
            PutDataRequest request = mapRequest.AsPutDataRequest();
            if (urgent) request.SetUrgent();
            await WearableClass.DataApi.PutDataItem(mGoogleApiClient, request);

            Logger.WriteLine(LoggerLevel.Info, "{0}: CreateLocationDataRequest(): urgent: {1}", TAG, urgent.ToString());
        }

        private async Task CreateWeatherDataRequest(bool urgent)
        {
            // Don't send anything unless we're setup
            if (!Settings.WeatherLoaded)
                return;

            if (mWearNodesWithApp == null)
            {
                // Create requests if nodes exist with app support
                mWearNodesWithApp = await FindWearDevicesWithApp();

                if (mWearNodesWithApp == null || mWearNodesWithApp.Count == 0)
                    return;
            }

            PutDataMapRequest mapRequest = PutDataMapRequest.Create(WearableHelper.WeatherPath);
            var homeData = Settings.HomeData;
            var weatherData = await Settings.GetWeatherData(homeData.query);
            var alertData = await Settings.GetWeatherAlertData(homeData.query);

            if (weatherData != null)
            {
                weatherData.weather_alerts = alertData;

                // location
                // update_time
                // forecast
                // hr_forecast
                // txt_forecast
                // condition
                // atmosphere
                // astronomy
                // precipitation
                // ttl
                // source
                // query
                // locale

                mapRequest.DataMap.PutString("weatherData", weatherData?.ToJson());
                List<String> alerts = new List<String>();
                if (weatherData.weather_alerts.Count > 0)
                {
                    foreach (WeatherData.WeatherAlert alert in weatherData.weather_alerts)
                    {
                        alerts.Add(alert.ToJson());
                    }
                }
                mapRequest.DataMap.PutStringArrayList("weatherAlerts", alerts);
                mapRequest.DataMap.PutLong("update_time", weatherData.update_time.UtcTicks);
                PutDataRequest request = mapRequest.AsPutDataRequest();
                if (urgent) request.SetUrgent();
                await WearableClass.DataApi.PutDataItem(mGoogleApiClient, request);

                Logger.WriteLine(LoggerLevel.Info, "{0}: CreateWeatherDataRequest(): urgent: {1}", TAG, urgent.ToString());
            }
        }

        private void SendSetupStatus(String NodeID)
        {
            WearableClass.MessageApi.SendMessage(mGoogleApiClient, NodeID, WearableHelper.IsSetupPath,
                BitConverter.GetBytes(Settings.WeatherLoaded));
        }
    }
}