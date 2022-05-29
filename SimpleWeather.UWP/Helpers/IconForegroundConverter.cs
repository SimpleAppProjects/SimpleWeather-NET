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
    public class IconForegroundConverter : DependencyObject, IValueConverter
    {
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConverterParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(IconForegroundConverter), new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object p_, string l_)
        {
            var wim = SharedModule.Instance.WeatherIconsManager;

            if (wim.IsFontIcon)
            {
                if (ConverterParameter is Color paramColor)
                {
                    return new SolidColorBrush(paramColor);
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object p_, string l_)
        {
            throw new NotImplementedException();
        }
    }
}
