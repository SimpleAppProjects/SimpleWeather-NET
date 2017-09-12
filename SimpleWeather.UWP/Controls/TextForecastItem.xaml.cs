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
    public sealed partial class TextForecastItem : UserControl
    {
        public TextForecastItem()
        {
            this.InitializeComponent();
            this.SizeChanged += TextForecastItem_SizeChanged;
        }

        private void TextForecastItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FctTextBox.Width = FctText.Width = e.NewSize.Width - WeatherBox.ActualWidth;
        }
    }
}
