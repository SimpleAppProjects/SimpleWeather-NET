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
using Android.App;
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
        public static readonly DependencyProperty ForecastsProperty =
            DependencyProperty.Register("Forecasts", typeof(ObservableCollection<ForecastItemViewModel>),
            typeof(WeatherNowViewModel), new PropertyMetadata(null));
        public static readonly DependencyProperty SunriseProperty =
            DependencyProperty.Register("Sunrise", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty SunsetProperty =
            DependencyProperty.Register("Sunset", typeof(String),
            typeof(WeatherNowViewModel), new PropertyMetadata(""));
        public static readonly DependencyProperty WeatherDetailsProperty =
            DependencyProperty.Register("WeatherDetails", typeof(ObservableCollection<DetailItemViewModel>),
            typeof(WeatherNowViewModel), new PropertyMetadata(null));
        public static readonly DependencyProperty ExtrasProperty =
            DependencyProperty.Register("ExtrasProperty", typeof(WeatherExtrasViewModel),
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
        // Weather Details
        public ObservableCollection<DetailItemViewModel> WeatherDetails
        {
            get { return (ObservableCollection<DetailItemViewModel>)GetValue(WeatherDetailsProperty); }
            set { SetValue(WeatherDetailsProperty, value); OnPropertyChanged("WeatherDetails"); }
        }
        // Additional Details
        public WeatherExtrasViewModel Extras
        {
            get { return (WeatherExtrasViewModel)GetValue(ExtrasProperty); }
            set { SetValue(ExtrasProperty, value); OnPropertyChanged("Extras"); }
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
        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        // Weather Details
        public ObservableCollection<DetailItemViewModel> WeatherDetails { get; set; }

        // Forecast
        public ObservableCollection<ForecastItemViewModel> Forecasts { get; set; }

        // Additional Details
        public WeatherExtrasViewModel Extras { get; set; }

        // Background
        public string Background { get; set; }
        public Color PendingBackground { get; set; }

        public string WeatherCredit { get; set; }
        public string WeatherSource { get; set; }

        public string WeatherLocale { get; set; }
#endif

        private WeatherManager wm;

        public WeatherNowViewModel()
        {
            wm = WeatherManager.GetInstance();

#if WINDOWS_UWP
            Background = new ImageBrush()
            {
                Stretch = Stretch.UniformToFill,
                AlignmentX = AlignmentX.Center
            };
#endif
            Forecasts = new ObservableCollection<ForecastItemViewModel>();
            WeatherDetails = new ObservableCollection<DetailItemViewModel>();
            Extras = new WeatherExtrasViewModel();
        }

        public WeatherNowViewModel(Weather weather)
        {
            wm = WeatherManager.GetInstance();

#if WINDOWS_UWP
            Background = new ImageBrush()
            {
                Stretch = Stretch.UniformToFill,
                AlignmentX = AlignmentX.Center
            };
#endif
            Forecasts = new ObservableCollection<ForecastItemViewModel>();
            WeatherDetails = new ObservableCollection<DetailItemViewModel>();
            Extras = new WeatherExtrasViewModel();
            UpdateView(weather);
        }

        public void UpdateView(Weather weather)
        {
            if (weather.IsValid())
            {
#if WINDOWS_UWP
                var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
                var culture = new CultureInfo(userlang);
#else
                var culture = CultureInfo.CurrentCulture;
#endif
                // Update backgrounds
#if WINDOWS_UWP
                wm.SetBackground(Background, weather);
                PendingBackgroundColor = wm.GetWeatherBackgroundColor(weather);
#elif __ANDROID__
                Background = wm.GetBackgroundURI(weather);
                PendingBackground = wm.GetWeatherBackgroundColor(weather);
#endif

                // Location
                Location = weather.location.name;

                // Date Updated
                UpdateDate = WeatherUtils.GetLastBuildDate(weather);

                // Update Current Condition
                CurTemp = Settings.IsFahrenheit ?
                    Math.Round(weather.condition.temp_f) + "\uf045" : Math.Round(weather.condition.temp_c) + "\uf03c";
                CurCondition = (String.IsNullOrWhiteSpace(weather.condition.weather)) ? "---" : weather.condition.weather;
                WeatherIcon = weather.condition.icon;

                // WeatherDetails
                WeatherDetails.Clear();
                // Precipitation
                if (weather.precipitation != null)
                {
                    string Chance = weather.precipitation.pop + "%";
                    string Qpf_Rain = Settings.IsFahrenheit ?
                        weather.precipitation.qpf_rain_in.ToString("0.00", culture) + " in" : weather.precipitation.qpf_rain_mm.ToString(culture) + " mm";
                    string Qpf_Snow = Settings.IsFahrenheit ?
                        weather.precipitation.qpf_snow_in.ToString("0.00", culture) + " in" : weather.precipitation.qpf_snow_cm.ToString(culture) + " cm";

                    if (WeatherAPI.OpenWeatherMap.Equals(Settings.API) || WeatherAPI.MetNo.Equals(Settings.API))
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, Chance));
                    }
                    else
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, Chance));
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                    }
                }

                // Atmosphere
                var pressureVal = Settings.IsFahrenheit ? weather.atmosphere.pressure_in : weather.atmosphere.pressure_mb;
                var pressureUnit = Settings.IsFahrenheit ? "in" : "mb";

                if (float.TryParse(pressureVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressure))
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Pressure,
                        string.Format("{0} {1} {2}", GetPressureStateIcon(weather.atmosphere.pressure_trend), pressure.ToString(culture), pressureUnit)));

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Humidity,
                    weather.atmosphere.humidity.EndsWith("%", StringComparison.Ordinal) ?
                            weather.atmosphere.humidity : weather.atmosphere.humidity + "%"));

                if (!String.IsNullOrWhiteSpace(weather.atmosphere.dewpoint_f))
                {
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                           Settings.IsFahrenheit ?
                                Math.Round(float.Parse(weather.atmosphere.dewpoint_f)) + "º" :
                                Math.Round(float.Parse(weather.atmosphere.dewpoint_c)) + "º"));
                }

                var visibilityVal = Settings.IsFahrenheit ? weather.atmosphere.visibility_mi : weather.atmosphere.visibility_km;
                var visibilityUnit = Settings.IsFahrenheit ? "mi" : "km";

                if (float.TryParse(visibilityVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibility))
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                           string.Format("{0} {1}", visibility.ToString(culture), visibilityUnit)));

                if (weather.condition.uv != null)
                {
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.UV,
                           string.Format("{0}, {1}", weather.condition.uv.index, weather.condition.uv.desc)));
                }

                // Wind
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                       Settings.IsFahrenheit ?
                            Math.Round(weather.condition.feelslike_f) + "º" : Math.Round(weather.condition.feelslike_c) + "º"));
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                       Settings.IsFahrenheit ?
                            Math.Round(weather.condition.wind_mph) + " mph" : Math.Round(weather.condition.wind_kph) + " kph", GetWindIconRotation(weather.condition.wind_degrees)));

                if (weather.condition.beaufort != null)
                {
                    WeatherDetails.Add(new DetailItemViewModel(weather.condition.beaufort.scale,
                            weather.condition.beaufort.desc));
                }

                // Astronomy
