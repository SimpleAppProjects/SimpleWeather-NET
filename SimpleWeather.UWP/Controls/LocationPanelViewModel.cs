using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.System.UserProfile;
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
            await AsyncTask.TryRunOnUIThread(() =>
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

        private readonly WeatherManager wm;
        private Weather weather;
        private string tempUnit;

        public LocationPanelViewModel()
        {
            wm = WeatherManager.GetInstance();
            LocationData = new LocationData();
        }

        public LocationPanelViewModel(Weather weather)
        {
            wm = WeatherManager.GetInstance();
            LocationData = new LocationData();
            SetWeather(weather);
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

                    // Refresh locale/unit dependent values
                    RefreshView();

                    if (LocationData.query == null)
                    {
                        LocationData.query = weather.query;
                        LocationData.latitude = weather.location.latitude.GetValueOrDefault(0.0f);
                        LocationData.longitude = weather.location.longitude.GetValueOrDefault(0.0f);
                        LocationData.weatherSource = weather.source;
                    }

                    IsLoading = false;
                }
                else if (!Equals(tempUnit, Settings.Unit))
                {
                    RefreshView();
                }
            }
        }

        private void RefreshView()
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            tempUnit = Settings.Unit;

            if (weather.condition.temp_f.HasValue && weather.condition.temp_f != weather.condition.temp_c)
            {
                var temp = Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f.Value) : Math.Round(weather.condition.temp_c.Value);
                var unitTemp = Settings.IsFahrenheit ? WeatherIcons.FAHRENHEIT : WeatherIcons.CELSIUS;

                CurrTemp = String.Format(culture, "{0}{1}", temp, unitTemp);
            }
            else
            {
                CurrTemp = "--";
            }

            CurrWeather = weather.condition.weather;

            if (weather.condition.high_f.HasValue && weather.condition.high_f != weather.condition.high_c)
            {
                var temp = Settings.IsFahrenheit ? Math.Round(weather.condition.high_f.Value) : Math.Round(weather.condition.high_c.Value);
                HiTemp = String.Format(culture, "{0}°", temp);
            }
            else
            {
                HiTemp = "--";
            }

            if (weather.condition.low_f.HasValue && weather.condition.low_f != weather.condition.low_c)
            {
                var temp = Settings.IsFahrenheit ? Math.Round(weather.condition.low_f.Value) : Math.Round(weather.condition.low_c.Value);
                LoTemp = String.Format(culture, "{0}°", temp);
            }
            else
            {
                LoTemp = "--";
            }

            // Wind
            if (weather.condition.wind_mph.HasValue && weather.condition.wind_mph >= 0 &&
                weather.condition.wind_degrees.HasValue && weather.condition.wind_degrees >= 0)
            {
                var speedVal = Settings.IsFahrenheit ? Math.Round(weather.condition.wind_mph.Value) : Math.Round(weather.condition.wind_kph.Value);
                var speedUnit = Settings.IsFahrenheit ? "mph" : "kph";

                WindSpeed = String.Format(culture, "{0} {1}", speedVal, speedUnit);
                WindDirection = weather.condition.wind_degrees.Value + 180;
            }
            else
            {
                WindSpeed = "--";
                WindDirection = 0;
            }

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

            WeatherIcon = weather.condition.icon;
            WeatherSource = weather.source;
        }

        public Task UpdateBackground()
        {
            return Task.Run(async () =>
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
            });
        }
    }
}