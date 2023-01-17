using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using ResStrings = SimpleWeather.Resources.Strings.Resources;
using ResUnits = SimpleWeather.Resources.Strings.Units;

namespace SimpleWeather.Common.Controls
{
    public class WeatherUiModel
    {
        public string Location { get; internal set; }
        public string UpdateDate { get; private set; }
        public string CurTemp { get; private set; }
        public string CurCondition { get; private set; }
        public string WeatherIcon { get; private set; } = WeatherIcons.NA;
        public string HiTemp { get; private set; }
        public string LoTemp { get; private set; }
        public bool ShowHiLo { get; private set; } = false;
        public string WeatherSummary { get; private set; }
        public SunPhaseViewModel SunPhase { get; private set; }
        public UVIndexViewModel UVIndex { get; private set; }
        public BeaufortViewModel Beaufort { get; private set; }
        public MoonPhaseViewModel MoonPhase { get; private set; }
        public AirQualityViewModel AirQuality { get; private set; }
        public PollenViewModel Pollen { get; private set; }
        public WeatherUtils.Coordinate LocationCoord { get; } = new WeatherUtils.Coordinate(0, 0);
        public string WeatherCredit { get; private set; }
        public string WeatherSource { get; private set; }
        public string WeatherLocale { get; private set; }

        public DetailsMap<WeatherDetailsType, DetailItemViewModel> WeatherDetailsMap { get; } = new DetailsMap<WeatherDetailsType, DetailItemViewModel>();
        public IReadOnlyCollection<DetailItemViewModel> WeatherDetails { get => WeatherDetailsMap.ValuesWrapper; }

        public string Query => WeatherData?.query;

        internal Weather WeatherData { get; private set; }

        public string TempUnit { get; private set; }

        private string unitCode;
        //private string localeCode;
        public string IconProvider { get; private set; }

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public WeatherUiModel() { }

        public WeatherUiModel(Weather weather) : this()
        {
            UpdateView(weather);
        }

        public void UpdateView(Weather weather)
        {
            if (weather?.IsValid() == true)
            {
                if (!Equals(this.WeatherData, weather))
                {
                    this.WeatherData = weather;

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

                    // Additional Details
                    WeatherSource = weather?.source;

                    // Language
                    WeatherLocale = weather?.locale;

                    // Refresh locale/unit dependent values
                    RefreshView();
                }
                else if (!Equals(unitCode, SettingsManager.UnitString) || !Equals(IconProvider, SettingsManager.IconProvider))
                {
                    RefreshView();
                }
            }
        }

        private void RefreshView()
        {
            var culture = CultureUtils.UserCulture;
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherData.source);
            var isFahrenheit = Units.FAHRENHEIT.Equals(SettingsManager.TemperatureUnit);

            TempUnit = SettingsManager.TemperatureUnit;
            unitCode = SettingsManager.UnitString;

            IconProvider = SettingsManager.IconProvider;

            // Date Updated
            UpdateDate = WeatherUtils.GetLastBuildDate(WeatherData);

            // Update Current Condition
            if (WeatherData.condition.temp_f.HasValue && WeatherData.condition.temp_f != WeatherData.condition.temp_c)
            {
                var temp = isFahrenheit ? Math.Round(WeatherData.condition.temp_f.Value) : Math.Round(WeatherData.condition.temp_c.Value);
                CurTemp = String.Format(culture, "{0}°{1}", temp.ToString(culture), TempUnit);
            }
            else
            {
                CurTemp = WeatherIcons.PLACEHOLDER;
            }
            var weatherCondition = provider.SupportsWeatherLocale ? WeatherData.condition.weather : provider.GetWeatherCondition(WeatherData.condition.icon);
            CurCondition = (String.IsNullOrWhiteSpace(weatherCondition)) ? WeatherIcons.EM_DASH : weatherCondition;
            WeatherIcon = WeatherData.condition.icon;

            var shouldHideHi = false;
            var shouldHideLo = false;

            if (WeatherData.condition.high_f.HasValue && WeatherData.condition.high_f != WeatherData.condition.high_c)
            {
                var value = isFahrenheit ? Math.Round(WeatherData.condition.high_f.Value) : Math.Round(WeatherData.condition.high_c.Value);
                HiTemp = String.Format(culture, "{0}°", value);
            }
            else
            {
                HiTemp = null;
                shouldHideHi = true;
            }

            if (WeatherData.condition.low_f.HasValue && WeatherData.condition.low_f != WeatherData.condition.low_c)
            {
                var value = isFahrenheit ? Math.Round(WeatherData.condition.low_f.Value) : Math.Round(WeatherData.condition.low_c.Value);
                LoTemp = String.Format(culture, "{0}°", value);
            }
            else
            {
                LoTemp = null;
                shouldHideLo = true;
            }

