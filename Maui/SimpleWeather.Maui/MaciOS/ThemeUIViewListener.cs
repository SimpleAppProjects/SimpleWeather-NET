#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace SimpleWeather.Maui
{
    public static class ThemeUIViewListener
    {
        public static void InitThemeListener(this AppDelegate appDelegate)
        {
            if (OperatingSystem.IsIOSVersionAtLeast(13))
            {
                UIKit.UIWindow uiWindow = null;

                if (OperatingSystem.IsIOSVersionAtLeast(15))
                {
                    uiWindow = UIKit.UIApplication.SharedApplication.ConnectedScenes.OfType<UIKit.UIScene>()
                        .Select(s => (s as UIKit.UIWindowScene)?.KeyWindow)
                        .FirstOrDefault();
                }
                else
                {
                    uiWindow = UIKit.UIApplication.SharedApplication.ConnectedScenes.OfType<UIKit.UIScene>()
                        .SelectMany(s => (s as UIKit.UIWindowScene)?.Windows ?? Array.Empty<UIKit.UIWindow>())
                        .FirstOrDefault(w => w.IsKeyWindow);
                }

                // TODO: NOTE: workaround for auto theme change
                // Should be fixed in future .NET version
                var themeView = new ThemeUIView()
                {
                    Opaque = false,
                    Alpha = 0.0f
                };
                uiWindow?.AddSubview(themeView);
                uiWindow?.SendSubviewToBack(themeView);
            }
        }
    }

    internal class ThemeUIView : UIView
    {
        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            if (OperatingSystem.IsIOSVersionAtLeast(13))
            {
                if (this.TraitCollection.UserInterfaceStyle != previousTraitCollection.UserInterfaceStyle)
                {
                    App.Current.UserAppTheme = this.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark ? AppTheme.Dark : AppTheme.Light;
                    App.Current.UpdateAppTheme();
                }
            }
        }
    }
}
#endif