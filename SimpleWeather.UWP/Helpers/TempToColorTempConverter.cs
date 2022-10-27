using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Helpers
{
    public class TempToColorTempConverter : DependencyObject, IValueConverter
    {
        public Color FallbackColor
        {
            get { return (Color)GetValue(FallbackColorProperty); }
            set { SetValue(FallbackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FallbackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FallbackColorProperty =
            DependencyProperty.Register("FallbackColor", typeof(Color), typeof(TempToColorTempConverter), new PropertyMetadata(Colors.White));

        public object Convert(object value, Type targetType, object p_, string l_)
        {
            if (value is WeatherUiModel weather)
            {
                string temp = weather.CurTemp?.ToString();
                string temp_str = temp?.RemoveNonDigitChars();

                if (float.TryParse(temp_str, out float temp_f))
                {
                    var tempUnit = weather.TempUnit;

                    if (Equals(tempUnit, Units.CELSIUS) || temp.EndsWith(Units.CELSIUS))
                    {
                        temp_f = ConversionMethods.CtoF(temp_f);
                    }

                    return new SolidColorBrush(WeatherUtils.GetColorFromTempF(temp_f, FallbackColor));
                }
            }

            return new SolidColorBrush(FallbackColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
