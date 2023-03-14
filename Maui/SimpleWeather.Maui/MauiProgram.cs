using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using epj.ProgressBar.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Embedding;
using Microsoft.Maui.LifecycleEvents;
using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell;
using SimpleWeather.Maui.MaterialIcons;
using SimpleWeather.Maui.Preferences;
using SimpleWeather.Weather_API.Keys;
using SkiaSharp.Views.Maui.Controls.Hosting;

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
}
