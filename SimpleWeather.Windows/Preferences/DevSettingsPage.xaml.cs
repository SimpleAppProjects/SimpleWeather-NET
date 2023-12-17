using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.Utils;
using Windows.Storage;
using Windows.Storage.Pickers;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DevSettingsPage : Page
    {
        public DevSettingsPage()
        {
            this.InitializeComponent();

#if DEBUG
            // This should always be true in debug mode
            DebugSwitch.IsOn = Logger.IsDebugLoggerEnabled();
            DebugSwitch.IsEnabled = false;
#else
            DebugSwitch.IsOn = Logger.IsDebugLoggerEnabled();
            DebugSwitch.IsEnabled = true;
#endif
            DebugSwitch.Toggled += DebugSwitch_Toggled;

            SaveLogsBtn.Click += SaveLogsBtn_Click;
        }

        private void DebugSwitch_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var sw = sender as ToggleSwitch;

            Logger.EnableDebugLogger(sw.IsOn);
        }

        private async void SaveLogsBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            void ShowNoLogsFoundPrompt()
            {
                if (this.Content is Panel root)
                {
                    DispatcherQueue.EnqueueAsync(() =>
                    {
                        var snackMgr = new SnackbarManager(root);
                        snackMgr.Show(Snackbar.Make("No logs found", SnackbarDuration.Short));
                    });
                }
            }

            try
            {
                var logsDirectory = await ApplicationData.Current.LocalFolder.GetFolderAsync("logs");
                var logFiles = await logsDirectory.GetFilesAsync();

                if (logFiles.Count == 0)
                {
                    ShowNoLogsFoundPrompt();
                    return;
                }

                await DispatcherQueue.EnqueueAsync(async () =>
                {
                    var contentDialog = new ContentDialog
                    {
                        IsPrimaryButtonEnabled = false,
                        IsSecondaryButtonEnabled = true,
                        Title = "Save Logs",
                        XamlRoot = this.XamlRoot,
                        SecondaryButtonText = ResStrings.Label_Cancel
                    };

                    contentDialog.SecondaryButtonCommand = new RelayCommand(() =>
                    {
                        contentDialog.Hide();
                    });

                    var listView = new ListView()
                    {
                        IsItemClickEnabled = true,
                        IsSwipeEnabled = false,
                        IsMultiSelectCheckBoxEnabled = false,
                        ItemTemplate = this.Resources["LogFileTemplate"] as DataTemplate,
                        ItemsSource = logFiles
                    };

                    listView.ItemClick += async (s, e) =>
                    {
                        if (e.ClickedItem is not StorageFile logFile) return;

                        try
                        {
                            // Create a file picker
                            var savePicker = new FileSavePicker();

                            // Retrieve the window handle (HWND) of the current WinUI 3 window.
                            var window = MainWindow.Current;
                            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                            // Initialize the file picker with the window handle (HWND).
                            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

                            // Set options for your file picker
                            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                            // Dropdown of file types the user can save the file as
                            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".log" });
                            savePicker.SuggestedFileName = logFile.Name;

                            // Open the picker for the user to pick a file
                            var file = await savePicker.PickSaveFileAsync();

                            if (file != null)
                            {
                                await logFile.CopyAndReplaceAsync(file);
                                contentDialog.Hide();
                            }
                        }
                        catch { }
                    };

                    contentDialog.Content = listView;

                    await contentDialog.ShowAsync();
                });
            }
            catch (FileNotFoundException)
            {
                // Ignore error
                ShowNoLogsFoundPrompt();
            }
        }
    }
}
