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
        public Weather(NWS.Observation.ForecastRootobject forecastResponse, NWS.Hourly.HourlyForecastResponse hourlyForecastResponse)
        {
            location = new Location(forecastResponse);
            var now = DateTimeOffset.UtcNow;
            update_time = now;

            // ~8-day forecast
            forecast = new List<Forecast>(8);
            txt_forecast = new List<TextForecast>(16);

            {
                int periodsSize = forecastResponse.time.startValidTime.Length;
                for (int i = 0; i < periodsSize; i++)
                {
                    NWS.Observation.PeriodsItem forecastItem = new NWS.Observation.PeriodsItem(
                            forecastResponse.time.startPeriodName[i],
                            forecastResponse.time.startValidTime[i],
                            forecastResponse.time.tempLabel[i],
                            forecastResponse.data.temperature[i],
                            forecastResponse.data.pop[i],
                            forecastResponse.data.weather[i],
                            forecastResponse.data.iconLink[i],
                            forecastResponse.data.text[i]
                        );

                    if ((!forecast.Any() && !forecastItem.IsDaytime) || (forecast.Count == periodsSize - 1 && forecastItem.IsDaytime))
                    {
                        forecast.Add(new Forecast(forecastItem));
                        txt_forecast.Add(new TextForecast(forecastItem));
                    }
                    else if (forecastItem.IsDaytime && (i + 1) < periodsSize)
                    {
                        NWS.Observation.PeriodsItem nightForecastItem = new NWS.Observation.PeriodsItem(
                            forecastResponse.time.startPeriodName[i + 1],
                            forecastResponse.time.startValidTime[i + 1],
                            forecastResponse.time.tempLabel[i + 1],
                            forecastResponse.data.temperature[i + 1],
                            forecastResponse.data.pop[i + 1],
                            forecastResponse.data.weather[i + 1],
                            forecastResponse.data.iconLink[i + 1],
                            forecastResponse.data.text[i + 1]
                        );

                        forecast.Add(new Forecast(forecastItem, nightForecastItem));
                        txt_forecast.Add(new TextForecast(forecastItem, nightForecastItem));

                        i++;
                    }
                }
            }

            {
                bool adjustDate = false;
                var creationDate = hourlyForecastResponse.creationDate;
                hr_forecast = new List<HourlyForecast>(144);
                foreach (NWS.Hourly.PeriodsItem period in hourlyForecastResponse.periodsItems)
                {
                    int periodsSize = period.unixtime.Count;
                    for (int i = 0; i < periodsSize; i++)
                    {
                        var date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(period.unixtime[i]));

                        // BUG: NWS MapClick API
                        // The epoch time sometimes is a day ahead
                        // If this is the case, adjust all dates accordingly
                        if (i == 0 && period.periodName?.Contains("night") == true && Equals("6 pm", period.time[i]))
                        {
                            var hrDate = date.ToOffset(creationDate.Offset);
                            var futureDate = creationDate.AddDays(1).Date;
                            if (futureDate.Equals(hrDate.Date))
                            {
                                adjustDate = true;
                            }
                        }

                        if (adjustDate)
                        {
                            date = date.AddDays(-1);
                        }

                        if (date.UtcDateTime < now.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                            continue;

                        NWS.Hourly.PeriodItem forecastItem = new NWS.Hourly.PeriodItem(
                                period.unixtime[i],
                                period.windChill[i],
                                period.windSpeed[i],
                                period.cloudAmount[i],
                                period.pop[i],
                                period.relativeHumidity[i],
                                period.windGust[i],
                                period.temperature[i],
                                period.windDirection[i],
                                period.iconLink[i],
                                period.weather[i]
                            );

                        hr_forecast.Add(new HourlyForecast(forecastItem, adjustDate));
                    }
                }
            }

            condition = new Condition(forecastResponse);
            atmosphere = new Atmosphere(forecastResponse);
            //astronomy = new Astronomy(obsCurrentRootObject);
            precipitation = new Precipitation(forecastResponse);
            ttl = 180;

            if (!condition.high_f.HasValue && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f;
                condition.high_c = forecast[0].high_c;
            }
            if (!condition.low_f.HasValue && forecast.Count > 0)
            {
                condition.low_f = forecast[0].low_f;
                condition.low_c = forecast[0].low_c;
            }

            source = WeatherAPI.NWS;
        }
    }

    public partial class Location
    {
        public Location(NWS.Observation.ForecastRootobject forecastResponse)
        {
            // Use location name from location provider
            name = null;
            if (float.TryParse(forecastResponse.location.latitude, out float result_lat))
            {
                latitude = result_lat;
            }
            else
            {
                latitude = null;
            }
            if (float.TryParse(forecastResponse.location.longitude, out float result_lon))
            {
                longitude = result_lon;
            }
            else
            {
                longitude = null;
            }
            tz_long = null;
        }
    }

    public partial class Forecast
    {
        public Forecast(NWS.Observation.PeriodsItem forecastItem)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var culture = CultureUtils.UserCulture;

            date = forecastItem.startTime.DateTime;
            if (float.TryParse(forecastItem.temperature, out float temp))
            {
                if (forecastItem.IsDaytime)
                {
                    high_f = temp;
                    high_c = ConversionMethods.FtoC(temp);
                }
                else
                {
                    low_f = temp;
                    low_c = ConversionMethods.FtoC(temp);
                }
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = forecastItem.shortForecast;
            }
            else
            {
                condition = provider.GetWeatherCondition(forecastItem.icon);
            }
            icon = provider.GetWeatherIcon(!forecastItem.IsDaytime, forecastItem.icon);

            extras = new ForecastExtras();
            if (int.TryParse(forecastItem.pop, out int pop))
            {
                extras.pop = pop;
            }
            else
            {
                extras.pop = 0;
            }
        }

        public Forecast(NWS.Observation.PeriodsItem forecastItem, NWS.Observation.PeriodsItem nightForecastItem)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var culture = CultureUtils.UserCulture;

            date = forecastItem.startTime.DateTime;
            if (float.TryParse(forecastItem.temperature, out float hiTemp))
            {
                high_f = hiTemp;
                high_c = ConversionMethods.FtoC(hiTemp);
            }
            if (float.TryParse(nightForecastItem.temperature, out float loTemp))
            {
                low_f = loTemp;
                low_c = ConversionMethods.FtoC(loTemp);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = forecastItem.shortForecast;
            }
            else
            {
                condition = provider.GetWeatherCondition(forecastItem.icon);
            }
            icon = provider.GetWeatherIcon(false, forecastItem.icon);

            extras = new ForecastExtras();
            if (int.TryParse(forecastItem.pop, out int pop))
            {
                extras.pop = pop;
            }
            else
            {
                extras.pop = 0;
            }
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(NWS.Hourly.PeriodItem forecastItem, bool adjustDate = false)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var culture = CultureUtils.UserCulture;

            date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(forecastItem.unixTime));
            if (adjustDate)
            {
                date = date.AddDays(-1);
            }

            if (float.TryParse(forecastItem.temperature, out float temp))
            {
                high_f = temp;
                high_c = ConversionMethods.FtoC(temp);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = forecastItem.weather;
            }
            else
            {
                condition = provider.GetWeatherCondition(forecastItem.iconLink);
            }
            icon = forecastItem.iconLink;

            // Extras
            extras = new ForecastExtras();

            if (float.TryParse(forecastItem.windSpeed, out float windSpeed) &&
                int.TryParse(forecastItem.windDirection, out int windDir))
            {
                wind_degrees = windDir;
                wind_mph = windSpeed;
                wind_kph = ConversionMethods.MphToKph(windSpeed);

                extras.wind_degrees = wind_degrees;
                extras.wind_mph = wind_mph;
                extras.wind_kph = wind_kph;
            }

            if (float.TryParse(forecastItem.windChill, out float windChill))
            {
                extras.feelslike_f = windChill;
                extras.feelslike_c = ConversionMethods.FtoC(windChill);
            }

            if (int.TryParse(forecastItem.cloudAmount, out int cloudiness))
            {
                extras.cloudiness = cloudiness;
            }

            if (int.TryParse(forecastItem.pop, out int pop))
            {
                extras.pop = pop;
            }

            if (float.TryParse(forecastItem.windGust, out float windGust))
            {
                extras.windgust_mph = windGust;
                extras.windgust_kph = ConversionMethods.MphToKph(windGust);
            }
        }
    }

    public partial class TextForecast
    {
        public TextForecast(NWS.Observation.PeriodsItem forecastItem)
        {
            date = forecastItem.startTime;
            fcttext = String.Format(CultureInfo.InvariantCulture,
                "{0} - {1}", forecastItem.name, forecastItem.detailedForecast);
            fcttext_metric = fcttext;
        }

        public TextForecast(NWS.Observation.PeriodsItem forecastItem, NWS.Observation.PeriodsItem ntForecastItem)
        {
            date = forecastItem.startTime;
            fcttext = String.Format(CultureInfo.InvariantCulture,
                "{0} - {1}\n\n{2} - {3}",
                forecastItem.name, forecastItem.detailedForecast,
                ntForecastItem.name, ntForecastItem.detailedForecast);
            fcttext_metric = fcttext;
        }
    }

    public partial class Condition
    {
        public Condition(NWS.Observation.ForecastRootobject forecastResponse)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var culture = CultureUtils.UserCulture;

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                weather = forecastResponse.currentobservation.Weather;
            }
            else
            {
                weather = provider.GetWeatherCondition(forecastResponse.currentobservation.Weatherimage);
            }
            icon = forecastResponse.currentobservation.Weatherimage;

            if (float.TryParse(forecastResponse.currentobservation.Temp, out float temp))
            {
                temp_f = temp;
                temp_c = ConversionMethods.FtoC(temp);
            }

            if (int.TryParse(forecastResponse.currentobservation.Windd, out int windDir))
            {
                wind_degrees = windDir;
            }

            if (float.TryParse(forecastResponse.currentobservation.Winds, out float windSpeed))
            {
                wind_mph = windSpeed;
                wind_kph = ConversionMethods.MphToKph(windSpeed);
            }

            if (float.TryParse(forecastResponse.currentobservation.Gust, out float windGust))
            {
                windgust_mph = windGust;
                windgust_kph = ConversionMethods.MphToKph(windGust);
            }

            if (float.TryParse(forecastResponse.currentobservation.WindChill, out float windChill))
            {
                feelslike_f = windChill;
                feelslike_c = ConversionMethods.FtoC(windChill);
            }
            else if (temp_f.HasValue && !Equals(temp_f, temp_c) && wind_mph.HasValue)
            {
                if (float.TryParse(forecastResponse.currentobservation.Relh, out float humidity) && humidity >= 0)
                {
                    feelslike_f = WeatherUtils.GetFeelsLikeTemp(temp_f.Value, wind_mph.Value, (int)Math.Round(humidity));
                    feelslike_c = ConversionMethods.FtoC(feelslike_f.Value);
                }
            }

            if (wind_mph.HasValue)
            {
                beaufort = new Beaufort((int)WeatherUtils.GetBeaufortScale((int)Math.Round(wind_mph.Value)));
            }

            observation_time = forecastResponse.creationDate;
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(NWS.Observation.ForecastRootobject forecastResponse)
        {
            if (int.TryParse(forecastResponse.currentobservation.Relh, out int relh))
            {
                humidity = relh;
            }

            if (float.TryParse(forecastResponse.currentobservation.SLP, out float pressure))
            {
                pressure_in = pressure;
                pressure_mb = ConversionMethods.InHgToMB(pressure);
            }
            pressure_trend = String.Empty;

            if (float.TryParse(forecastResponse.currentobservation.Visibility, out float visibility))
            {
                visibility_mi = visibility;
                visibility_km = ConversionMethods.MiToKm(visibility);
            }

            if (float.TryParse(forecastResponse.currentobservation.Dewp, out float dewp))
            {
                dewpoint_f = dewp;
                dewpoint_c = ConversionMethods.FtoC(dewp);
            }
        }
    }

    public partial class Precipitation
    {
        public Precipitation(NWS.Observation.ForecastRootobject forecastResponse)
        {
            // The rest DNE
        }
    }
}