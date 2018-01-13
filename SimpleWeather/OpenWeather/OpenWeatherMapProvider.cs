using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Diagnostics;
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
using SimpleWeather.Droid;
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
        public override bool NeedsExternalLocationData => true;

        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string query)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            string queryAPI = "http://autocomplete.wunderground.com/aq?query=";
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

                AC_Rootobject root = JSONParser.Deserializer<AC_Rootobject>(contentStream);

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
                Debug.WriteLine(ex.StackTrace);
            }

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };

            return locations;
        }

        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord)
        {
            LocationQueryViewModel location = null;

            string queryAPI = "http://api.wunderground.com/auto/wui/geo/GeoLookupXML/index.xml?query=";
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
                        Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
                    });
                }
#endif

                Debug.WriteLine(ex.StackTrace);
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.query))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<bool> IsKeyValid(string key)
        {
            string queryAPI = "http://api.openweathermap.org/data/2.5/";
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
                var responseCode = response.StatusCode;

                // Check for errors
                switch (responseCode)
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
                    Android.Widget.Toast.MakeText(Droid.App.Context, wEx.Message, Android.Widget.ToastLength.Short).Show();
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

            currentAPI = "http://api.openweathermap.org/data/2.5/weather?{0}&appid={1}&lang=" + locale;
            currentURL = new Uri(string.Format(currentAPI, query, Settings.API_KEY));
            forecastAPI = "http://api.openweathermap.org/data/2.5/forecast?{0}&appid={1}&lang=" + locale;
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

                Debug.WriteLine(ex.StackTrace);
            }

            // End Stream
            webClient.Dispose();

            if (weather == null)
            {
                if (wEx == null)
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);

#if WINDOWS_UWP
                await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
#elif __ANDROID__
                Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
#endif
            }
            else
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
            weather.update_time = weather.update_time.ToOffset(location.tz_offset);
            foreach(HourlyForecast hr_forecast in weather.hr_forecast)
            {
                hr_forecast.date =  hr_forecast.date.Add(location.tz_offset);
            }
            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date =  forecast.date.Add(location.tz_offset);
            }
            weather.astronomy.sunrise = weather.astronomy.sunrise.Add(location.tz_offset);
            weather.astronomy.sunset = weather.astronomy.sunset.Add(location.tz_offset);

            return weather;
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
            string WeatherIcon = string.Empty;

            switch (icon)
            {
                // thunderstorm
                case "200":
                case "201":
                case "202":
                case "230":
                case "231":
                case "232":
                    WeatherIcon = "\uf01e";
                    break;

                // lightning
                case "210":
                case "211":
                case "212":
                case "221":
                    WeatherIcon = "\uf016";
                    break;

                // drizzle
                case "300":
                case "301":
                case "321":
                case "500":
                    WeatherIcon = "\uf01c";
                    break;

                // rain
                case "302":
                case "311":
                case "312":
                case "314":
                case "501":
                case "502":
                case "503":
                case "504":
                    WeatherIcon = "\uf019";
                    break;

                // rain-mix
                case "310":
                case "511":
                case "611":
                case "612":
                case "615":
                case "616":
                case "620":
                    WeatherIcon = "\uf017";
                    break;

                // rain showers
                case "313":
                case "520":
                case "521":
                case "522":
                case "701":
                    WeatherIcon = "\uf01a";
                    break;

                // storm-showers
                case "531":
                case "901":
                    WeatherIcon = "\uf01d";
                    break;

                // snow
                case "600":
                case "601":
                case "621":
                case "622":
                    WeatherIcon = "\uf01b";
                    break;

                // sleet
                case "602":
                    WeatherIcon = "\uf0b5";
                    break;

                // smoke
                case "711":
                    WeatherIcon = "\uf062";
                    break;

                // haze
                case "721":
                    WeatherIcon = "\uf0b6";
                    break;

                // dust
                case "731":
                case "761":
                case "762":
                    WeatherIcon = "\uf063";
                    break;

                // fog
                case "741":
                    WeatherIcon = "\uf014";
                    break;
                
                // cloudy-gusts
                case "771":
                case "801":
                case "802":
                    WeatherIcon = "\uf011";
                    break;

                // tornado
                case "781":
                case "900":
                    WeatherIcon = "\uf056";
                    break;

                // day-sunny
                case "800":
                    WeatherIcon = "\uf00d";
                    break;

                // cloudy-gusts
                case "803":
                    WeatherIcon = "\uf012";
                    break;
                
                // cloudy
                case "804":
                    WeatherIcon = "\uf013";
                    break;

                // hurricane
                case "902":
                    WeatherIcon = "\uf073";
                    break;

                // cold
                case "903":
                    WeatherIcon = "\uf076";
                    break;

                // hot
                case "904":
                    WeatherIcon = "\uf072";
                    break;

                // windy
                case "905":
                    WeatherIcon = "\uf021";
                    break;

                // hail
                case "906":
                    WeatherIcon = "\uf015";
                    break;

                // strong wind
                case "957":
                    WeatherIcon = "\uf015";
                    break;
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = "\uf07b";
            }

            return WeatherIcon;
        }

        public override bool IsNight(WeatherData.Weather weather)
        {
            bool isNight = false;

            if (!isNight)
            {
                // Fallback to sunset/rise time just in case
                TimeSpan sunrise = weather.astronomy.sunrise.TimeOfDay;
                TimeSpan sunset = weather.astronomy.sunset.TimeOfDay;
                TimeSpan now = DateTimeOffset.UtcNow.ToOffset(weather.location.tz_offset).TimeOfDay;

                // Determine whether its night using sunset/rise times
                if (now < sunrise || now > sunset)
                    isNight = true;
            }

            return isNight;
        }

