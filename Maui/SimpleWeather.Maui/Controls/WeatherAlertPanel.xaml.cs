using CommunityToolkit.Maui.Converters;
using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Globalization;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

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
        this.BindingContextChanged += (s, e) =>
        {
            if (AlertFmtdMessage != null)
            {
                AlertFmtdMessage.Spans.Clear();

                /*
                    Text="{x:Bind $'{ExpireDate}\n\n{Message}\n\n{Attribution}'}"
                 */
                AlertFmtdMessage.Spans.Add(new Span()
                {
                    Text = WeatherAlert?.ExpireDate
                });
                AlertFmtdMessage.Spans.Add(new LineBreakSpan());
                AlertFmtdMessage.Spans.Add(new LineBreakSpan());

                // Message
                if (WeatherAlert?.Message?.StartsWith("https://weatherkit.apple.com") == true)
                {
                    try
                    {
                        AlertFmtdMessage.Spans.Add(new HyperlinkSpan()
                        {
                            NavigateUri = WeatherAlert?.Message,
                            Text = ResStrings.label_moreinfo
                        });
                    }
                    catch
                    {
                        AlertFmtdMessage.Spans.Add(new Span()
                        {
                            Text = WeatherAlert?.Message
                        });
                    }
                }
                else
                {
                    AlertFmtdMessage.Spans.Add(new Span()
                    {
                        Text = WeatherAlert?.Message
                    });
                }

                AlertFmtdMessage.Spans.Add(new LineBreakSpan());
                AlertFmtdMessage.Spans.Add(new LineBreakSpan());
                AlertFmtdMessage.Spans.Add(new Span()
                {
                    Text = WeatherAlert?.Attribution
                });
            }
        };
    }
}

[AcceptEmptyServiceProvider]
internal class AlertSeverityColorConverter : BaseConverterOneWay<WeatherAlertSeverity, Color>
{
    public override Color DefaultConvertReturnValue { get; set; } = default;

    public override Color ConvertFrom(WeatherAlertSeverity value, CultureInfo culture)
    {
        return WeatherUtils.GetColorFromAlertSeverity(value);
    }
}

[AcceptEmptyServiceProvider]
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