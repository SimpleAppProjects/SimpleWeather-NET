using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class SolidColorBrushToColorConverter : BaseConverter<SolidColorBrush, Color>
    {
        public override Color DefaultConvertReturnValue { get; set; }
        public override SolidColorBrush DefaultConvertBackReturnValue { get; set; }

        public override SolidColorBrush ConvertBackTo(Color value, CultureInfo culture)
        {
            return new SolidColorBrush(value);
        }

        public override Color ConvertFrom(SolidColorBrush value, CultureInfo culture)
        {
            return value?.Color ?? DefaultConvertReturnValue;
        }
    }
}
