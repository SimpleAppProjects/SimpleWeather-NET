﻿using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.Preferences;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
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
            KeyEntryTextBlock.Text = SettingsManager.APIKeys[API] ?? ResStrings.key_hint;
        }

        private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var keydialog = new KeyEntryDialog(API)
            {
                RequestedTheme = this.ActualTheme,
                // NOTE: Required to avoid System.ArgumentException: This element is already associated with a XamlRoot...
                // https://github.com/microsoft/microsoft-ui-xaml/issues/4990#issuecomment-1181390828
                XamlRoot = this.XamlRoot
            };

            keydialog.PrimaryButtonClick += (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                var diag = dialog as KeyEntryDialog;

                string key = diag.Key;
                KeyEntryTextBlock.Text = key ?? string.Empty;
                SettingsManager.APIKeys[API] = key;
                SettingsManager.UsePersonalKeys[API] = true;

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
