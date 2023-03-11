using System;
using System.Collections;
using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace SimpleWeather.Maui.Converters
{
    public class ObjectToCollectionConverter : BaseConverter<object, IEnumerable>
    {
        public override IEnumerable DefaultConvertReturnValue { get; set; } = default;
        public override object DefaultConvertBackReturnValue { get; set; } = default;

        public override object ConvertBackTo(IEnumerable value, CultureInfo culture)
        {
            return value;
        }

        public override IEnumerable ConvertFrom(object value, CultureInfo culture)
        {
            return value as IEnumerable;
        }
    }
}

