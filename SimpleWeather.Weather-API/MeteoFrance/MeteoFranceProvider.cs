using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.SMC;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.Weather_API.WeatherData;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.MeteoFrance
{
    public partial class MeteoFranceProvider : WeatherProviderImpl
    {
        private const String BASE_URL = "https://webservice.meteofrance.com/";
        private const String CURRENT_QUERY_URL = BASE_URL + "observation/gridded?{0}&lang={1}&token={2}";
        private const String FORECAST_QUERY_URL = BASE_URL + "forecast?{0}&lang={1}&token={2}";
        private const String ALERTS_QUERY_URL = BASE_URL + "warning/full?domain={0}&token={1}";

        public MeteoFranceProvider() : base()
        {
            LocationProvider = this.RunCatching(() =>
            {
                return WeatherModule.Instance.LocationProviderFactory.GetLocationProvider(
                    RemoteConfigService.GetLocationProvider(WeatherAPI));
            }).GetOrElse<IWeatherLocationProvider, IWeatherLocationProvider>((t) =>
            {
                return new Bing.BingMapsLocationProvider();
            });
        }

        public override string WeatherAPI => WAPI.MeteoFrance;
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => false;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;
        public override int HourlyForecastInterval => 1;
        public override AuthType AuthType => AuthType.ApiKey;

        public override bool IsRegionSupported(string countryCode)
        {
            return LocationUtils.IsFrance(countryCode);
        }

        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override string GetAPIKey()
        {
            return APIKeys.GetMeteoFranceKey();
        }

        public override long GetRetryTime() => 60000;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        protected override async Task<SimpleWeather.WeatherData.Weather> GetWeatherData(SimpleWeather.LocationData.LocationData location)
        {
            SimpleWeather.WeatherData.Weather weather = null;
            WeatherException wEx = null;

            // MeteoFrance only supports locations in France
            if (!LocationUtils.IsFrance(location.country_code))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound, new Exception($"Unsupported country code: provider ({WeatherAPI}), country ({location.country_code})"));
            }

            var culture = LocaleUtils.GetLocale();

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);
            string query = UpdateLocationQuery(location);

            string key = GetAPIKey();

            if (String.IsNullOrWhiteSpace(key))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
            }

            try
            {
                this.CheckRateLimit();

                Uri currentURL = new Uri(string.Format(CURRENT_QUERY_URL, query, locale, key));
                Uri forecastURL = new Uri(string.Format(FORECAST_QUERY_URL, query, locale, key));

                using (var currentRequest = new HttpRequestMessage(HttpMethod.Get, currentURL))
                using (var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastURL))
                {
                    currentRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(15));
                    forecastRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using (var ctsC = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var currentResponse = await webClient.SendAsync(currentRequest, ctsC.Token))
                    using (var ctsF = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var forecastResponse = await webClient.SendAsync(forecastRequest, ctsF.Token))
                    {
                        await this.CheckForErrors(currentResponse);
                        currentResponse.EnsureSuccessStatusCode();

                        await this.CheckForErrors(forecastResponse);
                        forecastResponse.EnsureSuccessStatusCode();

                        Stream currentStream = await currentResponse.Content.ReadAsStreamAsync();
                        Stream forecastStream = await forecastResponse.Content.ReadAsStreamAsync();

                        // Load weather
                        var currRoot = await JSONParser.DeserializerAsync<CurrentsRootobject>(currentStream);
                        var foreRoot = await JSONParser.DeserializerAsync<ForecastRootobject>(forecastStream);
                        AlertsRootobject alertsRoot = null;

                        if (foreRoot.position?.dept != null)
                        {
                            Uri alertsURL = new Uri(string.Format(ALERTS_QUERY_URL, foreRoot.position.dept, key));

                            using (var alertsRequest = new HttpRequestMessage(HttpMethod.Get, alertsURL))
                            {
                                alertsRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                                using (var ctsA = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                                using (var alertsResponse = await webClient.SendAsync(alertsRequest, ctsA.Token))
                                {
                                    var alertsStream = await alertsResponse.Content.ReadAsStreamAsync();
                                    alertsRoot = await JSONParser.DeserializerAsync<AlertsRootobject>(alertsStream);
                                }
                            }
                        }

                        weather = this.CreateWeatherData(currRoot, foreRoot, alertsRoot);
                    }
                }
            }
            catch (Exception ex)
            {
                weather = null;

                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "MeteoFranceProvider: error getting weather data");
            }

            if (weather == null || !weather.IsValid())
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null)
            {
                if (SupportsWeatherLocale)
                    weather.locale = locale;

                weather.query = query;
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        protected override async Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location, SimpleWeather.WeatherData.Weather weather)
        {
            // MeteoFrance reports datetime in UTC; add location tz_offset
            var offset = location.tz_offset;
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            // Calculate astronomy
            weather.astronomy = await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);

            // Update icons
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            weather.condition.icon = GetWeatherIcon(now < sunrise || now > sunset, weather.condition.icon);

            foreach (var forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
            }

            foreach (var hr_forecast in weather.hr_forecast)
            {
                var hrf_date = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrf_date;
                var hrf_localTime = hrf_date.TimeOfDay;
                hr_forecast.icon = GetWeatherIcon(hrf_localTime < sunrise || hrf_localTime > sunset, hr_forecast.icon);
            }

            if (weather.weather_alerts?.Any() == true)
            {
                foreach (var alert in weather.weather_alerts)
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
        }

        public override string UpdateLocationQuery(SimpleWeather.WeatherData.Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            if (iso == "fr")
                return iso;

            return "en";
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

        // Icon reference
        // https://meteofrance.com/modules/custom/mf_tools_common_theme_public/svg/weather/pxx.svg
        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            if (icon == null)
                return WeatherIcons.NA;

            if (icon.StartsWith("p10bis"))
            {
                // Sprinkle
                WeatherIcon =
                    isNight ? WeatherIcons.NIGHT_ALT_SPRINKLE : WeatherIcons.DAY_SPRINKLE;
            }
            else if (icon.StartsWith("p10"))
            {
                // Light rain?
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SHOWERS : WeatherIcons.DAY_SHOWERS;
            }
            else if (icon.StartsWith("p11"))
            {
                // Light rain?
                WeatherIcon = WeatherIcons.SHOWERS;
            }
            else if (icon.StartsWith("p12"))
            {
                // Rain
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN : WeatherIcons.DAY_RAIN;
            }
            else if (icon.StartsWith("p13") || icon.StartsWith("p14"))
            {
                // Rain
                WeatherIcon = WeatherIcons.RAIN;
            }
            else if (icon.StartsWith("p15"))
            {
                // Heavy Rain
                WeatherIcon = WeatherIcons.RAIN_WIND;
            }
            else if (icon.StartsWith("p16") || icon.StartsWith("p25"))
            {
                // Storm Showers
                WeatherIcon = WeatherIcons.STORM_SHOWERS;
            }
            else if (icon.StartsWith("p17") || icon.StartsWith("p21"))
            {
                // Flurries?
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW;
            }
            else if (icon.StartsWith("p18") || icon.StartsWith("p22"))
            {
                // Flurries?
                WeatherIcon = WeatherIcons.SNOW;
            }
            else if (icon.StartsWith("p19"))
            {
                // Sleet / Rain mix
                WeatherIcon =
                    isNight ? WeatherIcons.NIGHT_ALT_RAIN_MIX : WeatherIcons.DAY_RAIN_MIX;
            }
            else if (icon.StartsWith("p20"))
            {
                // Sleet / Rain mix
                WeatherIcon = WeatherIcons.RAIN_MIX;
            }
            else if (icon.StartsWith("p21") || icon.StartsWith("p22") || icon.StartsWith("p23"))
            {
                // Snow
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW;
            }
            else if (icon.StartsWith("p23"))
            {
                // Heavy Snow
                WeatherIcon = WeatherIcons.SNOW_WIND;
            }
            else if (icon.StartsWith("p24") || icon.StartsWith("p25"))
            {
                // Thundershowers / T-storms
                WeatherIcon =
                    isNight ? WeatherIcons.NIGHT_ALT_STORM_SHOWERS : WeatherIcons.DAY_STORM_SHOWERS;
            }
            else if (icon.StartsWith("p26") || icon.StartsWith("p28"))
            {
                // Thunder
                WeatherIcon =
                    isNight ? WeatherIcons.NIGHT_ALT_LIGHTNING : WeatherIcons.DAY_LIGHTNING;
            }
            else if (icon.StartsWith("p27") || icon.StartsWith("p29"))
            {
                // Thunder
                WeatherIcon = WeatherIcons.LIGHTNING;
            }
            else if (icon.StartsWith("p30"))
            {
                // Snow/Thunder
                WeatherIcon =
                    isNight ? WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM : WeatherIcons.DAY_SNOW_THUNDERSTORM;
            }
            else if (icon.StartsWith("p32") || icon.StartsWith("p33"))
            {
                // Tornado
                WeatherIcon = WeatherIcons.TORNADO;
            }
            else if (icon.StartsWith("p34"))
            {
                // Hurricane
                WeatherIcon = WeatherIcons.HURRICANE;
            }
            else if (icon == "p1" || icon == "p1j" || icon == "p1n")
            {
                // Clear
                WeatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY;
            }
            else if (icon.StartsWith("p1bis") || icon.StartsWith("p2"))
            {
                // Partly Cloudy
                WeatherIcon =
                    isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY;
            }
            else if (icon.StartsWith("p2"))
            {
                // Mostly Cloudy
                WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY;
            }
            else if (icon.StartsWith("p3"))
            {
                // Mostly Cloudy
                WeatherIcon = WeatherIcons.CLOUDY;
            }
            else if (icon.StartsWith("p4"))
            {
                // Haze
                WeatherIcon = isNight ? WeatherIcons.NIGHT_HAZE : WeatherIcons.DAY_HAZE;
            }
            else if (icon.StartsWith("p5"))
            {
                // Fog
                WeatherIcon = isNight ? WeatherIcons.NIGHT_FOG : WeatherIcons.DAY_FOG;
            }
            else if (icon.StartsWith("p6") || icon.StartsWith("p7") || icon.StartsWith("p8"))
            {
                // Fog / Heavy Fog
                WeatherIcon = WeatherIcons.FOG;
            }
            else if (icon.StartsWith("p9"))
            {
                // Sprinkle
                WeatherIcon = WeatherIcons.SPRINKLE;
            }
            else
            {
                this.LogMissingIcon(icon);
                WeatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY;
            }

            if (string.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(SimpleWeather.WeatherData.Weather weather)
        {
            bool isNight = base.IsNight(weather);

            switch (weather.condition.icon)
            {
                // The following cases can be present at any time of day
                case WeatherIcons.SMOKE:
                case WeatherIcons.WINDY:
                case WeatherIcons.DUST:
                case WeatherIcons.TORNADO:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.SNOWFLAKE_COLD:
                case WeatherIcons.HAIL:
                case WeatherIcons.STRONG_WIND:
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