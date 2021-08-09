using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleWeather.WeatherData
{
    public partial class Weather
    {
        public Weather(AccuWeather.DailyForecastRootobject dailyRoot, AccuWeather.HourlyForecastRootobject hourlyRoot, AccuWeather.CurrentRootobject currentRoot)
        {
            var currentItem = currentRoot.Items[0];
            var observationTime = currentItem.LocalObservationDateTime;
            var now = DateTimeOffset.UtcNow.ToOffset(observationTime.Offset);

            update_time = now;

            location = new Location(currentRoot);

            forecast = new List<Forecast>(dailyRoot.DailyForecasts.Length);
            txt_forecast = new List<TextForecast>(dailyRoot.DailyForecasts.Length);
            var haveTodaysForecast = false;
            foreach (var fcast in dailyRoot.DailyForecasts)
            {
                if (!haveTodaysForecast && fcast.Date.Date.Equals(now.Date))
                {
                    astronomy = new Astronomy(fcast);
                    condition = new Condition(currentItem, fcast);
                    haveTodaysForecast = true;
                }

                forecast.Add(new Forecast(fcast));
                txt_forecast.Add(new TextForecast(fcast));
            }

            hr_forecast = new List<HourlyForecast>(hourlyRoot.Items.Length);
            foreach (var fcast in hourlyRoot.Items)
            {
                hr_forecast.Add(new HourlyForecast(fcast));
            }

            if (condition == null)
            {
                condition = new Condition(currentItem);
            }
            atmosphere = new Atmosphere(currentItem);
            precipitation = new Precipitation(currentItem);

            // Weather summary
            if (dailyRoot.Headline?.EffectiveEpochDate != null && dailyRoot.Headline?.EndEpochDate != null)
            {
                var effectiveDate = DateTimeOffset.FromUnixTimeSeconds(dailyRoot.Headline.EffectiveEpochDate.Value).ToOffset(observationTime.Offset);
                var endDate = DateTimeOffset.FromUnixTimeSeconds(dailyRoot.Headline.EndEpochDate.Value).ToOffset(observationTime.Offset);

                if (observationTime >= effectiveDate && observationTime <= endDate)
                {
                    condition.summary = dailyRoot.Headline?.Text;
                }
            }

            ttl = 180;
            source = WeatherAPI.AccuWeather;
        }
    }

    public partial class Location
    {
        public Location(AccuWeather.CurrentRootobject current)
        {
            /* Use name from location provider */
            name = null;
            latitude = null;
            longitude = null;
            tz_long = null;
        }
    }

    public partial class Forecast
    {
        public Forecast(AccuWeather.Dailyforecast daily)
        {
            date = daily.Date.DateTime;

            if (daily.Temperature.Maximum.Value.HasValue)
            {
                high_f = ConversionMethods.CtoF(daily.Temperature.Maximum.Value.Value);
                high_c = daily.Temperature.Maximum.Value;
            }
            if (daily.Temperature.Minimum.Value.HasValue)
            {
                low_f = ConversionMethods.CtoF(daily.Temperature.Minimum.Value.Value);
                low_c = daily.Temperature.Minimum.Value;
            }

            condition = daily.Day?.IconPhrase ?? daily.Night?.IconPhrase;
            icon = WeatherManager.GetProvider(WeatherAPI.AccuWeather)
                .GetWeatherIcon(false, daily.Day?.Icon?.ToString() ?? daily.Night?.Icon?.ToString());

            // Extras
            extras = new ForecastExtras();

            extras.feelslike_c = daily.RealFeelTemperature?.Maximum?.Value ?? daily.RealFeelTemperature?.Minimum?.Value;
            if (extras.feelslike_c.HasValue) extras.feelslike_f = ConversionMethods.CtoF(extras.feelslike_c.Value);

            extras.uv_index = daily.AirAndPollen?.FirstOrDefault(it => it?.Name == "UVIndex")?.Value;
            extras.pop = daily.Day?.PrecipitationProbability ?? daily.Night?.PrecipitationProbability;
            extras.cloudiness = daily.Day?.CloudCover?.RoundToInt() ?? daily.Night?.CloudCover?.RoundToInt();

            extras.qpf_rain_mm = daily.Day?.Rain?.Value ?? daily.Night?.Rain?.Value;
            if (extras.qpf_rain_mm.HasValue) extras.qpf_rain_in = ConversionMethods.MMToIn(extras.qpf_rain_mm.Value);
            extras.qpf_snow_cm = daily.Day?.Snow?.Value ?? daily.Night?.Snow?.Value;
            if (extras.qpf_snow_in.HasValue) extras.qpf_snow_in = ConversionMethods.MMToIn(extras.qpf_snow_cm.Value * 10);

            if (daily.Day?.Wind != null)
            {
                extras.wind_degrees = daily.Day?.Wind?.Direction?.Degrees?.RoundToInt();
                extras.wind_kph = daily.Day?.Wind?.Speed.Value;
                if (extras.wind_kph.HasValue) extras.wind_mph = ConversionMethods.KphToMph(extras.wind_kph.Value);
            }
            else if (daily.Night?.Wind != null)
            {
                extras.wind_degrees = daily.Night?.Wind?.Direction?.Degrees?.RoundToInt();
                extras.wind_kph = daily.Night?.Wind?.Speed.Value;
                if (extras.wind_kph.HasValue) extras.wind_mph = ConversionMethods.KphToMph(extras.wind_kph.Value);
            }

            if (daily.Day?.WindGust != null)
            {
                extras.windgust_kph = daily.Day?.WindGust?.Speed.Value;
                if (extras.windgust_kph.HasValue) extras.windgust_mph = ConversionMethods.KphToMph(extras.windgust_kph.Value);
            }
            else if (daily.Night?.WindGust != null)
            {
                extras.windgust_kph = daily.Night?.WindGust?.Speed.Value;
                if (extras.windgust_kph.HasValue) extras.windgust_mph = ConversionMethods.KphToMph(extras.windgust_kph.Value);
            }
        }
    }

    public partial class TextForecast
    {
        public TextForecast(AccuWeather.Dailyforecast daily)
        {
            date = daily.Date;

            var ResLoader = SimpleLibrary.GetInstance().ResLoader;
            var fctStr = new StringBuilder();
            if (daily.Day != null)
            {
                fctStr.Append($"{ResLoader.GetString("label_day")} - {daily.Day.LongPhrase}");
            }
            if (daily.Night != null)
            {
                if (fctStr.Length > 0) fctStr.AppendLine();
                fctStr.Append($"{ResLoader.GetString("label_night")} - {daily.Night.LongPhrase}");
            }

            fcttext = fctStr.ToString();
            fcttext_metric = fcttext;
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(AccuWeather.HourlyItem hourly)
        {
            date = hourly.DateTime;

            high_c = hourly.Temperature?.Value;
            high_f = hourly.Temperature?.Value?.Let((i) =>
            {
                return ConversionMethods.CtoF(i);
            });

            condition = hourly.IconPhrase;
            icon = WeatherManager.GetProvider(WeatherAPI.AccuWeather)
                .GetWeatherIcon(!hourly.IsDaylight.GetValueOrDefault(), hourly.WeatherIcon?.ToString());

            // Extras
            extras = new ForecastExtras();
            hourly.RealFeelTemperature?.Value?.Let((it) =>
            {
                extras.feelslike_c = it;
                extras.feelslike_f = ConversionMethods.CtoF(it);
            });
            extras.humidity = hourly.RelativeHumidity;
            hourly.DewPoint?.Value?.Let((it) =>
            {
                extras.dewpoint_c = it;
                extras.dewpoint_f = ConversionMethods.CtoF(it);
            });

            extras.uv_index = hourly.UVIndex;
            extras.pop = hourly.PrecipitationProbability;
            extras.cloudiness = hourly.CloudCover;

            hourly.Rain?.Value?.Let((it) =>
            {
                extras.qpf_rain_mm = it;
                extras.qpf_rain_in = ConversionMethods.MMToIn(it);
            });
            hourly.Snow?.Value?.Let((it) =>
            {
                extras.qpf_snow_cm = it;
                extras.qpf_snow_in = ConversionMethods.MMToIn(it * 10);
            });

            hourly.Wind?.Speed?.Value?.Let((it) =>
            {
                extras.wind_kph = it;
                wind_kph = it;

                extras.wind_mph = ConversionMethods.KphToMph(it);
                wind_mph = extras.wind_mph;
            });
            extras.wind_degrees = hourly.Wind?.Direction?.Degrees?.RoundToInt();

            hourly.Visibility?.Value?.Let((it) =>
            {
                extras.visibility_km = it;
                extras.visibility_mi = ConversionMethods.KmToMi(it);
            });

            hourly.WindGust?.Speed?.Value?.Let((it) =>
            {
                extras.windgust_kph = it;
                extras.windgust_mph = ConversionMethods.KphToMph(it);
            });
        }
    }

    public partial class Astronomy
    {
        public Astronomy(AccuWeather.Dailyforecast daily)
        {
            sunrise = daily.Sun?.Rise?.DateTime ?? DateTime.Now.Date.AddYears(1).AddTicks(-1);
            sunset = daily.Sun?.Set?.DateTime ?? DateTime.Now.Date.AddYears(1).AddTicks(-1);

            moonrise = daily.Moon?.Rise?.DateTime ?? DateTime.MinValue;
            moonset = daily.Moon?.Set?.DateTime ?? DateTime.MinValue;

            daily.Moon?.Phase?.Let((it) =>
            {
                moonphase = it.ToLowerInvariant() switch
                {
                    "new" or "newmoon" => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
                    "waxingcrescent" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingCrescent),
                    "first" or "firstquarter" => new MoonPhase(MoonPhase.MoonPhaseType.FirstQtr),
                    "waxinggibbous" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingGibbous),
                    "full" or "fullmoon" => new MoonPhase(MoonPhase.MoonPhaseType.FullMoon),
                    "waninggibbous" => new MoonPhase(MoonPhase.MoonPhaseType.WaningGibbous),
                    "third" or "last" => new MoonPhase(MoonPhase.MoonPhaseType.LastQtr),
                    "waningcrescent" => new MoonPhase(MoonPhase.MoonPhaseType.WaningCrescent),
                    _ => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
                };
            });
        }
    }

    public partial class Condition
    {
        public Condition(AccuWeather.CurrentsItem current, AccuWeather.Dailyforecast daily = null)
        {
            weather = current.WeatherText;
            icon = WeatherManager.GetProvider(WeatherAPI.AccuWeather)
                .GetWeatherIcon(!current.IsDayTime.GetValueOrDefault(), current.WeatherIcon?.ToString());

            temp_f = current.Temperature?.Imperial?.Value;
            temp_c = current.Temperature?.Metric?.Value;

            wind_degrees = current.Wind?.Direction?.Degrees;
            wind_mph = current.Wind?.Speed?.Imperial?.Value;
            wind_kph = current.Wind?.Speed?.Metric?.Value;
            windgust_mph = current.WindGust?.Speed?.Imperial?.Value;
            windgust_kph = current.WindGust?.Speed?.Metric?.Value;

            feelslike_f = current.RealFeelTemperature?.Imperial?.Value;
            feelslike_c = current.RealFeelTemperature?.Metric?.Value;

            current.Wind?.Speed?.Imperial?.Value?.Let((it) =>
            {
                beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(it.RoundToInt()));
            });

            uv = new UV()
            {
                index = current.UVIndex
            };

            if (current.TemperatureSummary?.Past6HourRange?.Maximum?.Imperial?.Value != null &&
                current.TemperatureSummary?.Past6HourRange?.Maximum?.Metric?.Value != null)
            {
                high_f = current.TemperatureSummary?.Past6HourRange?.Maximum?.Imperial?.Value;
                high_c = current.TemperatureSummary?.Past6HourRange?.Maximum?.Metric?.Value;
            }
            else if (current.TemperatureSummary?.Past12HourRange?.Maximum?.Imperial?.Value != null &&
                  current.TemperatureSummary?.Past12HourRange?.Maximum?.Metric?.Value != null)
            {
                high_f = current.TemperatureSummary?.Past12HourRange?.Maximum?.Imperial?.Value;
                high_c = current.TemperatureSummary?.Past12HourRange?.Maximum?.Metric?.Value;
            }
            else if (current.TemperatureSummary?.Past24HourRange?.Maximum?.Imperial?.Value != null &&
                  current.TemperatureSummary?.Past24HourRange?.Maximum?.Metric?.Value != null)
            {
                high_f = current.TemperatureSummary?.Past24HourRange?.Maximum?.Imperial?.Value;
                high_c = current.TemperatureSummary?.Past24HourRange?.Maximum?.Metric?.Value;
            }

            if (current.TemperatureSummary?.Past6HourRange?.Minimum?.Imperial?.Value != null &&
                    current.TemperatureSummary?.Past6HourRange?.Minimum?.Metric?.Value != null)
            {
                low_f = current.TemperatureSummary?.Past6HourRange?.Minimum?.Imperial?.Value;
                low_c = current.TemperatureSummary?.Past6HourRange?.Minimum?.Metric?.Value;
            }
            else if (current.TemperatureSummary?.Past12HourRange?.Minimum?.Imperial?.Value != null &&
                  current.TemperatureSummary?.Past12HourRange?.Minimum?.Metric?.Value != null)
            {
                low_f = current.TemperatureSummary?.Past12HourRange?.Minimum?.Imperial?.Value;
                low_c = current.TemperatureSummary?.Past12HourRange?.Minimum?.Metric?.Value;
            }
            else if (current.TemperatureSummary?.Past24HourRange?.Minimum?.Imperial?.Value != null &&
                  current.TemperatureSummary?.Past24HourRange?.Minimum?.Metric?.Value != null)
            {
                low_f = current.TemperatureSummary?.Past24HourRange?.Minimum?.Imperial?.Value;
                low_c = current.TemperatureSummary?.Past24HourRange?.Minimum?.Metric?.Value;
            }

            daily?.AirAndPollen?.FirstOrDefault(it => it?.Name == "AirQuality")?.Value?.Let((it) =>
            {
                airQuality = new AirQuality()
                {
                    index = it
                };
            });

            if (daily?.AirAndPollen?.Length > 0)
            {
                int? treePollenValue = null;
                int? grassPollenValue = null;
                int? ragweedPollenValue = null;

                foreach (var item in daily.AirAndPollen)
                {
                    if (item?.Name == "Grass")
                    {
                        grassPollenValue = item.Value;
                    }
                    if (item?.Name == "Tree")
                    {
                        treePollenValue = item.Value;
                    }
                    if (item?.Name == "Ragweed")
                    {
                        ragweedPollenValue = item.Value;
                    }
                }

                if (grassPollenValue != null || treePollenValue != null || ragweedPollenValue != null)
                {
                    pollen = new Pollen()
                    {
                        treePollenCount = treePollenValue switch
                        {
                            >= 0 and <= 14 => Pollen.PollenCount.Low,
                            >= 15 and <= 89 => Pollen.PollenCount.Moderate,
                            >= 90 and <= 1499 => Pollen.PollenCount.High,
                            >= 1500 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        },
                        grassPollenCount = grassPollenValue switch
                        {
                            >= 0 and <= 4 => Pollen.PollenCount.Low,
                            >= 5 and <= 19 => Pollen.PollenCount.Moderate,
                            >= 20 and <= 199 => Pollen.PollenCount.High,
                            >= 200 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        },
                        ragweedPollenCount = ragweedPollenValue switch
                        {
                            >= 0 and <= 9 => Pollen.PollenCount.Low,
                            >= 10 and <= 49 => Pollen.PollenCount.Moderate,
                            >= 50 and <= 499 => Pollen.PollenCount.High,
                            >= 500 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        }
                    };
                }
            }

            observation_time = current.LocalObservationDateTime;

            summary = daily?.Let((it) =>
            {
                var ResLoader = SimpleLibrary.GetInstance().ResLoader;
                var labelDay = ResLoader.GetString("label_day");
                var labelNite = ResLoader.GetString("label_night");

                var strBuilder = new StringBuilder();
                if (!String.IsNullOrWhiteSpace(it.Day?.LongPhrase))
                {
                    strBuilder.Append($"{labelDay} - {it.Day.LongPhrase}");
                }
                if (!String.IsNullOrWhiteSpace(it.Night?.LongPhrase))
                {
                    if (strBuilder.Length > 0) strBuilder.AppendLine();
                    strBuilder.Append($"{labelNite} - {it.Night.LongPhrase}");
                }

                return strBuilder.ToString();
            });
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(AccuWeather.CurrentsItem current)
        {
            humidity = current.RelativeHumidity;

            pressure_mb = current.Pressure?.Metric?.Value;
            pressure_in = current.Pressure?.Imperial?.Value;
            pressure_trend = current.PressureTendency?.Code switch
            {
                "F" => "Falling",
                "R" => "Rising",
                _ => string.Empty
            };

            visibility_mi = current.Visibility?.Imperial?.Value;
            visibility_km = current.Visibility?.Metric?.Value;

            dewpoint_f = current.DewPoint?.Imperial?.Value;
            dewpoint_c = current.DewPoint?.Metric?.Value;
        }
    }

    public partial class Precipitation
    {
        public Precipitation(AccuWeather.CurrentsItem current)
        {
            cloudiness = current.CloudCover;

            if (current.PrecipitationType == "Rain" || current.PrecipitationType == "Mixed")
            {
                qpf_rain_in = current.Precip1hr?.Imperial?.Value ?? current.PrecipitationSummary?.PastHour?.Imperial?.Value;

                if ((current.Precip1hr?.Metric?.Unit ?? current.PrecipitationSummary?.PastHour?.Metric?.Unit) == "mm")
                {
                    qpf_rain_mm = current.Precip1hr?.Metric?.Value ?? current.PrecipitationSummary?.PastHour?.Metric?.Value;
                }
                else if ((current.Precip1hr?.Metric?.Unit ?? current.PrecipitationSummary?.PastHour?.Metric?.Unit) == "cm")
                {
                    qpf_rain_mm = current.Precip1hr?.Metric?.Value?.Times(10) ?? current.PrecipitationSummary?.PastHour?.Metric?.Value?.Times(10);
                }
            }
            else if (current.PrecipitationType == "Snow" || current.PrecipitationType == "Ice")
            {
                qpf_snow_in = current.Precip1hr?.Imperial?.Value ?? current.PrecipitationSummary?.PastHour?.Imperial?.Value;

                if ((current.Precip1hr?.Metric?.Unit ?? current.PrecipitationSummary?.PastHour?.Metric?.Unit) == "mm")
                {
                    qpf_snow_cm = current.Precip1hr?.Metric?.Value?.Div(10) ?? current.PrecipitationSummary?.PastHour?.Metric?.Value;
                }
                else if ((current.Precip1hr?.Metric?.Unit ?? current.PrecipitationSummary?.PastHour?.Metric?.Unit) == "cm")
                {
                    qpf_snow_cm = current.Precip1hr?.Metric?.Value ?? current.PrecipitationSummary?.PastHour?.Metric?.Value;
                }
            }
        }
    }
}