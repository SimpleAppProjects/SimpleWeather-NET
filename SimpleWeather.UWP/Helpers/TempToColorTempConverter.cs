using SimpleWeather.Utils;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Helpers
{
    public class TempToColorTempConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string temp = value?.ToString();
            string temp_str = temp?.RemoveNonDigitChars();

            if (float.TryParse(temp_str, out float temp_f))
            {
                var tempUnit = Settings.TemperatureUnit;

                if (Equals(tempUnit, Units.CELSIUS) || temp.EndsWith(Units.CELSIUS))
                {
                    temp_f = ConversionMethods.CtoF(temp_f);
                }

                Color defaultColor = Colors.White;
                if (parameter is Color paramColor)
                {
                    defaultColor = paramColor;
                }

                return WeatherUtils.GetColorFromTempF(temp_f, defaultColor);
            }
            else
            {
                if (parameter is Color defaultColor)
                {
                    return defaultColor;
                }
                else
                {
                    return Colors.White;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
