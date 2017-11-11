using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SimpleWeather.Utils;
using Android.Content.PM;
using SimpleWeather.WeatherData;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Support.V4.Content;

namespace SimpleWeather.Droid.Shortcuts
{
    public static class ShortcutCreator
    {
        public static async Task UpdateShortcuts()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.NMr1)
            {
                var locations = new List<LocationData>(await Settings.GetLocationData());
                if (Settings.FollowGPS)
                    locations.Insert(0, Settings.HomeData);

                ShortcutManager shortcutMan = (ShortcutManager)App.Context.GetSystemService(Java.Lang.Class.FromType(typeof(ShortcutManager)));
                IList<ShortcutInfo> shortcuts = new List<ShortcutInfo>();

                shortcutMan.RemoveAllDynamicShortcuts();

                int MAX_SHORTCUTS = 4;
                if (locations.Count < MAX_SHORTCUTS)
                    MAX_SHORTCUTS = locations.Count;

                for (int i = 0; i < MAX_SHORTCUTS; i++)
                {
                    LocationData location = locations[i];
                    Weather weather = await Settings.GetWeatherData(location.query);

                    if (weather == null || shortcuts.Any(s => s.Id == location.query))
                    {
                        locations.RemoveAt(i);
                        i--;
                        if (locations.Count < MAX_SHORTCUTS)
                            MAX_SHORTCUTS = locations.Count;
                        continue;
                    }

                    // Start WeatherNow Activity with weather data
                    Intent intent = new Intent(App.Context, typeof(MainActivity))
                        .SetAction(Intent.ActionMain)
                        .PutExtra("shortcut-data", location.ToJson())
                        .SetFlags(ActivityFlags.NewTask | ActivityFlags.MultipleTask | ActivityFlags.NoHistory);

                    var bmp = await BitmapFactory.DecodeResourceAsync(App.Context.Resources, WeatherUtils.GetWeatherIconResource(weather.condition.icon),
                        new BitmapFactory.Options() { InMutable = true });
                    var newImage = Bitmap.CreateBitmap(bmp.Width, bmp.Height, bmp.GetConfig());
                    Paint paint = new Paint();
                    paint.SetColorFilter(new PorterDuffColorFilter(new Color(ContextCompat.GetColor(App.Context, Resource.Color.colorPrimary)),
                        PorterDuff.Mode.SrcIn));
                    var canvas = new Canvas(newImage);
                    canvas.DrawBitmap(bmp, 0, 0, paint);
                    bmp.Recycle();
                    ShortcutInfo shortcut = new ShortcutInfo.Builder(App.Context, location.query)
                        .SetShortLabel(weather.location.name)
                        .SetIcon(Icon.CreateWithBitmap(newImage))
                        .SetIntent(intent)
                        .Build();

                    shortcuts.Add(shortcut);
                }

                shortcutMan.SetDynamicShortcuts(shortcuts);
            }
        }

        public static void RemoveShortcuts()
        {
            ShortcutManager shortcutMan = (ShortcutManager)App.Context.GetSystemService(Java.Lang.Class.FromType(typeof(ShortcutManager)));
            shortcutMan.RemoveAllDynamicShortcuts();
        }
    }
}