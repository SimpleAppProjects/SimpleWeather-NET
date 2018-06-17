using SimpleWeather.Droid.Utils;
using SimpleWeather.Utils;
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Wearable.Activity;

namespace SimpleWeather.Droid.Wear
{
    [Activity(MainLauncher = true)]
    public class LaunchActivity : WearableActivity
    {
        private static String TAG = nameof(LaunchActivity);

        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.WearAppTheme);
            base.OnCreate(bundle);

            // Create your application here
            Intent intent = null;

            try
            {
                if (Settings.WeatherLoaded)
                {
                    intent = new Intent(this, typeof(MainActivity))
                        .SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                }
                else
                    intent = new Intent(this, typeof(SetupActivity));
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "SimpleWeather: {0}: error loading", TAG);
            }
            finally
            {
                if (intent == null)
                {
                    intent = new Intent(this, typeof(SetupActivity));
                }

                // Navigate
                StartActivity(intent);
                FinishAffinity();
            }
        }
    }
}


