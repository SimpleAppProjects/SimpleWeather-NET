using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.Web;
using Windows.Web.Http;

namespace SimpleWeather.WeatherUnderground
{
    public partial class WeatherUndergroundProvider : WeatherProviderImpl
    {
        public WeatherUndergroundProvider() : base()
        {
            LocationProvider = new WULocationProvider();
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.WeatherUnderground;
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => true;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
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

                CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);

                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await AsyncTask.RunAsync(webClient.GetAsync(queryURL).AsTask(cts.Token));
                response.EnsureSuccessStatusCode();
                Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                // Reset exception
                wEx = null;

                // End Stream
                webClient.Dispose();
                cts.Dispose();

                // Load data
                Rootobject root = await JSONParser.DeserializerAsync<Rootobject>(contentStream).ConfigureAwait(false);

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
                throw wEx;
            }

            return isValid;
        }

        public override string GetAPIKey()
        {
            return APIKeys.GetWUndergroundKey();
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(string location_query)
        {
            Weather weather = null;

            string queryAPI = null;
            Uri queryURL = null;

            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

            queryAPI = "https://api.wunderground.com/api/" + key + "/astronomy/conditions/forecast10day/hourly/alerts/lang:" + locale;
            string options = ".json";
            queryURL = new Uri(queryAPI + location_query + options);

            using (HttpClient webClient = new HttpClient())
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            {
                WeatherException wEx = null;

                try
                {
                    // Connect to webstream
                    HttpResponseMessage response = await AsyncTask.RunAsync(webClient.GetAsync(queryURL).AsTask(cts.Token));
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                    // Reset exception
                    wEx = null;

                    // Load weather
                    Rootobject root = await AsyncTask.RunAsync(() =>
                    {
                        return JSONParser.Deserializer<Rootobject>(contentStream);
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
                    contentStream?.Dispose();

                    weather = new Weather(root);

                    // Add weather alerts if available
                    if (root.alerts?.Length > 0)
                    {
                        if (weather.weather_alerts == null)
                            weather.weather_alerts = new List<WeatherAlert>(root.alerts.Length);

                        foreach (Alert result in root.alerts)
                        {
                            weather.weather_alerts.Add(new WeatherAlert(result));
                        }
                    }
                }
                catch (Exception ex)
                {
                    weather = null;
                    if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    }

                    Logger.WriteLine(LoggerLevel.Error, ex, "WeatherUndergroundProvider: error getting weather data");
                }

                // End Stream
                webClient.Dispose();

                if (wEx == null && (weather == null || !weather.IsValid()))
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
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await AsyncTask.RunAsync(base.GetWeather(location));

            // Just update hourly forecast dates to timezone
            var offset = location.tz_offset;

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                if (!hr_forecast.date.Offset.Equals(offset))
                    hr_forecast.date = new DateTimeOffset(hr_forecast.date.DateTime, offset);
            }

            // Update tz for weather alerts
            if (weather.weather_alerts != null && weather.weather_alerts.Any())
            {
                foreach (WeatherAlert alert in weather.weather_alerts)
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
            Uri queryURL = null;

            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

            queryAPI = "https://api.wunderground.com/api/" + key + "/alerts/lang:" + locale;
            string options = ".json";
            queryURL = new Uri(queryAPI + location.query + options);

            using (HttpClient webClient = new HttpClient())
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            {
                try
                {
                    // Connect to webstream
                    HttpResponseMessage response = await AsyncTask.RunAsync(webClient.GetAsync(queryURL).AsTask(cts.Token));
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                    // End Stream
                    webClient.Dispose();

                    // Load data
                    AlertRootobject root = JSONParser.Deserializer<AlertRootobject>(contentStream);

                    alerts = new List<WeatherAlert>(root.alerts.Length);

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
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format("/q/{0},{1}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return String.Format("/q/{0},{1}", location.latitude.ToString(CultureInfo.InvariantCulture), location.longitude.ToString(CultureInfo.InvariantCulture));
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

            if (icon.Contains("mostlycloudy") || icon.Contains("partlysunny") || icon.Contains("nt_cloudy"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY;
            else if (icon.Contains("partlycloudy") || icon.Contains("mostlysunny"))
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
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