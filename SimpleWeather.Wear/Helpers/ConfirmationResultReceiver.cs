using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Wearable.Activity;
using Android.Support.Wearable.Views;
using Android.Views;
using Android.Widget;
using Google.Android.Wearable.Intent;

namespace SimpleWeather.Droid.Wear.Helpers
{
    public class ConfirmationResultReceiver : ResultReceiver
    {
        private Activity Activity;

        public ConfirmationResultReceiver(Activity activity)
            : base(new Handler())
        {
            this.Activity = activity;
        }

        protected override void OnReceiveResult(int resultCode, Bundle resultData)
        {
            if (resultCode == RemoteIntent.ResultOk)
            {
                if (Activity != null)
                    ShowConfirmationOverlay(true);
            }
            else if (resultCode == RemoteIntent.ResultFailed)
            {
                if (Activity != null)
                    ShowConfirmationOverlay(false);
            }
            else
            {
                throw new Java.Lang.IllegalStateException("Unexpected result " + resultCode);
            }
        }

        private void ShowConfirmationOverlay(bool success)
        {
            var overlay = new ConfirmationOverlay();

            if (!success)
                overlay.SetType(ConfirmationOverlay.FailureAnimation);
            else
                overlay.SetType(ConfirmationOverlay.OpenOnPhoneAnimation);

            overlay.ShowOn(Activity);
        }
    }
}