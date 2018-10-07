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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Web;
using Windows.Web.Http;
#elif __ANDROID__
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.Widget;
using System.Net;
using System.Net.Http;
#endif

namespace SimpleWeather.OpenWeather
{
    public partial class OpenWeatherMapProvider : WeatherProviderImpl
    {
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => true;
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
                contentStream.Dispose();
            }
            catch (Exception ex)
            {
                locations = new ObservableCollection<LocationQueryViewModel>();
                Logger.WriteLine(LoggerLevel.Error, ex, "OpenWeatherMapProvider: error getting locations");
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
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif

                // End Stream
                webClient.Dispose();

                // Load data
                XmlSerializer deserializer = new XmlSerializer(typeof(location));
                result = (location)deserializer.Deserialize(contentStream);

                // End Stream
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
                Logger.WriteLine(LoggerLevel.Error, ex, "OpenWeatherMapProvider: error getting location");
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
            AC_RESULT result;
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
                var root = JSONParser.Deserializer<AC_Rootobject>(contentStream);
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
                Logger.WriteLine(LoggerLevel.Error, ex, "OpenWeatherMapProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.l))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<bool> IsKeyValid(string key)
        {
            string queryAPI = "https://api.openweathermap.org/data/2.5/";
            string query = "forecast?appid=";
            Uri queryURL = new Uri(queryAPI + query + key);
            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                if (String.IsNullOrWhiteSpace(key))
                    throw (wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey));

                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);

                // Check for errors
                switch (response.StatusCode)
                {
                    // 400 (OK since this isn't a valid request)
                    case HttpStatusCode.BadRequest:
                        isValid = true;
                        break;
                    // 401 (Unauthorized - Key is invalid)
                    case HttpStatusCode.Unauthorized:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                        isValid = false;
                        break;
                }

                // End Stream
                response.Dispose();
                webClient.Dispose();
            }
            catch (Exception ex)
            {
#if WINDOWS_UWP
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
#elif __ANDROID__
                if (ex is WebException || ex is HttpRequestException)
#endif
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }

                isValid = false;
            }

            if (wEx != null)
            {
#if WINDOWS_UWP
                await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
#elif __ANDROID__
                new Android.OS.Handler(Android.OS.Looper.MainLooper).Post(() =>
                {
                    Toast.MakeText(Application.Context, wEx.Message, ToastLength.Short).Show();
                });
#endif
            }

            return isValid;
        }

        public override async Task<WeatherData.Weather> GetWeather(string location_query)
        {
            WeatherData.Weather weather = null;

            string currentAPI = null;
            Uri currentURL = null;
            string forecastAPI = null;
            Uri forecastURL = null;
            string query = null;

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            if (int.TryParse(location_query, out int id))
            {
                query = string.Format("id={0}", id);
            }
            else
            {
                query = location_query;
            }

            currentAPI = "https://api.openweathermap.org/data/2.5/weather?{0}&appid={1}&lang=" + locale;
            currentURL = new Uri(string.Format(currentAPI, query, Settings.API_KEY));
            forecastAPI = "https://api.openweathermap.org/data/2.5/forecast?{0}&appid={1}&lang=" + locale;
            forecastURL = new Uri(string.Format(forecastAPI, query, Settings.API_KEY));

            HttpClient webClient = new HttpClient();
            WeatherException wEx = null;

            try
            {
                // Get response
                HttpResponseMessage currentResponse = await webClient.GetAsync(currentURL);
                currentResponse.EnsureSuccessStatusCode();
                HttpResponseMessage forecastResponse = await webClient.GetAsync(forecastURL);
                forecastResponse.EnsureSuccessStatusCode();

                Stream currentStream = null;
                Stream forecastStream = null;
#if WINDOWS_UWP
                currentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await currentResponse.Content.ReadAsInputStreamAsync());
                forecastStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await forecastResponse.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                currentStream = await currentResponse.Content.ReadAsStreamAsync();
                forecastStream = await forecastResponse.Content.ReadAsStreamAsync();
#endif
                // Reset exception
                wEx = null;

                // Load weather
                CurrentRootobject currRoot = null;
                ForecastRootobject foreRoot = null;
                await Task.Run(() =>
                {
                    currRoot = JSONParser.Deserializer<CurrentRootobject>(currentStream);
                });
                await Task.Run(() =>
                {
                    foreRoot = JSONParser.Deserializer<ForecastRootobject>(forecastStream);
                });

                // End Streams
                currentStream.Dispose();
                forecastStream.Dispose();

                weather = new WeatherData.Weather(currRoot, foreRoot);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "OpenWeatherMapProvider: error getting weather data");
            }

            // End Stream
            webClient.Dispose();

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

        public override async Task<WeatherData.Weather> GetWeather(LocationData location)
        {
            var weather = await base.GetWeather(location);

            // OWM reports datetime in UTC; add location tz_offset
            var offset = location.tz_offset;
            weather.update_time = weather.update_time.ToOffset(offset);
            foreach(HourlyForecast hr_forecast in weather.hr_forecast)
            {
                hr_forecast.date =  hr_forecast.date.ToOffset(offset);
            }
            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date =  forecast.date.Add(offset);
            }
            weather.astronomy.sunrise = weather.astronomy.sunrise.Add(offset);
            weather.astronomy.sunset = weather.astronomy.sunset.Add(offset);

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
#if !__ANDROID_WEAR__
                await Settings.UpdateLocation(location);
