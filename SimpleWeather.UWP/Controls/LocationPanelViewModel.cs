using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SimpleWeather.Controls
{
    public class LocationPanelViewModel : INotifyPropertyChanged
    {
        #region DependencyProperties
        private string locationName;
        private string currTemp;
        private string weatherIcon;
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
            await AsyncTask.RunOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }).ConfigureAwait(true);
        }
        #endregion

        #region Properties
        public string LocationName { get => locationName; set { if (!Equals(locationName, value)) { locationName = value; OnPropertyChanged("LocationName"); } } }
        public string CurrTemp { get => currTemp; set { if (!Equals(locationName, value)) { currTemp = value; OnPropertyChanged("CurrTemp"); } } }
        public string WeatherIcon { get => weatherIcon; set { if (!Equals(locationName, value)) { weatherIcon = value; OnPropertyChanged("WeatherIcon"); } } }
        public bool EditMode { get => editMode; set { if (!Equals(locationName, value)) { editMode = value; OnPropertyChanged("EditMode"); } } }
        public ImageDataViewModel ImageData { get => imageData; set { if (!Equals(imageData, value)) { imageData = value; OnPropertyChanged(nameof(ImageData)); } } }
        public ElementTheme BackgroundTheme { get => backgroundTheme; set { if (!Equals(locationName, value)) { backgroundTheme = value; OnPropertyChanged("BackgroundTheme"); } } }
        public bool IsLoading { get => isLoading; set { if (!Equals(locationName, value)) { isLoading = value; OnPropertyChanged("IsLoading"); } } }
        public LocationData LocationData { get => locationData; set { if (!Equals(locationName, value)) { locationData = value; OnPropertyChanged("LocationData"); } } }
        public string WeatherSource { get => weatherSource; set { if (!Equals(locationName, value)) { weatherSource = value; OnPropertyChanged("WeatherSource"); } } }
        public int LocationType
        {
            get
            {
                if (LocationData != null)
                    return (int)LocationData.locationType;
                return (int)Location.LocationType.Search;
            }
        }
        #endregion

        private readonly WeatherManager wm;
        private Weather weather;

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
            if ((bool)weather?.IsValid() && !Equals(this.weather, weather))
            {
                this.weather = weather;

                ImageData = null;
                BackgroundTheme = ElementTheme.Dark;

                LocationName = weather.location.name;
                CurrTemp = (Settings.IsFahrenheit ?
                    Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
                WeatherIcon = weather.condition.icon;
                WeatherSource = weather.source;

                if (LocationData.query == null)
                {
                    LocationData.query = weather.query;
                    LocationData.latitude = double.Parse(weather.location.latitude);
                    LocationData.longitude = double.Parse(weather.location.longitude);
                    LocationData.weatherSource = weather.source;
                }

                IsLoading = false;
            }
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
