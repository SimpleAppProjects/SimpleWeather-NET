using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.Images;
using SimpleWeather.ComponentModel;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using System.ComponentModel;
using ResUnits = SimpleWeather.Resources.Strings.Units;

namespace SimpleWeather.NET.Controls
{
    [Bindable(true)]
    public class LocationPanelUiModel : BaseViewModel
    {
        #region Backing Properties
        private string locationName;
        private string currTemp;
        private string currWeather;
        private string hiTemp;
        private string loTemp;
        private bool isShowHiLo = false;
        private string poP;
        private string poPIcon;
        private string weatherIcon;
        private string windSpeed;
        private int windDirection;
        private string windIcon;
        private bool editMode;
        private ImageDataViewModel imageData;
        private bool isLoading = true;
        private LocationData.LocationData locationData = new();
        private string weatherSource;
        private bool isWeatherValid = false;
        #endregion

        #region Properties
        public string LocationName
        {
            get => locationName;
            private set => SetProperty(ref locationName, value);
        }
        public string CurrTemp
        {
            get => currTemp;
            private set => SetProperty(ref currTemp, value);
        }
        public string CurrWeather
        {
            get => currWeather;
            private set => SetProperty(ref currWeather, value);
        }
        public string HiTemp
        {
            get => hiTemp;
            private set => SetProperty(ref hiTemp, value);
        }
        public string LoTemp
        {
            get => loTemp;
            private set => SetProperty(ref loTemp, value);
        }
        public bool IsShowHiLo
        {
            get => isShowHiLo;
            private set => SetProperty(ref isShowHiLo, value);
        }
        public string PoP
        {
            get => poP;
            private set => SetProperty(ref poP, value);
        }
        public string PoPIcon
        {
            get => poPIcon;
            private set => SetProperty(ref poPIcon, value);
        }
        public string WeatherIcon
        {
            get => weatherIcon;
            private set => SetProperty(ref weatherIcon, value);
        }
        public string WindSpeed
        {
            get => windSpeed;
            private set => SetProperty(ref windSpeed, value);
        }
        public int WindDirection
        {
            get => windDirection;
            private set => SetProperty(ref windDirection, value);
        }
        public string WindIcon
        {
            get => windIcon;
            private set => SetProperty(ref windIcon, value);
        }
        public bool EditMode
        {
            get => editMode;
            set => SetProperty(ref editMode, value);
        }
        public ImageDataViewModel ImageData
        {
            get => imageData;
            private set => SetProperty(ref imageData, value);
        }
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }
        public LocationData.LocationData LocationData
        {
            get => locationData;
            set => SetProperty(ref locationData, value);
        }
        public string WeatherSource
        {
            get => weatherSource;
            private set => SetProperty(ref weatherSource, value);
        }
        public bool IsWeatherValid
        {
            get => isWeatherValid;
            private set => SetProperty(ref isWeatherValid, value);
        }

        public int LocationType
        {
            get
            {
                if (LocationData != null)
                    return (int)LocationData.locationType;
                return (int)SimpleWeather.LocationData.LocationType.Search;
            }
        }

        private Weather weather;
        private string unitCode;
        #endregion

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public void SetWeather(LocationData.LocationData locationData, Weather weather)
        {
            if (weather?.IsValid() == true)
            {
                if (!Equals(this.weather, weather))
                {
                    this.weather = weather;

                    ImageData = null;

                    LocationName = weather.location.name;

                    if (weather.precipitation != null)
                    {
                        if (weather.precipitation.pop.HasValue)
                        {
                            PoP = weather.precipitation.pop.Value + "%";
                            PoPIcon = WeatherIcons.UMBRELLA;
                        }
                        else if (weather.precipitation.cloudiness.HasValue)
                        {
                            PoP = weather.precipitation.cloudiness.Value + "%";
                            PoPIcon = WeatherIcons.CLOUDY;
                        }
                    }
                    else
                    {
                        PoP = null;
                    }

                    WindIcon = WeatherIcons.WIND_DIRECTION;

                    WeatherIcon = weather.condition.icon;
                    WeatherSource = weather.source;

                    LocationData = locationData;

                    IsLoading = false;

                    // Refresh locale/unit dependent values
                    RefreshView();
                }
                else if (!Equals(unitCode, SettingsManager.UnitString))
                {
                    RefreshView();
                }

                IsWeatherValid = true;
            }
            else
            {
                LocationData = locationData;
                LocationName = locationData.name;
                WeatherSource = locationData.weatherSource;

                IsWeatherValid = false;
            }
        }

