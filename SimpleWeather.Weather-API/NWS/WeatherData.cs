using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Weather_API.NWS
{
    public static partial class NWSWeatherProviderExtensions
    {
        public static Weather CreateWeatherData(this NWSWeatherProvider _, NWS.Observation.ForecastRootobject forecastResponse, NWS.Hourly.HourlyForecastResponse hourlyForecastResponse)
        {
            var weather = new Weather();

            weather.location = _.CreateLocation(forecastResponse);
            var now = DateTimeOffset.UtcNow;
            weather.update_time = now;

            weather.condition = _.CreateCondition(forecastResponse);

            // ~8-day forecast
            weather.forecast = new List<Forecast>(8);
            weather.txt_forecast = new List<TextForecast>(16);

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

                    if ((!weather.forecast.Any() && !forecastItem.IsDaytime) || (weather.forecast.Count == periodsSize - 1 && forecastItem.IsDaytime))
                    {
                        var fcast = _.CreateForecast(forecastItem);
                        var txtfcast = _.CreateTextForecast(forecastItem);

                        weather.forecast.Add(fcast);
                        weather.txt_forecast.Add(txtfcast);

                        if (weather.condition.summary == null && weather.condition.observation_time.UtcDateTime.Date >= txtfcast.date.UtcDateTime.Date)
                        {
                            weather.condition.summary = String.Format(CultureInfo.InvariantCulture,
                                "{0} - {1}", forecastItem.name, forecastItem.detailedForecast);
                        }
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

                        var fcast = _.CreateForecast(forecastItem, nightForecastItem);
                        var txtfcast = _.CreateTextForecast(forecastItem, nightForecastItem);

                        weather.forecast.Add(fcast);
                        weather.txt_forecast.Add(txtfcast);

                        if (weather.condition.summary == null && weather.condition.observation_time.UtcDateTime.Date >= txtfcast.date.UtcDateTime.Date)
                        {
                            weather.condition.summary = String.Format(CultureInfo.InvariantCulture,
                                "{0} - {1}\n{2} - {3}",
                                forecastItem.name, forecastItem.detailedForecast,
                                nightForecastItem.name, nightForecastItem.detailedForecast);
                        }

                        i++;
                    }
                }
            }

            {
                bool adjustDate = false;
                var creationDate = hourlyForecastResponse.creationDate;
                weather.hr_forecast = new List<HourlyForecast>(144);
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

                        weather.hr_forecast.Add(_.CreateHourlyForecast(forecastItem, adjustDate));
                    }
                }
            }

            weather.atmosphere = _.CreateAtmosphere(forecastResponse);
            //weather.astronomy = _.CreateAstronomy(obsCurrentRootObject);
            weather.precipitation = _.CreatePrecipitation(forecastResponse);
            weather.ttl = 180;

            if (!weather.condition.high_f.HasValue && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f;
                weather.condition.high_c = weather.forecast[0].high_c;
            }
            if (!weather.condition.low_f.HasValue && weather.forecast.Count > 0)
            {
                weather.condition.low_f = weather.forecast[0].low_f;
                weather.condition.low_c = weather.forecast[0].low_c;
            }

            weather.source = WeatherAPI.NWS;

            return weather;
        }

        public static Location CreateLocation(this NWSWeatherProvider _, NWS.Observation.ForecastRootobject forecastResponse)
        {
            return new Location()
            {
                // Use location name from location provider
                name = null,
                latitude = forecastResponse.location.latitude?.TryParseFloat(),
                longitude = forecastResponse.location.longitude?.TryParseFloat(),
                tz_long = null,
            };
        }

        public static Forecast CreateForecast(this NWSWeatherProvider _, NWS.Observation.PeriodsItem forecastItem)
        {
            var forecast = new Forecast();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.NWS);
            var culture = LocaleUtils.GetLocale();

            forecast.date = forecastItem.startTime.DateTime;
            if (float.TryParse(forecastItem.temperature, out float temp))
            {
                if (forecastItem.IsDaytime)
                {
                    forecast.high_f = temp;
                    forecast.high_c = ConversionMethods.FtoC(temp);
                }
                else
                {
                    forecast.low_f = temp;
                    forecast.low_c = ConversionMethods.FtoC(temp);
                }
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                forecast.condition = forecastItem.shortForecast;
            }
            else
            {
                forecast.condition = provider.GetWeatherCondition(forecastItem.icon);
            }
            forecast.icon = provider.GetWeatherIcon(!forecastItem.IsDaytime, forecastItem.icon);

            forecast.extras = new ForecastExtras();
            if (int.TryParse(forecastItem.pop, out int pop))
            {
                forecast.extras.pop = pop;
            }
            else
            {
                forecast.extras.pop = 0;
            }

            return forecast;
        }

        public static Forecast CreateForecast(this NWSWeatherProvider _, NWS.Observation.PeriodsItem forecastItem, NWS.Observation.PeriodsItem nightForecastItem)
        {
            var forecast = new Forecast();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.NWS);
            var culture = LocaleUtils.GetLocale();

            forecast.date = forecastItem.startTime.DateTime;
            if (float.TryParse(forecastItem.temperature, out float hiTemp))
            {
                forecast.high_f = hiTemp;
                forecast.high_c = ConversionMethods.FtoC(hiTemp);
            }
            if (float.TryParse(nightForecastItem.temperature, out float loTemp))
            {
                forecast.low_f = loTemp;
                forecast.low_c = ConversionMethods.FtoC(loTemp);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                forecast.condition = forecastItem.shortForecast;
            }
            else
            {
                forecast.condition = provider.GetWeatherCondition(forecastItem.icon);
            }
            forecast.icon = provider.GetWeatherIcon(false, forecastItem.icon);

            forecast.extras = new ForecastExtras();
            if (int.TryParse(forecastItem.pop, out int pop))
            {
                forecast.extras.pop = pop;
            }
            else
            {
                forecast.extras.pop = 0;
            }

            return forecast;
        }

        public static HourlyForecast CreateHourlyForecast(this NWSWeatherProvider _, NWS.Hourly.PeriodItem forecastItem, bool adjustDate = false)
        {
            var hrf = new HourlyForecast();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.NWS);
            var culture = LocaleUtils.GetLocale();

            hrf.date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(forecastItem.unixTime));
            if (adjustDate)
            {
                hrf.date = hrf.date.AddDays(-1);
            }

            if (float.TryParse(forecastItem.temperature, out float temp))
            {
                hrf.high_f = temp;
                hrf.high_c = ConversionMethods.FtoC(temp);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                hrf.condition = forecastItem.weather;
            }
            else
            {
                hrf.condition = provider.GetWeatherCondition(forecastItem.iconLink);
            }
            hrf.icon = forecastItem.iconLink;

            // Extras
            hrf.extras = new ForecastExtras();

            if (float.TryParse(forecastItem.windSpeed, out float windSpeed) &&
                int.TryParse(forecastItem.windDirection, out int windDir))
            {
                hrf.wind_degrees = windDir;
                hrf.wind_mph = windSpeed;
                hrf.wind_kph = ConversionMethods.MphToKph(windSpeed);

                hrf.extras.wind_degrees = hrf.wind_degrees;
                hrf.extras.wind_mph = hrf.wind_mph;
                hrf.extras.wind_kph = hrf.wind_kph;
            }

            if (float.TryParse(forecastItem.windChill, out float windChill))
            {
                hrf.extras.feelslike_f = windChill;
                hrf.extras.feelslike_c = ConversionMethods.FtoC(windChill);
            }

            if (int.TryParse(forecastItem.cloudAmount, out int cloudiness))
            {
                hrf.extras.cloudiness = cloudiness;
            }

            if (int.TryParse(forecastItem.pop, out int pop))
            {
                hrf.extras.pop = pop;
            }

            if (float.TryParse(forecastItem.windGust, out float windGust))
            {
                hrf.extras.windgust_mph = windGust;
                hrf.extras.windgust_kph = ConversionMethods.MphToKph(windGust);
            }

            return hrf;
        }

        public static TextForecast CreateTextForecast(this NWSWeatherProvider _, NWS.Observation.PeriodsItem forecastItem)
        {
            var textForecast = new TextForecast();

            textForecast.date = forecastItem.startTime;
            textForecast.fcttext = String.Format(CultureInfo.InvariantCulture,
                "{0} - {1}", forecastItem.name, forecastItem.detailedForecast);
            textForecast.fcttext_metric = textForecast.fcttext;

            return textForecast;
        }

        public static TextForecast CreateTextForecast(this NWSWeatherProvider _, NWS.Observation.PeriodsItem forecastItem, NWS.Observation.PeriodsItem ntForecastItem)
        {
            var textForecast = new TextForecast();

            textForecast.date = forecastItem.startTime;
            textForecast.fcttext = String.Format(CultureInfo.InvariantCulture,
                "{0} - {1}\n\n{2} - {3}",
                forecastItem.name, forecastItem.detailedForecast,
                ntForecastItem.name, ntForecastItem.detailedForecast);
            textForecast.fcttext_metric = textForecast.fcttext;

            return textForecast;
        }

        public static Condition CreateCondition(this NWSWeatherProvider _, NWS.Observation.ForecastRootobject forecastResponse)
        {
            var condition = new Condition();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.NWS);
            var culture = LocaleUtils.GetLocale();

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                condition.weather = forecastResponse.currentobservation.Weather;
            }
            else
            {
                condition.weather = provider.GetWeatherCondition(forecastResponse.currentobservation.Weatherimage);
            }
            condition.icon = forecastResponse.currentobservation.Weatherimage;

            if (float.TryParse(forecastResponse.currentobservation.Temp, out float temp))
            {
                condition.temp_f = temp;
                condition.temp_c = ConversionMethods.FtoC(temp);
            }

            if (int.TryParse(forecastResponse.currentobservation.Windd, out int windDir))
            {
                condition.wind_degrees = windDir;
            }

            if (float.TryParse(forecastResponse.currentobservation.Winds, out float windSpeed))
            {
                condition.wind_mph = windSpeed;
                condition.wind_kph = ConversionMethods.MphToKph(windSpeed);
            }

            if (float.TryParse(forecastResponse.currentobservation.Gust, out float windGust))
            {
                condition.windgust_mph = windGust;
                condition.windgust_kph = ConversionMethods.MphToKph(windGust);
            }

            if (float.TryParse(forecastResponse.currentobservation.WindChill, out float windChill))
            {
                condition.feelslike_f = windChill;
                condition.feelslike_c = ConversionMethods.FtoC(windChill);
            }
            else if (condition.temp_f.HasValue && !Equals(condition.temp_f, condition.temp_c) && condition.wind_mph.HasValue)
            {
                if (float.TryParse(forecastResponse.currentobservation.Relh, out float humidity) && humidity >= 0)
                {
                    condition.feelslike_f = WeatherUtils.GetFeelsLikeTemp(condition.temp_f.Value, condition.wind_mph.Value, (int)Math.Round(humidity));
                    condition.feelslike_c = ConversionMethods.FtoC(condition.feelslike_f.Value);
                }
            }

            if (condition.wind_mph.HasValue)
            {
                condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale((int)Math.Round(condition.wind_mph.Value)));
            }

            condition.observation_time = forecastResponse.creationDate;

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this NWSWeatherProvider _, NWS.Observation.ForecastRootobject forecastResponse)
        {
            var atmosphere = new Atmosphere();

            if (int.TryParse(forecastResponse.currentobservation.Relh, out int relh))
            {
                atmosphere.humidity = relh;
            }

            if (float.TryParse(forecastResponse.currentobservation.SLP, out float pressure))
            {
                atmosphere.pressure_in = pressure;
                atmosphere.pressure_mb = ConversionMethods.InHgToMB(pressure);
            }
            atmosphere.pressure_trend = String.Empty;

            if (float.TryParse(forecastResponse.currentobservation.Visibility, out float visibility))
            {
                atmosphere.visibility_mi = visibility;
                atmosphere.visibility_km = ConversionMethods.MiToKm(visibility);
            }

            if (float.TryParse(forecastResponse.currentobservation.Dewp, out float dewp))
            {
                atmosphere.dewpoint_f = dewp;
                atmosphere.dewpoint_c = ConversionMethods.FtoC(dewp);
            }

            return atmosphere;
        }

        public static Precipitation CreatePrecipitation(this NWSWeatherProvider _, NWS.Observation.ForecastRootobject forecastResponse)
        {
            // The rest DNE
            return new Precipitation();
        }
    }
}