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
        #endregion
#elif __ANDROID__
        public ObservableCollection<HourlyForecastItemViewModel> HourlyForecast { get; set; }
        public ObservableCollection<TextForecastItemViewModel> TextForecast { get; set; }

        public string Chance { get; set; }
        public string Qpf_Rain { get; set; }
        public string Qpf_Snow { get; set; }
#endif

        public WeatherExtrasViewModel()
        {
            HourlyForecast = new ObservableCollection<HourlyForecastItemViewModel>();
            TextForecast = new ObservableCollection<TextForecastItemViewModel>();
        }

        public WeatherExtrasViewModel(Weather weather)
        {
            HourlyForecast = new ObservableCollection<HourlyForecastItemViewModel>();
            TextForecast = new ObservableCollection<TextForecastItemViewModel>();
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

            HourlyForecast.Clear();
            if (weather.hr_forecast != null && weather.hr_forecast.Length > 0)
            {
                foreach (HourlyForecast hr_forecast in weather.hr_forecast)
                {
                    HourlyForecastItemViewModel hrforecastView = new HourlyForecastItemViewModel(hr_forecast);
                    HourlyForecast.Add(hrforecastView);
                }
            }

            TextForecast.Clear();
            if (weather.txt_forecast != null && weather.txt_forecast.Length > 0)
            {
                foreach (TextForecast txt_forecast in weather.txt_forecast)
                {
                    TextForecastItemViewModel txtforecastView = new TextForecastItemViewModel(txt_forecast);
                    TextForecast.Add(txtforecastView);
                }
            }

            Chance = weather.precipitation.pop + "%";
            Qpf_Rain = Settings.IsFahrenheit ?
                weather.precipitation.qpf_rain_in.ToString("0.00", culture) + " in" : weather.precipitation.qpf_rain_mm.ToString(culture) + " mm";
            Qpf_Snow = Settings.IsFahrenheit ?
                weather.precipitation.qpf_snow_in.ToString("0.00", culture) + " in" : weather.precipitation.qpf_snow_cm.ToString(culture) + " cm";
        }

        public void Clear()
        {
            HourlyForecast.Clear();
            TextForecast.Clear();
            Chance = Qpf_Rain = Qpf_Snow = String.Empty;
        }
    }
}
