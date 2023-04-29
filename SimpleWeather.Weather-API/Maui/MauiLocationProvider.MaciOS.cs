#if __IOS__
using CloudKit;
using CoreLocation;
using Foundation;
using Microsoft.Maui.Devices.Sensors;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Bing;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static SimpleWeather.Weather_API.Utils.APIRequestUtils;

namespace SimpleWeather.Weather_API.Maui
{
    public partial class MauiLocationProvider : WeatherLocationProviderImpl, IRateLimitedRequest
    {
        public override string LocationAPI => WeatherAPI.Apple;

        public override async Task<ObservableCollection<LocationQuery>> GetLocations(string ac_query, string weatherAPI)
        {
            ObservableCollection<LocationQuery> locations = null;

            WeatherException wEx = null;
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                this.CheckRateLimit();

                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var geocoder = new CLGeocoder();

                var addresses = (await geocoder.GeocodeAddressAsync(ac_query).WaitAsync(cts.Token))?.Take(10);

                // Load data
                var locationSet = new HashSet<LocationQuery>();

                foreach (var result in addresses)
                {
                    // Filter: only store city results
                    bool added = locationSet.Add(this.CreateLocationModel(result, weatherAPI));

                    // Limit amount of results
                    if (added)
                    {
                        maxResults--;
                        if (maxResults <= 0)
                            break;
                    }
                }

                locations = new ObservableCollection<LocationQuery>(locationSet);
            }
            catch (NSErrorException err)
            {
                if (err.Code == (nint)CLError.Network)
                {
                    this.SetRateLimit(weatherAPI);
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, err);
                }
                else
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown, err);
                }

                Logger.WriteLine(LoggerLevel.Error, err, "MauiLocationProvider: error getting locations");
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "MauiLocationProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQuery>() { new LocationQuery() };

            return locations;
        }

        public override Task<LocationQuery> GetLocationFromID(LocationQuery model)
        {
            return Task.FromResult<LocationQuery>(null);
        }

        public override Task<LocationQuery> GetLocationFromName(LocationQuery model)
        {
            return base.GetLocationFromName(model);
        }

        public override async Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coordinate, string weatherAPI)
        {
            LocationQuery location = null;
            CLPlacemark result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var geocoder = new CLGeocoder();

                var addresses = await geocoder.ReverseGeocodeLocationAsync(new CLLocation(coordinate.Latitude, coordinate.Longitude)).WaitAsync(cts.Token);

                result = addresses.FirstOrDefault();
            }
            catch (NSErrorException err)
            {
                if (err.Code == (nint)CLError.Network)
                {
                    this.SetRateLimit(weatherAPI);
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, err);
                }
                else
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown, err);
                }

                Logger.WriteLine(LoggerLevel.Error, err, "MauiLocationProvider: error getting locations");
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "MauiLocationProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;

            if (result != null)
                location = this.CreateLocationModel(result, weatherAPI);
            else
                location = new LocationQuery();

            return location;
        }
    }
}
#endif