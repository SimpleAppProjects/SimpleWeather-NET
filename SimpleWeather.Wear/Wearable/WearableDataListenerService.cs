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
using Android.Support.V4.Content;
using Android.Support.Wearable.Activity;
using Android.Support.Wearable.Phone;
using Android.Support.Wearable.Views;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Wearable.Intent;
using SimpleWeather.Droid.Wear.Helpers;
using SimpleWeather.Utils;

namespace SimpleWeather.Droid.Wear
{
    [Service(Enabled = true)]
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
        private const String TAG = "WearableDataListenerService";

        // Actions
        public const String ACTION_OPENONPHONE = "SimpleWeather.Droid.Wear.action.OPEN_APP_ON_PHONE";
        public const String ACTION_SHOWSTORELISTING = "SimpleWeather.Droid.Wear.action.SHOW_STORE_LISTING";
        public const String ACTION_UPDATECONNECTIONSTATUS = "SimpleWeather.Droid.Wear.action.UPDATE_CONNECTION_STATUS";
        public const String ACTION_REQUESTSETTINGSUPDATE = "SimpleWeather.Droid.Wear.action.REQUEST_SETTINGS_UPDATE";
        public const String ACTION_REQUESTLOCATIONUPDATE = "SimpleWeather.Droid.Wear.action.REQUEST_LOCATION_UPDATE";
        public const String ACTION_REQUESTWEATHERUPDATE = "SimpleWeather.Droid.Wear.action.REQUEST_WEATHER_UPDATE";
        public const String ACTION_REQUESTSETUPSTATUS = "SimpleWeather.Droid.Wear.action.REQUEST_SETUP_STATUS";

        // Extras
        public const String EXTRA_SUCCESS = "SimpleWeather.Droid.Wear.extra.SUCCESS";
        public const String EXTRA_CONNECTIONSTATUS = "SimpleWeather.Droid.Wear.extra.CONNECTION_STATUS";
        public const String EXTRA_DEVICESETUPSTATUS = "SimpleWeather.Droid.Wear.extra.DEVICE_SETUP_STATUS";
        public const String EXTRA_FORCEUPDATE = "SimpleWeather.Droid.Wear.extra.FORCE_UPDATE";

