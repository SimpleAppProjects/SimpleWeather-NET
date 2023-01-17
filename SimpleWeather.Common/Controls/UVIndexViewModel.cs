using SimpleWeather.Icons;
using SimpleWeather.WeatherData;
using System.Collections.Generic;
using System;
#if WINUI
using Microsoft.UI;
using Windows.UI;
#else
using Microsoft.Maui.Graphics;
#endif
using ResStrings = SimpleWeather.Resources.Strings.Resources;
using ResUVIndex = SimpleWeather.Resources.Strings.UVIndex;

namespace SimpleWeather.Common.Controls
{
    public class UVIndexViewModel
    {
        public String Title { get; set; }
        public String Label { get; set; }
        public String Icon { get; set; }
        public int Index { get; set; }
        public String Description { get; set; }
        public int Progress { get; set; }
        public Color ProgressColor { get; set; }

        public UVIndexViewModel(UV uvIdx)
        {
            Title = ResStrings.label_uv;
            Index = (int)uvIdx.index;
            Progress = uvIdx.index >= 11 ? 100 : (int)((uvIdx.index / 11) * 100);

            if (uvIdx.index < 3)
            {
                Description = ResUVIndex.uv_0;
                ProgressColor = Colors.LimeGreen;
            }
            else if (uvIdx.index < 6)
            {
                Description = ResUVIndex.uv_3;
                ProgressColor = Colors.Yellow;
            }
            else if (uvIdx.index < 8)
            {
                Description = ResUVIndex.uv_6;
                ProgressColor = Colors.Orange;
            }
            else if (uvIdx.index < 11)
            {
                Description = ResUVIndex.uv_8;
#if WINUI
                ProgressColor = Color.FromArgb(0xFF, 0xBD, 0x00, 0x35); // Maroon
#else
                ProgressColor = Color.FromRgb(0xBD, 0x00, 0x35); // Maroon
#endif
            }
            else if (uvIdx.index >= 11)
            {
                Description = ResUVIndex.uv_11;
#if WINUI
                ProgressColor = Color.FromArgb(0xFF, 0xAA, 0x00, 0xFF); // Purple
#else
                ProgressColor = Color.FromRgb(0xAA, 0x00, 0xFF); // Purple
#endif
            }

            switch (Index)
            {
                case 1:
                    Icon = WeatherIcons.UV_INDEX_1;
                    break;
                case 2:
                    Icon = WeatherIcons.UV_INDEX_2;
                    break;
                case 3:
                    Icon = WeatherIcons.UV_INDEX_3;
                    break;
                case 4:
                    Icon = WeatherIcons.UV_INDEX_4;
                    break;
                case 5:
                    Icon = WeatherIcons.UV_INDEX_5;
                    break;
                case 6:
                    Icon = WeatherIcons.UV_INDEX_6;
                    break;
                case 7:
                    Icon = WeatherIcons.UV_INDEX_7;
                    break;
                case 8:
                    Icon = WeatherIcons.UV_INDEX_8;
                    break;
                case 9:
                    Icon = WeatherIcons.UV_INDEX_9;
                    break;
                case 10:
                    Icon = WeatherIcons.UV_INDEX_10;
                    break;
                case 11:
                    Icon = WeatherIcons.UV_INDEX_11;
                    break;
                default:
                    Icon = WeatherIcons.UV_INDEX;
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is UVIndexViewModel model &&
                   Title == model.Title &&
                   Label == model.Label &&
                   Icon == model.Icon &&
                   Index == model.Index &&
                   Description == model.Description &&
                   Progress == model.Progress &&
                   EqualityComparer<Color>.Default.Equals(ProgressColor, model.ProgressColor);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Label, Icon, Index, Description, Progress, ProgressColor);
        }
    }
}