#else
                Settings.SaveHomeData(location);
#endif
            }
        }

        public override async Task<string> UpdateLocationQuery(WeatherData.Weather weather)
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

        public override async Task<string> UpdateLocationQuery(WeatherData.LocationData location)
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

        public override String LocaleToLangCode(String iso, String name)
        {
            string code = "en";

            switch (iso)
            {
                // Arabic
                case "ar":
                // Bulgarian
                case "bg":
                // Catalan
                case "ca":
                // Croatian
                case "hr":
                // Dutch
                case "nl":
                // Farsi / Persian
                case "fa":
                // Finnish
                case "fi":
                // French
                case "fr":
                // Galician
                case "gl":
                // German
                case "de":
                // Greek
                case "el":
                // Hungarian
                case "hu":
                // Italian
                case "it":
                // Japanese
                case "ja":
                // Lithuanian
                case "lt":
                // Macedonian
                case "mk":
                // Polish
                case "pl":
                // Portuguese
                case "pt":
                // Romanian
                case "ro":
                // Russian
                case "ru":
                // Slovak
                case "sk":
                // Slovenian
                case "sl":
                // Spanish
                case "es":
                // Turkish
                case "tr":
                // Vietnamese
                case "vi":
                    code = iso;
                    break;
                // Chinese
                case "zh":
                    switch (name)
                    {
                        // Chinese - Traditional
                        case "zh-Hant":
                        case "zh-HK":
                        case "zh-MO":
                        case "zh-TW":
                            code = "zh_tw";
                            break;
                        // Chinese - Simplified
                        default:
                            code = "zh_cn";
                            break;
                    }
                    break;
                // Czech
                case "cs":
                    code = "cz";
                    break;
                // Korean
                case "ko":
                    code = "kr";
                    break;
                // Latvian
                case "lv":
                    code = "la";
                    break;
                // Swedish
                case "sv":
                    code = "se";
                    break;
                // Ukrainian
                case "uk":
                    code = "ua";
                    break;
                default:
                    // Default is English
                    code = "en";
                    break;
            }

            return code;
        }

        public override string GetWeatherIcon(string icon)
        {
            bool isNight = false;

            if (icon.EndsWith("n"))
                isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            switch (icon.Substring(0,3))
            {
                case "200": // thunderstorm w/ light rain
                case "201": // thunderstorm w/ rain
                case "210": // light thunderstorm
                case "230": // thunderstorm w/ light drizzle
                case "231": // thunderstorm w/ drizzle
                    WeatherIcon = WeatherIcons.STORM_SHOWERS;
                    break;

                case "211": // thunderstorm
                case "212": // heavy thunderstorm
                case "221": // ragged thunderstorm
                case "202": // thunderstorm w/ heavy rain
                case "232": // thunderstorm w/ heavy drizzle
                    WeatherIcon = WeatherIcons.THUNDERSTORM;
                    break;

                case "300": // light intensity drizzle
                case "301": // drizzle
                case "321": // shower drizzle
                    WeatherIcon = WeatherIcons.SPRINKLE;
                    break;

                case "302": // heavy intensity drizzle
                case "311": // drizzle rain
                case "312": // heavy intensity drizzle rain
                case "314": // heavy shower rain and drizzle
                    WeatherIcon = WeatherIcons.RAIN;
                    break;

                case "500": // light rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SPRINKLE;
                    else
                        WeatherIcon = WeatherIcons.DAY_SPRINKLE;
                    break;

                case "501": // moderate rain
                case "502": // heavy intensity rain
                case "503": // very heavy rain
                case "504": // extreme rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN;
                    else
                        WeatherIcon = WeatherIcons.DAY_RAIN;
                    break;

                case "310": // light intensity drizzle rain
                case "511": // freezing rain
                case "611": // sleet
                case "612": // shower sleet
                case "615": // light rain and snow
                case "616": // rain and snow
                case "620": // light shower snow
                    WeatherIcon = WeatherIcons.RAIN_MIX;
                    break;

                case "313": // shower rain and drizzle
                case "520": // light intensity shower rain
                case "521": // shower rain
                case "522": // heavy intensity shower rain
                case "701": // mist
                    WeatherIcon = WeatherIcons.SHOWERS;
                    break;

                case "531": // ragged shower rain
                case "901": // tropical storm
                    WeatherIcon = WeatherIcons.STORM_SHOWERS;
                    break;

                case "600": // light snow
                case "601": // snow
                case "621": // shower snow
                case "622": // heavy shower snow
                    WeatherIcon = WeatherIcons.SNOW;
                    break;

                case "602": // heavy snow
                    WeatherIcon = WeatherIcons.SNOW_WIND;
                    break;

                // smoke
                case "711":
                    WeatherIcon = WeatherIcons.SMOKE;
                    break;

                // haze
                case "721":
                    if (isNight)
                        WeatherIcon = WeatherIcons.WINDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_HAZE;
                    break;

                // dust
                case "731":
                case "761":
                case "762":
                    WeatherIcon = WeatherIcons.DUST;
                    break;

                // fog
                case "741":
                    WeatherIcon = WeatherIcons.FOG;
                    break;

                // cloudy-gusts
                case "771": // squalls
                    WeatherIcon = WeatherIcons.CLOUDY_GUSTS;
                    break;

                // tornado
                case "781":
                case "900":
                    WeatherIcon = WeatherIcons.TORNADO;
                    break;

                // day-sunny
                case "800": // clear sky
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY;
                    break;

                // cloudy-gusts
                case "801": // few clouds
                case "802": // scattered clouds
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS;
                    else
                        WeatherIcon = WeatherIcons.DAY_CLOUDY_GUSTS;
                    break;

                // cloudy-gusts
                case "803": // broken clouds
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY_WINDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_CLOUDY_WINDY;
                    break;

                // cloudy
                case "804": // overcast clouds
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_CLOUDY;
                    break;

                // hurricane
                case "902":
                    WeatherIcon = WeatherIcons.HURRICANE;
                    break;

                // cold
                case "903":
                    WeatherIcon = WeatherIcons.SNOWFLAKE_COLD;
                    break;

                // hot
                case "904":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                    else
                        WeatherIcon = WeatherIcons.DAY_HOT;
                    break;

                // windy
                case "905":
                    WeatherIcon = WeatherIcons.WINDY;
                    break;

                // hail
                case "906":
                    WeatherIcon = WeatherIcons.HAIL;
                    break;

                // strong wind
                case "957":
                    WeatherIcon = WeatherIcons.STRONG_WIND;
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
    }
}