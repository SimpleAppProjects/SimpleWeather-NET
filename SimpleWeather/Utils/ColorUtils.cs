using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace SimpleWeather.Utils
{
    public class ColorUtils
    {
        public static Color GetAccentColor()
        {
            var uiSettings = new UISettings();
            return uiSettings.GetColorValue(UIColorType.Accent);
        }

        public static bool IsColorDark(Color c)
        {
            return (5 * c.G + 2 * c.R + c.B) <= 8 * 128;
        }
    }
}
