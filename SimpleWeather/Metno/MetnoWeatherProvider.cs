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

namespace SimpleWeather.Metno
{
    public partial class MetnoWeatherProvider : WeatherProviderImpl
    {
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;
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

                OpenWeather.AC_Rootobject root = JSONParser.Deserializer<OpenWeather.AC_Rootobject>(contentStream);

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
            throw new NotImplementedException();
        }

        public override async Task<Weather> GetWeather(string location_query)
        {
            Weather weather = null;

            string forecastAPI = null;
            Uri forecastURL = null;
            string sunrisesetAPI = null;
            Uri sunrisesetURL = null;

            forecastAPI = "https://beta.api.met.no/weatherapi/locationforecastlts/1.3/?{0}";
            forecastURL = new Uri(string.Format(forecastAPI, location_query));
            sunrisesetAPI = "https://beta.api.met.no/weatherapi/sunrise/1.1/?{0}&date={1}";
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            sunrisesetURL = new Uri(string.Format(sunrisesetAPI, location_query, date));

            HttpClient webClient = new HttpClient();
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
                weather.query = location_query;
                // Add condition icons
                weather.condition.weather = GetWeatherCondition(weather.condition.icon);

                foreach(Forecast forecast in weather.forecast)
                {
                    forecast.condition = GetWeatherCondition(forecast.icon);
                }

                foreach (HourlyForecast hr_forecast in weather.hr_forecast)
                {
                    hr_forecast.condition = GetWeatherCondition(hr_forecast.icon);
                }
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        public string GetWeatherCondition(string icon)
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

        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await base.GetWeather(location);

            // OWM reports datetime in UTC; add location tz_offset
            weather.update_time = weather.update_time.ToOffset(location.tz_offset);
            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                hr_forecast.date = hr_forecast.date.Add(location.tz_offset);
            }
            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(location.tz_offset);
            }
            weather.astronomy.sunrise = weather.astronomy.sunrise.Add(location.tz_offset);
            weather.astronomy.sunset = weather.astronomy.sunset.Add(location.tz_offset);

            return weather;
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

        public override string GetWeatherIcon(string icon)
        {
            string WeatherIcon = string.Empty;

            switch (icon)
            {
                case "1": // Sun
                    WeatherIcon = "\uf00d";
                    break;

                case "2": // LightCloud
                case "3": // PartlyCloud
                    WeatherIcon = "\uf002";
                    break;

                case "4": // Cloud
                    WeatherIcon = "\uf013";
                    break;

                case "5": // LightRainSun
                    WeatherIcon = "\uf00b";
                    break;

                case "6": // LightRainThunderSun
                    WeatherIcon = "\uf010";
                    break;

                case "7": // SleetSun
                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                    WeatherIcon = "\uf0b2";
                    break;

                case "8": // SnowSun
                case "44": // LightSnowSun
                case "45": // HeavySnowSun
                    WeatherIcon = "\uf00a";
                    break;

                case "9": // LightRain
                case "46": // Drizzle
                    WeatherIcon = "\uf01c";
                    break;

                case "10": // Rain
                    WeatherIcon = "\uf019";
                    break;

                case "11": // RainThunder
                    WeatherIcon = "\uf01e";
                    break;

                case "12": // Sleet
                case "47": // LightSleet
                case "48": // HeavySleet
                    WeatherIcon = "\uf0b5";
                    break;

                case "13": // Snow
                case "49": // LightSnow
                    WeatherIcon = "\uf01b";
                    break;

                case "14": // SnowThunder
                case "21": // SnowSunThunder
                case "28": // LightSnowThunderSun
                case "29": // HeavySnowThunderSun
                case "33": // LightSnowThunder
                case "34": // HeavySnowThunder
                    WeatherIcon = "\uf06b";
                    break;

                case "15": // Fog
                    WeatherIcon = "\uf014";
                    break;

                case "20": // SleetSunThunder
                case "23": // SleetThunder
                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                    WeatherIcon = "\uf068";
                    break;

                case "22": // LightRainThunder
                case "30": // DrizzleThunder
                    WeatherIcon = "\uf01d";
                    break;

                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                    WeatherIcon = "\uf00e";
                    break;

                case "40": // DrizzleSun
                case "41": // RainSun
                    WeatherIcon = "\uf008";
                    break;

                case "50": // HeavySnow
                    WeatherIcon = "\uf064";
                    break;
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = "\uf07b";
            }

            return WeatherIcon;
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            switch (icon)
            {
                case "1": // Sun
                    if (isNight)
                        WeatherIcon = "\uf02e";
                    else
                        WeatherIcon = "\uf00d";
                    break;

                case "2": // LightCloud
                case "3": // PartlyCloud
                    if (isNight)
                        WeatherIcon = "\uf031";
                    else
                        WeatherIcon = "\uf002";
                    break;

                case "4": // Cloud
                    WeatherIcon = "\uf013";
                    break;

                case "5": // LightRainSun
                    if (isNight)
                        WeatherIcon = "\uf039";
                    else
                        WeatherIcon = "\uf00b";
                    break;

                case "6": // LightRainThunderSun
                    if (isNight)
                        WeatherIcon = "\uf03b";
                    else
                        WeatherIcon = "\uf010";
                    break;

                case "7": // SleetSun
                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                    if (isNight)
                        WeatherIcon = "\uf0b3";
                    else
                        WeatherIcon = "\uf0b2";
                    break;

                case "8": // SnowSun
                case "44": // LightSnowSun
                case "45": // HeavySnowSun
                    if (isNight)
                        WeatherIcon = "\uf038";
                    else
                        WeatherIcon = "\uf00a";
                    break;

                case "9": // LightRain
                case "46": // Drizzle
                    WeatherIcon = "\uf01c";
                    break;

                case "10": // Rain
                    WeatherIcon = "\uf019";
                    break;

                case "11": // RainThunder
                    WeatherIcon = "\uf01e";
                    break;

                case "12": // Sleet
                case "48": // HeavySleet
                    WeatherIcon = "\uf017";
                    break;

                case "13": // Snow
                case "49": // LightSnow
                    WeatherIcon = "\uf01b";
                    break;

                case "14": // SnowThunder
                case "21": // SnowSunThunder
                case "28": // LightSnowThunderSun
                case "29": // HeavySnowThunderSun
                case "33": // LightSnowThunder
                case "34": // HeavySnowThunder
                    if (isNight)
                        WeatherIcon = "\uf06c";
                    else
                        WeatherIcon = "\uf06b";
                    break;

                case "15": // Fog
                    WeatherIcon = "\uf014";
                    break;

                case "20": // SleetSunThunder
                case "23": // SleetThunder
                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                    if (isNight)
                        WeatherIcon = "\uf069";
                    else
                        WeatherIcon = "\uf068";
                    break;

                case "22": // LightRainThunder
                case "30": // DrizzleThunder
                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                    if (isNight)
                        WeatherIcon = "\uf03a";
                    else
                        WeatherIcon = "\uf00e";
                    break;

                case "40": // DrizzleSun
                case "41": // RainSun
                    if (isNight)
                        WeatherIcon = "\uf036";
                    else
                        WeatherIcon = "\uf008";
                    break;

                case "47": // LightSleet
                    WeatherIcon = "\uf0b5";
                    break;

                case "50": // HeavySnow
                    WeatherIcon = "\uf064";
                    break;
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
            switch (icon)
            {
                case "5": // LightRainSun
                case "6": // LightRainThunderSun
                case "7": // SleetSun
                case "8": // SnowSun
                case "9": // LightRain
                case "10": // Rain
                case "11": // RainThunder
                case "12": // Sleet
                case "13": // Snow
                case "14": // SnowThunder
                case "20": // SleetSunThunder
                case "21": // SnowSunThunder
                case "22": // LightRainThunder
                case "23": // SleetThunder
                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                case "28": // LightSnowThunderSun
                case "29": // HeavySnowThunderSun
                case "30": // DrizzleThunder
                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                case "33": // LightSnowThunder
                case "34": // HeavySnowThunder
                case "40": // DrizzleSun
                case "41": // RainSun
                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                case "44": // LightSnowSun
                case "45": // HeavySnowSun
                case "46": // Drizzle
                case "47": // LightSleet
                case "48": // HeavySleet
                case "49": // LightSnow
                case "50": // HeavySnow
                    // lighter than night color + cloudiness
                    rgb = new byte[3] { 53, 67, 116 };
                    break;

                case "15": // Fog
                    // add haziness
                    rgb = new byte[3] { 143, 163, 196 };
                    break;

                case "2": // LightCloud
                case "3": // PartlyCloud
                case "4": // Cloud
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

                case "1": // Sun
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