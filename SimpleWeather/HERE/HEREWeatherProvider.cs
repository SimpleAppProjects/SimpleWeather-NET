using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.Web;
using Windows.Web.Http;

namespace SimpleWeather.HERE
{
    public partial class HEREWeatherProvider : WeatherProviderImpl, IWeatherAlertProvider
    {
        private const String WEATHER_GLOBAL_QUERY_URL = "https://weather.ls.hereapi.com/weather/1.0/report.json?product=alerts&product=forecast_7days_simple" +
            "&product=forecast_hourly&product=forecast_astronomy&product=observation&oneobservation=true&{0}&language={1}&metric=false";
        private const String WEATHER_US_CA_QUERY_URL = "https://weather.ls.hereapi.com/weather/1.0/report.json?product=nws_alerts&product=forecast_7days_simple" +
            "&product=forecast_hourly&product=forecast_astronomy&product=observation&oneobservation=true&{0}&language={1}&metric=false";
        private const String ALERTS_GLOBAL_QUERY_URL = "https://weather.ls.hereapi.com/weather/1.0/report.json?product=alerts&{0}&language={1}&metric=false";
        private const String ALERTS_US_CA_QUERY_URL = "https://weather.ls.hereapi.com/weather/1.0/report.json?product=nws_alerts&{0}&language={1}&metric=false";

