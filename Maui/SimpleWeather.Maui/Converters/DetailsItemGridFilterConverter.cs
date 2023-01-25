using CommunityToolkit.Maui.Converters;
using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using System.Globalization;
using FeatureSettings = SimpleWeather.Maui.Utils.FeatureSettings;

namespace SimpleWeather.Maui.Converters
{
    public class DetailsItemGridFilterConverter : BaseConverterOneWay<IEnumerable<DetailItemViewModel>, IEnumerable<DetailItemViewModel>>
    {
        public override IEnumerable<DetailItemViewModel> DefaultConvertReturnValue { get; set; }

        public override IEnumerable<DetailItemViewModel> ConvertFrom(IEnumerable<DetailItemViewModel> value, CultureInfo culture)
        {
            return value is IEnumerable<DetailItemViewModel> detailItems
                ? detailItems.WhereNot(it => ((it.DetailsType == WeatherDetailsType.Sunrise || it.DetailsType == WeatherDetailsType.Sunset) && FeatureSettings.SunPhase) ||
                        ((it.DetailsType == WeatherDetailsType.Moonrise || it.DetailsType == WeatherDetailsType.Moonset || it.DetailsType == WeatherDetailsType.MoonPhase) && FeatureSettings.MoonPhase))
                : value;
        }
    }
}
