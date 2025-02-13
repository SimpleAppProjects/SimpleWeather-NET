using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Resources.Strings;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Maui;
using SimpleWeather.Weather_API.SMC;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.WeatherKit
{
    public partial class WeatherKitProvider : WeatherProviderImpl
    {
        private const String BASE_URL = "https://weatherkit.apple.com/api/v1/";
        private const String AVAILABILITY_QUERY_URL = BASE_URL + "availability/{0}?country={2}";

        private const String WEATHER_QUERY_URL = BASE_URL +
                                                 "weather/{0}/{1}?countryCode={2}&dataSets=currentWeather,forecastDaily,forecastHourly,forecastNextHour,weatherAlerts&timezone={timezone}";

        public WeatherKitProvider() : base()
        {
            LocationProvider = this.RunCatching(() =>
            {
                return WeatherModule.Instance.LocationProviderFactory.GetLocationProvider(
                    RemoteConfigService.GetLocationProvider(WeatherAPI));
            }).GetOrElse<IWeatherLocationProvider, IWeatherLocationProvider>((t) =>
            {
                return new MauiLocationProvider();
            });
        }

        public override string WeatherAPI => WAPI.Apple;
        public override bool SupportsWeatherLocale => true;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;
        public override bool KeyRequired => false;
        public override int HourlyForecastInterval => 1;
        public override AuthType AuthType => AuthType.Internal;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override string GetAPIKey()
        {
            return null;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        protected override async Task<SimpleWeather.WeatherData.Weather> GetWeatherData(
            SimpleWeather.LocationData.LocationData location)
        {
            SimpleWeather.WeatherData.Weather weather = null;
            WeatherException wEx = null;

            var culture = LocaleUtils.GetLocale();

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            try
            {
                this.CheckRateLimit();

                Uri weatherURL = BASE_URL.ToUriBuilderEx()
                    .AppendPath("weather")
                    .AppendPath(locale)
                    .AppendPath(await UpdateLocationQuery(location), encode: false)
                    .AppendQueryParameter("country", location.country_code)
                    .AppendQueryParameter("dataSets",
                        "currentWeather,forecastDaily,forecastHourly,forecastNextHour,weatherAlerts")
                    .AppendQueryParameter("timezone", location.tz_long)
                    .BuildUri();

                using var request = new HttpRequestMessage(HttpMethod.Get, weatherURL);
                // Add headers to request
                var token = await Auth.WeatherKitJwtService.GetBearerToken();
                if (!String.IsNullOrWhiteSpace(token))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                else
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey,
                        new Exception($"Invalid bearer token: {token}"));

                request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                // Get response
                var webClient = SharedModule.Instance.WebClient;
                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var response = await webClient.SendAsync(request, cts.Token);
                await this.CheckForErrors(response);
                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();

                // Load weather
                var root = await JSONParser.DeserializerAsync<Weather>(stream);

                weather = this.CreateWeatherData(root);
            }
            catch (Exception ex)
            {
                weather = null;

                if (ex is HttpRequestException or WebException or SocketException or IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException exception)
                {
                    wEx = exception;
                }
                else
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather, ex);
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherKitProvider: error getting weather data");
            }

            if (weather == null || !weather.IsValid())
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null)
            {
                if (SupportsWeatherLocale)
                    weather.locale = locale;

                weather.query = location.query;
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        protected override async Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location,
            SimpleWeather.WeatherData.Weather weather)
        {
            var offset = location.tz_offset;

            // Update tz for weather properties
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            foreach (SimpleWeather.WeatherData.HourlyForecast hr_forecast in weather.hr_forecast)
            {
                hr_forecast.date = hr_forecast.date.ToOffset(offset);
            }

            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
            }

            foreach (TextForecast forecast in weather.txt_forecast)
            {
                forecast.date = forecast.date.ToOffset(offset);
            }

            if (weather.min_forecast?.Any() == true)
            {
                foreach (MinutelyForecast min_forecast in weather.min_forecast)
                {
                    min_forecast.date = min_forecast.date.ToOffset(offset);
                }
            }

            if (weather.weather_alerts?.Any() == true)
            {
                foreach (var alert in weather.weather_alerts)
                {
                    alert.Date = alert.Date.ToOffset(offset);
                    alert.ExpiresDate = alert.ExpiresDate.ToOffset(offset);
                }
            }

            if (weather.astronomy != null)
            {
                // The time of day is set to max if the sun never sets/rises and
                // DateTime is set to min if not found
                // Don't change this if its set that way
                if (weather.astronomy.sunrise > DateTime.MinValue &&
                    weather.astronomy.sunrise.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    weather.astronomy.sunrise = weather.astronomy.sunrise.Add(offset);
                if (weather.astronomy.sunset > DateTime.MinValue &&
                    weather.astronomy.sunset.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    weather.astronomy.sunset = weather.astronomy.sunset.Add(offset);
                if (weather.astronomy.moonrise > DateTime.MinValue &&
                    weather.astronomy.moonrise.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    weather.astronomy.moonrise = weather.astronomy.moonrise.Add(offset);
                if (weather.astronomy.moonset > DateTime.MinValue &&
                    weather.astronomy.moonset.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    weather.astronomy.moonset = weather.astronomy.moonset.Add(offset);
            }
            else
            {
                weather.astronomy =
                    await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
            }
        }

        public override Task<string> UpdateLocationQuery(SimpleWeather.WeatherData.Weather weather)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "{0:0.####}/{1:0.####}",
                weather.location.latitude, weather.location.longitude));
        }

        public override Task<string> UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "{0:0.####}/{1:0.####}",
                location.latitude, location.longitude));
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            String code = "en_US";

            switch (iso)
            {
                // Chinese
                case "zh":
                    switch (name)
                    {
                        // Chinese - Traditional
                        case "zh-TW":
                            code = "zh_TW";
                            break;
                        case "zh-Hant":
                        case "zh-HK":
                        case "zh-MO":
                            code = "zh_HK";
                            break;
                        // Mandarin
                        case "zh-cmn":
                            code = "zh_cmn";
                            break;
                        // Wu
                        case "zh-wuu":
                            code = "zh_wuu";
                            break;
                        // Xiang
                        case "zh-hsn":
                            code = "zh_hsn";
                            break;
                        // Cantonese
                        case "zh-yue":
                            code = "zh_yue";
                            break;
                        // Chinese - Simplified
                        default:
                            code = "zh";
                            break;
                    }

                    break;
                default:
                    code = name;
                    break;
            }

            return code;
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = icon switch
            {
                "BlowingDust" => WeatherIcons.DUST,
                "Clear" => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,
                "Cloudy" => WeatherIcons.CLOUDY,
                "Foggy" => WeatherIcons.FOG,
                "Haze" => WeatherIcons.HAZE,
                "MostlyClear" or "PartlyCloudy" => isNight
                    ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY
                    : WeatherIcons.DAY_PARTLY_CLOUDY,
                "MostlyCloudy" => isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY,
                "Smoky" => WeatherIcons.SMOKE,

                "Breezy" => isNight ? WeatherIcons.NIGHT_LIGHT_WIND : WeatherIcons.DAY_LIGHT_WIND,
                "Windy" => isNight ? WeatherIcons.NIGHT_WINDY : WeatherIcons.DAY_WINDY,

                "Drizzle" => WeatherIcons.SPRINKLE,
                "HeavyRain" => WeatherIcons.RAIN_WIND,
                "IsolatedThunderstorms" or "ScatteredThunderstorms" or "Thunderstorms" => WeatherIcons.THUNDERSTORM,
                "Rain" => WeatherIcons.RAIN,
                "SunShowers" => isNight ? WeatherIcons.NIGHT_ALT_RAIN : WeatherIcons.DAY_RAIN,
                "StrongStorms" => WeatherIcons.STORM_SHOWERS,

                "Frigid" => WeatherIcons.SNOWFLAKE_COLD,
                "Hail" => WeatherIcons.HAIL,
                "Hot" => WeatherIcons.HOT,

                "Flurries" or "Snow" => WeatherIcons.SNOW,
                "Sleet" => WeatherIcons.SLEET,
                "SunFlurries" => isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW,
                "WintryMix" => WeatherIcons.RAIN_MIX,

                "Blizzard" or "BlowingSnow" or "HeavySnow" => WeatherIcons.SNOW_WIND,
                "FreezingDrizzle" or "FreezingRain" => WeatherIcons.RAIN_MIX,

                "Hurricane" or "TropicalStorm" => WeatherIcons.HURRICANE,

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        public override string GetWeatherCondition(string icon)
        {
            return icon switch
            {
                "BlowingDust" => WeatherConditions.weather_dust,
                "Clear" => WeatherConditions.weather_clear,
                "Cloudy" => WeatherConditions.weather_cloudy,
                "Foggy" => WeatherConditions.weather_foggy,
                "Haze" => WeatherConditions.weather_haze,
                "MostlyClear" => WeatherConditions.weather_mostlyclear,
                "MostlyCloudy" => WeatherConditions.weather_mostlycloudy,
                "PartlyCloudy" => WeatherConditions.weather_partlycloudy,
                "Smoky" => WeatherConditions.weather_smoky,

                "Breezy" => WeatherConditions.weather_lightwind,
                "Windy" => WeatherConditions.weather_windy,

                "Drizzle" => WeatherConditions.weather_drizzle,
                "HeavyRain" => WeatherConditions.weather_heavyrain,
                "IsolatedThunderstorms" => WeatherConditions.weather_isotstorms,
                "Rain" => WeatherConditions.weather_rain,
                "SunShowers" => WeatherConditions.weather_rainshowers,
                "ScatteredThunderstorms" => WeatherConditions.weather_scatteredtstorms,
                "StrongStorms" => WeatherConditions.weather_severetstorms,
                "Thunderstorms" => WeatherConditions.weather_tstorms,

                "Frigid" => WeatherConditions.weather_cold,
                "Hail" => WeatherConditions.weather_hail,
                "Hot" => WeatherConditions.weather_hot,

                "Flurries" or "SunFlurries" => WeatherConditions.weather_snowflurries,
                "Sleet" => WeatherConditions.weather_sleet,
                "Snow" => WeatherConditions.weather_snow,
                "WintryMix" => WeatherConditions.weather_rainandsnow,

                "Blizzard" => WeatherConditions.weather_blizzard,
                "BlowingSnow" => WeatherConditions.weather_blowingsnow,
                "FreezingDrizzle" or "FreezingRain" => WeatherConditions.weather_freezingrain,
                "HeavySnow" => WeatherConditions.weather_heavysnow,

                "Hurricane" => WeatherConditions.weather_hurricane,
                "TropicalStorm" => WeatherConditions.weather_tropicalstorm,

                _ => WeatherConditions.weather_notavailable,
            };
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(SimpleWeather.WeatherData.Weather weather)
        {
            bool isNight = base.IsNight(weather);

            if (!isNight)
            {
                // Fallback to sunset/rise time just in case
                LocalTime sunrise;
                LocalTime sunset;
                if (weather.astronomy != null)
                {
                    sunrise = LocalTime.FromTicksSinceMidnight(weather.astronomy.sunrise.TimeOfDay.Ticks);
                    sunset = LocalTime.FromTicksSinceMidnight(weather.astronomy.sunset.TimeOfDay.Ticks);
                }
                else
                {
                    sunrise = LocalTime.FromHourMinuteSecondTick(6, 0, 0, 0);
                    sunset = LocalTime.FromHourMinuteSecondTick(18, 0, 0, 0);
                }

                DateTimeZone tz = null;

                if (weather.location.tz_long != null)
                {
                    tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
                }

                if (tz == null)
                    tz = DateTimeZone.ForOffset(Offset.FromTimeSpan(weather.location.tz_offset));

                var now = SystemClock.Instance.GetCurrentInstant()
                    .InZone(tz).TimeOfDay;

                // Determine whether its night using sunset/rise times
                if (now < sunrise || now > sunset)
                    isNight = true;
            }

            return isNight;
        }
    }
}