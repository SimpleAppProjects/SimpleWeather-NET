using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class ProgressDialog : ContentDialog
    {
        CoreDispatcher dispatcher;

        public ProgressDialog()
        {
            this.InitializeComponent();
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        public new async Task ShowAsync()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await base.ShowAsync(); });
        }

        public async Task HideAsync()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { base.Hide(); });
        }
    }
}