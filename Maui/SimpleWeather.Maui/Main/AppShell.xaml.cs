using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Controls.AppBar;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.Preferences;
using SimpleWeather.Maui.ViewModels;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Main;

public sealed partial class AppShell : ViewModelShell, IViewModelProvider
{
    public static AppShell Instance { get; private set; }

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public AppShell()
    {
        Instance = this;
        InitializeComponent();
        // TODO: add support for passing arguments
        AnalyticsLogger.LogEvent("AppShell");

        App.Current.UpdateAppTheme();

        RegisterRoutes();
        UpdateBottomBarItemWidth();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("weather", typeof(WeatherNow));
        Routing.RegisterRoute("alerts", typeof(WeatherAlertPage));
        Routing.RegisterRoute("radar", typeof(WeatherRadarPage));
        Routing.RegisterRoute("locations", typeof(LocationsPage));
        Routing.RegisterRoute("settings", typeof(Settings_General));
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(ShellContents))
        {
            UpdateBottomBarItemWidth();
        }
    }

    private void UpdateBottomBarItemWidth()
    {
        // Update bottom bar
        if (DeviceInfo.Idiom != DeviceIdiom.Phone && ShellContents != null)
        {
            var deviceDisplay = DeviceDisplay.Current;
            ShellTabBar.ItemWidthRequest = (deviceDisplay.MainDisplayInfo.Width / deviceDisplay.MainDisplayInfo.Density) / this.ShellContents.Count;
        }
    }

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        if (args.CanCancel)
        {
            if (CurrentPage is IBackRequestedPage backRequestedPage && await backRequestedPage.OnBackRequested())
            {
                args.Cancel();
            }
        }

        if (CurrentPage?.ToolbarItems is INotifyCollectionChanged notifyColl)
        {
            notifyColl.CollectionChanged -= NotifyColl_CollectionChanged;
        }
        if (CurrentPage?.ToolbarItems is INotifyPropertyChanged notifyProp)
        {
            notifyProp.PropertyChanged -= NotifyProp_PropertyChanged;
        }

        base.OnNavigating(args);
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        if (ShellTabBar.Items?.Any() == true)
        {
            var currentPageType = this.CurrentPage.GetType();

            if (currentPageType == typeof(WeatherNow) || currentPageType == typeof(WeatherDetailsPage))
            {
                ShellTabBar.SelectedItem = ShellTabBar.Items.FirstOrDefault(sc => sc.Route == "weather");
            }
            else if (currentPageType == typeof(WeatherAlertPage))
            {
                ShellTabBar.SelectedItem = ShellTabBar.Items.FirstOrDefault(sc => sc.Route == "alerts");
            }
            else if (currentPageType == typeof(WeatherRadarPage))
            {
                ShellTabBar.SelectedItem = ShellTabBar.Items.FirstOrDefault(sc => sc.Route == "radar");
            }
            else if (currentPageType == typeof(LocationsPage))
            {
                ShellTabBar.SelectedItem = ShellTabBar.Items.FirstOrDefault(sc => sc.Route == "locations");
            }
            else if (currentPageType == typeof(Settings_General))
            {
                ShellTabBar.SelectedItem = ShellTabBar.Items.FirstOrDefault(sc => sc.Route == "settings");
            }
        }

        UpdateAppBar();
    }

    private async void ShellTabBar_ItemSelected(object sender, SimpleToolkit.SimpleShell.Controls.TabItemSelectedEventArgs e)
    {
        if (e.ShellItem.Route.Contains("weather"))
            await this.Navigation.PopToRootAsync(true);
        else if (!CurrentState.Location.OriginalString.Contains(e.ShellItem.Route))
            await this.GoToAsync($"/{e.ShellItem.Route}", true);
    }

    private void UpdateAppBar()
    {
        if (this.CurrentPage is Page currentPage)
        {
            ShellAppBar.Bind(AppBar.TitleProperty, nameof(currentPage.Title), BindingMode.OneWay, source: currentPage);
            ShellAppBar.Bind(AppBar.ToolbarItemsProperty, nameof(currentPage.ToolbarItems), BindingMode.OneWay, source: currentPage);

            if (currentPage.ToolbarItems is INotifyCollectionChanged notifyColl)
            {
                notifyColl.CollectionChanged += NotifyColl_CollectionChanged;
            }
            if (currentPage.ToolbarItems is INotifyPropertyChanged notifyProp)
            {
                notifyProp.PropertyChanged += NotifyProp_PropertyChanged;
            }

            ShellAppBar.IsVisible = Shell.GetNavBarIsVisible(currentPage);
        }
        else
        {
            ShellAppBar.ClearValue(AppBar.TitleProperty);
            ShellAppBar.ClearValue(AppBar.ToolbarItemsProperty);
            ShellAppBar.IsVisible = true;
        }

        ShellAppBar.BackButtonVisible = this.Navigation.NavigationStack.Count > 1;
    }

    private void NotifyColl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ShellAppBar.ToolbarItems = (sender as IList<ToolbarItem>)?.ToList();
    }

    private void NotifyProp_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ShellAppBar.ToolbarItems = (sender as IList<ToolbarItem>)?.ToList();
    }

    private async void ShellAppBar_BackTapped(object sender, EventArgs e)
    {
        await this.Navigation.PopAsync(true);
    }
}
