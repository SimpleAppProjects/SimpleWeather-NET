using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Xml.Serialization;
#if WINDOWS_UWP
using SimpleWeather.UWP.Controls;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
#elif __ANDROID__
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.Widget;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
#endif

namespace SimpleWeather.Metno
{
    public partial class MetnoWeatherProvider : WeatherProviderImpl
    {
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => true;

        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string query)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            string queryAPI = "https://autocomplete.wunderground.com/aq?query=";
            string options = "&h=0&cities=1";
            Uri queryURL = new Uri(queryAPI + query + options);
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif
                // End Stream
                webClient.Dispose();

                // Load data
                locations = new ObservableCollection<LocationQueryViewModel>();

                var root = JSONParser.Deserializer<OpenWeather.AC_Rootobject>(contentStream);

                foreach (OpenWeather.AC_RESULT result in root.RESULTS)
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
                Logger.WriteLine(LoggerLevel.Error, ex, "MetnoWeatherProvider: error getting locations");
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
            OpenWeather.location result;
            WeatherException wEx = null;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif

                // End Stream
                webClient.Dispose();

                // Load data
                XmlSerializer deserializer = new XmlSerializer(typeof(OpenWeather.location));
                result = (OpenWeather.location)deserializer.Deserialize(contentStream);

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                result = null;
#if WINDOWS_UWP
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                }
#elif __ANDROID__
                if (ex is WebException || ex is HttpRequestException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    new Android.OS.Handler(Android.OS.Looper.MainLooper).Post(() =>
                    {
                        Toast.MakeText(Application.Context, wEx.Message, ToastLength.Short).Show();
                    });
                }
#endif
                Logger.WriteLine(LoggerLevel.Error, ex, "MetnoWeatherProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.query))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<LocationQueryViewModel> GetLocation(string query)
        {
            LocationQueryViewModel location = null;

            string queryAPI = "https://autocomplete.wunderground.com/aq?query=";
            string options = "&h=0&cities=1";
            Uri queryURL = new Uri(queryAPI + query + options);
            OpenWeather.AC_RESULT result;
            WeatherException wEx = null;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif

                // End Stream
                webClient.Dispose();

                // Load data
                var root = JSONParser.Deserializer<OpenWeather.AC_Rootobject>(contentStream);
                result = root.RESULTS.FirstOrDefault();

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                result = null;
#if WINDOWS_UWP
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                }
#elif __ANDROID__
                if (ex is WebException || ex is HttpRequestException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    new Android.OS.Handler(Android.OS.Looper.MainLooper).Post(() =>
                    {
                        Toast.MakeText(Application.Context, wEx.Message, ToastLength.Short).Show();
                    });
                }
#endif
                Logger.WriteLine(LoggerLevel.Error, ex, "MetnoWeatherProvider: error getting location");
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

            string forecastAPI = null;
            Uri forecastURL = null;
            string sunrisesetAPI = null;
            Uri sunrisesetURL = null;

            forecastAPI = "https://api.met.no/weatherapi/locationforecastlts/1.3/?{0}";
            forecastURL = new Uri(string.Format(forecastAPI, location_query));
            sunrisesetAPI = "https://api.met.no/weatherapi/sunrise/1.1/?{0}&date={1}";
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            sunrisesetURL = new Uri(string.Format(sunrisesetAPI, location_query, date));

#if WINDOWS_UWP
            var handler = new HttpBaseProtocolFilter()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = true
            };
#else
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
#endif

            HttpClient webClient = new HttpClient(handler);

            // Use GZIP compression
#if WINDOWS_UWP
            var version = string.Format("v{0}.{1}.{2}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

            webClient.DefaultRequestHeaders.AcceptEncoding.Add(new HttpContentCodingWithQualityHeaderValue("gzip"));
            webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));
#elif __ANDROID__
            var packageInfo = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0);
            var version = string.Format("v{0}", packageInfo.VersionName);

            webClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            webClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", String.Format("SimpleWeather (thewizrd.dev@gmail.com) {0}", version));
