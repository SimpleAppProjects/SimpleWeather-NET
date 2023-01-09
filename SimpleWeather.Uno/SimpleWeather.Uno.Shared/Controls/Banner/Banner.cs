using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.Uno.Controls
{
    public enum BannerInfoType
    {
        Informational = muxc.InfoBarSeverity.Informational,
        Success = muxc.InfoBarSeverity.Success,
        Warning = muxc.InfoBarSeverity.Warning,
        Error = muxc.InfoBarSeverity.Error
    }

    /**
     * Wrapper for the Banner/Infobar
     * Banner is managed by BannerManager
     */

    public sealed partial class Banner : DependencyObject
    {
        private Banner()
        {
        }

        public string Message { get; set; }
        public string ButtonLabel { get; set; }
        public Action ButtonAction { get; set; }
        // TODO: add hyperlink option if needed

        public string Title { get; set; }
        public muxc.IconSource Icon { get; set; }
        public BannerInfoType InfoType { get; set; } = BannerInfoType.Informational;

        public static Banner Make(String message, BannerInfoType infoType = BannerInfoType.Informational)
        {
            Banner Banner = new Banner()
            {
                Message = message,
                InfoType = infoType
            };

            return Banner;
        }

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

        public static Task<Banner> MakeAsync(DispatcherQueue dispatcher, String message, BannerInfoType infoType = BannerInfoType.Informational)
        {
            return dispatcher.EnqueueAsync(() =>
            {
                return Banner.Make(message, infoType);
            });
        }

        public void SetAction(String buttonTxt, Action action)
        {
            ButtonLabel = buttonTxt;
            ButtonAction = action;
        }
    }
}