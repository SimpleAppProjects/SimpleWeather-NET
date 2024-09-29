using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Weather_API.OpenWeather
{
    public static partial class OpenWeatherMapProviderExtensions
    {
        public static SimpleWeather.WeatherData.Weather CreateWeatherData(this OpenWeatherMapProvider _, CurrentRootobject currRoot, ForecastRootobject foreRoot)
        {
            var weather = new SimpleWeather.WeatherData.Weather();

            weather.location = _.CreateLocation(foreRoot);
            weather.update_time = DateTimeOffset.FromUnixTimeSeconds(currRoot.dt);

            // 5-day forecast / 3-hr forecast
            // 24hr / 3hr = 8items for each day
            weather.forecast = new List<Forecast>(5);
            weather.hr_forecast = new List<HourlyForecast>(foreRoot.list.Length);

            // Store potential min/max values
            float dayMax = float.NaN;
            float dayMin = float.NaN;
            int lastDay = -1;

            for (int i = 0; i < foreRoot.list.Length; i++)
            {
                weather.hr_forecast.Add(_.CreateHourlyForecast(foreRoot.list[i]));

                float max = foreRoot.list[i].main.temp_max;
                if (!float.IsNaN(max) && (float.IsNaN(dayMax) || max > dayMax))
                {
                    dayMax = max;
                }

                float min = foreRoot.list[i].main.temp_min;
                if (!float.IsNaN(min) && (float.IsNaN(dayMin) || min < dayMin))
                {
                    dayMin = min;
                }

                // Add mid-day forecast
                var currHour = weather.hr_forecast[i].date.AddSeconds(currRoot.timezone).Hour;
                if (currHour >= 11 && currHour <= 13)
                {
                    weather.forecast.Add(_.CreateForecast(foreRoot.list[i]));
                    lastDay = weather.forecast.Count - 1;
                }

                // This is possibly the last forecast for the day (3-hrly forecast)
                // Set the min / max temp here and reset
                if (currHour >= 21)
                {
                    if (lastDay >= 0)
                    {
                        if (!float.IsNaN(dayMax))
                        {
                            weather.forecast[lastDay].high_f = ConversionMethods.KtoF(dayMax);
                            weather.forecast[lastDay].high_c = ConversionMethods.KtoC(dayMax);
                        }
                        if (!float.IsNaN(dayMin))
                        {
                            weather.forecast[lastDay].low_f = ConversionMethods.KtoF(dayMin);
                            weather.forecast[lastDay].low_c = ConversionMethods.KtoC(dayMin);
                        }
                    }

                    dayMax = float.NaN;
                    dayMin = float.NaN;
                }
            }
            weather.condition = _.CreateCondition(currRoot);
            weather.atmosphere = _.CreateAtmosphere(currRoot);
            weather.astronomy = _.CreateAstronomy(currRoot);
            weather.precipitation = _.CreatePrecipitation(currRoot);
            weather.ttl = 180;

            weather.query = currRoot.id.ToString();

            // Set feelslike temp
            if (!weather.condition.feelslike_f.HasValue && weather.condition.temp_f.HasValue && weather.condition.wind_mph.HasValue && weather.atmosphere.humidity.HasValue)
            {
                weather.condition.feelslike_f = WeatherUtils.GetFeelsLikeTemp(weather.condition.temp_f.Value, weather.condition.wind_mph.Value, weather.atmosphere.humidity.Value);
                weather.condition.feelslike_c = ConversionMethods.FtoC(weather.condition.feelslike_f.Value);
            }

            if ((!weather.condition.high_f.HasValue || !weather.condition.low_f.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f;
                weather.condition.high_c = weather.forecast[0].high_c;
                weather.condition.low_f = weather.forecast[0].low_f;
                weather.condition.low_c = weather.forecast[0].low_c;
            }

            if (!weather.atmosphere.dewpoint_c.HasValue && weather.condition.temp_c.HasValue && weather.atmosphere.humidity.HasValue &&
                weather.condition.temp_c > 0 && weather.condition.temp_c < 60 && weather.atmosphere.humidity > 1)
            {
                weather.atmosphere.dewpoint_c = WeatherUtils.CalculateDewpointC(weather.condition.temp_c.Value, weather.atmosphere.humidity.Value);
                weather.atmosphere.dewpoint_f = ConversionMethods.CtoF(weather.atmosphere.dewpoint_c.Value);
            }

            weather.source = WeatherAPI.OpenWeatherMap;

            return weather;
        }

        public static Location CreateLocation(this OpenWeatherMapProvider _, ForecastRootobject root)
        {
            return new Location()
            {
                // Use location name from location provider
                name = null,
                latitude = root.city.coord.lat,
                longitude = root.city.coord.lon,
                tz_long = null,
            };
        }

        public static Forecast CreateForecast(this OpenWeatherMapProvider _, List forecast)
        {
            var fcast = new Forecast();

            fcast.date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;
            fcast.high_f = ConversionMethods.KtoF(forecast.main.temp_max);
            fcast.high_c = ConversionMethods.KtoC(forecast.main.temp_max);
            fcast.low_f = ConversionMethods.KtoF(forecast.main.temp_min);
            fcast.low_c = ConversionMethods.KtoC(forecast.main.temp_min);
            fcast.condition = forecast.weather[0].description.ToUpperCase();
            fcast.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(forecast.weather[0].id.ToInvariantString());

            // Extras
            fcast.extras = new ForecastExtras()
            {
                humidity = forecast.main.humidity,
                cloudiness = forecast.clouds.all,
                // 1hPA = 1mbar
                pressure_mb = forecast.main.pressure,
                pressure_in = ConversionMethods.MBToInHg(forecast.main.pressure),
                wind_degrees = (int)Math.Round(forecast.wind.deg),
                wind_mph = ConversionMethods.MSecToMph(forecast.wind.speed),
                wind_kph = ConversionMethods.MSecToKph(forecast.wind.speed),
            };
            if (ConversionMethods.KtoC(forecast.main.temp) is float temp_c &&
                temp_c > 0 && temp_c < 60 && forecast.main.humidity > 1)
            {
                fcast.extras.dewpoint_c = WeatherUtils.CalculateDewpointC(ConversionMethods.KtoC(forecast.main.temp), forecast.main.humidity);
                fcast.extras.dewpoint_f = ConversionMethods.CtoF(fcast.extras.dewpoint_c.Value);
            }
            if (forecast.main.feels_like.HasValue)
            {
                fcast.extras.feelslike_f = ConversionMethods.KtoF(forecast.main.feels_like.Value);
                fcast.extras.feelslike_c = ConversionMethods.KtoC(forecast.main.feels_like.Value);
            }
            if (forecast.pop.HasValue)
            {
                fcast.extras.pop = (int)Math.Round(forecast.pop.Value * 100);
            }
            if (forecast.visibility.HasValue)
            {
                fcast.extras.visibility_km = forecast.visibility.Value / 1000;
                fcast.extras.visibility_mi = ConversionMethods.KmToMi(fcast.extras.visibility_km.Value);
            }
            if (forecast.wind.gust.HasValue)
            {
                fcast.extras.windgust_mph = ConversionMethods.MSecToMph(forecast.wind.gust.Value);
                fcast.extras.windgust_kph = ConversionMethods.MSecToKph(forecast.wind.gust.Value);
            }
            if (forecast.rain?._3h.HasValue == true)
            {
                fcast.extras.qpf_rain_mm = forecast.rain._3h.Value;
                fcast.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain._3h.Value);
            }
            if (forecast.snow?._3h.HasValue == true)
            {
                fcast.extras.qpf_snow_cm = forecast.snow._3h.Value / 10;
                fcast.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow._3h.Value);
            }

            return fcast;
        }

        public static HourlyForecast CreateHourlyForecast(this OpenWeatherMapProvider _, List hr_forecast)
        {
            var hrf = new HourlyForecast();

            hrf.date = DateTimeOffset.FromUnixTimeSeconds(hr_forecast.dt);
            hrf.high_f = ConversionMethods.KtoF(hr_forecast.main.temp);
            hrf.high_c = ConversionMethods.KtoC(hr_forecast.main.temp);
            hrf.condition = hr_forecast.weather[0].description.ToUpperCase();

            // Use icon to determine if day or night
            string ico = hr_forecast.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
                dn = String.Empty;

            hrf.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(hr_forecast.weather[0].id.ToInvariantString() + dn);

            hrf.wind_degrees = (int)hr_forecast.wind.deg;
            hrf.wind_mph = ConversionMethods.MSecToMph(hr_forecast.wind.speed);
            hrf.wind_kph = ConversionMethods.MSecToKph(hr_forecast.wind.speed);

            // Extras
            hrf.extras = new ForecastExtras
            {
                humidity = hr_forecast.main.humidity,
                cloudiness = hr_forecast.clouds.all,
                // 1hPA = 1mbar
                pressure_mb = hr_forecast.main.pressure,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.main.pressure),
                wind_degrees = hrf.wind_degrees,
                wind_mph = hrf.wind_mph,
                wind_kph = hrf.wind_kph
            };
            if (hrf.high_c.Value > 0 && hrf.high_c.Value < 60 && hr_forecast.main.humidity > 1)
            {
                hrf.extras.dewpoint_c = WeatherUtils.CalculateDewpointC(hrf.high_c.Value, hr_forecast.main.humidity);
                hrf.extras.dewpoint_f = ConversionMethods.CtoF(hrf.extras.dewpoint_c.Value);
            }
            if (hr_forecast.main.feels_like.HasValue)
            {
                hrf.extras.feelslike_f = ConversionMethods.KtoF(hr_forecast.main.feels_like.Value);
                hrf.extras.feelslike_c = ConversionMethods.KtoC(hr_forecast.main.feels_like.Value);
            }
            if (hr_forecast.pop.HasValue)
            {
                hrf.extras.pop = (int)Math.Round(hr_forecast.pop.Value * 100);
            }
            if (hr_forecast.wind.gust.HasValue)
            {
                hrf.extras.windgust_mph = ConversionMethods.MSecToMph(hr_forecast.wind.gust.Value);
                hrf.extras.windgust_kph = ConversionMethods.MSecToKph(hr_forecast.wind.gust.Value);
            }
            if (hr_forecast.visibility.HasValue)
            {
                hrf.extras.visibility_km = hr_forecast.visibility.Value / 1000;
                hrf.extras.visibility_mi = ConversionMethods.KmToMi(hrf.extras.visibility_km.Value);
            }
            if (hr_forecast.rain?._3h.HasValue == true)
            {
                hrf.extras.qpf_rain_mm = hr_forecast.rain._3h.Value;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(hr_forecast.rain._3h.Value);
            }
            if (hr_forecast.snow?._3h.HasValue == true)
            {
                hrf.extras.qpf_snow_cm = hr_forecast.snow._3h.Value / 10;
                hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(hr_forecast.snow._3h.Value);
            }

            return hrf;
        }

        public static Condition CreateCondition(this OpenWeatherMapProvider _, CurrentRootobject current)
        {
            var condition = new Condition();

            condition.weather = current.weather[0].description.ToUpperCase();
            condition.temp_f = ConversionMethods.KtoF(current.main.temp);
            condition.temp_c = ConversionMethods.KtoC(current.main.temp);
            condition.high_f = ConversionMethods.KtoF(current.main.temp_max);
            condition.high_c = ConversionMethods.KtoC(current.main.temp_max);
            condition.low_f = ConversionMethods.KtoF(current.main.temp_min);
            condition.low_c = ConversionMethods.KtoC(current.main.temp_min);
            condition.wind_degrees = (int)current.wind.deg;
            condition.wind_mph = ConversionMethods.MSecToMph(current.wind.speed);
            condition.wind_kph = ConversionMethods.MSecToKph(current.wind.speed);
            if (current.main.feels_like.HasValue)
            {
                condition.feelslike_f = ConversionMethods.KtoF(current.main.feels_like.Value);
                condition.feelslike_c = ConversionMethods.KtoC(current.main.feels_like.Value);
            }
            if (current.wind.gust.HasValue)
            {
                condition.windgust_mph = ConversionMethods.MSecToMph(current.wind.gust.Value);
                condition.windgust_kph = ConversionMethods.MSecToKph(current.wind.gust.Value);
            }

            string ico = current.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
                dn = String.Empty;

            condition.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(current.weather[0].id.ToInvariantString() + dn);

            condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(current.wind.speed));

            condition.observation_time = DateTimeOffset.FromUnixTimeSeconds(current.dt);

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this OpenWeatherMapProvider _, CurrentRootobject root)
        {
            return new Atmosphere()
            {
                humidity = root.main.humidity,
                // 1hPa = 1mbar
                pressure_mb = root.main.pressure,
                pressure_in = ConversionMethods.MBToInHg(root.main.pressure),
                pressure_trend = String.Empty,
                visibility_km = root.visibility / 1000,
                visibility_mi = ConversionMethods.KmToMi(root.visibility / 1000 /*visibility_km.Value*/),
            };
        }

        public static Astronomy CreateAstronomy(this OpenWeatherMapProvider _, CurrentRootobject root)
        {
            var astronomy = new Astronomy();

            try
            {
                astronomy.sunrise = DateTimeOffset.FromUnixTimeSeconds(root.sys.sunrise.Value).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                astronomy.sunset = DateTimeOffset.FromUnixTimeSeconds(root.sys.sunset.Value).UtcDateTime;
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

        public static Precipitation CreatePrecipitation(this OpenWeatherMapProvider _, CurrentRootobject root)
        {
            var precip = new Precipitation();

            // Use cloudiness value here
            precip.cloudiness = root.clouds.all;
            if (root.rain?._1h.HasValue == true)
            {
                precip.qpf_rain_in = ConversionMethods.MMToIn(root.rain._1h.Value);
                precip.qpf_rain_mm = root.rain._1h.Value;
            }
            else if (root.rain?._3h.HasValue == true)
            {
                precip.qpf_rain_in = ConversionMethods.MMToIn(root.rain._3h.Value);
                precip.qpf_rain_mm = root.rain._3h.Value;
            }

            if (root.snow?._1h.HasValue == true)
            {
                precip.qpf_snow_in = ConversionMethods.MMToIn(root.snow._1h.Value);
                precip.qpf_snow_cm = root.snow._1h.Value / 10;
            }
            else if (root.snow?._3h.HasValue == true)
            {
                precip.qpf_snow_in = ConversionMethods.MMToIn(root.snow._3h.Value);
                precip.qpf_snow_cm = root.snow._3h.Value / 10;
            }

            return precip;
        }
    }
}