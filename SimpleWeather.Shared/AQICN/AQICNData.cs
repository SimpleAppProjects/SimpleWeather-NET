using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.AQICN
{
    public class AQICNData : AirQualityData
    {
        public List<UviItem> uvi_forecast { get; set; }

        internal AQICNData(Rootobject root)
        {
            current = new()
            {
                no2 = root.data?.iaqi?.no2?.v.RoundToInt(),
                o3 = root.data?.iaqi?.o3?.v.RoundToInt(),
                pm25 = root.data?.iaqi?.pm25?.v.RoundToInt(),
                so2 = root.data?.iaqi?.so2?.v.RoundToInt(),
                pm10 = root.data?.iaqi?.pm10?.v.RoundToInt(),
                co = root.data?.iaqi?.co?.v.RoundToInt(),
                attribution = "World Air Quality Index Project"
            };

            current.index = root.data?.aqi ?? current.GetIndexFromData();

            aqiForecast = CreateAQIForecasts(root);

            uvi_forecast = root.data?.forecast?.daily?.uvi?.ToList();
        }

        private List<AirQuality> CreateAQIForecasts(Rootobject root)
        {
            var dailyData = root.data?.forecast?.daily;
            var maxAmtFcasts = NumberUtils.MaxOf(
                dailyData?.o3?.Length ?? 0,
                dailyData?.pm10?.Length ?? 0,
                dailyData?.pm25?.Length ?? 0
                );

            if (dailyData != null && maxAmtFcasts > 0)
            {
                var aqiForecasts = new List<AirQuality>(maxAmtFcasts);
                const string dateFormat = "yyyy-MM-dd";

                dailyData.o3?.ForEach(it =>
                {
                    // 2021-12-17
                    var itemDate = DateTime.ParseExact(it.day, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                    var existing = aqiForecasts.FirstOrDefault(aqi => aqi.date == itemDate);

                    if (existing != null)
                    {
                        existing.o3 = it.avg;
                    }
                    else
                    {
                        aqiForecasts.Add(new()
                        {
                            date = itemDate,
                            o3 = it.avg
                        });
                    }
                });

                dailyData.pm25?.ForEach(it =>
                {
                    // 2021-12-17
                    var itemDate = DateTime.ParseExact(it.day, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                    var existing = aqiForecasts.FirstOrDefault(aqi => aqi.date == itemDate);

                    if (existing != null)
                    {
                        existing.pm25 = it.avg;
                    }
                    else
                    {
                        aqiForecasts.Add(new()
                        {
                            date = itemDate,
                            pm25 = it.avg
                        });
                    }
                });

                dailyData.pm10?.ForEach(it =>
                {
                    // 2021-12-17
                    var itemDate = DateTime.ParseExact(it.day, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                    var existing = aqiForecasts.FirstOrDefault(aqi => aqi.date == itemDate);

                    if (existing != null)
                    {
                        existing.pm10 = it.avg;
                    }
                    else
                    {
                        aqiForecasts.Add(new()
                        {
                            date = itemDate,
                            pm10 = it.avg
                        });
                    }
                });

                return aqiForecasts.OrderBy(it =>
                {
                    it.index = it.GetIndexFromData();
                    return it.date;
                }).ToList();
            }

            return null;
        }
    }
}
