using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class BooleanToGridLengthConverter : BaseConverterOneWay<bool, GridLength>
    {
        public bool IsInverse { get; set; }

        public override GridLength DefaultConvertReturnValue { get; set; }

        public override GridLength ConvertFrom(bool value, CultureInfo culture)
        {
            var visibility = value;
            if (IsInverse)
            {
                visibility = !visibility;
            }

            return visibility ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        }
    }
}
