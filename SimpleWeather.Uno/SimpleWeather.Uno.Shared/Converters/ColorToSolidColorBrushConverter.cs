using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Converters
{
    public partial class ColorToSolidColorBrushConverter : DependencyObject, IValueConverter
    {
        public bool IsInverse
        {
            get => (bool)GetValue(IsInverseProperty);
            set => SetValue(IsInverseProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsInverse.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInverseProperty =
            DependencyProperty.Register("IsInverse", typeof(bool), typeof(ColorToSolidColorBrushConverter), new PropertyMetadata(false));

        public object FallbackColor
        {
            get => GetValue(FallbackColorProperty);
            set => SetValue(FallbackColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for FallbackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FallbackColorProperty =
            DependencyProperty.Register("FallbackColor", typeof(object), typeof(ColorToSolidColorBrushConverter), new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!IsInverse)
            {
                return value is Color color ? new SolidColorBrush(color) : FallbackColor;
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
