using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
#if __IOS__
using Microsoft.Maui.ApplicationModel;
#endif
using SimpleWeather.Extras;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.Google
{
    public partial class GoogleWeatherProvider : WeatherProviderImpl
    {
        private const string BASE_URL = "https://weather.googleapis.com/v1/";
        private const string CONDITIONS_PATH = "currentConditions:lookup";
        private const string FORECAST_PATH = "forecast/days:lookup";
        private const string HOURLY_FORECAST_PATH = "forecast/hours:lookup";

        public GoogleWeatherProvider() : base()
        {
            LocationProvider = this.RunCatching(() =>
            {
                return WeatherModule.Instance.LocationProviderFactory.GetLocationProvider(
                    RemoteConfigService.GetLocationProvider(WeatherAPI));
            }).GetOrElse<IWeatherLocationProvider, IWeatherLocationProvider>((t) =>
            {
                return new Radar.RadarLocationProvider();
            });
        }

        public override string WeatherAPI => WAPI.Google;
        public override bool KeyRequired => true;
        public override bool SupportsWeatherLocale => true;
        public override int HourlyForecastInterval => 1;
        public override AuthType AuthType => AuthType.ApiKey;

        public override long GetRetryTime()
        {
            return 5000;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<bool> IsKeyValid(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);

            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                var requestUri = BASE_URL.ToUriBuilderEx()
                    .AppendPath(FORECAST_PATH, encode: false)
                    .AppendQueryParameter("key", key)
                    .BuildUri();

                // Connect to webstream
                var webClient = SharedModule.Instance.WebClient;
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

#if __IOS__
                request.Headers.Add("X-Ios-Bundle-Identifier", AppInfo.PackageName);
#endif
                request.Headers.UserAgent.AddAppUserAgent();
                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromDays(1)
                };

                using (request)
                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                using (var response = await webClient.SendAsync(request, cts.Token))
                {
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
            }
            catch (Exception ex)
            {
                isValid = false;
                if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }
            }

            if (wEx != null && wEx.ErrorStatus != WeatherUtils.ErrorStatus.InvalidAPIKey)
            {
                throw wEx;
            }

            return isValid;
        }

        public override string GetAPIKey()
        {
            return APIKeys.GetGWeatherKey();
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        protected override async Task<Weather> GetWeatherData(SimpleWeather.LocationData.LocationData location)
        {
            Weather weather = null;
            WeatherException wEx = null;

            var culture = LocaleUtils.GetLocale();
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetProviderKey();

            if (String.IsNullOrWhiteSpace(key))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
            }

            var query = await UpdateLocationQuery(location);

            try
            {
                this.CheckRateLimit();

                var conditionRequestUri = BASE_URL.ToUriBuilderEx()
                    .AppendPath(CONDITIONS_PATH, encode: false)
                    .AppendQueryParameter("location.latitude", location.latitude.ToString("0.####"))
                    .AppendQueryParameter("location.longitude", location.longitude.ToString("0.####"))
                    .AppendQueryParameter("units_system", "METRIC")
                    .AppendQueryParameter("languageCode", locale)
                    .AppendQueryParameter("key", key)
                    .BuildUri();

                var conditionRequest = new HttpRequestMessage(HttpMethod.Get, conditionRequestUri)
                    .CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(15))
                    .Apply(request =>
                    {
#if __IOS__
                        request.Headers.Add("X-Ios-Bundle-Identifier", AppInfo.PackageName);
#endif
                        request.Headers.UserAgent.AddAppUserAgent();
                    });

                var dailyRequestUri = BASE_URL.ToUriBuilderEx()
                    .AppendPath(FORECAST_PATH, encode: false)
                    .AppendQueryParameter("location.latitude", location.latitude.ToString("0.####"))
                    .AppendQueryParameter("location.longitude", location.longitude.ToString("0.####"))
                    .AppendQueryParameter("units_system", "METRIC")
                    .AppendQueryParameter("languageCode", locale)
                    .AppendQueryParameter("key", key)
                    .BuildUri();

                var dailyRequest = new HttpRequestMessage(HttpMethod.Get, dailyRequestUri)
                    .CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1))
                    .Apply(request =>
                    {
#if __IOS__
                        request.Headers.Add("X-Ios-Bundle-Identifier", AppInfo.PackageName);
#endif
                        request.Headers.UserAgent.AddAppUserAgent();
                    });

                var hourlyRequestUri = BASE_URL.ToUriBuilderEx()
                    .AppendPath(HOURLY_FORECAST_PATH, encode: false)
                    .AppendQueryParameter("location.latitude", location.latitude.ToString("0.####"))
                    .AppendQueryParameter("location.longitude", location.longitude.ToString("0.####"))
                    .AppendQueryParameter("units_system", "METRIC")
                    .AppendQueryParameter("languageCode", locale)
                    .AppendQueryParameter("key", key)
                    .BuildUri();

                var hourlyRequest = new HttpRequestMessage(HttpMethod.Get, hourlyRequestUri)
                    .CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1))
                    .Apply(request =>
                    {
#if __IOS__
                        request.Headers.Add("X-Ios-Bundle-Identifier", AppInfo.PackageName);
#endif
                        request.Headers.UserAgent.AddAppUserAgent();
                    });

                // Get response
                var webClient = SharedModule.Instance.WebClient;
                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var conditionResponse = await webClient.SendAsync(conditionRequest, cts.Token);
                await this.CheckForErrors(conditionResponse);
                conditionResponse.EnsureSuccessStatusCode();

                await using var stream = await conditionResponse.Content.ReadAsStreamAsync();
                var conditionData = await JSONParser.DeserializerAsync<CurrentResponse>(stream);

                var dailyData = await GetDailyForecast(webClient, dailyRequest);
                var hourlyData = await GetHourlyForecast(webClient, hourlyRequest);

                weather = this.CreateWeatherData(conditionData, dailyData, hourlyData);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "GoogleWeatherProvider: error getting weather data");
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

        private async Task<DailyResponse> GetDailyForecast(HttpClient client, HttpRequestMessage dailyRequest)
        {
            var dailyData = new DailyResponse();
            var dailyForecasts = new List<ForecastDaysItem>();

            do
            {
                HttpRequestMessage request;

                if (dailyData.nextPageToken != null)
                {
                    var newUrl = dailyRequest.RequestUri.ToUriBuilderEx()
                        .SetQueryParameter("pageToken", dailyData.nextPageToken)
                        .BuildUri();

                    request = dailyRequest.Clone();
                    request.RequestUri = newUrl;
                }
                else
                {
                    request = dailyRequest;
                }

                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var dailyResponse = await client.SendAsync(request, cts.Token);
                await this.CheckForErrors(dailyResponse);
                dailyResponse.EnsureSuccessStatusCode();

                await using var stream = await dailyResponse.Content.ReadAsStreamAsync();
                var dailyResponseData = await JSONParser.DeserializerAsync<DailyResponse>(stream);

                dailyData.timeZone ??= dailyResponseData?.timeZone;
                dailyData.nextPageToken = dailyResponseData?.nextPageToken;

                dailyResponseData?.forecastDays?.Let(it => dailyForecasts.AddRange(it));
            } while (!string.IsNullOrWhiteSpace(dailyData.nextPageToken));

            dailyData.forecastDays = dailyForecasts;

            return dailyData;
        }

        private async Task<HourlyResponse> GetHourlyForecast(HttpClient client, HttpRequestMessage hourlyRequest)
        {
            var hourlyData = new HourlyResponse();
            var hourlyForecasts = new List<ForecastHoursItem>();

            do
            {
                HttpRequestMessage request;

                if (hourlyData.nextPageToken != null)
                {
                    var newUrl = hourlyRequest.RequestUri.ToUriBuilderEx()
                        .SetQueryParameter("pageToken", hourlyData.nextPageToken)
                        .BuildUri();

                    request = hourlyRequest.Clone();
                    request.RequestUri = newUrl;
                }
                else
                {
                    request = hourlyRequest;
                }

                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var hourlyResponse = await client.SendAsync(request, cts.Token);
                await this.CheckForErrors(hourlyResponse);
                hourlyResponse.EnsureSuccessStatusCode();

                await using var stream = await hourlyResponse.Content.ReadAsStreamAsync();
                var hourlyResponseData = await JSONParser.DeserializerAsync<HourlyResponse>(stream);

                hourlyData.timeZone ??= hourlyResponseData?.timeZone;
                hourlyData.nextPageToken = hourlyResponseData?.nextPageToken;

                hourlyResponseData?.forecastHours?.Let(it => hourlyForecasts.AddRange(it));
            } while (!string.IsNullOrWhiteSpace(hourlyData.nextPageToken));

            hourlyData.forecastHours = hourlyForecasts;

            return hourlyData;
        }

        protected override async Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location,
            Weather weather)
        {
            await base.UpdateWeatherData(location, weather);

            // Update forecast, hourly, sunrise/sunset, moonrise/moonset
            var offset = location.tz_offset;

            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            weather.forecast?.ForEach(forecast => { forecast.date = forecast.date.AddSeconds(offset.TotalSeconds); });
            weather.hr_forecast?.ForEach(hrForecast => { hrForecast.date = hrForecast.date.ToOffset(offset); });

            if (weather.astronomy != null)
            {
                // The time of day is set to max if the sun never sets/rises and
                // DateTime is set to min if not found
                // Don't change this if its set that way
                if (weather.astronomy.sunrise > DateTime.MinValue &&
                    weather.astronomy.sunrise.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    weather.astronomy.sunrise = weather.astronomy.sunrise.Add(offset);
                if (weather.astronomy.sunset > DateTime.MinValue &&
                    weather.astronomy.sunset.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    weather.astronomy.sunset = weather.astronomy.sunset.Add(offset);
                if (weather.astronomy.moonrise > DateTime.MinValue &&
                    weather.astronomy.moonrise.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    weather.astronomy.moonrise = weather.astronomy.moonrise.Add(offset);
                if (weather.astronomy.moonset > DateTime.MinValue &&
                    weather.astronomy.moonset.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    weather.astronomy.moonset = weather.astronomy.moonset.Add(offset);
            }
        }

        public override Task<string> UpdateLocationQuery(Weather weather)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture,
                "location.latitude={0:0.####}&location.longitude={1:0.####}",
                weather.location.latitude, weather.location.longitude));
        }

        public override Task<string> UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture,
                "location.latitude={0:0.####}&location.longitude={1:0.####}",
                location.latitude, location.longitude));
        }

        public override string LocaleToLangCode(string iso, string name)
        {
            return name;
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            return icon switch
            {
                "CLEAR" => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,

                "MOSTLY_CLEAR" or "PARTLY_CLOUDY" => isNight
                    ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY
                    : WeatherIcons.DAY_PARTLY_CLOUDY,

                "MOSTLY_CLOUDY" => isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY,

                "CLOUDY" => WeatherIcons.CLOUDY,

                "WINDY" => WeatherIcons.WINDY,

                "WIND_AND_RAIN" or "HEAVY_RAIN_SHOWERS" or "MODERATE_TO_HEAVY_RAIN" or "HEAVY_RAIN"
                    or "RAIN_PERIODICALLY_HEAVY" => WeatherIcons.RAIN_WIND,

                "LIGHT_RAIN_SHOWERS" or "CHANCE_OF_SHOWERS" or "LIGHT_RAIN" => WeatherIcons.SPRINKLE,

                "SCATTERED_SHOWERS" or "RAIN_SHOWERS" or "LIGHT_TO_MODERATE_RAIN" => WeatherIcons.SHOWERS,

                "RAIN" => WeatherIcons.RAIN,

                "LIGHT_SNOW_SHOWERS" or "CHANCE_OF_SNOW_SHOWERS" or "SCATTERED_SNOW_SHOWERS" or "SNOW_SHOWERS" or
                    "LIGHT_TO_MODERATE_SNOW" or "MODERATE_TO_HEAVY_SNOW" or "SNOW" or "LIGHT_SNOW" => WeatherIcons.SNOW,

                "HEAVY_SNOW_SHOWERS" or "HEAVY_SNOW" or "SNOWSTORM" or "SNOW_PERIODICALLY_HEAVY" or "HEAVY_SNOW_STORM"
                    or
                    "BLOWING_SNOW" => WeatherIcons.SNOW_WIND,

                "RAIN_AND_SNOW" => WeatherIcons.RAIN_MIX,

                "HAIL" or "HAIL_SHOWERS" => WeatherIcons.HAIL,

                "THUNDERSTORM" or "THUNDERSHOWER" or "LIGHT_THUNDERSTORM_RAIN" or "SCATTERED_THUNDERSTORMS"
                    or "HEAVY_THUNDERSTORM" => WeatherIcons.THUNDERSTORM,

                _ => WeatherIcons.NA
            };
        }
    }
}