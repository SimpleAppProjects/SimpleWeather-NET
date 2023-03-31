using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using epj.ProgressBar.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
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
                SwitchHandler.Mapper.AppendToMapping(nameof(ISwitch.IsOn), MapSwitchIsOn);
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
    /// <summary>
    /// ISSUE: Toggle button OnColor for Off state not working in iOS
    /// https://github.com/dotnet/maui/issues/10099
    /// </summary>
    public static void MapSwitchIsOn(ISwitchHandler handler, ISwitch view)
    {
        if (view == null)
            return;

        var uiSwitch = handler.PlatformView;
        if (uiSwitch == null)
            return;

        var uIView = OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsTvOSVersionAtLeast(13)
            ? uiSwitch.Subviews?[0]?.Subviews?[0]
            : uiSwitch.Subviews?[0]?.Subviews?[0]?.Subviews?[0];

        if (uIView is null)
            return;

        if (!view.IsOn)
        {
            // iOS 13+ uses the UIColor.SecondarySystemFill to support Light and Dark mode
            // else, use the RGBA equivalent of UIColor.SecondarySystemFill in Light mode
            uIView.BackgroundColor = OperatingSystem.IsIOSVersionAtLeast(13)
                ? UIKit.UIColor.SecondarySystemFill
                : UIKit.UIColor.FromRGBA(120, 120, 128, 40);
        }
        else if (view.TrackColor is not null)
        {
            uiSwitch.OnTintColor = view.TrackColor.ToPlatform();
            uIView.BackgroundColor = uiSwitch.OnTintColor;
        }
    }
#endif
}
