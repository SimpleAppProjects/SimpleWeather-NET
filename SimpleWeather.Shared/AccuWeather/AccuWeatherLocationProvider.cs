using SimpleWeather.Controls;
using SimpleWeather.Keys;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web;

namespace SimpleWeather.AccuWeather
{
    internal class AccuWeatherLocationProvider : Bing.BingMapsLocationProvider
    {
        private const string BASE_URL = "https://dataservice.accuweather.com/locations/v1/cities/geoposition/search";

        public override string LocationAPI => WeatherData.WeatherAPI.AccuWeather;

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
        public override async Task<LocationQueryViewModel> GetLocationFromID(LocationQueryViewModel model)
        {
            LocationQueryViewModel location;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            GeopositionRootobject result = null;
            WeatherException wEx = null;

            try
            {
                // If were under rate limit, deny request
                this.CheckRateLimit();

                var key = (Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey()) ?? DevSettingsEnabler.GetAPIKey(WeatherData.WeatherAPI.AccuWeather);

                if (String.IsNullOrWhiteSpace(key))
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
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
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

                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown || ex is HttpRequestException || ex is SocketException)
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
                location = new LocationQueryViewModel(result, model.WeatherSource);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            GeopositionRootobject result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                var key = (Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey()) ?? DevSettingsEnabler.GetAPIKey(WeatherData.WeatherAPI.AccuWeather);

                if (String.IsNullOrWhiteSpace(key))
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
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
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

                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown || ex is HttpRequestException || ex is SocketException)
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
                location = new LocationQueryViewModel(result, weatherAPI);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override string LocaleToLangCode(string iso, string name)
        {
            return name;
        }
    }
}
