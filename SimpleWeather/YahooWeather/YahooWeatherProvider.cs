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
                        TimeSpan sunrise = weather.astronomy.sunrise.TimeOfDay;
                        TimeSpan sunset = weather.astronomy.sunset.TimeOfDay;
                        TimeSpan now = DateTimeOffset.UtcNow.ToOffset(weather.location.tz_offset).TimeOfDay;

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
