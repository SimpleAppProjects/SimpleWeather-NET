using CommunityToolkit.Maui.Converters;
using SimpleWeather.Utils;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class AlphaColorConverter : BaseConverterOneWay<Color, Color>
    {
        public double Opacity { get; set; } = 1.0;

        public override Color DefaultConvertReturnValue { get; set; } = default;

        public override Color ConvertFrom(Color value, CultureInfo culture)
        {
            return ColorUtils.SetAlphaComponent(value, (byte)(Opacity * 0xff));
        }
    }
}
