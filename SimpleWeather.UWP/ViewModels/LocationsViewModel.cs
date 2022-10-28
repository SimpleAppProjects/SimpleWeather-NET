using Microsoft.Toolkit.Uwp;
using SimpleWeather.ComponentModel;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.ViewModels;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static SimpleWeather.Utils.WeatherUtils;

namespace SimpleWeather.UWP.ViewModels
{
    public class LocationsViewModel : BaseViewModel
    {
        private readonly WeatherManager wm = WeatherManager.GetInstance();
        private readonly LocationProvider locationProvider = new();

        private readonly bool[] ErrorCounter = new bool[Enum.GetValues(typeof(WeatherUtils.ErrorStatus)).Length];

        private LocationsUiState uistate = new() { IsLoading = true };
        public LocationsUiState UiState
        {
            get => uistate;
            private set => SetProperty(ref uistate, value);
        }

        private IEnumerable<LocationPanelUiModel> locations = null;
        public IEnumerable<LocationPanelUiModel> Locations
        {
            get => locations;
            private set => SetProperty(ref locations, value);
        }

        private IEnumerable<ErrorMessage> errorMessages = new List<ErrorMessage>(0);
        public IEnumerable<ErrorMessage> ErrorMessages
        {
            get => errorMessages;
            private set => SetProperty(ref errorMessages, value);
        }

        public event EventHandler<WeatherUpdatedEventArgs> WeatherUpdated;

        public LocationsViewModel()
        {
            this.PropertyChanged += LocationsViewModel_PropertyChanged;
        }

