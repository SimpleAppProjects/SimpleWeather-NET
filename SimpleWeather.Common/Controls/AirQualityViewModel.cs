using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace SimpleWeather.Common.Controls
{
    public class AirQualityViewModel
    {
        public int Index { get; set; }
        public String Level { get; set; }
        public String Description { get; set; }
        public int Progress { get; set; }
        public Color ProgressColor { get; set; }
        public String Attribution { get; set; }
        public String Date { get; set; }

        public int? NO2Index { get; set; }
        public int? O3Index { get; set; }
        public int? SO2Index { get; set; }
        public int? PM25Index { get; set; }
        public int? PM10Index { get; set; }
        public int? COIndex { get; set; }

        public AirQualityViewModel(AirQuality aqi)
        {
            Index = aqi.index.GetValueOrDefault();
            Progress = aqi.index >= 300 ? 100 : (int)((aqi.index / 300f) * 100);

            if (aqi.index < 51)
            {
                ProgressColor = Colors.LimeGreen;
                Level = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Level_0_50");
                Description = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Desc_0_50");
            }
            else if (aqi.index < 101)
            {
                ProgressColor = Color.FromArgb(0xff, 0xff, 0xde, 0x33);
                Level = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Level_51_100");
                Description = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Desc_51_100");
            }
            else if (aqi.index < 151)
            {
                ProgressColor = Color.FromArgb(0xff, 0xff, 0x99, 0x33);
                Level = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Level_101_150");
                Description = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Desc_101_150");
            }
            else if (aqi.index < 201)
            {
                ProgressColor = Color.FromArgb(0xff, 0xcc, 0x00, 0x33);
                Level = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Level_151_200");
                Description = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Desc_151_200");
            }
            else if (aqi.index < 301)
            {
                ProgressColor = Color.FromArgb(0xff, 0xaa, 0x00, 0xff); // 0xff660099
                Level = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Level_201_300");
                Description = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Desc_201_300");
            }
            else if (aqi.index >= 301)
            {
                ProgressColor = Color.FromArgb(0xff, 0xbd, 0x00, 0x35); // 0xff7e0023
                Level = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Level_300");
                Description = SharedModule.Instance.ResLoader.GetString("/AQIndex/AQI_Desc_300");
            }

            Attribution = aqi.attribution;

            NO2Index = aqi.no2;
            O3Index = aqi.o3;
            SO2Index = aqi.so2;
            PM25Index = aqi.pm25;
            PM10Index = aqi.pm10;
            COIndex = aqi.co;

            if (aqi.date.HasValue && aqi.date.Value != DateTime.MinValue)
            {
                Date = aqi.date.Value.ToString("dddd", CultureUtils.UserCulture);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is AirQualityViewModel model &&
                   Index == model.Index &&
                   Level == model.Level &&
                   Description == model.Description &&
                   Progress == model.Progress &&
                   EqualityComparer<Color>.Default.Equals(ProgressColor, model.ProgressColor) &&
                   Attribution == model.Attribution &&
                   NO2Index == model.NO2Index &&
                   O3Index == model.O3Index &&
                   SO2Index == model.SO2Index &&
                   PM25Index == model.PM25Index &&
                   PM10Index == model.PM10Index &&
                   COIndex == model.COIndex &&
                   Date == model.Date;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Index);
            hash.Add(Level);
            hash.Add(Description);
            hash.Add(Progress);
            hash.Add(ProgressColor);
            hash.Add(Attribution);
            hash.Add(NO2Index);
            hash.Add(O3Index);
            hash.Add(SO2Index);
            hash.Add(PM25Index);
            hash.Add(PM10Index);
            hash.Add(COIndex);
            hash.Add(Date);
            return hash.ToHashCode();
        }
    }
}