#endif

            WeatherException wEx = null;

            try
            {
                // Get response
                HttpResponseMessage forecastResponse = await webClient.GetAsync(forecastURL);
                forecastResponse.EnsureSuccessStatusCode();
                HttpResponseMessage sunrisesetResponse = await webClient.GetAsync(sunrisesetURL);
                sunrisesetResponse.EnsureSuccessStatusCode();

                Stream forecastStream = null;
                Stream sunrisesetStream = null;
#if WINDOWS_UWP
                forecastStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await forecastResponse.Content.ReadAsInputStreamAsync());
                sunrisesetStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await sunrisesetResponse.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                forecastStream = await forecastResponse.Content.ReadAsStreamAsync();
                sunrisesetStream = await sunrisesetResponse.Content.ReadAsStreamAsync();
#endif
                // Reset exception
                wEx = null;

                // Load weather
                weatherdata foreRoot = null;
                astrodata astroRoot = null;

                XmlSerializer foreDeserializer = new XmlSerializer(typeof(weatherdata));
                foreRoot = (weatherdata)foreDeserializer.Deserialize(forecastStream);
                XmlSerializer astroDeserializer = new XmlSerializer(typeof(astrodata));
                astroRoot = (astrodata)astroDeserializer.Deserialize(sunrisesetStream);

                weather = new Weather(foreRoot, astroRoot);
            }
            catch (Exception ex)
            {
                weather = null;
#if WINDOWS_UWP
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
#elif __ANDROID__
                if (ex is WebException || ex is HttpRequestException)
#endif
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "MetnoWeatherProvider: error getting weather data");
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

                foreach (Forecast forecast in weather.forecast)
                {
                    forecast.condition = GetWeatherCondition(forecast.icon);
                    forecast.icon = GetWeatherIcon(forecast.icon);
                }
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await base.GetWeather(location);

            // OWM reports datetime in UTC; add location tz_offset
            var offset = location.tz_offset;
            weather.update_time = weather.update_time.ToOffset(offset);

            // The time of day is set to max if the sun never sets/rises and
            // DateTime is set to min if not found
            // Don't change this if its set that way
            if (weather.astronomy.sunrise > DateTime.MinValue &&
                weather.astronomy.sunrise.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                weather.astronomy.sunrise = weather.astronomy.sunrise.Add(offset);
            if (weather.astronomy.sunset > DateTime.MinValue &&
                weather.astronomy.sunset.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                weather.astronomy.sunset = weather.astronomy.sunset.Add(offset);

            // Set condition here
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            weather.condition.weather = GetWeatherCondition(weather.condition.icon);
            weather.condition.icon = GetWeatherIcon(now < sunrise || now > sunset, weather.condition.icon);

            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
            }

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                hr_forecast.date = hr_forecast.date.ToOffset(offset);

                var hrnow = hr_forecast.date.TimeOfDay;
                var sunriseTime = weather.astronomy.sunrise.TimeOfDay;
                var sunsetTime = weather.astronomy.sunset.TimeOfDay;

                hr_forecast.condition = GetWeatherCondition(hr_forecast.icon);
                hr_forecast.icon = GetWeatherIcon(hrnow < sunriseTime || hrnow > sunsetTime, hr_forecast.icon);
            }

            return weather;
        }

        // TODO: Move this out
        public static string GetWeatherCondition(string icon)
        {
            string condition = String.Empty;

            switch (icon)
            {
                case "1": // Sun
                    condition = "Clear";
                    break;
                case "2": // LightCloud
                case "3": // PartlyCloud
                    condition = "Partly Cloudy";
                    break;
                case "4": // Cloud
                    condition = "Mostly Cloudy";
                    break;
                case "5": // LightRainSun
                case "6": // LightRainThunderSun
                case "9": // LightRain
                case "22": // LightRainThunder
                    condition = "Light Rain";
                    break;
                case "7": // SleetSun
                case "12": // Sleet
                case "20": // SleetSunThunder
                case "23": // SleetThunder
                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                case "47": // LightSleet
                case "48": // HeavySleet
                    condition = "Sleet";
                    break;
                case "8": // SnowSun
                case "13": // Snow
                case "14": // SnowThunder
                case "21": // SnowSunThunder
                    condition = "Snow";
                    break;
                case "10": // Rain
                case "11": // RainThunder
                case "25": // RainThunderSun
                case "41": // RainSun
                    condition = "Rain";
                    break;
                case "15": // Fog
                    condition = "Fog";
                    break;
                case "24": // DrizzleThunderSun
                case "30": // DrizzleThunder
                case "40": // DrizzleSun
                case "46": // Drizzle
                    condition = "Drizzle";
                    break;
                case "28": // LightSnowThunderSun
                case "33": // LightSnowThunder
                case "44": // LightSnowSun
                case "49": // LightSnow
                    condition = "Light Snow";
                    break;
                case "29": // HeavySnowThunderSun
                case "34": // HeavySnowThunder
                case "45": // HeavySnowSun
                case "50": // HeavySnow
                    condition = "Heavy Snow";
                    break;
                default:
                    condition = Weather.NA;
                    break;
            }

            return condition;
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
#if !__ANDROID_WEAR__
                await Settings.UpdateLocation(location);
#else
                Settings.SaveHomeData(location);
#endif
            }
        }

        public override async Task<string> UpdateLocationQuery(Weather weather)
        {
            string query = string.Empty;
            var coord = new WeatherUtils.Coordinate(double.Parse(weather.location.latitude), double.Parse(weather.location.longitude));
            var qview = await GetLocation(coord);

            if (String.IsNullOrEmpty(qview.LocationQuery))
                query = string.Format("lat={0}&lon={1}", coord.Latitude, coord.Longitude);
            else
                query = qview.LocationQuery;

            return query;
        }

        public override async Task<string> UpdateLocationQuery(LocationData location)
        {
            string query = string.Empty;
            var coord = new WeatherUtils.Coordinate(location.latitude, location.longitude);
            var qview = await GetLocation(coord);

            if (String.IsNullOrEmpty(qview.LocationQuery))
                query = string.Format("lat={0}&lon={1}", coord.Latitude, coord.Longitude);
            else
                query = qview.LocationQuery;

            return query;
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        // Needed b/c icons don't show whether night or not
        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            switch (icon)
            {
                case "1": // Sun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY;
                    break;

                case "2": // LightCloud
                case "3": // PartlyCloud
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
                    break;

                case "4": // Cloud
                    WeatherIcon = WeatherIcons.CLOUDY;
                    break;

                case "5": // LightRainSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SPRINKLE;
                    else
                        WeatherIcon = WeatherIcons.DAY_SPRINKLE;
                    break;

                case "6": // LightRainThunderSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_THUNDERSTORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_THUNDERSTORM;
                    break;

                case "7": // SleetSun
                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SLEET;
                    else
                        WeatherIcon = WeatherIcons.DAY_SLEET;
                    break;

                case "8": // SnowSun
                case "44": // LightSnowSun
                case "45": // HeavySnowSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW;
                    else
                        WeatherIcon = WeatherIcons.DAY_SNOW;
                    break;

                case "9": // LightRain
                case "46": // Drizzle
                    WeatherIcon = WeatherIcons.SPRINKLE;
                    break;

                case "10": // Rain
                    WeatherIcon = WeatherIcons.RAIN;
                    break;

                case "11": // RainThunder
                    WeatherIcon = WeatherIcons.THUNDERSTORM;
                    break;

                case "12": // Sleet
                case "47": // LightSleet
                case "48": // HeavySleet
                    WeatherIcon = WeatherIcons.SLEET;
                    break;

                case "13": // Snow
                case "49": // LightSnow
                    WeatherIcon = WeatherIcons.SNOW;
                    break;

                case "14": // SnowThunder
                case "21": // SnowSunThunder
                case "28": // LightSnowThunderSun
                case "29": // HeavySnowThunderSun
                case "33": // LightSnowThunder
                case "34": // HeavySnowThunder
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_SNOW_THUNDERSTORM;
                    break;

                case "15": // Fog
                    WeatherIcon = WeatherIcons.FOG;
                    break;

                case "20": // SleetSunThunder
                case "23": // SleetThunder
                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SLEET_STORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_SLEET_STORM;
                    break;

                case "22": // LightRainThunder
                case "30": // DrizzleThunder
                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_STORM_SHOWERS;
                    else
                        WeatherIcon = WeatherIcons.DAY_STORM_SHOWERS;
                    break;

                case "40": // DrizzleSun
                case "41": // RainSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN;
                    else
                        WeatherIcon = WeatherIcons.DAY_RAIN;
                    break;

                case "50": // HeavySnow
                    WeatherIcon = WeatherIcons.SNOW_WIND;
                    break;

                default:
                    break;
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        // Met.no conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(Weather weather)
        {
            bool isNight = base.IsNight(weather);

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

            return isNight;
        }
    }
}