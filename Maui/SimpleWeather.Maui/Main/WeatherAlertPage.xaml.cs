using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Utils;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.NET.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SimpleWeather.Maui.Main;

public partial class WeatherAlertPage : ContentPage, ISnackbarManager, ISnackbarPage
{
    private LocationData.LocationData locationData { get; set; }
    public WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();
    public WeatherAlertsViewModel AlertsView { get; } = AppShell.Instance.GetViewModel<WeatherAlertsViewModel>();

    private SnackbarManager SnackMgr { get; set; }

    public WeatherAlertPage()
	{
		InitializeComponent();
        AnalyticsLogger.LogEvent("WeatherAlertPage");
    }

    internal WeatherAlertPage(WeatherPageArgs args) : this()
    {
        locationData = args.Location;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (locationData == null)
        {
            WNowViewModel.PropertyChanged += WNowViewModel_PropertyChanged;
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        WNowViewModel.PropertyChanged -= WNowViewModel_PropertyChanged;
    }

    private void WNowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WNowViewModel.UiState))
        {
            locationData = WNowViewModel.UiState.LocationData;
            Initialize();
        }
    }

    private void Initialize()
    {
        if (locationData == null)
        {
            locationData = WNowViewModel.UiState.LocationData;
        }

        if (locationData != null)
        {
            AlertsView.UpdateAlerts(locationData);
        }
    }

    public void InitSnackManager()
    {
        if (SnackMgr == null)
        {
            SnackMgr = new SnackbarManager(Content);
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
}