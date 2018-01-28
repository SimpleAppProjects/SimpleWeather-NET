using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimpleWeather.Utils;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SimpleWeather.Droid.Widgets
{
    [Activity(
        Name = "com.thewizrd.simpleweather.WeatherWidgetConfigActivity",
        Theme = "@android:style/Theme.Translucent.NoTitleBar")]
    [IntentFilter(new string[]
    {
        AppWidgetManager.ActionAppwidgetConfigure
    })]
    public class WeatherWidgetConfigActivity : Activity
    {
        // Widget id for ConfigurationActivity
        private int mAppWidgetId = AppWidgetManager.InvalidAppwidgetId;

        // RequestCode
        public const int WIDGET_CONFIG_REQUEST = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the result to CANCELED.  This will cause the widget host to cancel
            // out of the widget placement if they press the back button.
            SetResult(Result.Canceled, new Intent().PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId));

            // Find the widget id from the intent.
            if (Intent != null && Intent.Extras != null)
            {
                mAppWidgetId = Intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId);
            }

            if (mAppWidgetId != AppWidgetManager.InvalidAppwidgetId)
            {
                if (Settings.WeatherLoaded)
                {
                    // Weather is already loaded; no need to setup
                    // Trigger widget service to update widget
                    WeatherWidgetService.EnqueueWork(this,
                        new Intent(this, typeof(WeatherWidgetService))
                        .SetAction(WeatherWidgetService.ACTION_REFRESHWIDGET)
                        .PutExtra(WeatherWidgetProvider.EXTRA_WIDGET_IDS, new int[] { mAppWidgetId }));

                    // Create return intent
                    Intent resultValue = new Intent();
                    resultValue.PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId);
                    SetResult(Result.Ok, resultValue);
                    Finish();
                }
                else
                {
                    // Start SetupActivity
                    Intent setupIntent = new Intent(this, typeof(SetupActivity))
                                         .SetAction(AppWidgetManager.ActionAppwidgetConfigure)
                                         .PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId);
                    StartActivityForResult(setupIntent, WIDGET_CONFIG_REQUEST);
                }
            }
            else
            {
                // If they gave us an intent without the widget id, just bail.
                Finish();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == WIDGET_CONFIG_REQUEST)
            {
                if (resultCode == Result.Ok)
                {
                    // Create return intent
                    Intent resultValue = new Intent();
                    resultValue.PutExtra(AppWidgetManager.ExtraAppwidgetId, mAppWidgetId);
                    SetResult(Result.Ok, resultValue);
                }
            }

            // Finish up
            Finish();
        }
    }
}