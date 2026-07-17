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
        public double Progress { get; set; }
        public Color ProgressColor { get; set; }

        public UVIndexViewModel(UV uvIdx)
        {
            Title = ResStrings.label_uv;
            Index = (int)uvIdx.index.GetValueOrDefault(0);
            Progress = uvIdx.index >= 11 ? 100d : ((uvIdx.index ?? 0f) / 11) * 100d;

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

            Icon = Index switch
            {
                1 => WeatherIcons.UV_INDEX_1,
                2 => WeatherIcons.UV_INDEX_2,
                3 => WeatherIcons.UV_INDEX_3,
                4 => WeatherIcons.UV_INDEX_4,
                5 => WeatherIcons.UV_INDEX_5,
                6 => WeatherIcons.UV_INDEX_6,
                7 => WeatherIcons.UV_INDEX_7,
                8 => WeatherIcons.UV_INDEX_8,
                9 => WeatherIcons.UV_INDEX_9,
                10 => WeatherIcons.UV_INDEX_10,
                11 => WeatherIcons.UV_INDEX_11,
                _ => WeatherIcons.UV_INDEX,
            };
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
