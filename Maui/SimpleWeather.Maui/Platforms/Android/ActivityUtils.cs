using Android.OS;
using AndroidX.Annotations;
using AndroidX.Core.Graphics;
using Color = Android.Graphics.Color;
using Window = Android.Views.Window;

namespace SimpleWeather.Maui.Platforms.Android
{
    public static class ActivityUtils
    {
        public static void SetTransparentWindow(this Window window, [ColorInt] int color)
        {
            window.SetTransparentWindow(color, color, color, true);
        }

        public static void SetTransparentWindow(this Window window, [ColorInt] int backgroundColor, [ColorInt] int statusBarColor, [ColorInt] int navBarColor)
        {
            window.SetTransparentWindow(backgroundColor, statusBarColor, navBarColor, true);
        }

        public static void SetTransparentWindow(this Window window, [ColorInt] int backgroundColor, [ColorInt] int statusBarColor, [ColorInt] int navBarColor, bool setColors)
        {
            var isLightNavBar = navBarColor != Color.Transparent && ColorUtils.CalculateContrast(Color.White, ColorUtils.SetAlphaComponent(navBarColor, 0xff)) < 4.5f ||
                navBarColor == Color.Transparent && ColorUtils.CalculateContrast(Color.White, backgroundColor) < 4.5f;
            var navBarProtected = Build.VERSION.SdkInt >= BuildVersionCodes.O || !isLightNavBar;

            var isLightStatusBar = statusBarColor != Color.Transparent && ColorUtils.CalculateContrast(Color.White, ColorUtils.SetAlphaComponent(statusBarColor, 0xff)) < 4.5f ||
                statusBarColor == Color.Transparent && ColorUtils.CalculateContrast(Color.White, backgroundColor) < 4.5f;
            var statusBarProtected = Build.VERSION.SdkInt >= BuildVersionCodes.M || !isLightStatusBar;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {

            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {

            }

            if (setColors)
            {
                if (statusBarProtected)
                {
                    window.SetStatusBarColor(new Color(statusBarColor));
                }
                else
                {
                    window.SetStatusBarColor(new Color(ColorUtils.BlendARGB(statusBarColor, Color.Black, 0.25f)));
                }

                if (navBarProtected)
                {
                    window.SetNavigationBarColor(new Color(navBarColor));
                }
                else
                {
                    window.SetNavigationBarColor(new Color(ColorUtils.BlendARGB(navBarColor, Color.Black, 0.25f)));
                }
            }
            else
            {
                window.SetStatusBarColor(Color.Transparent);
                window.SetNavigationBarColor(Color.Transparent);
            }
        }
    }
}
