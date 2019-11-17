using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.ComponentModel;
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
        private string backgroundURI;
        private ElementTheme backgroundTheme = ElementTheme.Dark;
        private bool isLoading = true;
        private LocationData locationData;
        private string weatherSource;

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Properties
        public string LocationName { get => locationName; set { locationName = value; OnPropertyChanged("LocationName"); } }
        public string CurrTemp { get => currTemp; set { currTemp = value; OnPropertyChanged("CurrTemp"); } }
        public string WeatherIcon { get => weatherIcon; set { weatherIcon = value; OnPropertyChanged("WeatherIcon"); } }
        public bool EditMode { get => editMode; set { editMode = value; OnPropertyChanged("EditMode"); } }
        public string BackgroundURI { get => backgroundURI; set { backgroundURI = value; OnPropertyChanged("BackgroundURI"); } }
        public ElementTheme BackgroundTheme { get => backgroundTheme; set { backgroundTheme = value; OnPropertyChanged("BackgroundTheme"); } }
        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged("IsLoading"); } }
        public LocationData LocationData { get => locationData; set { locationData = value; OnPropertyChanged("LocationData"); } }
        public string WeatherSource { get => weatherSource; set { weatherSource = value; OnPropertyChanged("WeatherSource"); } }
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
            // Update background
            BackgroundURI = wm.GetBackgroundURI(weather);
            var PendingBackgroundColor = wm.GetWeatherBackgroundColor(weather);

            BackgroundTheme = ColorUtils.IsSuperLight(PendingBackgroundColor) ?
                ElementTheme.Light : ElementTheme.Dark;

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
}
