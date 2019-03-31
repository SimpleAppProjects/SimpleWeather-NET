using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Globalization;
using SimpleWeather.UWP.Controls;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SimpleWeather.WeatherYahoo
{
    public partial class YahooWeatherProvider : WeatherProviderImpl
    {
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => true;

        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string location_query)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            string queryAPI = "https://autocomplete.wunderground.com/aq?query=";
            string options = "&h=0&cities=1";
            Uri queryURL = new Uri(queryAPI + location_query + options);
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                // End Stream
                webClient.Dispose();

                // Load data
                locations = new ObservableCollection<LocationQueryViewModel>();

                var root = JSONParser.Deserializer<AC_Rootobject>(contentStream);

                foreach (AC_RESULT result in root.RESULTS)
                {
                    // Filter: only store city results
                    if (result.type != "city")
                        continue;

                    locations.Add(new LocationQueryViewModel(result));

                    // Limit amount of results
                    maxResults--;
                    if (maxResults <= 0)
                        break;
                }

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                locations = new ObservableCollection<LocationQueryViewModel>();
                Logger.WriteLine(LoggerLevel.Error, ex, "YahooWeatherProvider: error getting locations");
            }

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };

            return locations;
        }

        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord)
        {
            LocationQueryViewModel location = null;

            string queryAPI = "https://api.wunderground.com/auto/wui/geo/GeoLookupXML/index.xml?query=";
            string options = "";
            string query = string.Format("{0},{1}", coord.Latitude, coord.Longitude);
            Uri queryURL = new Uri(queryAPI + query + options);
            location result;
            WeatherException wEx = null;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                // End Stream
                webClient.Dispose();

                // Load data
                XmlSerializer deserializer = new XmlSerializer(typeof(location));
                result = (location)deserializer.Deserialize(contentStream);

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                result = null;
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "YahooWeatherProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.query))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<LocationQueryViewModel> GetLocation(string location_query)
        {
            LocationQueryViewModel location = null;

            string queryAPI = "https://autocomplete.wunderground.com/aq?query=";
            string options = "&h=0&cities=1";
            Uri queryURL = new Uri(queryAPI + location_query + options);
            AC_RESULT result;
            WeatherException wEx = null;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                // End Stream
                webClient.Dispose();

                // Load data
                AC_Rootobject root = JSONParser.Deserializer<AC_Rootobject>(contentStream);
                result = root.RESULTS.FirstOrDefault();

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                result = null;
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "YahooWeatherProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.l))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<bool> IsKeyValid(string key)
        {
            throw new NotImplementedException();
        }

        public override async Task<Weather> GetWeather(string location_query)
        {
            Weather weather = null;

            string queryAPI = "https://weather-ydn-yql.media.yahoo.com/forecastrss";
            Uri weatherURL = null;

            OAuthRequest authRequest = new OAuthRequest(GetCliID(), GetCliSecr());

            HttpClient webClient = new HttpClient();
            WeatherException wEx = null;

            try
            {
                string query = "?" + location_query + "&format=json&u=f";
                weatherURL = new Uri(queryAPI + query);
                string authorization = authRequest.GetAuthorizationHeader(weatherURL);

                // Get response
                using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                {
                    // Add headers to request
                    request.Headers.Add("Authorization", authorization);
                    request.Headers.Add("Yahoo-App-Id", GetAppID());
                    request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await webClient.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                    // Reset exception
                    wEx = null;

                    // Load weather
                    Rootobject root = null;
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<Rootobject>(contentStream);
                    });

                    // End Stream
                    if (contentStream != null)
                        contentStream.Dispose();

                    weather = new Weather(root);
                }
            }
            catch (Exception ex)
            {
                weather = null;
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "YahooWeatherProvider: error getting weather data");
            }

            // End Stream
            webClient.Dispose();

            if (weather == null || !weather.IsValid())
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null)
            {
                weather.query = location_query;
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await base.GetWeather(location);

            weather.update_time = weather.update_time.ToOffset(location.tz_offset);

            return weather;
        }

        // Use location name here instead of query since we use the AutoComplete API
        public override async Task UpdateLocationData(LocationData location)
        {
            var qview = await GetLocation(location.name);

            if (qview != null)
            {
                location.name = qview.LocationName;
                location.latitude = qview.LocationLat;
                location.longitude = qview.LocationLong;
                location.tz_long = qview.LocationTZ_Long;

                // Update DB here or somewhere else
                await Settings.UpdateLocation(location);
            }
        }

        public override async Task<string> UpdateLocationQuery(Weather weather)
        {
            string query = string.Empty;
            WeatherUtils.Coordinate coord = new WeatherUtils.Coordinate(double.Parse(weather.location.latitude), double.Parse(weather.location.longitude));
            var qview = await GetLocation(coord);

            if (String.IsNullOrEmpty(qview.LocationQuery))
                query = string.Format(CultureInfo.InvariantCulture, "lat={0}&lon={1}", coord.Latitude, coord.Longitude);
            else
                query = qview.LocationQuery;

            return query;
        }

        public override async Task<string> UpdateLocationQuery(LocationData location)
        {
            string query = string.Empty;
            WeatherUtils.Coordinate coord = new WeatherUtils.Coordinate(location.latitude, location.longitude);
            var qview = await GetLocation(coord);

            if (String.IsNullOrEmpty(qview.LocationQuery))
                query = string.Format(CultureInfo.InvariantCulture, "lat={0}&lon={1}", coord.Latitude, coord.Longitude);
            else
                query = qview.LocationQuery;

            return query;
        }

        public override string GetWeatherIcon(string icon)
        {
            bool isNight = false;

            if (int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    case 27: // Mostly Cloudy (Night)
                    case 29: // Partly Cloudy (Night)
                    case 31: // Clear (Night)
                    case 33: // Fair (Night)
                        isNight = true;
                        break;
                    default:
                        break;
                }
            }

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            if (int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    case 0: // Tornado
                        WeatherIcon = WeatherIcons.TORNADO;
                        break;
                    case 1: // Tropical Storm
                    case 37:
                    case 38: // Scattered Thunderstorms/showers
                    case 39:
                    case 45:
                    case 47:
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_STORM_SHOWERS;
                        else
                            WeatherIcon = WeatherIcons.DAY_STORM_SHOWERS;
                        break;
                    case 2: // Hurricane
                        WeatherIcon = WeatherIcons.HURRICANE;
                        break;
                    case 3:
                    case 4: // Scattered Thunderstorms
                        WeatherIcon = WeatherIcons.THUNDERSTORM;
                        break;
                    case 5: // Mixed Rain/Snow
                    case 6: // Mixed Rain/Sleet
                    case 7: // Mixed Snow/Sleet
                    case 18: // Sleet
                    case 35: // Mixed Rain/Hail
                        WeatherIcon = WeatherIcons.RAIN_MIX;
                        break;
                    case 8: // Freezing Drizzle
                    case 10: // Freezing Rain
                    case 17: // Hail
                        WeatherIcon = WeatherIcons.HAIL;
                        break;
                    case 9: // Drizzle
                    case 11: // Showers
                    case 12:
                    case 40: // Scattered Showers
                        WeatherIcon = WeatherIcons.SHOWERS;
                        break;
                    case 13: // Snow Flurries
                    case 14: // Light Snow Showers
                    case 16: // Snow
                    case 42: // Scattered Snow Showers
                    case 46: // Snow Showers
                        WeatherIcon = WeatherIcons.SNOW;
                        break;
                    case 15: // Blowing Snow
                    case 41: // Heavy Snow
                    case 43:
                        WeatherIcon = WeatherIcons.SNOW_WIND;
                        break;
                    case 19: // Dust
                        WeatherIcon = WeatherIcons.DUST;
                        break;
                    case 20: // Foggy
                        WeatherIcon = WeatherIcons.FOG;
                        break;
                    case 21: // Haze
                        if (isNight)
                            WeatherIcon = WeatherIcons.WINDY;
                        else
                            WeatherIcon = WeatherIcons.DAY_HAZE;
                        break;
                    case 22: // Smoky
                        WeatherIcon = WeatherIcons.SMOKE;
                        break;
                    case 23: // Blustery
                    case 24: // Windy
                        WeatherIcon = WeatherIcons.STRONG_WIND;
                        break;
                    case 25: // Cold
                        WeatherIcon = WeatherIcons.SNOWFLAKE_COLD;
                        break;
                    case 26: // Cloudy
                        WeatherIcon = WeatherIcons.CLOUDY;
                        break;
                    case 27: // Mostly Cloudy (Night)
                    case 28: // Mostly Cloudy (Day)
                    case 29: // Partly Cloudy (Night)
                    case 30: // Partly Cloudy (Day)
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                        else
                            WeatherIcon = WeatherIcons.DAY_CLOUDY;
                        break;
                    case 31: // Clear (Night)
                    case 32: // Sunny
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                        else
                            WeatherIcon = WeatherIcons.DAY_SUNNY;
                        break;
                    case 33: // Fair (Night)
                    case 34: // Fair (Day)
                    case 44: // Partly Cloudy
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                        else
                            WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
                        break;
                    case 36: // HOT
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                        else
                            WeatherIcon = WeatherIcons.DAY_HOT;
                        break;
                    case 3200: // Not Available
                    default:
                        WeatherIcon = WeatherIcons.NA;
                        break;
                }
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(Weather weather)
        {
            bool isNight = base.IsNight(weather);

            switch (weather.condition.icon)
            {
                // The following cases can be present at any time of day
                case WeatherIcons.CLOUDY:
                case WeatherIcons.SNOWFLAKE_COLD:
                case WeatherIcons.STRONG_WIND:
                    if (!isNight)
                    {
                        // Fallback to sunset/rise time just in case
                        var sunrise = NodaTime.LocalTime.FromTicksSinceMidnight(weather.astronomy.sunrise.TimeOfDay.Ticks);
                        var sunset = NodaTime.LocalTime.FromTicksSinceMidnight(weather.astronomy.sunset.TimeOfDay.Ticks);

                        var tz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
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
