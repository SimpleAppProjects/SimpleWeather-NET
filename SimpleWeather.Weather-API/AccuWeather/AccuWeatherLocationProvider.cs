using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.AccuWeather
{
    internal class AccuWeatherLocationProvider : Bing.BingMapsLocationProvider
    {
        private const string BASE_URL = "https://dataservice.accuweather.com/locations/v1/cities/geoposition/search";

        public override string LocationAPI => WeatherAPI.AccuWeather;

        public override bool KeyRequired => false;
        public override bool SupportsLocale => true;
        public override bool NeedsLocationFromID => true;
        public override bool NeedsLocationFromName => false;
        public override bool NeedsLocationFromGeocoder => false;

        public override string GetAPIKey()
        {
            return APIKeys.GetAccuWeatherKey();
        }

        /// <exception cref="WeatherException">Ignore. Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQuery> GetLocationFromID(LocationQuery model)
        {
            LocationQuery location;

            var culture = LocaleUtils.GetLocale();
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            GeopositionRootobject result = null;
            WeatherException wEx = null;

            try
            {
                // If were under rate limit, deny request
                this.CheckRateLimit();

                var key = SettingsManager.UsePersonalKey ? SettingsManager.APIKeys[WeatherAPI.AccuWeather] : GetAPIKey();

                if (string.IsNullOrWhiteSpace(key))
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);

                var requestUri = BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("q", $"{model.LocationLat.ToInvariantString("0.##")},{model.LocationLong.ToInvariantString("0.##")}")
                    .AppendQueryParameter("language", locale)
                    .AppendQueryParameter("details", "false")
                    .AppendQueryParameter("toplevel", "false")
                    .BuildUri();

                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(14)
                    };

                    // Connect to webstream
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        using var contentStream = await response.Content.ReadAsStreamAsync();

                        // Load data
                        result = await JSONParser.DeserializerAsync<GeopositionRootobject>(contentStream);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;

                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "AccuWeatherLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null)
                location = this.CreateLocationModel(result);
            else
                location = new LocationQuery();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQuery location = null;

            var culture = LocaleUtils.GetLocale();
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            GeopositionRootobject result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                var key = SettingsManager.UsePersonalKey ? SettingsManager.APIKeys[WeatherAPI.AccuWeather] : GetAPIKey();

                if (string.IsNullOrWhiteSpace(key))
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);

                var requestUri = BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("q", $"{coord.Latitude.ToInvariantString("0.##")},{coord.Longitude.ToInvariantString("0.##")}")
                    .AppendQueryParameter("language", locale)
                    .AppendQueryParameter("details", "false")
                    .AppendQueryParameter("toplevel", "false")
                    .BuildUri();

                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(14)
                    };

                    // Connect to webstream
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        using var contentStream = await response.Content.ReadAsStreamAsync();

                        // Load data
                        result = await JSONParser.DeserializerAsync<GeopositionRootobject>(contentStream);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;

                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "AccuWeatherLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null)
                location = this.CreateLocationModel(result);
            else
                location = new LocationQuery();

            return location;
        }

        public override string LocaleToLangCode(string iso, string name)
        {
            return name;
        }
    }
}
