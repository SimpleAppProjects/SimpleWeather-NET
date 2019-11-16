using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SimpleWeather.UWP.Helpers
{
    public class BooleanToGridLengthConverter : IValueConverter
    {
        public bool IsInverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool visibility = (bool)value;
            if (IsInverse)
            {
                visibility = !visibility;
            }

            return visibility ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}