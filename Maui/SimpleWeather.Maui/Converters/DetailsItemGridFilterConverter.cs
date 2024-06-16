using CommunityToolkit.Maui.Converters;
using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using FeatureSettings = SimpleWeather.NET.Utils.FeatureSettings;

namespace SimpleWeather.Maui.Converters
{
    public class DetailsItemGridFilterConverter : BaseConverterOneWay<object, IEnumerable>
    {
        public override IEnumerable DefaultConvertReturnValue { get; set; }

        public override IEnumerable ConvertFrom(object value, CultureInfo culture)
        {
            return value is IEnumerable<DetailItemViewModel> detailItems
                ? detailItems.WhereNot(it => ((it.DetailsType == WeatherDetailsType.Sunrise || it.DetailsType == WeatherDetailsType.Sunset) && FeatureSettings.SunPhase) ||
                        ((it.DetailsType == WeatherDetailsType.Moonrise || it.DetailsType == WeatherDetailsType.Moonset || it.DetailsType == WeatherDetailsType.MoonPhase) && FeatureSettings.MoonPhase))
                : value as IEnumerable<DetailItemViewModel>;
        }
    }
}
