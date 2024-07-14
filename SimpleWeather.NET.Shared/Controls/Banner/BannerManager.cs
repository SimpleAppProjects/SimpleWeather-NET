#if WINDOWS
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using CommunityToolkit.Maui.Markup;
using SimpleWeather.Maui.Controls;
#endif

namespace SimpleWeather.NET.Controls
{
    public sealed class BannerManager
    {
        private Banner CurrentBanner;
#if WINDOWS
        private Panel ParentView;
#else
        private Layout ParentView;
#endif
        private BannerContent BannerView;

#if WINDOWS
        public BannerManager(Panel Parent)
#else
        public BannerManager(Layout Parent)
#endif
        {
            if (Parent == null)
                throw new ArgumentException("Parent is not a Panel control", nameof(Parent));

            ParentView = Parent;
        }

        public void Show(Banner banner)
        {
            // Add current banner to stack
            CurrentBanner = banner;

            // Update banner view
            UpdateView();
        }

        public void Dismiss()
        {
            CurrentBanner = null;
            UpdateView();
        }

        /**
         * Update the banner view
         */
        private void UpdateView()
        {
            // Get current banner
            Banner banner = CurrentBanner;
            if (banner == null)
            {
                // Dismiss view if there are no more banners
                if (BannerView != null)
                {
                    ParentView?.Children.Remove(BannerView);
                    BannerView = null;
                }
            }
            else
            {
                // Check if InAppNotification view exists
                if (BannerView == null)
                {
                    BannerView = new BannerContent();
                }

                // Update button command
#if WINDOWS
                BannerView.DataContext = banner;

                if (BannerView.FindName("ActionButton") is Button button)
                {
                    button.Command = new RelayCommand(() =>
                    {
                        banner?.ButtonAction?.Invoke();
                    });
                }

                if (!ParentView.Children.Contains(BannerView))
                {
                    ParentView.Children.Insert(0, BannerView);
                }

                if (!BannerView.IsShowing)
                {
                    BannerView.Show();
                }
#else
                BannerView.BindingContext = banner;

                if (!ParentView.Children.Contains(BannerView))
                {
                    ParentView.Children.Insert(0, BannerView);
                }

                if (!BannerView.IsVisible)
                {
                    BannerView.IsVisible(true);
                }
#endif
            }
        }
    }
}