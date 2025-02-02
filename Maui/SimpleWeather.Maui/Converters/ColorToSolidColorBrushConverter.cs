using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class ColorToSolidColorBrushConverter : BaseConverterTwoWay<Color, SolidColorBrush>
    {
        public override SolidColorBrush DefaultConvertReturnValue { get; set; }
        public override Color DefaultConvertBackReturnValue { get; set; }

        public override Color ConvertBackTo(SolidColorBrush value, CultureInfo culture)
        {
            return value?.Color ?? DefaultConvertBackReturnValue;
        }

        public override SolidColorBrush ConvertFrom(Color value, CultureInfo culture)
        {
            return new SolidColorBrush(value);
        }
    }
}