using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using epj.ProgressBar.Maui;
using MauiIcons.Cupertino;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Embedding;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using Plugin.Maui.SegmentedControl;
#if __MACCATALYST__
using Sentry.Protocol;
#endif
using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell;
using SimpleWeather.Keys;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.MaterialIcons;
using SimpleWeather.Maui.Preferences;
using SimpleWeather.Weather_API.Keys;
using SkiaSharp.Views.Maui.Controls.Hosting;
using UIKit;
using FirebaseAuth = Firebase.Auth;
using FirebaseDb = Firebase.Database;

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
            .UseSkiaSharp()
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
                handlers.AddHandler<TimePicker, NativeTimePickerHandler>();
                handlers.AddHandler<ReorderableCollectionView, ReorderableCollectionViewHandler<ReorderableCollectionView>>();
                LabelHandler.Mapper.AppendToMapping(nameof(Label.LineBreakMode), UpdateMaxLines);
                LabelHandler.Mapper.AppendToMapping(nameof(Label.MaxLines), UpdateMaxLines);
#endif
            })
#if MACCATALYST
            .UseSentry(options =>
            {
                options.Dsn = SentryConfig.GetDsn();
                options.IsGlobalModeEnabled = true;

                // Limit exceptions captured
                options.AddExceptionFilter(new ExceptionFilter());
            })
#endif
            .UseProgressBar()
            .UseSimpleShell()
            .UseSimpleToolkit()
            .UseSegmentedControl()
            .UseMaterialMauiIcons()
            .UseCupertinoMauiIcons()
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

#if MACCATALYST
    private class ExceptionFilter : Sentry.Extensibility.IExceptionFilter
    {
        public bool Filter(Exception ex)
        {
            // Don't filter unhandled exceptions of any type
            if (ex.Data is not null && ex.Data.Contains(Mechanism.HandledKey) && ex.Data[Mechanism.HandledKey] is false)
            {
                return false;
            }

            if (ex is IOException && ex.Message?.Contains("HTTP") == true)
            {
                return true;
            }

            if (ex is ArgumentNullException && (ex.StackTrace?.Contains("FromJson") == true || ex.StackTrace?.Contains("JSONParser") == true))
            {
                return true;
            }

            if (ex is NullReferenceException && (ex.StackTrace?.Contains("SKCanvasView") == true || ex.StackTrace?.Contains("SQLite") == true))
            {
                return true;
            }

            if (ex is not null && ex.Message?.Contains("tz_long") == true)
            {
                return true;
            }

            if (ex is System.Text.Json.JsonException || ex is System.Net.Http.HttpRequestException ||
                ex is System.Net.WebException || ex is System.Runtime.InteropServices.COMException ||
                ex is SimpleWeather.Utils.WeatherException || ex is System.IO.FileNotFoundException ||
                ex is System.Threading.Tasks.TaskCanceledException || ex is System.TimeoutException ||
                ex is SQLite.SQLiteException || ex is Newtonsoft.Json.JsonReaderException)
            {
                return true;
            }

            return false;
        }
    }
#endif
}
