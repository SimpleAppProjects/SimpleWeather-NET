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
using System.Globalization;
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

namespace SimpleWeather.HERE
{
    public partial class HEREWeatherProvider : WeatherProviderImpl
    {
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => true;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;

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
                Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting locations");
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
                        Toast.MakeText(Application.Context, wEx.Message, ToastLength.Short).Show();
                    });
                }
#endif
                Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.woeid))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<LocationQueryViewModel> GetLocation(string location_query)
        {
            LocationQueryViewModel location = null;

            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
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
                        Toast.MakeText(Application.Context, wEx.Message, ToastLength.Short).Show();
                    });
                }
#endif
                Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.woeid))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<bool> IsKeyValid(string key)
        {
            string queryAPI = "https://weather.cit.api.here.com/weather/1.0/report.json";
            string app_id = key.Split(';').First();
            string app_code = key.Split(';').Last();
            Uri queryURL = new Uri(String.Format("{0}?app_id={1}&app_code={2}", queryAPI, app_id, app_code));
            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                if (String.IsNullOrWhiteSpace(app_id) || String.IsNullOrWhiteSpace(app_code))
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

        public override String GetAPIKey()
        {
            if (String.IsNullOrWhiteSpace(GetAppID()) && String.IsNullOrWhiteSpace(GetAppCode()))
                return String.Empty;
            else
                return String.Format("{0};{1}", GetAppID(), GetAppCode());
        }

        public override async Task<Weather> GetWeather(string location_query)
        {
            Weather weather = null;

            string queryAPI = null;
            Uri weatherURL = null;

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);
#else
            var culture = CultureInfo.CurrentCulture;
#endif
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);
            queryAPI = "https://weather.cit.api.here.com/weather/1.0/report.json?product=alerts&product=forecast_7days_simple" +
                "&product=forecast_hourly&product=forecast_astronomy&product=observation&oneobservation=true&{0}" +
                "&language={1}&metric=false&app_id={2}&app_code={3}";

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();
            string app_id = key.Split(';').First();
            string app_code = key.Split(';').Last();
            weatherURL = new Uri(String.Format(queryAPI, location_query, locale, app_id, app_code));

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
                if (root.Type != null)
                {
                    switch (root.Type)
                    {
                        case "Invalid Request":
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                            break;
                        case "Unauthorized":
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
                if (root.alerts?.alerts != null && root.alerts.alerts.Length > 0)
                {
                    if (weather.weather_alerts == null)
                        weather.weather_alerts = new List<WeatherAlert>();

                    foreach (Alert result in root.alerts.alerts)
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

                Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting weather data");
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

            var offset = location.tz_offset;

            if (weather.weather_alerts?.Count > 0)
            {
                foreach (WeatherAlert alert in weather.weather_alerts)
                {
                    if (!alert.Date.Offset.Equals(offset))
                    {
                        alert.Date = new DateTimeOffset(alert.Date.DateTime, offset);
                    }

                    if (!alert.ExpiresDate.Offset.Equals(offset))
                    {
                        alert.ExpiresDate = new DateTimeOffset(alert.ExpiresDate.DateTime, offset);
                    }
                }
            }
            else if (weather.weather_alerts?.Count == 0 && "US".Equals(location.country_code))
            {
                weather.weather_alerts = await new NWS.NWSAlertProvider().GetAlerts(location);
            }

            // Update tz for weather properties
            weather.update_time = weather.update_time.ToOffset(offset);

            foreach (WeatherData.Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
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
            var culture = new CultureInfo(userlang);
#else
            var culture = CultureInfo.CurrentCulture;
#endif
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            queryAPI = "https://weather.cit.api.here.com/weather/1.0/report.json?product=alerts&{0}" +
                "&language={1}&metric=false&app_id={2}&app_code={3}";

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

            string app_id = key.Split(';').First();
            string app_code = key.Split(';').Last();
            weatherURL = new Uri(String.Format(queryAPI, location.query, locale, app_id, app_code));

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

                Rootobject root = JSONParser.Deserializer<Rootobject>(contentStream);

                foreach (Alert result in root.alerts.alerts)
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
                Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting weather alert data");
            }

            if (alerts == null)
                alerts = new List<WeatherAlert>();

            return alerts;
        }

        // Fix format of query to pass to Yahoo API
        public override async Task UpdateLocationData(LocationData location)
        {
            string location_query = string.Format("({0},{1})", location.latitude, location.longitude);

            var qview = await GetLocation(location_query);

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
                query = string.Format("latitude={0}&longitude={1}", weather.location.latitude, weather.location.longitude);
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
                query = string.Format("latitude={0}&longitude={1}", location.latitude, location.longitude);
            else
                query = qview.LocationQuery;

            return query;
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            return name;
        }

        public override string GetWeatherIcon(string icon)
        {
            bool isNight = false;

            if (icon.StartsWith("N_", StringComparison.Ordinal) || icon.Contains("night_"))
                isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            if (icon.Contains("mostly_sunny") || icon.Contains("mostly_clear") || icon.Contains("partly_cloudy")
                    || icon.Contains("passing_clounds") || icon.Contains("more_sun_than_clouds") || icon.Contains("scattered_clouds")
                    || icon.Contains("decreasing_cloudiness") || icon.Contains("clearing_skies") || icon.Contains("overcast")
                    || icon.Contains("low_clouds") || icon.Contains("passing_clouds"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
            else if (icon.Contains("cloudy") || icon.Contains("a_mixture_of_sun_and_clouds") || icon.Contains("increasing_cloudiness")
                     || icon.Contains("breaks_of_sun_late") || icon.Contains("afternoon_clouds") || icon.Contains("morning_clouds")
                     || icon.Contains("partly_sunny") || icon.Contains("more_clouds_than_sun") || icon.Contains("broken_clouds"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY;
            else if (icon.Contains("high_level_clouds") || icon.Contains("high_clouds"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY_HIGH;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY_HIGH;
            else if (icon.Contains("flurries") || icon.Contains("snowstorm") || icon.Contains("blizzard"))
                WeatherIcon = WeatherIcons.SNOW_WIND;
            else if (icon.Contains("fog"))
                WeatherIcon = WeatherIcons.FOG;
            else if (icon.Contains("hazy") || icon.Contains("haze"))
                if (isNight)
                    WeatherIcon = WeatherIcons.WINDY;
                else
                    WeatherIcon = WeatherIcons.DAY_HAZE;
            else if (icon.Contains("sleet") || icon.Contains("snow_changing_to_an_icy_mix") || icon.Contains("an_icy_mix_changing_to_snow")
                    || icon.Contains("rain_changing_to_snow"))
                WeatherIcon = WeatherIcons.SLEET;
            else if (icon.Contains("mixture_of_precip") || icon.Contains("icy_mix") || icon.Contains("snow_changing_to_rain")
                    || icon.Contains("snow_rain_mix") || icon.Contains("freezing_rain"))
                WeatherIcon = WeatherIcons.RAIN_MIX;
            else if (icon.Contains("hail"))
                WeatherIcon = WeatherIcons.HAIL;
            else if (icon.Contains("snow"))
                WeatherIcon = WeatherIcons.SNOW;
            else if (icon.Contains("sprinkles") || icon.Contains("drizzle"))
                WeatherIcon = WeatherIcons.SPRINKLE;
            else if (icon.Contains("light_rain") || icon.Contains("showers"))
                WeatherIcon = WeatherIcons.SHOWERS;
            else if (icon.Contains("rain") || icon.Contains("flood"))
                WeatherIcon = WeatherIcons.RAIN;
            else if (icon.Contains("tstorms") || icon.Contains("thunderstorms") || icon.Contains("thundershowers")
                    || icon.Contains("tropical_storm"))
                WeatherIcon = WeatherIcons.THUNDERSTORM;
            else if (icon.Contains("smoke"))
                WeatherIcon = WeatherIcons.SMOKE;
            else if (icon.Contains("tornado"))
                WeatherIcon = WeatherIcons.TORNADO;
            else if (icon.Contains("hurricane"))
                WeatherIcon = WeatherIcons.HURRICANE;
            else if (icon.Contains("sandstorm"))
                WeatherIcon = WeatherIcons.SANDSTORM;
            else if (icon.Contains("duststorm"))
                WeatherIcon = WeatherIcons.DUST;
            else if (icon.Contains("clear") || icon.Contains("sunny"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;
            else if (icon.Contains("cw_no_report_icon") || icon.StartsWith("night_", StringComparison.Ordinal))
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