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
        public static readonly DependencyProperty PairProperty =
            DependencyProperty.Register("Pair", typeof(KeyValuePair<int, string>),
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
        public static readonly DependencyProperty IsHomeProperty =
            DependencyProperty.Register("IsHome", typeof(bool),
            typeof(LocationPanelViewModel), new PropertyMetadata(false));
        public static readonly DependencyProperty HomeBoxVisibilityProperty =
            DependencyProperty.Register("HomeBoxVisibility", typeof(Visibility),
            typeof(LocationPanelViewModel), new PropertyMetadata(Visibility.Collapsed));

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
        public KeyValuePair<int, string> Pair
        {
            get { return (KeyValuePair<int, string>)GetValue(PairProperty); }
            set { SetValue(PairProperty, value); OnPropertyChanged("Pair"); }
        }
        public bool EditMode
        {
            get { return (bool)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); setEditMode(value); OnPropertyChanged("EditMode"); }
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
        public bool IsHome
        {
            get { return (bool)GetValue(IsHomeProperty); }
            set { SetValue(IsHomeProperty, value); setHome(value); OnPropertyChanged("IsHome"); }
        }
        public Visibility HomeBoxVisibility
        {
            get { return (Visibility)GetValue(HomeBoxVisibilityProperty); }
            set { SetValue(HomeBoxVisibilityProperty, value); OnPropertyChanged("HomeBoxVisibility"); }
        }
        #endregion

        private void setEditMode(bool value)
        {
            if (!IsHome && !value)
            {
                HomeBoxVisibility = Visibility.Collapsed;
                OnPropertyChanged("HomeBoxVisibility");
            }
            else if (!IsHome && value)
            {
                HomeBoxVisibility = Visibility.Visible;
                OnPropertyChanged("HomeBoxVisibility");
            }
        }

        private void setHome(bool value)
        {
            if (!EditMode)
            {
                HomeBoxVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                OnPropertyChanged("HomeBoxVisibility");
            }
        }

        public LocationPanelViewModel()
        {
        }

        public LocationPanelViewModel(Weather weather)
        {
            setWeather(weather);
        }

        public void setWeather(Weather weather)
        {
            // Update background
            if (Background as ImageBrush == null)
            {
                Background = new ImageBrush();
                (Background as ImageBrush).Stretch = Stretch.UniformToFill;
                (Background as ImageBrush).AlignmentX = AlignmentX.Center;
            }
            WeatherUtils.SetBackground(Background as ImageBrush, weather);

            LocationName = weather.location.name;
            CurrTemp = (Settings.Unit == "F" ?
                Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon);

            IsLoading = false;
        }
    }
}
