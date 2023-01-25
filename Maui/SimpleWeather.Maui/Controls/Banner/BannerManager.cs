using CommunityToolkit.Maui.Markup;

namespace SimpleWeather.Maui.Controls
{
    public sealed class BannerManager
    {
        private Banner CurrentBanner;
        private Layout ParentView;
        private BannerContent BannerView;

        public BannerManager(Layout Parent)
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
                BannerView.BindingContext = banner;

                if (!ParentView.Children.Contains(BannerView))
                {
                    ParentView.Children.Insert(0, BannerView);
                }

                if (!BannerView.IsVisible)
                {
                    BannerView.IsVisible(true);
                }
            }
        }
    }
}