        private void RefreshView()
        {
            var isFahrenheit = Units.FAHRENHEIT.Equals(SettingsManager.TemperatureUnit);
            var culture = LocaleUtils.GetLocale();
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(weather.source);

            unitCode = SettingsManager.TemperatureUnit;

            if (weather.condition.temp_f.HasValue && weather.condition.temp_f != weather.condition.temp_c)
            {
                var temp = isFahrenheit ? Math.Round(weather.condition.temp_f.Value) : Math.Round(weather.condition.temp_c.Value);
                var unitTemp = isFahrenheit ? Units.FAHRENHEIT : Units.CELSIUS;

                CurrTemp = string.Format(culture, "{0}°{1}", temp, unitTemp);
            }
            else
            {
                CurrTemp = WeatherIcons.PLACEHOLDER;
            }

            CurrWeather = provider.SupportsWeatherLocale ? weather.condition.weather : provider.GetWeatherCondition(weather.condition.icon);

            if (weather.condition.high_f.HasValue && weather.condition.high_f != weather.condition.high_c)
            {
                var temp = isFahrenheit ? Math.Round(weather.condition.high_f.Value) : Math.Round(weather.condition.high_c.Value);
                HiTemp = string.Format(culture, "{0}°", temp);
            }
            else
            {
                HiTemp = WeatherIcons.PLACEHOLDER;
            }

            if (weather.condition.low_f.HasValue && weather.condition.low_f != weather.condition.low_c)
            {
                var temp = isFahrenheit ? Math.Round(weather.condition.low_f.Value) : Math.Round(weather.condition.low_c.Value);
                LoTemp = string.Format(culture, "{0}°", temp);
            }
            else
            {
                LoTemp = WeatherIcons.PLACEHOLDER;
            }

            IsShowHiLo = !Equals(HiTemp, LoTemp);

            // Wind
            if (weather.condition.wind_mph.HasValue && weather.condition.wind_mph >= 0 &&
                weather.condition.wind_degrees.HasValue && weather.condition.wind_degrees >= 0)
            {
                string unit = SettingsManager.SpeedUnit;
                int speedVal;
                string speedUnit;

                switch (unit)
                {
                    case Units.MILES_PER_HOUR:
                    default:
                        speedVal = (int)Math.Round(weather.condition.wind_mph.Value);
                        speedUnit = ResUnits.unit_mph;
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(weather.condition.wind_kph.Value);
                        speedUnit = ResUnits.unit_kph;
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(weather.condition.wind_kph.Value));
                        speedUnit = ResUnits.unit_msec;
                        break;
                    case Units.KNOTS:
                        speedVal = (int)Math.Round(ConversionMethods.MphToKts(weather.condition.wind_mph.Value));
                        speedUnit = ResUnits.unit_knots;
                        break;
                }

                WindSpeed = string.Format(culture, "{0} {1}", speedVal, speedUnit);
                WindDirection = weather.condition.wind_degrees.Value + 180;
            }
            else
            {
                WindSpeed = null;
                WindDirection = 0;
            }
        }

        public async Task UpdateBackground()
        {
            if (ImageData == null)
            {
                ImageData = await (weather?.GetImageData() ?? Task.FromResult<ImageDataViewModel>(default));
            }
        }

        public override bool Equals(object obj)
        {
            return obj is LocationPanelUiModel model &&
                   EqualityComparer<ImageDataViewModel>.Default.Equals(imageData, model.imageData) &&
                   EqualityComparer<LocationData.LocationData>.Default.Equals(locationData, model.locationData) &&
                   EqualityComparer<Weather>.Default.Equals(weather, model.weather);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(imageData, locationData, weather);
        }
    }
}