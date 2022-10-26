using SimpleWeather.ComponentModel;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace SimpleWeather.ViewModels
{
    public class WeatherNowViewModel : BaseViewModel
    {
        private readonly WeatherDataLoader weatherDataLoader = new WeatherDataLoader();
        private readonly WeatherManager wm = WeatherManager.GetInstance();
        private readonly ResourceLoader ResLoader = SharedModule.Instance.ResLoader;

        private readonly LocationProvider locationProvider = new LocationProvider();

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

        public WeatherNowViewModel()
        {
            this.PropertyChanged += WeatherNowViewModel_PropertyChanged;
        }

        private void WeatherNowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UiState))
            {
                this.Weather = UiState?.Weather;
                this.ErrorMessages = UiState?.ErrorMessages;
            }
        }

        private LocationData GetLocationData()
        {
            return UiState?.LocationData;
        }

        public void Initialize(LocationData locationData = null)
        {
            UiState = UiState with { IsLoading = true };

            Task.Run(async () =>
            {
                var locData = locationData ?? await Settings.GetHomeData();

                if (Settings.FollowGPS)
                {
                    if (locData != null && Settings.API != locData.weatherSource)
                    {
                        await Settings.UpdateLocation(LocationData.BuildEmptyGPSLocation());
                    }

                    var result = await UpdateLocation();

                    if (result is LocationResult.Changed)
                    {
                        await Settings.UpdateLocation(result.Data);
                        locData = result.Data;
                    }
                }

                UpdateLocation(locData);
            });
        }

        public void RefreshWeather(bool forceRefresh = false)
        {
            Task.Run(async () =>
            {
                if (Settings.FollowGPS)
                {
                    var locationResult = await UpdateLocation();

                    if (locationResult is LocationResult.Changed)
                    {
                        await Settings.UpdateLocation(locationResult.Data);
                        weatherDataLoader.UpdateLocation(locationResult.Data);
                    }
                }

                var result = await weatherDataLoader.LoadWeatherResult(
                    new WeatherRequest.Builder()
                    .ForceRefresh(forceRefresh)
                    .LoadAlerts()
                    .Build());

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

                        if (state.LocationData?.country_code?.Let(it => !wm.IsRegionSupported(it)) == true)
                        {
                            Logger.WriteLine(LoggerLevel.Warn, "Location: {0}", JSONParser.Serializer(state.LocationData));
                            Logger.WriteLine(LoggerLevel.Warn, new CustomException(ResLoader.GetString("error_message_weather_region_unsupported")));
                        }

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

                        Task.Run(async () =>
                        {
                            ImageData = await weatherData.GetImageData();
                        });
                    }
                    break;
                case WeatherResult.WeatherWithError wrError:
                    {
                        var weatherData = result.Data.ToUiModel();

                        var state = UiState;

                        if (state.LocationData?.country_code?.Let(it => !wm.IsRegionSupported(it)) == true)
                        {
                            Logger.WriteLine(LoggerLevel.Warn, "Location: {0}", JSONParser.Serializer(state.LocationData));
                            Logger.WriteLine(LoggerLevel.Warn, new CustomException(ResLoader.GetString("error_message_weather_region_unsupported")));
                        }

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

                        Task.Run(async () =>
                        {
                            ImageData = await weatherData.GetImageData();
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

            if (Settings.FollowGPS && locationData?.locationType == LocationType.GPS)
            {
                return await locationProvider.GetLatestLocationData();
            }

            return new LocationResult.NotChanged(locationData);
        }

        public void UpdateLocation(LocationData locationData)
        {
            UiState = UiState with { LocationData = locationData };

            if (locationData?.IsValid() == true)
            {
                weatherDataLoader.UpdateLocation(locationData);
                RefreshWeather(false);
            }
            else
            {
                CheckInvalidLocation();

                UiState = UiState with { IsLoading = false };
            }
        }

        private void CheckInvalidLocation()
        {
            var locationData = GetLocationData();

            if (locationData?.IsValid() != true)
            {
                Task.Run(async () =>
                {
                    Logger.WriteLine(LoggerLevel.Warn, "Location: {0}", JSONParser.Serializer(locationData));
                    Logger.WriteLine(LoggerLevel.Warn, "Home: {0}", JSONParser.Serializer(await Settings.GetHomeData()));

                    Logger.WriteLine(LoggerLevel.Warn, new InvalidOperationException("Invalid location data"));
                });

                UiState = UiState with
                {
                    NoLocationAvailable = true,
                    IsLoading = false
                };
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
        LocationData LocationData = null,
        bool NoLocationAvailable = false,
        bool ShowDisconnectedView = false,
        bool IsImageLoading = false
    )
    {
        public IEnumerable<ErrorMessage> ErrorMessages { get; init; } = new List<ErrorMessage>(0);
    }
}
