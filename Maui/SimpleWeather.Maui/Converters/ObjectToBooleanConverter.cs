using System;
using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace SimpleWeather.Maui.Converters
{
    public class ObjectToBooleanConverter : BaseConverterOneWay<object, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; } = false;

        public override bool ConvertFrom(object value, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value?.ToString());
        }
    }
}

