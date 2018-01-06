using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.Controls
{
    public class LocationPanelViewModel : DependencyObject, INotifyPropertyChanged
    {
        #region DependencyProperties
        public static readonly DependencyProperty LocationNameProperty =
            DependencyProperty.Register("LocationName", typeof(String),
            typeof(LocationPanelViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty CurrTempProperty =
            DependencyProperty.Register("CurrTemp", typeof(String),
            typeof(LocationPanelViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty WeatherIconProperty =
            DependencyProperty.Register("WeatherIcon", typeof(String),
            typeof(LocationPanelViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register("EditMode", typeof(bool),
            typeof(LocationPanelViewModel), new PropertyMetadata(false));
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush),
            typeof(LocationPanelViewModel), new PropertyMetadata(new SolidColorBrush(UWP.App.AppColor)));
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool),
            typeof(LocationPanelViewModel), new PropertyMetadata(true));
        public static readonly DependencyProperty LocationDataProperty =
            DependencyProperty.Register("LocationData", typeof(LocationData),
            typeof(LocationPanelViewModel), new PropertyMetadata(null));
        public static readonly DependencyProperty WeatherSourceProperty =
            DependencyProperty.Register("WeatherSource", typeof(String),
            typeof(LocationPanelViewModel), new PropertyMetadata(""));

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Properties
        public string LocationName
        {
            get { return (string)GetValue(LocationNameProperty); }
            set { SetValue(LocationNameProperty, value); OnPropertyChanged("LocationName"); }
        }
        public string CurrTemp
        {
            get { return (string)GetValue(CurrTempProperty); }
            set { SetValue(CurrTempProperty, value); OnPropertyChanged("CurrTemp"); }
        }
        public string WeatherIcon
        {
            get { return (string)GetValue(WeatherIconProperty); }
            set { SetValue(WeatherIconProperty, value); OnPropertyChanged("WeatherIcon"); }
        }
        public bool EditMode
        {
            get { return (bool)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); OnPropertyChanged("EditMode"); }
        }
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); OnPropertyChanged("Background"); }
        }
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); OnPropertyChanged("IsLoading"); }
        }
        public LocationData LocationData
        {
            get { return (LocationData)GetValue(LocationDataProperty); }
            set { SetValue(LocationDataProperty, value); OnPropertyChanged("LocationDataProperty"); }
        }
        public string WeatherSource
        {
            get { return (string)GetValue(WeatherSourceProperty); }
            set { SetValue(WeatherSourceProperty, value); OnPropertyChanged("WeatherSource"); }
        }
        #endregion

        private WeatherManager wm;

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
            if (!(Background is ImageBrush))
            {
                Background = new ImageBrush();
                (Background as ImageBrush).Stretch = Stretch.UniformToFill;
                (Background as ImageBrush).AlignmentX = AlignmentX.Center;
            }
            wm.SetBackground(Background as ImageBrush, weather);

            LocationName = weather.location.name;
            CurrTemp = (Settings.IsFahrenheit ?
                Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            WeatherIcon = wm.GetWeatherIcon(weather.condition.icon);
            WeatherSource = weather.source;

            if (LocationData.query == null)
            {
                LocationData.query = weather.query;
                LocationData.source = weather.source;
            }

            IsLoading = false;
        }
    }
}
