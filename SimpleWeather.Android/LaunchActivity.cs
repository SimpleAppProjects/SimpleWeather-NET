using SimpleWeather.Utils;
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;

namespace SimpleWeather.Droid
{
    [Activity(Name = "SimpleWeather.Droid.LaunchActivity",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        MainLauncher = true)]
    public class LaunchActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppTheme);
            base.OnCreate(savedInstanceState);

            // Create your application here
            Intent intent = null;
            int homeIdx = 0;

            String ARG_QUERY = "query";
            String ARG_INDEX = "index";

            try
            {
                if (Settings.WeatherLoaded)
                {
                    List<String> locations = await Settings.getLocations();
                    String local = locations[homeIdx];

                    intent = new Intent(this, typeof(MainActivity));
                    intent.PutExtra(ARG_QUERY, local);
                    intent.PutExtra(ARG_INDEX, homeIdx);
                }
                else
                {
                    intent = new Intent(this, typeof(SetupActivity));
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(LogPriority.Error, "LaunchActivity", e.StackTrace);
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