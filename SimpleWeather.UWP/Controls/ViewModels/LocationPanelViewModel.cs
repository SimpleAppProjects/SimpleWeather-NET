using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.ComponentModel;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SimpleWeather.Controls
{
    public partial class LocationPanelViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string locationName;
        [ObservableProperty]
        private string currTemp;
        [ObservableProperty]
        private string currWeather;
        [ObservableProperty]
        private string hiTemp;
        [ObservableProperty]
        private string loTemp;
        [ObservableProperty]
        private string poP;
        [ObservableProperty]
        private string poPIcon;
        [ObservableProperty]
        private string weatherIcon;
        [ObservableProperty]
        private string windSpeed;
        [ObservableProperty]
        private int windDirection;
        [ObservableProperty]
        private string windIcon;
        [ObservableProperty]
        private bool editMode;
        [ObservableProperty]
        private ImageDataViewModel imageData;
        [ObservableProperty]
        private ElementTheme backgroundTheme = ElementTheme.Dark;
        [ObservableProperty]
        private bool isLoading = true;
        [ObservableProperty]
        private LocationData locationData = new LocationData();
        [ObservableProperty]
        private string weatherSource;

        public int LocationType
        {
            get
            {
                if (LocationData != null)
                    return (int)LocationData.locationType;
                return (int)Location.LocationType.Search;
            }
        }

        private readonly WeatherManager wm = WeatherManager.GetInstance();
        private Weather weather;
        private string unitCode;

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
                        speedUnit = SharedModule.Instance.ResLoader.GetString("/Units/unit_mph");
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(weather.condition.wind_kph.Value);
                        speedUnit = SharedModule.Instance.ResLoader.GetString("/Units/unit_kph");
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(weather.condition.wind_kph.Value));
                        speedUnit = SharedModule.Instance.ResLoader.GetString("/Units/unit_msec");
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

        public async Task UpdateBackground()
        {
            if (weather != null && ImageData == null)
            {
                var imageData = await weather.GetImageData();

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
        }
    }
}