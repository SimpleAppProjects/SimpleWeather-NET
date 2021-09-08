using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Helpers
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public bool IsInverse { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!IsInverse)
            {
                Color color = value is Color ? (Color)value : new Color();
                return new SolidColorBrush(color);
            }
            else
            {
                SolidColorBrush brush = (SolidColorBrush)value;
                return brush.Color;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
