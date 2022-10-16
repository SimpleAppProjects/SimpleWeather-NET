using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web;

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
            LocationProvider = RemoteConfig.RemoteConfig.GetLocationProvider(WeatherAPI);
            if (LocationProvider == null)
            {
                LocationProvider = new Bing.BingMapsLocationProvider();
            }
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.Here;
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => false;
        public override bool NeedsExternalAlertData => false;
        public override AuthType AuthType => AuthType.Internal; // or AppID/AppCode

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
        public override async Task<Weather> GetWeather(string location_query, string country_code)
        {
            Weather weather = null;
            WeatherException wEx = null;

            var culture = CultureUtils.UserCulture;

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

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
                this.CheckRateLimit();

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    // Add headers to request
                    var token = await HEREOAuthUtils.GetBearerToken();
                    if (!String.IsNullOrWhiteSpace(token))
                        request.Headers.Add("Authorization", token);
                    else
                        throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError);

                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                    // Connect to webstream
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = await response.Content.ReadAsStreamAsync();

                        // Load weather
                        Rootobject root = await JSONParser.DeserializerAsync<Rootobject>(contentStream);

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
                        weather.weather_alerts = CreateWeatherAlerts(root,
                            weather.location.latitude.Value, weather.location.longitude.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                weather = null;
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown || ex is HttpRequestException || ex is SocketException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
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
        }

        protected override async Task UpdateWeatherData(LocationData location, Weather weather)
        {
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

            var old = weather.astronomy;
            if (DateTime.Equals(old.moonset, DateTime.MinValue) || DateTime.Equals(old.moonrise, DateTime.MinValue))
            {
                var newAstro = await new SMC.SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
                newAstro.sunrise = old.sunrise;
                newAstro.sunset = old.sunset;
                weather.astronomy = newAstro;
            }

            foreach (WeatherData.Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
            }
        }

        public override async Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            ICollection<WeatherAlert> alerts = null;

            var culture = CultureUtils.UserCulture;

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            try
            {
                this.CheckRateLimit();

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

                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                    // Connect to webstream
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = await response.Content.ReadAsStreamAsync();

                        // Load data
                        Rootobject root = await JSONParser.DeserializerAsync<Rootobject>(contentStream);

                        // Add weather alerts if available
                        alerts = CreateWeatherAlerts(root, (float)location.latitude, (float)location.longitude);
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

            if (icon.Contains("night_"))
                isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            if (icon == null)
                return WeatherIcons.NA;

            string neutralIcon;

            if (icon.StartsWith("night_"))
                neutralIcon = icon.ReplaceFirst("night_", "");
            else
                neutralIcon = icon;

            string WeatherIcon = neutralIcon switch
            {
                "sunny" or "clear" => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,

                "mostly_sunny" or "passing_clounds" or "passing_clouds" or "more_sun_than_clouds" or
                "mostly_clear" or "scattered_clouds" or "partly_cloudy" or "decreasing_cloudiness" or
                "clearing_skies" => isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY,

                "a_mixture_of_sun_and_clouds" or "increasing_cloudiness" or "breaks_of_sun_late" or
                "afternoon_clouds" or "morning_clouds" or "partly_sunny" or "more_clouds_than_sun" or
                "broken_clouds" or "mostly_cloudy" => isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY,

                "high_level_clouds" or "high_clouds" => isNight ? WeatherIcons.NIGHT_ALT_CLOUDY_HIGH : WeatherIcons.DAY_CLOUDY_HIGH,

                "rain_early" or "rain" or "rain_late" => WeatherIcons.RAIN,

                "strong_thunderstorms" or "severe_thunderstorms" or "thunderstorms" or "tstorms_early" or
                "isolated_tstorms_late" or "tstorms" or "tstorms_late" => WeatherIcons.THUNDERSTORM,

                "widely_scattered_tstorms" or "isolated_tstorms" or "a_few_tstorms" or
                "scattered_tstorms" or "scattered_tstorms_late" => isNight ? WeatherIcons.NIGHT_ALT_THUNDERSTORM : WeatherIcons.DAY_THUNDERSTORM,

                "thundershowers" => WeatherIcons.STORM_SHOWERS,

                "ice_fog" => WeatherIcons.FOG,

                "scattered_showers" or "a_few_showers" or "light_showers" or "passing_showers" or "rain_showers" or
                "showers" or "numerous_showers" or "showery" or "showers_early" or "showers_late" => isNight ? WeatherIcons.NIGHT_ALT_SHOWERS : WeatherIcons.DAY_SHOWERS,

                "hazy_sunshine" or "haze" or "low_level_haze" => isNight ? WeatherIcons.NIGHT_HAZE : WeatherIcons.DAY_HAZE,

                "smoke" => WeatherIcons.SMOKE,

                "early_fog_followed_by_sunny_skies" or "early_fog" or "light_fog" => isNight ? WeatherIcons.NIGHT_FOG : WeatherIcons.DAY_FOG,

                "fog" or "dense_fog" => WeatherIcons.FOG,

                "cloudy" or "low_clouds" => WeatherIcons.CLOUDY,

                "overcast" => WeatherIcons.OVERCAST,

                "hail" => WeatherIcons.HAIL,

                "sleet" => WeatherIcons.SLEET,

                "light_mixture_of_precip" or "icy_mix" or "mixture_of_precip" or "heavy_mixture_of_precip" or
                "snow_changing_to_rain" or "snow_changing_to_an_icy_mix" or "an_icy_mix_changing_to_snow" or
                "an_icy_mix_changing_to_rain" or "rain_changing_to_snow" or "rain_changing_to_an_icy_mix" or
                "light_icy_mix_early" or "icy_mix_early" or "light_icy_mix_late" or "icy_mix_late" or
                "snow_rain_mix" or "light_freezing_rain" or "freezing_rain" => WeatherIcons.RAIN_MIX,

                "scattered_flurries" or "snow_flurries" or "light_snow_showers" or "snow_showers" or "light_snow" or
                "flurries_early" or "snow_showers_early" or "light_snow_early" or "flurries_late" or "snow_showers_late" or
                "light_snow_late" or "snow" or "moderate_snow" or "snow_early" or "snow_late" => WeatherIcons.SNOW,

                "heavy_rain_early" or "heavy_rain" or "lots_of_rain" or "tons_of_rain" or "heavy_rain_late" or
                "flash_floods" or "flood" => WeatherIcons.RAIN_WIND,

                "drizzle" or "light_rain" or "sprinkles_early" or "light_rain_early" or "sprinkles_late" or
                "light_rain_late" or "sprinkles" => isNight ? WeatherIcons.NIGHT_ALT_SPRINKLE : WeatherIcons.DAY_SPRINKLE,

                "heavy_snow" or "heavy_snow_early" or "heavy_snow_late" or "snowstorm" or "blizzard" => WeatherIcons.SNOW_WIND,

                "tornado" => WeatherIcons.TORNADO,

                "tropical_storm" => WeatherIcons.SHOWERS,

                "hurricane" => WeatherIcons.HURRICANE,

                "sandstorm" => WeatherIcons.SANDSTORM,

                "duststorm" => WeatherIcons.DUST,

                _ => string.Empty,
            };

            // Fallback
            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                if (icon.Contains("overcast"))
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_OVERCAST;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
                else if (icon.Contains("mostly_sunny") || icon.Contains("mostly_clear") || icon.Contains("partly_cloudy")
                        || icon.Contains("passing_clounds") || icon.Contains("more_sun_than_clouds") || icon.Contains("scattered_clouds")
                        || icon.Contains("decreasing_cloudiness") || icon.Contains("clearing_skies")
                        || icon.Contains("low_clouds") || icon.Contains("passing_clouds"))
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_PARTLY_CLOUDY;
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
                else if (icon.Contains("snowstorm") || icon.Contains("blizzard"))
                    WeatherIcon = WeatherIcons.SNOW_WIND;
                else if (icon.Contains("fog"))
                    WeatherIcon = WeatherIcons.FOG;
                else if (icon.Contains("hazy") || icon.Contains("haze"))
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_HAZE;
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
                else if (icon.Contains("flurries") || icon.Contains("snow"))
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
                else if (icon.Contains("cw_no_report_icon"))
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY;
                else
                    // Not Available
                    WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }
    }
}