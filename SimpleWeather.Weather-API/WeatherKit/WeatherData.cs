using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Weather_API.WeatherKit
{
    public static partial class WeatherKitProviderExtensions
    {
        public static SimpleWeather.WeatherData.Weather CreateWeatherData(this WeatherKitProvider _, Weather root)
        {
            var weather = new SimpleWeather.WeatherData.Weather();

            var now = root.currentWeather.metadata.readTime;
            Forecast todaysForecast = null;
            TextForecast todaysTxtForecast = null;

            weather.location = _.CreateLocation(root.currentWeather);
            weather.update_time = now;

            weather.forecast = new List<SimpleWeather.WeatherData.Forecast>(root.forecastDaily.days.Length);
            weather.txt_forecast = new List<SimpleWeather.WeatherData.TextForecast>(root.forecastDaily.days.Length);
            weather.hr_forecast = new List<SimpleWeather.WeatherData.HourlyForecast>(root.forecastHourly.hours.Length);
            weather.min_forecast = new List<SimpleWeather.WeatherData.MinutelyForecast>(root.forecastNextHour.minutes.Length);

            // Forecast
            foreach (var day in root.forecastDaily.days)
            {
                var dailyFcast = _.CreateForecast(day);
                var txtFcast = _.CreateTextForecast(day);

                weather.forecast.Add(dailyFcast);
                weather.txt_forecast.Add(txtFcast);

                if (todaysForecast == null && dailyFcast.date.Date == now.UtcDateTime.Date)
                {
                    todaysForecast = dailyFcast;
                    todaysTxtForecast = txtFcast;
                }
            }

            // Hourly Forecast
            foreach (var hour in root.forecastHourly.hours)
            {
                weather.hr_forecast.Add(_.CreateHourlyForecast(hour));
            }

            // Minutely Forecast
            foreach (var minute in root.forecastNextHour.minutes)
            {
                weather.min_forecast.Add(_.CreateMinutelyForecast(minute));
            }

            weather.condition = _.CreateCondition(root.currentWeather, todaysForecast, todaysTxtForecast);
            weather.atmosphere = _.CreateAtmosphere(root.currentWeather);

            if (root.forecastDaily.days.FirstOrDefault(it => it.forecastStart.UtcDateTime.Date == weather.condition.observation_time.UtcDateTime.Date) is DayWeatherConditions today)
            {
                weather.astronomy = _.CreateAstronomy(today);
            }
            weather.precipitation = _.CreatePrecipitation(root.currentWeather);
            weather.ttl = 180;

            if ((!weather.condition.high_f.HasValue || !weather.condition.high_c.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f;
                weather.condition.high_c = weather.forecast[0].high_c;
                weather.condition.low_f = weather.forecast[0].low_f;
                weather.condition.low_c = weather.forecast[0].low_c;
            }

            weather.weather_alerts = _.CreateWeatherAlerts(root.weatherAlerts);

            weather.source = WeatherAPI.Apple;

            return weather;
        }

        public static SimpleWeather.WeatherData.Location CreateLocation(this WeatherKitProvider _, CurrentWeather current)
        {
            return new SimpleWeather.WeatherData.Location()
            {
                /* Use name from location provider */
                name = null,
                latitude = current.metadata.latitude,
                longitude = current.metadata.longitude,
                tz_long = null,
            };
        }

        public static SimpleWeather.WeatherData.Forecast CreateForecast(this WeatherKitProvider _, DayWeatherConditions day)
        {
            var forecast = new SimpleWeather.WeatherData.Forecast();

            forecast.date = day.forecastStart.UtcDateTime;

            forecast.high_c = day.temperatureMax;
            forecast.low_c = day.temperatureMin;
            forecast.high_f = ConversionMethods.CtoF(day.temperatureMax);
            forecast.low_f = ConversionMethods.CtoF(day.temperatureMin);

            forecast.condition = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple)
                .GetWeatherCondition(day.conditionCode);
            forecast.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple)
                .GetWeatherIcon(day.conditionCode);

            // Extras
            forecast.extras = new ForecastExtras
            {
                humidity = day.daytimeForecast?.humidity.Let(it => (int)MathF.Round(it * 100)),
                uv_index = day.maxUvIndex,
                pop = (int)MathF.Round(day.precipitationChance * 100),
                qpf_rain_mm = day.precipitationAmount,
                qpf_rain_in = ConversionMethods.MMToIn(day.precipitationAmount),
                qpf_snow_cm = day.snowfallAmount * 10,
                qpf_snow_in = ConversionMethods.MMToIn(day.snowfallAmount),
                wind_kph = day.daytimeForecast?.windSpeed,
                wind_mph = day.daytimeForecast?.windSpeed.Let(ConversionMethods.KphToMph),
                wind_degrees = day.daytimeForecast?.windDirection,
                cloudiness = day.daytimeForecast?.cloudCover.Let(it => (int)MathF.Round(it * 100))
            };

            return forecast;
        }

        public static SimpleWeather.WeatherData.HourlyForecast CreateHourlyForecast(this WeatherKitProvider _, HourWeatherConditions hour)
        {
            var hrf = new SimpleWeather.WeatherData.HourlyForecast();
            hrf.date = hour.forecastStart;

            hrf.high_c = hour.temperature;
            hrf.high_f = ConversionMethods.CtoF(hour.temperature);

            hrf.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple)
                .GetWeatherIcon(!hour.daylight.GetValueOrDefault(true), hour.conditionCode);
            hrf.condition = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple)
                .GetWeatherCondition(hour.conditionCode);

            hrf.wind_kph = hour.windSpeed;
            hrf.wind_mph = ConversionMethods.KphToMph(hour.windSpeed);
            hrf.wind_degrees = hour.windDirection;

            // Extras
            hrf.extras = new ForecastExtras
            {
                feelslike_c = hour.temperatureApparent,
                feelslike_f = ConversionMethods.CtoF(hour.temperatureApparent),
                humidity = (int)MathF.Round(hour.humidity * 100),
                dewpoint_c = hour.temperatureDewPoint,
                dewpoint_f = hour.temperatureDewPoint?.Let(ConversionMethods.CtoF),
                uv_index = hour.uvIndex,
                pop = (int)MathF.Round(hour.precipitationChance * 100),
                cloudiness = (int)MathF.Round(hour.cloudCover),
                qpf_rain_mm = hour.precipitationType != PrecipitationType.Snow ? hour.precipitationAmount : null,
                qpf_rain_in = hour.precipitationType != PrecipitationType.Snow ? hour.precipitationAmount?.Let(ConversionMethods.MMToIn) : null,
                qpf_snow_cm = hour.precipitationType == PrecipitationType.Snow ? hour.precipitationAmount?.Let(it => it * 10) : null,
                qpf_snow_in = hour.precipitationType == PrecipitationType.Snow ? hour.precipitationAmount?.Let(ConversionMethods.MMToIn) : null,
                pressure_mb = hour.pressure,
                pressure_in = ConversionMethods.MBToInHg(hour.pressure),
                wind_degrees = hour.windDirection,
                wind_kph = hour.windSpeed,
                wind_mph = ConversionMethods.KphToMph(hour.windSpeed),
                visibility_km = hour.visibility / 1000,
                visibility_mi = ConversionMethods.KmToMi(hour.visibility / 1000),
                windgust_kph = hour.windGust,
                windgust_mph = hour.windGust?.Let(ConversionMethods.KphToMph),
            };

            return hrf;
        }

        public static SimpleWeather.WeatherData.MinutelyForecast CreateMinutelyForecast(this WeatherKitProvider _, ForecastMinute minute)
        {
            return new SimpleWeather.WeatherData.MinutelyForecast()
            {
                date = minute.startTime,
                rain_mm = minute.precipitationIntensity
            };
        }

        public static SimpleWeather.WeatherData.TextForecast CreateTextForecast(this WeatherKitProvider _, DayWeatherConditions day)
        {
            var textForecast = new TextForecast
            {
                date = day.forecastStart.UtcDateTime
            };

            textForecast.fcttext = new StringBuilder()
                .Apply(sb =>
                {
                    if (day.daytimeForecast != null)
                    {
                        var dayConditionText = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple)
                            .GetWeatherCondition(day.daytimeForecast.conditionCode);
                        sb.Append($"{ResStrings.label_day}: {dayConditionText};");
                        sb.AppendLine($" {ResStrings.label_chance}: {(int)MathF.Round(day.daytimeForecast.precipitationChance * 100)}%");
                    }

                    if (day.overnightForecast != null)
                    {
                        var ntConditionText = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple)
                                .GetWeatherCondition(day.overnightForecast.conditionCode);
                        sb.Append($"{ResStrings.label_night}: {ntConditionText};");
                        sb.AppendLine($" {ResStrings.label_chance}: {(int)MathF.Round(day.overnightForecast.precipitationChance * 100)}%");
                    }
                })
                .ToString();

            textForecast.fcttext_metric = textForecast.fcttext;

            return textForecast;
        }

        public static SimpleWeather.WeatherData.Condition CreateCondition(this WeatherKitProvider _, CurrentWeather current, Forecast todaysForecast, TextForecast todaysTxtForecast)
        {
            return new SimpleWeather.WeatherData.Condition()
            {
                weather = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple)
                    .GetWeatherCondition(current.conditionCode),

                temp_c = current.temperature,
                temp_f = ConversionMethods.CtoF(current.temperature),

                wind_degrees = current.windDirection,
                wind_kph = current.windSpeed,
                wind_mph = ConversionMethods.KphToMph(current.windSpeed),

                windgust_kph = current.windGust,
                windgust_mph = current.windGust?.Let(ConversionMethods.KphToMph),

                feelslike_c = current.temperatureApparent,
                feelslike_f = ConversionMethods.CtoF(current.temperatureApparent),

                icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple)
                    .GetWeatherIcon(!current.daylight.GetValueOrDefault(true), current.conditionCode),

                beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(ConversionMethods.KphToMSec(current.windSpeed))),
                uv = new UV(current.uvIndex),
                observation_time = current.asOf,

                // fcttext & fcttextMetric are the same
                summary = todaysTxtForecast.fcttext,

                high_c = todaysForecast.high_c,
                high_f = todaysForecast.high_f,
                low_c = todaysForecast.low_c,
                low_f = todaysForecast.low_f,
            };
        }

        public static Atmosphere CreateAtmosphere(this WeatherKitProvider _, CurrentWeather current)
        {
            return new Atmosphere()
            {
                humidity = (int)MathF.Round(current.humidity * 100),

                pressure_mb = current.pressure,
                pressure_in = ConversionMethods.MBToInHg(current.pressure),
                pressure_trend = current.pressureTrend switch
                {
                    PressureTrend.Rising => "+",
                    PressureTrend.Falling => "-",
                    _ => string.Empty,
                },

                visibility_km = current.visibility / 1000,
                visibility_mi = ConversionMethods.KmToMi(current.visibility / 1000),

                dewpoint_c = current.temperatureDewPoint,
                dewpoint_f = ConversionMethods.CtoF(current.temperatureDewPoint),
            };
        }

        public static Astronomy CreateAstronomy(this WeatherKitProvider _, DayWeatherConditions day)
        {
            var astronomy = new Astronomy();

            astronomy.sunrise = day.sunrise?.UtcDateTime ?? DateTime.Now.Date.AddYears(1).AddTicks(-1);
            astronomy.sunset = day.sunset?.UtcDateTime ?? DateTime.Now.Date.AddYears(1).AddTicks(-1);
            astronomy.moonrise = day.moonrise?.UtcDateTime ?? DateTime.MinValue;
            astronomy.moonset = day.moonset?.UtcDateTime ?? DateTime.MinValue;

            astronomy.moonphase = day.moonPhase switch
            {
                MoonPhase.New => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.NewMoon),
                MoonPhase.WaxingCrescent => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.WaxingCrescent),
                MoonPhase.FirstQuarter => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.FirstQtr),
                MoonPhase.WaxingGibbous => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.WaxingGibbous),
                MoonPhase.Full => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.FullMoon),
                MoonPhase.WaningGibbous => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.WaningGibbous),
                MoonPhase.ThirdQuarter => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.LastQtr),
                MoonPhase.WaningCrescent => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.WaningCrescent),
                _ => new SimpleWeather.WeatherData.MoonPhase(SimpleWeather.WeatherData.MoonPhase.MoonPhaseType.NewMoon),
            };

            return astronomy;
        }

        public static Precipitation CreatePrecipitation(this WeatherKitProvider _, CurrentWeather current)
        {
            return new Precipitation()
            {
                cloudiness = current.cloudCover?.Let(it => (int)MathF.Round(it * 100)),

                qpf_rain_mm = current.precipitationIntensity,
                qpf_rain_in = ConversionMethods.MMToIn(current.precipitationIntensity),
            };
        }
    }
}