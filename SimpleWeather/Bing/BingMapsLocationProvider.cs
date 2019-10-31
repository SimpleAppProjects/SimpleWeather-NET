using SimpleWeather.Controls;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.System.UserProfile;
using Windows.Web;
using Windows.Web.Http;

namespace SimpleWeather.Bing
{
    public class BingMapsLocationProvider : LocationProviderImpl
    {
        public override string LocationAPI => WeatherAPI.BingMaps;

        public override bool KeyRequired => false;

        public override bool SupportsWeatherLocale => true;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string location_query, string weatherAPI)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            var userlang = GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);

            // http://dev.virtualearth.net/REST/v1/Autosuggest?query=new%20york&userLocation=0,0&includeEntityTypes=Place&key=API_KEY&culture=fr-FR&userRegion=FR
            string queryAPI = "http://dev.virtualearth.net/REST/v1/Autosuggest";
            string query = "?query={0}&userLocation=0,0&includeEntityTypes=Place&key={1}&culture={2}&maxResults=10";

            // TODO: NOTE: Decide if we will allow users to provide their own keys for loc providers
            string key = GetAPIKey();

            Uri queryURL = new Uri(String.Format(queryAPI + query, location_query, key, culture.Name));
            WeatherException wEx = null;
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);

                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL).AsTask(cts.Token);
                response.EnsureSuccessStatusCode();
                Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                // End Stream
                webClient.Dispose();

                // Load data
                var locationSet = new HashSet<LocationQueryViewModel>();
                AC_Rootobject root = null;
                await Task.Run(() =>
                {
                    root = JSONParser.Deserializer<AC_Rootobject>(contentStream);
                });

                foreach (Value result in root.resourceSets[0].resources[0].value)
                {
                    // Filter: only store city results
                    bool added = false;
                    if (!String.IsNullOrWhiteSpace(result.address.locality))
                        added = locationSet.Add(new LocationQueryViewModel(result.address, weatherAPI));
                    else
                        continue;

                    // Limit amount of results
                    if (added)
                    {
                        maxResults--;
                        if (maxResults <= 0)
                            break;
                    }
                }

                locations = new ObservableCollection<LocationQueryViewModel>(locationSet);

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;
            
            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };

            return locations;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var userlang = GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);

            // TODO: NOTE: Decide if we will allow users to provide their own keys for loc providers
            string key = GetAPIKey();

            MapLocation result = null;
            WeatherException wEx = null;

            try
            {
                MapService.ServiceToken = key;
                // The nearby location to use as a query hint.
                BasicGeoposition geoPoint = new BasicGeoposition
                {
                    Latitude = coord.Latitude,
                    Longitude = coord.Longitude
                };
                Geopoint pointToReverseGeocode = new Geopoint(geoPoint);

                // Geocode the specified address, using the specified reference point
                // as a query hint. Return no more than a single result.
                CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode, MapLocationDesiredAccuracy.High).AsTask(cts.Token);

                switch (mapResult.Status)
                {
                    case MapLocationFinderStatus.Success:
                        result = mapResult.Locations[0];
                        break;
                    case MapLocationFinderStatus.UnknownError:
                    case MapLocationFinderStatus.IndexFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                        break;
                    case MapLocationFinderStatus.InvalidCredentials:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                        break;
                    case MapLocationFinderStatus.BadLocation:
                    case MapLocationFinderStatus.NotSupported:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                        break;
                    case MapLocationFinderStatus.NetworkFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;
            
            if (result != null && !String.IsNullOrWhiteSpace(result.DisplayName))
                location = new LocationQueryViewModel(result, weatherAPI);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public async Task<LocationQueryViewModel> GetLocationFromAddress(string address, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var userlang = GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);

            // TODO: NOTE: Decide if we will allow users to provide their own keys for loc providers
            string key = GetAPIKey();

            MapLocation result = null;
            WeatherException wEx = null;

            try
            {
                MapService.ServiceToken = key;
                // The nearby location to use as a query hint.
                BasicGeoposition queryHint = new BasicGeoposition
                {
                    Latitude = 0,
                    Longitude = 0
                };
                Geopoint hintPoint = new Geopoint(queryHint);

                // Geocode the specified address, using the specified reference point
                // as a query hint. Return no more than a single result.
                CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAsync(address, hintPoint, 1).AsTask(cts.Token);

                switch (mapResult.Status)
                {
                    case MapLocationFinderStatus.Success:
                        result = mapResult.Locations[0];
                        break;
                    case MapLocationFinderStatus.UnknownError:
                    case MapLocationFinderStatus.IndexFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                        break;
                    case MapLocationFinderStatus.InvalidCredentials:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                        break;
                    case MapLocationFinderStatus.BadLocation:
                    case MapLocationFinderStatus.NotSupported:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                        break;
                    case MapLocationFinderStatus.NetworkFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx; 
            
            if (result != null && !String.IsNullOrWhiteSpace(result.DisplayName))
                location = new LocationQueryViewModel(result, weatherAPI);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<bool> IsKeyValid(string key)
        {
            bool isValid = false;
            WeatherException wEx = null;

            MapService.ServiceToken = key;
            // The nearby location to use as a query hint.
            BasicGeoposition geoPoint = new BasicGeoposition
            {
                Latitude = 0,
                Longitude = 0
            };
            Geopoint pointToReverseGeocode = new Geopoint(geoPoint);

            // Geocode the specified address, using the specified reference point
            // as a query hint. Return no more than a single result.
            MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode, MapLocationDesiredAccuracy.Low);

            switch (mapResult.Status)
            {
                case MapLocationFinderStatus.UnknownError:
                case MapLocationFinderStatus.IndexFailure:
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                    isValid = false;
                    break;
                case MapLocationFinderStatus.InvalidCredentials:
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                    isValid = false;
                    break;
                case MapLocationFinderStatus.NetworkFailure:
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    isValid = false;
                    break;
                case MapLocationFinderStatus.Success:
                case MapLocationFinderStatus.NotSupported:
                case MapLocationFinderStatus.BadLocation:
                    isValid = true;
                    break;
            }

            if (wEx != null)
                throw wEx;

            return isValid;
        }

        public override String GetAPIKey()
        {
            return APIKeys.GetBingMapsKey();
        }
    }
}
