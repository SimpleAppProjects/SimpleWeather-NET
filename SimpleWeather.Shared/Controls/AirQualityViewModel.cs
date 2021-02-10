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
            Title = SimpleLibrary.GetInstance().ResLoader.GetString("AQI_Label");
            Index = aqi.index.GetValueOrDefault();
            Progress = aqi.index >= 300 ? 100 : (int)((aqi.index / 300f) * 100);

            if (aqi.index < 51)
            {
                ProgressColor = Colors.LimeGreen;
                Level = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Level_0_50");
                Description = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Desc_0_50");
            }
            else if (aqi.index < 101)
            {
                ProgressColor = Color.FromArgb(0xff, 0xff, 0xde, 0x33);
                Level = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Level_51_100");
                Description = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Desc_51_100");
            }
            else if (aqi.index < 151)
            {
                ProgressColor = Color.FromArgb(0xff, 0xff, 0x99, 0x33);
                Level = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Level_101_150");
                Description = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Desc_101_150");
            }
            else if (aqi.index < 201)
            {
                ProgressColor = Color.FromArgb(0xff, 0xcc, 0x00, 0x33);
                Level = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Level_151_200");
                Description = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Desc_151_200");
            }
            else if (aqi.index < 301)
            {
                ProgressColor = Color.FromArgb(0xff, 0xaa, 0x00, 0xff); // 0xff660099
                Level = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Level_201_300");
                Description = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Desc_201_300");
            }
            else if (aqi.index >= 301)
            {
                ProgressColor = Color.FromArgb(0xff, 0xbd, 0x00, 0x35); // 0xff7e0023
                Level = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Level_300");
                Description = SimpleLibrary.GetInstance().ResLoader.GetString("/AQIndex/AQI_Desc_300");
            }
        }
    }
}