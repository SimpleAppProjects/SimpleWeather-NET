// NOTE: Using MapLocationFinder causes application not close
// https://social.msdn.microsoft.com/Forums/SECURITY/en-us/ed4fbea6-84f9-4c46-967f-78918e47327c/bug-in-maplocationfinderfindlocationsatasync?forum=bingmapssilverlightwpfcontrols
#if false // WINDOWS
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace SimpleWeather.Weather_API.Bing
{
    public partial class BingMapsLocationProvider : WeatherLocationProviderImpl
    {
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override partial async Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQuery location = null;

            var culture = LocaleUtils.GetLocale();

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

                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                {
                    // Geocode the specified address, using the specified reference point
                    // as a query hint. Return no more than a single result.
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
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null && !string.IsNullOrWhiteSpace(result.DisplayName))
                location = this.CreateLocationModel(result, weatherAPI);
            else
                location = new LocationQuery();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override partial async Task<LocationQuery> GetLocationFromName(LocationQuery model)
        {
            LocationQuery location = null;

            var culture = LocaleUtils.GetLocale();

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

                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                {
                    // Geocode the specified address, using the specified reference point
                    // as a query hint. Return no more than a single result.
                    MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAsync(model.Location_Query, hintPoint, 1).AsTask(cts.Token);

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

            if (result != null && !string.IsNullOrWhiteSpace(result.DisplayName))
                location = this.CreateLocationModel(result, model.WeatherSource);
            else
                location = new LocationQuery();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override partial async Task<bool> IsKeyValid(string key)
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
    }
}
#endif