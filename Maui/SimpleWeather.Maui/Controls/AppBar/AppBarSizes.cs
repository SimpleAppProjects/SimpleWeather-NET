using System;
using System.ComponentModel;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Controls.AppBar
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class AppBarSizes
    {
        public static double GetDefaultBarHeight()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return 60d;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return 52d;

            return 60d;
        }

        public static string GetDefaultFontFamly()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return "Roboto-Medium.ttf#Roboto Medium";

            return string.Empty;
        }

        public static double GetDefaultFontSize()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return 20d;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return 17d;

            return 20d;
        }

        public static double GetNavigationFontSize()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return 24d;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return 20d;

            return 24d;
        }

        public static string GetNavigationFontFamily()
        {
            return MaterialIcons.MaterialIcon.FONT_ALIAS;
        }

        public static string GetRightItemsGlyph()
        {
            return "\ue5d4";
        }

        public static string GetNavigationGlyph()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return "\ue5c4";

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return "\ue5e0";

            return "\ue5c4";
        }

        public static string GetDefaultBackButtonTitle()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return string.Empty;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return ResStrings.label_back;

            return string.Empty;
        }

        public static Color GetDefaultBorderColor()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return Colors.Transparent;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return Colors.Black;

            return Colors.Transparent;
        }

        public static Thickness GetNavigationIconMargin()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return new Thickness(16, 0, 0, 0);

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return new Thickness(16, 0, 12, 0);

            return new Thickness(16, 0, 0, 0);
        }

        public static double GetBackContainerMinWidth()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return 0d;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return 52d;

            return 0d;
        }

        public static LayoutOptions GetTitleLayoutOptions()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return LayoutOptions.Start;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return LayoutOptions.Center;

            return LayoutOptions.Start;
        }

        public static Thickness GetTitleMargin()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return new Thickness(16, 0, 24, 0);

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return new Thickness(0, 0, 0, 0);

            return new Thickness(16, 0, 24, 0);
        }

        public static double GetToolBarItemSize()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return 32d;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return 48d;

            return 48d;
        }
    }
}

