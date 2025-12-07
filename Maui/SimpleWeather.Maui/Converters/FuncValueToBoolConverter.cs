using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace SimpleWeather.Maui.Converters;

public class FuncValueToBoolConverter : ICommunityToolkitMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values[0] switch
        {
            Func<object, bool> func => func(values[1]),
            Func<object, object, bool> func2 => func2(values[1], values[2]),
            _ => false
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
    }
}