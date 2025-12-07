using CommunityToolkit.Maui.Storage;
using Foundation;
using Microsoft.Maui.Handlers;
using SimpleWeather.Helpers;
using SimpleWeather.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.Utils;
#if MACCATALYST
using UIKit;
#endif
#if __IOS__
using FirebaseInstallations = Firebase.Installations.Installations;
#endif
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class DevSettingsPage : ContentPage, ISnackbarManager, ISnackbarPage
{
    private SnackbarManager SnackMgr;

    public DevSettingsPage()
    {
        InitializeComponent();

        UpdateSettingsTableTheme();

#if __IOS__
        Task.Run(async () =>
        {
            try
            {
                var firebaseId = await FirebaseInstallations.DefaultInstance.GetInstallationIdAsync();

                await Dispatcher.DispatchAsync(() => { FirebaseIdCell.Detail = firebaseId; });
            }
            catch
            {
            }
        });

        FirebaseIdCell.Tapped += async (s, e) =>
        {
            await Clipboard.Default.SetTextAsync(FirebaseIdCell.Detail);
            SnackMgr?.Show(Snackbar.Make("Copied to clipboard", SnackbarDuration.Short));
        };
#endif

#if DEBUG
        // This should always be true in debug mode
        DebugSwitch.On = Logger.IsDebugLoggerEnabled();
        DebugSwitch.IsEnabled = false;
#else
        DebugSwitch.On = Logger.IsDebugLoggerEnabled();
        DebugSwitch.IsEnabled = true;
#endif

        DebugSwitch.OnChanged += DebugSwitch_OnChanged;
        LogsCell.Tapped += LogsCell_Tapped;
    }

    private void DebugSwitch_OnChanged(object sender, ToggledEventArgs e)
    {
        Logger.EnableDebugLogger(e.Value);
    }

    private void UpdateSettingsTableTheme()
    {
        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Color.Parse("#767676"), Color.Parse("#a2a2a2"),
            LightPrimary as Color, DarkPrimary as Color);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        AnalyticsLogger.LogEvent("DevSettingsPage: OnNavigatedTo");
        InitSnackManager();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        UnloadSnackManager();
    }

    public void InitSnackManager()
    {
        SnackMgr ??= new SnackbarManager(SnackbarContainer);
    }

    public void ShowSnackbar(Snackbar snackbar)
    {
        SnackMgr?.Show(snackbar);
    }

    public void DismissAllSnackbars()
    {
        SnackMgr?.DismissAll();
    }

    public void UnloadSnackManager()
    {
        DismissAllSnackbars();
        SnackMgr = null;
    }

    private void LogsCell_Tapped(object sender, EventArgs _)
    {
        void ShowNoLogsFoundPrompt()
        {
            Dispatcher.Dispatch(() => { SnackMgr?.Show(Snackbar.Make("No logs found", SnackbarDuration.Short)); });
        }

        try
        {
            var logFiles = new List<string>();

            var appLogsDirectory = Path.Combine(ApplicationDataHelper.GetLocalFolderPath(), "logs");
#if __IOS__
            var containerUrl = NSFileManager.DefaultManager.GetContainerUrl(AppDelegate.GROUP_IDENTIFIER);
            var logsContainerUrl = containerUrl.Append("logs", true);
            var groupLogsDirectory = logsContainerUrl.Path;

            try
            {
                if (Directory.Exists(groupLogsDirectory))
                {
                    var groupLogFiles = Directory.EnumerateFiles(groupLogsDirectory);
                    logFiles.AddRange(groupLogFiles);
                }
            }
            catch
            {
            }
#endif

            if (Directory.Exists(appLogsDirectory))
            {
                logFiles.AddRange(Directory.EnumerateFiles(appLogsDirectory));
            }

            if (logFiles.Count == 0)
            {
                ShowNoLogsFoundPrompt();
                return;
            }

            Dispatcher.Dispatch(async () =>
            {
                var cancelText = ResStrings.Label_Cancel;

#if MACCATALYST
                // Workaround for NullReferenceException issue
                // Source: https://github.com/dotnet/maui/issues/18156#issuecomment-1876739055
                if ((Handler as PageHandler)?.ViewController is not UIViewController viewController) return;

                var completion = new TaskCompletionSource<string>();

                var alert = UIAlertController.Create("Save Logs", null, UIAlertControllerStyle.ActionSheet);
                alert.AddAction(UIAlertAction.Create(cancelText, UIAlertActionStyle.Cancel,
                    _ => completion.TrySetResult(cancelText)));

                logFiles.Select(f => Path.GetFileName(f)).ForEach(f =>
                {
                    alert.AddAction(UIAlertAction.Create(f, UIAlertActionStyle.Default,
                        _ => completion.TrySetResult(f)));
                });

                viewController.PresentViewController(alert, true, null);

                var action = await completion.Task;
#else
                var action = await DisplayActionSheet("Save Logs", ResStrings.Label_Cancel, null, logFiles.Select(f =>
                {
                    return Path.GetFileName(f);
                }).OrderDescending().ToArray());
#endif

                if (!Equals(action, cancelText))
                {
                    // Get actual full file path
                    var filePath = logFiles.FirstOrDefault(f => Equals(action, Path.GetFileName(f)));

                    try
                    {
                        using var fileStream = File.OpenRead(filePath);
                        var saveResult = await FileSaver.SaveAsync(action, fileStream);

                        if (saveResult.Exception is not null)
                        {
                            Logger.WriteLine(LoggerLevel.Error, saveResult.Exception, "Error saving log file");
                        }
                    }
                    catch
                    {
                    }
                }
            });
        }
        catch (FileNotFoundException)
        {
            // Ignore error
            ShowNoLogsFoundPrompt();
        }
    }
}