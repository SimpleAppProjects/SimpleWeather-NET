using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml;

namespace SimpleWeather.Controls
{
    public class WeatherNowViewModel : INotifyPropertyChanged
    {
        #region DependencyProperties

        private string location;
        private string updateDate;

        // Current Condition
        private string curTemp;
        private string curCondition;
        private string weatherIcon;
        private string hiTemp;
        private string loTemp;
        private bool showHiLo;

        // Weather Details
        private string sunrise;
        private string sunset;
        private SimpleObservableList<DetailItemViewModel> weatherDetails;
        private UVIndexViewModel uvIndex;
        private BeaufortViewModel beaufort;
        private MoonPhaseViewModel moonPhase;
        private AirQualityViewModel airQuality;

        // Background
        private ImageDataViewModel imageData;
        private Color defaultColor = Color.FromArgb(255, 0, 111, 191); // SimpleBlue;
        private Color pendingBackgroundColor = Color.FromArgb(255, 0, 111, 191);

        // Radar
        private const string RadarUriFormat = "https://earth.nullschool.net/#current/wind/surface/level/overlay=precip_3hr/orthographic={1},{0},3000";
        private Uri radarURL;

        private String weatherCredit;
        private String weatherSource;

        private String weatherLocale = "EN";

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected async void OnPropertyChanged(string name)
        {
            await AsyncTask.TryRunOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }).ConfigureAwait(true);
        }

        #endregion DependencyProperties

        #region Properties

        public string Location { get => location; private set { if (!Equals(location, value)) { location = value; OnPropertyChanged(nameof(Location)); } } }
        public string UpdateDate { get => updateDate; private set { if (!Equals(updateDate, value)) { updateDate = value; OnPropertyChanged(nameof(UpdateDate)); } } }
        public string CurTemp { get => curTemp; private set { if (!Equals(curTemp, value)) { curTemp = value; OnPropertyChanged(nameof(CurTemp)); } } }
        public string CurCondition { get => curCondition; private set { if (!Equals(curCondition, value)) { curCondition = value; OnPropertyChanged(nameof(CurCondition)); } } }
        public string WeatherIcon { get => weatherIcon; private set { if (!Equals(weatherIcon, value)) { weatherIcon = value; OnPropertyChanged(nameof(WeatherIcon)); } } }
        public string HiTemp { get => hiTemp; private set { if (!Equals(hiTemp, value)) { hiTemp = value; OnPropertyChanged(nameof(HiTemp)); } } }
        public string LoTemp { get => loTemp; private set { if (!Equals(loTemp, value)) { loTemp = value; OnPropertyChanged(nameof(LoTemp)); } } }
        public bool ShowHiLo { get => showHiLo; private set { if (!Equals(showHiLo, value)) { showHiLo = value; OnPropertyChanged(nameof(ShowHiLo)); } } }
        public string Sunrise { get => sunrise; private set { if (!Equals(sunrise, value)) { sunrise = value; OnPropertyChanged(nameof(Sunrise)); } } }
        public string Sunset { get => sunset; private set { if (!Equals(sunset, value)) { sunset = value; OnPropertyChanged(nameof(Sunset)); } } }
        public SimpleObservableList<DetailItemViewModel> WeatherDetails { get => weatherDetails; private set { weatherDetails = value; OnPropertyChanged(nameof(WeatherDetails)); } }
        public UVIndexViewModel UVIndex { get => uvIndex; private set { if (!Equals(uvIndex, value)) { uvIndex = value; OnPropertyChanged(nameof(UVIndex)); } } }
        public BeaufortViewModel Beaufort { get => beaufort; private set { if (!Equals(beaufort, value)) { beaufort = value; OnPropertyChanged(nameof(Beaufort)); } } }
        public MoonPhaseViewModel MoonPhase { get => moonPhase; private set { if (!Equals(moonPhase, value)) { moonPhase = value; OnPropertyChanged(nameof(MoonPhase)); } } }
        public AirQualityViewModel AirQuality { get => airQuality; private set { if (!Equals(airQuality, value)) { airQuality = value; OnPropertyChanged(nameof(AirQuality)); } } }
        public ImageDataViewModel ImageData { get => imageData; private set { if (!Equals(imageData, value)) { imageData = value; OnPropertyChanged(nameof(ImageData)); } } }
        public Color PendingBackgroundColor { get => pendingBackgroundColor; private set { if (!Equals(pendingBackgroundColor, value)) { pendingBackgroundColor = value; OnPropertyChanged(nameof(PendingBackgroundColor)); } } }
        public Uri RadarURL { get => radarURL; private set { if (!Equals(radarURL?.ToString(), value?.ToString())) { radarURL = value; OnPropertyChanged(nameof(RadarURL)); } } }
        public string WeatherCredit { get => weatherCredit; private set { if (!Equals(weatherCredit, value)) { weatherCredit = value; OnPropertyChanged(nameof(WeatherCredit)); } } }
        public string WeatherSource { get => weatherSource; private set { if (!Equals(weatherSource, value)) { weatherSource = value; OnPropertyChanged(nameof(WeatherSource)); } } }
        public string WeatherLocale { get => weatherLocale; private set { if (!Equals(weatherLocale, value)) { weatherLocale = value; OnPropertyChanged(nameof(WeatherLocale)); } } }

        #endregion Properties

        private WeatherManager wm;
        private Weather weather;
        private string tempUnit;

        public WeatherNowViewModel()
        {
            wm = WeatherManager.GetInstance();
            WeatherDetails = new SimpleObservableList<DetailItemViewModel>();
        }

        public WeatherNowViewModel(Weather weather) : this()
        {
            UpdateView(weather);
        }

        private void Forecasts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Forecasts));
        }

        private void HourlyForecasts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HourlyForecasts));
        }

        public void UpdateView(Weather weather)
        {
            if ((bool)weather?.IsValid())
            {
                if (!Equals(this.weather, weather))
                {
                    this.weather = weather;

                    // Update backgrounds
                    ImageData = null;
                    PendingBackgroundColor = defaultColor;

                    // Location
                    Location = weather?.location?.name;

                    // Additional Details
                    if (weather?.location?.latitude.HasValue == true && weather?.location?.longitude.HasValue == true)
                        RadarURL = new Uri(String.Format(CultureInfo.InvariantCulture, RadarUriFormat, weather.location.latitude, weather.location.longitude));
                    else
                        RadarURL = null;

                    // Language
                    WeatherLocale = weather?.locale;

                    // Refresh locale/unit dependent values
                    RefreshView();
                }
                else if (!Equals(tempUnit, Settings.Unit))
                {
                    RefreshView();
                }
            }
        }

        private void RefreshView()
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            tempUnit = Settings.Unit;

            // Date Updated
            UpdateDate = WeatherUtils.GetLastBuildDate(weather);

            // Update Current Condition
            String tmpCurTemp;
            if (weather.condition.temp_f.HasValue && weather.condition.temp_f != weather.condition.temp_c)
            {
                var temp = Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f.Value) : Math.Round(weather.condition.temp_c.Value);
                tmpCurTemp = temp.ToString(culture);
            }
            else
            {
                tmpCurTemp = "--";
            }
            var unitTemp = Settings.IsFahrenheit ? WeatherIcons.FAHRENHEIT : WeatherIcons.CELSIUS;
            CurTemp = tmpCurTemp + unitTemp;
            CurCondition = (String.IsNullOrWhiteSpace(weather.condition.weather)) ? "--" : weather.condition.weather;
            WeatherIcon = weather.condition.icon;

            if (weather.condition.high_f.HasValue && weather.condition.high_f != weather.condition.high_c)
            {
                var value = Settings.IsFahrenheit ? Math.Round(weather.condition.high_f.Value) : Math.Round(weather.condition.high_c.Value);
                HiTemp = String.Format(culture, "{0}º", value);
            }
            else
            {
                HiTemp = null;
            }

            if (weather.condition.low_f.HasValue && weather.condition.low_f != weather.condition.low_c)
            {
                var value = Settings.IsFahrenheit ? Math.Round(weather.condition.low_f.Value) : Math.Round(weather.condition.low_c.Value);
                LoTemp = String.Format(culture, "{0}º", value);
            }
            else
            {
                LoTemp = null;
            }

            ShowHiLo = HiTemp != null || LoTemp != null;

            // WeatherDetails
            WeatherDetails.Clear();

            // Precipitation
            if (weather.precipitation != null)
            {
                string Chance = weather.precipitation.pop.HasValue ? weather.precipitation.pop.Value + "%" : null;

                if (WeatherAPI.OpenWeatherMap.Equals(Settings.API) || WeatherAPI.MetNo.Equals(Settings.API))
                {
                    if (weather.precipitation.qpf_rain_in.HasValue && weather.precipitation.qpf_rain_in >= 0)
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain,
                            Settings.IsFahrenheit ?
                            weather.precipitation.qpf_rain_in.Value.ToString("0.00", culture) + " in" :
                            weather.precipitation.qpf_rain_mm.Value.ToString("0.00", culture) + " mm"));
                    }
                    if (weather.precipitation.qpf_snow_in.HasValue && weather.precipitation.qpf_snow_in >= 0)
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow,
                            Settings.IsFahrenheit ?
                            weather.precipitation.qpf_snow_in.Value.ToString("0.00", culture) + " in" :
                            weather.precipitation.qpf_snow_cm.Value.ToString("0.00", culture) + " cm"));
                    }
                    if (weather.precipitation.pop.HasValue && weather.precipitation.pop >= 0)
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, Chance));
                }
                else
                {
                    if (weather.precipitation.pop.HasValue && weather.precipitation.pop >= 0)
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, Chance));
                    if (weather.precipitation.qpf_rain_in.HasValue && weather.precipitation.qpf_rain_in >= 0)
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain,
                            Settings.IsFahrenheit ?
                            weather.precipitation.qpf_rain_in.Value.ToString("0.00", culture) + " in" :
                            weather.precipitation.qpf_rain_mm.Value.ToString("0.00", culture) + " mm"));
                    }
                    if (weather.precipitation.qpf_snow_in.HasValue && weather.precipitation.qpf_snow_in >= 0)
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow,
                            Settings.IsFahrenheit ?
                            weather.precipitation.qpf_snow_in.Value.ToString("0.00", culture) + " in" :
                            weather.precipitation.qpf_snow_cm.Value.ToString("0.00", culture) + " cm"));
                    }
                }
            }

            // Atmosphere
            if (weather.atmosphere.pressure_mb.HasValue)
            {
                var pressureVal = Settings.IsFahrenheit ? weather.atmosphere.pressure_in : weather.atmosphere.pressure_mb;
                var pressureUnit = Settings.IsFahrenheit ? "in" : "mb";

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Pressure,
                    String.Format(culture, "{0} {1:0.00} {2}",
                    WeatherUtils.GetPressureStateIcon(weather.atmosphere.pressure_trend),
                    pressureVal, pressureUnit)));
            }

            if (weather.atmosphere.humidity.HasValue)
            {
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Humidity,
                    String.Format(culture, "{0}%", weather.atmosphere.humidity.Value)));
            }

            if (weather.atmosphere.dewpoint_f.HasValue && (weather.atmosphere.dewpoint_f != weather.atmosphere.dewpoint_c))
            {
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                    String.Format(culture, "{0}º",
                    Settings.IsFahrenheit ?
                        Math.Round(weather.atmosphere.dewpoint_f.Value) :
                        Math.Round(weather.atmosphere.dewpoint_c.Value))));
            }

            if (weather.atmosphere.visibility_mi.HasValue && weather.atmosphere.visibility_mi >= 0)
            {
                var visibilityVal = Settings.IsFahrenheit ? weather.atmosphere.visibility_mi : weather.atmosphere.visibility_km;
                var visibilityUnit = Settings.IsFahrenheit ? "mi" : "km";

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                    String.Format(culture, "{0:0.00} {1}", visibilityVal, visibilityUnit)));
            }

            UVIndex = weather.condition.uv != null ? new UVIndexViewModel(weather.condition.uv) : null;

            if (weather.condition.feelslike_f.HasValue && (weather.condition.feelslike_f != weather.condition.feelslike_c))
            {
                var value = Settings.IsFahrenheit ? Math.Round(weather.condition.feelslike_f.Value) : Math.Round(weather.condition.feelslike_c.Value);

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                       String.Format(culture, "{0}º", value)));
            }

            // Wind
            if (weather.condition.wind_mph.HasValue && weather.condition.wind_mph >= 0 &&
                weather.condition.wind_degrees.HasValue && weather.condition.wind_degrees >= 0)
            {
                var speedVal = Settings.IsFahrenheit ? Math.Round(weather.condition.wind_mph.Value) : Math.Round(weather.condition.wind_kph.Value);
                var speedUnit = Settings.IsFahrenheit ? "mph" : "kph";

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                   String.Format(culture, "{0} {1}, {2}", speedVal, speedUnit, WeatherUtils.GetWindDirection(weather.condition.wind_degrees.Value)),
                   weather.condition.wind_degrees.Value + 180));
            }

            Beaufort = weather.condition.beaufort != null ? new BeaufortViewModel(weather.condition.beaufort) : null;

            // Astronomy
            if (weather.astronomy != null)
            {
                Sunrise = weather.astronomy.sunrise.ToString("t", CultureInfo.InvariantCulture);
                Sunset = weather.astronomy.sunset.ToString("t", CultureInfo.InvariantCulture);

                if (weather.astronomy.moonrise != null && weather.astronomy.moonset != null
                        && weather.astronomy.moonrise.CompareTo(DateTime.MinValue) > 0
                        && weather.astronomy.moonset.CompareTo(DateTime.MinValue) > 0)
                {
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonrise,
                           weather.astronomy.moonrise.ToString("t", culture)));
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonset,
                           weather.astronomy.moonset.ToString("t", culture)));
                }

                MoonPhase = weather.astronomy.moonphase != null ? new MoonPhaseViewModel(weather.astronomy.moonphase) : null;
            }
            else
            {
                Sunrise = null;
                Sunset = null;
                MoonPhase = null;
            }
            AsyncTask.TryRunOnUIThread(() =>
            {
                OnPropertyChanged(nameof(WeatherDetails));
                WeatherDetails.NotifyCollectionChanged();
            });

            // Additional Details
            AirQuality = weather.condition.airQuality != null ? new AirQualityViewModel(weather.condition.airQuality) : null;

            WeatherSource = weather?.source;

            string creditPrefix = SimpleLibrary.ResLoader.GetString("Credit_Prefix/Text");
            WeatherCredit = String.Format("{0} {1}",
                creditPrefix, WeatherAPI.APIs.First(WApi => Equals(WeatherSource, WApi.Value)));
        }

        public Task UpdateBackground()
        {
            return Task.Run(async () =>
            {
                if (weather == null) return;

                var imageData = await WeatherUtils.GetImageData(weather);

                if (imageData != null)
                {
                    ImageData = imageData;
                    PendingBackgroundColor = imageData.Color;
                }
                else
                {
                    ImageData = null;
                    PendingBackgroundColor = defaultColor;
                }
            });
        }

        public bool IsValid => weather != null && weather.IsValid();
    }
}