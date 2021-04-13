using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Utils;
using SimpleWeather.WeatherData;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SimpleWeather.Controls
{
    public class LocationPanelViewModel : INotifyPropertyChanged
    {
        #region DependencyProperties

        private string locationName;
        private string currTemp;
        private string currWeather;
        private string weatherIcon;
        private string hiTemp;
        private string loTemp;
        private string pop;
        private string popIcon;
        private int windDir;
        private string windSpeed;
        private string windIcon;
        private bool editMode;
        private ImageDataViewModel imageData;
        private ElementTheme backgroundTheme = ElementTheme.Dark;
        private bool isLoading = true;
        private LocationData locationData;
        private string weatherSource;

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected async void OnPropertyChanged(string name)
        {
            await Dispatcher.RunOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }).ConfigureAwait(true);
        }

        #endregion DependencyProperties

        #region Properties

        public string LocationName { get => locationName; set { if (!Equals(locationName, value)) { locationName = value; OnPropertyChanged(nameof(LocationName)); } } }
        public string CurrTemp { get => currTemp; set { if (!Equals(currTemp, value)) { currTemp = value; OnPropertyChanged(nameof(CurrTemp)); } } }
        public string CurrWeather { get => currWeather; set { if (!Equals(currWeather, value)) { currWeather = value; OnPropertyChanged(nameof(CurrWeather)); } } }
        public string HiTemp { get => hiTemp; set { if (!Equals(hiTemp, value)) { hiTemp = value; OnPropertyChanged(nameof(HiTemp)); } } }
        public string LoTemp { get => loTemp; set { if (!Equals(loTemp, value)) { loTemp = value; OnPropertyChanged(nameof(LoTemp)); } } }
        public string PoP { get => pop; set { if (!Equals(pop, value)) { pop = value; OnPropertyChanged(nameof(PoP)); } } }
        public string PoPIcon { get => popIcon; set { if (!Equals(popIcon, value)) { popIcon = value; OnPropertyChanged(nameof(PoPIcon)); } } }
        public string WeatherIcon { get => weatherIcon; set { if (!Equals(weatherIcon, value)) { weatherIcon = value; OnPropertyChanged(nameof(WeatherIcon)); } } }
        public string WindSpeed { get => windSpeed; set { if (!Equals(windSpeed, value)) { windSpeed = value; OnPropertyChanged(nameof(WindSpeed)); } } }
        public int WindDirection { get => windDir; set { if (!Equals(windDir, value)) { windDir = value; OnPropertyChanged(nameof(WindDirection)); } } }
        public string WindIcon { get => windIcon; set { if (!Equals(windIcon, value)) { windIcon = value; OnPropertyChanged(nameof(WindIcon)); } } }
        public bool EditMode { get => editMode; set { if (!Equals(editMode, value)) { editMode = value; OnPropertyChanged(nameof(EditMode)); } } }
        public ImageDataViewModel ImageData { get => imageData; set { if (!Equals(imageData, value)) { imageData = value; OnPropertyChanged(nameof(ImageData)); } } }
        public ElementTheme BackgroundTheme { get => backgroundTheme; set { if (!Equals(backgroundTheme, value)) { backgroundTheme = value; OnPropertyChanged(nameof(BackgroundTheme)); } } }
        public bool IsLoading { get => isLoading; set { if (!Equals(isLoading, value)) { isLoading = value; OnPropertyChanged(nameof(IsLoading)); } } }
        public LocationData LocationData { get => locationData; set { if (!Equals(locationData, value)) { locationData = value; OnPropertyChanged(nameof(LocationData)); } } }
        public string WeatherSource { get => weatherSource; set { if (!Equals(weatherSource, value)) { weatherSource = value; OnPropertyChanged(nameof(WeatherSource)); } } }

        public int LocationType
        {
            get
            {
                if (LocationData != null)
                    return (int)LocationData.locationType;
                return (int)Location.LocationType.Search;
            }
        }

        #endregion Properties

        private CoreDispatcher Dispatcher;

        private readonly WeatherManager wm;
        private Weather weather;
        private string unitCode;

        public LocationPanelViewModel(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            wm = WeatherManager.GetInstance();
            LocationData = new LocationData();
        }

        public void SetWeather(Weather weather)
        {
            if ((bool)weather?.IsValid())
            {
                if (!Equals(this.weather, weather))
                {
                    this.weather = weather;

                    ImageData = null;
                    BackgroundTheme = ElementTheme.Dark;

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

                    if (LocationData?.query == null)
                    {
                        LocationData = new LocationData(weather);
                    }

                    IsLoading = false;

                    // Refresh locale/unit dependent values
                    RefreshView();
                }
                else if (!Equals(unitCode, Settings.UnitString))
                {
                    RefreshView();
                }
            }
        }

        private void RefreshView()
        {
            var isFahrenheit = Units.FAHRENHEIT.Equals(Settings.TemperatureUnit);
            var culture = CultureUtils.UserCulture;
            var provider = WeatherManager.GetProvider(weather.source);

            unitCode = Settings.TemperatureUnit;

            if (weather.condition.temp_f.HasValue && weather.condition.temp_f != weather.condition.temp_c)
            {
                var temp = isFahrenheit ? Math.Round(weather.condition.temp_f.Value) : Math.Round(weather.condition.temp_c.Value);
                var unitTemp = isFahrenheit ? Units.FAHRENHEIT : Units.CELSIUS;

                CurrTemp = String.Format(culture, "{0}°{1}", temp, unitTemp);
            }
            else
            {
                CurrTemp = WeatherIcons.PLACEHOLDER;
            }

            CurrWeather = provider.SupportsWeatherLocale ? weather.condition.weather : provider.GetWeatherCondition(weather.condition.icon);

            if (weather.condition.high_f.HasValue && weather.condition.high_f != weather.condition.high_c)
            {
                var temp = isFahrenheit ? Math.Round(weather.condition.high_f.Value) : Math.Round(weather.condition.high_c.Value);
                HiTemp = String.Format(culture, "{0}°", temp);
            }
            else
            {
                HiTemp = WeatherIcons.PLACEHOLDER;
            }

            if (weather.condition.low_f.HasValue && weather.condition.low_f != weather.condition.low_c)
            {
                var temp = isFahrenheit ? Math.Round(weather.condition.low_f.Value) : Math.Round(weather.condition.low_c.Value);
                LoTemp = String.Format(culture, "{0}°", temp);
            }
            else
            {
                LoTemp = WeatherIcons.PLACEHOLDER;
            }

            // Wind
            if (weather.condition.wind_mph.HasValue && weather.condition.wind_mph >= 0 &&
                weather.condition.wind_degrees.HasValue && weather.condition.wind_degrees >= 0)
            {
                string unit = Settings.SpeedUnit;
                int speedVal;
                string speedUnit;

                switch (unit)
                {
                    case Units.MILES_PER_HOUR:
                    default:
                        speedVal = (int)Math.Round(weather.condition.wind_mph.Value);
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mph");
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(weather.condition.wind_kph.Value);
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_kph");
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(weather.condition.wind_kph.Value));
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_msec");
                        break;
                }

                WindSpeed = String.Format(culture, "{0} {1}", speedVal, speedUnit);
                WindDirection = weather.condition.wind_degrees.Value + 180;
            }
            else
            {
                WindSpeed = null;
                WindDirection = 0;
            }
        }

        public Task UpdateBackground()
        {
            return Task.Run(async () =>
            {
                if (weather != null && ImageData == null)
                {
                    var imageData = await WeatherUtils.GetImageData(weather);

                    if (imageData != null)
                    {
                        ImageData = imageData;
                        BackgroundTheme = ColorUtils.IsSuperLight(imageData.Color) ?
                            ElementTheme.Light : ElementTheme.Dark;
                    }
                    else
                    {
                        ImageData = null;
                        BackgroundTheme = ElementTheme.Dark;
                    }
                }
            });
        }
    }
}