using System;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using muxc = Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Uno.Controls
{
    public sealed partial class BannerContent : UserControl
    {
        public Banner BannerModel
        {
            get { return this.DataContext as Banner; }
        }

        public BannerContent()
        {
            this.InitializeComponent();
        }

        public void Dismiss()
        {
            this.Infobar.IsOpen = false;
        }

        public bool IsShowing => this.Infobar.IsOpen;

        public void Show()
        {
            this.Infobar.IsOpen = true;
        }

        internal static muxc.InfoBarSeverity ToSeverity(BannerInfoType type)
        {
            return (muxc.InfoBarSeverity)type;
        }
    }
}
