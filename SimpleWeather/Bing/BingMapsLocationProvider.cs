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

        public override bool KeyRequired => true;

        public override bool SupportsWeatherLocale => true;

        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string location_query, string weatherAPI)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            var userlang = GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);

            // http://dev.virtualearth.net/REST/v1/Autosuggest?query=new%20york&userLocation=0,0&includeEntityTypes=Place&key=API_KEY&culture=fr-FR&userRegion=FR
            string queryAPI = "http://dev.virtualearth.net/REST/v1/Autosuggest";
            string query = "?query={0}&userLocation=0,0&includeEntityTypes=Place&key={1}&culture={2}&maxResults=10";

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

            Uri queryURL = new Uri(String.Format(queryAPI + query, location_query, key, culture.Name));
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
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
                        added = locationSet.Add(new LocationQueryViewModel(result.address));
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
                locations = new ObservableCollection<LocationQueryViewModel>();
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting locations");
            }

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };

            return locations;
        }

        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var userlang = GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

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
                MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode, MapLocationDesiredAccuracy.High);

                switch (mapResult.Status)
                {
                    case MapLocationFinderStatus.Success:
                        result = mapResult.Locations[0];
                        break;
                    case MapLocationFinderStatus.UnknownError:
                    case MapLocationFinderStatus.IndexFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.InvalidCredentials:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.BadLocation:
                    case MapLocationFinderStatus.NotSupported:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.NetworkFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.DisplayName))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<LocationQueryViewModel> GetLocation(string location_query, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var userlang = GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

            MapLocation result = null;
            WeatherException wEx = null;

            try
            {
                var args = location_query.Split('&').ToDictionary(c => c.Split('=')[0], c => Uri.UnescapeDataString(c.Split('=')[1]));

                MapService.ServiceToken = key;
                // The nearby location to use as a query hint.
                BasicGeoposition geoPoint = new BasicGeoposition
                {
                    Latitude = double.Parse(args["lat"]),
                    Longitude = double.Parse(args["lon"])
                };
                Geopoint pointToReverseGeocode = new Geopoint(geoPoint);

                // Geocode the specified address, using the specified reference point
                // as a query hint. Return no more than a single result.
                MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode, MapLocationDesiredAccuracy.High);

                switch (mapResult.Status)
                {
                    case MapLocationFinderStatus.Success:
                        result = mapResult.Locations[0];
                        break;
                    case MapLocationFinderStatus.UnknownError:
                    case MapLocationFinderStatus.IndexFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.InvalidCredentials:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.BadLocation:
                    case MapLocationFinderStatus.NotSupported:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.NetworkFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.DisplayName))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public async Task<LocationQueryViewModel> GetLocationFromAddress(string address)
        {
            LocationQueryViewModel location = null;

            var userlang = GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

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
                MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAsync(address, hintPoint, 1);

                switch (mapResult.Status)
                {
                    case MapLocationFinderStatus.Success:
                        result = mapResult.Locations[0];
                        break;
                    case MapLocationFinderStatus.UnknownError:
                    case MapLocationFinderStatus.IndexFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.InvalidCredentials:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.BadLocation:
                    case MapLocationFinderStatus.NotSupported:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                    case MapLocationFinderStatus.NetworkFailure:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                        await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (result != null && !String.IsNullOrWhiteSpace(result.DisplayName))
                location = new LocationQueryViewModel(result);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override async Task<bool> IsKeyValid(string key)
        {
            bool isValid = false;

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
                case MapLocationFinderStatus.InvalidCredentials:
                case MapLocationFinderStatus.UnknownError:
                case MapLocationFinderStatus.IndexFailure:
                case MapLocationFinderStatus.NetworkFailure:
                    isValid = false;
                    break;

                case MapLocationFinderStatus.Success:
                case MapLocationFinderStatus.NotSupported:
                case MapLocationFinderStatus.BadLocation:
                    isValid = true;
                    break;
            }

            return isValid;
        }

        public override String GetAPIKey()
        {
            return APIKeys.GetBingMapsKey();
        }
    }
}
