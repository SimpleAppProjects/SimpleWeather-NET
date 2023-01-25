using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.ViewModels;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Main;

public partial class AppShell : ViewModelShell, IViewModelProvider, ISnackbarManager, IBannerManager
{
    private SnackbarManager SnackMgr { get; set; }
    private BannerManager BannerMgr { get; set; }

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public AppShell()
    {
        InitializeComponent();
        // TODO: add support for passing arguments
        AnalyticsLogger.LogEvent("AppShell");

        InitSnackManager();
        InitBannerManager();

        App.Current.UpdateAppTheme();
    }

    public void InitSnackManager()
    {
        if (SnackMgr == null)
        {
            //SnackMgr = new SnackbarManager(this);
        }
    }

    public void ShowSnackbar(Snackbar snackbar)
    {
        Dispatcher.Dispatch(() =>
        {
            SnackMgr?.Show(snackbar);
        });
    }

    public void DismissAllSnackbars()
    {
        Dispatcher.Dispatch(() =>
        {
            SnackMgr?.DismissAll();
        });
    }

    public void UnloadSnackManager()
    {
        DismissAllSnackbars();
        SnackMgr = null;
    }

    public void InitBannerManager()
    {
        if (BannerMgr == null)
        {
            //BannerMgr = new BannerManager(this);
        }
    }

    public void ShowBanner(Banner banner)
    {
        Dispatcher.Dispatch(() =>
        {
            BannerMgr?.Show(banner);
        });
    }

    public void DismissBanner()
    {
        Dispatcher.Dispatch(() =>
        {
            BannerMgr?.Dismiss();
        });
    }

    public void UnloadBannerManager()
    {
        DismissBanner();
        BannerMgr = null;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        InitSnackManager();
        InitBannerManager();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs e)
    {
        base.OnNavigatedFrom(e);
        UnloadSnackManager();
        UnloadBannerManager();
    }
}
