using SimpleWeather.Utils;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Controls
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

    public sealed class Banner : DependencyObject
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

        public static Task<Banner> MakeAsync(CoreDispatcher dispatcher, String message, BannerInfoType infoType = BannerInfoType.Informational)
        {
            return DispatcherExtensions.RunOnUIThread(dispatcher, () =>
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