            ShowHiLo = (!shouldHideHi || !shouldHideLo) && !Equals(HiTemp, LoTemp);

            // WeatherDetails
            WeatherDetailsMap.Clear();

            // Precipitation
            if (WeatherData.precipitation != null)
            {
                string Chance = WeatherData.precipitation.pop.HasValue ? WeatherData.precipitation.pop.Value + "%" : null;

                if (WeatherData.precipitation.pop.HasValue && WeatherData.precipitation.pop >= 0)
                    WeatherDetailsMap.Add(WeatherDetailsType.PoPChance, new DetailItemViewModel(WeatherDetailsType.PoPChance, WeatherData.precipitation.pop.Value + "%"));
                if (WeatherData.precipitation.qpf_rain_in.HasValue && WeatherData.precipitation.qpf_rain_in >= 0)
                {
                    string unit = SettingsManager.PrecipitationUnit;
                    float precipValue;
                    string precipUnit;

                    switch (unit)
                    {
                        case Units.INCHES:
                        default:
                            precipValue = WeatherData.precipitation.qpf_rain_in.Value;
                            precipUnit = ResUnits.unit_in;
                            break;
                        case Units.MILLIMETERS:
                            precipValue = WeatherData.precipitation.qpf_rain_mm.Value;
                            precipUnit = ResUnits.unit_mm;
                            break;
                    }

                    WeatherDetailsMap.Add(WeatherDetailsType.PoPRain, new DetailItemViewModel(WeatherDetailsType.PoPRain,
                        String.Format(culture, "{0:0.00} {1}", precipValue, precipUnit)));
                }
                if (WeatherData.precipitation.qpf_snow_in.HasValue && WeatherData.precipitation.qpf_snow_in >= 0)
                {
                    string unit = SettingsManager.PrecipitationUnit;
                    float precipValue;
                    string precipUnit;

                    switch (unit)
                    {
                        case Units.INCHES:
                        default:
                            precipValue = WeatherData.precipitation.qpf_snow_in.Value;
                            precipUnit = ResUnits.unit_in;
                            break;
                        case Units.MILLIMETERS:
                            precipValue = WeatherData.precipitation.qpf_snow_cm.Value * 10;
                            precipUnit = ResUnits.unit_mm;
                            break;
                    }

                    WeatherDetailsMap.Add(WeatherDetailsType.PoPSnow, new DetailItemViewModel(WeatherDetailsType.PoPSnow,
                        String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit)));
                }
                if (WeatherData.precipitation.cloudiness.HasValue && WeatherData.precipitation.cloudiness >= 0)
                    WeatherDetailsMap.Add(WeatherDetailsType.PoPCloudiness, new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, WeatherData.precipitation.cloudiness.Value + "%"));
            }

            // Atmosphere
            if (WeatherData.atmosphere.pressure_mb.HasValue)
            {
                string unit = SettingsManager.PressureUnit;
                float pressureVal;
                string pressureUnit;

                switch (unit)
                {
                    case Units.INHG:
                    default:
                        pressureVal = WeatherData.atmosphere.pressure_in.Value;
                        pressureUnit = ResUnits.unit_inHg;
                        break;
                    case Units.MILLIBAR:
                        pressureVal = WeatherData.atmosphere.pressure_mb.Value;
                        pressureUnit = ResUnits.unit_mBar;
                        break;
                }

                WeatherDetailsMap.Add(WeatherDetailsType.Pressure, new DetailItemViewModel(WeatherDetailsType.Pressure,
                    String.Format(culture, "{0} {1:0.00} {2}",
                    WeatherUtils.GetPressureStateIcon(WeatherData.atmosphere.pressure_trend),
                    pressureVal, pressureUnit)));
            }

            if (WeatherData.atmosphere.humidity.HasValue)
            {
                WeatherDetailsMap.Add(WeatherDetailsType.Humidity, new DetailItemViewModel(WeatherDetailsType.Humidity,
                    String.Format(culture, "{0}%", WeatherData.atmosphere.humidity.Value)));
            }

            if (WeatherData.atmosphere.dewpoint_f.HasValue && (WeatherData.atmosphere.dewpoint_f != WeatherData.atmosphere.dewpoint_c))
            {
                WeatherDetailsMap.Add(WeatherDetailsType.Dewpoint, new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                    String.Format(culture, "{0}°",
                    isFahrenheit ?
                        Math.Round(WeatherData.atmosphere.dewpoint_f.Value) :
                        Math.Round(WeatherData.atmosphere.dewpoint_c.Value))));
            }

            if (WeatherData.atmosphere.visibility_mi.HasValue && WeatherData.atmosphere.visibility_mi >= 0)
            {
                string unit = SettingsManager.DistanceUnit;
                int visibilityVal;
                string visibilityUnit;

                switch (unit)
                {
                    case Units.MILES:
                    default:
                        visibilityVal = (int)Math.Round(WeatherData.atmosphere.visibility_mi.Value);
                        visibilityUnit = ResUnits.unit_miles;
                        break;
                    case Units.KILOMETERS:
                        visibilityVal = (int)Math.Round(WeatherData.atmosphere.visibility_km.Value);
                        visibilityUnit = ResUnits.unit_kilometers;
                        break;
                }

                WeatherDetailsMap.Add(WeatherDetailsType.Visibility, new DetailItemViewModel(WeatherDetailsType.Visibility,
                    String.Format(culture, "{0:0.##} {1}", visibilityVal, visibilityUnit)));
            }

            UVIndex = WeatherData.condition.uv?.index != null ? new UVIndexViewModel(WeatherData.condition.uv) : null;

            if (WeatherData.condition.feelslike_f.HasValue && (WeatherData.condition.feelslike_f != WeatherData.condition.feelslike_c))
            {
                var value = isFahrenheit ? Math.Round(WeatherData.condition.feelslike_f.Value) : Math.Round(WeatherData.condition.feelslike_c.Value);

                WeatherDetailsMap.Add(WeatherDetailsType.FeelsLike, new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                       String.Format(culture, "{0}°", value)));
            }

            // Wind
            if (WeatherData.condition.wind_mph.HasValue && WeatherData.condition.wind_mph >= 0 &&
                WeatherData.condition.wind_degrees.HasValue && WeatherData.condition.wind_degrees >= 0)
            {
                string unit = SettingsManager.SpeedUnit;
                int speedVal;
                string speedUnit;

                switch (unit)
                {
                    case Units.MILES_PER_HOUR:
                    default:
                        speedVal = (int)Math.Round(WeatherData.condition.wind_mph.Value);
                        speedUnit = ResUnits.unit_mph;
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(WeatherData.condition.wind_kph.Value);
                        speedUnit = ResUnits.unit_kph;
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(WeatherData.condition.wind_kph.Value));
                        speedUnit = ResUnits.unit_msec;
                        break;
                }

                WeatherDetailsMap.Add(WeatherDetailsType.WindSpeed, new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                   String.Format(culture, "{0} {1}, {2}", speedVal, speedUnit, WeatherUtils.GetWindDirection(WeatherData.condition.wind_degrees.Value)),
                   WeatherData.condition.wind_degrees.Value + 180));
            }

            if (WeatherData.condition.windgust_mph.HasValue && (WeatherData.condition.windgust_mph != WeatherData.condition.windgust_kph))
            {
                string unit = SettingsManager.SpeedUnit;
                int speedVal;
                string speedUnit;

                switch (unit)
                {
                    case Units.MILES_PER_HOUR:
                    default:
                        speedVal = (int)Math.Round(WeatherData.condition.windgust_mph.Value);
                        speedUnit = ResUnits.unit_mph;
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(WeatherData.condition.windgust_kph.Value);
                        speedUnit = ResUnits.unit_kph;
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(WeatherData.condition.windgust_kph.Value));
                        speedUnit = ResUnits.unit_msec;
                        break;
                }

                WeatherDetailsMap.Add(WeatherDetailsType.WindGust, new DetailItemViewModel(WeatherDetailsType.WindGust,
                    String.Format(culture, "{0} {1}", speedVal, speedUnit)));
            }

            Beaufort = WeatherData.condition.beaufort != null ? new BeaufortViewModel(WeatherData.condition.beaufort) : null;

            // Astronomy
            if (WeatherData.astronomy != null)
            {
                SunPhase = new SunPhaseViewModel(WeatherData.astronomy, WeatherData.location.tz_offset);

                if (WeatherData.astronomy.sunrise > DateTime.MinValue && WeatherData.astronomy.sunset > DateTime.MinValue)
                {
                    WeatherDetailsMap.Add(WeatherDetailsType.Sunrise, new DetailItemViewModel(WeatherDetailsType.Sunrise,
                           WeatherData.astronomy.sunrise.ToString("t", culture)));
                    WeatherDetailsMap.Add(WeatherDetailsType.Sunset, new DetailItemViewModel(WeatherDetailsType.Sunset,
                           WeatherData.astronomy.sunset.ToString("t", culture)));
                }

                MoonPhase = new MoonPhaseViewModel(WeatherData.astronomy);

                if (WeatherData.astronomy.moonrise != null && WeatherData.astronomy.moonset != null)
                {
                    if (WeatherData.astronomy.moonrise > DateTime.MinValue)
                    {
                        WeatherDetailsMap.Add(WeatherDetailsType.Moonrise, new DetailItemViewModel(WeatherDetailsType.Moonrise,
                           WeatherData.astronomy.moonrise.ToString("t", culture)));
                    }

                    if (WeatherData.astronomy.moonset > DateTime.MinValue)
                    {
                        WeatherDetailsMap.Add(WeatherDetailsType.Moonset, new DetailItemViewModel(WeatherDetailsType.Moonset,
                           WeatherData.astronomy.moonset.ToString("t", culture)));
                    }
                }

                if (WeatherData.astronomy.moonphase != null)
                {
                    WeatherDetailsMap.Add(WeatherDetailsType.MoonPhase, new DetailItemViewModel(WeatherData.astronomy.moonphase.phase));
                }
            }
            else
            {
                SunPhase = null;
                MoonPhase = null;
            }

            WeatherDetailsMap.NotifyCollectionChanged();

            // Additional Details
            AirQuality = WeatherData.condition.airQuality?.index != null ? new AirQualityViewModel(WeatherData.condition.airQuality) : null;
            Pollen = WeatherData.condition.pollen != null ? new PollenViewModel(WeatherData.condition.pollen) : null;

            string creditPrefix = ResStrings.credit_prefix;
            WeatherCredit = String.Format("{0} {1}",
                creditPrefix, WeatherAPI.APIs.FirstOrDefault(WApi => Equals(WeatherSource, WApi.Value))?.ToString() ?? WeatherIcons.EM_DASH);
        }

        public override bool Equals(object obj)
        {
            return obj is WeatherUiModel model &&
                   Location == model.Location &&
                   UpdateDate == model.UpdateDate &&
                   CurTemp == model.CurTemp &&
                   CurCondition == model.CurCondition &&
                   WeatherIcon == model.WeatherIcon &&
                   HiTemp == model.HiTemp &&
                   LoTemp == model.LoTemp &&
                   ShowHiLo == model.ShowHiLo &&
                   WeatherSummary == model.WeatherSummary &&
                   EqualityComparer<SunPhaseViewModel>.Default.Equals(SunPhase, model.SunPhase) &&
                   EqualityComparer<UVIndexViewModel>.Default.Equals(UVIndex, model.UVIndex) &&
                   EqualityComparer<BeaufortViewModel>.Default.Equals(Beaufort, model.Beaufort) &&
                   EqualityComparer<MoonPhaseViewModel>.Default.Equals(MoonPhase, model.MoonPhase) &&
                   EqualityComparer<AirQualityViewModel>.Default.Equals(AirQuality, model.AirQuality) &&
                   EqualityComparer<PollenViewModel>.Default.Equals(Pollen, model.Pollen) &&
                   EqualityComparer<WeatherUtils.Coordinate>.Default.Equals(LocationCoord, model.LocationCoord) &&
                   WeatherCredit == model.WeatherCredit &&
                   WeatherSource == model.WeatherSource &&
                   WeatherLocale == model.WeatherLocale &&
                   EqualityComparer<DetailsMap<WeatherDetailsType, DetailItemViewModel>>.Default.Equals(WeatherDetailsMap, model.WeatherDetailsMap) &&
                   unitCode == model.unitCode &&
                   //localeCode == model.localeCode &&
                   IconProvider == model.IconProvider;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Location);
            hash.Add(UpdateDate);
            hash.Add(CurTemp);
            hash.Add(CurCondition);
            hash.Add(WeatherIcon);
            hash.Add(HiTemp);
            hash.Add(LoTemp);
            hash.Add(ShowHiLo);
            hash.Add(WeatherSummary);
            hash.Add(SunPhase);
            hash.Add(UVIndex);
            hash.Add(Beaufort);
            hash.Add(MoonPhase);
            hash.Add(AirQuality);
            hash.Add(Pollen);
            hash.Add(LocationCoord);
            hash.Add(WeatherCredit);
            hash.Add(WeatherSource);
            hash.Add(WeatherLocale);
            hash.Add(WeatherDetailsMap);
            hash.Add(unitCode);
            //hash.Add(localeCode);
            hash.Add(IconProvider);
            return hash.ToHashCode();
        }

        public bool IsValid => WeatherData?.IsValid() == true;
    }
}