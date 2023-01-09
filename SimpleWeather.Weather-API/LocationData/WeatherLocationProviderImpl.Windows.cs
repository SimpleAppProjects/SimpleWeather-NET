#if WINDOWS
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Bing;
using SimpleWeather.Weather_API.Keys;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace SimpleWeather.LocationData
{
    public partial class WeatherLocationProviderImpl
    {
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public virtual async partial Task<LocationQuery> GetLocationFromName(LocationQuery model)
        {
            LocationQuery location = null;

            var culture = CultureUtils.UserCulture;

            MapLocation result = null;
            WeatherException wEx = null;

            try
            {
                MapService.ServiceToken = APIKeys.GetBingMapsKey();
                // The nearby location to use as a query hint.
                BasicGeoposition queryHint = new BasicGeoposition
                {
                    Latitude = 0,
                    Longitude = 0
                };
                Geopoint hintPoint = new Geopoint(queryHint);

                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                {
                    // Geocode the specified address, using the specified reference point
                    // as a query hint. Return no more than a single result.
                    MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAsync(model.LocationName, hintPoint, 1).AsTask(cts.Token);

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
            }
            catch (Exception ex)
            {
                result = null;
                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null && !String.IsNullOrWhiteSpace(result.DisplayName))
                location = this.CreateLocationModel(result, model.WeatherSource);
            else
                location = new LocationQuery();

            location.LocationSource = LocationAPI;

            return location;
        }
    }
}
#endif