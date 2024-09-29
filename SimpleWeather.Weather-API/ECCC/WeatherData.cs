using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleWeather.Weather_API.ECCC
{
    public static partial class ECCCWeatherProviderExtensions
    {
        public static Weather CreateWeatherData(this ECCCWeatherProvider _, LocationsItem root)
        {
            var weather = new Weather();

            var now = _.RunCatching(() => DateTimeOffset.FromUnixTimeSeconds(root.lastUpdated))
                .GetOrDefault(DateTimeOffset.UtcNow);

            weather.location = _.CreateLocation(root);
            weather.update_time = now;

            var startDate = root.dailyFcst?.dailyIssuedTimeEpoch?.TryParseLong()?.Let(it =>
            {
                return DateTimeOffset.FromUnixTimeSeconds(it).UtcDateTime;
            });

            if (startDate != null)
            {
                root.dailyFcst?.daily?.GroupBy(it => it.date)?.Let(entry =>
                {
                    var fcasts = new List<Forecast>(entry.Count());
                    var txtFcasts = new List<TextForecast>(entry.Count());

                    entry.ForEach(kv =>
                    {
                        var values = kv.OrderBy(it => it.periodID);
                        var day = values.FirstOrDefault(it => it.temperature?.periodHigh != null);
                        var night = values.FirstOrDefault(it => it.temperature?.periodLow != null);

                        fcasts.Add(_.CreateForecast(day, night).Apply(it =>
                        {
                            it.date = startDate.Value;
                        }));
                        txtFcasts.Add(_.CreateTextForecast(day, night).Apply(it =>
                        {
                            it.date = new DateTimeOffset(startDate.Value, TimeSpan.Zero);
                        }));
                        startDate = startDate.Value.AddDays(1);
                    });

                    weather.forecast = fcasts;
                    weather.txt_forecast = txtFcasts;
                });
            }

            weather.hr_forecast = root.hourlyFcst?.hourly?.Select(it =>
            {
                return _.CreateHourlyForecast(it);
            })?.ToList();

            root.observation?.Let(it =>
            {
                weather.condition = _.CreateCondition(it);
                weather.atmosphere = _.CreateAtmosphere(it);
            });

            weather.astronomy = _.CreateAstronomy(root.riseSet);
            weather.weather_alerts = _.CreateWeatherAlerts(root.alert);

            weather.ttl = 180;

            if ((!weather.condition.high_f.HasValue || !weather.condition.low_f.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f;
                weather.condition.high_c = weather.forecast[0].high_c;
                weather.condition.low_f = weather.forecast[0].low_f;
                weather.condition.low_c = weather.forecast[0].low_c;
            }

            weather.source = WeatherAPI.ECCC;

            return weather;
        }

        public static Location CreateLocation(this ECCCWeatherProvider _, LocationsItem root)
        {
            var location = new Location();

            if (root.displayName != null && root.observation?.provinceCode != null)
            {
                location.name = $"{root.displayName}, {root.observation.provinceCode}";
            }
            else
            {
                location.name = root.displayName;
            }

            return location;
        }

        public static Condition CreateCondition(this ECCCWeatherProvider _, Observation observation)
        {
            var condition = new Condition();

            condition.weather = observation.condition;

            condition.temp_f = (observation.temperature?.imperialUnrounded ?? observation.temperature?.imperial)?.TryParseFloat();
            condition.temp_c = (observation.temperature?.metricUnrounded ?? observation.temperature?.metric)?.TryParseFloat();

            condition.wind_degrees = observation.windBearing?.TryParseFloat()?.RoundToInt();

            condition.wind_mph = observation.windSpeed?.imperial?.TryParseFloat();
            condition.wind_kph = observation.windSpeed?.metric?.TryParseFloat();

            condition.windgust_mph = observation.windGust?.imperial?.TryParseFloat();
            condition.windgust_kph = observation.windGust?.metric?.TryParseFloat();

            condition.feelslike_f = observation.feelsLike?.imperial?.TryParseFloat();
            condition.feelslike_c = observation.feelsLike?.metric?.TryParseFloat();

            condition.icon = observation.iconCode;

            condition.beaufort = condition.wind_mph?.Let(it =>
            {
                return new Beaufort(WeatherUtils.GetBeaufortScale(it.RoundToInt()));
            });

            condition.observation_time = observation.timeStamp;

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this ECCCWeatherProvider _, Observation observation)
        {
            var atmosphere = new Atmosphere();

            atmosphere.humidity = observation.humidity?.TryParseInt();

            atmosphere.pressure_mb = observation.pressure?.metric?.TryParseFloat()?.Let(it =>
            {
                return ConversionMethods.PaToMB(it * 1000f);
            });
            atmosphere.pressure_in = observation.pressure?.imperial?.TryParseFloat();
            atmosphere.pressure_trend = observation.tendency?.Let(it =>
            {
                return it switch
                {
                    "falling" => "-",
                    "rising" => "+",
                    _ => "",
                };
            });

            atmosphere.visibility_mi = observation.visibility?.imperial?.TryParseFloat();
            atmosphere.visibility_km = observation.visibility?.metric?.TryParseFloat();

            atmosphere.dewpoint_f = (observation.dewpoint?.imperialUnrounded ?? observation.dewpoint?.imperial)?.TryParseFloat();
            atmosphere.dewpoint_c = (observation.dewpoint?.metricUnrounded ?? observation.dewpoint?.metric)?.TryParseFloat();

            return atmosphere;
        }

        public static Astronomy CreateAstronomy(this ECCCWeatherProvider _, Riseset riseset)
        {
            var astronomy = new Astronomy();

            riseset?.rise?.Let(it =>
            {
                astronomy.sunrise = it.epochTimeRounded?.TryParseLong()?.Let(riseEpoch =>
                {
                    return DateTimeOffset.FromUnixTimeSeconds(riseEpoch).UtcDateTime;
                }) ?? DateTime.MinValue;
            });
            riseset?.set?.Let(it =>
            {
                astronomy.sunset = it.epochTimeRounded?.TryParseLong()?.Let(riseEpoch =>
                {
                    return DateTimeOffset.FromUnixTimeSeconds(riseEpoch).UtcDateTime;
                }) ?? DateTime.MinValue;
            });

            return astronomy;
        }

        public static HourlyForecast CreateHourlyForecast(this ECCCWeatherProvider _, Hourly item)
        {
            var hrf = new HourlyForecast();

            hrf.date = item.epochTime?.Let(it =>
            {
                return DateTimeOffset.FromUnixTimeSeconds(it);
            }) ?? DateTimeOffset.MinValue;

            hrf.high_f = item.temperature?.imperial?.TryParseFloat();
            hrf.high_c = item.temperature?.metric?.TryParseFloat();

            hrf.wind_degrees = item.windDir?.Let(WeatherUtils.GetWindDirection);
            hrf.wind_mph = item.windSpeed?.imperial?.TryParseFloat();
            hrf.wind_kph = item.windSpeed?.metric?.TryParseFloat();

            hrf.icon = item.iconCode;
            hrf.condition = item.condition;

            // Extras
            hrf.extras = new ForecastExtras()
            {
                feelslike_f = item.feelsLike?.imperial?.TryParseFloat(),
                feelslike_c = item.feelsLike?.metric?.TryParseFloat(),
                pop = item.precip?.TryParseInt(),
                wind_degrees = hrf.wind_degrees,
                wind_mph = hrf.wind_mph,
                wind_kph = hrf.wind_kph,
                windgust_mph = item.windGust?.imperial?.TryParseFloat(),
                windgust_kph = item.windGust?.metric?.TryParseFloat(),
                uv_index = item.uv?.index?.TryParseFloat()
            };

            return hrf;
        }

        public static Forecast CreateForecast(this ECCCWeatherProvider _, Daily dayItem, Daily nightItem)
        {
            var forecast = new Forecast();

            // date is not in standard format to parse; set it manually
            var highFStr = dayItem?.temperature?.imperial;
            forecast.high_f = highFStr?.TryParseFloat();
            var highCStr = dayItem?.temperature?.metric;
            forecast.high_c = highCStr?.TryParseFloat();

            var lowFStr = nightItem?.temperature?.imperial;
            forecast.low_f = lowFStr?.TryParseFloat();
            var lowCStr = nightItem?.temperature?.metric;
            forecast.low_c = lowCStr?.TryParseFloat();

            forecast.condition = dayItem?.text?.Let(text =>
            {
                var tempText = dayItem.temperatureText?.TakeIf(it =>
                {
                    return highCStr != null && highFStr != null && !string.IsNullOrWhiteSpace(it);
                })?.Replace(highCStr, $"{highCStr}°C / {highFStr}°F"); // High 10°C / 50°F

                if (tempText != null)
                {
                    return text.Replace(dayItem.temperatureText, tempText);
                }
                else
                {
                    return text;
                }
            }) ?? nightItem?.text?.Let(text =>
            {
                var tempText = nightItem.temperatureText?.TakeIf(it =>
                {
                    return lowCStr != null && lowFStr != null && !string.IsNullOrWhiteSpace(it);
                })?.Replace(lowCStr, $"{lowCStr}°C / {lowFStr}°F"); // Low 10°C / 50°F

                if (tempText != null)
                {
                    return text.Replace(nightItem.temperatureText, tempText);
                }
                else
                {
                    return text;
                }
            });
            forecast.icon = dayItem?.iconCode ?? nightItem.iconCode;

            forecast.extras = new ForecastExtras();
            forecast.extras.pop = dayItem?.precip?.TryParseInt();

            // Humidex / Feels like
            (dayItem?.text?.IndexOf("Humidex "))?.TakeIf(it => it >= 0)?.Let(humidexStart =>
            {
                var substr = dayItem.text?.Substring(humidexStart);
                if (substr != null)
                {
                    var endStr = dayItem.text.Substring(humidexStart);
                    var endIndex = endStr.IndexOf('.');
                    if (endIndex >= 0)
                    {
                        var humidexStr = dayItem.text?.SubstringByIndex(humidexStart, endIndex + humidexStart);
                        var humidexMetric = humidexStr?.RemovePrefix("Humidex ")?.TryParseFloat();

                        if (humidexMetric.HasValue)
                        {
                            forecast.extras.feelslike_c = humidexMetric;
                            forecast.extras.feelslike_f = ConversionMethods.CtoF(humidexMetric.Value);
                        }
                    }
                }
            });

            return forecast;
        }

        public static TextForecast CreateTextForecast(this ECCCWeatherProvider _, Daily dayItem, Daily nightItem)
        {
            var forecast = new TextForecast();

            // date is not in standard format to parse; set it manually
            var highF = dayItem?.temperature?.imperial;
            var highC = dayItem?.temperature?.metric;

            var lowF = nightItem?.temperature?.imperial;
            var lowC = nightItem?.temperature?.metric;

            forecast.fcttext_metric = new StringBuilder().Apply(sb =>
            {
                if (dayItem != null)
                {
                    sb.Append(dayItem.periodLabel)
                      .Append(" : ")
                      .Append(dayItem.text)
                      .AppendLine();
                }

                if (nightItem != null)
                {
                    sb.Append(nightItem.periodLabel)
                      .Append(" : ")
                      .Append(nightItem.text);
                }
            }).ToString();

            forecast.fcttext = new StringBuilder().Apply(sb =>
            {
                if (dayItem != null)
                {
                    var tempText = dayItem.temperatureText?.TakeIf(it =>
                    {
                        return highC != null && highF != null && !string.IsNullOrWhiteSpace(it);
                    })?.Replace(highC, $"{highF}°F");

                    sb.Append(dayItem.periodLabel)
                      .Append(" : ");
                    if (dayItem.text != null && tempText != null)
                    {
                        sb.Append(dayItem.text.Replace(dayItem.temperatureText, tempText));
                    }
                    else
                    {
                        sb.Append(dayItem.text);
                    }
                    sb.AppendLine();
                }

                if (nightItem != null)
                {
                    var tempText = nightItem.temperatureText?.TakeIf(it =>
                    {
                        return lowC != null && lowF != null && !string.IsNullOrWhiteSpace(it);
                    })?.Replace(lowC, $"{lowF}°F");

                    sb.Append(nightItem.periodLabel)
                      .Append(" : ");
                    if (nightItem.text != null && tempText != null)
                    {
                        sb.Append(nightItem.text.Replace(nightItem.temperatureText, tempText));
                    }
                    else
                    {
                        sb.Append(nightItem.text);
                    }
                    sb.AppendLine();
                }
            }).ToString();

            return forecast;
        }
    }
}