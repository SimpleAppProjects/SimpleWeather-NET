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
        private string CurrentAPI { get; }

        public KeyEntryDialog(String CurrentAPI)
        {
            this.CurrentAPI = CurrentAPI;
            Initialize();
        }

        public KeyEntryDialog()
        {
            if (String.IsNullOrWhiteSpace(CurrentAPI))
                CurrentAPI = Utils.Settings.API;

            Initialize();
        }

        public void Initialize()
        {
            this.InitializeComponent();
            this.Closing += KeyEntryDialog_Closing;
            KeyEntry.TextChanged += KeyEntry_TextChanged;
            KeyEntry_2.TextChanged += KeyEntry_TextChanged;

            if (CurrentAPI == WeatherData.WeatherAPI.Here)
            {
                Key = Utils.Settings.API_KEY;

                string app_id = String.Empty;
                string app_code = String.Empty;

                if (!String.IsNullOrWhiteSpace(Key))
                {
                    var keyArr = Key.Split(';');
                    if (keyArr.Length > 0)
                    {
                        app_id = keyArr[0];
                        app_code = keyArr[keyArr.Length > 1 ? keyArr.Length - 1 : 0];
                    }
                }

                KeyEntry.PlaceholderText = "App ID";
                KeyEntry_2.PlaceholderText = "App Code";

                KeyEntry.Text = app_id;
                KeyEntry_2.Text = app_code;

                KeyEntry_2.Visibility = Visibility.Visible;
            }
            else
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
            if (CurrentAPI == WeatherData.WeatherAPI.Here)
                Key = String.Format("{0};{1}", KeyEntry.Text, KeyEntry_2.Text);
            else
                Key = KeyEntry.Text;
        }
    }
}
