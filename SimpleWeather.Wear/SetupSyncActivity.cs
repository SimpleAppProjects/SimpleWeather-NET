using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.Wear.Widget;
using Android.Support.Wearable.Views;
using Android.Views;
using Android.Widget;
using Google.Android.Wearable.Intent;
using SimpleWeather.Droid.Wear.Helpers;

namespace SimpleWeather.Droid.Wear
{
    [Activity(Theme = "@style/WearAppTheme")]
    public class SetupSyncActivity : Activity
    {
        private bool SettingsDataReceived = false;
        private bool LocationDataReceived = false;
        private bool WeatherDataReceived = false;

        private CircularProgressLayout mCircularProgress;
        private TextView mTextView;
        private LocalBroadcastReceiver mBroadcastReceiver;
        private IntentFilter intentFilter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_setup_sync);

            mCircularProgress = FindViewById<CircularProgressLayout>(Resource.Id.circular_progress);
            mCircularProgress.Indeterminate = true;
            mCircularProgress.Click += (sender, e) =>
            {
                // User canceled, abort the action
                mCircularProgress.StopTimer();
                SetResult(Result.Canceled);
                Finish();
            };
            mCircularProgress.TimerFinished += (sender, e) =>
            {
                // User didn't cancel, perform the action
                // All data received finish activity
                if (SettingsDataReceived && LocationDataReceived && WeatherDataReceived)
                    SetResult(Result.Ok);
                else
                    SetResult(Result.Canceled);
                Finish();
            };
            mTextView = FindViewById<TextView>(Resource.Id.message);
            mBroadcastReceiver = new LocalBroadcastReceiver();
            mBroadcastReceiver.BroadcastReceived += BroadcastReceiver_BroadcastReceived;

            mTextView.Text = GetString(Resource.String.message_gettingstatus);

            intentFilter = new IntentFilter();
            intentFilter.AddAction(WearableDataListenerService.ACTION_UPDATECONNECTIONSTATUS);
            intentFilter.AddAction(WearableHelper.IsSetupPath);
            intentFilter.AddAction(WearableHelper.LocationPath);
            intentFilter.AddAction(WearableHelper.SettingsPath);
            intentFilter.AddAction(WearableHelper.WeatherPath);
            intentFilter.AddAction(WearableHelper.ErrorPath);

