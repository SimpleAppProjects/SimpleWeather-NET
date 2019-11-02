using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using SimpleWeather.UWP;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.System.UserProfile;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

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
        private string sunrise;
        private string sunset;
        // Forecast
        private ObservableCollection<ForecastItemViewModel> forecasts;
        // Weather Details
        private ObservableCollection<DetailItemViewModel> weatherDetails;
        // Additional Details
        private WeatherExtrasViewModel extras;
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
        #endregion

        #region Properties
        public string Location { get => location; set { location = value; OnPropertyChanged("Location"); } }
        public string UpdateDate { get => updateDate; set { updateDate = value; OnPropertyChanged("UpdateDate"); } }
        public string CurTemp { get => curTemp; set { curTemp = value; OnPropertyChanged("CurTemp"); } }
        public string CurCondition { get => curCondition; set { curCondition = value; OnPropertyChanged("CurCondition"); } }
        public string WeatherIcon { get => weatherIcon; set { weatherIcon = value; OnPropertyChanged("WeatherIcon"); } }
        public string Sunrise { get => sunrise; set { sunrise = value; OnPropertyChanged("Sunrise"); } }
        public string Sunset { get => sunset; set { sunset = value; OnPropertyChanged("Sunset"); } }
        public ObservableCollection<ForecastItemViewModel> Forecasts { get => forecasts; set { forecasts = value; OnPropertyChanged("Forecasts"); } }
        public ObservableCollection<DetailItemViewModel> WeatherDetails { get => weatherDetails; set { weatherDetails = value; OnPropertyChanged("WeatherDetails"); } }
        public WeatherExtrasViewModel Extras { get => extras; set { extras = value; OnPropertyChanged("Extras"); } }
        public string BackgroundURI { get => backgroundURI; set { backgroundURI = value; OnPropertyChanged("BackgroundURI"); } }
        public ImageDataViewModel ImageData { get => imageData; set { imageData = value; OnPropertyChanged("ImageData"); } }
        public Color PendingBackgroundColor { get => pendingBackgroundColor; set { pendingBackgroundColor = value; OnPropertyChanged("PendingBackgroundColor"); } }
        public ElementTheme BackgroundTheme { get => backgroundTheme; set { backgroundTheme = value; OnPropertyChanged("BackgroundTheme"); } }
        public string WeatherCredit { get => weatherCredit; set { weatherCredit = value; OnPropertyChanged("WeatherCredit"); } }
        public string WeatherSource { get => weatherSource; set { weatherSource = value; OnPropertyChanged("WeatherSource"); } }
        public string WeatherLocale { get => weatherLocale; set { weatherLocale = value; OnPropertyChanged("WeatherLocale"); } }
        #endregion

        private WeatherManager wm;

        public WeatherNowViewModel()
        {
            wm = WeatherManager.GetInstance();

            Forecasts = new ObservableCollection<ForecastItemViewModel>();
            WeatherDetails = new ObservableCollection<DetailItemViewModel>();
            Extras = new WeatherExtrasViewModel();
        }

        public WeatherNowViewModel(Weather weather)
        {
            wm = WeatherManager.GetInstance();

            Forecasts = new ObservableCollection<ForecastItemViewModel>();
            WeatherDetails = new ObservableCollection<DetailItemViewModel>();
            Extras = new WeatherExtrasViewModel();
            UpdateView(weather);
        }

        public void UpdateView(Weather weather)
        {
            if (weather.IsValid())
            {
                var userlang = GlobalizationPreferences.Languages[0];
                var culture = new CultureInfo(userlang);

                Extras.UpdateView(weather);
                OnPropertyChanged("Extras");

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
                Location = weather.location.name;

                // Date Updated
                UpdateDate = WeatherUtils.GetLastBuildDate(weather);

                // Update Current Condition
                CurTemp = Settings.IsFahrenheit ?
                    Math.Round(weather.condition.temp_f) + WeatherIcons.FAHRENHEIT : Math.Round(weather.condition.temp_c) + WeatherIcons.CELSIUS;
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
                            String.Format("{0} mph, {1}", Math.Round(weather.condition.wind_mph), WeatherUtils.GetWindDirection(weather.condition.wind_degrees)) :
                            String.Format("{0} kph, {1}", Math.Round(weather.condition.wind_kph), WeatherUtils.GetWindDirection(weather.condition.wind_degrees)),
                       weather.condition.wind_degrees));

                if (weather.condition.beaufort != null)
                {
                    WeatherDetails.Add(new DetailItemViewModel(weather.condition.beaufort.scale,
                            weather.condition.beaufort.desc));
                }

                // Astronomy
                Sunrise = weather.astronomy.sunrise.ToString("t", culture);
                Sunset = weather.astronomy.sunset.ToString("t", culture);
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunrise, Sunrise));
                WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Sunset, Sunset));

                if (weather.astronomy.moonrise != null && weather.astronomy.moonset != null
                        && weather.astronomy.moonrise.CompareTo(DateTime.MinValue) > 0
                        && weather.astronomy.moonset.CompareTo(DateTime.MinValue) > 0)
                {
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonrise,
                           weather.astronomy.moonrise.ToString("t", culture)));
                    WeatherDetails.Add(new DetailItemViewModel(WeatherDetailsType.Moonset,
                           weather.astronomy.moonset.ToString("t", culture)));
                }

                if (weather.astronomy.moonphase != null)
                {
                    WeatherDetails.Add(new DetailItemViewModel(weather.astronomy.moonphase.phase,
                           weather.astronomy.moonphase.desc));
                }

                OnPropertyChanged("WeatherDetails");

                // Add UI elements
                Forecasts.Clear();
                bool isDayAndNt = Extras.TextForecast.Count == weather.forecast.Length * 2;
                bool addTextFct = isDayAndNt || Extras.TextForecast.Count == weather.forecast.Length;
                for (int i = 0; i < weather.forecast.Length; i++)
                {
                    Forecast forecast = weather.forecast[i];
                    ForecastItemViewModel forecastView;
                    
                    if (addTextFct)
                    {
                        if (isDayAndNt)
                            forecastView = new ForecastItemViewModel(forecast, Extras.TextForecast[i * 2], Extras.TextForecast[(i * 2) + 1]);
                        else
                            forecastView = new ForecastItemViewModel(forecast, Extras.TextForecast[i]);
                    }
                    else
                        forecastView = new ForecastItemViewModel(forecast);

                    Forecasts.Add(forecastView);
                }
                OnPropertyChanged("Forecasts");

                // Additional Details
                WeatherSource = weather.source;
                string creditPrefix = "Data from";

                creditPrefix = App.ResLoader.GetString("Credit_Prefix");

                WeatherCredit = String.Format("{0} {1}",
                    creditPrefix, WeatherAPI.APIs.First(WApi => WeatherSource.Equals(WApi.Value)));

                // Language
                WeatherLocale = weather.locale;
            }
        }

        public class ImageDataViewModel
        {
            public string ArtistName { get; set; }
            public string ArtistLink { get; set; }
            public string SiteName { get; set; }
            public string SiteLink { get; set; }
        }

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
    }
}
