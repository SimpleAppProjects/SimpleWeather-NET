using SimpleWeather.ComponentModel;
using SimpleWeather.Icons;
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
#if WINDOWS_UWP
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endif

namespace SimpleWeather.Controls
{
    public class WeatherNowViewModel : IViewModel
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
        private string weatherSummary;

        // Weather Details
        private SunPhaseViewModel sunPhase;
        private SimpleObservableList<DetailItemViewModel> weatherDetails;
        private UVIndexViewModel uvIndex;
        private BeaufortViewModel beaufort;
        private MoonPhaseViewModel moonPhase;
        private AirQualityViewModel airQuality;
        private PollenViewModel pollen;

        // Background
        private ImageDataViewModel imageData;
        private Color defaultColor = Color.FromArgb(0xFF, 0, 0x70, 0xC0); // SimpleBlue;
        private Color pendingBackgroundColor = Color.FromArgb(0xFF, 0, 0x70, 0xC0);

        // Radar
        private WeatherUtils.Coordinate locationCoord;

        private String weatherCredit;
        private String weatherSource;

        private String weatherLocale = "EN";

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
#if WINDOWS_UWP
            Dispatcher?.LaunchOnUIThread(() =>
            {
#endif
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
#if WINDOWS_UWP
            });
#endif
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
        public string WeatherSummary { get => weatherSummary; private set { if (!Equals(weatherSummary, value)) { weatherSummary = value; OnPropertyChanged(nameof(WeatherSummary)); } } }
        public SunPhaseViewModel SunPhase { get => sunPhase; private set { if (!Equals(sunPhase, value)) { sunPhase = value; OnPropertyChanged(nameof(SunPhase)); } } }
        public SimpleObservableList<DetailItemViewModel> WeatherDetails { get => weatherDetails; private set { weatherDetails = value; OnPropertyChanged(nameof(WeatherDetails)); } }
        public UVIndexViewModel UVIndex { get => uvIndex; private set { if (!Equals(uvIndex, value)) { uvIndex = value; OnPropertyChanged(nameof(UVIndex)); } } }
        public BeaufortViewModel Beaufort { get => beaufort; private set { if (!Equals(beaufort, value)) { beaufort = value; OnPropertyChanged(nameof(Beaufort)); } } }
        public MoonPhaseViewModel MoonPhase { get => moonPhase; private set { if (!Equals(moonPhase, value)) { moonPhase = value; OnPropertyChanged(nameof(MoonPhase)); } } }
        public AirQualityViewModel AirQuality { get => airQuality; private set { if (!Equals(airQuality, value)) { airQuality = value; OnPropertyChanged(nameof(AirQuality)); } } }
        public PollenViewModel Pollen { get => pollen; private set { if (!Equals(pollen, value)) { pollen = value; OnPropertyChanged(nameof(Pollen)); } } }
        public ImageDataViewModel ImageData { get => imageData; private set { if (!Equals(imageData, value)) { imageData = value; OnPropertyChanged(nameof(ImageData)); } } }
        public Color PendingBackgroundColor { get => pendingBackgroundColor; private set { if (!Equals(pendingBackgroundColor, value)) { pendingBackgroundColor = value; OnPropertyChanged(nameof(PendingBackgroundColor)); } } }
        public WeatherUtils.Coordinate LocationCoord { get => locationCoord; private set { if (!Equals(locationCoord, value)) { locationCoord = value; OnPropertyChanged(nameof(LocationCoord)); } } }
        public string WeatherCredit { get => weatherCredit; private set { if (!Equals(weatherCredit, value)) { weatherCredit = value; OnPropertyChanged(nameof(WeatherCredit)); } } }
        public string WeatherSource { get => weatherSource; private set { if (!Equals(weatherSource, value)) { weatherSource = value; OnPropertyChanged(nameof(WeatherSource)); } } }
        public string WeatherLocale { get => weatherLocale; private set { if (!Equals(weatherLocale, value)) { weatherLocale = value; OnPropertyChanged(nameof(WeatherLocale)); } } }

