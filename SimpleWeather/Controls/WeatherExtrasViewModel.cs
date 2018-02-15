using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
#if WINDOWS_UWP
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#elif __ANDROID__
using Android.Graphics;
using Android.Views;
using SimpleWeather.Droid;
#endif

namespace SimpleWeather.Controls
{
    public class WeatherExtrasViewModel
#if WINDOWS_UWP
        : DependencyObject, INotifyPropertyChanged
#endif
    {
#if WINDOWS_UWP
        #region DependencyProperties
        public static readonly DependencyProperty HourlyForecastProperty =
            DependencyProperty.Register("HourlyForecast", typeof(ObservableCollection<HourlyForecastItemViewModel>),
            typeof(WeatherNowViewModel), new PropertyMetadata(null));
        public static readonly DependencyProperty TextForecastProperty =
            DependencyProperty.Register("TextForecast", typeof(ObservableCollection<TextForecastItemViewModel>),
            typeof(WeatherNowViewModel), new PropertyMetadata(null));
        public static readonly DependencyProperty ChanceProperty =
            DependencyProperty.Register("Chance", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty Qpf_RainProperty =
            DependencyProperty.Register("Qpf_Rain", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty Qpf_SnowProperty =
            DependencyProperty.Register("Qpf_Snow", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty AlertsProperty =
            DependencyProperty.Register("Alerts", typeof(ObservableCollection<WeatherAlertViewModel>),
            typeof(WeatherNowViewModel), new PropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Properties
        public ObservableCollection<HourlyForecastItemViewModel> HourlyForecast
        {
            get { return (ObservableCollection<HourlyForecastItemViewModel>)GetValue(HourlyForecastProperty); }
            set { SetValue(HourlyForecastProperty, value); OnPropertyChanged("HourlyForecast"); }
        }
        public ObservableCollection<TextForecastItemViewModel> TextForecast
        {
            get { return (ObservableCollection<TextForecastItemViewModel>)GetValue(TextForecastProperty); }
            set { SetValue(TextForecastProperty, value); OnPropertyChanged("TextForecast"); }
        }
        public string Chance
        {
            get { return (string)GetValue(ChanceProperty); }
            set { SetValue(ChanceProperty, value); OnPropertyChanged("Chance"); }
        }
        public string Qpf_Rain
        {
            get { return (string)GetValue(Qpf_RainProperty); }
            set { SetValue(Qpf_RainProperty, value); OnPropertyChanged("Qpf_Rain"); }
        }
        public string Qpf_Snow
        {
            get { return (string)GetValue(Qpf_SnowProperty); }
            set { SetValue(Qpf_SnowProperty, value); OnPropertyChanged("Qpf_Snow"); }
        }
        public ObservableCollection<WeatherAlertViewModel> Alerts
        {
            get { return (ObservableCollection<WeatherAlertViewModel>)GetValue(AlertsProperty); }
            set { SetValue(AlertsProperty, value); OnPropertyChanged("Alerts"); }
        }
        #endregion
#elif __ANDROID__
        public ObservableCollection<HourlyForecastItemViewModel> HourlyForecast { get; set; }
        public ObservableCollection<TextForecastItemViewModel> TextForecast { get; set; }

        public string Chance { get; set; }
        public string Qpf_Rain { get; set; }
        public string Qpf_Snow { get; set; }

        public ObservableCollection<WeatherAlertViewModel> Alerts { get; set; }
#endif

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
#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif

            // Clear all data
            Clear();

            if (weather.hr_forecast != null && weather.hr_forecast.Length > 0)
            {
                foreach (HourlyForecast hr_forecast in weather.hr_forecast)
                {
                    HourlyForecastItemViewModel hrforecastView = new HourlyForecastItemViewModel(hr_forecast);
                    HourlyForecast.Add(hrforecastView);
                }
            }

            if (weather.txt_forecast != null && weather.txt_forecast.Length > 0)
            {
                foreach (TextForecast txt_forecast in weather.txt_forecast)
                {
                    TextForecastItemViewModel txtforecastView = new TextForecastItemViewModel(txt_forecast);
                    TextForecast.Add(txtforecastView);
                }
            }

            if (weather.precipitation != null)
            {
                Chance = weather.precipitation.pop + "%";
                Qpf_Rain = Settings.IsFahrenheit ?
                    weather.precipitation.qpf_rain_in.ToString("0.00", culture) + " in" : weather.precipitation.qpf_rain_mm.ToString(culture) + " mm";
                Qpf_Snow = Settings.IsFahrenheit ?
                    weather.precipitation.qpf_snow_in.ToString("0.00", culture) + " in" : weather.precipitation.qpf_snow_cm.ToString(culture) + " cm";
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
            }
        }

        public void Clear()
        {
            HourlyForecast.Clear();
            TextForecast.Clear();
            Chance = Qpf_Rain = Qpf_Snow = String.Empty;
            Alerts.Clear();
        }
    }
}
