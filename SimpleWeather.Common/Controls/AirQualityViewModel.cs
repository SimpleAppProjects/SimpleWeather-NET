using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using ResAQIndex = SimpleWeather.Resources.Strings.AQIndex;
#if WINUI
using Microsoft.UI;
using Windows.UI;
#else
using Microsoft.Maui.Graphics;
#endif

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
            Index = Progress = aqi.index.GetValueOrDefault();

            if (aqi.index < 51)
            {
                ProgressColor = Colors.LimeGreen;
                Level = ResAQIndex.aqi_level_0_50;
                Description = ResAQIndex.aqi_desc_0_50;
            }
            else if (aqi.index < 101)
            {
#if WINUI
                ProgressColor = Color.FromArgb(0xff, 0xff, 0xde, 0x33);
#else
                ProgressColor = Color.FromRgb(0xff, 0xde, 0x33);
#endif
                Level = ResAQIndex.aqi_level_51_100;
                Description = ResAQIndex.aqi_desc_51_100;
            }
            else if (aqi.index < 151)
            {
#if WINUI
                ProgressColor = Color.FromArgb(0xff, 0xff, 0x99, 0x33);
#else
                ProgressColor = Color.FromRgb(0xff, 0x99, 0x33);
#endif
                Level = ResAQIndex.aqi_level_101_150;
                Description = ResAQIndex.aqi_desc_101_150;
            }
            else if (aqi.index < 201)
            {
#if WINUI
                ProgressColor = Color.FromArgb(0xff, 0xcc, 0x00, 0x33);
#else
                ProgressColor = Color.FromRgb(0xcc, 0x00, 0x33);
#endif
                Level = ResAQIndex.aqi_level_151_200;
                Description = ResAQIndex.aqi_desc_151_200;
            }
            else if (aqi.index < 301)
            {
#if WINUI
                ProgressColor = Color.FromArgb(0xff, 0xaa, 0x00, 0xff); // 0xff660099
#else
                ProgressColor = Color.FromRgb(0xaa, 0x00, 0xff); // 0xff660099
#endif
                Level = ResAQIndex.aqi_level_201_300;
                Description = ResAQIndex.aqi_desc_201_300;
            }
            else if (aqi.index >= 301)
            {
#if WINUI
                ProgressColor = Color.FromArgb(0xff, 0xbd, 0x00, 0x35); // 0xff7e0023
#else
                ProgressColor = Color.FromRgb(0xbd, 0x00, 0x35); // 0xff7e0023
#endif
                Level = ResAQIndex.aqi_level_300;
                Description = ResAQIndex.aqi_desc_300;
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
                Date = aqi.date.Value.ToString("dddd", LocaleUtils.GetLocale());
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