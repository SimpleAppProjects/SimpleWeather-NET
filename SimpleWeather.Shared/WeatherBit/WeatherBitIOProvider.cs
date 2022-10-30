using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web;

namespace SimpleWeather.WeatherBit
{
    public partial class WeatherBitIOProvider : WeatherProviderImpl
    {
        private const String BASE_URL = "https://api.weatherbit.io/v2.0/";
        private const String KEYCHECK_QUERY_URL = BASE_URL + "current?key={0}";
        private const String CURRENT_QUERY_URL = BASE_URL + "current?{0}&lang={1}&units=M&include=minutely,alerts&key={2}";
        private const String FORECAST_QUERY_URL = BASE_URL + "forecast/daily?{0}&lang={1}&units=M&key={2}";
        private const String ALERTS_QUERY_URL = BASE_URL + "alerts?{0}&key={1}";

        public WeatherBitIOProvider() : base()
        {
            LocationProvider = RemoteConfig.RemoteConfig.GetLocationProvider(WeatherAPI);
            if (LocationProvider == null)
            {
                LocationProvider = new Bing.BingMapsLocationProvider();
            }
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.WeatherBitIo;
        public override bool SupportsWeatherLocale => true;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;
        public override bool KeyRequired => true;
        public override int HourlyForecastInterval => 0;
        public override AuthType AuthType => AuthType.ApiKey;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<bool> IsKeyValid(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);

            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new(String.Format(KEYCHECK_QUERY_URL, key));

                // Connect to webstream
                var webClient = SharedModule.Instance.WebClient;

                using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                using var response = await webClient.GetAsync(queryURL, cts.Token);
                // Check for errors
                await this.ThrowIfRateLimited(response);

                switch (response.StatusCode)
                {
                    // 400 (OK since this isn't a valid request)
                    case HttpStatusCode.BadRequest:
                        isValid = true;
                        break;
                    // 401 (Unauthorized - Key is invalid)
                    // 403 (Forbidden - Key is invalid)
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                        isValid = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }
            }

            if (wEx != null)
            {
                throw wEx;
            }

            return isValid;
        }

        public override string GetAPIKey()
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
            var key = Settings.UsePersonalKey ? Settings.APIKeys[WeatherData.WeatherAPI.WeatherBitIo] : GetAPIKey();

