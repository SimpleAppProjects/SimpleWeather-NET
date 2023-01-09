using Microsoft.UI;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using Windows.UI;
using static SimpleWeather.WeatherData.Beaufort;

namespace SimpleWeather.Common.Controls
{
    public class BeaufortViewModel
    {
        public String Title { get; set; }
        public DetailItemViewModel Beaufort { get; set; }
        public int BeaufortScaleProgress { get; set; }
        public Color BeaufortScaleProgressColor { get; set; }

        public BeaufortViewModel(Beaufort beaufort)
        {
            if (beaufort != null)
            {
                Beaufort = (new DetailItemViewModel(beaufort.scale));
                Title = Beaufort.Label;

                switch (beaufort.scale)
                {
                    case BeaufortScale.B0:
                        BeaufortScaleProgress = 0;
                        BeaufortScaleProgressColor = Colors.DodgerBlue;
                        break;
                    case BeaufortScale.B1:
                        BeaufortScaleProgress = (int)((1f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.DeepSkyBlue;
                        break;
                    case BeaufortScale.B2:
                        BeaufortScaleProgress = (int)((2f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.SkyBlue;
                        break;
                    case BeaufortScale.B3:
                        BeaufortScaleProgress = (int)((3f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.LimeGreen;
                        break;
                    case BeaufortScale.B4:
                        BeaufortScaleProgress = (int)((4f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.Lime;
                        break;
                    case BeaufortScale.B5:
                        BeaufortScaleProgress = (int)((5f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.GreenYellow;
                        break;
                    case BeaufortScale.B6:
                        BeaufortScaleProgress = (int)((6f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.PaleGreen;
                        break;
                    case BeaufortScale.B7:
                        BeaufortScaleProgress = (int)((7f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.LightYellow;
                        break;
                    case BeaufortScale.B8:
                        BeaufortScaleProgress = (int)((8f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.Goldenrod;
                        break;
                    case BeaufortScale.B9:
                        BeaufortScaleProgress = (int)((9f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.Orange;
                        break;
                    case BeaufortScale.B10:
                        BeaufortScaleProgress = (int)((10f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.DarkOrange;
                        break;
                    case BeaufortScale.B11:
                        BeaufortScaleProgress = (int)((11f / 12) * 100);
                        BeaufortScaleProgressColor = Colors.OrangeRed;
                        break;
                    case BeaufortScale.B12:
                        BeaufortScaleProgress = 100;
                        BeaufortScaleProgressColor = Color.FromArgb(0xFF, 0xBD, 0x00, 0x35); // FFBD0035
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is BeaufortViewModel model &&
                   Title == model.Title &&
                   EqualityComparer<DetailItemViewModel>.Default.Equals(Beaufort, model.Beaufort) &&
                   BeaufortScaleProgress == model.BeaufortScaleProgress &&
                   EqualityComparer<Color>.Default.Equals(BeaufortScaleProgressColor, model.BeaufortScaleProgressColor);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Beaufort, BeaufortScaleProgress, BeaufortScaleProgressColor);
        }
    }
}
