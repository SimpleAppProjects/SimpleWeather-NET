using System.Globalization;
using CommunityToolkit.Maui.Converters;
using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Maui.Controls;

public partial class WeatherAlertPanel : ContentView
{
    public WeatherAlertViewModel WeatherAlert
    {
        get { return (this.BindingContext as WeatherAlertViewModel); }
    }

    public WeatherAlertPanel()
	{
		InitializeComponent();
	}
}

internal class AlertSeverityColorConverter : BaseConverterOneWay<WeatherAlertSeverity, Color>
{
    public override Color DefaultConvertReturnValue { get; set; } = default;

    public override Color ConvertFrom(WeatherAlertSeverity value, CultureInfo culture)
    {
        return WeatherUtils.GetColorFromAlertSeverity(value);
    }
}

internal class AlertTypeIconConverter : BaseConverterOneWay<WeatherAlertType, ImageSource>
{
    public override ImageSource DefaultConvertReturnValue { get; set; } = default;

    public override ImageSource ConvertFrom(WeatherAlertType value, CultureInfo culture)
    {
        var uri = WeatherUtils.GetAssetFromAlertType(value);
        return new FileImageSource()
        {
            File = uri
        };
    }
}