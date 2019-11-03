using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.Controls
{
    public class WeatherExtrasViewModel : INotifyPropertyChanged
    {
        #region DependencyProperties
        private ObservableCollection<HourlyForecastItemViewModel> hourlyForecast;
        private ObservableCollection<TextForecastItemViewModel> textForecast;
        private ObservableCollection<WeatherAlertViewModel> alerts;

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Properties
        public ObservableCollection<HourlyForecastItemViewModel> HourlyForecast { get => hourlyForecast; set { hourlyForecast = value; OnPropertyChanged("HourlyForecast"); } }
        public ObservableCollection<TextForecastItemViewModel> TextForecast { get => textForecast; set { textForecast = value; OnPropertyChanged("TextForecast"); } }
        public ObservableCollection<WeatherAlertViewModel> Alerts { get => alerts; set { alerts = value; OnPropertyChanged("Alerts"); } }
        #endregion

        public WeatherExtrasViewModel()
        {
            HourlyForecast = new ObservableCollection<HourlyForecastItemViewModel>();
            TextForecast = new ObservableCollection<TextForecastItemViewModel>();
            Alerts = new ObservableCollection<WeatherAlertViewModel>();
        }

        public WeatherExtrasViewModel(Weather weather)
        {
            HourlyForecast = new ObservableCollection<HourlyForecastItemViewModel>();
            TextForecast = new ObservableCollection<TextForecastItemViewModel>();
            Alerts = new ObservableCollection<WeatherAlertViewModel>();
            UpdateView(weather);
        }

        public void UpdateView(Weather weather)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            // Clear all data
            Clear();

            if (weather.hr_forecast != null && weather.hr_forecast.Length > 0)
            {
                foreach (HourlyForecast hr_forecast in weather.hr_forecast)
                {
                    var hrforecastView = new HourlyForecastItemViewModel(hr_forecast);
                    HourlyForecast.Add(hrforecastView);
                }
                OnPropertyChanged("HourlyForecast");
            }

            if (weather.txt_forecast != null && weather.txt_forecast.Length > 0)
            {
                foreach (TextForecast txt_forecast in weather.txt_forecast)
                {
                    var txtforecastView = new TextForecastItemViewModel(txt_forecast);
                    TextForecast.Add(txtforecastView);
                }
                OnPropertyChanged("TextForecast");
            }

            if (weather.weather_alerts != null && weather.weather_alerts.Count > 0)
            {
                foreach(WeatherAlert alert in weather.weather_alerts)
                {
                    // Skip if alert has expired
                    if (alert.ExpiresDate <= DateTimeOffset.Now)
                        continue;

                    WeatherAlertViewModel alertView = new WeatherAlertViewModel(alert);
                    Alerts.Add(alertView);
                }
                OnPropertyChanged("Alerts");
            }
        }

        public void Clear()
        {
            HourlyForecast.Clear();
            TextForecast.Clear();
            Alerts.Clear();

            OnPropertyChanged("HourlyForecast");
            OnPropertyChanged("TextForecast");
            OnPropertyChanged("Alerts");
        }
    }
}
