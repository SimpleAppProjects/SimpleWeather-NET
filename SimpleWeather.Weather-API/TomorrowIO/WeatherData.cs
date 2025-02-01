using System;
using System.Collections.Generic;
using System.Text;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using Condition = SimpleWeather.WeatherData.Condition;
using Location = SimpleWeather.WeatherData.Location;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Weather_API.TomorrowIO
{
    public static partial class TomorrowIOWeatherProviderExtensions
    {
        public static Weather CreateWeatherData(this TomorrowIOWeatherProvider _, Rootobject root,
            Rootobject minutelyRoot, AlertRootobject alertRoot)
        {
            var weather = new Weather();

            weather.location = _.CreateLocation(root);
            weather.update_time = DateTimeOffset.UtcNow;

            foreach (var timeline in root.data.timelines)
            {
                if (timeline.timestep == "1h")
                {
                    weather.hr_forecast = new List<HourlyForecast>(timeline.intervals.Length);

                    foreach (var interval in timeline.intervals)
                    {
                        weather.hr_forecast.Add(_.CreateHourlyForecast(interval));
                    }
                }
                else if (timeline.timestep == "1d")
                {
                    weather.forecast = new List<Forecast>(timeline.intervals.Length);
                    weather.txt_forecast = new List<TextForecast>(timeline.intervals.Length);
                    weather.aqi_forecast = new List<AirQuality>(timeline.intervals.Length);

                    foreach (var interval in timeline.intervals)
                    {
                        if (weather.astronomy == null && weather.update_time.Date.Equals(interval.startTime.Date))
                        {
                            weather.astronomy = _.CreateAstronomy(interval);
                        }

                        weather.forecast.Add(_.CreateForecast(interval));
                        weather.txt_forecast.Add(_.CreateTextForecast(interval));

                        if (interval.values.epaIndex.HasValue)
                        {
                            weather.aqi_forecast.Add(_.CreateAQIForecast(interval));
                        }
                    }
                }
                else if (timeline.timestep == "current")
                {
                    weather.condition = _.CreateCondition(timeline.intervals[0]);
                    weather.atmosphere = _.CreateAtmosphere(timeline.intervals[0]);
                    weather.precipitation = _.CreatePrecipitation(timeline.intervals[0]);
                }
            }

            if (minutelyRoot != null)
            {
                foreach (var timeline in minutelyRoot.data.timelines)
                {
                    if (timeline.timestep == "1m")
                    {
                        weather.min_forecast = new List<MinutelyForecast>(timeline.intervals.Length);

                        foreach (var interval in timeline.intervals)
                        {
                            weather.min_forecast.Add(_.CreateMinutelyForecast(interval));
                        }
                    }
                }
            }

            if ((!weather.condition.high_f.HasValue || !weather.condition.high_c.HasValue ||
                 weather.condition.high_f == weather.condition.low_f) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f;
                weather.condition.high_c = weather.forecast[0].high_c;
                weather.condition.low_f = weather.forecast[0].low_f;
                weather.condition.low_c = weather.forecast[0].low_c;
            }

            weather.weather_alerts = _.CreateWeatherAlerts(alertRoot);

            weather.ttl = 120;
            weather.source = WeatherAPI.TomorrowIo;

            return weather;
        }

        public static Location CreateLocation(this TomorrowIOWeatherProvider _, Rootobject root)
        {
            return new Location()
            {
                /* Use name from location provider */
                name = null,
                latitude = null,
                longitude = null,
                tz_long = null,
            };
        }

        public static Forecast CreateForecast(this TomorrowIOWeatherProvider _, Interval item)
        {
            var forecast = new Forecast();

            forecast.date = item.startTime.UtcDateTime;

            if (item.values.temperatureMax.HasValue)
            {
                forecast.high_f = ConversionMethods.CtoF(item.values.temperatureMax.Value);
                forecast.high_c = item.values.temperatureMax;
            }

            if (item.values.temperatureMin.HasValue)
            {
                forecast.low_f = ConversionMethods.CtoF(item.values.temperatureMin.Value);
                forecast.low_c = item.values.temperatureMin;
            }

            forecast.icon = (item.values.weatherCodeFullDay ?? item.values.weatherCode)?.ToString();

            // Extras
            forecast.extras = new ForecastExtras();
            if (item.values.temperatureApparent.HasValue)
            {
                forecast.extras.feelslike_f = ConversionMethods.CtoF(item.values.temperatureApparent.Value);
                forecast.extras.feelslike_c = item.values.temperatureApparent;
            }

            if (item.values.humidity.HasValue)
            {
                forecast.extras.humidity = (int)MathF.Round(item.values.humidity.Value);
            }

            if (item.values.dewPoint.HasValue)
            {
                forecast.extras.dewpoint_c = item.values.dewPoint.Value;
                forecast.extras.dewpoint_f = ConversionMethods.CtoF(item.values.dewPoint.Value);
            }

            forecast.extras.pop = item.values.precipitationProbability?.RoundToInt();
            if (item.values.cloudCover.HasValue)
            {
                forecast.extras.cloudiness = (int)MathF.Round(item.values.cloudCover.Value);
            }

            if (item.values.precipitationIntensity.HasValue)
            {
                forecast.extras.qpf_rain_mm = item.values.precipitationIntensity.Value;
                forecast.extras.qpf_rain_in = ConversionMethods.MMToIn(item.values.precipitationIntensity.Value);
            }

            if (item.values.snowAccumulation.HasValue)
            {
                forecast.extras.qpf_snow_cm = item.values.snowAccumulation.Value / 10;
                forecast.extras.qpf_snow_in = ConversionMethods.MMToIn(item.values.snowAccumulation.Value);
            }

            if (item.values.pressureSeaLevel.HasValue)
            {
                forecast.extras.pressure_mb = item.values.pressureSeaLevel.Value;
                forecast.extras.pressure_in = ConversionMethods.MBToInHg(item.values.pressureSeaLevel.Value);
            }

            if (item.values.windDirection.HasValue)
            {
                forecast.extras.wind_degrees = (int)MathF.Round(item.values.windDirection.Value);
            }

            if (item.values.windSpeed.HasValue)
            {
                forecast.extras.wind_mph = ConversionMethods.MSecToMph(item.values.windSpeed.Value);
                forecast.extras.wind_kph = ConversionMethods.MSecToKph(item.values.windSpeed.Value);
            }

            if (item.values.windGust.HasValue)
            {
                forecast.extras.windgust_mph = ConversionMethods.MSecToMph(item.values.windGust.Value);
                forecast.extras.windgust_kph = ConversionMethods.MSecToKph(item.values.windGust.Value);
            }

            if (item.values.visibility.HasValue)
            {
                forecast.extras.visibility_mi = ConversionMethods.KmToMi(item.values.visibility.Value);
                forecast.extras.visibility_km = item.values.visibility;
            }

            return forecast;
        }

        public static TextForecast CreateTextForecast(this TomorrowIOWeatherProvider _, Interval item)
        {
            var txtForecast = new TextForecast();

            txtForecast.date = item.startTime.UtcDateTime;

            var fcastStr = new StringBuilder().Apply(sb =>
            {
                var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.TomorrowIo);

                sb.AppendFormat("{0} - {1}", ResStrings.label_day,
                    provider.GetWeatherCondition(item.values.weatherCodeDay?.ToString()));
                sb.AppendLine();
                sb.AppendFormat("{0} - {1}", ResStrings.label_night,
                    provider.GetWeatherCondition(item.values.weatherCodeNight?.ToString()));
            }).ToString();

            txtForecast.fcttext = fcastStr;
            txtForecast.fcttext_metric = fcastStr;

            return txtForecast;
        }

        public static HourlyForecast CreateHourlyForecast(this TomorrowIOWeatherProvider _, Interval item)
        {
            var hrf = new HourlyForecast();

            hrf.date = item.startTime;

            if (item.values.temperatureMax.HasValue)
            {
                hrf.high_f = ConversionMethods.CtoF(item.values.temperatureMax.Value);
                hrf.high_c = item.values.temperatureMax;
            }

            hrf.icon = item.values.weatherCode?.ToString();

            // Extras
            hrf.extras = new ForecastExtras();
            if (item.values.temperatureApparent.HasValue)
            {
                hrf.extras.feelslike_f = ConversionMethods.CtoF(item.values.temperatureApparent.Value);
                hrf.extras.feelslike_c = item.values.temperatureApparent;
            }

            if (item.values.humidity.HasValue)
            {
                hrf.extras.humidity = (int)MathF.Round(item.values.humidity.Value);
            }

            if (item.values.dewPoint.HasValue)
            {
                hrf.extras.dewpoint_c = item.values.dewPoint.Value;
                hrf.extras.dewpoint_f = ConversionMethods.CtoF(item.values.dewPoint.Value);
            }

            hrf.extras.pop = item.values.precipitationProbability?.RoundToInt();
            if (item.values.cloudCover.HasValue)
            {
                hrf.extras.cloudiness = (int)MathF.Round(item.values.cloudCover.Value);
            }

            if (item.values.precipitationIntensity.HasValue)
            {
                hrf.extras.qpf_rain_mm = item.values.precipitationIntensity.Value;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(item.values.precipitationIntensity.Value);
            }

            if (item.values.snowAccumulation.HasValue)
            {
                hrf.extras.qpf_snow_cm = item.values.snowAccumulation.Value / 10;
                hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(item.values.snowAccumulation.Value);
            }

            if (item.values.pressureSeaLevel.HasValue)
            {
                hrf.extras.pressure_mb = item.values.pressureSeaLevel.Value;
                hrf.extras.pressure_in = ConversionMethods.MBToInHg(item.values.pressureSeaLevel.Value);
            }

            if (item.values.windDirection.HasValue)
            {
                hrf.extras.wind_degrees = (int)MathF.Round(item.values.windDirection.Value);
                hrf.wind_degrees = hrf.extras.wind_degrees;
            }

            if (item.values.windSpeed.HasValue)
            {
                hrf.extras.wind_mph = ConversionMethods.MSecToMph(item.values.windSpeed.Value);
                hrf.wind_mph = hrf.extras.wind_mph;

                hrf.extras.wind_kph = ConversionMethods.MSecToKph(item.values.windSpeed.Value);
                hrf.wind_kph = hrf.extras.wind_kph;
            }

            if (item.values.windGust.HasValue)
            {
                hrf.extras.windgust_mph = ConversionMethods.MSecToMph(item.values.windGust.Value);
                hrf.extras.windgust_kph = ConversionMethods.MSecToKph(item.values.windGust.Value);
            }

            if (item.values.visibility.HasValue)
            {
                hrf.extras.visibility_mi = ConversionMethods.KmToMi(item.values.visibility.Value);
                hrf.extras.visibility_km = item.values.visibility;
            }

            return hrf;
        }

        public static MinutelyForecast CreateMinutelyForecast(this TomorrowIOWeatherProvider _, Interval item)
        {
            return new MinutelyForecast()
            {
                date = item.startTime,
                rain_mm = item.values.precipitationIntensity,
            };
        }

        public static Condition CreateCondition(this TomorrowIOWeatherProvider _, Interval item)
        {
            var condition = new Condition();

            condition.weather = null;

            if (item.values.temperature.HasValue)
            {
                condition.temp_f = ConversionMethods.CtoF(item.values.temperature.Value);
                condition.temp_c = item.values.temperature;
            }

            condition.wind_degrees = item.values.windDirection?.RoundToInt();
            if (item.values.windSpeed.HasValue)
            {
                condition.wind_mph = ConversionMethods.MSecToMph(item.values.windSpeed.Value);
                condition.wind_kph = ConversionMethods.MSecToKph(item.values.windSpeed.Value);
                condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(item.values.windSpeed.Value));
            }

            if (item.values.windGust.HasValue)
            {
                condition.windgust_mph = ConversionMethods.MSecToMph(item.values.windGust.Value);
                condition.windgust_kph = ConversionMethods.MSecToKph(item.values.windGust.Value);
            }

            if (item.values.temperatureApparent.HasValue)
            {
                condition.feelslike_f = ConversionMethods.CtoF(item.values.temperatureApparent.Value);
                condition.feelslike_c = item.values.temperatureApparent;
            }

            condition.icon = item.values.weatherCode?.ToString();

            if (item.values.temperatureMax.HasValue)
            {
                condition.high_c = item.values.temperatureMax;
                condition.high_f = ConversionMethods.CtoF(item.values.temperatureMax.Value);
            }

            if (item.values.temperatureMin.HasValue)
            {
                condition.low_c = item.values.temperatureMin;
                condition.low_f = ConversionMethods.CtoF(item.values.temperatureMin.Value);
            }

            condition.airQuality = new AirQuality()
            {
                index = item.values.epaIndex,
                pm25 = item.values.particulateMatter25?.Let(it =>
                    _.RunCatching(() => AirQualityUtils.AQIPM2_5(it)).GetOrNull()),
                pm10 = item.values.particulateMatter10?.Let(it =>
                    _.RunCatching(() => AirQualityUtils.AQIPM10(it)).GetOrNull()),
                o3 = item.values.pollutantO3?.Let(it => _.RunCatching(() => AirQualityUtils.AQIO3(it)).GetOrNull()),
                no2 = item.values.pollutantNO2?.Let(it => _.RunCatching(() => AirQualityUtils.AQINO2(it)).GetOrNull()),
                co = item.values.pollutantCO?.Let(it => _.RunCatching(() => AirQualityUtils.AQICO(it)).GetOrNull()),
                so2 = item.values.pollutantSO2?.Let(it => _.RunCatching(() => AirQualityUtils.AQISO2(it)).GetOrNull()),
            };

            condition.pollen = new Pollen()
            {
                treePollenCount = item.values.treeIndex switch
                {
                    1 or 2 => Pollen.PollenCount.Low,
                    3 => Pollen.PollenCount.Moderate,
                    4 => Pollen.PollenCount.High,
                    5 => Pollen.PollenCount.VeryHigh,
                    _ => Pollen.PollenCount.Unknown
                },
                grassPollenCount = item.values.grassIndex switch
                {
                    1 or 2 => Pollen.PollenCount.Low,
                    3 => Pollen.PollenCount.Moderate,
                    4 => Pollen.PollenCount.High,
                    5 => Pollen.PollenCount.VeryHigh,
                    _ => Pollen.PollenCount.Unknown
                },
                ragweedPollenCount = item.values.weedIndex switch
                {
                    1 or 2 => Pollen.PollenCount.Low,
                    3 => Pollen.PollenCount.Moderate,
                    4 => Pollen.PollenCount.High,
                    5 => Pollen.PollenCount.VeryHigh,
                    _ => Pollen.PollenCount.Unknown
                }
            };

            condition.observation_time = item.startTime;

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this TomorrowIOWeatherProvider _, Interval item)
        {
            var atmosphere = new Atmosphere();

            atmosphere.humidity = item.values.humidity?.RoundToInt();

            if (item.values.pressureSeaLevel.HasValue)
            {
                atmosphere.pressure_mb = item.values.pressureSeaLevel;
                atmosphere.pressure_in = ConversionMethods.MBToInHg(item.values.pressureSeaLevel.Value);
            }

            atmosphere.pressure_trend = string.Empty;

            if (item.values.visibility.HasValue)
            {
                atmosphere.visibility_mi = ConversionMethods.KmToMi(item.values.visibility.Value);
                atmosphere.visibility_km = item.values.visibility.Value;
            }

            if (item.values.dewPoint.HasValue)
            {
                atmosphere.dewpoint_f = ConversionMethods.CtoF(item.values.dewPoint.Value);
                atmosphere.dewpoint_c = item.values.dewPoint.Value;
            }

            return atmosphere;
        }

        public static Astronomy CreateAstronomy(this TomorrowIOWeatherProvider _, Interval item)
        {
            var astronomy = new Astronomy();

            try
            {
                astronomy.sunrise = item.values.sunriseTime.UtcDateTime;
            }
            catch
            {
            }

            try
            {
                astronomy.sunset = item.values.sunsetTime.UtcDateTime;
            }
            catch
            {
            }

            astronomy.moonphase = item.values.moonPhase switch
            {
                0 => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
                1 => new MoonPhase(MoonPhase.MoonPhaseType.WaxingCrescent),
                2 => new MoonPhase(MoonPhase.MoonPhaseType.FirstQtr),
                3 => new MoonPhase(MoonPhase.MoonPhaseType.WaxingGibbous),
                4 => new MoonPhase(MoonPhase.MoonPhaseType.FullMoon),
                5 => new MoonPhase(MoonPhase.MoonPhaseType.WaningGibbous),
                6 => new MoonPhase(MoonPhase.MoonPhaseType.LastQtr),
                7 => new MoonPhase(MoonPhase.MoonPhaseType.WaningCrescent),
                _ => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
            };

            // If the sun won't set/rise, set time to the future
            if (astronomy.sunrise == null)
            {
                astronomy.sunrise = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }

            if (astronomy.sunset == null)
            {
                astronomy.sunset = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }

            if (astronomy.moonrise == null)
            {
                astronomy.moonrise = DateTime.MinValue;
            }

            if (astronomy.moonset == null)
            {
                astronomy.moonset = DateTime.MinValue;
            }

            return astronomy;
        }

        public static Precipitation CreatePrecipitation(this TomorrowIOWeatherProvider _, Interval item)
        {
            var precip = new Precipitation();

            precip.pop = item.values.precipitationProbability?.RoundToInt();
            precip.cloudiness = item.values.cloudCover?.RoundToInt();

            if (item.values.precipitationIntensity.HasValue)
            {
                precip.qpf_rain_in = ConversionMethods.MMToIn(item.values.precipitationIntensity.Value);
                precip.qpf_rain_mm = item.values.precipitationIntensity.Value;
            }

            return precip;
        }

        public static AirQuality CreateAQIForecast(this TomorrowIOWeatherProvider _, Interval item)
        {
            return new AirQuality
            {
                date = item.startTime.ToUniversalTime().Date,
                pm25 = item.values.particulateMatter25?.Let(it =>
                    _.RunCatching(() => AirQualityUtils.AQIPM2_5(it)).GetOrNull()),
                pm10 = item.values.particulateMatter10?.Let(it =>
                    _.RunCatching(() => AirQualityUtils.AQIPM10(it)).GetOrNull()),
                o3 = item.values.pollutantO3?.Let(it => _.RunCatching(() => AirQualityUtils.AQIO3(it)).GetOrNull()),
                no2 = item.values.pollutantNO2?.Let(it => _.RunCatching(() => AirQualityUtils.AQINO2(it)).GetOrNull()),
                co = item.values.pollutantCO?.Let(it => _.RunCatching(() => AirQualityUtils.AQICO(it)).GetOrNull()),
                so2 = item.values.pollutantSO2?.Let(it => _.RunCatching(() => AirQualityUtils.AQISO2(it)).GetOrNull()),
            }.Apply(it => { it.index = item.values.epaIndex ?? it.GetIndexFromData(); });
        }
    }
}