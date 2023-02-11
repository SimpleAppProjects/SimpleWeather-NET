#if WINDOWS
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using muxc = Microsoft.UI.Xaml.Controls;
#endif

namespace SimpleWeather.NET.Controls
{
#if WINDOWS
    public enum BannerInfoType
    {
        Informational = muxc.InfoBarSeverity.Informational,
        Success = muxc.InfoBarSeverity.Success,
        Warning = muxc.InfoBarSeverity.Warning,
        Error = muxc.InfoBarSeverity.Error
    }
#endif

    /**
     * Wrapper for the Banner/Infobar
     * Banner is managed by BannerManager
     */

#if WINDOWS
    public sealed partial class Banner : DependencyObject
#else
    public sealed partial class Banner : BindableObject
#endif
    {
        private Banner()
        {
        }

        public string Message { get; set; }
        public string ButtonLabel { get; set; }
        public Action ButtonAction { get; set; }
        // TODO: add hyperlink option if needed

        public string Title { get; set; }
#if WINDOWS
        public IconSource Icon { get; set; }
#else
        public ImageSource Icon { get; set; }
#endif
#if WINDOWS
        public BannerInfoType InfoType { get; set; } = BannerInfoType.Informational;
#endif

        public static Banner Make(String message
#if WINDOWS
            , BannerInfoType infoType = BannerInfoType.Informational
#endif
            )
        {
            Banner Banner = new Banner()
            {
                Message = message,
#if WINDOWS
                InfoType = infoType
#endif
            };

            return Banner;
        }

#if WINDOWS
        public static Banner MakeSuccess(String message)
        {
            return Banner.Make(message, BannerInfoType.Success);
        }

        public static Banner MakeWarning(String message)
        {
            return Banner.Make(message, BannerInfoType.Warning);
        }

        public static Banner MakeError(String message)
        {
            return Banner.Make(message, BannerInfoType.Error);
        }
#endif

#if WINDOWS
        public static Task<Banner> MakeAsync(DispatcherQueue dispatcher, String message, BannerInfoType infoType = BannerInfoType.Informational)
        {
            return dispatcher.EnqueueAsync(() =>
            {
                return Banner.Make(message, infoType);
            });
        }
#else
        public static Task<Banner> MakeAsync(IDispatcher dispatcher, String message)
        {
            return DispatcherExtensions.DispatchAsync(dispatcher, () =>
            {
                return Banner.Make(message);
            });
        }
#endif

        public void SetAction(String buttonTxt, Action action)
        {
            ButtonLabel = buttonTxt;
            ButtonAction = action;
        }
    }
}