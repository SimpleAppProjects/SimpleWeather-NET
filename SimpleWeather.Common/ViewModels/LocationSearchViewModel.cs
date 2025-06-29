#if WINUI
using CommunityToolkit.WinUI;
#endif
using SimpleWeather.Common.Location;
using SimpleWeather.Common.Utils;
using SimpleWeather.ComponentModel;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.TZDB;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Common.ViewModels
{
    [Bindable(BindableSupport.Default)]
    public partial class LocationSearchViewModel : BaseViewModel, IDisposable
    {
        private readonly WeatherProviderManager wm;
        private readonly SettingsManager SettingsManager;
        private readonly IRemoteConfigService RemoteConfigService;
        private readonly ITZDBService TZDBService;

        private CancellationTokenSource cts = new();

        private readonly LocationProvider locationProvider = new();

        private LocationSearchUiState uistate = new();
        public LocationSearchUiState UiState
        {
            get => uistate;
            private set => SetProperty(ref uistate, value);
        }

        private IEnumerable<ErrorMessage> errorMessages = new List<ErrorMessage>(0);
        public IEnumerable<ErrorMessage> ErrorMessages
        {
            get => errorMessages;
            private set => SetProperty(ref errorMessages, value);
        }

        private LocationData.LocationData currentLocation = null;
        public LocationData.LocationData CurrentLocation
        {
            get => currentLocation;
            private set => SetProperty(ref currentLocation, value);
        }

        private LocationSearchResult selectedSearchLocation = null;
        public LocationSearchResult SelectedSearchLocation
        {
            get => selectedSearchLocation;
            private set => SetProperty(ref selectedSearchLocation, value);
        }

        private IEnumerable<LocationQuery> locations = new List<LocationQuery>(0);
        public IEnumerable<LocationQuery> Locations
        {
            get => locations;
            private set => SetProperty(ref locations, value);
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get => isLoading;
            private set => SetProperty(ref isLoading, value);
        }

        private string footer;
        public string Footer
        {
            get => footer;
            private set => SetProperty(ref footer, value);
        }

        private bool disposedValue;

        public LocationSearchViewModel(
            WeatherProviderManager weatherProviderManager, SettingsManager settingsManager,
            IRemoteConfigService remoteConfigService, ITZDBService tZDBService)
        {
            this.wm = weatherProviderManager;
            this.SettingsManager = settingsManager;
            this.RemoteConfigService = remoteConfigService;
            this.TZDBService = tZDBService;

            PropertyChanged += LocationSearchViewModel_PropertyChanged;
        }

        private void RefreshToken()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
        }

        private void LocationSearchViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UiState))
            {
                IsLoading = UiState?.IsLoading ?? false;
                ErrorMessages = UiState?.ErrorMessages;
                CurrentLocation = UiState?.CurrentLocation;
                SelectedSearchLocation = UiState?.SelectedSearchLocation;
                Locations = UiState?.Locations;
            }
        }

        public void Initialize()
        {
            var locationAPI = wm.LocationProvider.LocationAPI;

            var entry = WeatherAPI.LocationAPIs.First(lapi => locationAPI.Equals(lapi.Value));
            var credit = $"{ResStrings.credit_prefix} {entry.ToString() ?? WeatherIcons.EM_DASH}";
            Footer = credit;
        }

        public void FetchGeoLocation()
        {
            UiState = UiState with { IsLoading = true };

            RefreshToken();
            Task.Run(async () =>
            {
                var result = await locationProvider.GetLatestLocationData();
                LocationData.LocationData currentLocation = null;

                switch (result)
                {
                    case LocationResult.Changed:
                        currentLocation = result.Data;
                        break;
                    case LocationResult.NotChanged:
                        if (result.Data?.IsValid() == true)
                        {
                            currentLocation = result.Data;
                        }
                        else
                        {
                            PostErrorMessage(new ErrorMessage.String(ResStrings.error_retrieve_location));
                        }
                        break;
                    case LocationResult.PermissionDenied:
                        PostErrorMessage(new ErrorMessage.String(ResStrings.error_location_denied));
                        break;
                    case LocationResult.Error err:
                        PostErrorMessage(err.ErrorMessage);
                        break;
                    default:
                        PostErrorMessage(new ErrorMessage.String(ResStrings.error_retrieve_location));
                        break;
                }

                if (currentLocation?.IsValid() == true)
                {
                    var locQuery = new LocationQuery(currentLocation);

                    if (!SettingsManager.WeatherLoaded)
                    {
                        // Set default provider based on location
                        var provider = RemoteConfigService.GetDefaultWeatherProvider(locQuery);
                        SettingsManager.API = provider;
                        locQuery.UpdateWeatherSource(provider);
                        wm.UpdateAPI();
                    }

                    if (SettingsManager.UsePersonalKeys[SettingsManager.API] && string.IsNullOrWhiteSpace(SettingsManager.APIKey) && wm.KeyRequired)
                    {
                        PostErrorMessage(new ErrorMessage.String(ResStrings.werror_invalidkey));
                        UiState = UiState with { IsLoading = false };
                        return;
                    }

                    UiState = UiState with { CurrentLocation = locQuery.ToLocationData(LocationType.GPS) };
                }

                UiState = UiState with { IsLoading = false };
            }, cts.Token);
        }

        public void FetchLocations(string queryString)
        {
            UiState = UiState with { IsLoading = true };

            RefreshToken();
            Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    try
                    {
                        var results = await wm.GetLocations(queryString);

                        UiState = UiState with
                        {
                            Locations = results ?? new ObservableCollection<LocationQuery>()
                        };
                    }
                    catch (Exception e)
                    {
                        if (e is WeatherException wEx)
                        {
                            PostErrorMessage(new ErrorMessage.WeatherError(wEx));
                        }

                        UiState = UiState with { Locations = ImmutableList.Create(new LocationQuery()) };
                    }
                }
                else
                {
                    UiState = UiState with { Locations = new List<LocationQuery>(0) };
                }

                UiState = UiState with { IsLoading = false };
            }, cts.Token);
        }

        public void OnLocationSelected(LocationQuery locQuery)
        {
            UiState = UiState with { IsLoading = true };

            RefreshToken();
#if WINUI
            DispatcherQueue.EnqueueAsync(async () =>
#else
            Dispatcher.Dispatch(async () =>
#endif
            {
                if (string.IsNullOrWhiteSpace(locQuery.Location_Query))
                {
                    PostErrorMessage(new ErrorMessage.String(ResStrings.error_retrieve_location));
                    UiState = UiState with { IsLoading = false };
                    return;
                }

                if (SettingsManager.UsePersonalKeys[SettingsManager.API] && string.IsNullOrWhiteSpace(SettingsManager.APIKey) && wm.KeyRequired)
                {
                    PostErrorMessage(new ErrorMessage.String(ResStrings.werror_invalidkey));
                    UiState = UiState with { IsLoading = false };
                    return;
                }

                var queryResult = locQuery;

                // Need to get FULL location data for HERE API
                // Data provided is incomplete
                if (wm.LocationProvider.NeedsLocationFromID)
                {
                    queryResult = await Task.Run(async () =>
                    {
                        return await wm.LocationProvider.GetLocationFromID(locQuery);
                    }, cts.Token);
                }
                else if (wm.LocationProvider.NeedsLocationFromName)
                {
                    queryResult = await Task.Run(async () =>
                    {
                        return await wm.LocationProvider.GetLocationFromName(locQuery);
                    }, cts.Token);
                }
                else if (wm.LocationProvider.NeedsLocationFromGeocoder)
                {
                    queryResult = await Task.Run(async () =>
                    {
                        return await wm.LocationProvider.GetLocation(
                            new WeatherUtils.Coordinate(locQuery.LocationLat, locQuery.LocationLong),
                            locQuery.WeatherSource
                        );
                    }, cts.Token);
                }

                if (queryResult == null || string.IsNullOrWhiteSpace(queryResult.Location_Query))
                {
                    // Stop since there is no valid query
                    PostErrorMessage(new ErrorMessage.String(ResStrings.error_retrieve_location));
                    UiState = UiState with { IsLoading = false };
                    return;
                }
                else if (string.IsNullOrWhiteSpace(queryResult.LocationTZLong) && queryResult.LocationLat != 0 && queryResult.LocationLong != 0)
                {
                    var tzId = await Task.Run(() =>
                    {
                        return TZDBService.GetTimeZone(queryResult.LocationLat, queryResult.LocationLong);
                    }, cts.Token);

                    if (!Equals("unknown", tzId))
                    {
                        queryResult.LocationTZLong = tzId;
                    }
                }

                if (!SettingsManager.WeatherLoaded)
                {
                    // Set default provider based on location
                    var provider = RemoteConfigService.GetDefaultWeatherProvider(queryResult);
                    SettingsManager.API = provider;
                    queryResult.UpdateWeatherSource(provider);
                    wm.UpdateAPI();
                }

                // Check if location already exists
                var locData = await SettingsManager.GetLocationData();
                var loc = locData.FirstOrDefault(input => Equals(input.query, queryResult.Location_Query));
                LocationSearchResult result;

                if (loc == null)
                {
                    // Location does not exist
                    result = new LocationSearchResult.Success(queryResult.ToLocationData());
                }
                else
                {
                    result = new LocationSearchResult.AlreadyExists(loc);
                }

                UiState = UiState with
                {
                    SelectedSearchLocation = result,
                    IsLoading = false
                };
            });
        }

        private void PostErrorMessage(ErrorMessage error)
        {
            var state = UiState;

            var errorMessages = new List<ErrorMessage>(state.ErrorMessages) { error };

            UiState = state with
            {
                ErrorMessages = errorMessages
            };
        }

        public void SetErrorMessageShown(ErrorMessage error)
        {
            var state = UiState;

            UiState = state with
            {
                ErrorMessages = state.ErrorMessages?.WhereNot(it => it == error)
            };
        }

        public override void OnCleared()
        {
            base.OnCleared();
            PropertyChanged -= LocationSearchViewModel_PropertyChanged;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    cts?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~LocationSearchViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public interface LocationSearchResult
    {
        LocationData.LocationData Data { get; }

        public record Success(LocationData.LocationData Data) : LocationSearchResult;
        public record AlreadyExists(LocationData.LocationData Data) : LocationSearchResult;
        public record Failed(LocationData.LocationData? Data) : LocationSearchResult;
    }

    public record LocationSearchUiState(
        bool IsLoading = false,
        LocationData.LocationData CurrentLocation = null,
        LocationSearchResult SelectedSearchLocation = null
    )
    {
        public IEnumerable<ErrorMessage> ErrorMessages { get; init; } = new List<ErrorMessage>(0);
        public IEnumerable<LocationQuery> Locations { get; init; } = new List<LocationQuery>(0);
    }
}