        private void LocationsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UiState))
            {
                this.Locations = UiState?.Locations;
                this.ErrorMessages = UiState?.ErrorMessages;
            }
        }

        public void LoadLocations()
        {
            UiState = UiState with { IsLoading = true };

            DispatcherQueue.EnqueueAsync(async () =>
            {
                var locations = (await Settings.GetFavorites()).ToList();

                if (Settings.FollowGPS)
                {
                    (await GetGPSData())?.Let(loc =>
                    {
                        locations.Insert(0, loc);
                    });
                }

                RefreshLocationWeather(locations);
            });
        }

        public void RefreshLocations()
        {
            DispatcherQueue.EnqueueAsync(async () =>
            {
                var locations = (await Settings.GetFavorites()).ToList();

                if (Settings.FollowGPS)
                {
                    (await GetGPSData())?.Let(loc =>
                    {
                        locations.Insert(0, loc);
                    });
                }

                var currentLocations = UiState.Locations;

                var reload = locations.Count != currentLocations.Count() || (locations.Count > 0 && locations.First().weatherSource != Settings.API);

                if (reload)
                {
                    LoadLocations();
                    return;
                }

                RefreshLocationWeather(locations);
            });
        }

        private void RefreshLocationWeather(IEnumerable<LocationData> locations)
        {
            var locationMap = locations.ToDictionary(_ => _, locData => new LocationPanelUiModel() { LocationData = locData });

            UiState = UiState with { Locations = locationMap.Values, IsLoading = false };

            Task.Run(async () =>
            {
                foreach (var entry in locationMap)
                {
                    var result = await new WeatherDataLoader(entry.Key)
                        .LoadWeatherResult(
                            new WeatherRequest.Builder()
                                .ForceRefresh(false)
                                .Build()
                        );

                    switch (result)
                    {
                        case WeatherResult.Error wrError:
                            {
                                var state = UiState;

                                // Show error message and only warn once
                                if (!ErrorCounter[(int)wrError.Exception.ErrorStatus])
                                {
                                    ErrorCounter[(int)wrError.Exception.ErrorStatus] = true;

                                    var errorMessages = new List<ErrorMessage>(state.ErrorMessages)
                                    {
                                        new ErrorMessage.WeatherError(wrError.Exception)
                                    };

                                    UiState = state with { ErrorMessages = errorMessages };
                                }

                                entry.Value.SetWeather(entry.Key, null);

                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    WeatherUpdated?.Invoke(this, new WeatherUpdatedEventArgs(entry.Value));
                                });
                            }
                            break;
                        case WeatherResult.NoWeather:
                            {
                                var state = UiState;

                                // Show error message and only warn once
                                if (!ErrorCounter[(int)ErrorStatus.NoWeather])
                                {
                                    ErrorCounter[(int)ErrorStatus.NoWeather] = true;

                                    var errorMessages = new List<ErrorMessage>(state.ErrorMessages)
                                    {
                                        new ErrorMessage.WeatherError(new WeatherException(ErrorStatus.NoWeather))
                                    };

                                    UiState = state with { ErrorMessages = errorMessages };
                                }

                                entry.Value.SetWeather(entry.Key, null);

                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    WeatherUpdated?.Invoke(this, new WeatherUpdatedEventArgs(entry.Value));
                                });
                            }
                            break;
                        case WeatherResult.Success:
                            {
                                entry.Value.SetWeather(entry.Key, result.Data);

                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    WeatherUpdated?.Invoke(this, new WeatherUpdatedEventArgs(entry.Value));
                                });
                            }
                            break;
                        case WeatherResult.WeatherWithError wrError:
                            {
                                var state = UiState;

                                // Show error message and only warn once
                                if (!ErrorCounter[(int)wrError.Exception.ErrorStatus])
                                {
                                    ErrorCounter[(int)wrError.Exception.ErrorStatus] = true;

                                    var errorMessages = new List<ErrorMessage>(state.ErrorMessages)
                                    {
                                        new ErrorMessage.WeatherError(wrError.Exception)
                                    };

                                    UiState = state with { ErrorMessages = errorMessages };
                                }

                                entry.Value.SetWeather(entry.Key, result.Data);

                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    WeatherUpdated?.Invoke(this, new WeatherUpdatedEventArgs(entry.Value));
                                });
                            }
                            break;
                    }
                }
            });
        }

        private async Task<LocationData> GetGPSData()
        {
            if (Settings.FollowGPS)
            {
                var locData = await Settings.GetLastGPSLocData();

                if (locData?.IsValid() == true)
                {
                    return locData;
                }
                else
                {
                    var result = await UpdateLocation();

                    switch (result)
                    {
                        case LocationResult.Changed:
                            {
                                // update location to system
                                Settings.SaveLastGPSLocData(result.Data);
                                SharedModule.Instance.RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE);
                            }
                            break;
                        case LocationResult.NotChanged:
                        case LocationResult.ChangedInvalid:
                            if (result.Data?.IsValid() == true)
                            {
                                return result.Data;
                            }
                            break;
                        case LocationResult.Error:
                        case LocationResult.PermissionDenied:
                            // propagate error to frontend
                            break;
                    }
                }
            }

            return null;
        }

        private async Task<LocationResult> UpdateLocation()
        {
            if (Settings.FollowGPS)
            {
                if (!await locationProvider.CheckPermissions())
                {
                    return new LocationResult.PermissionDenied();
                }

                return await locationProvider.GetLatestLocationData();
            }

            return new LocationResult.NotChanged(null);
        }

        public void SetErrorMessageShown(ErrorMessage error)
        {
            var state = UiState;

            UiState = state with
            {
                ErrorMessages = state.ErrorMessages?.WhereNot(it => it == error)
            };
        }
    }

    public record LocationsUiState(bool IsLoading = false)
    {
        public IEnumerable<LocationPanelUiModel> Locations { get; init; } = new List<LocationPanelUiModel>(0);
        public IEnumerable<ErrorMessage> ErrorMessages { get; init; } = new List<ErrorMessage>(0);
    }

    public sealed class WeatherUpdatedEventArgs : EventArgs
    {
        public LocationPanelUiModel Location { get; }

        internal WeatherUpdatedEventArgs(LocationPanelUiModel uiModel)
        {
            Location = uiModel;
        }
    }
}
