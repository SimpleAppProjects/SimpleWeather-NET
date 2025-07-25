﻿using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using WinRT.Interop;
using WinUIEx;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Safety", "UWP001:Platform-specific", Justification = "Platform check is defined")]
    public sealed partial class Settings_About : Page
    {
        private StoreContext context = null;
        private DevSettingsController devSettingsController;

        public Settings_About()
        {
            this.InitializeComponent();

            devSettingsController = new DevSettingsController();

            Version.Description = string.Format(CultureInfo.InvariantCulture, "v{0}.{1}.{2}.{3}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);

            if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
            {
                if (context == null)
                {
                    context = StoreContext.GetDefault();

                    // Required for WinUI
                    var window = MainWindow.Current ?? Window.Current;
                    InitializeWithWindow.Initialize(context, window.GetWindowHandle());
                }

                UpdateProgressPanel.Visibility = Visibility.Visible;
                Task.Run(CheckForUpdates);
            }
            else
            {
                UpdateProgressPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void SettingsAbout_Resuming(object sender, object e)
        {
            Task.Run(CheckForUpdates);
        }

        private async Task CheckForUpdates()
        {
            // Show that we're checking for updates
            await DispatcherQueue.EnqueueAsync(() =>
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

            await DispatcherQueue.EnqueueAsync(() =>
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

        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
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
                await DispatcherQueue.EnqueueAsync(() =>
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private async void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
            {
                await DispatcherQueue.EnqueueAsync(async () =>
                {
                    try
                    {
                        await context.RequestRateAndReviewAppAsync();
                    }
                    catch (Exception)
                    {
                        // Debug: store context does not exist
                    }
                });
            }
            else
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NKC37BC8SRX"));
            }
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"mailto://{Constants.SUPPORT_EMAIL_ADDRESS}"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            devSettingsController.OnStart();
        }

        private void Version_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            devSettingsController.OnClick();
        }
    }
}
