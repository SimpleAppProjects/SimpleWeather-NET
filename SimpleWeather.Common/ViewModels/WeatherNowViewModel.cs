#if WINUI
using CommunityToolkit.WinUI;
#endif
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.Location;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.ComponentModel;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Common.ViewModels
{
    [Bindable(true)]
    public partial class WeatherNowViewModel : BaseViewModel
    {
        private readonly SettingsManager SettingsManager;
        private readonly WeatherProviderManager wm;
        private readonly WeatherDataLoader weatherDataLoader = new();

        private readonly LocationProvider locationProvider = new();

        private WeatherNowState uistate = new() { IsLoading = true, NoLocationAvailable = true };
        public WeatherNowState UiState
        {
            get => uistate;
            private set => SetProperty(ref uistate, value);
        }

        private WeatherUiModel weather = null;
        public WeatherUiModel Weather
        {
            get => weather;
            private set => SetProperty(ref weather, value);
        }

        private ImageDataViewModel imageData = null;
        public ImageDataViewModel ImageData
        {
            get => imageData;
            private set => SetProperty(ref imageData, value);
        }

        private ICollection<WeatherAlert> alerts = new List<WeatherAlert>(0);
        public ICollection<WeatherAlert> Alerts
        {
            get => alerts;
            private set => SetProperty(ref alerts, value);
        }

        private IEnumerable<ErrorMessage> errorMessages = new List<ErrorMessage>(0);
        public IEnumerable<ErrorMessage> ErrorMessages
        {
            get => errorMessages;
            private set => SetProperty(ref errorMessages, value);
        }

        private bool isInitialized = false;
        public bool IsInitialized
        {
            get => isInitialized;
            private set => SetProperty(ref isInitialized, value);
        }

        public WeatherNowViewModel(SettingsManager settingsManager, WeatherProviderManager weatherProviderManager)
        {
            this.SettingsManager = settingsManager;
            this.wm = weatherProviderManager;

            PropertyChanged += WeatherNowViewModel_PropertyChanged;
        }

        private void WeatherNowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UiState))
            {
                Weather = UiState?.Weather;
                ErrorMessages = UiState?.ErrorMessages;
                IsInitialized = UiState?.IsInitialized ?? false;
            }
        }

        private LocationData.LocationData GetLocationData()
        {
            return UiState?.LocationData;
        }

        public void Initialize(LocationData.LocationData locationData = null)
        {
            UiState = UiState with { IsLoading = true };

#if WINUI
            DispatcherQueue.EnqueueAsync(async () =>
#else
            Dispatcher.Dispatch(async () =>
#endif
            {
                var locData = locationData ?? await SettingsManager.GetHomeData();

                if (SettingsManager.FollowGPS)
                {
                    if (locData != null && SettingsManager.API != locData.weatherSource)
                    {
                        await SettingsManager.UpdateLocation(LocationData.LocationData.BuildEmptyGPSLocation());
                    }

                    var result = await UpdateLocation();

                    if (result is LocationResult.Changed)
                    {
                        await SettingsManager.UpdateLocation(result.Data);
                        locData = result.Data;
                    }
                }

                UpdateLocation(locData);

                UiState = UiState with { IsInitialized = true };
            });
        }

        public void RefreshWeather(bool forceRefresh = false)
        {
            UiState = UiState with { IsLoading = true };

#if WINUI
            DispatcherQueue.EnqueueAsync(async () =>
#else
            Dispatcher.Dispatch(async () =>
#endif
            {
                if (SettingsManager.FollowGPS)
                {
                    var locationResult = await UpdateLocation();

                    if (locationResult is LocationResult.Changed)
                    {
                        await SettingsManager.UpdateLocation(locationResult.Data);
                        weatherDataLoader.UpdateLocation(locationResult.Data);
                    }
                    else if (locationResult is LocationResult.NotChanged)
                    {
                        locationResult?.Data?.Let(data =>
                        {
                            if (data.IsValid())
                            {
                                if (!weatherDataLoader.IsLocationValid())
                                {
                                    weatherDataLoader.UpdateLocation(data);
                                    UiState = UiState with { LocationData = data };
                                }
                            }
                        });
                    }
                }

                WeatherResult result;

                if (weatherDataLoader.IsLocationValid())
                {
                    result = await weatherDataLoader.LoadWeatherResult(
                        new WeatherRequest.Builder()
                        .ForceRefresh(forceRefresh)
                        .LoadAlerts()
                        .Apply(it =>
                        {
                            if (forceRefresh)
                            {
                                it.LoadForecasts();
                            }
                        })
                        .Build());
                }
                else
                {
                    result = new WeatherResult.NoWeather();
                }

                UpdateWeatherState(result);
            });
        }

        private void UpdateWeatherState(WeatherResult result)
        {
            switch (result)
            {
                case WeatherResult.Error wrError:
                    {
                        var state = UiState;

                        var errorMessages = new List<ErrorMessage>(state.ErrorMessages)
                        {
                            new ErrorMessage.WeatherError(wrError.Exception)
                        };

                        UiState = state with
                        {
                            ErrorMessages = errorMessages,
                            IsLoading = false,
                            NoLocationAvailable = false
                        };
                    }
                    break;
                case WeatherResult.NoWeather:
                    {
                        var state = UiState;

                        var errorMessages = new List<ErrorMessage>(state.ErrorMessages)
                        {
                            new ErrorMessage.WeatherError(new WeatherException(WeatherUtils.ErrorStatus.NoWeather))
                        };

                        UiState = state with
                        {
                            ErrorMessages = errorMessages,
                            IsLoading = false,
                            NoLocationAvailable = false
                        };
                    }
                    break;
                case WeatherResult.Success:
                    {
                        var weatherData = result.Data.ToUiModel();

                        var state = UiState;

                        UiState = state with
                        {
                            Weather = weatherData,
                            IsLoading = false,
                            NoLocationAvailable = false,
                            IsGPSLocation = state.LocationData?.locationType == LocationType.GPS
                        };

                        Alerts = result.Data.weather_alerts;

#if WINUI
                        DispatcherQueue.EnqueueAsync(async () =>
#else
                        Dispatcher.Dispatch(async () =>
#endif
                        {
                            ImageData = await Task.Run(weatherData.GetImageData);
                        });
                    }
                    break;
                case WeatherResult.WeatherWithError wrError:
                    {
                        var weatherData = result.Data.ToUiModel();

                        var state = UiState;

                        var errorMessages = new List<ErrorMessage>(state.ErrorMessages)
                        {
                            new ErrorMessage.WeatherError(wrError.Exception)
                        };

                        UiState = state with
                        {
                            Weather = weatherData,
                            ErrorMessages = errorMessages,
                            IsLoading = false,
                            NoLocationAvailable = false,
                            IsGPSLocation = state.LocationData?.locationType == LocationType.GPS
                        };

                        Alerts = result.Data.weather_alerts;

#if WINUI
                        DispatcherQueue.EnqueueAsync(async () =>
#else
                        Dispatcher.Dispatch(async () =>
#endif
                        {
                            ImageData = await Task.Run(weatherData.GetImageData);
                        });
                    }
                    break;
            }
        }

        public void SetErrorMessageShown(ErrorMessage error)
        {
            var state = UiState;

            UiState = state with
            {
                ErrorMessages = state.ErrorMessages?.WhereNot(it => it == error)
            };
        }

        private async Task<LocationResult> UpdateLocation()
        {
            var locationData = GetLocationData();

            if (SettingsManager.FollowGPS && (locationData == null || locationData.locationType == LocationType.GPS))
            {
                return await locationProvider.GetLatestLocationData();
            }

            return new LocationResult.NotChanged(locationData);
        }

        public void UpdateLocation(LocationData.LocationData locationData)
        {
            UiState = UiState with { LocationData = locationData, NoLocationAvailable = false };

            if (locationData?.IsValid() == true)
            {
                weatherDataLoader.UpdateLocation(locationData);
                RefreshWeather(false);
            }
            else
            {
                CheckInvalidLocation(locationData);

                UiState = UiState with { IsLoading = false };
            }
        }

        private void CheckInvalidLocation(LocationData.LocationData locationData)
        {
            if (locationData == null || !locationData.IsValid())
            {
#if WINUI
                DispatcherQueue.EnqueueAsync(async () =>
#else
                Dispatcher.Dispatch(async () =>
#endif
                {
                    await Task.Run(async () =>
                    {
                        Logger.WriteLine(LoggerLevel.Warn, "Location: {0}", JSONParser.Serializer(locationData));
                        Logger.WriteLine(LoggerLevel.Warn, "Home: {0}", JSONParser.Serializer(await SettingsManager.GetHomeData()));

                        Logger.WriteLine(LoggerLevel.Warn, new InvalidOperationException("Invalid location data"));
                    });

                    UiState = UiState with
                    {
                        NoLocationAvailable = true,
                        IsLoading = false
                    };
                });
            }
        }

        public void OnImageLoading()
        {
            UiState = UiState with { IsImageLoading = true };
        }

        public void OnImageLoaded()
        {
            UiState = UiState with { IsImageLoading = false };
        }
    }

    public record WeatherNowState(
        WeatherUiModel Weather = null,
        bool IsLoading = false,
        bool IsGPSLocation = false,
        LocationData.LocationData LocationData = null,
        bool NoLocationAvailable = false,
        bool ShowDisconnectedView = false,
        bool IsImageLoading = false,
        bool IsInitialized = false
    )
    {
        public IEnumerable<ErrorMessage> ErrorMessages { get; init; } = new List<ErrorMessage>(0);
    }
}