        #endregion Properties

#if WINDOWS_UWP
        private CoreDispatcher Dispatcher;
#endif

        private WeatherManager wm;
        private Weather weather;
        private string unitCode;
        private string iconProvider;

#if WINDOWS_UWP
        public WeatherNowViewModel(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            Initialize();
        }
#endif

        public WeatherNowViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            wm = WeatherManager.GetInstance();
            WeatherDetails = new SimpleObservableList<DetailItemViewModel>();
            LocationCoord = new WeatherUtils.Coordinate(0, 0);
        }

        public WeatherNowViewModel(Weather weather) : this()
        {
            UpdateView(weather);
        }

        private void Forecasts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Forecasts));
        }

        private void HourlyForecasts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HourlyForecasts));
        }

        public void UpdateView(Weather weather)
        {
            if (weather?.IsValid() == true)
            {
                if (!Equals(this.weather, weather))
                {
                    this.weather = weather;

                    // Update backgrounds
                    ImageData = null;
                    PendingBackgroundColor = defaultColor;

                    // Location
                    Location = weather?.location?.name;

                    // Summary
                    WeatherSummary = weather?.condition?.summary;

                    // Additional Details
                    if (weather?.location?.latitude.HasValue == true && weather?.location?.longitude.HasValue == true)
                    {
                        LocationCoord.SetCoordinate(weather.location.latitude.Value, weather.location.longitude.Value);
                    }
                    else
                    {
                        LocationCoord.SetCoordinate(0, 0);
                    }

                    // Language
                    WeatherLocale = weather?.locale;

                    // Refresh locale/unit dependent values
                    RefreshView();
                }
                else if (!Equals(unitCode, Settings.UnitString) || !Equals(iconProvider, Settings.IconProvider))
                {
                    RefreshView();
                }
            }
        }

        private void RefreshView()
        {
            var culture = CultureUtils.UserCulture;
            var provider = WeatherManager.GetProvider(weather.source);
            var isFahrenheit = Units.FAHRENHEIT.Equals(Settings.TemperatureUnit);

            unitCode = Settings.UnitString;
            iconProvider = Settings.IconProvider;

            // Date Updated
            UpdateDate = WeatherUtils.GetLastBuildDate(weather);

            // Update Current Condition
            String tmpCurTemp;
            if (weather.condition.temp_f.HasValue && weather.condition.temp_f != weather.condition.temp_c)
            {
                var temp = isFahrenheit ? Math.Round(weather.condition.temp_f.Value) : Math.Round(weather.condition.temp_c.Value);
                tmpCurTemp = temp.ToString(culture);
            }
            else
            {
                tmpCurTemp = WeatherIcons.PLACEHOLDER;
            }
            var unitTemp = isFahrenheit ? Units.FAHRENHEIT : Units.CELSIUS;
            CurTemp = String.Format(culture, "{0}°{1}", tmpCurTemp, unitTemp);
            var weatherCondition = provider.SupportsWeatherLocale ? weather.condition.weather : provider.GetWeatherCondition(weather.condition.icon);
            CurCondition = (String.IsNullOrWhiteSpace(weatherCondition)) ? WeatherIcons.EM_DASH : weatherCondition;
            WeatherIcon = weather.condition.icon;

            var shouldHideHi = false;
            var shouldHideLo = false;

            if (weather.condition.high_f.HasValue && weather.condition.high_f != weather.condition.high_c)
            {
                var value = isFahrenheit ? Math.Round(weather.condition.high_f.Value) : Math.Round(weather.condition.high_c.Value);
                HiTemp = String.Format(culture, "{0}°", value);
            }
            else
            {
                HiTemp = null;
                shouldHideHi = true;
            }

            if (weather.condition.low_f.HasValue && weather.condition.low_f != weather.condition.low_c)
            {
                var value = isFahrenheit ? Math.Round(weather.condition.low_f.Value) : Math.Round(weather.condition.low_c.Value);
                LoTemp = String.Format(culture, "{0}°", value);
            }
            else
            {
                LoTemp = null;
                shouldHideLo = true;
            }

            ShowHiLo = (!shouldHideHi || !shouldHideLo) && !Equals(HiTemp, LoTemp);

            // WeatherDetails
            WeatherDetails.Clear();

            // Precipitation
            if (weather.precipitation != null)
            {
                string Chance = weather.precipitation.pop.HasValue ? weather.precipitation.pop.Value + "%" : null;

                if (weather.precipitation.pop.HasValue && weather.precipitation.pop >= 0)
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, weather.precipitation.pop.Value + "%"));
                if (weather.precipitation.qpf_rain_in.HasValue && weather.precipitation.qpf_rain_in >= 0)
                {
                    string unit = Settings.PrecipitationUnit;
                    float precipValue;
                    string precipUnit;

                    switch (unit)
                    {
                        case Units.INCHES:
                        default:
                            precipValue = weather.precipitation.qpf_rain_in.Value;
                            precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_in");
                            break;
                        case Units.MILLIMETERS:
                            precipValue = weather.precipitation.qpf_rain_mm.Value;
                            precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mm");
                            break;
                    }

                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain,
                        String.Format(culture, "{0:0.00} {1}", precipValue, precipUnit)));
                }
                if (weather.precipitation.qpf_snow_in.HasValue && weather.precipitation.qpf_snow_in >= 0)
                {
                    string unit = Settings.PrecipitationUnit;
                    float precipValue;
                    string precipUnit;

                    switch (unit)
                    {
                        case Units.INCHES:
                        default:
                            precipValue = weather.precipitation.qpf_snow_in.Value;
                            precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_in");
                            break;
                        case Units.MILLIMETERS:
                            precipValue = weather.precipitation.qpf_snow_cm.Value * 10;
                            precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mm");
                            break;
                    }

                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow,
                        String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit)));
                }
                if (weather.precipitation.cloudiness.HasValue && weather.precipitation.cloudiness >= 0)
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, weather.precipitation.cloudiness.Value + "%"));
            }

            // Atmosphere
            if (weather.atmosphere.pressure_mb.HasValue)
            {
                string unit = Settings.PressureUnit;
                float pressureVal;
                string pressureUnit;

                switch (unit)
                {
                    case Units.INHG:
                    default:
                        pressureVal = weather.atmosphere.pressure_in.Value;
                        pressureUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_inHg");
                        break;
                    case Units.MILLIBAR:
                        pressureVal = weather.atmosphere.pressure_mb.Value;
                        pressureUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mBar");
                        break;
                }

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
                    String.Format(culture, "{0}°",
                    isFahrenheit ?
                        Math.Round(weather.atmosphere.dewpoint_f.Value) :
                        Math.Round(weather.atmosphere.dewpoint_c.Value))));
            }

            if (weather.atmosphere.visibility_mi.HasValue && weather.atmosphere.visibility_mi >= 0)
            {
                string unit = Settings.DistanceUnit;
                int visibilityVal;
                string visibilityUnit;

                switch (unit)
                {
                    case Units.MILES:
                    default:
                        visibilityVal = (int)Math.Round(weather.atmosphere.visibility_mi.Value);
                        visibilityUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_miles");
                        break;
                    case Units.KILOMETERS:
                        visibilityVal = (int)Math.Round(weather.atmosphere.visibility_km.Value);
                        visibilityUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_kilometers");
                        break;
                }

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                    String.Format(culture, "{0:0.##} {1}", visibilityVal, visibilityUnit)));
            }

            UVIndex = weather.condition.uv?.index != null ? new UVIndexViewModel(weather.condition.uv) : null;

            if (weather.condition.feelslike_f.HasValue && (weather.condition.feelslike_f != weather.condition.feelslike_c))
            {
                var value = isFahrenheit ? Math.Round(weather.condition.feelslike_f.Value) : Math.Round(weather.condition.feelslike_c.Value);

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                       String.Format(culture, "{0}°", value)));
            }

            // Wind
            if (weather.condition.wind_mph.HasValue && weather.condition.wind_mph >= 0 &&
                weather.condition.wind_degrees.HasValue && weather.condition.wind_degrees >= 0)
            {
                string unit = Settings.SpeedUnit;
                int speedVal;
                string speedUnit;

                switch (unit)
                {
                    case Units.MILES_PER_HOUR:
                    default:
                        speedVal = (int)Math.Round(weather.condition.wind_mph.Value);
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mph");
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(weather.condition.wind_kph.Value);
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_kph");
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(weather.condition.wind_kph.Value));
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_msec");
                        break;
                }

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                   String.Format(culture, "{0} {1}, {2}", speedVal, speedUnit, WeatherUtils.GetWindDirection(weather.condition.wind_degrees.Value)),
                   weather.condition.wind_degrees.Value + 180));
            }

            if (weather.condition.windgust_mph.HasValue && (weather.condition.windgust_mph != weather.condition.windgust_kph))
            {
                string unit = Settings.SpeedUnit;
                int speedVal;
                string speedUnit;

                switch (unit)
                {
                    case Units.MILES_PER_HOUR:
                    default:
                        speedVal = (int)Math.Round(weather.condition.windgust_mph.Value);
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mph");
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(weather.condition.windgust_kph.Value);
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_kph");
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(weather.condition.windgust_kph.Value));
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_msec");
                        break;
                }

                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.WindGust,
                    String.Format(culture, "{0} {1}", speedVal, speedUnit)));
            }

            Beaufort = weather.condition.beaufort != null ? new BeaufortViewModel(weather.condition.beaufort) : null;

            // Astronomy
            if (weather.astronomy != null)
            {
                SunPhase = new SunPhaseViewModel(weather.astronomy);

                if (weather.astronomy.sunrise > DateTime.MinValue && weather.astronomy.sunset > DateTime.MinValue)
                {
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunrise,
                           weather.astronomy.sunrise.ToString("t", culture)));
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunset,
                           weather.astronomy.sunset.ToString("t", culture)));
                }

                if (weather.astronomy.moonrise != null && weather.astronomy.moonset != null)
                {
                    if (weather.astronomy.moonrise > DateTime.MinValue)
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonrise,
                           weather.astronomy.moonrise.ToString("t", culture)));
                    }

                    if (weather.astronomy.moonset > DateTime.MinValue)
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonset,
                           weather.astronomy.moonset.ToString("t", culture)));
                    }
                }

                MoonPhase = weather.astronomy.moonphase != null ? new MoonPhaseViewModel(weather.astronomy.moonphase) : null;
            }
            else
            {
                SunPhase = null;
                MoonPhase = null;
            }

#if WINDOWS_UWP
            Dispatcher?.LaunchOnUIThread(() =>
            {
#endif
                OnPropertyChanged(nameof(WeatherDetails));
                WeatherDetails.NotifyCollectionChanged();
#if WINDOWS_UWP
            });
#endif

            // Additional Details
            AirQuality = weather.condition.airQuality?.index != null ? new AirQualityViewModel(weather.condition.airQuality) : null;
            Pollen = weather.condition.pollen != null ? new PollenViewModel(weather.condition.pollen) : null;

            WeatherSource = weather?.source;

            string creditPrefix = SimpleLibrary.GetInstance().ResLoader.GetString("credit_prefix");
            WeatherCredit = String.Format("{0} {1}",
                creditPrefix, WeatherAPI.APIs.FirstOrDefault(WApi => Equals(WeatherSource, WApi.Value))?.ToString() ?? WeatherIcons.EM_DASH);
        }

        public async Task UpdateBackground()
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
        }

        public bool IsValid => weather != null && weather.IsValid();
    }
}