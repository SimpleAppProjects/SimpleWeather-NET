using CommunityToolkit.Maui.Converters;
using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class AQIIndexToColorConverter : BaseConverterOneWay<int?, Color>
    {
        public override Color DefaultConvertReturnValue { get; set; } = Colors.LimeGreen;

        public override Color ConvertFrom(int? value, CultureInfo culture)
        {
            return AirQualityUtils.GetColorFromIndex(value);
        }
    }

    public class AQIIndexToBrushConverter : BaseConverterOneWay<int?, SolidColorBrush>
    {
        public override SolidColorBrush DefaultConvertReturnValue { get; set; } = new SolidColorBrush(Colors.LimeGreen);

        public override SolidColorBrush ConvertFrom(int? value, CultureInfo culture)
        {
            return AirQualityUtils.GetBrushFromIndex(value);
        }
    }
}