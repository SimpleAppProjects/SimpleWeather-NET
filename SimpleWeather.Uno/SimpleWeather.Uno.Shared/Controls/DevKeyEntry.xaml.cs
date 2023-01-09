using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.Preferences;
using System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Uno.Controls
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

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public DevKeyEntry()
        {
            this.InitializeComponent();
        }

        public void UpdateKeyEntry()
        {
            KeyEntryTextBlock.Text = SettingsManager.APIKeys[API] ?? App.Current.ResLoader.GetString("key_hint");
        }

        private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var keydialog = new KeyEntryDialog(API)
            {
                RequestedTheme = this.ActualTheme
            };

            keydialog.PrimaryButtonClick += (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                var diag = dialog as KeyEntryDialog;

                string key = diag.Key;
                KeyEntryTextBlock.Text = key ?? string.Empty;
                SettingsManager.APIKeys[API] = key;

                diag.CanClose = true;
                diag.Hide();
            };

            await DispatcherQueue.EnqueueAsync(async () =>
            {
                await keydialog.ShowAsync();
            });
        }
    }
}
