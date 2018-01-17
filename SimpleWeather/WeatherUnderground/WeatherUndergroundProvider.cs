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
using System.Diagnostics;
#if WINDOWS_UWP
using SimpleWeather.UWP.Controls;
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

namespace SimpleWeather.WeatherUnderground
{
    public partial class WeatherUndergroundProvider : WeatherProviderImpl
    {
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => true;
        public override bool NeedsExternalLocationData => false;

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
                if (contentStream != null)
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
            string queryAPI = "http://api.wunderground.com/api/";
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
                    Android.Widget.Toast.MakeText(Droid.App.Context, wEx.Message, Android.Widget.ToastLength.Short).Show();
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

            queryAPI = "http://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day/hourly/lang:" + locale;
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
            string WeatherIcon = string.Empty;

            if (icon.Contains("nt_mostlycloudy") || icon.Contains("nt_partlysunny") || icon.Contains("nt_cloudy"))
                WeatherIcon = "\uf031";
            else if (icon.Contains("nt_partlycloudy") || icon.Contains("nt_mostlysunny"))
                WeatherIcon = "\uf083";
            else if (icon.Contains("nt_clear") || icon.Contains("nt_sunny") || icon.Contains("nt_unknown"))
                WeatherIcon = "\uf02e";
            else if (icon.Contains("chancerain"))
                WeatherIcon = "\uf019";
            else if (icon.Contains("clear") || icon.Contains("sunny"))
                WeatherIcon = "\uf00d";
            else if (icon.Contains("cloudy"))
                WeatherIcon = "\uf002";
            else if (icon.Contains("flurries"))
                WeatherIcon = "\uf064";
            else if (icon.Contains("fog"))
                WeatherIcon = "\uf014";
            else if (icon.Contains("hazy"))
                WeatherIcon = "\uf021";
            else if (icon.Contains("sleet") || icon.Contains("sleat"))
                WeatherIcon = "\uf0b5";
            else if (icon.Contains("rain"))
                WeatherIcon = "\uf01a";
            else if (icon.Contains("snow"))
                WeatherIcon = "\uf01b";
            else if (icon.Contains("tstorms"))
                WeatherIcon = "\uf01e";
            else if (icon.Contains("unknown"))
                WeatherIcon = "\uf00d";
            else if (icon.Contains("nt_"))
                WeatherIcon = "\uf02e";

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = "\uf07b";
            }

            return WeatherIcon;
        }

        public override bool IsNight(Weather weather)
        {
            bool isNight = false;

            if (weather.condition.icon.StartsWith("nt_"))
                isNight = true;

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
        public override Color GetWeatherBackgroundColor(Weather weather)
#elif __ANDROID__
        public override Color GetWeatherBackgroundColor(Weather weather)
#endif
        {
            byte[] rgb = null;
            String icon = weather.condition.icon;

            // Apply background based on weather condition
            /* WeatherUnderground */
            if (icon.Contains("mostly") || icon.Contains("partly") ||
                icon.Contains("cloudy"))
            {
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
            }
            else if (icon.Contains("rain") || icon.Contains("sleet") || icon.Contains("sleat") ||
                     icon.Contains("flurries") || icon.Contains("snow") || icon.Contains("tstorms"))
            {
                // lighter than night color + cloudiness
                rgb = new byte[3] { 53, 67, 116 };
            }
            else if (icon.Contains("hazy") || icon.Contains("fog"))
            {
                // add haziness
                rgb = new byte[3] { 143, 163, 196 };
            }
            else if (icon.Contains("clear") || icon.Contains("sunny"))
            {
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
            }

            // Just in case
            if (rgb == null)
            {
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
            }

#if WINDOWS_UWP
            return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
#elif __ANDROID__
            return new Color(rgb[0], rgb[1], rgb[2]);
#endif
        }
    }
}