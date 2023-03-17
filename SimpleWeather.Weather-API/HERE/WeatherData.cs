using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Condition = SimpleWeather.WeatherData.Condition;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Weather_API.HERE
{
    public static partial class HEREWeatherProviderExtensions
    {
        public static Weather CreateWeatherData(this HEREWeatherProvider _, HERE.Rootobject root)
        {
            var weather = new Weather();

            var now = root.feedCreation;
            SimpleWeather.WeatherData.Forecast todaysForecast = null;
            TextForecast todaysTxtForecast = null;

            weather.location = _.CreateLocation(root.observations.location[0]);
            weather.update_time = now;
            weather.forecast = new List<SimpleWeather.WeatherData.Forecast>(root.dailyForecasts.forecastLocation.forecast.Length);
            weather.txt_forecast = new List<TextForecast>(root.dailyForecasts.forecastLocation.forecast.Length);
            foreach (HERE.Forecast fcast in root.dailyForecasts.forecastLocation.forecast)
            {
                var dailyFcast = _.CreateForecast(fcast);
                var txtFcast = _.CreateTextForecast(fcast);

                weather.forecast.Add(dailyFcast);
                weather.txt_forecast.Add(txtFcast);

                if (todaysForecast == null && dailyFcast.date.Date == now.UtcDateTime.Date)
                {
                    todaysForecast = dailyFcast;
                    todaysTxtForecast = txtFcast;
                }
            }
            weather.hr_forecast = new List<HourlyForecast>(root.hourlyForecasts.forecastLocation.forecast.Length);
            foreach (HERE.Forecast1 forecast1 in root.hourlyForecasts.forecastLocation.forecast)
            {
                if (forecast1.utcTime.UtcDateTime < now.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                    continue;

                weather.hr_forecast.Add(_.CreateHourlyForecast(forecast1));
            }

            var observation = root.observations.location[0].observation[0];

            weather.condition = _.CreateCondition(observation, todaysForecast, todaysTxtForecast);
            weather.atmosphere = _.CreateAtmosphere(observation);
            weather.astronomy = _.CreateAstronomy(root.astronomy.astronomy);
            weather.precipitation = _.CreatePrecipitation(observation, todaysForecast);
            weather.ttl = 180;

            weather.source = WeatherAPI.Here;

            return weather;
        }

        public static SimpleWeather.WeatherData.Location CreateLocation(this HEREWeatherProvider _, HERE.Location location)
        {
            return new SimpleWeather.WeatherData.Location()
            {
                // Use location name from location provider
                name = null,
                latitude = location.latitude,
                longitude = location.longitude,
                tz_long = null,
            };
        }

        public static SimpleWeather.WeatherData.Forecast CreateForecast(this HEREWeatherProvider _, HERE.Forecast forecast)
        {
            var fcast = new SimpleWeather.WeatherData.Forecast();

            fcast.date = forecast.utcTime.UtcDateTime;
            if (float.TryParse(forecast.highTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float highF))
            {
                fcast.high_f = highF;
                fcast.high_c = ConversionMethods.FtoC(highF);
            }
            if (float.TryParse(forecast.lowTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float lowF))
            {
                fcast.low_f = lowF;
                fcast.low_c = ConversionMethods.FtoC(lowF);
            }
            fcast.condition = new StringBuilder(forecast.description.ToPascalCase()).Apply(sb =>
            {
                if (!String.IsNullOrWhiteSpace(forecast.airDescription) && !Equals(forecast.airDescription, "*"))
                {
                    if (!sb.ToString().EndsWith('.'))
                    {
                        sb.Append('.');
                    }
                    sb.Append($" {forecast.airDescription}");
                }
            }).ToString();
            fcast.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Here)
                   .GetWeatherIcon(Equals(forecast.daylight, "N") || forecast.iconName.StartsWith("night_"), forecast.iconName);

            // Extras
            fcast.extras = new ForecastExtras();
            if (float.TryParse(forecast.comfort, NumberStyles.Float, CultureInfo.InvariantCulture, out float comfortTempF))
            {
                fcast.extras.feelslike_f = comfortTempF;
                fcast.extras.feelslike_c = ConversionMethods.FtoC(comfortTempF);
            }
            if (int.TryParse(forecast.humidity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int humidity))
            {
                fcast.extras.humidity = humidity;
            }
            if (float.TryParse(forecast.dewPoint, NumberStyles.Float, CultureInfo.InvariantCulture, out float dewpointF))
            {
                fcast.extras.dewpoint_f = dewpointF;
                fcast.extras.dewpoint_c = ConversionMethods.FtoC(dewpointF);
            }
            if (int.TryParse(forecast.precipitationProbability, NumberStyles.Integer, CultureInfo.InvariantCulture, out int pop))
            {
                fcast.extras.pop = pop;
            }
            if (float.TryParse(forecast.rainFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float rain_in))
            {
                fcast.extras.qpf_rain_in = rain_in;
                fcast.extras.qpf_rain_mm = ConversionMethods.InToMM(rain_in);
            }
            if (float.TryParse(forecast.snowFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float snow_in))
            {
                fcast.extras.qpf_snow_in = snow_in;
                fcast.extras.qpf_snow_cm = ConversionMethods.InToMM(snow_in / 10);
            }
            if (float.TryParse(forecast.barometerPressure, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressureIN))
            {
                fcast.extras.pressure_in = pressureIN;
                fcast.extras.pressure_mb = ConversionMethods.InHgToMB(pressureIN);
            }
            if (int.TryParse(forecast.windDirection, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windDegrees))
            {
                fcast.extras.wind_degrees = windDegrees;
            }
            if (float.TryParse(forecast.windSpeed, NumberStyles.Float, CultureInfo.InvariantCulture, out float windSpeed))
            {
                fcast.extras.wind_mph = windSpeed;
                fcast.extras.wind_kph = ConversionMethods.MphToKph(windSpeed);
            }
            if (float.TryParse(forecast.uvIndex, NumberStyles.Float, CultureInfo.InvariantCulture, out float uv_index))
            {
                fcast.extras.uv_index = uv_index;
            }

            return fcast;
        }

        public static HourlyForecast CreateHourlyForecast(this HEREWeatherProvider _, HERE.Forecast1 hr_forecast)
        {
            var hrf = new HourlyForecast();

            hrf.date = hr_forecast.utcTime;
            if (float.TryParse(hr_forecast.temperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float highF))
            {
                hrf.high_f = highF;
                hrf.high_c = ConversionMethods.FtoC(highF);
            }
            hrf.condition = new StringBuilder(hr_forecast.description.ToPascalCase()).Apply(sb =>
            {
                if (!String.IsNullOrWhiteSpace(hr_forecast.airDescription) && !Equals(hr_forecast.airDescription, "*"))
                {
                    if (!sb.ToString().EndsWith('.'))
                    {
                        sb.Append('.');
                    }
                    sb.Append($" {hr_forecast.airDescription}");
                }
            }).ToString();

            hrf.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Here)
                   .GetWeatherIcon(Equals(hr_forecast.daylight, "N") || hr_forecast.iconName.StartsWith("night_"), hr_forecast.iconName);

            if (int.TryParse(hr_forecast.windDirection, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windDeg))
                hrf.wind_degrees = windDeg;
            if (float.TryParse(hr_forecast.windSpeed, NumberStyles.Float, CultureInfo.InvariantCulture, out float windSpeed))
            {
                hrf.wind_mph = windSpeed;
                hrf.wind_kph = ConversionMethods.MphToKph(windSpeed);
            }

            // Extras
            hrf.extras = new ForecastExtras();
            if (float.TryParse(hr_forecast.comfort, NumberStyles.Float, CultureInfo.InvariantCulture, out float comfortTemp_f))
            {
                hrf.extras.feelslike_f = comfortTemp_f;
                hrf.extras.feelslike_c = ConversionMethods.FtoC(comfortTemp_f);
            }
            if (int.TryParse(hr_forecast.humidity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int humidity))
            {
                hrf.extras.humidity = humidity;
            }
            if (float.TryParse(hr_forecast.dewPoint, NumberStyles.Float, CultureInfo.InvariantCulture, out float dewpointF))
            {
                hrf.extras.dewpoint_f = dewpointF;
                hrf.extras.dewpoint_c = ConversionMethods.FtoC(dewpointF);
            }
            if (float.TryParse(hr_forecast.visibility, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibilityMI))
            {
                hrf.extras.visibility_mi = visibilityMI;
                hrf.extras.visibility_km = ConversionMethods.MiToKm(visibilityMI);
            }
            if (int.TryParse(hr_forecast.precipitationProbability, NumberStyles.Integer, CultureInfo.InvariantCulture, out int PoP))
            {
                hrf.extras.pop = PoP;
            }
            if (float.TryParse(hr_forecast.rainFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float rain_in))
            {
                hrf.extras.qpf_rain_in = rain_in;
                hrf.extras.qpf_rain_mm = ConversionMethods.InToMM(rain_in);
            }
            if (float.TryParse(hr_forecast.snowFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float snow_in))
            {
                hrf.extras.qpf_snow_in = snow_in;
                hrf.extras.qpf_snow_cm = ConversionMethods.InToMM(snow_in / 10);
            }
            if (float.TryParse(hr_forecast.barometerPressure, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressureIN))
            {
                hrf.extras.pressure_in = pressureIN;
                hrf.extras.pressure_mb = ConversionMethods.InHgToMB(pressureIN);
            }
            hrf.extras.wind_degrees = hrf.wind_degrees;
            hrf.extras.wind_mph = hrf.wind_mph;
            hrf.extras.wind_kph = hrf.wind_kph;

            return hrf;
        }

        public static TextForecast CreateTextForecast(this HEREWeatherProvider _, HERE.Forecast forecast)
        {
            var textForecast = new TextForecast();

            textForecast.date = forecast.utcTime;
            textForecast.fcttext = new StringBuilder(
                String.Format(
                    CultureInfo.InvariantCulture, "{0} - {1}",
                    forecast.weekday,
                    forecast.description.ToPascalCase()
                )
            ).Apply(sb =>
            {
                if (!String.IsNullOrWhiteSpace(forecast.beaufortDescription) && !Equals(forecast.beaufortDescription, "*"))
                {
                    if (!sb.ToString().EndsWith('.'))
                    {
                        sb.Append('.');
                    }
                    sb.Append($" {forecast.beaufortDescription}");
                }

                if (!String.IsNullOrWhiteSpace(forecast.airDescription) && !Equals(forecast.airDescription, "*"))
                {
                    if (!sb.ToString().EndsWith('.'))
                    {
                        sb.Append('.');
                    }
                    sb.Append($" {forecast.airDescription}");
                }
            }).ToString();
            textForecast.fcttext_metric = textForecast.fcttext;

            return textForecast;
        }

        public static Condition CreateCondition(this HEREWeatherProvider _, HERE.Observation observation, SimpleWeather.WeatherData.Forecast todaysForecast, TextForecast todaysTxtForecast)
        {
            var condition = new Condition();

            condition.weather = observation.description.ToPascalCase();
            if (float.TryParse(observation.temperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float tempF))
            {
                condition.temp_f = tempF;
                condition.temp_c = ConversionMethods.FtoC(tempF);
            }

            if (float.TryParse(observation.highTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float hiTempF) &&
                float.TryParse(observation.lowTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float loTempF))
            {
                condition.high_f = hiTempF;
                condition.high_c = ConversionMethods.FtoC(hiTempF);
                condition.low_f = loTempF;
                condition.low_c = ConversionMethods.FtoC(loTempF);
            }
            else
            {
                condition.high_f = todaysForecast?.high_f;
                condition.high_c = todaysForecast?.high_c;
                condition.low_f = todaysForecast?.low_f;
                condition.low_c = todaysForecast?.low_c;
            }

            if (int.TryParse(observation.windDirection, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windDegrees))
                condition.wind_degrees = windDegrees;
            else
                condition.wind_degrees = 0;

            if (float.TryParse(observation.windSpeed, NumberStyles.Float, CultureInfo.InvariantCulture, out float wind_Speed))
            {
                condition.wind_mph = wind_Speed;
                condition.wind_kph = ConversionMethods.MphToKph(wind_Speed);
                condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale((int)Math.Round(wind_Speed)));
            }

            if (float.TryParse(observation.comfort, NumberStyles.Float, CultureInfo.InvariantCulture, out float comfortTempF))
            {
                condition.feelslike_f = comfortTempF;
                condition.feelslike_c = ConversionMethods.FtoC(comfortTempF);
            }

            condition.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Here)
                   .GetWeatherIcon(Equals(observation.daylight, "N") || observation.iconName.StartsWith("night_"), observation.iconName);

            if (todaysForecast?.extras?.uv_index.HasValue == true)
                condition.uv = new UV(todaysForecast.extras.uv_index.Value);

            condition.observation_time = observation.utcTime;

            if (todaysForecast != null && todaysTxtForecast != null)
            {
                var culture = LocaleUtils.GetLocale();

                // fcttext & fcttextMetric are the same
                var summaryStr = new StringBuilder(todaysTxtForecast.fcttext).Apply(sb =>
                {
                    if (todaysForecast?.extras?.pop.HasValue == true)
                    {
                        if (!sb.ToString().EndsWith('.'))
                        {
                            sb.Append('.');
                        }
                        sb.AppendFormat(" {0}: {1}%", ResStrings.label_chance, todaysForecast.extras.pop.Value);
                    }
                });

                condition.summary = summaryStr.ToString();
            }

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this HEREWeatherProvider _, HERE.Observation observation)
        {
            var atmosphere = new Atmosphere();

            if (int.TryParse(observation.humidity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Humidity))
            {
                atmosphere.humidity = Humidity;
            }

            if (float.TryParse(observation.barometerPressure, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressureIN))
            {
                atmosphere.pressure_in = pressureIN;
                atmosphere.pressure_mb = ConversionMethods.InHgToMB(pressureIN);
            }
            atmosphere.pressure_trend = observation.barometerTrend;

            if (float.TryParse(observation.visibility, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibilityMI))
            {
                atmosphere.visibility_mi = visibilityMI;
                atmosphere.visibility_km = ConversionMethods.MiToKm(visibilityMI);
            }

            if (float.TryParse(observation.dewPoint, NumberStyles.Float, CultureInfo.InvariantCulture, out float dewpointF))
            {
                atmosphere.dewpoint_f = dewpointF;
                atmosphere.dewpoint_c = ConversionMethods.FtoC(dewpointF);
            }

            return atmosphere;
        }

        public static SimpleWeather.WeatherData.Astronomy CreateAstronomy(this HEREWeatherProvider _, HERE.Astronomy1[] astronomy)
        {
            var astro = new SimpleWeather.WeatherData.Astronomy();

            var astroData = astronomy[0];

            if (DateTime.TryParse(astroData.sunrise, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sunrise))
                astro.sunrise = sunrise;
            if (DateTime.TryParse(astroData.sunset, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sunset))
            {
                if (sunrise != null && sunrise != DateTime.MinValue && sunset.CompareTo(sunrise) < 0)
                {
                    // Is next day
                    astro.sunset = sunset.AddDays(1);
                }
                else
                {
                    astro.sunset = sunset;
                }
            }
            if (DateTime.TryParse(astroData.moonrise, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime moonrise))
                astro.moonrise = moonrise;
            if (DateTime.TryParse(astroData.moonset, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime moonset))
                astro.moonset = moonset;

            // If the sun won't set/rise, set time to the future
            if (astro.sunrise == null)
            {
                astro.sunrise = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }
            if (astro.sunset == null)
            {
                astro.sunset = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }
            if (astro.moonrise == null)
            {
                astro.moonrise = DateTime.MinValue;
            }
            if (astro.moonset == null)
            {
                astro.moonset = DateTime.MinValue;
            }

            astro.moonphase = astroData.iconName switch
            {
                "cw_new_moon" => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
                "cw_waxing_crescent" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingCrescent),
                "cw_first_qtr" => new MoonPhase(MoonPhase.MoonPhaseType.FirstQtr),
                "cw_waxing_gibbous" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingGibbous),
                "cw_full_moon" => new MoonPhase(MoonPhase.MoonPhaseType.FullMoon),
                "cw_waning_gibbous" => new MoonPhase(MoonPhase.MoonPhaseType.WaningGibbous),
                "cw_last_quarter" => new MoonPhase(MoonPhase.MoonPhaseType.LastQtr),
                "cw_waning_crescent" => new MoonPhase(MoonPhase.MoonPhaseType.WaningCrescent),
                _ => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
            };
            return astro;
        }

        public static Precipitation CreatePrecipitation(this HEREWeatherProvider _, HERE.Observation observation, SimpleWeather.WeatherData.Forecast todaysForecast)
        {
            var precip = new Precipitation();

            precip.pop = todaysForecast?.extras?.pop;

            if (float.TryParse(observation.precipitation1H, NumberStyles.Float, CultureInfo.InvariantCulture, out float precipitation1H))
            {
                precip.qpf_rain_in = precipitation1H;
                precip.qpf_rain_mm = ConversionMethods.InToMM(precipitation1H);
            }
            else if (float.TryParse(observation.precipitation3H, NumberStyles.Float, CultureInfo.InvariantCulture, out float precipitation3H))
            {
                precip.qpf_rain_in = precipitation3H;
                precip.qpf_rain_mm = ConversionMethods.InToMM(precipitation3H);
            }
            else if (float.TryParse(observation.precipitation6H, NumberStyles.Float, CultureInfo.InvariantCulture, out float precipitation6H))
            {
                precip.qpf_rain_in = precipitation6H;
                precip.qpf_rain_mm = ConversionMethods.InToMM(precipitation6H);
            }
            else if (float.TryParse(observation.precipitation12H, NumberStyles.Float, CultureInfo.InvariantCulture, out float precipitation12H))
            {
                precip.qpf_rain_in = precipitation12H;
                precip.qpf_rain_mm = ConversionMethods.InToMM(precipitation12H);
            }
            else if (float.TryParse(observation.precipitation24H, NumberStyles.Float, CultureInfo.InvariantCulture, out float precipitation24H))
            {
                precip.qpf_rain_in = precipitation24H;
                precip.qpf_rain_mm = ConversionMethods.InToMM(precipitation24H);
            }
            else if (todaysForecast?.extras != null)
            {
                precip.qpf_rain_in = todaysForecast?.extras?.qpf_rain_in;
                precip.qpf_rain_mm = todaysForecast?.extras?.qpf_rain_mm;
            }

            if (float.TryParse(observation.snowCover, NumberStyles.Float, CultureInfo.InvariantCulture, out float snowCover))
            {
                precip.qpf_snow_in = snowCover;
                precip.qpf_snow_cm = ConversionMethods.InToMM(snowCover) / 10;
            }
            else if (todaysForecast?.extras != null)
            {
                precip.qpf_snow_in = todaysForecast?.extras?.qpf_snow_in;
                precip.qpf_snow_cm = todaysForecast?.extras?.qpf_snow_cm;
            }

            return precip;
        }
    }
}