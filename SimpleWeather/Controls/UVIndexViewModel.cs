using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;

namespace SimpleWeather.Controls
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
            Title = SimpleLibrary.ResLoader.GetString("UV_Label");
            Icon = WeatherIcons.DAY_SUNNY;
            Description = uvIdx.desc;
            Index = (int)uvIdx.index;
            Progress = uvIdx.index >= 11 ? 100 : (int)((uvIdx.index / 11) * 100);

            if (uvIdx.index < 3)
            {
                ProgressColor = Colors.LimeGreen;
            }
            else if (uvIdx.index < 6)
            {
                ProgressColor = Colors.Yellow;
            }
            else if (uvIdx.index < 8)
            {
                ProgressColor = Colors.Orange;
            }
            else if (uvIdx.index < 11)
            {
                ProgressColor = Color.FromArgb(0xFF, 0xBD, 0x00, 0x35); // Maroon
            }
            else if (uvIdx.index >= 11)
            {
                ProgressColor = Color.FromArgb(0xFF, 0xAA, 0x00, 0xFF); // Purple
            }
        }
    }
}
