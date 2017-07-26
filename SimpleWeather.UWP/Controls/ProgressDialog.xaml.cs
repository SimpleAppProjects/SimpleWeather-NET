using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class ProgressDialog : UserControl
    {
        public ProgressDialog()
        {
            this.InitializeComponent();
            this.IsEnabledChanged += ProgressDialog_IsEnabledChanged;

            this.IsEnabled = false;
        }

        private async void ProgressDialog_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if ((bool)e.NewValue)
                    await Dialog.ShowAsync();
                else
                    Dialog.Hide();
            });
        }
    }
}