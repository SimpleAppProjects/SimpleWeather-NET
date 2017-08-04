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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class KeyEntryDialog : ContentDialog
    {
        public string Key { get; set; }
        public bool CanClose { get; set; }

        public KeyEntryDialog()
        {
            this.InitializeComponent();
            this.Closing += KeyEntryDialog_Closing;
            KeyEntry.TextChanged += KeyEntry_TextChanged;

            KeyEntry.Text = Key = Utils.Settings.API_KEY;
        }

        private void KeyEntryDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (!CanClose)
                args.Cancel = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CanClose = true;
            this.Hide();
        }

        private void KeyEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Key = KeyEntry.Text;
        }
    }
}
