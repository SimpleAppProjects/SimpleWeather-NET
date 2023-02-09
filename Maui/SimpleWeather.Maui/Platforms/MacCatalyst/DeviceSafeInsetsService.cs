using System;
using UIKit;

namespace SimpleWeather.Maui.Services
{
    public partial class DeviceSafeInsetsService
    {
        public partial double GetSafeAreaBottom()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                UIWindow window = UIApplication.SharedApplication.Delegate.GetWindow();
                var bottomPadding = window.SafeAreaInsets.Bottom;
                return bottomPadding;
            }
            return 0;
        }

        public partial double GetSafeAreaTop()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                UIWindow window = UIApplication.SharedApplication.Delegate.GetWindow();
                var TopPadding = window.SafeAreaInsets.Top;
                return TopPadding;
            }
            return 0;
        }
    }
}

