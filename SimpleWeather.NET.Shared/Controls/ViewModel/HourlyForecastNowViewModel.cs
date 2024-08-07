﻿using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using System.ComponentModel;
using ResUnits = SimpleWeather.Resources.Strings.Units;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    [Bindable(true)]
    public class HourlyForecastNowViewModel
    {
        public string Date { get; set; }
        public string ShortDate { get; set; }
        public string Icon { get; set; }
        public string Temperature { get; set; }
        public string Condition { get; set; }
        public string PoPChance { get; set; }
        public string WindSpeed { get; set; }
        public int WindDirection { get; set; }

        public HourlyForecastNowViewModel(HourlyForecast forecast)
        {
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();
            var culture = LocaleUtils.GetLocale();
            var isFahrenheit = Units.FAHRENHEIT.Equals(SettingsManager.TemperatureUnit);
            var wm = WeatherModule.Instance.WeatherManager;

            if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
            {
                Date = forecast.date.ToString("ddd HH:00", culture);
                ShortDate = forecast.date.ToString("HH:00", culture);
            }
            else
            {
                Date = forecast.date.ToString("ddd h tt", culture);
                ShortDate = forecast.date.ToString("h tt", culture);
            }

            Icon = forecast.icon;

            try
            {
                if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                {
                    var value = isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value);
                    Temperature = String.Format(culture, "{0}°", value);
                }
                else
                {
                    Temperature = WeatherIcons.PLACEHOLDER;
                }
            }
            catch (FormatException ex)
            {
                Temperature = WeatherIcons.PLACEHOLDER;
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }

            Condition = wm.SupportsWeatherLocale ? forecast.condition : wm.GetWeatherCondition(forecast.icon);

            if (forecast.wind_mph.HasValue && forecast.wind_mph >= 0 &&
                forecast.wind_degrees.HasValue && forecast.wind_degrees >= 0)
            {
                string unit = SettingsManager.SpeedUnit;
                int speedVal;
                string speedUnit;

                switch (unit)
                {
                    case Units.MILES_PER_HOUR:
                    default:
                        speedVal = (int)Math.Round(forecast.wind_mph.Value);
                        speedUnit = ResUnits.unit_mph;
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(forecast.wind_kph.Value);
                        speedUnit = ResUnits.unit_kph;
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(forecast.wind_kph.Value));
                        speedUnit = ResUnits.unit_msec;
                        break;
                    case Units.KNOTS:
                        speedVal = (int)Math.Round(ConversionMethods.MphToKts(forecast.wind_mph.Value));
                        speedUnit = ResUnits.unit_knots;
                        break;
                }

                WindDirection = forecast.wind_degrees.GetValueOrDefault(0) + 180;

                WindSpeed = String.Format(culture, "{0} {1}", speedVal, speedUnit);
            }

            if (forecast.extras?.pop.HasValue == true && forecast.extras?.pop >= 0)
            {
                PoPChance = forecast.extras.pop.Value + "%";
            }
        }

        public override bool Equals(object obj)
        {
            return obj is HourlyForecastNowViewModel model &&
                   Date == model.Date &&
                   ShortDate == model.ShortDate &&
                   Icon == model.Icon &&
                   Temperature == model.Temperature &&
                   Condition == model.Condition &&
                   PoPChance == model.PoPChance &&
                   WindSpeed == model.WindSpeed &&
                   WindDirection == model.WindDirection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Date,
                ShortDate,
                Icon,
                Temperature,
                Condition,
                PoPChance,
                WindSpeed,
                WindDirection
            );
        }
    }
}
