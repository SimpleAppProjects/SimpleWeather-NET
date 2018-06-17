using SimpleWeather.Utils;
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace SimpleWeather.Droid.App
{
    [Activity(MainLauncher = true)]
    public class LaunchActivity : AppCompatActivity
    {
        private static String TAG = nameof(LaunchActivity);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppTheme);
            base.OnCreate(savedInstanceState);

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