using SimpleWeather.Icons;
using SimpleWeather.Resources.Strings;
using SimpleWeather.WeatherData;
using System;
using static SimpleWeather.WeatherData.Beaufort;
using ResAQIndex = SimpleWeather.Resources.Strings.AQIndex;
using ResBeaufort = SimpleWeather.Resources.Strings.Beaufort;
using ResMoonPhases = SimpleWeather.Resources.Strings.MoonPhases;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Common.Controls
{
    public enum WeatherDetailsType
    {
        Sunrise,
        Sunset,
        FeelsLike,
        WindSpeed,
        WindGust,
        Humidity,
        Pressure,
        Visibility,
        PoPCloudiness,
        PoPChance,
        PoPRain,
        PoPSnow,
        Dewpoint,
        Moonrise,
        Moonset,
        MoonPhase,
        Beaufort,
        UV,
        AirQuality
    }

    public class DetailItemViewModel
    {
        public WeatherDetailsType DetailsType { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public string Value { get; set; }
        public int IconRotation { get; set; }

        public DetailItemViewModel(WeatherDetailsType detailsType, String value)
            : this(detailsType, value, 0)
        {
        }

        public DetailItemViewModel(WeatherDetailsType detailsType, String value, int iconRotation)
        {
            this.DetailsType = detailsType;

            switch (detailsType)
            {
                case WeatherDetailsType.Sunrise:
                    this.Label = ResStrings.label_sunrise;
                    this.Icon = WeatherIcons.SUNRISE;
                    break;

                case WeatherDetailsType.Sunset:
                    this.Label = ResStrings.label_sunset;
                    this.Icon = WeatherIcons.SUNSET;
                    break;

                case WeatherDetailsType.FeelsLike:
                    this.Label = ResStrings.label_feelslike;
                    this.Icon = WeatherIcons.THERMOMETER;
                    break;

                case WeatherDetailsType.WindSpeed:
                    this.Label = ResStrings.label_wind;
                    this.Icon = WeatherIcons.WIND_DIRECTION;
                    break;

                case WeatherDetailsType.WindGust:
                    this.Label = ResStrings.label_windgust;
                    this.Icon = WeatherIcons.CLOUDY_GUSTS;
                    break;

                case WeatherDetailsType.Humidity:
                    this.Label = ResStrings.label_humidity;
                    this.Icon = WeatherIcons.HUMIDITY;
                    break;

                case WeatherDetailsType.Pressure:
                    this.Label = ResStrings.label_pressure;
                    this.Icon = WeatherIcons.BAROMETER;
                    break;

                case WeatherDetailsType.Visibility:
                    this.Label = ResStrings.label_visibility;
                    this.Icon = WeatherIcons.FOG;
                    break;

                case WeatherDetailsType.PoPCloudiness:
                    this.Label = ResStrings.label_cloudiness;
                    this.Icon = WeatherIcons.CLOUDY;
                    break;

                case WeatherDetailsType.PoPChance:
                    this.Label = ResStrings.label_chance;
                    this.Icon = WeatherIcons.UMBRELLA;
                    break;

                case WeatherDetailsType.PoPRain:
                    this.Label = ResStrings.label_qpf_rain;
                    this.Icon = WeatherIcons.RAINDROPS;
                    break;

                case WeatherDetailsType.PoPSnow:
                    this.Label = ResStrings.label_qpf_snow;
                    this.Icon = WeatherIcons.SNOWFLAKE_COLD;
                    break;

                case WeatherDetailsType.Dewpoint:
                    this.Label = ResStrings.label_dewpoint;
                    this.Icon = WeatherIcons.THERMOMETER;
                    break;

                case WeatherDetailsType.Moonrise:
                    this.Label = ResStrings.label_moonrise;
                    this.Icon = WeatherIcons.MOONRISE;
                    break;

                case WeatherDetailsType.Moonset:
                    this.Label = ResStrings.label_moonset;
                    this.Icon = WeatherIcons.MOONSET;
                    break;

                case WeatherDetailsType.MoonPhase:
                    this.Label = ResStrings.label_moonphase;
                    this.Icon = WeatherIcons.MOON_ALT_NEW;
                    break;

                case WeatherDetailsType.Beaufort:
                    this.Label = ResStrings.label_beaufort;
                    this.Icon = WeatherIcons.WIND_BEAUFORT_0;
                    break;

                case WeatherDetailsType.UV:
                    this.Label = ResStrings.label_uv;
                    this.Icon = WeatherIcons.DAY_SUNNY;
                    break;
            }

            this.Value = value;
            this.IconRotation = iconRotation;
        }

        public DetailItemViewModel(MoonPhase.MoonPhaseType moonPhaseType)
        {
            this.DetailsType = WeatherDetailsType.MoonPhase;
            this.Label = ResStrings.label_moonphase;
            this.IconRotation = 0;

            switch (moonPhaseType)
            {
                case MoonPhase.MoonPhaseType.NewMoon:
                    this.Icon = WeatherIcons.MOON_ALT_NEW;
                    this.Value = ResMoonPhases.moonphase_new;
                    break;

                case MoonPhase.MoonPhaseType.WaxingCrescent:
                    this.Icon = WeatherIcons.MOON_ALT_WAXING_CRESCENT_3;
                    this.Value = ResMoonPhases.moonphase_waxcrescent;
                    break;

                case MoonPhase.MoonPhaseType.FirstQtr:
                    this.Icon = WeatherIcons.MOON_ALT_FIRST_QUARTER;
                    this.Value = ResMoonPhases.moonphase_firstqtr;
                    break;

                case MoonPhase.MoonPhaseType.WaxingGibbous:
                    this.Icon = WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3;
                    this.Value = ResMoonPhases.moonphase_waxgibbous;
                    break;

                case MoonPhase.MoonPhaseType.FullMoon:
                    this.Icon = WeatherIcons.MOON_ALT_FULL;
                    this.Value = ResMoonPhases.moonphase_full;
                    break;

                case MoonPhase.MoonPhaseType.WaningGibbous:
                    this.Icon = WeatherIcons.MOON_ALT_WANING_GIBBOUS_3;
                    this.Value = ResMoonPhases.moonphase_wangibbous;
                    break;

                case MoonPhase.MoonPhaseType.LastQtr:
                    this.Icon = WeatherIcons.MOON_ALT_THIRD_QUARTER;
                    this.Value = ResMoonPhases.moonphase_lastqtr;
                    break;

                case MoonPhase.MoonPhaseType.WaningCrescent:
                    this.Icon = WeatherIcons.MOON_ALT_WANING_CRESCENT_3;
                    this.Value = ResMoonPhases.moonphase_wancrescent;
                    break;
            }
        }

        public DetailItemViewModel(BeaufortScale beaufortScale)
        {
            this.DetailsType = WeatherDetailsType.Beaufort;
            this.Label = ResStrings.label_beaufort;
            this.IconRotation = 0;

            switch (beaufortScale)
            {
                case BeaufortScale.B0:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_0;
                    this.Value = ResBeaufort.beaufort_0;
                    break;

                case BeaufortScale.B1:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_1;
                    this.Value = ResBeaufort.beaufort_1;
                    break;

                case BeaufortScale.B2:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_2;
                    this.Value = ResBeaufort.beaufort_2;
                    break;

                case BeaufortScale.B3:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_3;
                    this.Value = ResBeaufort.beaufort_3;
                    break;

                case BeaufortScale.B4:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_4;
                    this.Value = ResBeaufort.beaufort_4;
                    break;

                case BeaufortScale.B5:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_5;
                    this.Value = ResBeaufort.beaufort_5;
                    break;

                case BeaufortScale.B6:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_6;
                    this.Value = ResBeaufort.beaufort_6;
                    break;

                case BeaufortScale.B7:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_7;
                    this.Value = ResBeaufort.beaufort_7;
                    break;

                case BeaufortScale.B8:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_8;
                    this.Value = ResBeaufort.beaufort_8;
                    break;

                case BeaufortScale.B9:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_9;
                    this.Value = ResBeaufort.beaufort_9;
                    break;

                case BeaufortScale.B10:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_10;
                    this.Value = ResBeaufort.beaufort_10;
                    break;

                case BeaufortScale.B11:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_11;
                    this.Value = ResBeaufort.beaufort_11;
                    break;

                case BeaufortScale.B12:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_12;
                    this.Value = ResBeaufort.beaufort_12;
                    break;
            }
        }

        public DetailItemViewModel(UV uv)
        {
            this.DetailsType = WeatherDetailsType.UV;
            this.Label = ResStrings.label_uv;
            this.IconRotation = 0;

            if (uv.index < 3)
            {
                this.Value = UVIndex.uv_0;
            }
            else if (uv.index < 6)
            {
                this.Value = UVIndex.uv_3;
            }
            else if (uv.index < 8)
            {
                this.Value = UVIndex.uv_6;
            }
            else if (uv.index < 11)
            {
                this.Value = UVIndex.uv_8;
            }
            else if (uv.index >= 11)
            {
                this.Value = UVIndex.uv_11;
            }

            if (uv.index.HasValue)
            {
                switch ((int)uv.index.Value)
                {
                    case 1:
                        Icon = WeatherIcons.UV_INDEX_1;
                        break;
                    case 2:
                        Icon = WeatherIcons.UV_INDEX_2;
                        break;
                    case 3:
                        Icon = WeatherIcons.UV_INDEX_3;
                        break;
                    case 4:
                        Icon = WeatherIcons.UV_INDEX_4;
                        break;
                    case 5:
                        Icon = WeatherIcons.UV_INDEX_5;
                        break;
                    case 6:
                        Icon = WeatherIcons.UV_INDEX_6;
                        break;
                    case 7:
                        Icon = WeatherIcons.UV_INDEX_7;
                        break;
                    case 8:
                        Icon = WeatherIcons.UV_INDEX_8;
                        break;
                    case 9:
                        Icon = WeatherIcons.UV_INDEX_9;
                        break;
                    case 10:
                        Icon = WeatherIcons.UV_INDEX_10;
                        break;
                    case 11:
                        Icon = WeatherIcons.UV_INDEX_11;
                        break;
                    default:
                        Icon = WeatherIcons.UV_INDEX;
                        break;
                }
            }
        }

        public DetailItemViewModel(AirQuality aqi)
        {
            this.DetailsType = WeatherDetailsType.AirQuality;
            this.Label = ResStrings.label_airquality;
            this.Icon = WeatherIcons.CLOUDY_GUSTS;
            this.IconRotation = 0;

            if (aqi.index < 51)
            {
                this.Value = ResAQIndex.aqi_level_0_50;
            }
            else if (aqi.index < 101)
            {
                this.Value = ResAQIndex.aqi_level_51_100;
            }
            else if (aqi.index < 151)
            {
                this.Value = ResAQIndex.aqi_level_101_150;
            }
            else if (aqi.index < 201)
            {
                this.Value = ResAQIndex.aqi_level_151_200;
            }
            else if (aqi.index < 301)
            {
                this.Value = ResAQIndex.aqi_level_201_300;
            }
            else if (aqi.index >= 301)
            {
                this.Value = ResAQIndex.aqi_level_300;
            }
        }
    }
}