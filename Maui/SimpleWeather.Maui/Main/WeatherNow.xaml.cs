using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.Location;
using SimpleWeather.Common.Utils;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Controls.Flow;
using SimpleWeather.Maui.Controls.Graphs;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.MaterialIcons;
using SimpleWeather.Maui.Utils;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.NET.Radar;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using System.ComponentModel;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Main;

public partial class WeatherNow : ScopePage, ISnackbarManager, ISnackbarPage, IBannerManager, IBannerPage
{
    private SnackbarManager SnackMgr { get; set; }
    private BannerManager BannerMgr { get; set; }

    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private RadarViewProvider radarViewProvider;

    private WeatherNowArgs args;

    private WeatherNowViewModel WNowViewModel { get; } = AppShell.Instance.GetViewModel<WeatherNowViewModel>();
    private ForecastsNowViewModel ForecastView { get; } = AppShell.Instance.GetViewModel<ForecastsNowViewModel>();
    private WeatherAlertsViewModel AlertsView { get; } = AppShell.Instance.GetViewModel<WeatherAlertsViewModel>();

    public Color ConditionPanelTextColor
    {
        get => (Color)GetValue(ConditionPanelTextColorProperty);
        set => SetValue(ConditionPanelTextColorProperty, value);
    }

    public static readonly BindableProperty ConditionPanelTextColorProperty =
        BindableProperty.Create(nameof(ConditionPanelTextColor), typeof(Color), typeof(WeatherNow), Colors.White);

    private bool UpdateBindings = false;
    private bool UpdateTheme = false;
    private bool ClearGraphIconCache = false;

    // Views
    private Label CurTemp { get; set; }
    private View ConditionPanelLayout { get; set; }
    private View Spacer { get; set; }
    private IconControl WeatherBox { get; set; }
    private Border RadarWebViewContainer { get; set; }

    public WeatherNow()
    {
        InitializeComponent();

        AnalyticsLogger.LogEvent("WeatherNow");

        Utils.FeatureSettings.OnFeatureSettingsChanged += FeatureSettings_OnFeatureSettingsChanged;
        SettingsManager.OnSettingsChanged += Settings_OnSettingsChanged;
        RadarProvider.RadarProviderChanged += RadarProvider_RadarProviderChanged;
        this.Loaded += WeatherNow_Loaded;
        this.Unloaded += WeatherNow_Unloaded;

        BindingContext = WNowViewModel;

        InitControls();
    }

    internal WeatherNow(WeatherNowArgs args) : this()
    {
        this.args = args;
    }

    public void InitSnackManager()
    {
        if (SnackMgr == null)
        {
            SnackMgr = new SnackbarManager(SnackbarContainer);
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
            BannerMgr = new BannerManager(MainGrid);
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

    private void Settings_OnSettingsChanged(SettingsChangedEventArgs e)
    {
        if (e.Key == SettingsManager.KEY_ICONSSOURCE)
        {
            // When page is loaded again from cache, clear icon cache
            ClearGraphIconCache = true;
            UpdateBindings = true;
        }
        else if (e.Key == SettingsManager.KEY_USERTHEME)
        {
            UpdateBindings = true;
            // Update theme
            UpdateTheme = true;
        }
    }

    private void FeatureSettings_OnFeatureSettingsChanged(FeatureSettingsChangedEventArgs e)
    {
        // When page is loaded again from cache, update bindings
        UpdateBindings = true;
        UpdateTheme = true;
    }

    private async void WeatherView_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(WNowViewModel.UiState):
                var uiState = WNowViewModel.UiState;

                if (Utils.FeatureSettings.WeatherRadar)
                {
                    WNowViewModel.Weather?.LocationCoord?.Let(coords =>
                    {
                        radarViewProvider?.UpdateCoordinates(coords, true);
                    });
                }

                if (uiState.NoLocationAvailable)
                {
                    await Dispatcher.DispatchAsync(() =>
                    {
                        var banner = Banner.Make(ResStrings.prompt_location_not_set);
                        banner.Icon = new MaterialIcon(MaterialSymbol.Map);
                        banner.SetAction(ResStrings.label_fab_add_location, async () =>
                        {
                            await Navigation.PushAsync(new LocationsPage());
                        });
                        ShowBanner(banner);
                    });
                }
                else
                {
                    DismissBanner();
                }
                break;

            case nameof(WNowViewModel.Weather):
                WNowViewModel.UiState?.LocationData?.Let(locationData =>
                {
                    ForecastView.UpdateForecasts(locationData);
                    AlertsView.UpdateAlerts(locationData);

                    /*
                    Task.Run(async () =>
                    {
                        // Update home tile if it hasn't been already
                        bool isHome = Equals(locationData, await SettingsManager.GetHomeData());
                        if (isHome && (TimeSpan.FromTicks((long)(DateTime.Now.Ticks - SettingsManager.UpdateTime.Ticks)).TotalMinutes > SettingsManager.RefreshInterval))
                        {
                            await WeatherUpdateBackgroundTask.RequestAppTrigger();
                        }
                        else if (isHome || SecondaryTileUtils.Exists(locationData?.query))
                        {
                            await WeatherTileCreator.TileUpdater(locationData);
                        }
                    });
                    */
                });
                break;

            case nameof(WNowViewModel.Alerts):
                {
                    var weatherAlerts = WNowViewModel.Alerts;
                    var locationData = WNowViewModel.UiState?.LocationData;

                    /*
                    if (wm.SupportsAlerts && locationData != null)
                    {
                        _ = Task.Run(async () =>
                        {
                            // Alerts are posted to the user here. Set them as notified.
#if DEBUG
                            await WeatherAlertHandler.PostAlerts(locationData, weatherAlerts);
#endif
                            await WeatherAlertHandler.SetasNotified(locationData, weatherAlerts);

                        });
                    }
                    */
                }
                break;

            case nameof(WNowViewModel.ErrorMessages):
                {
                    var errorMessages = WNowViewModel.ErrorMessages;

                    var error = errorMessages.FirstOrDefault();

                    if (error != null)
                    {
                        OnErrorMessage(error);
                    }
                }
                break;
        }
    }

