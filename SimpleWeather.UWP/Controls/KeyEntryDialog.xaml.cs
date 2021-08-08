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

        public KeyEntryDialog(bool UseDefaultKey = true)
        {
            Initialize(UseDefaultKey);
        }

        public void Initialize(bool UseDefaultKey = true)
        {
            this.InitializeComponent();
            this.Closing += KeyEntryDialog_Closing;
            KeyEntry.TextChanged += KeyEntry_TextChanged;

            if (UseDefaultKey)
            {
                KeyEntry.Text = Key = Settings.API_KEY ?? string.Empty;
            }
        }

        public void SetKey(string value)
        {
            KeyEntry.Text = Key = value ?? string.Empty;
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