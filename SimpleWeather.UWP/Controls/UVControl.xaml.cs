using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class UVControl : UserControl
    {
        public UVIndexViewModel ViewModel
        {
            get { return (this.DataContext as UVIndexViewModel); }
        }

        private readonly WeatherIconsManager wim;

        public bool UseMonochrome
        {
            get { return (bool)GetValue(UseMonochromeProperty); }
            set { SetValue(UseMonochromeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseMonochrome.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseMonochromeProperty =
            DependencyProperty.Register("UseMonochrome", typeof(bool), typeof(UVControl), new PropertyMetadata(false));

        public UVControl()
        {
            this.InitializeComponent();
            wim = SharedModule.Instance.WeatherIconsManager;
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
                UVIcon.ShowAsMonochrome = wim.ShouldUseMonochrome(UVIcon.IconProvider ?? Settings.IconProvider);
            };
        }
    }
}
