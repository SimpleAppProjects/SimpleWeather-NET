using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Weather_API.OpenWeather.OneCall
{
    public static partial class OWMOneCallWeatherProviderExtensions
    {
        public static SimpleWeather.WeatherData.Weather CreateWeatherData(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Rootobject root)
        {
            var weather = new SimpleWeather.WeatherData.Weather();

            weather.location = _.CreateLocation(root);
            weather.update_time = DateTimeOffset.FromUnixTimeSeconds(root.current.dt);

            weather.forecast = new List<Forecast>(root.daily.Length);
            weather.txt_forecast = new List<TextForecast>(root.daily.Length);
            foreach (var daily in root.daily)
            {
                weather.forecast.Add(_.CreateForecast(daily));
                weather.txt_forecast.Add(_.CreateTextForecast(daily));
            }
            weather.hr_forecast = new List<HourlyForecast>(root.hourly.Length);
            foreach (var hourly in root.hourly)
            {
                weather.hr_forecast.Add(_.CreateHourlyForecast(hourly));
            }
            if (root.minutely?.Any() == true)
            {
                weather.min_forecast = new List<MinutelyForecast>(root.minutely.Length);
                foreach (var min in root.minutely)
                {
                    weather.min_forecast.Add(_.CreateMinutelyForecast(min));
                }
            }

            weather.condition = _.CreateCondition(root.current);
            weather.atmosphere = _.CreateAtmosphere(root.current);
            weather.precipitation = _.CreatePrecipitation(root.current);
            weather.ttl = 180;

            weather.query = string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);

            if ((!weather.condition.high_f.HasValue || !weather.condition.low_f.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f.Value;
                weather.condition.high_c = weather.forecast[0].high_c.Value;
                weather.condition.low_f = weather.forecast[0].low_f.Value;
                weather.condition.low_c = weather.forecast[0].low_c.Value;
            }

            weather.condition.observation_time = weather.update_time;

            var firstDate = DateTimeOffset.FromUnixTimeSeconds(root.daily[0].dt).DateTime;
            if (firstDate.Date == weather.condition.observation_time.Date)
            {
                weather.astronomy = _.CreateAstronomy(root.daily[0]);
            }
            else
            {
                weather.astronomy = _.CreateAstronomy(root.current);
            }

            weather.weather_alerts = _.CreateWeatherAlerts(root.alerts);

            weather.source = WeatherAPI.OpenWeatherMap;

            return weather;
        }

        /* OpenWeather OneCall */
        public static SimpleWeather.WeatherData.Location CreateLocation(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Rootobject root)
        {
            return new SimpleWeather.WeatherData.Location()
            {
                // Use location name from location provider
                name = null,
                latitude = root.lat,
                longitude = root.lon,
                tz_long = root.timezone,
            };
        }

        /* OpenWeather OneCall */
        public static SimpleWeather.WeatherData.Forecast CreateForecast(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Daily forecast)
        {
            var fcast = new SimpleWeather.WeatherData.Forecast();

            fcast.date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;
            fcast.high_f = ConversionMethods.KtoF(forecast.temp.max);
            fcast.high_c = ConversionMethods.KtoC(forecast.temp.max);
            fcast.low_f = ConversionMethods.KtoF(forecast.temp.min);
            fcast.low_c = ConversionMethods.KtoC(forecast.temp.min);
            fcast.condition = forecast.weather[0].description.ToUpperCase();
            fcast.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(forecast.weather[0].id.ToInvariantString());

            // Extras
            fcast.extras = new ForecastExtras()
            {
                dewpoint_f = ConversionMethods.KtoF(forecast.dew_point),
                dewpoint_c = ConversionMethods.KtoC(forecast.dew_point),
                humidity = forecast.humidity,
                cloudiness = forecast.clouds,
                // 1hPA = 1mbar
                pressure_mb = forecast.pressure,
                pressure_in = ConversionMethods.MBToInHg(forecast.pressure),
                wind_degrees = forecast.wind_deg,
                wind_mph = ConversionMethods.MSecToMph(forecast.wind_speed),
                wind_kph = ConversionMethods.MSecToKph(forecast.wind_speed),
                uv_index = forecast.uvi
            };
            if (forecast.pop.HasValue)
            {
                fcast.extras.pop = (int)Math.Round(forecast.pop.Value * 100);
            }
            if (forecast.visibility.HasValue)
            {
                fcast.extras.visibility_km = forecast.visibility.Value / 1000;
                fcast.extras.visibility_mi = ConversionMethods.KmToMi(fcast.extras.visibility_km.Value);
            }
            if (forecast.wind_gust.HasValue)
            {
                fcast.extras.windgust_mph = ConversionMethods.MSecToMph(forecast.wind_gust.Value);
                fcast.extras.windgust_kph = ConversionMethods.MSecToKph(forecast.wind_gust.Value);
            }
            if (forecast.rain.HasValue)
            {
                fcast.extras.qpf_rain_mm = forecast.rain.Value;
                fcast.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain.Value);
            }
            if (forecast.snow.HasValue)
            {
                fcast.extras.qpf_snow_cm = forecast.snow.Value / 10;
                fcast.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow.Value);
            }

            return fcast;
        }

        /* OpenWeather OneCall */
        public static SimpleWeather.WeatherData.HourlyForecast CreateHourlyForecast(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Hourly hr_forecast)
        {
            var hrf = new SimpleWeather.WeatherData.HourlyForecast();

            hrf.date = DateTimeOffset.FromUnixTimeSeconds(hr_forecast.dt);
            hrf.high_f = ConversionMethods.KtoF(hr_forecast.temp);
            hrf.high_c = ConversionMethods.KtoC(hr_forecast.temp);
            hrf.condition = hr_forecast.weather[0].description.ToUpperCase();

            // Use icon to determine if day or night
            string ico = hr_forecast.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
                dn = String.Empty;

            hrf.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(hr_forecast.weather[0].id.ToInvariantString() + dn);

            hrf.wind_degrees = hr_forecast.wind_deg;
            hrf.wind_mph = ConversionMethods.MSecToMph(hr_forecast.wind_speed);
            hrf.wind_kph = ConversionMethods.MSecToKph(hr_forecast.wind_speed);

            // Extras
            hrf.extras = new ForecastExtras()
            {
                feelslike_f = ConversionMethods.KtoF(hr_forecast.feels_like),
                feelslike_c = ConversionMethods.KtoC(hr_forecast.feels_like),
                dewpoint_f = ConversionMethods.KtoF(hr_forecast.dew_point),
                dewpoint_c = ConversionMethods.KtoC(hr_forecast.dew_point),
                humidity = hr_forecast.humidity,
                cloudiness = hr_forecast.clouds,
                // 1hPA = 1mbar
                pressure_mb = hr_forecast.pressure,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.pressure),
                wind_degrees = hrf.wind_degrees,
                wind_mph = hrf.wind_mph,
                wind_kph = hrf.wind_kph
            };
            if (hr_forecast.pop.HasValue)
            {
                hrf.extras.pop = (int)Math.Round(hr_forecast.pop.Value * 100);
            }
            if (hr_forecast.wind_gust.HasValue)
            {
                hrf.extras.windgust_mph = ConversionMethods.MSecToMph(hr_forecast.wind_gust.Value);
                hrf.extras.windgust_kph = ConversionMethods.MSecToKph(hr_forecast.wind_gust.Value);
            }
            if (hr_forecast.visibility.HasValue)
            {
                hrf.extras.visibility_km = hr_forecast.visibility.Value / 1000;
                hrf.extras.visibility_mi = ConversionMethods.KmToMi(hrf.extras.visibility_km.Value);
            }
            if (hr_forecast.rain != null)
            {
                hrf.extras.qpf_rain_mm = hr_forecast.rain._1h;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(hr_forecast.rain._1h);
            }
            if (hr_forecast.snow != null)
            {
                hrf.extras.qpf_snow_cm = hr_forecast.snow._1h / 10;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(hr_forecast.snow._1h);
            }
            if (hr_forecast.uvi.HasValue)
            {
                hrf.extras.uv_index = hr_forecast.uvi;
            }

            return hrf;
        }

        public static SimpleWeather.WeatherData.TextForecast CreateTextForecast(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Daily forecast)
        {
            var txtForecast = new SimpleWeather.WeatherData.TextForecast();

            txtForecast.date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;

            if (!string.IsNullOrWhiteSpace(forecast.summary))
            {
                txtForecast.fcttext = txtForecast.fcttext_metric = forecast.summary;
            }
            else
            {
                var sb = new StringBuilder()
                .AppendFormat(CultureInfo.InvariantCulture,
                    "{0} - {1}: {2}°; {3}: {4}°", ResStrings.label_morning,
                    ResStrings.label_temp,
                    Math.Round(ConversionMethods.KtoF(forecast.temp.morn)),
                    ResStrings.label_feelslike,
                    Math.Round(ConversionMethods.KtoF(forecast.feels_like.morn)))
                .AppendLine()
                .AppendFormat(CultureInfo.InvariantCulture,
                    "{0} - {1}: {2}°; {3}: {4}°", ResStrings.label_afternoon,
                    ResStrings.label_temp,
                    Math.Round(ConversionMethods.KtoF(forecast.temp.day)),
                    ResStrings.label_feelslike,
                    Math.Round(ConversionMethods.KtoF(forecast.temp.day)))
                .AppendFormat(CultureInfo.InvariantCulture,
                    "{0} - {1}: {2}°; {3}: {4}°", ResStrings.label_eve,
                    ResStrings.label_temp,
                    Math.Round(ConversionMethods.KtoF(forecast.temp.eve)),
                    ResStrings.label_feelslike,
                    Math.Round(ConversionMethods.KtoF(forecast.feels_like.eve)))
                .AppendLine()
                .AppendFormat(CultureInfo.InvariantCulture,
                    "{0} - {1}: {2}°; {3}: {4}°", ResStrings.label_night,
                    ResStrings.label_temp,
                    Math.Round(ConversionMethods.KtoF(forecast.temp.night)),
                    ResStrings.label_feelslike,
                    Math.Round(ConversionMethods.KtoF(forecast.feels_like.night)));

                txtForecast.fcttext = sb.ToString();

                var sb_metric = new StringBuilder()
                    .AppendFormat(CultureInfo.InvariantCulture,
                        "{0} - {1}: {2}°; {3}: {4}°", ResStrings.label_morning,
                        ResStrings.label_temp,
                        Math.Round(ConversionMethods.KtoC(forecast.temp.morn)),
                        ResStrings.label_feelslike,
                        Math.Round(ConversionMethods.KtoC(forecast.feels_like.morn)))
                    .AppendLine()
                    .AppendFormat(CultureInfo.InvariantCulture,
                        "{0} - {1}: {2}°; {3}: {4}°", ResStrings.label_afternoon,
                        ResStrings.label_temp,
                        Math.Round(ConversionMethods.KtoC(forecast.temp.day)),
                        ResStrings.label_feelslike,
                        Math.Round(ConversionMethods.KtoC(forecast.feels_like.day)))
                    .AppendLine()
                    .AppendFormat(CultureInfo.InvariantCulture,
                        "{0} - {1}: {2}°; {3}: {4}°", ResStrings.label_eve,
                        ResStrings.label_temp,
                        Math.Round(ConversionMethods.KtoC(forecast.temp.eve)),
                        ResStrings.label_feelslike,
                        Math.Round(ConversionMethods.KtoC(forecast.feels_like.eve)))
                    .AppendLine()
                    .AppendFormat(CultureInfo.InvariantCulture,
                        "{0} - {1}: {2}°; {3}: {4}°", ResStrings.label_night,
                        ResStrings.label_temp,
                        Math.Round(ConversionMethods.KtoC(forecast.temp.night)),
                        ResStrings.label_feelslike,
                        Math.Round(ConversionMethods.KtoC(forecast.feels_like.night)));

                txtForecast.fcttext_metric = sb_metric.ToString();
            }

            return txtForecast;
        }

        public static SimpleWeather.WeatherData.MinutelyForecast CreateMinutelyForecast(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Minutely minute)
        {
            return new SimpleWeather.WeatherData.MinutelyForecast()
            {
                date = DateTimeOffset.FromUnixTimeSeconds(minute.dt),
                rain_mm = minute.precipitation,
            };
        }

        public static SimpleWeather.WeatherData.Condition CreateCondition(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Current current)
        {
            var condition = new SimpleWeather.WeatherData.Condition();

            condition.weather = current.weather[0].description.ToUpperCase();
            condition.temp_f = ConversionMethods.KtoF(current.temp);
            condition.temp_c = ConversionMethods.KtoC(current.temp);
            condition.wind_degrees = current.wind_deg;
            condition.wind_mph = ConversionMethods.MSecToMph(current.wind_speed);
            condition.wind_kph = ConversionMethods.MSecToKph(current.wind_speed);
            condition.feelslike_f = ConversionMethods.KtoF(current.feels_like);
            condition.feelslike_c = ConversionMethods.KtoC(current.feels_like);
            if (current.wind_gust.HasValue)
            {
                condition.windgust_mph = ConversionMethods.MSecToMph(current.wind_gust.Value);
                condition.windgust_kph = ConversionMethods.MSecToKph(current.wind_gust.Value);
            }

            string ico = current.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
                dn = String.Empty;

            condition.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(current.weather[0].id.ToInvariantString() + dn);

            condition.uv = new UV(current.uvi);
            condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(current.wind_speed));

            condition.observation_time = DateTimeOffset.FromUnixTimeSeconds(current.dt);

            return condition;
        }

        public static SimpleWeather.WeatherData.Atmosphere CreateAtmosphere(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Current current)
        {
            return new SimpleWeather.WeatherData.Atmosphere()
            {
                humidity = current.humidity,
                // 1hPa = 1mbar
                pressure_mb = current.pressure,
                pressure_in = ConversionMethods.MBToInHg(current.pressure),
                pressure_trend = String.Empty,
                visibility_km = current.visibility / 1000,
                visibility_mi = ConversionMethods.KmToMi(current.visibility / 1000 /*visibility_km.Value*/),
                dewpoint_f = ConversionMethods.KtoF(current.dew_point),
                dewpoint_c = ConversionMethods.KtoC(current.dew_point),
            };
        }

        public static SimpleWeather.WeatherData.Astronomy CreateAstronomy(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Current current)
        {
            var astronomy = new SimpleWeather.WeatherData.Astronomy();

            try
            {
                astronomy.sunrise = DateTimeOffset.FromUnixTimeSeconds(current.sunrise).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                astronomy.sunset = DateTimeOffset.FromUnixTimeSeconds(current.sunset).UtcDateTime;
            }
            catch (Exception) { }

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

        public static SimpleWeather.WeatherData.Astronomy CreateAstronomy(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Daily day)
        {
            var astronomy = new SimpleWeather.WeatherData.Astronomy();

            try
            {
                astronomy.sunrise = DateTimeOffset.FromUnixTimeSeconds(day.sunrise).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                astronomy.sunset = DateTimeOffset.FromUnixTimeSeconds(day.sunset).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                astronomy.moonrise = DateTimeOffset.FromUnixTimeSeconds(day.moonrise.Value).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                astronomy.moonset = DateTimeOffset.FromUnixTimeSeconds(day.moonset.Value).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                MoonPhase.MoonPhaseType moonPhaseType;

                if (day.moon_phase == 0f || day.moon_phase == 1f)
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.NewMoon;
                }
                else if (day.moon_phase == 0.25f)
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.FirstQtr;
                }
                else if (day.moon_phase == 0.5f)
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.FullMoon;
                }
                else if (day.moon_phase == 0.75f)
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.LastQtr;
                }
                else if (day.moon_phase > 0f && day.moon_phase < 0.25f)
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.WaxingCrescent;
                }
                else if (day.moon_phase > 0.25f && day.moon_phase < 0.5f)
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.WaxingGibbous;
                }
                else if (day.moon_phase > 0.5f && day.moon_phase < 0.75f)
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.WaningGibbous;
                }
                else if (day.moon_phase > 0.75f && day.moon_phase < 1f)
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.WaningCrescent;
                }
                else
                {
                    moonPhaseType = MoonPhase.MoonPhaseType.NewMoon;
                }

                astronomy.moonphase = new MoonPhase(moonPhaseType);
            }
            catch (Exception) { }

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

        public static SimpleWeather.WeatherData.Precipitation CreatePrecipitation(this OWMOneCallWeatherProvider _, OpenWeather.OneCall.Current current)
        {
            var precip = new SimpleWeather.WeatherData.Precipitation();

            // Use cloudiness value here
            precip.cloudiness = current.clouds;
            if (current.rain != null)
            {
                precip.qpf_rain_in = ConversionMethods.MMToIn(current.rain._1h);
                precip.qpf_rain_mm = current.rain._1h;
            }
            if (current.snow != null)
            {
                precip.qpf_snow_in = ConversionMethods.MMToIn(current.snow._1h);
                precip.qpf_snow_cm = current.snow._1h / 10;
            }

            return precip;
        }

        public static SimpleWeather.WeatherData.AirQualityData CreateAirQuality(this OWMOneCallWeatherProvider _, AirPollutionRootobject root)
        {
            if (root?.list?.Any() == true)
            {
                var aqiForecasts = new List<AirQuality>(root.list.Length);

                foreach (var data in root.list)
                {
                    AirQuality aqiData = new();

                    // Convert
                    if (data.components.co.HasValue)
                    {
                        try
                        {
                            aqiData.co = AirQualityUtils.AQICO(AirQualityUtils.CO_ugm3_TO_ppm(data.components.co.Value));
                        }
                        catch (ArgumentOutOfRangeException) { }
                    }

                    if (data.components.no2.HasValue)
                    {
                        try
                        {
                            aqiData.no2 = AirQualityUtils.AQINO2(AirQualityUtils.NO2_ugm3_to_ppb(data.components.no2.Value));
                        }
                        catch (ArgumentOutOfRangeException) { }
                    }

                    if (data.components.o3.HasValue)
                    {
                        try
                        {
                            aqiData.o3 = AirQualityUtils.AQIO3(AirQualityUtils.O3_ugm3_to_ppb(data.components.o3.Value));
                        }
                        catch (ArgumentOutOfRangeException) { }
                    }

                    if (data.components.so2.HasValue)
                    {
                        try
                        {
                            aqiData.so2 = AirQualityUtils.AQISO2(AirQualityUtils.SO2_ugm3_to_ppb(data.components.so2.Value));
                        }
                        catch (ArgumentOutOfRangeException) { }
                    }

                    if (data.components.pm2_5.HasValue)
                    {
                        try
                        {
                            aqiData.pm25 = AirQualityUtils.AQIPM2_5(data.components.pm2_5.Value);
                        }
                        catch (ArgumentOutOfRangeException) { }
                    }

                    if (data.components.pm10.HasValue)
                    {
                        try
                        {
                            aqiData.pm10 = AirQualityUtils.AQIPM10(data.components.pm10.Value);
                        }
                        catch (ArgumentOutOfRangeException) { }
                    }

                    aqiData.index = aqiData.GetIndexFromData();
                }

                return new SimpleWeather.WeatherData.AirQualityData()
                {
                    current = aqiForecasts.FirstOrDefault(),
                    aqiForecast = aqiForecasts
                };
            }

            return null;
        }
    }
}