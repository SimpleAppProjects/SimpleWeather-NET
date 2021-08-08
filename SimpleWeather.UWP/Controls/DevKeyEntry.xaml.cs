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
    public sealed partial class DevKeyEntry : UserControl
    {
        public string API
        {
            get { return (string)GetValue(APIProperty); }
            set { SetValue(APIProperty, value); }
        }

        // Using a DependencyProperty as the backing store for API.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty APIProperty =
            DependencyProperty.Register("API", typeof(string), typeof(DevKeyEntry), new PropertyMetadata(null, (s, e) => (s as DevKeyEntry)?.UpdateKeyEntry()));

        public DevKeyEntry()
        {
            this.InitializeComponent();
        }

        public void UpdateKeyEntry()
        {
            KeyEntryTextBlock.Text = DevSettingsEnabler.GetAPIKey(API) ?? App.ResLoader.GetString("key_hint");
        }

        private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var keydialog = new KeyEntryDialog(false)
            {
                RequestedTheme = this.ActualTheme
            };
            keydialog.SetKey(DevSettingsEnabler.GetAPIKey(API));

            keydialog.PrimaryButtonClick += (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                var diag = dialog as KeyEntryDialog;

                string key = diag.Key;
                KeyEntryTextBlock.Text = key ?? string.Empty;
                DevSettingsEnabler.SetAPIKey(API, key);

                diag.CanClose = true;
                diag.Hide();
            };

            await Dispatcher.RunOnUIThread(async () =>
            {
                await keydialog.ShowAsync();
            });
        }
    }
}