        private GoogleApiClient mGoogleApiClient;
        private INode mPhoneNodeWithApp;
        private WearConnectionStatus mConnectionStatus = WearConnectionStatus.Disconnected;
        private bool Loaded = false;

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
        }

        public async void OnConnected(Bundle connectionHint)
        {
            await WearableClass.CapabilityApi.AddCapabilityListenerAsync(
                mGoogleApiClient,
                this,
                WearableHelper.CAPABILITY_PHONE_APP);
            await WearableClass.DataApi.AddListenerAsync(mGoogleApiClient, this);
            await WearableClass.MessageApi.AddListenerAsync(mGoogleApiClient, this);

            mPhoneNodeWithApp = await CheckIfPhoneHasApp();

            if (mPhoneNodeWithApp == null)
                mConnectionStatus = WearConnectionStatus.AppNotInstalled;
            else
                mConnectionStatus = WearConnectionStatus.Connected;

            LocalBroadcastManager.GetInstance(this)
                .SendBroadcast(new Intent(ACTION_UPDATECONNECTIONSTATUS)
                    .PutExtra(EXTRA_CONNECTIONSTATUS, (int)mConnectionStatus));

            Loaded = true;
        }

        public override async void OnDestroy()
        {
            if ((mGoogleApiClient != null) && mGoogleApiClient.IsConnected)
            {
                await WearableClass.CapabilityApi.RemoveCapabilityListenerAsync(
                    mGoogleApiClient,
                    this,
                    WearableHelper.CAPABILITY_PHONE_APP);
                await WearableClass.DataApi.RemoveListenerAsync(mGoogleApiClient, this);
                await WearableClass.MessageApi.RemoveListenerAsync(mGoogleApiClient, this);

                mGoogleApiClient.Disconnect();
            }
            Loaded = false;

            base.OnDestroy();
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Debug(TAG, "onConnectionSuspended(): connection to location client suspended: " + cause);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Error(TAG, "onConnectionFailed(): " + result);

            mConnectionStatus = WearConnectionStatus.Disconnected;

            LocalBroadcastManager.GetInstance(this)
                .SendBroadcast(new Intent(ACTION_UPDATECONNECTIONSTATUS)
                    .PutExtra(EXTRA_CONNECTIONSTATUS, (int)mConnectionStatus));
        }

        public override void OnDataChanged(DataEventBuffer dataEvents)
        {
            foreach (IDataEvent @event in dataEvents)
            {
                if (@event.Type == DataEvent.TypeChanged)
                {
                    IDataItem item = @event.DataItem;
                    if (item.Uri.Path.CompareTo(WearableHelper.SettingsPath) == 0)
                    {
                        DataMap dataMap = DataMapItem.FromDataItem(item).DataMap;
                        UpdateSettings(dataMap);
                    }
                    else if (item.Uri.Path.CompareTo(WearableHelper.LocationPath) == 0)
                    {
                        DataMap dataMap = DataMapItem.FromDataItem(item).DataMap;
                        UpdateLocation(dataMap);
                    }
                    else if (item.Uri.Path.CompareTo(WearableHelper.WeatherPath) == 0)
                    {
                        DataMap dataMap = DataMapItem.FromDataItem(item).DataMap;
                        UpdateWeather(dataMap);
                    }
                }
            }
        }

        public override void OnMessageReceived(IMessageEvent messageEvent)
        {
            base.OnMessageReceived(messageEvent);

            if (messageEvent.Path.Equals(WearableHelper.ErrorPath))
            {
                LocalBroadcastManager.GetInstance(this)
                    .SendBroadcast(new Intent(WearableHelper.ErrorPath));
            }
            else if (messageEvent.Path.Equals(WearableHelper.IsSetupPath))
            {
                var data = messageEvent.GetData();
                bool isDeviceSetup = BitConverter.ToBoolean(data, 0);
                LocalBroadcastManager.GetInstance(this)
                    .SendBroadcast(new Intent(WearableHelper.IsSetupPath)
                        .PutExtra(EXTRA_DEVICESETUPSTATUS, isDeviceSetup)
                        .PutExtra(EXTRA_CONNECTIONSTATUS, (int)mConnectionStatus));
            }
        }

        public override void OnCapabilityChanged(ICapabilityInfo capabilityInfo)
        {
            base.OnCapabilityChanged(capabilityInfo);

            mPhoneNodeWithApp = PickBestNodeId(capabilityInfo.Nodes);

            if (mPhoneNodeWithApp == null)
                mConnectionStatus = WearConnectionStatus.AppNotInstalled;
            else
                mConnectionStatus = WearConnectionStatus.Connected;

            LocalBroadcastManager.GetInstance(this)
                .SendBroadcast(new Intent(ACTION_UPDATECONNECTIONSTATUS)
                    .PutExtra(EXTRA_CONNECTIONSTATUS, (int)mConnectionStatus));
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (ACTION_OPENONPHONE.Equals(intent?.Action))
            {
                OpenAppOnPhone();
                return StartCommandResult.NotSticky;
            }
            else if (ACTION_UPDATECONNECTIONSTATUS.Equals(intent?.Action))
            {
                LocalBroadcastManager.GetInstance(this)
                    .SendBroadcast(new Intent(ACTION_UPDATECONNECTIONSTATUS)
                        .PutExtra(EXTRA_CONNECTIONSTATUS, (int)mConnectionStatus));
                return StartCommandResult.NotSticky;
            }
            else if (ACTION_REQUESTSETTINGSUPDATE.Equals(intent?.Action))
            {
                SendSettingsRequest();
                return StartCommandResult.NotSticky;
            }
            else if (ACTION_REQUESTLOCATIONUPDATE.Equals(intent?.Action))
            {
                SendLocationRequest();
                return StartCommandResult.NotSticky;
            }
            else if (ACTION_REQUESTWEATHERUPDATE.Equals(intent?.Action))
            {
                bool forceUpdate = (bool)intent?.GetBooleanExtra(EXTRA_FORCEUPDATE, false);
                if (!forceUpdate)
                    SendWeatherRequest();
                else
                    SendWeatherUpdateRequest();
                return StartCommandResult.NotSticky;
            }
            else if (ACTION_REQUESTSETUPSTATUS.Equals(intent?.Action))
            {
                SendSetupStatusRequest();
                return StartCommandResult.NotSticky;
            }

            return base.OnStartCommand(intent, flags, startId);
        }

        private async Task<INode> CheckIfPhoneHasApp()
        {
            var getCapabilityResult = await WearableClass.CapabilityApi.GetCapabilityAsync(
                mGoogleApiClient,
                WearableHelper.CAPABILITY_PHONE_APP,
                CapabilityApi.FilterAll);

            if (getCapabilityResult != null && getCapabilityResult.Status.IsSuccess)
            {
                var capabilityInfo = getCapabilityResult.Capability;
                return PickBestNodeId(capabilityInfo.Nodes);
            }

            return null;
        }

        /*
         * There should only ever be one phone in a node set (much less w/ the correct capability), so
         * I am just grabbing the first one (which should be the only one).
         */
        private INode PickBestNodeId(ICollection<INode> nodes)
        {
            INode bestNodeId = null;
            // Find a nearby node/phone or pick one arbitrarily. Realistically, there is only one phone.
            foreach (INode node in nodes)
            {
                bestNodeId = node;
            }
            return bestNodeId;
        }

        private async Task<bool> Connect()
        {
            if (mGoogleApiClient.IsConnecting)
            {
                while (mGoogleApiClient.IsConnecting)
                    await Task.Delay(250);
            }
            else if (!mGoogleApiClient.IsConnected)
            {
                mGoogleApiClient.Connect();
                while (mGoogleApiClient.IsConnecting)
                    await Task.Delay(250);
            }

            if (!Loaded && mPhoneNodeWithApp == null)
                mPhoneNodeWithApp = await CheckIfPhoneHasApp();

            if (mPhoneNodeWithApp == null)
                return false;
            else
                return true;
        }

        private async void OpenAppOnPhone()
        {
            await Connect();

            if (mPhoneNodeWithApp == null)
            {
                Toast.MakeText(this, "Device is not connected or app is not installed on device...", ToastLength.Short).Show();

                int deviceType = PhoneDeviceType.GetPhoneDeviceType(this);
                switch (deviceType)
                {
                    case PhoneDeviceType.DeviceTypeAndroid:
                        LocalBroadcastManager.GetInstance(this).SendBroadcast(
                            new Intent(ACTION_SHOWSTORELISTING));
                        break;
                    case PhoneDeviceType.DeviceTypeIos:
                    default:
                        Toast.MakeText(this, "Connected device is not supported", ToastLength.Short).Show();
                        break;
                }
            }
            else
            {
                // Send message to device to start activity
                var sendMessageResult = await WearableClass.MessageApi.SendMessageAsync(mGoogleApiClient, mPhoneNodeWithApp.Id,
                    WearableHelper.StartActivityPath, new byte[0]);

                LocalBroadcastManager.GetInstance(this)
                    .SendBroadcast(new Intent(ACTION_OPENONPHONE)
                        .PutExtra(EXTRA_SUCCESS, sendMessageResult.Status.IsSuccess));
            }
        }

        private async void SendSettingsRequest()
        {
            if (!(await Connect()))
            {
                LocalBroadcastManager.GetInstance(this).SendBroadcast(
                    new Intent(WearableHelper.ErrorPath));
                return;
            }

            IDataApiDataItemResult data = await WearableClass.DataApi.GetDataItemAsync(
                mGoogleApiClient, WearableHelper.GetWearDataUri(mPhoneNodeWithApp.Id, WearableHelper.SettingsPath));

            if (!data.Status.IsSuccess || data.DataItem == null)
            {
                // Send message to device to get settings
                var sendMessageResult = await WearableClass.MessageApi.SendMessageAsync(mGoogleApiClient, mPhoneNodeWithApp.Id,
                    WearableHelper.SettingsPath, new byte[0]);
            }
            else
            {
                // Update with data
                UpdateSettings(DataMapItem.FromDataItem(data.DataItem).DataMap);
            }
        }

        private async void SendLocationRequest()
        {
            if (!(await Connect()))
            {
                LocalBroadcastManager.GetInstance(this).SendBroadcast(
                    new Intent(WearableHelper.ErrorPath));
                return;
            }

            IDataApiDataItemResult data = await WearableClass.DataApi.GetDataItemAsync(
                mGoogleApiClient, WearableHelper.GetWearDataUri(mPhoneNodeWithApp.Id, WearableHelper.LocationPath));

            if (!data.Status.IsSuccess || data.DataItem == null)
            {
                // Send message to device to get settings
                var sendMessageResult = await WearableClass.MessageApi.SendMessageAsync(mGoogleApiClient, mPhoneNodeWithApp.Id,
                    WearableHelper.LocationPath, new byte[0]);
            }
            else
            {
                // Update with data
                UpdateLocation(DataMapItem.FromDataItem(data.DataItem).DataMap);
            }
        }

        private async void SendWeatherRequest()
        {
            if (!(await Connect()))
            {
                LocalBroadcastManager.GetInstance(this).SendBroadcast(
                    new Intent(WearableHelper.ErrorPath));
                return;
            }

            IDataApiDataItemResult data = await WearableClass.DataApi.GetDataItemAsync(
                mGoogleApiClient, WearableHelper.GetWearDataUri(mPhoneNodeWithApp.Id, WearableHelper.WeatherPath));

            if (!data.Status.IsSuccess || data.DataItem == null)
            {
                // Send message to device to get settings
                var sendMessageResult = await WearableClass.MessageApi.SendMessageAsync(mGoogleApiClient, mPhoneNodeWithApp.Id,
                    WearableHelper.WeatherPath, new byte[0]);
            }
            else
            {
                // Update with data
                UpdateWeather(DataMapItem.FromDataItem(data.DataItem).DataMap);
            }
        }

        private async void SendWeatherUpdateRequest()
        {
            if (!(await Connect()))
            {
                LocalBroadcastManager.GetInstance(this).SendBroadcast(
                    new Intent(WearableHelper.ErrorPath));
                return;
            }

            // Send message to device to get settings
            var sendMessageResult = await WearableClass.MessageApi.SendMessageAsync(mGoogleApiClient, mPhoneNodeWithApp.Id,
                WearableHelper.WeatherPath, BitConverter.GetBytes(true));
        }

        private async void SendSetupStatusRequest()
        {
            if (!(await Connect()))
            {
                LocalBroadcastManager.GetInstance(this).SendBroadcast(
                    new Intent(WearableHelper.ErrorPath));
                return;
            }

            await WearableClass.MessageApi.SendMessageAsync(mGoogleApiClient, mPhoneNodeWithApp.Id, WearableHelper.IsSetupPath, new byte[0]);
        }

        private void UpdateSettings(DataMap dataMap)
        {
            if (dataMap != null && !dataMap.IsEmpty)
            {
                var API = dataMap.GetString("API", String.Empty);
                var API_KEY = dataMap.GetString("API_KEY", String.Empty);
                var KeyVerified = dataMap.GetBoolean("KeyVerified", false);
                if (!String.IsNullOrWhiteSpace(API))
                {
                    Settings.API = API;
                    if (WeatherData.WeatherManager.IsKeyRequired(API))
                    {
                        Settings.API_KEY = API_KEY;
                        Settings.KeyVerified = KeyVerified;
                    }
                    else
                    {
                        Settings.API_KEY = String.Empty;
                        Settings.KeyVerified = false;
                    }
                }

                var FollowGPS = dataMap.GetBoolean("FollowGPS", false);
                Settings.FollowGPS = FollowGPS;

                // Send callback to receiver
                LocalBroadcastManager.GetInstance(this).SendBroadcast(
                    new Intent(WearableHelper.SettingsPath));
            }
        }

        private void UpdateLocation(DataMap dataMap)
        {
            if (dataMap != null && !dataMap.IsEmpty)
            {
                var locationJSON = dataMap.GetString("locationData", String.Empty);
                if (!String.IsNullOrWhiteSpace(locationJSON))
                {
                    var locationData = WeatherData.LocationData.FromJson(
                        new Newtonsoft.Json.JsonTextReader(
                            new System.IO.StringReader(locationJSON)));

                    if (locationData != null)
                    {
                        if (!locationData.Equals(Settings.HomeData))
                        {
                            Settings.SaveHomeData(locationData);
                        }

                        // Send callback to receiver
                        LocalBroadcastManager.GetInstance(this).SendBroadcast(
                            new Intent(WearableHelper.LocationPath));
                    }
                }
            }
        }

        private async void UpdateWeather(DataMap dataMap)
        {
            if (dataMap != null && !dataMap.IsEmpty)
            {
                var update_time = dataMap.GetLong("update_time", 0);
                if (update_time != 0)
                {
                    if (Settings.HomeData is WeatherData.LocationData location)
                    {
                        var upDateTime = new DateTime(update_time, DateTimeKind.Utc);
                        /*
                            DateTime < 0 - This instance is earlier than value.
                            DateTime == 0 - This instance is the same as value.
                            DateTime > 0 - This instance is later than value.
                        */
                        if (Settings.UpdateTime.CompareTo(upDateTime) >= 0)
                        {
                            // Send callback to receiver
                            LocalBroadcastManager.GetInstance(this).SendBroadcast(
                                new Intent(WearableHelper.WeatherPath));
                            return;
                        }
                    }
                }

                var weatherJSON = dataMap.GetString("weatherData", String.Empty);
                if (!String.IsNullOrWhiteSpace(weatherJSON))
                {
                    var weatherData = WeatherData.Weather.FromJson(
                        new Newtonsoft.Json.JsonTextReader(
                            new System.IO.StringReader(weatherJSON)));
                    var alerts = dataMap.GetStringArrayList("weatherAlerts");

                    if (weatherData != null && weatherData.IsValid())
                    {
                        if (alerts.Count > 0)
                        {
                            weatherData.weather_alerts = new List<WeatherData.WeatherAlert>();
                            foreach(String alertJSON in alerts)
                            {
                                var alert = WeatherData.WeatherAlert.FromJson(
                                    new Newtonsoft.Json.JsonTextReader(
                                        new System.IO.StringReader(alertJSON)));

                                if (alert != null)
                                    weatherData.weather_alerts.Add(alert);
                            }
                        }

                        await Settings.SaveWeatherAlerts(Settings.HomeData, weatherData.weather_alerts);
                        await Settings.SaveWeatherData(weatherData);
                        Settings.UpdateTime = weatherData.update_time.UtcDateTime;

                        // Send callback to receiver
                        LocalBroadcastManager.GetInstance(this).SendBroadcast(
                            new Intent(WearableHelper.WeatherPath));
                    }
                }
            }
        }
    }
}