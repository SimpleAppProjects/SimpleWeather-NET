using System.Collections;
using System.Collections.Immutable;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    [AcceptEmptyServiceProvider]
    public class ObjectToCollectionConverter : BaseConverterTwoWay<object?, IEnumerable?>
    {
        public override IEnumerable? DefaultConvertReturnValue { get; set; } = null;
        public override object? DefaultConvertBackReturnValue { get; set; } = null;

        public override object? ConvertBackTo(IEnumerable? value, CultureInfo? culture)
        {
            return value;
        }

        public override IEnumerable? ConvertFrom(object? value, CultureInfo? culture)
        {
            if (value is not null)
            {
                return value as IEnumerable ?? ImmutableList.Create(value);
            }

            return null;
        }
    }
}