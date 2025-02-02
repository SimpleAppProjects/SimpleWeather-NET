using System.Collections;
using System.Collections.Immutable;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class ObjectToCollectionConverter : BaseConverterTwoWay<object, IEnumerable>
    {
        public override IEnumerable DefaultConvertReturnValue { get; set; } = default;
        public override object DefaultConvertBackReturnValue { get; set; } = default;

        public override object ConvertBackTo(IEnumerable value, CultureInfo culture)
        {
            return value;
        }

        public override IEnumerable ConvertFrom(object value, CultureInfo culture)
        {
            if (value is not null)
            {
                return value is IEnumerable enumerable ? enumerable : ImmutableList.Create(value);
            }

            return null;
        }
    }
}