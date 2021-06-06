using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleWeather.WeatherData
{
    public partial class Weather
    {
        public Weather(Metno.Rootobject foreRoot, Metno.AstroRootobject astroRoot)
        {
            var now = DateTimeOffset.UtcNow;

            location = new Location(foreRoot);
            update_time = now;

            // 9-day forecast / hrly -> 6hrly forecast
            forecast = new List<Forecast>(10);
            hr_forecast = new List<HourlyForecast>(foreRoot.properties.timeseries.Length);

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
                    condition = new Condition(time);
                    atmosphere = new Atmosphere(time);
                    precipitation = new Precipitation(time);
                }

                // Add a new hour
                if (time.time >= now.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                    hr_forecast.Add(new HourlyForecast(time));

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

                        forecast.Add(fcast);
                    }

                    currentDate = date;
                    fcast = new Forecast(time)
                    {
                        date = date
                    };

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

            fcast = forecast.LastOrDefault();
            if (fcast?.condition == null && fcast?.icon == null)
            {
                forecast.RemoveAt(forecast.Count - 1);
            }

            if (hr_forecast.LastOrDefault() is HourlyForecast hrfcast &&
                hrfcast?.condition == null && hrfcast?.icon == null)
            {
                hr_forecast.RemoveAt(hr_forecast.Count - 1);
            }

            astronomy = new Astronomy(astroRoot);
            ttl = 120;

            query = string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);

            if ((!condition.high_f.HasValue || !condition.low_f.HasValue) && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f.Value;
                condition.high_c = forecast[0].high_c.Value;
                condition.low_f = forecast[0].low_f.Value;
                condition.low_c = forecast[0].low_c.Value;
            }

            condition.observation_time = foreRoot.properties.meta.updated_at;

            source = WeatherAPI.MetNo;
        }
    }

    public partial class Location
    {
        public Location(Metno.Rootobject foreRoot)
        {
            // API doesn't provide location name (at all)
            name = null;
            latitude = foreRoot.geometry.coordinates[1];
            longitude = foreRoot.geometry.coordinates[0];
            tz_long = null;
        }
    }

    public partial class Forecast
    {
        public Forecast(Metno.Timesery time)
        {
            date = time.time;

            if (time.data.next_12_hours != null)
            {
                icon = time.data.next_12_hours.summary.symbol_code;
            }
            else if (time.data.next_6_hours != null)
            {
                icon = time.data.next_6_hours.summary.symbol_code;
            }
            else if (time.data.next_1_hours != null)
            {
                icon = time.data.next_1_hours.summary.symbol_code;
            }
            // Don't bother setting other values; they're not available yet
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(Metno.Timesery hr_forecast)
        {
            date = new DateTimeOffset(hr_forecast.time, TimeSpan.Zero);
            high_f = ConversionMethods.CtoF(hr_forecast.data.instant.details.air_temperature.Value);
            high_c = hr_forecast.data.instant.details.air_temperature.Value;
            wind_degrees = (int)Math.Round(hr_forecast.data.instant.details.wind_from_direction.Value);
            wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.data.instant.details.wind_speed.Value));
            wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.data.instant.details.wind_speed.Value));

            if (hr_forecast.data.next_1_hours != null)
            {
                icon = hr_forecast.data.next_1_hours.summary.symbol_code;
            }
            else if (hr_forecast.data.next_6_hours != null)
            {
                icon = hr_forecast.data.next_6_hours.summary.symbol_code;
            }
            else if (hr_forecast.data.next_12_hours != null)
            {
                icon = hr_forecast.data.next_12_hours.summary.symbol_code;
            }

            float humidity = hr_forecast.data.instant.details.relative_humidity.Value;
            // Extras
            extras = new ForecastExtras()
            {
                feelslike_f = WeatherUtils.GetFeelsLikeTemp(high_f.Value, wind_mph.Value, (int)Math.Round(humidity)),
                feelslike_c = ConversionMethods.FtoC(WeatherUtils.GetFeelsLikeTemp(high_f.Value, wind_mph.Value, (int)Math.Round(humidity))),
                humidity = (int)Math.Round(humidity),
                dewpoint_f = ConversionMethods.CtoF(hr_forecast.data.instant.details.dew_point_temperature.Value),
                dewpoint_c = hr_forecast.data.instant.details.dew_point_temperature.Value,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.data.instant.details.air_pressure_at_sea_level.Value),
                pressure_mb = hr_forecast.data.instant.details.air_pressure_at_sea_level.Value,
                wind_degrees = wind_degrees,
                wind_mph = wind_mph,
                wind_kph = wind_kph
            };
            if (hr_forecast.data.instant.details.cloud_area_fraction.HasValue)
            {
                extras.cloudiness = (int)Math.Round(hr_forecast.data.instant.details.cloud_area_fraction.Value);
            }
            // Precipitation
            if (hr_forecast.data.instant.details?.probability_of_precipitation.HasValue == true)
            {
                extras.pop = (int)Math.Round(hr_forecast.data.instant.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_1_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                extras.pop = (int)Math.Round(hr_forecast.data.next_1_hours.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_6_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                extras.pop = (int)Math.Round(hr_forecast.data.next_6_hours.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_12_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                extras.pop = (int)Math.Round(hr_forecast.data.next_12_hours.details.probability_of_precipitation.Value);
            }
            if (hr_forecast.data.instant.details.wind_speed_of_gust.HasValue)
            {
                extras.windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.data.instant.details.wind_speed_of_gust.Value));
                extras.windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.data.instant.details.wind_speed_of_gust.Value));
            }
            if (hr_forecast.data.instant.details.fog_area_fraction.HasValue)
            {
                float visMi = 10.0f;
                extras.visibility_mi = (visMi - (visMi * hr_forecast.data.instant.details.fog_area_fraction.Value / 100));
                extras.visibility_km = ConversionMethods.MiToKm(extras.visibility_mi.Value);
            }
            if (hr_forecast.data.instant.details.ultraviolet_index_clear_sky.HasValue)
            {
                extras.uv_index = hr_forecast.data.instant.details.ultraviolet_index_clear_sky.Value;
            }
        }
    }

    public partial class Condition
    {
        public Condition(Metno.Timesery time)
        {
            // weather
            temp_f = ConversionMethods.CtoF(time.data.instant.details.air_temperature.Value);
            temp_c = (float)time.data.instant.details.air_temperature.Value;
            wind_degrees = (int)Math.Round(time.data.instant.details.wind_from_direction.Value);
            wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(time.data.instant.details.wind_speed.Value));
            wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(time.data.instant.details.wind_speed.Value));
            feelslike_f = WeatherUtils.GetFeelsLikeTemp(temp_f.Value, wind_mph.Value, (int)time.data.instant.details.relative_humidity.Value);
            feelslike_c = ConversionMethods.FtoC(feelslike_f.Value);
            if (time.data.instant.details.wind_speed_of_gust.HasValue)
            {
                windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(time.data.instant.details.wind_speed_of_gust.Value));
                windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(time.data.instant.details.wind_speed_of_gust.Value));
            }

            if (time.data.next_12_hours != null)
            {
                icon = time.data.next_12_hours.summary.symbol_code;
            }
            else if (time.data.next_6_hours != null)
            {
                icon = time.data.next_6_hours.summary.symbol_code;
            }
            else if (time.data.next_1_hours != null)
            {
                icon = time.data.next_1_hours.summary.symbol_code;
            }

            beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(time.data.instant.details.wind_speed.Value));
            if (time.data.instant.details.ultraviolet_index_clear_sky.HasValue)
            {
                uv = new UV(time.data.instant.details.ultraviolet_index_clear_sky.Value);
            }
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(Metno.Timesery time)
        {
            humidity = (int)Math.Round(time.data.instant.details.relative_humidity.Value);
            pressure_mb = time.data.instant.details.air_pressure_at_sea_level.Value;
            pressure_in = ConversionMethods.MBToInHg(time.data.instant.details.air_pressure_at_sea_level.Value);
            pressure_trend = String.Empty;

            if (time.data.instant.details.fog_area_fraction.HasValue)
            {
                float visMi = 10.0f;
                visibility_mi = (visMi - (visMi * time.data.instant.details.fog_area_fraction.Value / 100));
                visibility_km = ConversionMethods.MiToKm(visibility_mi.Value);
            }

            if (time.data.instant.details.dew_point_temperature.HasValue)
            {
                dewpoint_f = ConversionMethods.CtoF(time.data.instant.details.dew_point_temperature.Value);
                dewpoint_c = time.data.instant.details.dew_point_temperature.Value;
            }
        }
    }

    public partial class Astronomy
    {
        public Astronomy(Metno.AstroRootobject astroRoot)
        {
            int moonPhaseValue = -1;

            foreach (Metno.Time time in astroRoot.location.time)
            {
                if (time.sunrise != null)
                {
                    sunrise = time.sunrise.time.ToUniversalTime();
                }
                if (time.sunset != null)
                {
                    sunset = time.sunset.time.ToUniversalTime();
                }

                if (time.moonrise != null)
                {
                    moonrise = time.moonrise.time.ToUniversalTime();
                }
                if (time.moonset != null)
                {
                    moonset = time.moonset.time.ToUniversalTime();
                }

                if (time.moonphase != null)
                {
                    moonPhaseValue = (int)Math.Round(double.Parse(time.moonphase.value, CultureInfo.InvariantCulture));
                }
            }

            // If the sun won't set/rise, set time to the future
            if (sunrise == null)
            {
                sunrise = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }
            if (sunset == null)
            {
                sunset = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }
            if (moonrise == null)
            {
                moonrise = DateTime.MinValue;
            }
            if (moonset == null)
            {
                moonset = DateTime.MinValue;
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

            this.moonphase = new MoonPhase(moonPhaseType);
        }
    }

    public partial class Precipitation
    {
        public Precipitation(Metno.Timesery time)
        {
            // Use cloudiness value here
            cloudiness = (int)Math.Round(time.data.instant.details.cloud_area_fraction.Value);
            // Precipitation
            if (time.data.instant.details?.probability_of_precipitation.HasValue == true)
            {
                pop = (int)Math.Round(time.data.instant.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_1_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                pop = (int)Math.Round(time.data.next_1_hours.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_6_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                pop = (int)Math.Round(time.data.next_6_hours.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_12_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                pop = (int)Math.Round(time.data.next_12_hours.details.probability_of_precipitation.Value);
            }
            // The rest DNE
        }
    }
}