#if WINDOWS_UWP
        public override Color GetWeatherBackgroundColor(WeatherData.Weather weather)
#elif __ANDROID__
        public override Color GetWeatherBackgroundColor(WeatherData.Weather weather)
#endif
        {
            byte[] rgb = null;
            String icon = weather.condition.icon;

            // Apply background based on weather condition
            switch (icon)
            {
                // thunderstorm
                case "200":
                case "201":
                case "202":
                case "230":
                case "231":
                case "232":
                // lightning
                case "210":
                case "211":
                case "212":
                case "221":
                // drizzle
                case "300":
                case "301":
                case "321":
                case "500":
                // rain
                case "302":
                case "311":
                case "312":
                case "314":
                case "501":
                case "502":
                case "503":
                case "504":
                // rain-mix
                case "310":
                case "511":
                case "611":
                case "612":
                case "615":
                case "616":
                case "620":
                // rain showers
                case "313":
                case "520":
                case "521":
                case "522":
                case "701":
                // storm-showers
                case "531":
                case "901":
                // snow
                case "600":
                case "601":
                case "621":
                case "622":
                // sleet
                case "602":
                // tornado
                case "781":
                case "900":
                // hurricane
                case "902":
                // hail
                case "906":
                    // lighter than night color + cloudiness
                    rgb = new byte[3] { 53, 67, 116 };
                    break;

                // smoke
                case "711":
                // haze
                case "721":
                // dust
                case "731":
                case "761":
                case "762":
                // fog
                case "741":
                    // add haziness
                    rgb = new byte[3] { 143, 163, 196 };
                    break;

                // cloudy-gusts
                case "771":
                case "801":
                case "802":
                // cloudy-gusts
                case "803":
                // cloudy
                case "804":
                    if (IsNight(weather))
                    {
                        // Add night background plus cloudiness
                        rgb = new byte[3] { 16, 37, 67 };
                    }
                    else
                    {
                        // add day bg + cloudiness
                        rgb = new byte[3] { 119, 148, 196 };
                    }
                    break;

                // day-sunny
                case "800":
                // cold
                case "903":
                // hot
                case "904":
                // windy
                case "905":
                // strong wind
                case "957":
                default:
                    // Set background based using sunset/rise times
                    if (IsNight(weather))
                    {
                        // Night background
                        rgb = new byte[3] { 26, 36, 74 };
                    }
                    else
                    {
                        // set day bg
                        rgb = new byte[3] { 72, 116, 191 };
                    }
                    break;
            }

#if WINDOWS_UWP
            return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
#elif __ANDROID__
            return new Color(rgb[0], rgb[1], rgb[2]);
#endif
        }
    }
}