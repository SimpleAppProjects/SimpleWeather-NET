using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.NWS;
using SimpleWeather.Weather_API.SMC;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using ResConditions = SimpleWeather.Resources.Strings.WeatherConditions;
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.ECCC
{
    public partial class ECCCWeatherProvider : WeatherProviderImpl
    {
        private const String BASE_URL = "https://app.weather.gc.ca/v2/{0}/Location/{1}";

        public ECCCWeatherProvider() : base()
        {
            LocationProvider = this.RunCatching(() =>
            {
                return WeatherModule.Instance.LocationProviderFactory.GetLocationProvider(
                    RemoteConfigService.GetLocationProvider(WeatherAPI));
            }).GetOrElse<IWeatherLocationProvider, IWeatherLocationProvider>((t) =>
            {
                return new WeatherApi.WeatherApiLocationProvider();
            });
        }

        public override string WeatherAPI => WAPI.ECCC;
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => false;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;
        public override int HourlyForecastInterval => 1;

        public override bool IsRegionSupported(SimpleWeather.LocationData.LocationData location)
        {
            return LocationUtils.IsCanada(location);
        }

        public override bool IsRegionSupported(LocationQuery location)
        {
            return LocationUtils.IsCanada(location);
        }

        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override string GetAPIKey()
        {
            return null;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        protected override async Task<Weather> GetWeatherData(SimpleWeather.LocationData.LocationData location)
        {
            Weather weather = null;
            WeatherException wEx = null;

            // ECCC
            if (!LocationUtils.IsCanada(location))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound,
                    new Exception(
                        $"Unsupported country code: provider ({WeatherAPI}), country ({location.country_code})"));
            }

            var culture = LocaleUtils.GetLocale();

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);
            string query = await UpdateLocationQuery(location);

            try
            {
                this.CheckRateLimit();

                var forecastURL = new Uri(string.Format(BASE_URL, locale, query));

                using var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastURL)
                    .CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                // Get response
                var webClient = SharedModule.Instance.WebClient;
                using var ctsF = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var forecastResponse = await webClient.SendAsync(forecastRequest, ctsF.Token);

                await this.CheckForErrors(forecastResponse);
                forecastResponse.EnsureSuccessStatusCode();

                Stream forecastStream = await forecastResponse.Content.ReadAsStreamAsync();

                // Load weather
                var root = await JSONParser.DeserializerAsync<LocationsItem[]>(forecastStream);
                var foreRoot = root?.First() ?? throw new ArgumentNullException(nameof(root));

                weather = this.CreateWeatherData(foreRoot);
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
                else
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather, ex);
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "ECCCWeatherProvider: error getting weather data");
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

        protected override async Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location,
            Weather weather)
        {
            var offset = location.tz_offset;
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            // Calculate astronomy
            Astronomy newAstro;

            try
            {
                newAstro = await new SunMoonCalcProvider().GetAstronomyData(location,
                    weather.condition.observation_time);
            }
            catch (WeatherException)
            {
                newAstro = await new SolCalcAstroProvider().GetAstronomyData(location,
                    weather.condition.observation_time);
            }

            if (weather.astronomy != null)
            {
                weather.astronomy.moonrise = newAstro.moonrise;
                weather.astronomy.moonset = newAstro.moonset;
            }
            else
            {
                weather.astronomy = newAstro;
            }

            // Update icons
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            if (string.IsNullOrWhiteSpace(weather.condition.weather))
            {
                weather.condition.weather = GetWeatherCondition(weather.condition.icon);
            }

            weather.condition.icon = GetWeatherIcon( /*now < sunrise || now > sunset, */weather.condition.icon);

            foreach (var forecast in weather.forecast)
            {
                forecast.icon?.Let(it =>
                {
                    forecast.icon = GetWeatherIcon(it);
                    forecast.condition = GetWeatherCondition(it);
                });
            }

            foreach (var hr_forecast in weather.hr_forecast)
            {
                var hrf_date = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrf_date;

                var hrf_localTime = hrf_date.TimeOfDay;

                hr_forecast.icon?.Let(it =>
                {
                    hr_forecast.icon = GetWeatherIcon(hrf_localTime < sunrise || hrf_localTime > sunset, it);
                    hr_forecast.condition = GetWeatherCondition(it);
                });
            }

            if (weather.weather_alerts?.Any() == true)
            {
                foreach (var alert in weather.weather_alerts)
                {
                    alert.Date = alert.Date.ToOffset(offset);
                    alert.ExpiresDate = alert.ExpiresDate.ToOffset(offset);
                }
            }
        }

        public override Task<string> UpdateLocationQuery(Weather weather)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}",
                weather.location.latitude, weather.location.longitude));
        }

        public override Task<string> UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}",
                location.latitude, location.longitude));
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

            var iconCode = icon.TryParseInt();

            if (iconCode != null)
            {
                isNight = 30 <= iconCode && iconCode <= 39;
            }

            return GetWeatherIcon(isNight, icon);
        }

        // https://eccc-msc.github.io/open-data/msc-data/citypage-weather/readme_citypageweather-datamart_en/#icons-of-the-xml-product
        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string weatherIcon = string.Empty;

            var iconCode = icon?.TryParseInt();

            if (iconCode == null) return WeatherIcons.NA;

            weatherIcon = iconCode switch
            {
                0 or 1 or 30 or 31 => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,

                2 or 32 => isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY,

                3 or 4 or 5 or 22 or 33 or 34 or 35 =>
                    isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY,

                6 or 36 => isNight ? WeatherIcons.NIGHT_ALT_SPRINKLE : WeatherIcons.DAY_SPRINKLE,

                7 or 37 => isNight ? WeatherIcons.NIGHT_ALT_RAIN_MIX : WeatherIcons.DAY_RAIN_MIX,

                8 or 38 => isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW,

                9 or 39 => isNight ? WeatherIcons.NIGHT_ALT_THUNDERSTORM : WeatherIcons.DAY_THUNDERSTORM,

                10 => isNight ? WeatherIcons.NIGHT_OVERCAST : WeatherIcons.DAY_SUNNY_OVERCAST,

                11 => WeatherIcons.RAIN,
                12 => WeatherIcons.SHOWERS,
                13 => WeatherIcons.RAIN_WIND,
                14 or 15 or 28 => WeatherIcons.RAIN_MIX,
                16 => WeatherIcons.SNOW,
                17 or 18 or 25 or 40 => WeatherIcons.SNOW_WIND,
                19 => WeatherIcons.STORM_SHOWERS,
                23 => WeatherIcons.HAZE,
                24 => WeatherIcons.FOG,
                26 or 27 => WeatherIcons.HAIL,
                29 => WeatherIcons.NA,
                41 or 42 or 48 => WeatherIcons.TORNADO,
                43 => WeatherIcons.WINDY,
                44 => WeatherIcons.SMOKE,
                45 or 47 => WeatherIcons.DUST,
                46 => WeatherIcons.SNOW_THUNDERSTORM,
                _ => string.Empty,
            };

            if (string.IsNullOrWhiteSpace(weatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
                weatherIcon = WeatherIcons.NA;
            }

            return weatherIcon;
        }

        public override string GetWeatherCondition(string icon)
        {
            var iconCode = icon?.TryParseInt();

            if (!iconCode.HasValue) return base.GetWeatherCondition(icon);

            return iconCode switch
            {
                0 or 1 => ResConditions.weather_sunny,
                2 or 32 => ResConditions.weather_partlycloudy,
                3 or 4 or 5 or 22 or 33 or 34 or 35 => ResConditions.weather_mostlycloudy,
                6 or 36 => ResConditions.weather_drizzle,
                7 or 15 or 28 or 37 => ResConditions.weather_rainandsnow,
                8 or 38 => ResConditions.weather_snowflurries,
                9 or 19 or 39 => ResConditions.weather_tstorms,
                10 => ResConditions.weather_cloudy,
                11 => ResConditions.weather_rain,
                12 or 13 => ResConditions.weather_rainshowers,
                14 or 26 => ResConditions.weather_freezingrain,
                16 or 17 => ResConditions.weather_snow,
                18 => ResConditions.weather_heavysnow,
                23 => ResConditions.weather_haze,
                24 => ResConditions.weather_fog,
                25 or 40 => ResConditions.weather_blowingsnow,
                27 or 46 => ResConditions.weather_hail,
                29 => ResConditions.weather_notavailable,
                30 or 31 => ResConditions.weather_clear,
                41 or 42 or 48 => ResConditions.weather_tornado,
                43 => ResConditions.weather_windy,
                44 => ResConditions.weather_smoky,
                45 or 47 => ResConditions.weather_dust,
                _ => base.GetWeatherCondition(icon),
            };
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(Weather weather)
        {
            bool isNight = base.IsNight(weather);

            if (!isNight)
            {
                // Fallback to sunset/rise time just in case
                LocalTime sunrise;
                LocalTime sunset;
                if (weather.astronomy != null)
                {
                    sunrise = LocalTime.FromTicksSinceMidnight(weather.astronomy.sunrise.TimeOfDay.Ticks);
                    sunset = LocalTime.FromTicksSinceMidnight(weather.astronomy.sunset.TimeOfDay.Ticks);
                }
                else
                {
                    sunrise = LocalTime.FromHourMinuteSecondTick(6, 0, 0, 0);
                    sunset = LocalTime.FromHourMinuteSecondTick(18, 0, 0, 0);
                }

                DateTimeZone tz = null;

                if (weather.location.tz_long != null)
                {
                    tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
                }

                if (tz == null)
                    tz = DateTimeZone.ForOffset(Offset.FromTimeSpan(weather.location.tz_offset));

                var now = SystemClock.Instance.GetCurrentInstant()
                    .InZone(tz).TimeOfDay;

                // Determine whether its night using sunset/rise times
                if (now < sunrise || now > sunset)
                    isNight = true;
            }

            return isNight;
        }
    }
}