#if WINDOWS_UWP
                Sunrise = weather.astronomy.sunrise.ToString("t", culture);
                Sunset = weather.astronomy.sunset.ToString("t", culture);
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunrise, Sunrise));
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunset, Sunset));
#elif __ANDROID__
                if (Android.Text.Format.DateFormat.Is24HourFormat(Application.Context))
                {
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunrise,
                           weather.astronomy.sunrise.ToString("HH:mm")));
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunset,
                           weather.astronomy.sunset.ToString("HH:mm")));
                }
                else
                {
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunrise,
                           weather.astronomy.sunrise.ToString("h:mm tt")));
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunset,
                           weather.astronomy.sunset.ToString("h:mm tt")));
                }
#endif

                if (weather.astronomy.moonrise != null && weather.astronomy.moonset != null
                        && weather.astronomy.moonrise.CompareTo(DateTime.MinValue) > 0
                        && weather.astronomy.moonset.CompareTo(DateTime.MinValue) > 0)
                {
#if WINDOWS_UWP
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonrise,
                           weather.astronomy.moonrise.ToString("t", culture)));
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonset,
                           weather.astronomy.moonset.ToString("t", culture)));
#elif __ANDROID__
                    if (Android.Text.Format.DateFormat.Is24HourFormat(Application.Context))
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonrise,
                               weather.astronomy.moonrise.ToString("HH:mm")));
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonset,
                               weather.astronomy.moonset.ToString("HH:mm")));
                    }
                    else
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonrise,
                               weather.astronomy.moonrise.ToString("h:mm tt")));
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonset,
                               weather.astronomy.moonset.ToString("h:mm tt")));
                    }
#endif
                }

                if (weather.astronomy.moonphase != null)
                {
                    WeatherDetails.Add(new DetailItemViewModel(weather.astronomy.moonphase.phase,
                           weather.astronomy.moonphase.desc));
                }

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
                creditPrefix = Application.Context.GetString(Resource.String.credit_prefix);
#endif

                if (weather.source == WeatherAPI.WeatherUnderground)
                    WeatherCredit = string.Format("{0} WeatherUnderground", creditPrefix);
                else if (weather.source == WeatherAPI.Yahoo)
                    WeatherCredit = string.Format("{0} Yahoo!", creditPrefix);
                else if (weather.source == WeatherAPI.OpenWeatherMap)
                    WeatherCredit = string.Format("{0} OpenWeatherMap", creditPrefix);
                else if (weather.source == WeatherAPI.MetNo)
                    WeatherCredit = string.Format("{0} MET Norway", creditPrefix);
                else if (weather.source == WeatherAPI.Here)
                    WeatherCredit = string.Format("{0} HERE Weather", creditPrefix);

                Extras.UpdateView(weather);

                // Language
                WeatherLocale = weather.locale;
            }
        }

        private String GetPressureStateIcon(string state)
        {
            switch (state)
            {
                // Steady
                case "0":
                default:
                    return string.Empty;
                // Rising
                case "1":
                case "+":
                case "Rising":
                    return "\uf058\uf058";
                // Falling
                case "2":
                case "-":
                case "Falling":
                    return "\uf044\uf044";
            }
        }

        private int GetWindIconRotation(int angle)
        {
            return angle - 180;
        }
    }
}
