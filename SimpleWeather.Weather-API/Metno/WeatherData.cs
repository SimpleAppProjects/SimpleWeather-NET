﻿using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Weather_API.Metno
{
    public static partial class MetnoWeatherProviderExtensions
    {
        public static Weather CreateWeatherData(this MetnoWeatherProvider _, Rootobject foreRoot, AstroRootobject astroRoot)
        {
            var weather = new Weather();

            var now = DateTimeOffset.UtcNow;

            weather.location = _.CreateLocation(foreRoot);
            weather.update_time = now;

            // 9-day forecast / hrly -> 6hrly forecast
            weather.forecast = new List<Forecast>(10);
            weather.hr_forecast = new List<HourlyForecast>(foreRoot.properties.timeseries.Length);

            // Store potential min/max values
            float dayMax = float.NaN;
            float dayMin = float.NaN;

            DateTime currentDate = DateTime.MinValue;
            Forecast fcast = null;

            // Metno data is troublesome to parse thru
            for (int i = 0; i < foreRoot.properties.timeseries.Length; i++)
            {
                var time = foreRoot.properties.timeseries[i];
                DateTime date = time.time;

                // Create condition for next 2hrs from data
                if (i == 0)
                {
                    weather.condition = _.CreateCondition(time);
                    weather.atmosphere = _.CreateAtmosphere(time);
                    weather.precipitation = _.CreatePrecipitation(time);
                }

                // Add a new hour
                if (time.time >= now.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                    weather.hr_forecast.Add(_.CreateHourlyForecast(time));

                // Create new forecast
                if (currentDate.Date != date.Date && date >= currentDate.AddDays(1))
                {
                    // Last forecast for day; create forecast
                    if (fcast != null)
                    {
                        // condition (set in provider GetWeather method)
                        // date
                        fcast.date = currentDate;
                        // high
                        fcast.high_f = ConversionMethods.CtoF(dayMax);
                        fcast.high_c = (float)Math.Round(dayMax);
                        // low
                        fcast.low_f = ConversionMethods.CtoF(dayMin);
                        fcast.low_c = (float)Math.Round(dayMin);

                        weather.forecast.Add(fcast);
                    }

                    currentDate = date;
                    fcast = _.CreateForecast(time);
                    fcast.date = date;

                    // Reset
                    dayMax = float.NaN;
                    dayMin = float.NaN;
                }

                // Find max/min for each hour
                float temp = time.data.instant.details.air_temperature ?? float.NaN;
                if (!float.IsNaN(temp) && (float.IsNaN(dayMax) || temp > dayMax))
                {
                    dayMax = temp;
                }
                if (!float.IsNaN(temp) && (float.IsNaN(dayMin) || temp < dayMin))
                {
                    dayMin = temp;
                }
            }

            fcast = weather.forecast.LastOrDefault();
            if (fcast?.condition == null && fcast?.icon == null)
            {
                weather.forecast.RemoveAt(weather.forecast.Count - 1);
            }

            if (weather.hr_forecast.LastOrDefault() is HourlyForecast hrfcast &&
                hrfcast?.condition == null && hrfcast?.icon == null)
            {
                weather.hr_forecast.RemoveAt(weather.hr_forecast.Count - 1);
            }

            weather.astronomy = _.CreateAstronomy(astroRoot);
            weather.ttl = 120;

            weather.query = string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);

            if ((!weather.condition.high_f.HasValue || !weather.condition.low_f.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f.Value;
                weather.condition.high_c = weather.forecast[0].high_c.Value;
                weather.condition.low_f = weather.forecast[0].low_f.Value;
                weather.condition.low_c = weather.forecast[0].low_c.Value;
            }

            weather.condition.observation_time = foreRoot.properties.meta.updated_at;

            weather.source = WeatherAPI.MetNo;

            return weather;
        }

        public static SimpleWeather.WeatherData.Location CreateLocation(this MetnoWeatherProvider _, Rootobject foreRoot)
        {
            return new SimpleWeather.WeatherData.Location()
            {
                // API doesn't provide location name (at all)
                name = null,
                latitude = foreRoot.geometry.coordinates[1],
                longitude = foreRoot.geometry.coordinates[0],
                tz_long = null,
            };
        }

        public static Forecast CreateForecast(this MetnoWeatherProvider _, Timesery time)
        {
            var forecast = new Forecast();

            forecast.date = time.time;

            if (time.data.next_12_hours != null)
            {
                forecast.icon = time.data.next_12_hours.summary.symbol_code;
            }
            else if (time.data.next_6_hours != null)
            {
                forecast.icon = time.data.next_6_hours.summary.symbol_code;
            }
            else if (time.data.next_1_hours != null)
            {
                forecast.icon = time.data.next_1_hours.summary.symbol_code;
            }
            // Don't bother setting other values; they're not available yet

            return forecast;
        }

        public static HourlyForecast CreateHourlyForecast(this MetnoWeatherProvider _, Timesery hr_forecast)
        {
            var hrf = new HourlyForecast();

            hrf.date = new DateTimeOffset(hr_forecast.time, TimeSpan.Zero);
            hrf.high_f = ConversionMethods.CtoF(hr_forecast.data.instant.details.air_temperature.Value);
            hrf.high_c = hr_forecast.data.instant.details.air_temperature.Value;
            hrf.wind_degrees = (int)Math.Round(hr_forecast.data.instant.details.wind_from_direction.Value);
            hrf.wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.data.instant.details.wind_speed.Value));
            hrf.wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.data.instant.details.wind_speed.Value));

            if (hr_forecast.data.next_1_hours != null)
            {
                hrf.icon = hr_forecast.data.next_1_hours.summary.symbol_code;
            }
            else if (hr_forecast.data.next_6_hours != null)
            {
                hrf.icon = hr_forecast.data.next_6_hours.summary.symbol_code;
            }
            else if (hr_forecast.data.next_12_hours != null)
            {
                hrf.icon = hr_forecast.data.next_12_hours.summary.symbol_code;
            }

            float humidity = hr_forecast.data.instant.details.relative_humidity.Value;
            // Extras
            hrf.extras = new ForecastExtras()
            {
                feelslike_f = WeatherUtils.GetFeelsLikeTemp(hrf.high_f.Value, hrf.wind_mph.Value, (int)Math.Round(humidity)),
                feelslike_c = ConversionMethods.FtoC(WeatherUtils.GetFeelsLikeTemp(hrf.high_f.Value, hrf.wind_mph.Value, (int)Math.Round(humidity))),
                humidity = (int)Math.Round(humidity),
                dewpoint_f = ConversionMethods.CtoF(hr_forecast.data.instant.details.dew_point_temperature.Value),
                dewpoint_c = hr_forecast.data.instant.details.dew_point_temperature.Value,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.data.instant.details.air_pressure_at_sea_level.Value),
                pressure_mb = hr_forecast.data.instant.details.air_pressure_at_sea_level.Value,
                wind_degrees = hrf.wind_degrees,
                wind_mph = hrf.wind_mph,
                wind_kph = hrf.wind_kph
            };
            if (hr_forecast.data.instant.details.cloud_area_fraction.HasValue)
            {
                hrf.extras.cloudiness = (int)Math.Round(hr_forecast.data.instant.details.cloud_area_fraction.Value);
            }
            // Precipitation
            if (hr_forecast.data.instant.details?.probability_of_precipitation.HasValue == true)
            {
                hrf.extras.pop = (int)Math.Round(hr_forecast.data.instant.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_1_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                hrf.extras.pop = (int)Math.Round(hr_forecast.data.next_1_hours.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_6_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                hrf.extras.pop = (int)Math.Round(hr_forecast.data.next_6_hours.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_12_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                hrf.extras.pop = (int)Math.Round(hr_forecast.data.next_12_hours.details.probability_of_precipitation.Value);
            }
            if (hr_forecast.data.instant.details.wind_speed_of_gust.HasValue)
            {
                hrf.extras.windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.data.instant.details.wind_speed_of_gust.Value));
                hrf.extras.windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.data.instant.details.wind_speed_of_gust.Value));
            }
            if (hr_forecast.data.instant.details.fog_area_fraction.HasValue)
            {
                float visMi = 10.0f;
                hrf.extras.visibility_mi = (visMi - (visMi * hr_forecast.data.instant.details.fog_area_fraction.Value / 100));
                hrf.extras.visibility_km = ConversionMethods.MiToKm(hrf.extras.visibility_mi.Value);
            }
            if (hr_forecast.data.instant.details.ultraviolet_index_clear_sky.HasValue)
            {
                hrf.extras.uv_index = hr_forecast.data.instant.details.ultraviolet_index_clear_sky.Value;
            }

            return hrf;
        }

        public static Condition CreateCondition(this MetnoWeatherProvider _, Timesery time)
        {
            var condition = new Condition();

            // weather
            condition.temp_f = ConversionMethods.CtoF(time.data.instant.details.air_temperature.Value);
            condition.temp_c = (float)time.data.instant.details.air_temperature.Value;
            condition.wind_degrees = (int)Math.Round(time.data.instant.details.wind_from_direction.Value);
            condition.wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(time.data.instant.details.wind_speed.Value));
            condition.wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(time.data.instant.details.wind_speed.Value));
            condition.feelslike_f = WeatherUtils.GetFeelsLikeTemp(condition.temp_f.Value, condition.wind_mph.Value, (int)time.data.instant.details.relative_humidity.Value);
            condition.feelslike_c = ConversionMethods.FtoC(condition.feelslike_f.Value);
            if (time.data.instant.details.wind_speed_of_gust.HasValue)
            {
                condition.windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(time.data.instant.details.wind_speed_of_gust.Value));
                condition.windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(time.data.instant.details.wind_speed_of_gust.Value));
            }

            if (time.data.next_1_hours != null)
            {
                condition.icon = time.data.next_1_hours.summary.symbol_code;
            }
            else if (time.data.next_6_hours != null)
            {
                condition.icon = time.data.next_6_hours.summary.symbol_code;
            }
            else if (time.data.next_12_hours != null)
            {
                condition.icon = time.data.next_12_hours.summary.symbol_code;
            }

            condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(time.data.instant.details.wind_speed.Value));
            if (time.data.instant.details.ultraviolet_index_clear_sky.HasValue)
            {
                condition.uv = new UV(time.data.instant.details.ultraviolet_index_clear_sky.Value);
            }

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this MetnoWeatherProvider _, Timesery time)
        {
            var atmosphere = new Atmosphere();

            atmosphere.humidity = (int)Math.Round(time.data.instant.details.relative_humidity.Value);
            atmosphere.pressure_mb = time.data.instant.details.air_pressure_at_sea_level.Value;
            atmosphere.pressure_in = ConversionMethods.MBToInHg(time.data.instant.details.air_pressure_at_sea_level.Value);
            atmosphere.pressure_trend = String.Empty;

            if (time.data.instant.details.fog_area_fraction.HasValue)
            {
                float visMi = 10.0f;
                atmosphere.visibility_mi = (visMi - (visMi * time.data.instant.details.fog_area_fraction.Value / 100));
                atmosphere.visibility_km = ConversionMethods.MiToKm(atmosphere.visibility_mi.Value);
            }

            if (time.data.instant.details.dew_point_temperature.HasValue)
            {
                atmosphere.dewpoint_f = ConversionMethods.CtoF(time.data.instant.details.dew_point_temperature.Value);
                atmosphere.dewpoint_c = time.data.instant.details.dew_point_temperature.Value;
            }

            return atmosphere;
        }

        public static Astronomy CreateAstronomy(this MetnoWeatherProvider _, AstroRootobject astroRoot)
        {
            var astronomy = new Astronomy();

            int moonPhaseValue = -1;

            foreach (Time time in astroRoot.location.time)
            {
                if (time.sunrise != null)
                {
                    astronomy.sunrise = time.sunrise.time.UtcDateTime;
                }
                if (time.sunset != null)
                {
                    astronomy.sunset = time.sunset.time.UtcDateTime;
                }

                if (time.moonrise != null)
                {
                    astronomy.moonrise = time.moonrise.time.UtcDateTime;
                }
                if (time.moonset != null)
                {
                    astronomy.moonset = time.moonset.time.UtcDateTime;
                }

                if (time.moonphase != null)
                {
                    moonPhaseValue = (int)Math.Round(double.Parse(time.moonphase.value, CultureInfo.InvariantCulture));
                }
            }

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

            MoonPhase.MoonPhaseType moonPhaseType;
            if (moonPhaseValue >= 2 && moonPhaseValue < 23)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaxingCrescent;
            }
            else if (moonPhaseValue >= 23 && moonPhaseValue < 26)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.FirstQtr;
            }
            else if (moonPhaseValue >= 26 && moonPhaseValue < 48)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaxingGibbous;
            }
            else if (moonPhaseValue >= 48 && moonPhaseValue < 52)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.FullMoon;
            }
            else if (moonPhaseValue >= 52 && moonPhaseValue < 73)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaningGibbous;
            }
            else if (moonPhaseValue >= 73 && moonPhaseValue < 76)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.LastQtr;
            }
            else if (moonPhaseValue >= 76 && moonPhaseValue < 98)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaningCrescent;
            }
            else
            {
                // 0, 1, 98, 99, 100
                moonPhaseType = MoonPhase.MoonPhaseType.NewMoon;
            }

            astronomy.moonphase = new MoonPhase(moonPhaseType);

            return astronomy;
        }

        public static Precipitation CreatePrecipitation(this MetnoWeatherProvider _, Timesery time)
        {
            var precip = new Precipitation();

            // Use cloudiness value here
            precip.cloudiness = (int)Math.Round(time.data.instant.details.cloud_area_fraction.Value);
            // Precipitation
            if (time.data.instant.details?.probability_of_precipitation.HasValue == true)
            {
                precip.pop = (int)Math.Round(time.data.instant.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_1_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                precip.pop = (int)Math.Round(time.data.next_1_hours.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_6_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                precip.pop = (int)Math.Round(time.data.next_6_hours.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_12_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                precip.pop = (int)Math.Round(time.data.next_12_hours.details.probability_of_precipitation.Value);
            }
            // The rest DNE

            return precip;
        }
    }
}