    private void MainViewer_SizeChanged(object sender, EventArgs e)
    {
        AnalyticsLogger.LogEvent("WeatherNow: MainGrid_SizeChanged");
    }

    private void InitControls()
    {
        App.Current.Resources.TryGetValue("inverseBoolConverter", out var inverseBoolConverter);
        App.Current.Resources.TryGetValue("objectBooleanConverter", out var objectBooleanConverter);
        App.Current.Resources.TryGetValue("stringBooleanConverter", out var stringBooleanConverter);
        App.Current.Resources.TryGetValue("collectionBooleanConverter", out var collectionBooleanConverter);
        App.Current.Resources.TryGetValue("bool2GridLengthConverter", out var bool2GridLengthConverter);
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);
        Resources.TryGetValue("detailsFilter", out var detailsFilter);
        Resources.TryGetValue("graphDataConv", out var graphDataConv);
        Resources.TryGetValue("graphDataGridLengthConv", out var graphDataGridLengthConv);

        // Refresh toolbar item
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            var refreshTlbrItm = new ToolbarItem()
            {
                Text = ResStrings.action_refresh,
                IconImageSource = new MaterialIcon(MaterialSymbol.Refresh)
                {
                    Size = 24,
                    FontAutoScalingEnabled = true
                }.AppThemeColorBinding(MaterialIcon.ColorProperty, Colors.Black, Colors.White),
                Order = ToolbarItemOrder.Primary,
            }.Bind(ToolbarItem.IsEnabledProperty, nameof(RefreshLayout.IsRefreshing), BindingMode.OneWay, inverseBoolConverter as IValueConverter, source: RefreshLayout);
            refreshTlbrItm.Clicked += RefreshBtn_Clicked;
            ToolbarItems.Add(refreshTlbrItm);
        }

        // Condition Panel
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {

            }
            else
            {
                ListLayout.Add(
                    new VerticalStackLayout()
                    {
                        Children =
                        {
                            // GPSIcon + Location
                            new Grid()
                            {
                                Margin = new Thickness(5,10,5,5),
                                VerticalOptions = LayoutOptions.Center,
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition(GridLength.Auto),
                                    new ColumnDefinition(GridLength.Star)
                                },
                                Children =
                                {
                                    new Border()
                                    {
                                        Margin = new Thickness(16, 0),
                                        VerticalOptions = LayoutOptions.Center,
                                        Content = new Image()
                                        {
                                            HeightRequest = 24,
                                            WidthRequest = 24,
                                            Source = new MaterialIcon(MaterialSymbol.MyLocation)
                                            {

                                            }.Bind(FontImageSource.ColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this)
                                        }
                                    }
                                    .Column(0)
                                    .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.UiState)}.{nameof(WNowViewModel.UiState.IsGPSLocation)}",
                                        BindingMode.OneWay, source: WNowViewModel, targetNullValue: false, fallbackValue: false
                                    ),
                                    new Label()
                                    {
                                        Margin = new Thickness(0,0,0,4),
                                        FontAttributes = FontAttributes.Bold,
                                        FontSize = 28,
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        LineBreakMode = LineBreakMode.WordWrap,
                                        MaxLines = 2,
                                        VerticalOptions = LayoutOptions.Center,
                                        VerticalTextAlignment = TextAlignment.Center
                                    }
                                    .Column(1)
                                    .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Location)}",
                                        BindingMode.OneWay, source: WNowViewModel
                                    )
                                    .Bind(Label.TextColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this)
                                }
                            },
                            // UpDate
                            new Border()
                            {
                                Stroke = Colors.Transparent,
                                Margin = new Thickness(0,0,0,4),
                                HorizontalOptions = LayoutOptions.Center,
                                Content = new Label()
                                {
                                    Padding = new Thickness(2),
                                    FontAttributes = FontAttributes.Bold,
                                    FontSize = 12,
                                    HorizontalOptions = LayoutOptions.Center,
                                    HorizontalTextAlignment = TextAlignment.Center
                                }
                                    .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.UpdateDate)}",
                                        BindingMode.OneWay, source: WNowViewModel
                                    )
                                    .Bind(Label.TextColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this)
                            },
                            // Alert Button
                            new Grid()
                            {
                                Margin = new Thickness(0, 5),
                                Padding = new Thickness(11, 5, 11, 6),
                                BackgroundColor = Colors.OrangeRed,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                BindingContext = AlertsView,
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition(GridLength.Auto),
                                    new ColumnDefinition(GridLength.Star),
                                    new ColumnDefinition(new GridLength(50))
                                },
                                Children =
                                {
                                    new Image()
                                    {
                                        Margin = new Thickness(5,2,5,0),
                                        Source = new MaterialIcon(MaterialSymbol.Error)
                                        {
                                            Size = 24,
                                            Color = Colors.White
                                        }
                                    }.Column(0),
                                    new Label()
                                    {
                                        TextColor = Colors.White,
                                        Padding = new Thickness(5, 0),
                                        FontSize = 12,
                                        Text = ResStrings.title_fragment_alerts,
                                        VerticalOptions = LayoutOptions.Center,
                                        VerticalTextAlignment = TextAlignment.Center
                                    }.Column(1),
                                    new Image()
                                    {
                                        Margin = new Thickness(5,0),
                                        HorizontalOptions = LayoutOptions.End,
                                        VerticalOptions = LayoutOptions.Center,
                                        Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                                        {
                                            Size = 24,
                                            Color = Colors.White
                                        }
                                    }.Column(2)
                                }
                            }
                            .Bind(VisualElement.IsVisibleProperty, $"{nameof(AlertsView.Alerts)}",
                                BindingMode.OneWay, source: AlertsView, converter: collectionBooleanConverter as IValueConverter
                            )
                            .TapGesture(GotoAlertsPage),
                            // Spacer
                            new Rectangle()
                            {
                                Margin = new Thickness(8),
                                IsVisible = Utils.FeatureSettings.BackgroundImage && WNowViewModel.ImageData != null
                            }.Apply(it =>
                            {
                                Spacer = it;

                                WNowViewModel.PropertyChanged += (s, e) =>
                                {
                                    if (e.PropertyName == nameof(WNowViewModel.ImageData))
                                    {
                                        it.IsVisible = Utils.FeatureSettings.BackgroundImage && WNowViewModel.ImageData != null;
                                    }
                                };
                            }),
                            // Condition Panel
                            new Grid()
                            {
                                Padding = new Thickness(16, 0),
                                MinimumHeightRequest = 108,
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition(GridLength.Auto),
                                    new ColumnDefinition(GridLength.Auto),
                                    new ColumnDefinition(GridLength.Star),
                                    new ColumnDefinition(GridLength.Auto),
                                },
                                RowDefinitions =
                                {
                                    new RowDefinition(GridLength.Auto),
                                    new RowDefinition(GridLength.Auto),
                                    new RowDefinition(GridLength.Auto),
                                    new RowDefinition(GridLength.Auto),
                                },
                                Children =
                                {
                                    // CurTemp
                                    new Label()
                                    {
                                        Margin = new Thickness(0, 15),
                                        FontFamily = "OpenSansLight",
                                        FontSize = 84,
                                        TextColor = Colors.White,
                                    }
                                    .Column(0)
                                    .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.CurTemp)}",
                                        BindingMode.OneWay, source: WNowViewModel
                                    )
                                    .Apply(it =>
                                    {
                                        CurTemp = it;
                                    }),
                                    // HiLoPanel
                                    new Grid()
                                    {
                                        Padding = new Thickness(25,0,0,0),
                                        HorizontalOptions = LayoutOptions.Center,
                                        VerticalOptions = LayoutOptions.Center,
                                        RowDefinitions =
                                        {
                                            new RowDefinition(GridLength.Star),
                                            new RowDefinition(GridLength.Star),
                                        },
                                        Children =
                                        {
                                            new HorizontalStackLayout()
                                            {
                                                HorizontalOptions = LayoutOptions.End,
                                                VerticalOptions = LayoutOptions.End,
                                                Children =
                                                {
                                                    new Label()
                                                        .Font(size: 24)
                                                        .Height(32)
                                                        .TextCenterHorizontal()
                                                        .CenterVertical()
                                                        .TextCenterVertical()
                                                        .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.HiTemp)}",
                                                            BindingMode.OneWay, source: WNowViewModel, targetNullValue: '\u2022'
                                                        )
                                                        .Bind(Label.TextColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this),
                                                    new IconControl()
                                                    {
                                                        IconColor = Colors.OrangeRed,
                                                        IconProvider = WeatherIconsEFProvider.KEY,
                                                        ShowAsMonochrome = true,
                                                        WeatherIcon = WeatherIcons.DIRECTION_UP,
                                                        IconWidth = 25,
                                                        IconHeight = 25,
                                                        WidthRequest = 25
                                                    }
                                                },
                                            }.Row(0),
                                            new HorizontalStackLayout()
                                            {
                                                HorizontalOptions = LayoutOptions.End,
                                                VerticalOptions = LayoutOptions.Start,
                                                Children =
                                                {
                                                    new Label()
                                                        .Font(size: 24)
                                                        .Height(32)
                                                        .TextCenterHorizontal()
                                                        .CenterVertical()
                                                        .TextCenterVertical()
                                                        .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.LoTemp)}",
                                                            BindingMode.OneWay, source: WNowViewModel, targetNullValue: '\u2022'
                                                        )
                                                        .Bind(Label.TextColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this),
                                                    new IconControl()
                                                    {
                                                        IconColor = Colors.DeepSkyBlue,
                                                        IconProvider = WeatherIconsEFProvider.KEY,
                                                        ShowAsMonochrome = true,
                                                        WeatherIcon = WeatherIcons.DIRECTION_DOWN,
                                                        IconWidth = 25,
                                                        IconHeight = 25,
                                                        WidthRequest = 25
                                                    }
                                                },
                                            }.Row(1),
                                        }
                                    }
                                    .Column(1)
                                    .Bind(VisualElement.HeightRequestProperty, nameof(Height), source: CurTemp),
                                    // WeatherBox
                                    new IconControl()
                                    {
                                        ForceDarkTheme = Utils.FeatureSettings.BackgroundImage,
                                        HeightRequest = 108,
                                        HorizontalOptions = LayoutOptions.End,
                                        VerticalOptions = LayoutOptions.Center,
                                        IconHeight = 108,
                                        IconWidth = 108,
                                        WidthRequest = 108
                                    }
                                    .Column(3)
                                    .Bind(IconControl.WeatherIconProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherIcon)}",
                                        BindingMode.OneWay, source: WNowViewModel
                                    )
                                    .Bind(IconControl.IconColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this)
                                    .Apply(it =>
                                    {
                                        WeatherBox = it;
                                    }),
                                    // CurCondition
                                    new Label()
                                        .Padding(5)
                                        .Font(size: 24)
                                        .Row(1)
                                        .ColumnSpan(4)
                                        .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.CurCondition)}",
                                            BindingMode.OneWay, source: WNowViewModel
                                        )
                                        .Bind(Label.TextColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this),
                                    // Summary
                                    new Label()
                                    {
                                        LineBreakMode = LineBreakMode.WordWrap
                                    }
                                    .Row(2)
                                    .ColumnSpan(5)
                                    .Margin(0, 10)
                                    .Padding(5)
                                    .Font(size: 12)
                                    .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherSummary)}",
                                        BindingMode.OneWay, source: WNowViewModel
                                    )
                                    .Bind(Label.TextColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this),
                                    // Attribution
                                    new Button()
                                        .Row(3)
                                        .ColumnSpan(4)
                                        .End()
                                        .Margin(16,0)
                                        .Padding(5)
                                        .BackgroundColor(Colors.Transparent)
                                        .Font(size: 11)
                                        .Bind(Button.IsVisibleProperty, $"{nameof(WNowViewModel.ImageData)}.{nameof(WNowViewModel.ImageData.OriginalLink)}",
                                            BindingMode.OneWay, source: WNowViewModel, converter: objectBooleanConverter as IValueConverter
                                        )
                                        .Bind(Label.TextColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this)
                                        .Apply(it =>
                                        {
                                            it.Clicked += (s, e) =>
                                            {
                                                WNowViewModel?.ImageData?.OriginalLink?.Let(async uri =>
                                                {
                                                    await Browser.Default.OpenAsync(uri);
                                                });
                                            };

                                            it.Text = $"{ResStrings.attrib_prefix} {WNowViewModel?.ImageData?.ArtistName} ({WNowViewModel?.ImageData?.SiteName})";
                                            WNowViewModel.PropertyChanged += (s, e) =>
                                            {
                                                if (e.PropertyName == nameof(WNowViewModel.ImageData))
                                                {
                                                    it.Text = $"{ResStrings.attrib_prefix} {WNowViewModel?.ImageData?.ArtistName} ({WNowViewModel?.ImageData?.SiteName})";
                                                }
                                            };
                                        }),
                                }
                            }
                            .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Location)}",
                                BindingMode.OneWay, source: WNowViewModel, converter: stringBooleanConverter as IValueConverter
                            )
                        }
                    }
                    .Row(0)
                    .Apply(it =>
                    {
                        ConditionPanelLayout = it;

                        it.SizeChanged += ControlMaxWidth_SizeChanged;
                        it.SizeChanged += (s, e) =>
                        {
                            if (RefreshLayout == null) return;

                            double h = RefreshLayout.Height;

                            if (Spacer != null)
                            {
                                if (Utils.FeatureSettings.BackgroundImage && h > 0)
                                {
                                    Spacer.HeightRequest = h - (ConditionPanelLayout.Height - Spacer.Height);
                                }
                                else
                                {
                                    Spacer.HeightRequest = 0;
                                }
                            }
                        };
                    })
                );
            }

            // Overlay
            ListLayout.Add(new BoxView()
            {
                CornerRadius = new CornerRadius(8,8,0,0),
            }
                .DynamicResource(BoxView.ColorProperty, "RegionColor")
                .Row(1)
            );
        }

        // Add Grid
        var GridLayout = new Grid()
        {
            RowDefinitions =
            {
                // Forecast
                new RowDefinition(GridLength.Auto),
                // Hourly Forecast
                new RowDefinition(GridLength.Auto),
                // Charts
                new RowDefinition(GridLength.Auto),
                // Details
                new RowDefinition(GridLength.Auto),
                // Sun Phase
                new RowDefinition(GridLength.Auto),
                // Radar
                new RowDefinition(GridLength.Auto),
                // Credits
                new RowDefinition(GridLength.Auto),
            }
        }.Paddings(16, 4, 16, 0);
        ListLayout.Add(GridLayout, row: 1);

        // Forecast Panel
        if (Utils.FeatureSettings.Forecast)
        {
            GridLayout.Add(
                new Grid()
                {
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Star),
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    Children =
                    {
                        // Header
                        new Label()
                        {
                            Text = ResStrings.label_forecast,
                        }
                        .Column(0)
                        .Row(0)
                        .AppThemeColorBinding(Label.TextColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel"),
                        new Image()
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                            {
                                Size = 24
                            }.AppThemeColorBinding(MaterialIcon.ColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        }
                        .Column(1)
                        .Row(0),
                        // Content
                        new RangeBarGraphPanel()
                        {

                        }
                        .Bind(RangeBarGraphPanel.ForecastDataProperty, $"{nameof(ForecastView.ForecastGraphData)}",
                                BindingMode.OneWay, source: ForecastView
                        )
                        .Row(1)
                        .ColumnSpan(2)
                        .Apply(it =>
                        {
                            it.GraphViewTapped += (s, e) =>
                            {
                                if (s is IGraph control)
                                {
                                    if (e is TappedEventArgs ev)
                                    {
                                        GotoDetailsPage(false,
                                            control.GetItemPositionFromPoint((float)(ev.GetPosition(control.Control)?.X + control.ScrollViewer.ScrollX)));
                                    }
                                    else
                                    {
                                        GotoDetailsPage(false, 0);
                                    }
                                }
                            };
                        })
                    }
                }
                .Row(0)
                .Margins(bottom: 25)
                .Bind(VisualElement.IsVisibleProperty, $"{nameof(ForecastView.ForecastGraphData)}",
                        BindingMode.OneWay, graphDataConv as IValueConverter, source: ForecastView
                )
                .Apply(it =>
                {
                    it.SizeChanged += ControlMaxWidth_SizeChanged;
                })
            );
        }

        // HourlyForecast Panel
        if (Utils.FeatureSettings.HourlyForecast)
        {
            GridLayout.Add(
                new Grid()
                {
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Star),
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    Children =
                    {
                        // Header
                        new Label()
                        {
                            Text = ResStrings.label_hourlyforecast,
                        }
                        .Column(0)
                        .Row(0)
                        .AppThemeColorBinding(Label.TextColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel"),
                        new Image()
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                            {
                                Size = 24
                            }.AppThemeColorBinding(MaterialIcon.ColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        }
                        .Column(1)
                        .Row(0),
                        // Content
                        new HourlyForecastItemPanel()
                        {

                        }
                        .Bind(HourlyForecastItemPanel.ForecastDataProperty, $"{nameof(ForecastView.HourlyForecastData)}",
                                BindingMode.OneWay, source: ForecastView
                        )
                        .MinHeight(250)
                        .Row(1)
                        .ColumnSpan(2)
                        .Apply(it =>
                        {
                            it.ItemClick += (s, e) =>
                            {
                                AnalyticsLogger.LogEvent("WeatherNow: GraphView_Tapped");
                                GotoDetailsPage(true, it.GetItemPosition(e.Item));
                            };
                        })
                    }
                }
                .Row(1)
                .Margins(bottom: 25)
                .Bind(VisualElement.IsVisibleProperty, $"{nameof(ForecastView.HourlyForecastData)}",
                        BindingMode.OneWay, collectionBooleanConverter as IValueConverter, source: ForecastView
                )
                .Apply(it =>
                {
                    it.SizeChanged += ControlMaxWidth_SizeChanged;
                })
            );
        }

        // Charts
        if (Utils.FeatureSettings.Charts)
        {
            GridLayout.Add(
                new Grid()
                {
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Star),
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    Children =
                    {
                        // Header
                        new Label()
                        {
                            Text = ResStrings.pref_title_feature_charts,
                        }
                        .Column(0)
                        .Row(0)
                        .AppThemeColorBinding(Label.TextColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel"),
                        new Image()
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                            {
                                Size = 24
                            }.AppThemeColorBinding(MaterialIcon.ColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        }
                        .Column(1)
                        .Row(0),
                        // Content
                        new Grid()
                        {
                            RowDefinitions =
                            {
                                new RowDefinition()
                                    .Bind(RowDefinition.HeightProperty, $"{nameof(ForecastView.MinutelyPrecipitationGraphData)}",
                                        BindingMode.OneWay, graphDataGridLengthConv as IValueConverter, source: ForecastView
                                    ),
                                new RowDefinition()
                                    .Bind(RowDefinition.HeightProperty, $"{nameof(ForecastView.HourlyPrecipitationGraphData)}",
                                        BindingMode.OneWay, graphDataGridLengthConv as IValueConverter, source: ForecastView
                                    ),
                            },
                            Children =
                            {
                                // Minutely
                                new ForecastGraphPanel()
                                {
                                    Margin = new Thickness(0, 5)
                                }
                                .Row(0)
                                .Bind(ForecastGraphPanel.GraphDataProperty, $"{nameof(ForecastView.MinutelyPrecipitationGraphData)}",
                                        BindingMode.OneWay, source: ForecastView
                                )
                                .Bind(VisualElement.IsVisibleProperty, $"{nameof(ForecastView.MinutelyPrecipitationGraphData)}",
                                        BindingMode.OneWay, graphDataConv as IValueConverter, source: ForecastView
                                )
                                .Apply(it =>
                                {
                                    it.GraphViewTapped += async (s, e) =>
                                    {
                                        await Navigation.PushAsync(new WeatherChartsPage());
                                    };
                                }),
                                // Hourly
                                new ForecastGraphPanel()
                                {
                                    Margin = new Thickness(0, 5)
                                }
                                .Row(1)
                                .Bind(ForecastGraphPanel.GraphDataProperty, $"{nameof(ForecastView.HourlyPrecipitationGraphData)}",
                                        BindingMode.OneWay, source: ForecastView
                                )
                                .Bind(VisualElement.IsVisibleProperty, $"{nameof(ForecastView.HourlyPrecipitationGraphData)}",
                                        BindingMode.OneWay, graphDataConv as IValueConverter, source: ForecastView
                                )
                                .Apply(it =>
                                {
                                    it.GraphViewTapped += async (s, e) =>
                                    {
                                        await Navigation.PushAsync(new WeatherChartsPage());
                                    };
                                })
                            }
                        }
                        .Row(1)
                        .ColumnSpan(2)
                    }
                }
                .Row(2)
                .Margins(bottom: 25)
                .Bind(VisualElement.IsVisibleProperty, $"{nameof(ForecastView.IsPrecipitationDataPresent)}", BindingMode.OneWay, source: ForecastView)
                .Apply(it =>
                {
                    it.SizeChanged += ControlMaxWidth_SizeChanged;
                })
            );
        }

        // Details
        if (Utils.FeatureSettings.DetailsEnabled)
        {
            GridLayout.Add(
                new Grid()
                {
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Star),
                    },
                    Children =
                    {
                        // Header
                        new Label()
                        {
                            Text = ResStrings.label_details,
                        }
                        .Row(0)
                        .AppThemeColorBinding(Label.TextColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel"),
                    }
                }
                .Row(3)
                .Margins(bottom: 25)
                .Apply(grid =>
                {
                    var detailsStackLayout = new VerticalStackLayout();
                    if (Utils.FeatureSettings.WeatherDetails)
                    {
                        detailsStackLayout.Add(
                            new FlexLayout()
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                Wrap = FlexWrap.Wrap,
                                JustifyContent = FlexJustify.Center,
                            }
                            .Bind(BindableLayout.ItemsSourceProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherDetails)}",
                                    BindingMode.OneWay, detailsFilter as IValueConverter, source: WNowViewModel
                            )
                            .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherDetails)}",
                                    BindingMode.OneWay, collectionBooleanConverter as IValueConverter, source: WNowViewModel
                            )
                            .DynamicResource(BindableLayout.ItemTemplateProperty, "DetailItemTemplate")
                        );
                    }
                    if (Utils.FeatureSettings.ExtraDetailsEnabled)
                    {
                        detailsStackLayout.Add(
                            new FlowLayout()
                            .Apply(detailExtrasLayout =>
                            {
                                if (Utils.FeatureSettings.UV)
                                {
                                    detailExtrasLayout.Add(
                                        new UVControl()
                                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.UVIndex)}",
                                                    BindingMode.OneWay, source: WNowViewModel
                                            )
                                            .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.UVIndex)}",
                                                    BindingMode.OneWay, objectBooleanConverter as IValueConverter, source: WNowViewModel
                                            )
                                    );
                                }

                                if (Utils.FeatureSettings.Beaufort)
                                {
                                    detailExtrasLayout.Add(
                                        new BeaufortControl()
                                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Beaufort)}",
                                                    BindingMode.OneWay, source: WNowViewModel
                                            )
                                            .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Beaufort)}",
                                                    BindingMode.OneWay, objectBooleanConverter as IValueConverter, source: WNowViewModel
                                            )
                                    );
                                }

                                if (Utils.FeatureSettings.AQIndex)
                                {
                                    detailExtrasLayout.Add(
                                        new AQIControl()
                                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.AirQuality)}",
                                                    BindingMode.OneWay, source: WNowViewModel
                                            )
                                            .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.AirQuality)}",
                                                    BindingMode.OneWay, objectBooleanConverter as IValueConverter, source: WNowViewModel
                                            )
                                            .TapGesture(async () =>
                                            {
                                                await Navigation.PushAsync(new WeatherAQIPage());
                                            })
                                    );
                                }

                                if (Utils.FeatureSettings.PollenEnabled)
                                {
                                    detailExtrasLayout.Add(
                                        new PollenCountControl()
                                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Pollen)}",
                                                    BindingMode.OneWay, source: WNowViewModel
                                            )
                                            .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Pollen)}",
                                                    BindingMode.OneWay, objectBooleanConverter as IValueConverter, source: WNowViewModel
                                            )
                                    );
                                }

                                if (Utils.FeatureSettings.MoonPhase)
                                {
                                    detailExtrasLayout.Add(
                                        new MoonPhaseControl()
                                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.MoonPhase)}",
                                                    BindingMode.OneWay, source: WNowViewModel
                                            )
                                            .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.MoonPhase)}",
                                                    BindingMode.OneWay, objectBooleanConverter as IValueConverter, source: WNowViewModel
                                            )
                                    );
                                }
                            })
                        );
                    }

                    grid.Add(detailsStackLayout, row: 1);
                })
                .Apply(it =>
                {
                    it.SizeChanged += ControlMaxWidth_SizeChanged;
                })
            );
        }

        // Sun Phase
        if (Utils.FeatureSettings.SunPhase)
        {
            GridLayout.Add(
                new Grid()
                {
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Star),
                    },
                    Children =
                    {
                        // Header
                        new Label()
                        {
                            Text = ResStrings.label_sunriseset,
                        }
                        .Row(0)
                        .AppThemeColorBinding(Label.TextColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel"),
                        // Content
                        new SunPhaseView()
                            .Row(1)
                            .Bind(
                                VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.SunPhase)}",
                                BindingMode.OneWay, source: WNowViewModel
                            )
                            .CenterHorizontal()
                            .Apply(it =>
                            {
                                it.PropertyChanged += (s, e) =>
                                {
                                    if (e.PropertyName == nameof(it.Height))
                                    {
                                        it.WidthRequest = it.Height * 2;
                                    }
                                };
                                it.WidthRequest = it.Height * 2;
                            })
                    }
                }
                .Row(4)
                .Margins(bottom: 25)
                .Bind(
                    VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.SunPhase)}",
                    BindingMode.OneWay, objectBooleanConverter as IValueConverter, source: WNowViewModel
                )
                .Apply(it =>
                {
                    it.SizeChanged += ControlMaxWidth_SizeChanged;
                })
            );
        }

        if (Utils.FeatureSettings.WeatherRadar)
        {
            GridLayout.Add(
                new Grid()
                {
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Star),
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    Children =
                    {
                        // Header
                        new Label()
                        {
                            Text = ResStrings.label_radar,
                        }
                        .Column(0)
                        .Row(0)
                        .AppThemeColorBinding(Label.TextColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel"),
                        new Image()
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                            {
                                Size = 24
                            }.AppThemeColorBinding(MaterialIcon.ColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                        }
                        .Column(1)
                        .Row(0),
                        // Content
                        new Border()
                        {
                            BackgroundColor = Colors.Transparent,
                            Stroke = Colors.Transparent,
                            HeightRequest = 360,
                            MaximumWidthRequest = 640,
                            ZIndex = 1,
                        }
                        .Row(1)
                        .ColumnSpan(2)
                        .TapGesture(async () =>
                        {
                            await Navigation.PushAsync(new WeatherRadarPage());
                        }),
                        new Border()
                        {
                            HeightRequest = 360,
                            MaximumWidthRequest = 640,
                        }
                        .Row(1)
                        .ColumnSpan(2)
                        .Apply(it =>
                        {
                            RadarWebViewContainer = it;
                            it.Loaded += RadarWebView_Loaded;
                            it.SizeChanged += (s, e) =>
                            {
                                if (it.Width > it.MaximumWidthRequest)
                                {
                                    it.WidthRequest = it.MaximumWidthRequest;
                                }
                            };
                        })
                    }
                }
                .Row(5)
                .Margins(bottom: 25)
                .Apply(it =>
                {
                    it.SizeChanged += ControlMaxWidth_SizeChanged;
                })
            );
        }

        // Weather Credit
        GridLayout.Add(
            new Label()
            {
                Padding = 10,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14
            }.Bind(
                Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherCredit)}",
                BindingMode.OneWay, source: WNowViewModel, fallbackValue: "Data from Weather Provider"
            )
            .Row(6)
        );
    }

    private void ControlMaxWidth_SizeChanged(object sender, EventArgs e)
    {
        if (DeviceInfo.Idiom != DeviceIdiom.Phone)
        {
            if (sender is VisualElement view && view.Width > 1280)
            {
                view.WidthRequest = 1280;
            }
        }
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        radarViewProvider?.OnDestroyView();
        base.OnNavigatingFrom(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs e)
    {
        base.OnNavigatedFrom(e);
        WNowViewModel.PropertyChanged -= WeatherView_PropertyChanged;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);

        AnalyticsLogger.LogEvent("WeatherNow: OnNavigatedTo");

        WNowViewModel.PropertyChanged += WeatherView_PropertyChanged;

        MainViewer.ScrollToAsync(0, 0, false);

        if (UpdateTheme)
        {
            UpdateControlTheme();
            UpdateTheme = false;
        }

        if (ClearGraphIconCache)
        {
            WeatherBox?.UpdateWeatherIcon();
            ClearGraphIconCache = false;
        }

        if (UpdateBindings)
        {
            this.ApplyBindings();
            UpdateBindings = false;
        }

        await Dispatcher.DispatchAsync(async () =>
        {
            await InitializeState();
        });
    }

    private void OnErrorMessage(ErrorMessage error)
    {
        Dispatcher.Dispatch(() =>
        {
            switch (error)
            {
                case ErrorMessage.String err:
                    {
                        ShowSnackbar(Snackbar.MakeError(err.Message, SnackbarDuration.Short));
                    }
                    break;
                case ErrorMessage.WeatherError err:
                    {
                        OnWeatherError(err.Exception);
                    }
                    break;
            }
        });

        WNowViewModel.SetErrorMessageShown(error);
    }

    private void OnWeatherError(WeatherException wEx)
    {
        switch (wEx.ErrorStatus)
        {
            case WeatherUtils.ErrorStatus.NetworkError:
            case WeatherUtils.ErrorStatus.NoWeather:
                // Show error message and prompt to refresh
                Snackbar snackbar = Snackbar.MakeError(wEx.Message, SnackbarDuration.Long);
                snackbar.SetAction(ResStrings.action_retry, () =>
                {
                    WNowViewModel.RefreshWeather(false);
                });
                ShowSnackbar(snackbar);
                break;

            case WeatherUtils.ErrorStatus.QueryNotFound:
                ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                break;

            default:
                // Show error message
                ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                break;
        }
    }

    private async Task<LocationResult> VerifyLocationData()
    {
        var locationData = WNowViewModel.UiState?.LocationData;
        bool locationChanged = false;

        // Check if current location still exists (is valid)
        if (locationData?.locationType == LocationType.Search)
        {
            if (await SettingsManager.GetLocation(locationData?.query).ConfigureAwait(true) == null)
            {
                locationData = null;
                locationChanged = true;
            }
        }

        // Load new favorite location if argument data is present
        if (args == null || args?.IsHome == true)
        {
            // Check if home location changed
            // For ex. due to GPS setting change
            var homeData = await SettingsManager.GetHomeData().ConfigureAwait(true);
            if (!Equals(locationData, homeData))
            {
                locationData = homeData;
                locationChanged = true;
            }
        }
        else if (args?.Location != null && !Equals(locationData, args?.Location))
        {
            locationData = args?.Location;
            locationChanged = true;
        }

        if (locationChanged)
        {
            if (locationData != null)
            {
                return new LocationResult.Changed(locationData);
            }
            else
            {
                return new LocationResult.ChangedInvalid(null);
            }
        }
        else
        {
            return new LocationResult.NotChanged(locationData);
        }
    }

    private async Task InitializeState()
    {
        var result = await VerifyLocationData();

        await result.Data?.Let(async locationData =>
        {
            if (locationData.locationType == LocationType.GPS && SettingsManager.FollowGPS)
            {
                if (!await this.LocationPermissionEnabled())
                {
                    var snackbar = Snackbar.Make(ResStrings.Msg_LocDeniedSettings, SnackbarDuration.Short);
                    snackbar.SetAction(ResStrings.action_settings, async () =>
                    {
                        await this.LaunchLocationSettings();
                    });
                    ShowSnackbar(snackbar);
                    return;
                }
            }
        });

        if (result is LocationResult.Changed || result is LocationResult.ChangedInvalid)
        {
            WNowViewModel.Initialize(result.Data);
        }
        else
        {
            WNowViewModel.RefreshWeather();
        }
    }

    private void RefreshBtn_Clicked(object sender, EventArgs e)
    {
        AnalyticsLogger.LogEvent("WeatherNow: RefreshButton_Click");
        WNowViewModel.RefreshWeather(true);
    }

    private async void GotoAlertsPage()
    {
        AnalyticsLogger.LogEvent("WeatherNow: GotoAlertsPage");
        await Navigation.PushAsync(new WeatherAlertPage());
    }

    private async void GotoDetailsPage(bool IsHourly, int Position)
    {
        await Navigation.PushAsync(new WeatherDetailsPage(new DetailsPageArgs()
        {
            IsHourly = IsHourly,
            ScrollToPosition = Position
        }));
    }

    private void RadarWebView_Loaded(object sender, EventArgs e)
    {
        var container = sender as Border;
        var cToken = GetCancellationToken();

        AsyncTask.Run(async () =>
        {
            await Dispatcher.DispatchAsync(() =>
            {
                if (radarViewProvider == null)
                {
                    radarViewProvider = RadarProvider.GetRadarViewProvider(container);
                }
                radarViewProvider.EnableInteractions(false);
                WNowViewModel.Weather?.Let(it =>
                {
                    radarViewProvider?.UpdateCoordinates(it.LocationCoord, true);
                });
            });
        }, 1000, cToken);
    }

    private void RadarProvider_RadarProviderChanged(RadarProviderChangedEventArgs e)
    {
        if (Utils.FeatureSettings.WeatherRadar && RadarWebViewContainer != null)
        {
            radarViewProvider?.OnDestroyView();
            radarViewProvider = RadarProvider.GetRadarViewProvider(RadarWebViewContainer);
        }
    }

    private void WeatherNow_Loaded(object sender, EventArgs e)
    {
        OnAppThemeChanged(App.Current.CurrentTheme);
        App.Current.RequestedThemeChanged += WeatherNow_RequestedThemeChanged;
    }

    private void WeatherNow_Unloaded(object sender, EventArgs e)
    {
        App.Current.RequestedThemeChanged -= WeatherNow_RequestedThemeChanged;
    }

    private void WeatherNow_RequestedThemeChanged(object sender, AppThemeChangedEventArgs args)
    {
        OnAppThemeChanged(args.RequestedTheme);
    }

    private void OnAppThemeChanged(AppTheme requestedTheme)
    {
        var isLight = requestedTheme switch
        {
            AppTheme.Light => true,
            AppTheme.Dark => false,
            _ => !App.Current.IsSystemDarkTheme, // AppTheme.Unspecified
        };

        ForecastView.IsLight = isLight;
    }

    private void UpdateControlTheme()
    {
        UpdateControlTheme(Utils.FeatureSettings.BackgroundImage);
    }

    private void UpdateControlTheme(bool backgroundEnabled)
    {
        if (backgroundEnabled)
        {
            if (GradientOverlay != null)
            {
                GradientOverlay.IsVisible = true;
            }
            ConditionPanelTextColor = Colors.White;
            if (CurTemp != null)
            {
                CurTemp.TextColor = GetTempColor();
            }
        }
        else
        {
            if (GradientOverlay != null)
            {
                GradientOverlay.IsVisible = false;
            }
            this.SetAppThemeColor(ConditionPanelTextColorProperty, Colors.Black, Colors.White);
            if (CurTemp != null)
            {
                CurTemp.TextColor = GetTempColor();
            }
        }
    }

    private Color GetTempColor()
    {
        string temp = WNowViewModel?.Weather?.CurTemp;
        string temp_str = temp?.RemoveNonDigitChars();

        if (float.TryParse(temp_str, out float temp_f))
        {
            var tempUnit = SettingsManager.TemperatureUnit;

            if (Equals(tempUnit, Units.CELSIUS) || temp.EndsWith(Units.CELSIUS))
            {
                temp_f = ConversionMethods.CtoF(temp_f);
            }

            var color = WeatherUtils.GetColorFromTempF(temp_f, Colors.Transparent);

            if (color != Colors.Transparent)
            {
                return color;
            }
        }

        return ConditionPanelTextColor;
    }

    private void BackgroundOverlay_Loaded(object sender, EventArgs e)
    {
        var bgImage = sender as Image;
        bgImage.PropertyChanged += BgImage_PropertyChanged;
        UpdateControlTheme(bgImage.Source != null);
    }

    private void BackgroundOverlay_Unloaded(object sender, EventArgs e)
    {
        var bgImage = sender as Image;
        bgImage.PropertyChanged -= BgImage_PropertyChanged;
    }

    private void BgImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var bgImage = sender as Image;
        if (e.PropertyName == nameof(bgImage.Source))
        {
            UpdateControlTheme(bgImage.Source != null);
        }
    }
}