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
#if WINDOWS_UWP
using SimpleWeather.UWP.Controls;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Web;
using Windows.Web.Http;
#elif __ANDROID__
using Android.Graphics;
using Android.Widget;
using System.Net;
using System.Net.Http;
using SimpleWeather.Droid;
#endif

namespace SimpleWeather.WeatherYahoo
{
    public partial class YahooWeatherProvider : WeatherProviderImpl
    {
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;
        public override bool NeedsExternalLocationData => false;

        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string location_query)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
            string query = "select * from geo.places where text=\"" + location_query + "*\"";
            Uri queryURL = new Uri(yahooAPI + query);
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
                XmlSerializer deserializer = new XmlSerializer(typeof(query));
                query root = (query)deserializer.Deserialize(contentStream);

                foreach (place result in root.results)
                {
                    // Filter: only store city results
                    if (result.placeTypeName.Value == "Town"
                        || result.placeTypeName.Value == "Suburb"
                        || (result.placeTypeName.Value == "Zip Code"
                        || result.placeTypeName.Value == "Postal Code" &&
                            (result.locality1 != null && result.locality1.type == "Town")
                            || (result.locality1 != null && result.locality1.type == "Suburb")))
                        locations.Add(new LocationQueryViewModel(result));
                    else
                        continue;

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
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };

            return locations;
        }

        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord)
        {
            LocationQueryViewModel location = null;

            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
            string location_query = string.Format("({0},{1})", coord.Latitude, coord.Longitude);
            string query = "select * from geo.places where text=\"" + location_query + "\"";
            Uri queryURL = new Uri(yahooAPI + query);
            place result = null;
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
                XmlSerializer deserializer = new XmlSerializer(typeof(query));
                query root = (query)deserializer.Deserialize(contentStream);

                if (root.results != null)
                    result = root.results[0];

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                result = null;
#if WINDOWS_UWP
                if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                }
#elif __ANDROID__
                if (ex is System.Net.WebException || ex is HttpRequestException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    new Android.OS.Handler(Android.OS.Looper.MainLooper).Post(() =>
                    {
                        Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
                    });
                }
#endif
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.woeid))
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

            string queryAPI = null;
            Uri weatherURL = null;

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            if (int.TryParse(location_query, out int woeid))
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid=\""
                    + woeid + "\" and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);
            }
            else
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=\""
                    + location_query + "\") and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);
            }

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

                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
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
                query = string.Format("({0})", coord);
            else
                query = qview.LocationQuery;

            return query;
        }

        public override string GetWeatherIcon(string icon)
        {
            string WeatherIcon = string.Empty;

            if (int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    case 0: // Tornado
                        WeatherIcon = "\uf056";
                        break;
                    case 1: // Tropical Storm
                    case 37:
                    case 38: // Scattered Thunderstorms/showers
                    case 39:
                    case 45:
                    case 47:
                        WeatherIcon = "\uf00e";
                        break;
                    case 2: // Hurricane
                        WeatherIcon = "\uf073";
                        break;
                    case 3:
                    case 4: // Scattered Thunderstorms
                        WeatherIcon = "\uf01e";
                        break;
                    case 5: // Mixed Rain/Snow
                    case 6: // Mixed Rain/Sleet
                    case 7: // Mixed Snow/Sleet
                    case 18: // Sleet
                    case 35: // Mixed Rain/Hail
                        WeatherIcon = "\uf017";
                        break;
                    case 8: // Freezing Drizzle
                    case 10: // Freezing Rain
                    case 17: // Hail
                        WeatherIcon = "\uf015";
                        break;
                    case 9: // Drizzle
                    case 11: // Showers
                    case 12:
                    case 40: // Scattered Showers
                        WeatherIcon = "\uf01a";
                        break;
                    case 13: // Snow Flurries
                    case 14: // Light Snow Showers
                    case 16: // Snow
                    case 42: // Scattered Snow Showers
                    case 46: // Snow Showers
                        WeatherIcon = "\uf01b";
                        break;
                    case 15: // Blowing Snow
                    case 41: // Heavy Snow
                    case 43:
                        WeatherIcon = "\uf064";
                        break;
                    case 19: // Dust
                        WeatherIcon = "\uf063";
                        break;
                    case 20: // Foggy
                        WeatherIcon = "\uf014";
                        break;
                    case 21: // Haze
                        WeatherIcon = "\uf021";
                        break;
                    case 22: // Smoky
                        WeatherIcon = "\uf062";
                        break;
                    case 23: // Blustery
                    case 24: // Windy
                        WeatherIcon = "\uf050";
                        break;
                    case 25: // Cold
                        WeatherIcon = "\uf076";
                        break;
                    case 26: // Cloudy
                        WeatherIcon = "\uf013";
                        break;
                    case 27: // Mostly Cloudy (Night)
                    case 29: // Partly Cloudy (Night)
                        WeatherIcon = "\uf031";
                        break;
                    case 28: // Mostly Cloudy (Day)
                    case 30: // Partly Cloudy (Day)
                        WeatherIcon = "\uf002";
                        break;
                    case 31: // Clear (Night)
                        WeatherIcon = "\uf02e";
                        break;
                    case 32: // Sunny
                        WeatherIcon = "\uf00d";
                        break;
                    case 33: // Fair (Night)
                        WeatherIcon = "\uf083";
                        break;
                    case 34: // Fair (Day)
                    case 44: // Partly Cloudy
                        WeatherIcon = "\uf00c";
                        break;
                    case 36: // HOT
                        WeatherIcon = "\uf072";
                        break;
                    case 3200: // Not Available
                    default:
                        WeatherIcon = "\uf07b";
                        break;
                }
            }

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

            if (int.TryParse(weather.condition.icon, out int code))
            {
                switch (code)
                {
                    case 27: // Mostly Cloudy (Night)
                    case 29: // Partly Cloudy (Night)
                    case 31: // Clear (Night)
                    case 33: // Fair (Night)
                        isNight = true;
                        break;
                }
            }

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
            if (int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    // Rain 
                    case 9:
                    case 11:
                    case 12:
                    case 40:
                    // (Mixed) Rain/Snow/Sleet
                    case 5:
                    case 6:
                    case 7:
                    case 18:
                    // Hail / Freezing Rain
                    case 8:
                    case 10:
                    case 17:
                    case 35:
                    // Snow / Snow Showers/Storm
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 41:
                    case 42:
                    case 43:
                    case 46:
                    // Tornado / Hurricane / Thunderstorm / Tropical Storm
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 37:
                    case 38:
                    case 39:
                    case 45:
                    case 47:
                        // lighter than night color + cloudiness
                        rgb = new byte[3] { 53, 67, 116 };
                        break;
                    // Dust
                    case 19:
                    // Foggy / Haze
                    case 20:
                    case 21:
                    case 22:
                        // add haziness
                        rgb = new byte[3] { 143, 163, 196 };
                        break;
                    // Night
                    case 31:
                    case 33:
                        // Night background
                        rgb = new byte[3] { 26, 36, 74 };
                        break;
                    /* Ambigious weather conditions */
                    // (Mostly) Cloudy
                    case 28:
                    case 26:
                    case 27:
                    // Partly Cloudy
                    case 44:
                    case 29:
                    case 30:
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
                    case 3200:
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
