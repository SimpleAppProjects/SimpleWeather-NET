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
                    if (!String.IsNullOrWhiteSpace(weather.location.latitude) && !String.IsNullOrWhiteSpace(weather.location.longitude))
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
            if (weather.condition.temp_f != weather.condition.temp_c)
            {
                var temp = (int)(Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c));
                tmpCurTemp = temp.ToString();
            }
            else
            {
                tmpCurTemp = "--";
            }
            var unitTemp = Settings.IsFahrenheit ? WeatherIcons.FAHRENHEIT : WeatherIcons.CELSIUS;
            CurTemp = tmpCurTemp + unitTemp;
            CurCondition = (String.IsNullOrWhiteSpace(weather.condition.weather)) ? "--" : weather.condition.weather;
            WeatherIcon = weather.condition.icon;

            if (weather.condition.high_f != weather.condition.high_c)
            {
                HiTemp = (int)(Settings.IsFahrenheit ? Math.Round(weather.condition.high_f) : Math.Round(weather.condition.high_c)) + "°";
            }
            else
            {
                HiTemp = null;
            }

            if (weather.condition.low_f != weather.condition.low_c)
            {
                LoTemp = (int)(Settings.IsFahrenheit ? Math.Round(weather.condition.low_f) : Math.Round(weather.condition.low_c)) + "°";
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
                string Chance = weather.precipitation.pop + "%";
                string Qpf_Rain = Settings.IsFahrenheit ?
                    weather.precipitation.qpf_rain_in.ToString("0.00", culture) + " in" : weather.precipitation.qpf_rain_mm.ToString(culture) + " mm";
                string Qpf_Snow = Settings.IsFahrenheit ?
                    weather.precipitation.qpf_snow_in.ToString("0.00", culture) + " in" : weather.precipitation.qpf_snow_cm.ToString(culture) + " cm";

                if (WeatherAPI.OpenWeatherMap.Equals(Settings.API) || WeatherAPI.MetNo.Equals(Settings.API))
                {
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                    if (!String.IsNullOrWhiteSpace(weather.precipitation.pop))
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, Chance));
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(weather.precipitation.pop))
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, Chance));
                    }
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                }
            }

            // Atmosphere
            if (!String.IsNullOrWhiteSpace(weather.atmosphere.pressure_mb))
            {
                var pressureVal = Settings.IsFahrenheit ? weather.atmosphere.pressure_in : weather.atmosphere.pressure_mb;
                var pressureUnit = Settings.IsFahrenheit ? "in" : "mb";

                if (float.TryParse(pressureVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressure))
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Pressure,
                        string.Format("{0} {1} {2}", WeatherUtils.GetPressureStateIcon(weather.atmosphere.pressure_trend), pressure.ToString(culture), pressureUnit)));
            }

            if (!String.IsNullOrWhiteSpace(weather.atmosphere.humidity))
            {
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Humidity,
                weather.atmosphere.humidity.EndsWith("%", StringComparison.Ordinal) ?
                        weather.atmosphere.humidity : weather.atmosphere.humidity + "%"));
            }

            if (!String.IsNullOrWhiteSpace(weather.atmosphere.dewpoint_f))
            {
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                       Settings.IsFahrenheit ?
                            Math.Round(float.Parse(weather.atmosphere.dewpoint_f)) + "º" :
                            Math.Round(float.Parse(weather.atmosphere.dewpoint_c)) + "º"));
            }

            if (!String.IsNullOrWhiteSpace(weather.atmosphere.visibility_mi))
            {
                var visibilityVal = Settings.IsFahrenheit ? weather.atmosphere.visibility_mi : weather.atmosphere.visibility_km;
                var visibilityUnit = Settings.IsFahrenheit ? "mi" : "km";

                if (float.TryParse(visibilityVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibility))
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                           string.Format("{0} {1}", visibility.ToString(culture), visibilityUnit)));
            }

            UVIndex = weather.condition.uv != null ? new UVIndexViewModel(weather.condition.uv) : null;

            if (weather.condition.feelslike_f != weather.condition.feelslike_c)
            {
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                       Settings.IsFahrenheit ?
                            Math.Round(weather.condition.feelslike_f) + "º" : Math.Round(weather.condition.feelslike_c) + "º"));
            }
            // Wind
            if (weather.condition.wind_mph >= 0 && weather.condition.wind_kph >= 0)
            {
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                   Settings.IsFahrenheit ?
                        String.Format("{0} mph, {1}", Math.Round(weather.condition.wind_mph), WeatherUtils.GetWindDirection(weather.condition.wind_degrees)) :
                        String.Format("{0} kph, {1}", Math.Round(weather.condition.wind_kph), WeatherUtils.GetWindDirection(weather.condition.wind_degrees)),
                   weather.condition.wind_degrees + 180));
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

        public Task UpdateViewAsync(Weather weather)
        {
            return Task.Run(() =>
            {
                UpdateView(weather);
            });
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