using SimpleWeather.Utils;
using SimpleWeather.UWP.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Safety", "UWP001:Platform-specific", Justification = "Platform check is defined")]
    public sealed partial class Settings_About : Page
    {
#if WINDOWS_UWP
        private StoreContext context = null;
#endif
        private DevSettingsController devSettingsController;

        public Settings_About()
        {
            this.InitializeComponent();

            devSettingsController = new DevSettingsController();

            Version.Text = string.Format(CultureInfo.InvariantCulture, "v{0}.{1}.{2}.{3}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);

#if WINDOWS_UWP
            if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
            {
                if (context == null)
                {
                    context = StoreContext.GetDefault();
                }

                UpdateProgressPanel.Visibility = Visibility.Visible;
                Task.Run(CheckForUpdates);
            }
            else
#endif
            {
                UpdateProgressPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void SettingsAbout_Resuming(object sender, object e)
        {
#if WINDOWS_UWP
            Task.Run(CheckForUpdates);
#endif
        }

#if WINDOWS_UWP
        private async Task CheckForUpdates()
        {
            // Show that we're checking for updates
            await Dispatcher.RunOnUIThread(() =>
            {
                CheckUpdateButton.Visibility = Visibility.Visible;
            }).ConfigureAwait(false);

            // Get the updates that are available.
            IReadOnlyList<StorePackageUpdate> updates = null;

            try
            {
                updates = await context.GetAppAndOptionalStorePackageUpdatesAsync();
            }
            catch (FileNotFoundException)
            {
                // Debug: store context does not exist
            }

            await Dispatcher.RunOnUIThread(() =>
            {
                CheckUpdateButton.Visibility = Visibility.Collapsed;

                if (updates?.Count > 0)
                {
                    // Updates are available
                    InstallButton.Content = App.Current.ResLoader.GetString("prompt_update_available");
                    InstallButton.IsHitTestVisible = true;
                    InstallButton.Click += InstallButton_Click;
                }
                else
                {
                    // No updates available
                    InstallButton.Content = App.Current.ResLoader.GetString("NoUpdateAvailablePrompt");
                    InstallButton.IsHitTestVisible = false;
                    InstallButton.Click -= InstallButton_Click;
                }

                InstallButton.Visibility = Visibility.Visible;
            }).ConfigureAwait(false);
        }
#endif

        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
#if WINDOWS_UWP
            CheckUpdateButton.IsEnabled = false;

            UpdateProgressBar.IsIndeterminate = true;
            UpdateProgressBar.ShowError = false;
            UpdateProgressBar.Value = 0;
            UpdateProgressBar.Visibility = Visibility.Visible;

            // Download and install the updates.
            IReadOnlyList<StorePackageUpdate> updates =
                await context.GetAppAndOptionalStorePackageUpdatesAsync();
            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
                context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

            // The Progress async method is called one time for each step in the download
            // and installation process for each package in this request.
            downloadOperation.Progress = async (asyncInfo, progress) =>
            {
                await Dispatcher.RunOnUIThread(() =>
                {
                    if (UpdateProgressBar.IsIndeterminate)
                    {
                        UpdateProgressBar.IsIndeterminate = false;
                    }

                    UpdateProgressBar.Value = progress.PackageDownloadProgress;

                    if (progress.PackageDownloadProgress >= 0.8)
                    {
                        InstallButton.Content = App.Current.ResLoader.GetString("InstallingPrompt");
                    }
                    else
                    {
                        InstallButton.Content = App.Current.ResLoader.GetString("DownloadingPrompt");
                    }
                }).ConfigureAwait(true);
            };

            StorePackageUpdateResult result = await downloadOperation.AsTask().ConfigureAwait(true);

            switch (result.OverallState)
            {
                case StorePackageUpdateState.Completed:
                    break;
                // If the user cancelled the download or you can't perform the download for some other
                // reason (for example, Wi-Fi might have been turned off and the device is now on
                // a metered network) try again later. The RetryDownloadAndInstallLater method is not  
                // implemented in this example, you should implement it as needed for your own app.
                case StorePackageUpdateState.Canceled:
                case StorePackageUpdateState.ErrorLowBattery:
                case StorePackageUpdateState.ErrorWiFiRecommended:
                case StorePackageUpdateState.ErrorWiFiRequired:
                case StorePackageUpdateState.OtherError:
                    InstallButton.Content = App.Current.ResLoader.GetString("InstallErrorPrompt");
                    UpdateProgressBar.ShowError = true;
                    CheckUpdateButton.IsEnabled = true;
                    return;
                default:
                    CheckUpdateButton.IsEnabled = true;
                    break;
            }
#endif
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Application.Current.Resuming -= SettingsAbout_Resuming;
        }

        private async void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
#if WINDOWS_UWP
            if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
            {
                await context.RequestRateAndReviewAppAsync();
            }
            else
#endif
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NKC37BC8SRX"));
            }
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
#if WINDOWS_UWP
            var deviceType = DeviceTypeHelper.DeviceType;
            if ((deviceType == DeviceTypeHelper.DeviceTypes.Desktop || deviceType == DeviceTypeHelper.DeviceTypes.Mobile) && Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
                await launcher.LaunchAsync();
            }
            else
#endif
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto://thewizrd.dev+SimpleWeatherWindows@gmail.com"));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            devSettingsController.OnStart();
        }

        private void Version_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            devSettingsController.OnClick();
        }
    }
}
