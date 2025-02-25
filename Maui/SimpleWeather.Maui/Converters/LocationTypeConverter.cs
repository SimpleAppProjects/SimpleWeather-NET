using CommunityToolkit.Maui.Converters;
using SimpleWeather.LocationData;
using System.Globalization;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Converters
{
    [AcceptEmptyServiceProvider]
    public class LocationTypeConverter : BaseConverterOneWay<LocationType, string?>
    {
        public override string? DefaultConvertReturnValue { get; set; }

        public override string? ConvertFrom(LocationType value, CultureInfo? culture)
        {
            return value switch
            {
                LocationType.GPS => ResStrings.label_currentlocation,
                LocationType.Search => ResStrings.label_favoritelocations,
                _ => null,
            };
        }
    }
}
