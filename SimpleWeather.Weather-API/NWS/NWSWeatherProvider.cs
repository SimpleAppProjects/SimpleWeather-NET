using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using SimpleWeather.Extras;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Resources.Strings;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.NWS.Hourly;
using SimpleWeather.Weather_API.NWS.Observation;
using SimpleWeather.Weather_API.SMC;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using Location = SimpleWeather.Weather_API.NWS.Hourly.Location;
using PeriodsItem = SimpleWeather.Weather_API.NWS.Hourly.PeriodsItem;
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.NWS
{
    public partial class NWSWeatherProvider : WeatherProviderImpl
    {
        private const string FORECAST_QUERY_URL = "https://forecast.weather.gov/MapClick.php?{0}&FcstType=json";

        private const string HRFORECAST_QUERY_URL =
            "https://forecast.weather.gov/MapClick.php?{0}&FcstType=digitalJSON";

        private const string POINTS_QUERY_URL = "https://api.weather.gov/points/{0}";
        private const int MAX_ATTEMPTS = 2;

        public NWSWeatherProvider() : base()
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

        public override string WeatherAPI => WAPI.NWS;
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;

        public override bool IsRegionSupported(SimpleWeather.LocationData.LocationData location)
        {
            return LocationUtils.IsNWSSupported(location);
        }

        public override bool IsRegionSupported(LocationQuery location)
        {
            return LocationUtils.IsNWSSupported(location);
        }

        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override String GetAPIKey()
        {
            return null;
        }

        public override long GetRetryTime() => 30000;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        protected override async Task<Weather> GetWeatherData(SimpleWeather.LocationData.LocationData location)
        {
            Weather weather = null;
            WeatherException wEx = null;

            // NWS only supports locations in U.S. or U.S. territories
            if (!LocationUtils.IsNWSSupported(location))
            {
                throw new WeatherException(
                    WeatherUtils.ErrorStatus.LocationNotSupported,
                    CustomException.CreateUnsupportedLocationException(WeatherAPI, location));
            }

            var query = await UpdateLocationQuery(location);

            try
            {
                this.CheckRateLimit();

                var observationURL = new Uri(string.Format(FORECAST_QUERY_URL, query));
                var hrlyForecastURL = new Uri(string.Format(HRFORECAST_QUERY_URL, query));

                using var observationRequest = new HttpRequestMessage(HttpMethod.Get, observationURL);
                observationRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(15));
                observationRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
                observationRequest.Headers.UserAgent.AddAppUserAgent();

                using var forecastRequest = new HttpRequestMessage(HttpMethod.Get, hrlyForecastURL);
                forecastRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));
                forecastRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
                forecastRequest.Headers.UserAgent.AddAppUserAgent();

                // Get response
                var webClient = SharedModule.Instance.WebClient;
                using var ctsO = new CancellationTokenSource((int)(SettingsManager.READ_TIMEOUT * 1.5f));
                using var observationResponse = await webClient.SendAsync(observationRequest, ctsO.Token);

                // Check for errors
                await this.CheckForErrors(observationResponse);

                Stream observationStream = await observationResponse.Content.ReadAsStreamAsync();

                // Load point json data
                ForecastRootobject observationData =
                    await JSONParser.DeserializerAsync<ForecastRootobject>(observationStream);

                using var ctsF = new CancellationTokenSource((int)(SettingsManager.READ_TIMEOUT * 1.5f));
                using var forecastResponse = await webClient.SendAsync(forecastRequest, ctsF.Token);

                // Check for errors
                await this.CheckForErrors(forecastResponse);

                Stream forecastStream = await forecastResponse.Content.ReadAsStreamAsync();

                // Load point json data
                HourlyForecastResponse forecastData = await CreateHourlyForecastResponse(forecastStream);

                if (forecastData?.periodsItems?.Count <= 0)
                {
                    forecastData = await GetPointsHourlyForecastResponse(location);
                }

                weather = this.CreateWeatherData(observationData, forecastData);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "NWSWeatherProvider: error getting weather data");
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

        private async Task<HourlyForecastResponse> CreateHourlyForecastResponse(Stream forecastStream)
        {
            var forecastData = new HourlyForecastResponse();
            var forecastObj = await JObject.LoadAsync(new JsonTextReader(new StreamReader(forecastStream)));

            if (forecastObj.HasValues)
            {
                var fcastRoot = forecastObj.Root.ToObject<JObject>();

                forecastData.creationDate = fcastRoot.Property("creationDate").Value.ToObject<DateTimeOffset>();
                forecastData.location = new Location();

                var location = fcastRoot.Property("location").Value.ToObject<JObject>();
                forecastData.location.latitude = location.Property("latitude").Value.ToObject<double>();
                forecastData.location.longitude = location.Property("longitude").Value.ToObject<double>();

                var periodNameList = fcastRoot.Property("PeriodNameList").Value.ToObject<JObject>();
                SortedSet<string> sortedKeys = new SortedSet<string>(periodNameList.Properties().Select(p => p.Name),
                    new StrNumComparator());

                forecastData.periodsItems = new List<PeriodsItem>(sortedKeys.Count);

                foreach (var periodNumber in sortedKeys)
                {
                    var periodName = periodNameList.Value<string>(periodNumber);

                    if (!fcastRoot.ContainsKey(periodName))
                        continue;

                    var item = new PeriodsItem();

                    var periodObj = fcastRoot.Property(periodName)?.Value?.ToObject<JObject>();
                    if (periodObj == null) continue;
                    var timeArr = periodObj.Property("time")?.Value?.ToObject<JArray>();
                    if (timeArr == null) continue;
                    var unixTimeArr = periodObj.Property("unixtime")?.Value?.ToObject<JArray>();
                    if (unixTimeArr == null) continue;
                    var tempArr = periodObj.Property("temperature")?.Value?.ToObject<JArray>();
                    if (tempArr == null) continue;
                    var iconArr = periodObj.Property("iconLink")?.Value?.ToObject<JArray>();
                    if (iconArr == null) continue;
                    var conditionTxtArr = periodObj.Property("weather")?.Value?.ToObject<JArray>();
                    if (conditionTxtArr == null) continue;
                    var windChillArr = periodObj.Property("windChill")?.Value?.ToObject<JArray>();
                    var windSpeedArr = periodObj.Property("windSpeed")?.Value?.ToObject<JArray>();
                    var cloudAmtArr = periodObj.Property("cloudAmount")?.Value?.ToObject<JArray>();
                    var popArr = periodObj.Property("pop")?.Value?.ToObject<JArray>();
                    var humidityArr = periodObj.Property("relativeHumidity")?.Value?.ToObject<JArray>();
                    var windGustArr = periodObj.Property("windGust")?.Value?.ToObject<JArray>();
                    var windDirArr = periodObj.Property("windDirection")?.Value?.ToObject<JArray>();

                    item.periodName = periodObj.Property("periodName").Value.ToObject<string>();

                    item.time = new List<string>(timeArr.Count);
                    foreach (var jsonElement in timeArr.Children<JValue>())
                    {
                        String time = jsonElement.Value?.ToString();
                        item.time.Add(time);
                    }

                    item.unixtime = new List<string>(unixTimeArr.Count);
                    foreach (var jsonElement in unixTimeArr.Children<JValue>())
                    {
                        String time = jsonElement.Value?.ToString();
                        item.unixtime.Add(time);
                    }

                    item.temperature = new List<string>(tempArr.Count);
                    foreach (var jsonElement in tempArr.Children<JValue>())
                    {
                        String temp = jsonElement.Value?.ToString();
                        item.temperature.Add(temp);
                    }

                    item.iconLink = new List<string>(iconArr.Count);
                    foreach (var jsonElement in iconArr.Children<JValue>())
                    {
                        String icon = jsonElement.Value?.ToString();
                        item.iconLink.Add(icon);
                    }

                    item.weather = new List<string>(conditionTxtArr.Count);
                    foreach (var jsonElement in conditionTxtArr.Children<JValue>())
                    {
                        String condition = jsonElement.Value?.ToString();
                        item.weather.Add(condition);
                    }

                    if (windChillArr != null)
                    {
                        item.windChill = new List<string>(windChillArr.Count);
                        foreach (var jsonElement in windChillArr.Children<JValue>())
                        {
                            String windChill = jsonElement.Value?.ToString();
                            item.windChill.Add(windChill);
                        }
                    }
                    else
                    {
                        item.windChill = Enumerable.Repeat<string>(null, unixTimeArr.Count).ToList();
                    }

                    if (windSpeedArr != null)
                    {
                        item.windSpeed = new List<string>(windSpeedArr.Count);
                        foreach (var jsonElement in windSpeedArr.Children<JValue>())
                        {
                            String windSpeed = jsonElement.Value?.ToString();
                            item.windSpeed.Add(windSpeed);
                        }
                    }
                    else
                    {
                        item.windSpeed = Enumerable.Repeat<string>(null, unixTimeArr.Count).ToList();
                    }

                    if (cloudAmtArr != null)
                    {
                        item.cloudAmount = new List<string>(cloudAmtArr.Count);
                        foreach (var jsonElement in cloudAmtArr.Children<JValue>())
                        {
                            String cloudAmt = jsonElement.Value?.ToString();
                            item.cloudAmount.Add(cloudAmt);
                        }
                    }
                    else
                    {
                        item.cloudAmount = Enumerable.Repeat<string>(null, unixTimeArr.Count).ToList();
                    }

                    if (popArr != null)
                    {
                        item.pop = new List<string>(popArr.Count);
                        foreach (var jsonElement in popArr.Children<JValue>())
                        {
                            String pop = jsonElement.Value?.ToString();
                            item.pop.Add(pop);
                        }
                    }
                    else
                    {
                        item.pop = Enumerable.Repeat<string>(null, unixTimeArr.Count).ToList();
                    }

                    if (humidityArr != null)
                    {
                        item.relativeHumidity = new List<string>(humidityArr.Count);
                        foreach (var jsonElement in humidityArr.Children<JValue>())
                        {
                            String humidity = jsonElement.Value?.ToString();
                            item.relativeHumidity.Add(humidity);
                        }
                    }
                    else
                    {
                        item.relativeHumidity = Enumerable.Repeat<string>(null, unixTimeArr.Count).ToList();
                    }

                    if (windGustArr != null)
                    {
                        item.windGust = new List<string>(windGustArr.Count);
                        foreach (var jsonElement in windGustArr.Children<JValue>())
                        {
                            String windGust = jsonElement.Value?.ToString();
                            item.windGust.Add(windGust);
                        }
                    }
                    else
                    {
                        item.windGust = Enumerable.Repeat<string>(null, unixTimeArr.Count).ToList();
                    }

                    if (windDirArr != null)
                    {
                        item.windDirection = new List<string>(windDirArr.Count);
                        foreach (var jsonElement in windDirArr.Children<JValue>())
                        {
                            String windDir = jsonElement.Value?.ToString();
                            item.windDirection.Add(windDir);
                        }
                    }
                    else
                    {
                        item.windDirection = Enumerable.Repeat<string>(null, unixTimeArr.Count).ToList();
                    }

                    forecastData.periodsItems.Add(item);
                }
            }

            return forecastData;
        }

        private Task<HourlyForecastResponse> GetPointsHourlyForecastResponse(
            SimpleWeather.LocationData.LocationData location)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var webClient = SharedModule.Instance.WebClient;

                    Uri pointsUrl = new Uri(string.Format(POINTS_QUERY_URL, UpdatePointsLocationQuery(location)));

                    using var pointsRequest = new HttpRequestMessage(HttpMethod.Get, pointsUrl);
                    pointsRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));
                    pointsRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
                    pointsRequest.Headers.UserAgent.AddAppUserAgent();

                    using var ctsP = new CancellationTokenSource((int)(SettingsManager.READ_TIMEOUT * 1.5f));
                    using var pointsResponse = await webClient.SendAsync(pointsRequest, ctsP.Token);
                    await this.CheckForErrors(pointsResponse);

                    Stream pointsStream = await pointsResponse.Content.ReadAsStreamAsync();

                    // Load point json data
                    PointsRootobject pointsRootobject =
                        await JSONParser.DeserializerAsync<PointsRootobject>(pointsStream);

                    using var forecastRequest =
                        new HttpRequestMessage(HttpMethod.Get, pointsRootobject.forecastHourly + "?units=us");
                    forecastRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));
                    forecastRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
                    forecastRequest.Headers.UserAgent.AddAppUserAgent();

                    HourlyPointsRootobject forecastResponseData = null;

                    for (int i = 0; i < MAX_ATTEMPTS; i++)
                    {
                        try
                        {
                            using var ctsF = new CancellationTokenSource((int)(SettingsManager.READ_TIMEOUT * 1.5f));
                            using var response = await webClient.SendAsync(forecastRequest, ctsF.Token);

                            if (response.StatusCode != HttpStatusCode.BadRequest)
                            {
                                // Check for errors
                                await this.CheckForErrors(response);

                                Stream forecastStream = await response.Content.ReadAsStreamAsync();

                                forecastResponseData = JSONParser.Deserializer<HourlyPointsRootobject>(forecastStream);
                            }
                        }
                        catch
                        {
                        }

                        if (forecastResponseData == null)
                        {
                            await Task.Delay(1000);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return forecastResponseData?.ToResponse(pointsRootobject) ??
                           throw new ArgumentNullException(nameof(forecastResponseData));
                }
                catch
                {
                    return new HourlyForecastResponse();
                }
            });
        }

        private class StrNumComparator : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (int.TryParse(x, out int numX) && int.TryParse(y, out int numY))
                {
                    return numX.CompareTo(numY);
                }
                else
                {
                    return StringComparer.OrdinalIgnoreCase.Compare(x, y);
                }
            }
        }

        protected override async Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location,
            Weather weather)
        {
            var offset = location.tz_offset;

            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(location.tz_offset);

            // NWS does not provide astrodata; calculate this ourselves (using their calculator)
            var solCalcData =
                await new SolCalcAstroProvider().GetAstronomyData(location, weather.condition.observation_time);
            weather.astronomy =
                await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
            weather.astronomy.sunrise = solCalcData.sunrise;
            weather.astronomy.sunset = solCalcData.sunset;

            // Update icons
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            weather.condition.icon = GetWeatherIcon(now < sunrise || now > sunset, weather.condition.icon);

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                var hrf_date = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrf_date;

                var hrf_localTime = hrf_date.DateTime.TimeOfDay;
                hr_forecast.icon = GetWeatherIcon(hrf_localTime < sunrise || hrf_localTime > sunset, hr_forecast.icon);
            }
        }

        public override Task<string> UpdateLocationQuery(Weather weather)
        {
            var str = string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}",
                weather.location.latitude, weather.location.longitude);
            return Task.FromResult(str);
        }

        public override Task<string> UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            var str = string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude,
                location.longitude);
            return Task.FromResult(str);
        }

        private string UpdatePointsLocationQuery(Weather weather)
        {
            var str = string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", weather.location.latitude,
                weather.location.longitude);
            return str;
        }

        private string UpdatePointsLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            var str = string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", location.latitude,
                location.longitude);
            return str;
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            // Example: https://api.weather.gov/icons/land/day/tsra_hi,20?size=medium
            string WeatherIcon = string.Empty;

            if (icon == null)
                return WeatherIcons.NA;

            if (icon.Contains("fog") || icon.Equals("fg.png") || icon.Equals("nfg.png") || icon.Contains("nfg") ||
                icon.Matches(".*([/]?)([n]?)fg([0-9]{0,3})((.png)?).*"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_FOG;
                else
                    WeatherIcon = WeatherIcons.DAY_FOG;
            }
            else if (icon.Contains("blizzard"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW_WIND;
                else
                    WeatherIcon = WeatherIcons.DAY_SNOW_WIND;
            }
            else if (icon.Contains("cold"))
            {
                WeatherIcon = WeatherIcons.SNOWFLAKE_COLD;
            }
            else if (icon.Contains("hot"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_HOT;
                else
                    WeatherIcon = WeatherIcons.DAY_HOT;
            }
            else if (icon.Contains("haze") || icon.Equals("hz.png") ||
                     icon.Matches(".*([/]?)hz([0-9]{0,3})((.png)?).*"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_HAZE;
                else
                    WeatherIcon = WeatherIcons.DAY_HAZE;
            }
            else if (icon.Contains("smoke") || icon.Equals("fu.png") || icon.Equals("nfu.png") ||
                     icon.Contains("nfu") || icon.Matches(".*([/]?)([n]?)fu([0-9]{0,3})((.png)?).*"))
            {
                WeatherIcon = WeatherIcons.SMOKE;
            }
            else if (icon.Contains("dust") || icon.Equals("du.png") || icon.Equals("ndu.png") || icon.Contains("ndu") ||
                     icon.Matches(".*([/]?)([n]?)du([0-9]{0,3})((.png)?).*"))
            {
                WeatherIcon = WeatherIcons.DUST;
            }
            else if (icon.Contains("tropical_storm") || icon.Contains("hurricane") || icon.Contains("hur_warn") ||
                     icon.Contains("hur_watch") || icon.Contains("ts_warn") || icon.Contains("ts_watch") ||
                     icon.Contains("ts_nowarn"))
            {
                WeatherIcon = WeatherIcons.HURRICANE;
            }
            else if (icon.Contains("tsra"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_THUNDERSTORM;
                else
                    WeatherIcon = WeatherIcons.DAY_THUNDERSTORM;
            }
            else if (icon.Contains("tornado") || icon.Contains("tor") || icon.Equals("tor.png") ||
                     icon.Equals("fc.png") || icon.Equals("nfc.png") || icon.Contains("nfc") ||
                     icon.Matches(".*([/]?)([n]?)fc([0-9]{0,3})((.png)?).*"))
            {
                WeatherIcon = WeatherIcons.TORNADO;
            }
            else if (icon.Contains("rain_showers") || icon.Contains("shra") || icon.Contains("shwrs"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_SHOWERS;
                else
                    WeatherIcon = WeatherIcons.DAY_SHOWERS;
            }
            else if (icon.Contains("fzra") || icon.Contains("rain_sleet") || icon.Contains("rain_snow") ||
                     icon.Contains("ra_sn"))
            {
                WeatherIcon = WeatherIcons.RAIN_MIX;
            }
            else if (icon.Contains("sleet") || icon.Contains("raip"))
            {
                WeatherIcon = WeatherIcons.SLEET;
            }
            else if (icon.Contains("minus_ra"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_SPRINKLE;
                else
                    WeatherIcon = WeatherIcons.DAY_SPRINKLE;
            }
            else if (icon.Contains("rain") || icon.Equals("ra.png") || icon.Equals("nra.png") || icon.Contains("nra") ||
                     icon.Matches(".*([/]?)([n]?)ra([0-9]{0,3})((.png)?).*"))
            {
                WeatherIcon = WeatherIcons.RAIN;
            }
            else if (icon.Contains("snow") || icon.Equals("sn.png") || icon.Equals("nsn.png") || icon.Contains("nsn") ||
                     icon.Matches(".*([/]?)([n]?)sn([0-9]{0,3})((.png)?).*"))
            {
                WeatherIcon = WeatherIcons.SNOW;
            }
            else if (icon.Contains("snip") || icon.Equals("ip.png") || icon.Equals("nip.png") || icon.Contains("nip") ||
                     icon.Matches(".*([/]?)([n]?)ip([0-9]{0,3})((.png)?).*"))
            {
                WeatherIcon = WeatherIcons.HAIL;
            }
            else if (icon.Contains("wind_bkn") || icon.Contains("wind_ovc") || icon.Contains("wind_sct"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY_WINDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY_WINDY;
            }
            else if (icon.Contains("wind_skc") || icon.Contains("wind_few") || icon.Contains("wind"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_WINDY;
                else
                    WeatherIcon = WeatherIcons.DAY_WINDY;
            }
            else if (icon.Contains("ovc"))
            {
                WeatherIcon = WeatherIcons.OVERCAST;
            }
            else if (icon.Contains("sct") || icon.Contains("few"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_PARTLY_CLOUDY;
            }
            else if (icon.Contains("bkn"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY;
            }
            else if (icon.Contains("skc"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;
            }
            else
            {
                this.LogMissingIcon(icon);
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        public override String GetWeatherCondition(String icon)
        {
            if (icon == null)
                return WeatherConditions.weather_notavailable;

            if (!icon.Contains(".png") && !icon.Contains("weather.gov"))
            {
                return base.GetWeatherCondition(icon);
            }

            if (icon.Contains("fog") || icon.Equals("fg.png") || icon.Equals("nfg.png") || icon.Contains("nfg") ||
                icon.Matches(".*([/]?)([n]?)fg([0-9]{0,3})((.png)?).*"))
            {
                return WeatherConditions.weather_fog;
            }
            else if (icon.Contains("blizzard"))
            {
                return WeatherConditions.weather_blizzard;
            }
            else if (icon.Contains("cold"))
            {
                return WeatherConditions.weather_cold;
            }
            else if (icon.Contains("hot"))
            {
                return WeatherConditions.weather_hot;
            }
            else if (icon.Contains("haze") || icon.Equals("hz.png") ||
                     icon.Matches(".*([/]?)hz([0-9]{0,3})((.png)?).*"))
            {
                return WeatherConditions.weather_haze;
            }
            else if (icon.Contains("smoke") || icon.Equals("fu.png") || icon.Equals("nfu.png") ||
                     icon.Contains("nfu") || icon.Matches(".*([/]?)([n]?)fu([0-9]{0,3})((.png)?).*"))
            {
                return WeatherConditions.weather_smoky;
            }
            else if (icon.Contains("dust") || icon.Equals("du.png") || icon.Equals("ndu.png") || icon.Contains("ndu") ||
                     icon.Matches(".*([/]?)([n]?)du([0-9]{0,3})((.png)?).*"))
            {
                return WeatherConditions.weather_dust;
            }
            else if (icon.Contains("tropical_storm") || icon.Contains("ts_warn") || icon.Contains("ts_watch") ||
                     icon.Contains("ts_nowarn"))
            {
                return WeatherConditions.weather_tropicalstorm;
            }
            else if (icon.Contains("hurricane") || icon.Contains("hur_warn") || icon.Contains("hur_watch"))
            {
                return WeatherConditions.weather_hurricane;
            }
            else if (icon.Contains("tsra"))
            {
                return WeatherConditions.weather_tstorms;
            }
            else if (icon.Contains("tornado") || icon.Contains("tor") || icon.Equals("tor.png") ||
                     icon.Equals("fc.png") || icon.Equals("nfc.png") || icon.Contains("nfc") ||
                     icon.Matches(".*([/]?)([n]?)fc([0-9]{0,3})((.png)?).*"))
            {
                return WeatherConditions.weather_tornado;
            }
            else if (icon.Contains("rain_showers") || icon.Contains("shra") || icon.Contains("shwrs"))
            {
                return WeatherConditions.weather_rainshowers;
            }
            else if (icon.Contains("rain_sleet") || icon.Contains("raip"))
            {
                return WeatherConditions.weather_rainandsleet;
            }
            else if (icon.Contains("rain_snow") || icon.Contains("ra_sn"))
            {
                return WeatherConditions.weather_rainandsnow;
            }
            else if (icon.Contains("fzra"))
            {
                return WeatherConditions.weather_freezingrain;
            }
            else if (icon.Contains("snow_sleet"))
            {
                return WeatherConditions.weather_snowandsleet;
            }
            else if (icon.Contains("sleet"))
            {
                return WeatherConditions.weather_sleet;
            }
            else if (icon.Contains("rain") || icon.Equals("ra.png") || icon.Equals("nra.png") || icon.Contains("nra") ||
                     icon.Matches(".*([/]?)([n]?)ra([0-9]{0,3})((.png)?).*"))
            {
                return WeatherConditions.weather_rain;
            }
            else if (icon.Contains("snow") || icon.Equals("sn.png") || icon.Equals("nsn.png") || icon.Contains("nsn") ||
                     icon.Matches(".*([/]?)([n]?)sn([0-9]{0,3})((.png)?).*"))
            {
                return WeatherConditions.weather_snow;
            }
            else if (icon.Contains("snip") || icon.Equals("ip.png") || icon.Equals("nip.png") || icon.Contains("nip") ||
                     icon.Matches(".*([/]?)([n]?)ip([0-9]{0,3})((.png)?).*"))
            {
                return WeatherConditions.weather_hail;
            }
            else if (icon.Contains("wind_bkn") || icon.Contains("wind_ovc") || icon.Contains("wind_sct") ||
                     icon.Contains("wind"))
            {
                return WeatherConditions.weather_windy;
            }
            else if (icon.Contains("ovc"))
            {
                return WeatherConditions.weather_overcast;
            }
            else if (icon.Contains("sct") || icon.Contains("few"))
            {
                return WeatherConditions.weather_partlycloudy;
            }
            else if (icon.Contains("bkn"))
            {
                return WeatherConditions.weather_cloudy;
            }
            else if (icon.Contains("skc"))
            {
                return WeatherConditions.weather_clearsky;
            }
            else
            {
                return base.GetWeatherCondition(icon);
            }
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(Weather weather)
        {
            bool isNight = base.IsNight(weather);

            // The following cases can be present at any time of day
            switch (weather.condition.icon)
            {
                case WeatherIcons.SNOWFLAKE_COLD:
                case WeatherIcons.SMOKE:
                case WeatherIcons.DUST:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.TORNADO:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.SLEET:
                case WeatherIcons.RAIN:
                case WeatherIcons.SNOW:
                case WeatherIcons.HAIL:
                case WeatherIcons.OVERCAST:
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

                    break;
            }

            return isNight;
        }
    }
}