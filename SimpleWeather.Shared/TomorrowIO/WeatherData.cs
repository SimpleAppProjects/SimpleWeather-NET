using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.WeatherData
{
    public partial class Weather
    {
        public Weather(TomorrowIO.Rootobject root, TomorrowIO.Rootobject minutelyRoot, TomorrowIO.AlertRootobject alertRoot)
        {
            location = new Location(root);
            update_time = DateTimeOffset.UtcNow;

            foreach (var timeline in root.data.timelines)
            {
                if (timeline.timestep == "1h")
                {
                    hr_forecast = new List<HourlyForecast>(timeline.intervals.Length);

                    foreach (var interval in timeline.intervals)
                    {
                        hr_forecast.Add(new HourlyForecast(interval));
                    }
                }
                else if (timeline.timestep == "1d")
                {
                    forecast = new List<Forecast>(timeline.intervals.Length);

                    foreach (var interval in timeline.intervals)
                    {
                        if (astronomy == null && update_time.Date.Equals(interval.startTime.Date))
                        {
                            astronomy = new Astronomy(interval);
                        }

                        forecast.Add(new Forecast(interval));
                    }
                }
                else if (timeline.timestep == "current")
                {
                    condition = new Condition(timeline.intervals[0]);
                    atmosphere = new Atmosphere(timeline.intervals[0]);
                    precipitation = new Precipitation(timeline.intervals[0]);
                }
            }

            if (minutelyRoot != null)
            {
                foreach (var timeline in minutelyRoot.data.timelines)
                {
                    if (timeline.timestep == "1m")
                    {
                        min_forecast = new List<MinutelyForecast>(timeline.intervals.Length);

                        foreach (var interval in timeline.intervals)
                        {
                            min_forecast.Add(new MinutelyForecast(interval));
                        }
                    }
                }
            }

            if ((!condition.high_f.HasValue || !condition.high_c.HasValue) && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f;
                condition.high_c = forecast[0].high_c;
                condition.low_f = forecast[0].low_f;
                condition.low_c = forecast[0].low_c;
            }

            if (alertRoot?.data?.events?.Length > 0)
            {
                weather_alerts = new HashSet<WeatherAlert>(alertRoot.data.events.Length);

                foreach (var @event in alertRoot.data.events)
                {
                    weather_alerts.Add(new WeatherAlert(@event));
                }
            }

            ttl = 180;
            source = WeatherAPI.TomorrowIo;
        }
    }

    public partial class Location
    {
        public Location(TomorrowIO.Rootobject root)
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
        public Forecast(TomorrowIO.Interval item)
        {
            date = item.startTime.UtcDateTime;

            if (item.values.temperatureMax.HasValue)
            {
                high_f = ConversionMethods.CtoF(item.values.temperatureMax.Value);
                high_c = item.values.temperatureMax;
            }
            if (item.values.temperatureMin.HasValue)
            {
                low_f = ConversionMethods.CtoF(item.values.temperatureMin.Value);
                low_c = item.values.temperatureMin;
            }

            icon = item.values.weatherCode?.ToString();

            // Extras
            extras = new ForecastExtras();
            if (item.values.temperatureApparent.HasValue)
            {
                extras.feelslike_f = ConversionMethods.CtoF(item.values.temperatureApparent.Value);
                extras.feelslike_c = item.values.temperatureApparent;
            }
            if (item.values.humidity.HasValue)
            {
                extras.humidity = (int)MathF.Round(item.values.humidity.Value);
            }
            if (item.values.dewPoint.HasValue)
            {
                extras.dewpoint_c = item.values.dewPoint.Value;
                extras.dewpoint_f = MathF.Round(ConversionMethods.CtoF(item.values.dewPoint.Value));
            }
            extras.pop = item.values.precipitationProbability?.RoundToInt();
            if (item.values.cloudCover.HasValue)
            {
                extras.cloudiness = (int)MathF.Round(item.values.cloudCover.Value);
            }
            if (item.values.precipitationIntensity.HasValue)
            {
                extras.qpf_rain_mm = item.values.precipitationIntensity.Value;
                extras.qpf_rain_in = ConversionMethods.MMToIn(item.values.precipitationIntensity.Value);
            }
            if (item.values.snowAccumulation.HasValue)
            {
                extras.qpf_snow_cm = item.values.snowAccumulation.Value / 10;
                extras.qpf_snow_in = ConversionMethods.MMToIn(item.values.snowAccumulation.Value);
            }
            if (item.values.pressureSeaLevel.HasValue)
            {
                extras.pressure_mb = item.values.pressureSeaLevel.Value;
                extras.pressure_in = ConversionMethods.MBToInHg(item.values.pressureSeaLevel.Value).RoundToInt();
            }
            if (item.values.windDirection.HasValue)
            {
                extras.wind_degrees = (int)MathF.Round(item.values.windDirection.Value);
            }
            if (item.values.windSpeed.HasValue)
            {
                extras.wind_mph = ConversionMethods.MSecToMph(item.values.windSpeed.Value);
                extras.wind_kph = ConversionMethods.MSecToKph(item.values.windSpeed.Value);
            }
            if (item.values.windGust.HasValue)
            {
                extras.windgust_mph = ConversionMethods.MSecToMph(item.values.windGust.Value);
                extras.windgust_kph = ConversionMethods.MSecToKph(item.values.windGust.Value);
            }
            if (item.values.visibility.HasValue)
            {
                extras.visibility_mi = ConversionMethods.KmToMi(item.values.visibility.Value);
                extras.visibility_km = item.values.visibility;
            }
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(TomorrowIO.Interval item)
        {
            date = item.startTime;

            if (item.values.temperatureMax.HasValue)
            {
                high_f = ConversionMethods.CtoF(item.values.temperatureMax.Value);
                high_c = item.values.temperatureMax;
            }

            icon = item.values.weatherCode?.ToString();

            // Extras
            extras = new ForecastExtras();
            if (item.values.temperatureApparent.HasValue)
            {
                extras.feelslike_f = ConversionMethods.CtoF(item.values.temperatureApparent.Value);
                extras.feelslike_c = item.values.temperatureApparent;
            }
            if (item.values.humidity.HasValue)
            {
                extras.humidity = (int)MathF.Round(item.values.humidity.Value);
            }
            if (item.values.dewPoint.HasValue)
            {
                extras.dewpoint_c = item.values.dewPoint.Value;
                extras.dewpoint_f = MathF.Round(ConversionMethods.CtoF(item.values.dewPoint.Value));
            }
            extras.pop = item.values.precipitationProbability?.RoundToInt();
            if (item.values.cloudCover.HasValue)
            {
                extras.cloudiness = (int)MathF.Round(item.values.cloudCover.Value);
            }
            if (item.values.precipitationIntensity.HasValue)
            {
                extras.qpf_rain_mm = item.values.precipitationIntensity.Value;
                extras.qpf_rain_in = ConversionMethods.MMToIn(item.values.precipitationIntensity.Value);
            }
            if (item.values.snowAccumulation.HasValue)
            {
                extras.qpf_snow_cm = item.values.snowAccumulation.Value / 10;
                extras.qpf_snow_in = ConversionMethods.MMToIn(item.values.snowAccumulation.Value);
            }
            if (item.values.pressureSeaLevel.HasValue)
            {
                extras.pressure_mb = item.values.pressureSeaLevel.Value;
                extras.pressure_in = ConversionMethods.MBToInHg(item.values.pressureSeaLevel.Value).RoundToInt();
            }
            if (item.values.windDirection.HasValue)
            {
                extras.wind_degrees = (int)MathF.Round(item.values.windDirection.Value);
                wind_degrees = extras.wind_degrees;
            }
            if (item.values.windSpeed.HasValue)
            {
                extras.wind_mph = ConversionMethods.MSecToMph(item.values.windSpeed.Value);
                wind_mph = extras.wind_mph;

                extras.wind_kph = ConversionMethods.MSecToKph(item.values.windSpeed.Value);
                wind_kph = extras.wind_kph;
            }
            if (item.values.windGust.HasValue)
            {
                extras.windgust_mph = ConversionMethods.MSecToMph(item.values.windGust.Value);
                extras.windgust_kph = ConversionMethods.MSecToKph(item.values.windGust.Value);
            }
            if (item.values.visibility.HasValue)
            {
                extras.visibility_mi = ConversionMethods.KmToMi(item.values.visibility.Value);
                extras.visibility_km = item.values.visibility;
            }
        }
    }

    public partial class MinutelyForecast
    {
        public MinutelyForecast(TomorrowIO.Interval item)
        {
            date = item.startTime;
            rain_mm = item.values.precipitationIntensity;
        }
    }

    public partial class Condition
    {
        public Condition(TomorrowIO.Interval item)
        {
            weather = null;

            if (item.values.temperature.HasValue)
            {
                temp_f = ConversionMethods.CtoF(item.values.temperature.Value);
                temp_c = item.values.temperature;
            }

            wind_degrees = item.values.windDirection?.RoundToInt();
            if (item.values.windSpeed.HasValue)
            {
                wind_mph = ConversionMethods.MSecToMph(item.values.windSpeed.Value);
                wind_kph = ConversionMethods.MSecToKph(item.values.windSpeed.Value);
                beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(item.values.windSpeed.Value));
            }

            if (item.values.windGust.HasValue)
            {
                windgust_mph = ConversionMethods.MSecToMph(item.values.windGust.Value);
                windgust_kph = ConversionMethods.MSecToKph(item.values.windGust.Value);
            }

            if (item.values.temperatureApparent.HasValue)
            {
                feelslike_f = ConversionMethods.CtoF(item.values.temperatureApparent.Value);
                feelslike_c = item.values.temperatureApparent;
            }

            icon = item.values.weatherCode?.ToString();

            if (item.values.temperatureMax.HasValue)
            {
                high_c = item.values.temperatureMax;
                high_f = ConversionMethods.CtoF(item.values.temperatureMax.Value);
            }

            if (item.values.temperatureMin.HasValue)
            {
                low_c = item.values.temperatureMin;
                low_f = ConversionMethods.CtoF(item.values.temperatureMin.Value);
            }

            airQuality = new AirQuality()
            {
                index = item.values.epaIndex
            };

            pollen = new Pollen()
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

            observation_time = item.startTime;
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(TomorrowIO.Interval item)
        {
            humidity = item.values.humidity?.RoundToInt();

            if (item.values.pressureSeaLevel.HasValue)
            {
                pressure_mb = item.values.pressureSeaLevel;
                pressure_in = ConversionMethods.MBToInHg(item.values.pressureSeaLevel.Value);
            }
            pressure_trend = string.Empty;

            if (item.values.visibility.HasValue)
            {
                visibility_mi = ConversionMethods.KmToMi(item.values.visibility.Value);
                visibility_km = item.values.visibility.Value;
            }

            if (item.values.dewPoint.HasValue)
            {
                dewpoint_f = ConversionMethods.CtoF(item.values.dewPoint.Value);
                dewpoint_c = item.values.dewPoint.Value;
            }
        }
    }

    public partial class Astronomy
    {
        public Astronomy(TomorrowIO.Interval item)
        {
            try
            {
                sunrise = item.values.sunriseTime.UtcDateTime;
            }
            catch { }

            try
            {
                sunset = item.values.sunsetTime.UtcDateTime;
            }
            catch { }

            moonphase = item.values.moonPhase switch
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
        }
    }

    public partial class Precipitation
    {
        public Precipitation(TomorrowIO.Interval item)
        {
            pop = item.values.precipitationProbability?.RoundToInt();
            cloudiness = item.values.cloudCover?.RoundToInt();

            if (item.values.precipitationIntensity.HasValue)
            {
                qpf_rain_in = ConversionMethods.MMToIn(item.values.precipitationIntensity.Value);
                qpf_rain_mm = item.values.precipitationIntensity.Value;
            }
        }
    }
}