        public HEREWeatherProvider() : base()
        {
            LocationProvider = new HERELocationProvider();
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.Here;
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => false;
        public override bool NeedsExternalAlertData => false;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override String GetAPIKey()
        {
            return null;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<Weather> GetWeather(string location_query, string country_code)
        {
            return Task.Run(async () =>
            {
                Weather weather = null;
                WeatherException wEx = null;

                var culture = CultureUtils.UserCulture;

                string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                OAuthRequest authRequest = new OAuthRequest(APIKeys.GetHERECliID(), APIKeys.GetHERECliSecr());
                Uri queryURL;
                if (LocationUtils.IsUSorCanada(country_code))
                {
                    queryURL = new Uri(String.Format(WEATHER_US_CA_QUERY_URL, location_query, locale));
                }
                else
                {
                    queryURL = new Uri(String.Format(WEATHER_GLOBAL_QUERY_URL, location_query, locale));
                }

                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                    {
                        // Add headers to request
                        var token = await HEREOAuthUtils.GetBearerToken();
                        if (!String.IsNullOrWhiteSpace(token))
                            request.Headers.Add("Authorization", token);
                        else
                            throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError);

                        request.Headers.CacheControl.MaxAge = TimeSpan.FromHours(1);

                        // Connect to webstream
                        var webClient = SimpleLibrary.WebClient;
                        using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                        using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                        {
                            response.EnsureSuccessStatusCode();
                            Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                            // Reset exception
                            wEx = null;

                            // Load weather
                            Rootobject root = JSONParser.Deserializer<Rootobject>(contentStream);

                            // Check for errors
                            if (root.Type != null)
                            {
                                switch (root.Type)
                                {
                                    case "Invalid Request":
                                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                                        break;

                                    case "Unauthorized":
                                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                                        break;

                                    default:
                                        break;
                                }
                            }

                            weather = new Weather(root);

                            // Add weather alerts if available
                            if (root.alerts?.alerts?.Length > 0)
                            {
                                weather.weather_alerts = new List<WeatherAlert>(root.alerts.alerts.Length);

                                foreach (Alert result in root.alerts.alerts)
                                {
                                    weather.weather_alerts.Add(new WeatherAlert(result));
                                }
                            }
                            else if (root.nwsAlerts?.watch?.Length > 0 || root.nwsAlerts?.warning?.Length > 0)
                            {
                                int numOfAlerts = (root.nwsAlerts?.watch?.Length ?? 0) + (root.nwsAlerts?.warning?.Length ?? 0);

                                weather.weather_alerts = new HashSet<WeatherAlert>(numOfAlerts);

                                float lat = weather.location.latitude.Value;
                                float lon = weather.location.longitude.Value;

                                if (root.nwsAlerts.watch != null)
                                {
                                    foreach (var watchItem in root.nwsAlerts.watch)
                                    {
                                        // Add watch item if location is within 20km of the center of the alert zone
                                        if (ConversionMethods.CalculateHaversine(lat, lon, double.Parse(watchItem.latitude), double.Parse(watchItem.longitude)) < 20000)
                                        {
                                            weather.weather_alerts.Add(new WeatherAlert(watchItem));
                                        }
                                    }
                                }
                                if (root.nwsAlerts.warning != null)
                                {
                                    foreach (var warningItem in root.nwsAlerts.warning)
                                    {
                                        // Add warning item if location is within 25km of the center of the alert zone
                                        if (ConversionMethods.CalculateHaversine(lat, lon, double.Parse(warningItem.latitude), double.Parse(warningItem.longitude)) < 25000)
                                        {
                                            weather.weather_alerts.Add(new WeatherAlert(warningItem));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    weather = null;
                    if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    }

                    Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting weather data");
                }

                if (wEx == null && (weather == null || !weather.IsValid()))
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
                }
                else if (weather != null)
                {
                    if (SupportsWeatherLocale)
                        weather.locale = locale;

                    weather.query = location_query;
                }

                if (wEx != null)
                    throw wEx;

                return weather;
            });
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await base.GetWeather(location);

            var offset = location.tz_offset;

            if (weather.weather_alerts?.Any() == true)
            {
                foreach (WeatherAlert alert in weather.weather_alerts)
                {
                    if (!alert.Date.Offset.Equals(offset))
                    {
                        alert.Date = new DateTimeOffset(alert.Date.DateTime, offset);
                    }

                    if (!alert.ExpiresDate.Offset.Equals(offset))
                    {
                        alert.ExpiresDate = new DateTimeOffset(alert.ExpiresDate.DateTime, offset);
                    }
                }
            }

            // Update tz for weather properties
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(location.tz_offset);

            foreach (WeatherData.Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
            }

            return weather;
        }

        public override Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            return Task.Run(async () =>
            {
                ICollection<WeatherAlert> alerts = null;

                var culture = CultureUtils.UserCulture;

                string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                try
                {
                    OAuthRequest authRequest = new OAuthRequest(APIKeys.GetHERECliID(), APIKeys.GetHERECliSecr());
                    Uri queryURL;
                    if (LocationUtils.IsUSorCanada(location.country_code))
                    {
                        queryURL = new Uri(String.Format(ALERTS_US_CA_QUERY_URL, location.query, locale));
                    }
                    else
                    {
                        queryURL = new Uri(String.Format(ALERTS_GLOBAL_QUERY_URL, location.query, locale));
                    }

                    using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                    {
                        // Add headers to request
                        var token = await HEREOAuthUtils.GetBearerToken();
                        if (!String.IsNullOrWhiteSpace(token))
                            request.Headers.Add("Authorization", token);
                        else
                            throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError);

                        // Connect to webstream
                        var webClient = SimpleLibrary.WebClient;
                        using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                        using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                        {
                            response.EnsureSuccessStatusCode();
                            Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                            // Load data
                            Rootobject root = JSONParser.Deserializer<Rootobject>(contentStream);

                            // Add weather alerts if available
                            if (root.alerts?.alerts?.Length > 0)
                            {
                                alerts = new List<WeatherAlert>(root.alerts.alerts.Length);

                                foreach (Alert result in root.alerts.alerts)
                                {
                                    alerts.Add(new WeatherAlert(result));
                                }
                            }
                            else if (root.nwsAlerts?.watch?.Length > 0 || root.nwsAlerts?.warning?.Length > 0)
                            {
                                int numOfAlerts = (root.nwsAlerts?.watch?.Length ?? 0) + (root.nwsAlerts?.warning?.Length ?? 0);

                                alerts = new HashSet<WeatherAlert>(numOfAlerts);

                                float lat = (float)location.latitude;
                                float lon = (float)location.longitude;

                                if (root.nwsAlerts.watch != null)
                                {
                                    foreach (var watchItem in root.nwsAlerts.watch)
                                    {
                                        // Add watch item if location is within 20km of the center of the alert zone
                                        if (ConversionMethods.CalculateHaversine(lat, lon, double.Parse(watchItem.latitude), double.Parse(watchItem.longitude)) < 20000)
                                        {
                                            alerts.Add(new WeatherAlert(watchItem));
                                        }
                                    }
                                }
                                if (root.nwsAlerts.warning != null)
                                {
                                    foreach (var warningItem in root.nwsAlerts.warning)
                                    {
                                        // Add warning item if location is within 25km of the center of the alert zone
                                        if (ConversionMethods.CalculateHaversine(lat, lon, double.Parse(warningItem.latitude), double.Parse(warningItem.longitude)) < 25000)
                                        {
                                            alerts.Add(new WeatherAlert(warningItem));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    alerts = new List<WeatherAlert>();
                    Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting weather alert data");
                }

                if (alerts == null)
                    alerts = new List<WeatherAlert>();

                return alerts;
            });
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "latitude={0:0.####}&longitude={1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "latitude={0:0.####}&longitude={1:0.####}", location.latitude, location.longitude);
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            return name;
        }

        public override string GetWeatherIcon(string icon)
        {
            bool isNight = false;

            if (icon == null)
                return WeatherIcons.NA;

            if (icon.StartsWith("N_", StringComparison.Ordinal) || icon.Contains("night_"))
                isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            if (icon == null)
                return WeatherIcons.NA;

            if (icon.Contains("mostly_sunny") || icon.Contains("mostly_clear") || icon.Contains("partly_cloudy")
                    || icon.Contains("passing_clounds") || icon.Contains("more_sun_than_clouds") || icon.Contains("scattered_clouds")
                    || icon.Contains("decreasing_cloudiness") || icon.Contains("clearing_skies") || icon.Contains("overcast")
                    || icon.Contains("low_clouds") || icon.Contains("passing_clouds"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
            else if (icon.Contains("cloudy") || icon.Contains("a_mixture_of_sun_and_clouds") || icon.Contains("increasing_cloudiness")
                     || icon.Contains("breaks_of_sun_late") || icon.Contains("afternoon_clouds") || icon.Contains("morning_clouds")
                     || icon.Contains("partly_sunny") || icon.Contains("more_clouds_than_sun") || icon.Contains("broken_clouds"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY;
            else if (icon.Contains("high_level_clouds") || icon.Contains("high_clouds"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY_HIGH;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY_HIGH;
            else if (icon.Contains("flurries") || icon.Contains("snowstorm") || icon.Contains("blizzard"))
                WeatherIcon = WeatherIcons.SNOW_WIND;
            else if (icon.Contains("fog"))
                WeatherIcon = WeatherIcons.FOG;
            else if (icon.Contains("hazy") || icon.Contains("haze"))
                if (isNight)
                    WeatherIcon = WeatherIcons.WINDY;
                else
                    WeatherIcon = WeatherIcons.DAY_HAZE;
            else if (icon.Contains("sleet") || icon.Contains("snow_changing_to_an_icy_mix") || icon.Contains("an_icy_mix_changing_to_snow")
                    || icon.Contains("rain_changing_to_snow"))
                WeatherIcon = WeatherIcons.SLEET;
            else if (icon.Contains("mixture_of_precip") || icon.Contains("icy_mix") || icon.Contains("snow_changing_to_rain")
                    || icon.Contains("snow_rain_mix") || icon.Contains("freezing_rain"))
                WeatherIcon = WeatherIcons.RAIN_MIX;
            else if (icon.Contains("hail"))
                WeatherIcon = WeatherIcons.HAIL;
            else if (icon.Contains("snow"))
                WeatherIcon = WeatherIcons.SNOW;
            else if (icon.Contains("sprinkles") || icon.Contains("drizzle"))
                WeatherIcon = WeatherIcons.SPRINKLE;
            else if (icon.Contains("light_rain") || icon.Contains("showers"))
                WeatherIcon = WeatherIcons.SHOWERS;
            else if (icon.Contains("rain") || icon.Contains("flood"))
                WeatherIcon = WeatherIcons.RAIN;
            else if (icon.Contains("tstorms") || icon.Contains("thunderstorms") || icon.Contains("thundershowers")
                    || icon.Contains("tropical_storm"))
                WeatherIcon = WeatherIcons.THUNDERSTORM;
            else if (icon.Contains("smoke"))
                WeatherIcon = WeatherIcons.SMOKE;
            else if (icon.Contains("tornado"))
                WeatherIcon = WeatherIcons.TORNADO;
            else if (icon.Contains("hurricane"))
                WeatherIcon = WeatherIcons.HURRICANE;
            else if (icon.Contains("sandstorm"))
                WeatherIcon = WeatherIcons.SANDSTORM;
            else if (icon.Contains("duststorm"))
                WeatherIcon = WeatherIcons.DUST;
            else if (icon.Contains("clear") || icon.Contains("sunny"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;
            else if (icon.Contains("cw_no_report_icon") || icon.StartsWith("night_", StringComparison.Ordinal))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }
    }
}