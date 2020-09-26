using SimpleWeather.Utils;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class KeyEntryDialog : ContentDialog
    {
        public string Key { get; set; }
        public bool CanClose { get; set; }

        public KeyEntryDialog()
        {
            Initialize();
        }

        public void Initialize()
        {
            this.InitializeComponent();
            this.Closing += KeyEntryDialog_Closing;
            KeyEntry.TextChanged += KeyEntry_TextChanged;
            KeyEntry.Text = Key = Settings.API_KEY;
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