            if (String.IsNullOrWhiteSpace(key))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
            }

            try
            {
                this.CheckRateLimit();

                Uri currentURL = new(string.Format(CURRENT_QUERY_URL, location_query, locale, key));
                Uri forecastURL = new(string.Format(FORECAST_QUERY_URL, location_query, locale, key));

                using var currentRequest = new HttpRequestMessage(HttpMethod.Get, currentURL);
                using var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastURL);
                currentRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(20));
                forecastRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                var webClient = SharedModule.Instance.WebClient;

                // Get response
                using var ctsC = new CancellationTokenSource(Settings.READ_TIMEOUT);
                using var currentResponse = await webClient.SendAsync(currentRequest, ctsC.Token);
                await this.CheckForErrors(currentResponse);
                currentResponse.EnsureSuccessStatusCode();

                using var ctsF = new CancellationTokenSource(Settings.READ_TIMEOUT);
                using var forecastResponse = await webClient.SendAsync(forecastRequest, ctsF.Token);
                await this.CheckForErrors(forecastResponse);
                forecastResponse.EnsureSuccessStatusCode();

                Stream currentStream = await currentResponse.Content.ReadAsStreamAsync();
                Stream forecastStream = await forecastResponse.Content.ReadAsStreamAsync();

                // Load weather
                CurrentRootobject currRoot = await JSONParser.DeserializerAsync<CurrentRootobject>(currentStream);
                ForecastRootobject foreRoot = await JSONParser.DeserializerAsync<ForecastRootobject>(forecastStream);

                weather = new Weather(currRoot, foreRoot);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherBitIOProvider: error getting weather data");
            }

            if (weather == null || !weather.IsValid())
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

        public override async Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            ICollection<WeatherAlert> alerts = null;

            var culture = CultureUtils.UserCulture;

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = Settings.UsePersonalKey ? Settings.APIKeys[WeatherData.WeatherAPI.WeatherBitIo] : GetAPIKey();

            try
            {
                this.CheckRateLimit();

                Uri alertsURL = new(string.Format(ALERTS_QUERY_URL, UpdateLocationQuery(location), key));

                using var request = new HttpRequestMessage(HttpMethod.Get, alertsURL);
                request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                var webClient = SharedModule.Instance.WebClient;

                // Get response
                using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                using var response = await webClient.SendAsync(request, cts.Token);
                await this.CheckForErrors(response);
                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();

                // Load weather
                var root = await JSONParser.DeserializerAsync<AlertRootobject>(stream);

                if (root.alerts?.Length > 0)
                {
                    alerts = new HashSet<WeatherAlert>(root.alerts.Length);

                    var tzOffset = DateTimeUtils.TzidToOffset(root.timezone);

                    foreach (var alert in root.alerts)
                    {
                        alerts.Add(new WeatherAlert(alert).Apply(it =>
                        {
                            it.Date = it.Date.ToOffset(tzOffset);
                            it.ExpiresDate = it.ExpiresDate.ToOffset(tzOffset);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherBitIOProvider: error getting weather alert data");
            }

            if (alerts == null)
            {
                alerts = new List<WeatherAlert>(0);
            }

            return alerts;
        }

        protected override Task UpdateWeatherData(LocationData location, Weather weather)
        {
            return Task.CompletedTask;
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            string code = iso switch
            {
                "ar" or // Arabic
                "az" or // Azerbaijani
                "be" or // Belarusian
                "bg" or // Bulgarian
                "bs" or // Bosnian
                "ca" or // Catalan
                "da" or // Danish
                "de" or // German
                "fi" or // Finnish
                "fr" or // French
                "el" or // Greek
                "es" or // Spanish
                "et" or // Estonian
                "ja" or // Japanese
                "hr" or // Croatian
                "hu" or // Hungarian
                "id" or // Indonesian
                "it" or // Italian
                "is" or // Icelandic
                "kw" or // Cornish
                "lt" or // Lithuanian
                "nb" or // Norwegian Bokmål
                "nl" or // Dutch
                "pl" or // Polish
                "pt" or // Portuguese
                "ro" or // Romanian
                "ru" or // Russian
                "sk" or // Slovak
                "sl" or // Slovenian
                "sr" or // Serbian
                "sv" or // Swedish
                "tr" or // Turkish
                "uk" // Ukrainian
                => iso,
                // Chinese
                "zh" => name switch
                {
                    // Chinese - Traditional
                    "zh-Hant" or "zh-HK" or "zh-MO" or "zh-TW" => "zh_tw",
                    // Chinese - Simplified
                    _ => "zh",
                },
                "cs" => "cz", // Czech
                "he" => "iw", // Ukrainian
                _ => "en",// Default is English
            };
            return code;
        }

        public override string GetWeatherIcon(string icon)
        {
            bool isNight = false;

            if (icon == null)
                return WeatherIcons.NA;

            if (icon.EndsWith("n"))
                isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            if (icon == null)
                return WeatherIcons.NA;

            string WeatherIcon = string.Empty;

            if (icon.StartsWith("t01") || icon.StartsWith("t02") || icon.StartsWith("t03"))
            {
                // t01: 200	Thunderstorm with light rain
                // t02: 201	Thunderstorm with rain
                // t03: 202	Thunderstorm with heavy rain
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_STORM_SHOWERS : WeatherIcons.DAY_STORM_SHOWERS;
            }
            else if (icon.StartsWith("t04") || icon.StartsWith("t05"))
            {
                // t04: 230	Thunderstorm with light drizzle | 231 Thunderstorm with drizzle | 232 Thunderstorm with heavy drizzle
                // t05: 233	Thunderstorm with Hail
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_THUNDERSTORM : WeatherIcons.DAY_THUNDERSTORM;
            }
            else if (icon.StartsWith("d0"))
            {
                // d01: 300	Light Drizzle
                // d02: 301	Drizzle
                // d03: 302	Heavy Drizzle
                WeatherIcon = WeatherIcons.SPRINKLE;
            }
            else if (icon.StartsWith("r01") || icon.StartsWith("r02") || icon.StartsWith("u00"))
            {
                // r01: 500	Light Rain
                // r02: 501	Moderate Rain
                // u00: 900	Unknown Precipitation
                WeatherIcon = WeatherIcons.RAIN;
            }
            else if (icon.StartsWith("r03"))
            {
                // r03: 502	Heavy Rain
                WeatherIcon = WeatherIcons.RAIN_WIND;
            }
            else if (icon.StartsWith("f01"))
            {
                // f01: 511	Freezing rain
                WeatherIcon = WeatherIcons.RAIN_MIX;
            }
            else if (icon.StartsWith("r04") || icon.StartsWith("r06"))
            {
                // r04: 520	Light shower rain
                // r06: 522	Heavy shower rain
                WeatherIcon = WeatherIcons.SHOWERS;
            }
            else if (icon.StartsWith("r05"))
            {
                // r05: 521	Shower rain
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SHOWERS : WeatherIcons.DAY_SHOWERS;
            }
            else if (icon.StartsWith("s01"))
            {
                // s01: 600	Light snow | 621 Snow shower
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW;
            }
            else if (icon.StartsWith("s02") || icon.StartsWith("s03") || icon.StartsWith("s06"))
            {
                // s02: 601	Snow | 622 Heavy snow shower
                // s03: 602	Heavy Snow
                // s06: 623	Flurries
                WeatherIcon = WeatherIcons.SNOW;
            }
            else if (icon.StartsWith("s04"))
            {
                // s04: 610	Mix snow/rain
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN_MIX : WeatherIcons.DAY_RAIN_MIX;
            }
            else if (icon.StartsWith("s05"))
            {
                // s05: 611	Sleet | 612	Heavy sleet
                WeatherIcon = WeatherIcons.SLEET;
            }
            else if (icon.StartsWith("a01") || icon.StartsWith("a05") || icon.StartsWith("a06"))
            {
                // a01: 700	Mist
                // a05: 741	Fog
                // a06: 751 Freezing Fog
                WeatherIcon = isNight ? WeatherIcons.NIGHT_FOG : WeatherIcons.DAY_FOG;
            }
            else if (icon.StartsWith("a02"))
            {
                // a02: 711	Smoke
                WeatherIcon = WeatherIcons.SMOKE;
            }
            else if (icon.StartsWith("a03"))
            {
                // a03: 721	Haze
                WeatherIcon = isNight ? WeatherIcons.NIGHT_HAZE : WeatherIcons.DAY_HAZE;
            }
            else if (icon.StartsWith("a04"))
            {
                // a04: 731	Sand/dust
                WeatherIcon = WeatherIcons.DUST;
            }
            else if (icon.StartsWith("c01"))
            {
                // c01: 800	Clear sky
                WeatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY;
            }
            else if (icon.StartsWith("c02"))
            {
                // c02: 801	Few clouds
                // c03: 802	Scattered clouds
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY;
            }
            else if (icon.StartsWith("c03"))
            {
                // c03: 803	Broken clouds
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY;
            }
            else if (icon.StartsWith("c04"))
            {
                // c04: 804	Overcast clouds
                WeatherIcon = WeatherIcons.OVERCAST;
            }
            else
            {
                this.LogMissingIcon(icon);
                WeatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY;
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(WeatherData.Weather weather)
        {
            bool isNight = base.IsNight(weather);

            switch (weather.condition.icon)
            {
                // The following cases can be present at any time of day
                case WeatherIcons.SMOKE:
                case WeatherIcons.DUST:
                    if (!isNight)
                    {
                        // Fallback to sunset/rise time just in case
                        NodaTime.LocalTime sunrise;
                        NodaTime.LocalTime sunset;
                        if (weather.astronomy != null)
                        {
                            sunrise = NodaTime.LocalTime.FromTicksSinceMidnight(weather.astronomy.sunrise.TimeOfDay.Ticks);
                            sunset = NodaTime.LocalTime.FromTicksSinceMidnight(weather.astronomy.sunset.TimeOfDay.Ticks);
                        }
                        else
                        {
                            sunrise = NodaTime.LocalTime.FromHourMinuteSecondTick(6, 0, 0, 0);
                            sunset = NodaTime.LocalTime.FromHourMinuteSecondTick(18, 0, 0, 0);
                        }

                        NodaTime.DateTimeZone tz = null;

                        if (weather.location.tz_long != null)
                        {
                            tz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
                        }

                        if (tz == null)
                            tz = NodaTime.DateTimeZone.ForOffset(NodaTime.Offset.FromTimeSpan(weather.location.tz_offset));

                        var now = NodaTime.SystemClock.Instance.GetCurrentInstant()
                                    .InZone(tz).TimeOfDay;

                        // Determine whether its night using sunset/rise times
                        if (now < sunrise || now > sunset)
                            isNight = true;
                    }
                    break;

                default:
                    break;
            }

            return isNight;
        }
    }
}