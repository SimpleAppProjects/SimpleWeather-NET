using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Xml.Serialization;
#if WINDOWS_UWP
using SimpleWeather.UWP.Controls;
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

namespace SimpleWeather.WeatherUnderground
{
    public partial class WeatherUndergroundProvider : WeatherProviderImpl
    {
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => true;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;

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
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                locations = new ObservableCollection<LocationQueryViewModel>();
                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherUndergroundProvider: error getting locations");
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

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherUndergroundProvider: error getting location");
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
                AC_Rootobject root = JSONParser.Deserializer<AC_Rootobject>(contentStream);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherUndergroundProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.l))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<bool> IsKeyValid(string key)
        {
            string queryAPI = "https://api.wunderground.com/api/";
            string query = "/q/NY/New_York.json";
            Uri queryURL = new Uri(queryAPI + key + query);
            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                if (String.IsNullOrWhiteSpace(key))
                    throw (wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey));

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
                // Reset exception
                wEx = null;

                // End Stream
                webClient.Dispose();

                // Load data
                Rootobject root = await JSONParser.DeserializerAsync<Rootobject>(contentStream);

                // Check for errors
                if (root.response.error != null)
                {
                    switch (root.response.error.type)
                    {
                        case "keynotfound":
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                            isValid = false;
                            break;
                    }
                }
                else
                    isValid = true;

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception)
            {
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

        public override async Task<Weather> GetWeather(string location_query)
        {
            Weather weather = null;

            string queryAPI = null;
            Uri weatherURL = null;

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            queryAPI = "https://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day/hourly/alerts/lang:" + locale;
            string options = ".json";
            weatherURL = new Uri(queryAPI + location_query + options);

            HttpClient webClient = new HttpClient();
            WeatherException wEx = null;

            try
            {
                // Get response
                HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif
                // Reset exception
                wEx = null;

                // Load weather
                Rootobject root = null;
                await Task.Run(() =>
                {
                    root = JSONParser.Deserializer<Rootobject>(contentStream);
                });

                // Check for errors
                if (root.response.error != null)
                {
                    switch (root.response.error.type)
                    {
                        case "querynotfound":
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                            break;
                        case "keynotfound":
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                            break;
                        default:
                            break;
                    }
                }

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();

                weather = new Weather(root);

                // Add weather alerts if available
                if (root.alerts != null && root.alerts.Length > 0)
                {
                    if (weather.weather_alerts == null)
                        weather.weather_alerts = new List<WeatherAlert>();

                    foreach (Alert result in root.alerts)
                    {
                        weather.weather_alerts.Add(new WeatherAlert(result));
                    }
                }
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

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherUndergroundProvider: error getting weather data");
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

        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await base.GetWeather(location);

            // Just update hourly forecast dates to timezone
            var offset = location.tz_offset;

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                if (!hr_forecast.date.Offset.Equals(offset))
                    hr_forecast.date = new DateTimeOffset(hr_forecast.date.DateTime, offset);
            }

            // Update tz for weather alerts
            if (weather.weather_alerts != null && weather.weather_alerts.Count > 0)
            {
                foreach(WeatherAlert alert in weather.weather_alerts)
                {
                    if (!alert.Date.Offset.Equals(offset))
                    {
                        alert.Date = alert.Date.ToOffset(offset);
                    }

                    if (!alert.ExpiresDate.Offset.Equals(offset))
                    {
                        alert.ExpiresDate = alert.ExpiresDate.ToOffset(offset);
                    }
                }
            }

            return weather;
        }

        public override async Task<List<WeatherAlert>> GetAlerts(LocationData location)
        {
            List<WeatherAlert> alerts = null;

            string queryAPI = null;
            Uri weatherURL = null;

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            queryAPI = "https://api.wunderground.com/api/" + Settings.API_KEY + "/alerts/lang:" + locale;
            string options = ".json";
            weatherURL = new Uri(queryAPI + location.query + options);

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(weatherURL);
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
                alerts = new List<WeatherAlert>();

                AlertRootobject root = JSONParser.Deserializer<AlertRootobject>(contentStream);

                foreach (Alert result in root.alerts)
                {
                    alerts.Add(new WeatherAlert(result));
                }

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                alerts = new List<WeatherAlert>();
                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherUndergroundProvider: error getting weather alert data");
            }

            if (alerts == null)
                alerts = new List<WeatherAlert>();

            return alerts;
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
            string coord = string.Format("{0},{1}", weather.location.latitude, weather.location.longitude);
            var qview = await GetLocation(new WeatherUtils.Coordinate(coord));

            if (String.IsNullOrEmpty(qview.LocationQuery))
                query = string.Format("/q/{0}", coord);
            else
                query = qview.LocationQuery;

            return query;
        }

        public override async Task<string> UpdateLocationQuery(LocationData location)
        {
            string query = string.Empty;
            string coord = string.Format("{0},{1}", location.latitude, location.longitude);
            var qview = await GetLocation(new WeatherUtils.Coordinate(coord));

            if (String.IsNullOrEmpty(qview.LocationQuery))
                query = string.Format("/q/{0}", coord);
            else
                query = qview.LocationQuery;

            return query;
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            string code = "EN";

            switch (iso)
            {
                // Afrikaans
                case "af":
                // Arabic
                case "ar":
                // Armenian
                case "hy":
                // Azerbaijani
                case "az":
                // Basque
                case "eu":
                // Burmese
                case "my":
                // Catalan
                case "ca":
                // Dhivehi
                case "dv":
                // Dutch
                case "nl":
                // Esperanto
                case "eo":
                // Estonian
                case "et":
                // Farsi / Persian
                case "fa":
                // Finnish
                case "fi":
                // Georgian
                case "ka":
                // Gujarati
                case "gu":
                // Haitian Creole
                case "ht":
                // Hindi
                case "hi":
                // Hungarian
                case "hu":
                // Icelandic
                case "is":
                // Ido
                case "io":
                // Indonesian
                case "id":
                // Italian
                case "it":
                // Khmer
                case "km":
                // Kurdish
                case "ku":
                // Latin
                case "la":
                // Latvian
                case "lv":
                // Lithuanian
                case "lt":
                // Macedonian
                case "mk":
                // Maltese
                case "mt":
                // Maori
                case "mi":
                // Marathi
                case "mr":
                // Mongolian
                case "mn":
                // Norwegian
                case "no":
                // Occitan
                case "oc":
                // Pashto
                case "ps":
                // Polish
                case "pl":
                // Punjabi
                case "pa":
                // Romanian
                case "ro":
                // Russian
                case "ru":
                // Serbian
                case "sr":
                // Slovak
                case "sk":
                // Slovenian
                case "sl":
                // Tagalog
                case "tl":
                // Thai
                case "th":
                // Turkish
                case "tr":
                // Turkmen
                case "tk":
                // Uzbek
                case "uz":
                // Welsh
                case "cy":
                // Yiddish
                case "yi":
                    code = iso.ToUpper();
                    break;
                // Albanian
                case "sq":
                    code = "AL";
                    break;
                // Belarusian
                case "be":
                    code = "BY";
                    break;
                // Bulgarian
                case "bg":
                    code = "BU";
                    break;
                // English
                case "en":
                    // British English
                    if (name.Equals("en-GB"))
                        code = "LI";
                    else
                        code = "EN";
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
                            code = "TW";
                            break;
                        // Chinese - Simplified
                        default:
                            code = "CN";
                            break;
                    }
                    break;
                // Croatian
                case "hr":
                    code = "CR";
                    break;
                // Czech
                case "cs":
                    code = "CZ";
                    break;
                // Danish
                case "da":
                    code = "DK";
                    break;
                // French
                case "fr":
                    if (name.Equals("fr-CA"))
                        // French Canadian
                        code = "FC";
                    else
                        code = "FR";
                    break;
                // Galician
                case "gl":
                    code = "GZ";
                    break;
                // German
                case "de":
                    code = "DL";
                    break;
                // Greek
                case "el":
                    code = "GR";
                    break;
                // Hebrew
                case "he":
                    code = "IL";
                    break;
                // Irish
                case "ga":
                    code = "IR";
                    break;
                // Japanese
                case "ja":
                    code = "JP";
                    break;
                // Javanese
                case "jv":
                    code = "JW";
                    break;
                // Korean
                case "ko":
                    code = "KR";
                    break;
                // Portuguese
                case "pt":
                    code = "BR";
                    break;
                // Spanish
                case "es":
                    code = "SP";
                    break;
                // Swahili
                case "sw":
                    code = "SI";
                    break;
                // Swedish
                case "sv":
                    code = "SW";
                    break;
                // Swiss
                case "gsw":
                    code = "CH";
                    break;
                // Ukrainian
                case "uk":
                    code = "UA";
                    break;
                // Vietnamese
                case "vi":
                    code = "VU";
                    break;
                // Wolof
                case "wo":
                    code = "SN";
                    break;
                /*
                // Mandinka
                case "mandinka":
                    code = "GM";
                    break;
                // Plautdietsch
                case "plautdietsch":
                    code = "GN";
                    break;
                // Tatarish
                case "tatarish":
                    code = "TT";
                    break;
                // Yiddish - transliterated
                case "yiddish-tl":
                    code = "JI";
                    break;
                */
                default:
                    // Low German
                    if (name.Equals("nds") || name.StartsWith("nds-"))
                        code = "ND";
                    else
                        code = "EN";
                    break;
            }

            return code;
        }

        public override string GetWeatherIcon(string icon)
        {
            bool isNight = false;

            if (icon.Contains("nt_"))
                isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            if (icon.Contains("nt_mostlycloudy") || icon.Contains("nt_partlysunny") || icon.Contains("nt_cloudy"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY;
            else if (icon.Contains("nt_partlycloudy") || icon.Contains("nt_mostlysunny"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
            else if (icon.Contains("nt_clear") || icon.Contains("nt_sunny") || icon.Contains("nt_unknown"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;
            else if (icon.Contains("chancerain"))
                WeatherIcon = WeatherIcons.RAIN;
            else if (icon.Contains("clear") || icon.Contains("sunny"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;
            else if (icon.Contains("cloudy"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY;
            else if (icon.Contains("flurries"))
                WeatherIcon = WeatherIcons.SNOW_WIND;
            else if (icon.Contains("fog"))
                WeatherIcon = WeatherIcons.FOG;
            else if (icon.Contains("hazy"))
                if (isNight)
                    WeatherIcon = WeatherIcons.WINDY;
                else
                    WeatherIcon = WeatherIcons.DAY_HAZE;
            else if (icon.Contains("sleet") || icon.Contains("sleat"))
                WeatherIcon = WeatherIcons.SLEET;
            else if (icon.Contains("rain"))
                WeatherIcon = WeatherIcons.SHOWERS;
            else if (icon.Contains("snow"))
                WeatherIcon = WeatherIcons.SNOW;
            else if (icon.Contains("tstorms"))
                WeatherIcon = WeatherIcons.THUNDERSTORM;
            else if (icon.Contains("unknown") || icon.Contains("nt_"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }
    }
}