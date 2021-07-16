using SimpleWeather.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Helpers
{
    public class IconThemeConverter : DependencyObject, IValueConverter
    {
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConverterParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(IconThemeConverter), new PropertyMetadata(null));

        public bool ForceDarkTheme
        {
            get { return (bool)GetValue(ForceDarkThemeProperty); }
            set { SetValue(ForceDarkThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceDarkTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceDarkThemeProperty =
            DependencyProperty.Register("ForceDarkTheme", typeof(bool), typeof(IconThemeConverter), new PropertyMetadata(false));

        public object Convert(object value, Type targetType, object p_, string l_)
        {
            var wim = WeatherIconsManager.GetInstance();

            var icon = value?.ToString() ?? string.Empty;

            if (ForceDarkTheme)
            {
                return wim.GetWeatherIconURI(icon);
            }
            else if (ConverterParameter is ObjectContainer paramObj && paramObj.Value is bool)
            {
                return new Uri(wim.GetWeatherIconURI(icon, true, (bool)paramObj.Value));
            }
            else if (ConverterParameter is bool isLight)
            {
                return new Uri(wim.GetWeatherIconURI(icon, true, isLight));
            }
            else if (ConverterParameter is SolidColorBrush paramColorBrush)
            {
                return new Uri(wim.GetWeatherIconURI(icon, true, paramColorBrush.Color == Colors.Black));
            }
            else if (ConverterParameter is Color paramColor)
            {
                return new Uri(wim.GetWeatherIconURI(icon, true, paramColor == Colors.Black));
            }
            else
            {
                return new Uri(wim.GetWeatherIconURI(icon, true, false));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
