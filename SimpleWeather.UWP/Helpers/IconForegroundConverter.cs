using SimpleWeather.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace SimpleWeather.UWP.Helpers
{
    public class IconForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var wim = WeatherIconsManager.GetInstance();

            if (wim.IsFontIcon)
            {
                if (parameter is Color paramColor)
                {
                    return paramColor;
                }
                else
                {
                    return Colors.White;
                }
            }
            else
            {
                return Colors.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
