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

        public bool UseFallback
        {
            get { return (bool)GetValue(UseFallbackProperty); }
            set { SetValue(UseFallbackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseFallback.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseFallbackProperty =
            DependencyProperty.Register("UseFallback", typeof(bool), typeof(TempToColorTempConverter), new PropertyMetadata(true));

        public object Convert(object value, Type targetType, object p_, string l_)
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

                return WeatherUtils.GetColorFromTempF(temp_f, UseFallback ? FallbackColor : Colors.White);
            }
            else
            {
                return UseFallback ? FallbackColor : Colors.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
