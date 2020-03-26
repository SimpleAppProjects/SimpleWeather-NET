using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.System.UserProfile;
using Windows.UI;

namespace SimpleWeather.Controls
{
    public class BeaufortViewModel
    {
        public String Title { get; set; }
        public DetailItemViewModel Beaufort { get; set; }
        public int BeaufortScaleProgress { get; set; }
        public Color BeaufortScaleProgressColor { get; set; }

        public BeaufortViewModel(Beaufort beaufort)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            if (beaufort != null)
            {
                Beaufort = (new DetailItemViewModel(beaufort.scale, beaufort.desc));
                Title = Beaufort.Label;

                switch (beaufort.scale)
                {
                    case WeatherData.Beaufort.BeaufortScale.B0:
                        BeaufortScaleProgress = 0;
                        BeaufortScaleProgressColor = Colors.SkyBlue;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B1:
                        BeaufortScaleProgress = (int)((1f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.LightSkyBlue;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B2:
                        BeaufortScaleProgress = (int)((2f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.DeepSkyBlue;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B3:
                        BeaufortScaleProgress = (int)((3f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.Green;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B4:
                        BeaufortScaleProgress = (int)((4f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.GreenYellow;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B5:
                        BeaufortScaleProgress = (int)((5f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.LimeGreen;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B6:
                        BeaufortScaleProgress = (int)((6f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.LightGreen;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B7:
                        BeaufortScaleProgress = (int)((7f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.LightYellow;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B8:
                        BeaufortScaleProgress = (int)((8f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.Goldenrod;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B9:
                        BeaufortScaleProgress = (int)((9f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.Orange;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B10:
                        BeaufortScaleProgress = (int)((10f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.DarkOrange;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B11:
                        BeaufortScaleProgress = (int)((11f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.OrangeRed;
                        break;
                    case WeatherData.Beaufort.BeaufortScale.B12:
                        BeaufortScaleProgress = 100;
                        BeaufortScaleProgressColor = Colors.Red;
                        break;
                }
            }
        }
    }
}
