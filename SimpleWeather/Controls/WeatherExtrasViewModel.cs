using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Windows.System.UserProfile;

namespace SimpleWeather.Controls
{
    public class WeatherExtrasViewModel : INotifyPropertyChanged
    {
        #region DependencyProperties

        private ObservableForecastLoadingCollection<HourlyForecastItemViewModel> hourlyForecast;
        private ObservableForecastLoadingCollection<TextForecastItemViewModel> textForecast;
        private ObservableCollection<WeatherAlertViewModel> alerts;

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion DependencyProperties

        #region Properties

        public ObservableForecastLoadingCollection<HourlyForecastItemViewModel> HourlyForecast { get => hourlyForecast; set { hourlyForecast = value; OnPropertyChanged(nameof(HourlyForecast)); } }
        public ObservableForecastLoadingCollection<TextForecastItemViewModel> TextForecast { get => textForecast; set { textForecast = value; OnPropertyChanged(nameof(TextForecast)); } }
        public ObservableCollection<WeatherAlertViewModel> Alerts { get => alerts; private set { alerts = value; OnPropertyChanged(nameof(Alerts)); } }

        #endregion Properties

        public WeatherExtrasViewModel()
        {
            HourlyForecast = new ObservableForecastLoadingCollection<HourlyForecastItemViewModel>();
            TextForecast = new ObservableForecastLoadingCollection<TextForecastItemViewModel>();
            Alerts = new ObservableCollection<WeatherAlertViewModel>();

            HourlyForecast.CollectionChanged += HourlyForecasts_CollectionChanged;
            TextForecast.CollectionChanged += TextForecasts_CollectionChanged;
        }

        public WeatherExtrasViewModel(Weather weather)
        {
            HourlyForecast = new ObservableForecastLoadingCollection<HourlyForecastItemViewModel>(weather);
            TextForecast = new ObservableForecastLoadingCollection<TextForecastItemViewModel>(weather);
            Alerts = new ObservableCollection<WeatherAlertViewModel>();

            HourlyForecast.CollectionChanged += HourlyForecasts_CollectionChanged;
            TextForecast.CollectionChanged += TextForecasts_CollectionChanged;

            UpdateView(weather);
        }

        private void HourlyForecasts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HourlyForecast));
        }

        private void TextForecasts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TextForecast));
        }

        public void UpdateView(Weather weather)
        {
            if (weather?.hr_forecast?.Count > 0)
            {
                HourlyForecast.Clear();
                foreach (HourlyForecast hr_forecast in weather.hr_forecast)
                {
                    var hrforecastView = new HourlyForecastItemViewModel(hr_forecast);
                    HourlyForecast.Add(hrforecastView);
                }
            }
            else
            {
                // Let collection handle changes (clearing, etc.)
                HourlyForecast.SetWeather(weather);
            }
            OnPropertyChanged(nameof(HourlyForecast));

            if (weather?.txt_forecast?.Count > 0)
            {
                TextForecast.Clear();
                foreach (TextForecast txt_forecast in weather.txt_forecast)
                {
                    var txtforecastView = new TextForecastItemViewModel(txt_forecast);
                    TextForecast.Add(txtforecastView);
                }
            }
            else
            {
                // Let collection handle changes (clearing, etc.)
                TextForecast.SetWeather(weather);
            }
            OnPropertyChanged(nameof(TextForecast));

            Alerts.Clear();
            if (weather?.weather_alerts?.Any() == true)
            {
                foreach (WeatherAlert alert in weather.weather_alerts)
                {
                    // Skip if alert has expired
                    if (alert.ExpiresDate <= DateTimeOffset.Now)
                        continue;

                    WeatherAlertViewModel alertView = new WeatherAlertViewModel(alert);
                    Alerts.Add(alertView);
                }
            }
            OnPropertyChanged(nameof(Alerts));
        }
    }
}