using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;

namespace SimpleWeather.Controls
{
    public class AirQualityViewModel
    {
        public String Title { get; set; }
        public int Index { get; set; }
        public String Level { get; set; }
        public String Description { get; set; }
        public int Progress { get; set; }
        public Color ProgressColor { get; set; }

        public AirQualityViewModel(AirQuality aqi)
        {
            Title = SimpleLibrary.ResLoader.GetString("AQI_Label");
            Index = aqi.index;
            Progress = aqi.index >= 300 ? 100 : (int)((aqi.index / 300f) * 100);

            if (aqi.index < 51)
            {
                ProgressColor = Colors.Green;
                Level = SimpleLibrary.ResLoader.GetString("AQI_Level_0_50");
                Description = SimpleLibrary.ResLoader.GetString("AQI_Desc_0_50");
            }
            else if (aqi.index < 101)
            {
                ProgressColor = Colors.Yellow;
                Level = SimpleLibrary.ResLoader.GetString("AQI_Level_51_100");
                Description = SimpleLibrary.ResLoader.GetString("AQI_Desc_51_100");
            }
            else if (aqi.index < 151)
            {
                ProgressColor = Colors.Orange;
                Level = SimpleLibrary.ResLoader.GetString("AQI_Level_101_150");
                Description = SimpleLibrary.ResLoader.GetString("AQI_Desc_101_150");
            }
            else if (aqi.index < 201)
            {
                ProgressColor = Colors.Red;
                Level = SimpleLibrary.ResLoader.GetString("AQI_Level_151_200");
                Description = SimpleLibrary.ResLoader.GetString("AQI_Desc_151_200");
            }
            else if (aqi.index < 301)
            {
                ProgressColor = Colors.Purple;
                Level = SimpleLibrary.ResLoader.GetString("AQI_Level_201_300");
                Description = SimpleLibrary.ResLoader.GetString("AQI_Desc_201_300");
            }
            else if (aqi.index >= 301)
            {
                ProgressColor = Colors.Maroon;
                Level = SimpleLibrary.ResLoader.GetString("AQI_Level_300");
                Description = SimpleLibrary.ResLoader.GetString("AQI_Desc_300");
            }
        }
    }
}
