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
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.BrightSky
{
    public partial class BrightSkyProvider : WeatherProviderImpl
    {
        private const string BASE_URL = "https://api.brightsky.dev/";
        private const string CURRENT_QUERY_URL = BASE_URL + "current_weather?{0}";
        private const string FORECAST_QUERY_URL = BASE_URL + "weather?{0}";
        private const string ALERTS_QUERY_URL = BASE_URL + "alerts?{0}";

        public BrightSkyProvider() : base()
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

        public override string WeatherAPI => WAPI.DWD;
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;

        public override bool IsRegionSupported(SimpleWeather.LocationData.LocationData location)
        {
            return LocationUtils.IsGermany(location);
        }

        public override bool IsRegionSupported(LocationQuery location)
        {
            return LocationUtils.IsGermany(location);
        }

        public override int HourlyForecastInterval => 1;

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

            // DWD is best in Germany
            if (!LocationUtils.IsGermany(location))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound,
                    new Exception(
                        $"Unsupported country code: provider ({WeatherAPI}), country ({location.country_code})"));
            }

            var query = await UpdateLocationQuery(location);

            try
            {
                // If were under rate limit, deny request
                this.CheckRateLimit();

                var now = DateTimeOffset.UtcNow;

                var webClient = SharedModule.Instance.WebClient;

                var currentRequestUri = string.Format(CURRENT_QUERY_URL, query).ToUriBuilderEx()
                    .AppendQueryParameter("tz", "Etc/UTC")
                    .AppendQueryParameter("units", "dwd")
                    .BuildUri();

                using var currentRequest = new HttpRequestMessage(HttpMethod.Get, currentRequestUri)
                    .CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(15));

                var forecastRequestUri = string.Format(FORECAST_QUERY_URL, query).ToUriBuilderEx()
                    .AppendQueryParameter("date", now.ToInvariantString("yyyy-MM-dd"))
                    .AppendQueryParameter("last_date", now.AddDays(5).ToInvariantString("yyyy-MM-dd"))
                    .AppendQueryParameter("tz", "Etc/UTC")
                    .AppendQueryParameter("units", "dwd")
                    .BuildUri();

                using var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastRequestUri)
                    .CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                var alertsRequestUri = string.Format(ALERTS_QUERY_URL, query).ToUriBuilderEx()
                    .AppendQueryParameter("tz", "Etc/UTC")
                    .BuildUri();

                using var alertsRequest = new HttpRequestMessage(HttpMethod.Get, alertsRequestUri)
                    .CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                // Connect to webstream
                using var ctsC = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var currentResponse = await webClient.SendAsync(currentRequest, ctsC.Token);

                await this.CheckForErrors(currentResponse);
                currentResponse.EnsureSuccessStatusCode();

                using var ctsF = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var forecastResponse = await webClient.SendAsync(forecastRequest, ctsF.Token);

                await this.CheckForErrors(forecastResponse);
                forecastResponse.EnsureSuccessStatusCode();

                using var ctsA = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var alertsResponse = await webClient.SendAsync(alertsRequest, ctsA.Token);

                await this.CheckForErrors(alertsResponse);
                alertsResponse.EnsureSuccessStatusCode();

                var currentStream = await currentResponse.Content.ReadAsStringAsync();
                var forecastStream = await forecastResponse.Content.ReadAsStringAsync();
                var alertStream = await alertsResponse.Content.ReadAsStringAsync();

                // Load weather
                var currRoot = await JSONParser.DeserializerAsync<CurrentRootobject>(currentStream) ??
                               throw new ArgumentNullException("currRoot");
                var foreRoot = await JSONParser.DeserializerAsync<ForecastRootobject>(forecastStream) ??
                               throw new ArgumentNullException("foreRoot");
                var alertsRoot = await JSONParser.DeserializerAsync<AlertsRootobject>(alertStream);

                weather = this.CreateWeatherData(currRoot, foreRoot, location);
                weather.weather_alerts = this.CreateWeatherAlerts(alertsRoot);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "BrightSkyProvider: error getting weather data");
            }

            if (wEx == null && (weather == null || !weather.IsValid()))
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null)
            {
                weather.query = query;
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        protected override async Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location,
            Weather weather)
        {
            // DWD reports datetime in UTC; add location tz_offset
            var offset = location.tz_offset;
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            // Calculate astronomy
            try
            {
                weather.astronomy =
                    await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
            }
            catch (WeatherException)
            {
                weather.astronomy =
                    await new SolCalcAstroProvider().GetAstronomyData(location, weather.condition.observation_time);
            }

            // Set condition here
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            weather.condition.icon = GetWeatherIcon(now < sunrise || now > sunset, weather.condition.icon);

            if (string.IsNullOrWhiteSpace(weather.condition.weather))
            {
                weather.condition.weather = GetWeatherCondition(weather.condition.icon);
            }

            foreach (Forecast forecast in weather.forecast)
            {
                forecast.icon.Let(it =>
                {
                    forecast.icon = GetWeatherIcon(it);
                    forecast.condition = GetWeatherCondition(it);
                });
            }

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                var hrfDate = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrfDate;

                var hrfLocalTime = hrfDate.DateTime.TimeOfDay;

                hr_forecast.icon.Let(it =>
                {
                    hr_forecast.icon = GetWeatherIcon(hrfLocalTime < sunrise || hrfLocalTime > sunset, it);
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
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}",
                weather.location.latitude, weather.location.longitude));
        }

        public override Task<string> UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}",
                location.latitude, location.longitude));
        }

        public override string LocaleToLangCode(string iso, string name)
        {
            if (iso == "de")
                return iso;

            return "en";
        }

        public override string GetWeatherIcon(string icon)
        {
            var isNight = false;

            if (icon == null) return WeatherIcons.NA;

            if (icon.EndsWith("-night")) isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            if (icon == null) return WeatherIcons.NA;

            string weatherIcon = icon switch
            {
                "clear-day" or "clear-night" => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,

                "partly-cloudy-day" or "partly-cloudy-night" => isNight
                    ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY
                    : WeatherIcons.DAY_PARTLY_CLOUDY,

                "cloudy" => WeatherIcons.CLOUDY,
                "fog" => WeatherIcons.FOG,
                "wind" => WeatherIcons.WINDY,
                "rain" => WeatherIcons.RAIN,
                "sleet" => WeatherIcons.SLEET,
                "snow" => WeatherIcons.SNOW,
                "hail" => WeatherIcons.HAIL,
                "thunderstorm" => WeatherIcons.THUNDERSTORM,
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(weatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
                weatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY;
            }

            return weatherIcon;
        }

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