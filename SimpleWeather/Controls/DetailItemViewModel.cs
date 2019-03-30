using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS_UWP
using SimpleWeather.UWP;
using SimpleWeather.WeatherData;
#endif

namespace SimpleWeather.Controls
{
    public enum WeatherDetailsType
    {
        Sunrise,
        Sunset,
        FeelsLike,
        WindSpeed,
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
        UV
    }

    public class DetailItemViewModel
    {
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
            switch (detailsType)
            {
                case WeatherDetailsType.Sunrise:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Sunrise/Text");
#endif
                    this.Icon = WeatherIcons.SUNRISE;
                    break;
                case WeatherDetailsType.Sunset:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Sunset/Text");
#endif
                    this.Icon = WeatherIcons.SUNSET;
                    break;
                case WeatherDetailsType.FeelsLike:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_FeelsLike/Text");
#endif
                    this.Icon = WeatherIcons.THERMOMETER;
                    break;
                case WeatherDetailsType.WindSpeed:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Wind/Text");
#endif
                    this.Icon = WeatherIcons.WIND_DIRECTION;
                    break;
                case WeatherDetailsType.Humidity:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Humidity/Text");
#endif
                    this.Icon = WeatherIcons.HUMIDITY;
                    break;
                case WeatherDetailsType.Pressure:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Pressure/Text");
#endif
                    this.Icon = WeatherIcons.BAROMETER;
                    break;
                case WeatherDetailsType.Visibility:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Visibility/Text");
#endif
                    this.Icon = WeatherIcons.FOG;
                    break;
                case WeatherDetailsType.PoPCloudiness:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Cloudiness/Text");
#endif
                    this.Icon = WeatherIcons.CLOUDY;
                    break;
                case WeatherDetailsType.PoPChance:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Chance/Text");
#endif
                    this.Icon = WeatherIcons.RAINDROP;
                    break;
                case WeatherDetailsType.PoPRain:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Rain/Text");
#endif
                    this.Icon = WeatherIcons.RAINDROPS;
                    break;
                case WeatherDetailsType.PoPSnow:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Label_Snow/Text");
#endif
                    this.Icon = WeatherIcons.SNOWFLAKE_COLD;
                    break;
                case WeatherDetailsType.Dewpoint:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Dewpoint_Label");
#endif
                    this.Icon = WeatherIcons.THERMOMETER;
                    break;
                case WeatherDetailsType.Moonrise:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Moonrise_Label");
#endif
                    this.Icon = WeatherIcons.MOONRISE;
                    break;
                case WeatherDetailsType.Moonset:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Moonset_Label");
#endif
                    this.Icon = WeatherIcons.MOONSET;
                    break;
                case WeatherDetailsType.MoonPhase:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("MoonPhase_Label");
#endif
                    this.Icon = WeatherIcons.MOON_ALT_NEW;
                    break;
                case WeatherDetailsType.Beaufort:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("Beaufort_Label");
#endif
                    this.Icon = WeatherIcons.WIND_BEAUFORT_0;
                    break;
                case WeatherDetailsType.UV:
#if WINDOWS_UWP
                    this.Label = App.ResLoader.GetString("UV_Label");
#endif
                    this.Icon = WeatherIcons.DAY_SUNNY;
                    break;
            }

            this.Value = value;
            this.IconRotation = iconRotation;
        }

        public DetailItemViewModel(MoonPhase.MoonPhaseType moonPhaseType, String description)
        {
#if WINDOWS_UWP
            this.Label = App.ResLoader.GetString("MoonPhase_Label");
#endif
            this.Value = description;
            this.IconRotation = 0;

            switch (moonPhaseType)
            {
                case MoonPhase.MoonPhaseType.NewMoon:
                    this.Icon = WeatherIcons.MOON_ALT_NEW;
                    break;
                case MoonPhase.MoonPhaseType.WaxingCrescent:
                    this.Icon = WeatherIcons.MOON_ALT_WAXING_CRESCENT_3;
                    break;
                case MoonPhase.MoonPhaseType.FirstQtr:
                    this.Icon = WeatherIcons.MOON_ALT_FIRST_QUARTER;
                    break;
                case MoonPhase.MoonPhaseType.WaxingGibbous:
                    this.Icon = WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3;
                    break;
                case MoonPhase.MoonPhaseType.FullMoon:
                    this.Icon = WeatherIcons.MOON_ALT_FULL;
                    break;
                case MoonPhase.MoonPhaseType.WaningGibbous:
                    this.Icon = WeatherIcons.MOON_ALT_WANING_GIBBOUS_3;
                    break;
                case MoonPhase.MoonPhaseType.LastQtr:
                    this.Icon = WeatherIcons.MOON_ALT_THIRD_QUARTER;
                    break;
                case MoonPhase.MoonPhaseType.WaningCrescent:
                    this.Icon = WeatherIcons.MOON_ALT_WANING_CRESCENT_3;
                    break;
            }
        }

        public DetailItemViewModel(Beaufort.BeaufortScale beaufortScale, String description)
        {
#if WINDOWS_UWP
            this.Label = App.ResLoader.GetString("Beaufort_Label");
#endif
            this.Value = description;
            this.IconRotation = 0;

            switch (beaufortScale)
            {
                case Beaufort.BeaufortScale.B0:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_0;
                    break;
                case Beaufort.BeaufortScale.B1:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_1;
                    break;
                case Beaufort.BeaufortScale.B2:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_2;
                    break;
                case Beaufort.BeaufortScale.B3:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_3;
                    break;
                case Beaufort.BeaufortScale.B4:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_4;
                    break;
                case Beaufort.BeaufortScale.B5:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_5;
                    break;
                case Beaufort.BeaufortScale.B6:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_6;
                    break;
                case Beaufort.BeaufortScale.B7:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_7;
                    break;
                case Beaufort.BeaufortScale.B8:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_8;
                    break;
                case Beaufort.BeaufortScale.B9:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_9;
                    break;
                case Beaufort.BeaufortScale.B10:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_10;
                    break;
                case Beaufort.BeaufortScale.B11:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_11;
                    break;
                case Beaufort.BeaufortScale.B12:
                    this.Icon = WeatherIcons.WIND_BEAUFORT_12;
                    break;
            }
        }
    }
}
