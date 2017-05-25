using SimpleWeather.Utils;
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using SimpleWeather.Droid.Utils;

namespace SimpleWeather.Droid
{
    [Activity(Name = "SimpleWeather.Droid.LaunchActivity",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        MainLauncher = true)]
    public class LaunchActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppTheme);
            base.OnCreate(savedInstanceState);

            // Create your application here
            Intent intent = null;

            try
            {
                if (Settings.WeatherLoaded)
                    intent = new Intent(this, typeof(MainActivity));
                else
                    intent = new Intent(this, typeof(SetupActivity));
            }
            catch (Exception e)
            {
                Android.Util.Log.WriteLine(Android.Util.LogPriority.Error, "LaunchActivity", e.StackTrace);
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