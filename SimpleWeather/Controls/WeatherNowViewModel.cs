using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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

        // Weather Details
        private string sunrise;
        private string sunset;
        private ObservableCollection<DetailItemViewModel> weatherDetails;
        private UVIndexViewModel uvIndex;
        private BeaufortViewModel beaufort;
        private MoonPhaseViewModel moonPhase;
        private AirQualityViewModel airQuality;

        // Forecast
        private ObservableForecastLoadingCollection<ForecastItemViewModel> forecasts;

        // Additional Details
        private ObservableForecastLoadingCollection<HourlyForecastItemViewModel> hourlyForecasts;
        private ObservableCollection<WeatherAlertViewModel> alerts;

        // Background
        private String backgroundURI;
        private ImageDataViewModel imageData;
        private Color pendingBackgroundColor = Color.FromArgb(255, 0, 111, 191);
        private ElementTheme backgroundTheme = ElementTheme.Dark;

        private String weatherCredit;
        private String weatherSource;

        private String weatherLocale = "EN";

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion DependencyProperties

        #region Properties

        public string Location { get => location; set { location = value; OnPropertyChanged(nameof(Location)); } }
        public string UpdateDate { get => updateDate; set { updateDate = value; OnPropertyChanged(nameof(UpdateDate)); } }
        public string CurTemp { get => curTemp; set { curTemp = value; OnPropertyChanged(nameof(CurTemp)); } }
        public string CurCondition { get => curCondition; set { curCondition = value; OnPropertyChanged(nameof(CurCondition)); } }
        public string WeatherIcon { get => weatherIcon; set { weatherIcon = value; OnPropertyChanged(nameof(WeatherIcon)); } }
        public string HiTemp { get => hiTemp; set { hiTemp = value; OnPropertyChanged(nameof(HiTemp)); } }
        public string LoTemp { get => loTemp; set { loTemp = value; OnPropertyChanged(nameof(LoTemp)); } }
        public string Sunrise { get => sunrise; set { sunrise = value; OnPropertyChanged(nameof(Sunrise)); } }
        public string Sunset { get => sunset; set { sunset = value; OnPropertyChanged(nameof(Sunset)); } }
        public ObservableForecastLoadingCollection<ForecastItemViewModel> Forecasts { get => forecasts; set { forecasts = value; OnPropertyChanged(nameof(Forecasts)); } }
        public ObservableForecastLoadingCollection<HourlyForecastItemViewModel> HourlyForecasts { get => hourlyForecasts; set { hourlyForecasts = value; OnPropertyChanged(nameof(HourlyForecasts)); } }
        public ObservableCollection<WeatherAlertViewModel> Alerts { get => alerts; private set { alerts = value; OnPropertyChanged(nameof(Alerts)); } }
        public ObservableCollection<DetailItemViewModel> WeatherDetails { get => weatherDetails; private set { weatherDetails = value; OnPropertyChanged(nameof(WeatherDetails)); } }
        public UVIndexViewModel UVIndex { get => uvIndex; private set { uvIndex = value; OnPropertyChanged(nameof(UVIndex)); } }
        public BeaufortViewModel Beaufort { get => beaufort; private set { beaufort = value; OnPropertyChanged(nameof(Beaufort)); } }
        public MoonPhaseViewModel MoonPhase { get => moonPhase; private set { moonPhase = value; OnPropertyChanged(nameof(MoonPhase)); } }
        public AirQualityViewModel AirQuality { get => airQuality; private set { airQuality = value; OnPropertyChanged(nameof(AirQuality)); } }
        public string BackgroundURI { get => backgroundURI; set { backgroundURI = value; OnPropertyChanged(nameof(BackgroundURI)); } }
        public ImageDataViewModel ImageData { get => imageData; set { imageData = value; OnPropertyChanged(nameof(ImageData)); } }
        public Color PendingBackgroundColor { get => pendingBackgroundColor; set { pendingBackgroundColor = value; OnPropertyChanged(nameof(PendingBackgroundColor)); } }
        public ElementTheme BackgroundTheme { get => backgroundTheme; set { backgroundTheme = value; OnPropertyChanged(nameof(BackgroundTheme)); } }
        public string WeatherCredit { get => weatherCredit; set { weatherCredit = value; OnPropertyChanged(nameof(WeatherCredit)); } }
        public string WeatherSource { get => weatherSource; set { weatherSource = value; OnPropertyChanged(nameof(WeatherSource)); } }
        public string WeatherLocale { get => weatherLocale; set { weatherLocale = value; OnPropertyChanged(nameof(WeatherLocale)); } }

        #endregion Properties

        private WeatherManager wm;
        private Weather weather;

        public WeatherNowViewModel()
        {
            wm = WeatherManager.GetInstance();

            Forecasts = new ObservableForecastLoadingCollection<ForecastItemViewModel>();
            Forecasts.CollectionChanged += Forecasts_CollectionChanged;

            HourlyForecasts = new ObservableForecastLoadingCollection<HourlyForecastItemViewModel>();
            HourlyForecasts.CollectionChanged += HourlyForecasts_CollectionChanged;

            WeatherDetails = new ObservableCollection<DetailItemViewModel>();
            Alerts = new ObservableCollection<WeatherAlertViewModel>();
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
            if ((bool)weather?.IsValid() && !Object.Equals(this.weather, weather))
            {
                this.weather = weather;

                var userlang = GlobalizationPreferences.Languages[0];
                var culture = new CultureInfo(userlang);

                // Update backgrounds
                BackgroundURI = wm.GetBackgroundURI(weather);
                PendingBackgroundColor = wm.GetWeatherBackgroundColor(weather);
                BackgroundTheme = ColorUtils.IsSuperLight(PendingBackgroundColor) ?
                    ElementTheme.Light : ElementTheme.Dark;

                if (BackgroundURI.Contains("DaySky"))
                {
                    ImageData = bgAttribution["DaySky"];
                }
                else if (BackgroundURI.Contains("FoggySky"))
                {
                    ImageData = bgAttribution["FoggySky"];
                }
                else if (BackgroundURI.Contains("NightSky"))
                {
                    ImageData = bgAttribution["NightSky"];
                }
                else if (BackgroundURI.Contains("PartlyCloudy-Day"))
                {
                    ImageData = bgAttribution["PartlyCloudy-Day"];
                }
                else if (BackgroundURI.Contains("RainyDay"))
                {
                    ImageData = bgAttribution["RainyDay"];
                }
                else if (BackgroundURI.Contains("RainyNight"))
                {
                    ImageData = bgAttribution["RainyNight"];
                }
                else if (BackgroundURI.Contains("Snow-Windy"))
                {
                    ImageData = bgAttribution["Snow-Windy"];
                }
                else if (BackgroundURI.Contains("Snow"))
                {
                    ImageData = bgAttribution["Snow"];
                }
                else if (BackgroundURI.Contains("StormySky"))
                {
                    ImageData = bgAttribution["StormySky"];
                }
                else if (BackgroundURI.Contains("Thunderstorm-Day"))
                {
                    ImageData = bgAttribution["Thunderstorm-Day"];
                }
                else if (BackgroundURI.Contains("Thunderstorm-Night"))
                {
                    ImageData = bgAttribution["Thunderstorm-Night"];
                }
                else
                {
                    ImageData = null;
                }

                // Location
                Location = weather?.location?.name;

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
                    tmpCurTemp = "---";
                }
                var unitTemp = Settings.IsFahrenheit ? WeatherIcons.FAHRENHEIT : WeatherIcons.CELSIUS;
                CurTemp = tmpCurTemp + unitTemp;
                CurCondition = (String.IsNullOrWhiteSpace(weather.condition.weather)) ? "---" : weather.condition.weather;
                WeatherIcon = weather.condition.icon;

                if (weather.condition.high_f != weather.condition.high_c && weather.condition.low_f != weather.condition.low_c)
                {
                    HiTemp = (int)(Settings.IsFahrenheit ? Math.Round(weather.condition.high_f) : Math.Round(weather.condition.high_c)) + "°";
                    LoTemp = (int)(Settings.IsFahrenheit ? Math.Round(weather.condition.low_f) : Math.Round(weather.condition.low_c)) + "°";
                }
                else
                {
                    HiTemp = null;
                    LoTemp = null;
                }

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
                    Sunrise = weather.astronomy.sunrise.ToString("t", culture);
                    Sunset = weather.astronomy.sunset.ToString("t", culture);

                    if (weather.astronomy.moonrise != null && weather.astronomy.moonset != null
                            && weather.astronomy.moonrise.CompareTo(DateTime.MinValue) > 0
                            && weather.astronomy.moonset.CompareTo(DateTime.MinValue) > 0)
                    {
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonrise,
                               weather.astronomy.moonrise.ToString("t", culture)));
                        WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonset,
                               weather.astronomy.moonset.ToString("t", culture)));
                    }

                    MoonPhase = new MoonPhaseViewModel(weather.astronomy.moonphase);
                }
                else
                {
                    Sunrise = null;
                    Sunset = null;
                    MoonPhase = null;
                }
                OnPropertyChanged(nameof(WeatherDetails));

                // Add UI elements
                if (weather?.forecast?.Count > 0)
                {
                    Forecasts.Clear();
                    int textForecastSize = (weather?.txt_forecast?.Count).GetValueOrDefault(0);

                    bool isDayAndNt = textForecastSize == weather?.forecast?.Count * 2;
                    bool addTextFct = isDayAndNt || textForecastSize == weather?.forecast?.Count;
                    for (int i = 0; i < weather?.forecast?.Count; i++)
                    {
                        Forecast forecast = weather.forecast[i];
                        ForecastItemViewModel forecastView;

                        if (addTextFct)
                        {
                            if (isDayAndNt)
                                forecastView = new ForecastItemViewModel(forecast, weather?.txt_forecast[i * 2], weather?.txt_forecast[(i * 2) + 1]);
                            else
                                forecastView = new ForecastItemViewModel(forecast, weather?.txt_forecast[i]);
                        }
                        else
                            forecastView = new ForecastItemViewModel(forecast);

                        Forecasts.Add(forecastView);
                    }
                }
                else
                {
                    // Let collection handle changes (clearing, etc.)
                    Forecasts.SetWeather(weather);
                }
                OnPropertyChanged(nameof(Forecasts));

                if (weather?.hr_forecast?.Any() == true)
                {
                    HourlyForecasts.Clear();

                    foreach (HourlyForecast hr_forecast in weather.hr_forecast)
                    {
                        var hrforecastView = new HourlyForecastItemViewModel(hr_forecast);
                        HourlyForecasts.Add(hrforecastView);
                    }
                }
                else
                {
                    // Let collection handle changes (clearing, etc.)
                    HourlyForecasts.SetWeather(weather);
                }
                OnPropertyChanged(nameof(HourlyForecast));

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

                // Additional Details
                AirQuality = weather.condition.airQuality != null ? new AirQualityViewModel(weather.condition.airQuality) : null;

                // Additional Details
                WeatherSource = weather?.source;
                string creditPrefix = "Data from";

                creditPrefix = SimpleLibrary.ResLoader.GetString("Credit_Prefix");

                WeatherCredit = String.Format("{0} {1}",
                    creditPrefix, WeatherAPI.APIs.First(WApi => WeatherSource.Equals(WApi.Value)));

                // Language
                WeatherLocale = weather.locale;
            }
        }

        public bool IsValid => weather != null && weather.IsValid();

        private static readonly ReadOnlyDictionary<String, ImageDataViewModel> bgAttribution =
            new ReadOnlyDictionary<string, ImageDataViewModel>(new Dictionary<String, ImageDataViewModel>()
            {
                {
                    "DaySky", new ImageDataViewModel()
                    {
                        ArtistLink = "https://www.pexels.com/@amychandra?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        ArtistName = "Amy Chandra",
                        SiteLink = "https://www.pexels.com/photo/boat-on-ocean-789152/?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        SiteName = "Pexels"
                    }
                },
                {
                    "FoggySky", new ImageDataViewModel()
                    {
                        ArtistLink = "https://unsplash.com/photos/fjY26eEE78s?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        ArtistName = "Niklas Herrmann",
                        SiteLink = "https://unsplash.com/photos/fjY26eEE78s/?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        SiteName = "Unsplash"
                    }
                },
                {
                    "NightSky", new ImageDataViewModel()
                    {
                        ArtistLink = "https://www.pexels.com/@apasaric?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        ArtistName = "Aleksandar Pasaric",
                        SiteLink = "https://www.pexels.com/photo/dark-starry-sky-1694000/?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        SiteName = "Pexels"
                    }
                },
                {
                    "PartlyCloudy-Day", new ImageDataViewModel()
                    {
                        ArtistLink = "https://www.pexels.com/@grizzlybear?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        ArtistName = "Jonathan Petersson",
                        SiteLink = "https://www.pexels.com/photo/air-atmosphere-blue-bright-436383/?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        SiteName = "Pexels"
                    }
                },
                {
                    "RainyDay", new ImageDataViewModel()
                    {
                        ArtistLink = "https://pixabay.com/users/Olichel-529835/",
                        ArtistName = "Olichel",
                        SiteLink = "https://pixabay.com/images/id-2179933/",
                        SiteName = "Pixabay"
                    }
                },
                {
                    "RainyNight", new ImageDataViewModel()
                    {
                        ArtistLink = "https://unsplash.com/@walterrandlehoff?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        ArtistName = "Walter Randlehoff",
                        SiteLink = "https://unsplash.com/photos/jg486JWaYLc/?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        SiteName = "Unsplash"
                    }
                },
                {
                    "Snow", new ImageDataViewModel()
                    {
                        ArtistLink = "https://www.pexels.com/@deathless?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        ArtistName = "Mircea Iancu",
                        SiteLink = "https://www.pexels.com/photo/snow-flakes-948857/?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        SiteName = "Pexels"
                    }
                },
                {
                    "Snow-Windy", new ImageDataViewModel()
                    {
                        ArtistLink = "https://unsplash.com/@ceo77?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        ArtistName = "Christian SPULLER",
                        SiteLink = "https://unsplash.com/photos/Oaec-W0b2ss/?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        SiteName = "Unsplash"
                    }
                },
                {
                    "StormySky", new ImageDataViewModel()
                    {
                        ArtistLink = "https://unsplash.com/@anandu?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        ArtistName = "Anandu Vinod",
                        SiteLink = "https://unsplash.com/photos/pbxwxwfI0B4/?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        SiteName = "Unsplash"
                    }
                },
                {
                    "Thunderstorm-Day", new ImageDataViewModel()
                    {
                        ArtistLink = "https://www.pexels.com/@eberhardgross?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        ArtistName = "eberhard grossgasteiger",
                        SiteLink = "https://www.pexels.com/photo/photography-of-dark-clouds-1074428/?utm_content=attributionCopyText&amp;utm_medium=referral&amp;utm_source=pexels",
                        SiteName = "Pexels"
                    }
                },
                {
                    "Thunderstorm-Night", new ImageDataViewModel()
                    {
                        ArtistLink = "https://unsplash.com/@aliarifsoydas?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        ArtistName = "Ali Arif Soydaş",
                        SiteLink = "https://unsplash.com/photos/wwzLVOvzy6w/?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText",
                        SiteName = "Unsplash"
                    }
                },
            });
    }
}