            StartService(new Intent(this, typeof(WearableDataListenerService))
                .SetAction(WearableDataListenerService.ACTION_UPDATECONNECTIONSTATUS));
        }

        protected override void OnResume()
        {
            base.OnResume();

            LocalBroadcastManager.GetInstance(this)
                .RegisterReceiver(mBroadcastReceiver, intentFilter);
            // Allow service to parse OnDataChanged updates
            WearableDataListenerService.AcceptDataUpdates = true;
        }

        protected override void OnPause()
        {
            LocalBroadcastManager.GetInstance(this)
                .UnregisterReceiver(mBroadcastReceiver);

            SettingsDataReceived = false;
            LocationDataReceived = false;
            WeatherDataReceived = false;

            // Disallow service to parse OnDataChanged updates
            WearableDataListenerService.AcceptDataUpdates = false;

            base.OnPause();
        }

        private void ErrorProgress()
        {
            mCircularProgress.Indeterminate = false;
            mCircularProgress.TotalTime = 5000;
            mCircularProgress.StartTimer();

            SettingsDataReceived = false;
            LocationDataReceived = false;
            WeatherDataReceived = false;
        }

        private void ResetTimer()
        {
            mCircularProgress.StopTimer();
            mCircularProgress.Indeterminate = true;
        }

        private void SuccessProgress()
        {
            mTextView.Text = GetString(Resource.String.message_synccompleted);

            mCircularProgress.Indeterminate = false;
            mCircularProgress.TotalTime = 1;
            mCircularProgress.StartTimer();
        }

        private void BroadcastReceiver_BroadcastReceived(Context context, Intent intent)
        {
            if (WearableDataListenerService.ACTION_UPDATECONNECTIONSTATUS.Equals(intent?.Action))
            {
                var connStatus = (WearConnectionStatus)intent.GetIntExtra(WearableDataListenerService.EXTRA_CONNECTIONSTATUS, 0);
                switch (connStatus)
                {
                    case WearConnectionStatus.Disconnected:
                        mTextView.Text = GetString(Resource.String.status_disconnected);
                        ErrorProgress();
                        break;
                    case WearConnectionStatus.Connecting:
                        mTextView.Text = GetString(Resource.String.status_connecting);
                        ResetTimer();
                        break;
                    case WearConnectionStatus.AppNotInstalled:
                        mTextView.Text = GetString(Resource.String.error_notinstalled);
                        ResetTimer();

                        // Open store on remote device
                        var intentAndroid = new Intent(Intent.ActionView)
                            .AddCategory(Intent.CategoryBrowsable)
                            .SetData(WearableHelper.PlayStoreURI);

                        RemoteIntent.StartRemoteActivity(this, intentAndroid,
                            new ConfirmationResultReceiver(this));

                        ErrorProgress();
                        break;
                    case WearConnectionStatus.Connected:
                        mTextView.Text = GetString(Resource.String.status_connected);
                        ResetTimer();
                        // Continue operation
                        StartService(new Intent(this, typeof(WearableDataListenerService))
                            .SetAction(WearableDataListenerService.ACTION_REQUESTSETUPSTATUS));
                        break;
                }
            }
            else if (WearableHelper.ErrorPath.Equals(intent?.Action))
            {
                mTextView.Text = GetString(Resource.String.error_syncing);
                ErrorProgress();
            }
            else if (WearableHelper.IsSetupPath.Equals(intent?.Action))
            {
                bool isDeviceSetup = intent.GetBooleanExtra(WearableDataListenerService.EXTRA_DEVICESETUPSTATUS, false);

                Start(isDeviceSetup);
            }
            else if (WearableDataListenerService.ACTION_OPENONPHONE.Equals(intent?.Action))
            {
                bool success = (bool)intent?.GetBooleanExtra(WearableDataListenerService.EXTRA_SUCCESS, false);

                new ConfirmationOverlay()
                    .SetType(success ? ConfirmationOverlay.OpenOnPhoneAnimation : ConfirmationOverlay.FailureAnimation)
                    .ShowOn(this);

                if (!success)
                {
                    mTextView.Text = GetString(Resource.String.error_syncing);
                    ErrorProgress();
                }
            }
            else if (WearableHelper.SettingsPath.Equals(intent?.Action))
            {
                mTextView.Text = GetString(Resource.String.message_settingsretrieved);
                SettingsDataReceived = true;

                if (SettingsDataReceived && LocationDataReceived && WeatherDataReceived)
                    SuccessProgress();
            }
            else if (WearableHelper.LocationPath.Equals(intent?.Action))
            {
                mTextView.Text = GetString(Resource.String.message_locationretrieved);
                LocationDataReceived = true;

                if (SettingsDataReceived && LocationDataReceived && WeatherDataReceived)
                    SuccessProgress();
            }
            else if (WearableHelper.WeatherPath.Equals(intent?.Action))
            {
                mTextView.Text = GetString(Resource.String.message_weatherretrieved);
                WeatherDataReceived = true;

                if (SettingsDataReceived && LocationDataReceived && WeatherDataReceived)
                    SuccessProgress();
            }
        }

        private void Start(bool isDeviceSetup)
        {
            if (isDeviceSetup)
            {
                mTextView.Text = GetString(Resource.String.message_retrievingdata);

                StartService(new Intent(this, typeof(WearableDataListenerService))
                    .SetAction(WearableDataListenerService.ACTION_REQUESTSETTINGSUPDATE));
                StartService(new Intent(this, typeof(WearableDataListenerService))
                    .SetAction(WearableDataListenerService.ACTION_REQUESTLOCATIONUPDATE));
                StartService(new Intent(this, typeof(WearableDataListenerService))
                    .SetAction(WearableDataListenerService.ACTION_REQUESTWEATHERUPDATE));
            }
            else
            {
                mTextView.Text = GetString(Resource.String.message_continueondevice);

                StartService(new Intent(this, typeof(WearableDataListenerService))
                    .SetAction(WearableDataListenerService.ACTION_OPENONPHONE));
            }
        }
    }
}