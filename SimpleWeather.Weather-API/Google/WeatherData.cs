#nullable enable
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Condition = SimpleWeather.WeatherData.Condition;
using Location = SimpleWeather.WeatherData.Location;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Weather_API.Google
{
    public static partial class GoogleWeatherProviderExtensions
    {
        public static Weather CreateWeatherData(this GoogleWeatherProvider _, CurrentResponse current,
            DailyResponse daily, HourlyResponse hourly, MinutelyResponse? minutely = null, AlertsResponse? alerts = null)
        {
            var weather = new Weather();

            var now = DateTimeOffset.Parse(current.currentTime, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind);
            (ForecastDaysItem, Forecast)? todaysForecast = null;
            TextForecast? todaysTxtForecast = null;

            weather.location = _.CreateLocation(current);
            weather.update_time = now;

            // Forecast
            weather.forecast = new List<Forecast>(daily.forecastDays?.Count ?? 0);
            weather.txt_forecast = new List<TextForecast>(daily.forecastDays?.Count ?? 0);

            daily.forecastDays?.ForEach(day =>
            {
                var dailyFcast = _.CreateForecast(day);
                var txtFcast = _.CreateTextForecast(day);

                if (todaysTxtForecast == null && now.Date == dailyFcast.date.Date)
                {
                    todaysForecast = (day, dailyFcast);
                    todaysTxtForecast = txtFcast;
                }

                weather.forecast.Add(dailyFcast);
                weather.txt_forecast.Add(txtFcast);
            });

            // Hourly Forecast
            weather.hr_forecast = hourly.forecastHours?.Select(it => _.CreateHourlyForecast(it))?.ToList();

            weather.min_forecast = minutely?.segments?.Select(it => _.CreateMinutelyForecast(it))?.ToList();

            weather.condition = _.CreateCondition(current, todaysForecast?.Item2, todaysTxtForecast);
            weather.atmosphere = _.CreateAtmosphere(current);
            weather.astronomy = _.CreateAstronomy(todaysForecast?.Item1);
            weather.precipitation = _.CreatePrecipitation(current);
            weather.weather_alerts = _.CreateWeatherAlerts(alerts);
            weather.ttl = 60; // TODO: TBD

            if ((!weather.condition.high_f.HasValue || !weather.condition.high_c.HasValue ||
                 weather.condition.high_f == weather.condition.low_f) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f;
                weather.condition.high_c = weather.forecast[0].high_c;
                weather.condition.low_f = weather.forecast[0].low_f;
                weather.condition.low_c = weather.forecast[0].low_c;
            }

            weather.source = WeatherAPI.Google;

            return weather;
        }

        public static Location CreateLocation(this GoogleWeatherProvider _, CurrentResponse current)
        {
            return new Location()
            {
                /* Use name from location provider */
                name = null,
                latitude = null,
                longitude = null,
                tz_long = current.timeZone?.id,
            };
        }

        public static Condition CreateCondition(this GoogleWeatherProvider _, CurrentResponse current,
            Forecast? todaysForecast, TextForecast? todaysTxtForecast)
        {
            var condition = new Condition();

            var wm = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Google);

            condition.icon =
                wm.GetWeatherIcon(current.isDaytime?.Let(it => !it) ?? false, current.weatherCondition?.type);
            condition.weather = current.weatherCondition?.description?.text;

            current.temperature?.degrees?.Let(it =>
            {
                condition.temp_c = it;
                condition.temp_f = ConversionMethods.CtoF(it);
            });

            condition.wind_degrees = current.wind?.direction?.degrees;
            current.wind?.speed?.value?.Let(it =>
            {
                condition.wind_kph = it;
                condition.wind_mph = ConversionMethods.KphToMph(it);
                condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(condition.wind_mph.Value.RoundToInt()));
            });

            current.wind?.gust?.value?.Let(it =>
            {
                condition.windgust_kph = it;
                condition.windgust_mph = ConversionMethods.KphToMph(it);
            });

            current.feelsLikeTemperature?.degrees?.Let(it =>
            {
                condition.feelslike_c = it;
                condition.feelslike_f = ConversionMethods.CtoF(it);
            });

            current.uvIndex?.Let(it => condition.uv = new UV(it));
            condition.observation_time = DateTimeOffset.Parse(current.currentTime, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind);

            if (current.currentConditionsHistory?.maxTemperature?.degrees is { } maxTempC)
            {
                condition.high_c = maxTempC;
                condition.high_f = ConversionMethods.CtoF(maxTempC);
            }
            else
            {
                condition.high_c = todaysForecast?.high_c;
                condition.high_f = todaysForecast?.high_f;
            }

            if (current.currentConditionsHistory?.minTemperature?.degrees is { } minTempC)
            {
                condition.low_c = minTempC;
                condition.low_f = ConversionMethods.CtoF(minTempC);
            }
            else
            {
                condition.low_c = todaysForecast?.low_c;
                condition.low_f = todaysForecast?.low_f;
            }

            // fcttext & fcttextMetric are the same
            condition.summary = todaysTxtForecast?.fcttext;

            return condition;
        }

        public static Forecast CreateForecast(this GoogleWeatherProvider _, ForecastDaysItem day)
        {
            var forecast = new Forecast();

            forecast.date = DateTimeOffset
                .Parse(day.interval?.startTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).UtcDateTime;

            day.maxTemperature?.degrees?.Let(it =>
            {
                forecast.high_c = it;
                forecast.high_f = ConversionMethods.CtoF(it);
            });

            day.minTemperature?.degrees?.Let(it =>
            {
                forecast.low_c = it;
                forecast.low_f = ConversionMethods.CtoF(it);
            });

            var wm = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Google);

            forecast.icon = day.daytimeForecast?.weatherCondition?.type?.Let(it => wm.GetWeatherIcon(false, it))
                            ?? day.nighttimeForecast?.weatherCondition?.type?.Let(it => wm.GetWeatherIcon(true, it));
            forecast.condition = day.daytimeForecast?.weatherCondition?.description?.text ??
                                 day.nighttimeForecast?.weatherCondition?.description?.text;

            // Extras
            forecast.extras = new ForecastExtras()
            {
                humidity = day.daytimeForecast?.relativeHumidity,
                uv_index = day.daytimeForecast?.uvIndex,
                pop = day.daytimeForecast?.precipitation?.probability?.percent,
                wind_degrees = day.daytimeForecast?.wind?.direction?.degrees,
                cloudiness = day.daytimeForecast?.cloudCover
            };

            day.daytimeForecast?.precipitation?.qpf?.quantity?.Let(it =>
            {
                forecast.extras.qpf_rain_mm = it;
                forecast.extras.qpf_rain_in = ConversionMethods.MMToIn(it);
            });

            day.daytimeForecast?.wind?.speed?.value?.Let(it =>
            {
                forecast.extras.wind_kph = it;
                forecast.extras.wind_mph = ConversionMethods.KphToMph(it);
            });

            day.daytimeForecast?.wind?.gust?.value?.Let(it =>
            {
                forecast.extras.windgust_kph = it;
                forecast.extras.windgust_mph = ConversionMethods.KphToMph(it);
            });

            day.feelsLikeMaxTemperature?.degrees?.Let(it =>
            {
                forecast.extras.feelslike_c = it;
                forecast.extras.feelslike_f = ConversionMethods.CtoF(it);
            });

            return forecast;
        }

        public static TextForecast CreateTextForecast(this GoogleWeatherProvider _, ForecastDaysItem day)
        {
            var txtForecast = new TextForecast();

            txtForecast.date = DateTimeOffset.Parse(day.interval?.startTime, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind);

            txtForecast.fcttext = new StringBuilder().Apply(sb =>
            {
                day.daytimeForecast?.Let(dayCast =>
                {
                    sb.Append($"{ResStrings.label_day}: ");
                    dayCast.weatherCondition?.description?.text?.Let(it => { sb.Append($"{it}. "); });
                    dayCast.precipitation?.probability?.percent?.Let(it =>
                    {
                        sb.Append($"{ResStrings.label_chance}: {it}% ");
                    });
                    sb.AppendLine();
                });

                day.nighttimeForecast?.Let(nightCast =>
                {
                    sb.Append($"{ResStrings.label_night}: ");
                    nightCast.weatherCondition?.description?.text?.Let(it => { sb.Append($"{it}. "); });
                    nightCast.precipitation?.probability?.percent?.Let(it =>
                    {
                        sb.Append($"{ResStrings.label_chance}: {it}% ");
                    });
                    sb.AppendLine();
                });
            }).ToString();

            txtForecast.fcttext_metric = txtForecast.fcttext;

            return txtForecast;
        }

        public static HourlyForecast CreateHourlyForecast(this GoogleWeatherProvider _, ForecastHoursItem hour)
        {
            var hrf = new HourlyForecast();

            hrf.date = DateTimeOffset.Parse(hour.interval?.startTime, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind);

            hour.temperature?.degrees?.Let(it =>
            {
                hrf.high_c = it;
                hrf.high_f = ConversionMethods.CtoF(it);
            });

            var wm = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Google);

            hrf.icon =
                wm.GetWeatherIcon(hour.isDaytime?.Let(it => !it) ?? false, hour.weatherCondition?.type);
            hrf.condition = hour.weatherCondition?.description?.text;

            hrf.wind_degrees = hour.wind?.direction?.degrees;
            hour.wind?.speed?.value?.Let(it =>
            {
                hrf.wind_kph = it;
                hrf.wind_mph = ConversionMethods.KphToMph(it);
            });

            // Extras
            hrf.extras = new ForecastExtras();
            hour.feelsLikeTemperature?.degrees?.Let(it =>
            {
                hrf.extras.feelslike_c = it;
                hrf.extras.feelslike_f = ConversionMethods.CtoF(it);
            });
            hour.dewPoint?.degrees?.Let(it =>
            {
                hrf.extras.dewpoint_c = it;
                hrf.extras.dewpoint_f = ConversionMethods.CtoF(it);
            });
            hrf.extras.humidity = hour.relativeHumidity;
            hrf.extras.uv_index = hour.uvIndex;
            hrf.extras.pop = hour.precipitation?.probability?.percent;
            hour.precipitation?.qpf?.quantity?.Let(it =>
            {
                hrf.extras.qpf_rain_mm = it;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(it);
            });
            hour.airPressure?.meanSeaLevelMillibars?.Let(it =>
            {
                hrf.extras.pressure_mb = it;
                hrf.extras.pressure_in = ConversionMethods.MBToInHg(it);
            });
            hrf.extras.wind_degrees = hrf.wind_degrees;
            hrf.extras.wind_kph = hrf.wind_kph;
            hrf.extras.wind_mph = hrf.wind_mph;
            hour.wind?.gust?.value?.Let(it =>
            {
                hrf.extras.windgust_kph = it;
                hrf.extras.windgust_mph = ConversionMethods.KphToMph(it);
            });
            hour.visibility?.distance?.Let(it =>
            {
                hrf.extras.visibility_km = it;
                hrf.extras.visibility_mi = ConversionMethods.KmToMi(it);
            });
            hrf.extras.cloudiness = hour.cloudCover;

            return hrf;
        }

        public static MinutelyForecast CreateMinutelyForecast(this GoogleWeatherProvider _, Segment minute)
        {
            return new MinutelyForecast
            {
                date = DateTimeOffset.Parse(minute.timeFrame?.startTime, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind),
                rain_mm = minute.qpf?.quantity,
                snow_mm = minute.snowfallAmount?.quantity
            };
        }

        public static Atmosphere CreateAtmosphere(this GoogleWeatherProvider _, CurrentResponse current)
        {
            var atmosphere = new Atmosphere();

            current.dewPoint?.degrees?.Let(it =>
            {
                atmosphere.dewpoint_c = it;
                atmosphere.dewpoint_f = ConversionMethods.CtoF(it);
            });

            atmosphere.humidity = current.relativeHumidity;

            current.airPressure?.meanSeaLevelMillibars?.Let(it =>
            {
                atmosphere.pressure_mb = it;
                atmosphere.pressure_in = ConversionMethods.MBToInHg(it);
            });

            current.visibility?.distance?.Let(it =>
            {
                atmosphere.visibility_km = it;
                atmosphere.visibility_mi = ConversionMethods.KmToMi(it);
            });

            return atmosphere;
        }

        public static Astronomy CreateAstronomy(this GoogleWeatherProvider _, ForecastDaysItem? day)
        {
            var astronomy = new Astronomy();

            try
            {
                astronomy.sunrise = DateTimeOffset.Parse(day?.sunEvents?.sunriseTime, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind).UtcDateTime;
            }
            catch
            {
            }

            try
            {
                astronomy.sunset = DateTimeOffset.Parse(day?.sunEvents?.sunsetTime, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind).UtcDateTime;
            }
            catch
            {
            }

            try
            {
                astronomy.moonrise = DateTimeOffset.Parse(day?.moonEvents?.moonriseTimes?.First(),
                    CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).UtcDateTime;
            }
            catch
            {
            }

            try
            {
                astronomy.moonset = DateTimeOffset.Parse(day?.moonEvents?.moonsetTimes?.First(),
                    CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).UtcDateTime;
            }
            catch
            {
            }

            astronomy.moonphase = day?.moonEvents?.moonPhase switch
            {
                "NEW_MOON" => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
                "WAXING_CRESCENT" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingCrescent),
                "FIRST_QUARTER" => new MoonPhase(MoonPhase.MoonPhaseType.FirstQtr),
                "WAXING_GIBBOUS" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingGibbous),
                "FULL_MOON" => new MoonPhase(MoonPhase.MoonPhaseType.FullMoon),
                "WANING_GIBBOUS" => new MoonPhase(MoonPhase.MoonPhaseType.WaningGibbous),
                "LAST_QUARTER" => new MoonPhase(MoonPhase.MoonPhaseType.LastQtr),
                "WANING_CRESCENT" => new MoonPhase(MoonPhase.MoonPhaseType.WaningCrescent),
                _ => null
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

        public static SimpleWeather.WeatherData.Precipitation CreatePrecipitation(this GoogleWeatherProvider _,
            CurrentResponse current)
        {
            var precip = new SimpleWeather.WeatherData.Precipitation();

            precip.pop = current.precipitation?.probability?.percent;

            current.precipitation?.qpf?.quantity?.Let(it =>
            {
                precip.qpf_rain_mm = it;
                precip.qpf_rain_in = ConversionMethods.MMToIn(it);
            });

            precip.cloudiness = current.cloudCover;

            return precip;
        }
    }
}
#nullable restore