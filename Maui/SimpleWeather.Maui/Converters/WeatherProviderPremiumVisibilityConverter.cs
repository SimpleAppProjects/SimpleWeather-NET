using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Extras;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class WeatherProviderPremiumVisibilityConverter : BaseConverterOneWay<string, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; } = false;

        public override bool ConvertFrom(string value, CultureInfo culture)
        {
            var ExtrasService = Ioc.Default.GetService<IExtrasService>();

            return value != null && ExtrasService.IsPremiumWeatherAPI(value);
        }
    }
}
