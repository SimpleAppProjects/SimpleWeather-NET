using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
#if WINDOWS_UWP
using SimpleWeather.UWP;
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
    public class WeatherNowViewModel
#if WINDOWS_UWP
        : DependencyObject, INotifyPropertyChanged
#endif
    {
#if WINDOWS_UWP
        #region DependencyProperties
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty UpdateDateProperty =
            DependencyProperty.Register("UpdateDate", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty CurTempProperty =
            DependencyProperty.Register("CurTemp", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty CurConditionProperty =
            DependencyProperty.Register("CurCondition", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty WeatherIconProperty =
            DependencyProperty.Register("WeatherIcon", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty HumidityProperty =
            DependencyProperty.Register("Humidity", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty PressureProperty =
            DependencyProperty.Register("Pressure", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty RisingVisiblityProperty =
            DependencyProperty.Register("RisingVisiblity", typeof(Visibility),
            typeof(WeatherNowViewModel), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty RisingIconProperty =
            DependencyProperty.Register("RisingIcon", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty _VisibilityProperty =
            DependencyProperty.Register("_Visibility", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty WindChillProperty =
            DependencyProperty.Register("WindChill", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty WindDirectionProperty =
            DependencyProperty.Register("WindDirection", typeof(Transform),
            typeof(WeatherNowViewModel), new PropertyMetadata(new RotateTransform()));
        public static readonly DependencyProperty WindSpeedProperty =
            DependencyProperty.Register("WindSpeed", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty SunriseProperty =
            DependencyProperty.Register("Sunrise", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty SunsetProperty =
            DependencyProperty.Register("Sunset", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty ForecastsProperty =
            DependencyProperty.Register("Forecasts", typeof(ObservableCollection<ForecastItemViewModel>),
            typeof(WeatherNowViewModel), new PropertyMetadata(null));
        public static readonly DependencyProperty WUExtrasProperty =
            DependencyProperty.Register("WUExtrasProperty", typeof(WUExtrasViewModel),
            typeof(WeatherNowViewModel), new PropertyMetadata(null));
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(ImageBrush),
            typeof(WeatherNowViewModel), new PropertyMetadata(null));
        public static readonly DependencyProperty PendingBackgroundColorProperty =
            DependencyProperty.Register("PendingBackgroundColor", typeof(Color),
            typeof(WeatherNowViewModel), new PropertyMetadata(Color.FromArgb(255, 0, 111, 191)));
        public static readonly DependencyProperty WeatherCreditProperty =
            DependencyProperty.Register("WeatherCredit", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty WeatherSourceProperty =
            DependencyProperty.Register("WeatherSource", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty WeatherLocaleProperty =
            DependencyProperty.Register("WeatherLocale", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata("EN"));

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Properties
        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); OnPropertyChanged("Location"); }
        }
        public string UpdateDate
        {
            get { return (string)GetValue(UpdateDateProperty); }
            set { SetValue(UpdateDateProperty, value); OnPropertyChanged("UpdateDate"); }
        }
        // Current Condition
        public string CurTemp
        {
            get { return (string)GetValue(CurTempProperty); }
            set { SetValue(CurTempProperty, value); OnPropertyChanged("CurTemp"); }
        }
        public string CurCondition
        {
            get { return (string)GetValue(CurConditionProperty); }
            set { SetValue(CurConditionProperty, value); OnPropertyChanged("CurCondition"); }
        }
        public string WeatherIcon
        {
            get { return (string)GetValue(WeatherIconProperty); }
            set { SetValue(WeatherIconProperty, value); OnPropertyChanged("WeatherIcon"); }
        }
        // Weather Details
        public string Humidity
        {
            get { return (string)GetValue(HumidityProperty); }
            set { SetValue(HumidityProperty, value); OnPropertyChanged("Humidity"); }
        }
        public string Pressure
        {
            get { return (string)GetValue(PressureProperty); }
            set { SetValue(PressureProperty, value); OnPropertyChanged("Pressure"); }
        }
        public Visibility RisingVisiblity
        {
            get { return (Visibility)GetValue(RisingVisiblityProperty); }
            set { SetValue(RisingVisiblityProperty, value); OnPropertyChanged("RisingVisiblity"); }
        }
        public string RisingIcon
        {
            get { return (string)GetValue(RisingIconProperty); }
            set { SetValue(RisingIconProperty, value); OnPropertyChanged("RisingIcon"); }
        }
        public string _Visibility
        {
            get { return (string)GetValue(_VisibilityProperty); }
            set { SetValue(_VisibilityProperty, value); OnPropertyChanged("_Visibility"); }
        }
        public string WindChill
        {
            get { return (string)GetValue(WindChillProperty); }
            set { SetValue(WindChillProperty, value); OnPropertyChanged("WindChill"); }
        }
        public Transform WindDirection
        {
            get { return (Transform)GetValue(WindDirectionProperty); }
            set { SetValue(WindDirectionProperty, value); OnPropertyChanged("WindDirection"); }
        }
        public string WindSpeed
        {
            get { return (string)GetValue(WindSpeedProperty); }
            set { SetValue(WindSpeedProperty, value); OnPropertyChanged("WindSpeed"); }
        }
        public string Sunrise
        {
            get { return (string)GetValue(SunriseProperty); }
            set { SetValue(SunriseProperty, value); OnPropertyChanged("Sunrise"); }
        }
        public string Sunset
        {
            get { return (string)GetValue(SunsetProperty); }
            set { SetValue(SunsetProperty, value); OnPropertyChanged("Sunset"); }
        }
        // Forecast
        public ObservableCollection<ForecastItemViewModel> Forecasts
        {
            get { return (ObservableCollection<ForecastItemViewModel>)GetValue(ForecastsProperty); }
            set { SetValue(ForecastsProperty, value); OnPropertyChanged("Forecasts"); }
        }
        // Additional Details
        public WUExtrasViewModel WUExtras
        {
            get { return (WUExtrasViewModel)GetValue(WUExtrasProperty); }
            set { SetValue(WUExtrasProperty, value); OnPropertyChanged("WUExtras"); }
        }
        // Background
        public ImageBrush Background
        {
            get { return (ImageBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); OnPropertyChanged("Background"); }
        }
        public Color PendingBackgroundColor
        {
            get { return (Color)GetValue(PendingBackgroundColorProperty); }
            set { SetValue(PendingBackgroundColorProperty, value); OnPropertyChanged("PendingBackgroundColor"); }
        }
        public String WeatherCredit
        {
            get { return (String)GetValue(WeatherCreditProperty); }
            set { SetValue(WeatherCreditProperty, value); OnPropertyChanged("WeatherCredit"); }
        }
        public String WeatherSource
        {
            get { return (String)GetValue(WeatherSourceProperty); }
            set { SetValue(WeatherSourceProperty, value); OnPropertyChanged("WeatherSource"); }
        }
        public String WeatherLocale
        {
            get { return (String)GetValue(WeatherLocaleProperty); }
            set { SetValue(WeatherLocaleProperty, value); OnPropertyChanged("WeatherLocale"); }
        }
        #endregion
#elif __ANDROID__
        public string Location { get; set; }
        public string UpdateDate { get; set; }

        // Current Condition
        public string CurTemp { get; set; }
        public string CurCondition { get; set; }
        public string WeatherIcon { get; set; }

        // Weather Details
        public string Humidity { get; set; }
        public string Pressure { get; set; }
        public ViewStates RisingVisiblity { get; set; }
        public string RisingIcon { get; set; }
        public string _Visibility { get; set; }
        public string WindChill { get; set; }
        public int WindDirection { get; set; }
        public string WindSpeed { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        // Forecast
        public ObservableCollection<ForecastItemViewModel> Forecasts { get; set; }

        // Additional Details
        public WUExtrasViewModel WUExtras { get; set; }

        // Background
        public string Background { get; set; }
        public Color PendingBackground { get; set; }

        public string WeatherCredit { get; set; }
        public string WeatherSource { get; set; }

        public string WeatherLocale { get; set; }
#endif

        public WeatherNowViewModel()
        {
#if WINDOWS_UWP
            Background = new ImageBrush()
            {
                Stretch = Stretch.UniformToFill,
                AlignmentX = AlignmentX.Center
            };
#endif
            Forecasts = new ObservableCollection<ForecastItemViewModel>();
            WUExtras = new WUExtrasViewModel();
        }

        public WeatherNowViewModel(Weather weather)
        {
#if WINDOWS_UWP
            Background = new ImageBrush()
            {
                Stretch = Stretch.UniformToFill,
                AlignmentX = AlignmentX.Center
            };
#endif
            Forecasts = new ObservableCollection<ForecastItemViewModel>();
            WUExtras = new WUExtrasViewModel();
            UpdateView(weather);
        }

        public void UpdateView(Weather weather)
        {
#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = CultureInfo.CurrentCulture;
#endif
            // Update backgrounds
#if WINDOWS_UWP
            WeatherUtils.SetBackground(Background, weather);
            PendingBackgroundColor = WeatherUtils.GetWeatherBackgroundColor(weather);
#elif __ANDROID__
            Background = WeatherUtils.GetBackgroundURI(weather);
            PendingBackground = WeatherUtils.GetWeatherBackgroundColor(weather);
#endif

            // Location
            Location = weather.location.name;

            // Date Updated
            UpdateDate = WeatherUtils.GetLastBuildDate(weather);

            // Update Current Condition
            CurTemp = Settings.IsFahrenheit ?
                Math.Round(weather.condition.temp_f) + "\uf045" : Math.Round(weather.condition.temp_c) + "\uf03c";
            CurCondition = (String.IsNullOrWhiteSpace(weather.condition.weather)) ? "---" : weather.condition.weather;
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon);

            // WeatherDetails
            // Astronomy
#if WINDOWS_UWP
            Sunrise = weather.astronomy.sunrise.ToString("t", culture);
            Sunset = weather.astronomy.sunset.ToString("t", culture);
#elif __ANDROID__
            if (Android.Text.Format.DateFormat.Is24HourFormat(App.Context))
            {
                Sunrise = weather.astronomy.sunrise.ToString("HH:mm");
                Sunset = weather.astronomy.sunset.ToString("HH:mm");
            }
            else
            {
                Sunrise = weather.astronomy.sunrise.ToString("h:mm tt");
                Sunset = weather.astronomy.sunset.ToString("h:mm tt");
            }
#endif

            // Wind
            WindChill = Settings.IsFahrenheit ?
                Math.Round(weather.condition.feelslike_f) + "º" : Math.Round(weather.condition.feelslike_c) + "º";
            WindSpeed = Settings.IsFahrenheit ?
                weather.condition.wind_mph.ToString(culture) + " mph" : weather.condition.wind_kph.ToString(culture) + " kph";
            UpdateWindDirection(weather.condition.wind_degrees);

            // Atmosphere
            Humidity = weather.atmosphere.humidity;
            Pressure = Settings.IsFahrenheit ?
                float.Parse(weather.atmosphere.pressure_in, CultureInfo.InvariantCulture).ToString(culture) + " in" :
                float.Parse(weather.atmosphere.pressure_mb, CultureInfo.InvariantCulture).ToString(culture) + " mb";
            UpdatePressureState(weather.atmosphere.pressure_trend);
            _Visibility = Settings.IsFahrenheit ?
                float.Parse(weather.atmosphere.visibility_mi, CultureInfo.InvariantCulture).ToString(culture) + " mi" : 
                float.Parse(weather.atmosphere.visibility_km, CultureInfo.InvariantCulture).ToString(culture) + " km";
            
            if (_Visibility.StartsWith(" "))
                _Visibility = _Visibility.Insert(0, "--");

            // Add UI elements
            Forecasts.Clear();
            foreach (Forecast forecast in weather.forecast)
            {
                ForecastItemViewModel forecastView = new ForecastItemViewModel(forecast);
                Forecasts.Add(forecastView);
            }

            // Additional Details
            WeatherSource = weather.source;
            string creditPrefix = "Data from";

#if WINDOWS_UWP
            creditPrefix = App.ResLoader.GetString("Credit_Prefix");
#elif __ANDROID__
            creditPrefix = App.Context.GetString(Resource.String.credit_prefix);
#endif

            if (weather.source == Settings.API_WUnderground)
            {
                WeatherCredit = string.Format("{0} WeatherUnderground", creditPrefix);
                WUExtras.UpdateView(weather);
            }
            else if (weather.source == Settings.API_Yahoo)
            {
                WeatherCredit = string.Format("{0} Yahoo!", creditPrefix);
                // Clear data
                WUExtras.Clear();
            }

            // Language
            WeatherLocale = weather.locale;
        }

        private void UpdatePressureState(string state)
        {
            switch (state)
            {
                // Steady
                case "0":
                default:
#if WINDOWS_UWP
                    RisingVisiblity = Visibility.Collapsed;
#elif __ANDROID__
                    RisingVisiblity = ViewStates.Gone;
#endif
                    RisingIcon = string.Empty;
                    break;
                // Rising
                case "1":
                case "+":
#if WINDOWS_UWP
                    RisingVisiblity = Visibility.Visible;
#elif __ANDROID__
                    RisingVisiblity = ViewStates.Visible;
#endif
                    RisingIcon = "\uf058\uf058";
                    break;
                // Falling
                case "2":
                case "-":
#if WINDOWS_UWP
                    RisingVisiblity = Visibility.Visible;
#elif __ANDROID__
                    RisingVisiblity = ViewStates.Visible;
#endif
                    RisingIcon = "\uf044\uf044";
                    break;
            }
        }

        private void UpdateWindDirection(int angle)
        {
#if WINDOWS_UWP
            RotateTransform rotation = new RotateTransform()
            {
                Angle = angle
            };
            WindDirection = rotation;
#elif __ANDROID__
            WindDirection = angle;
#endif
        }
    }
}
