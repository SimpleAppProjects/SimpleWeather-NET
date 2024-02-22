using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using epj.ProgressBar.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Embedding;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell;
using SimpleWeather.Maui.MaterialIcons;
using SimpleWeather.Maui.Preferences;
using SimpleWeather.Weather_API.Keys;
using SkiaSharp.Views.Maui.Controls.Hosting;
using UIKit;

namespace SimpleWeather.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .UseMauiMaps()
            .UseSkiaSharp(true)
            .UseMauiEmbedding<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("OpenSans-Light.ttf", "OpenSansLight");
                fonts.AddFont("MaterialIcons-Regular.ttf", MaterialIcon.FONT_ALIAS);
                fonts.AddFont("weathericons-regular-webfont.ttf", "WeatherIcons");
            })
            .ConfigureEssentials(essentials =>
            {
                essentials
                    .UseVersionTracking()
                    .UseMapServiceToken(APIKeys.GetBingMapsKey());
            })
            .ConfigureLifecycleEvents(life =>
            {

            })
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<SelectableItemsView, SelectableItemsViewHandler<SelectableItemsView>>();
#if __IOS__
                handlers.AddHandler<TransparentViewCell, TransparentViewCellRenderer>();
                handlers.AddHandler<TextCell, NonReusableTextCellRenderer>();
                handlers.AddHandler<ScrollView, CustomScrollViewHandler>();
                LabelHandler.Mapper.AppendToMapping(nameof(Label.LineBreakMode), UpdateMaxLines);
                LabelHandler.Mapper.AppendToMapping(nameof(Label.MaxLines), UpdateMaxLines);
#endif
            })
            .UseProgressBar()
            .UseSimpleShell()
            .UseSimpleToolkit()
            //.ConfigureContainer()
            //.ConfigureGraphicsControls(DrawableType.Fluent)
            ;

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

#if __IOS__
    private static void UpdateMaxLines(ILabelHandler handler, ILabel label)
    {
        if (handler?.PlatformView is not UILabel uiLabel)
            return;

        if (label is Label _label && uiLabel.LineBreakMode == UILineBreakMode.TailTruncation)
        {
            uiLabel.Lines = _label.MaxLines != -1 ? _label.MaxLines : (nint)1;
        }
    }
#endif
}
