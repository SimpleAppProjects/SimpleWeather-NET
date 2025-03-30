using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
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
using SimpleWeather.Maui.Converters;
using SimpleWeather.Maui.DataBinding;
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
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Timers;
using UIKit;
using FeatureSettings = SimpleWeather.NET.Utils.FeatureSettings;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Main;

public partial class WeatherNow
{
    // Views
    private Grid MainGrid { get; set; }
    private RowDefinition BannerRow { get; set; }
    private RefreshView RefreshLayout { get; set; }
    private ScrollView MainViewer { get; set; }
    private Grid ListLayout { get; set; }
    private Grid GridLayout { get; set; }
    private ActivityIndicator ContentRing { get; set; }
    private Grid BannerContainer { get; set; }
    private Grid SnackbarContainer { get; set; }
    private Label CurTemp { get; set; }
    private IconControl WeatherBox { get; set; }
    private Border RadarWebViewContainer { get; set; }
    private VisualElement GradientOverlay { get; set; }
    private VisualElement HourlyForecastPanel { get; set; }

    // Resources
    private readonly DetailsItemGridFilterConverter detailsFilter = new();
    private readonly GraphDataGridLengthConverter graphDataGridLengthConv = new();
    private readonly GraphDataVisibilityConverter graphDataConv = new();

    private readonly HashSet<VisualElement> ResizeElements = new();

    private void Initialize()
    {
        App.Current.Resources.TryGetValue("inverseBoolConverter", out var inverseBoolConverter);
        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);

        this.Content = MainGrid = new Grid()
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                // BannerRow
                new RowDefinition(GridLength.Auto)
                    .Let(it => BannerRow = it),
                new RowDefinition(GridLength.Star),
            },
            Children =
            {
                // RefreshLayout
                new RefreshView()
                {
                    Content = new ScrollView()
                    {
                        Orientation = ScrollOrientation.Vertical,
                        Content = new Grid()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            RowDefinitions =
                            {
                                new RowDefinition(GridLength.Auto),
                                new RowDefinition(GridLength.Auto),
                            }
                        }
                        .Apply(it =>
                        {
                            it.SizeChanged += ListLayout_SizeChanged;

                            ListLayout = it;
                        })
                    }
                    .Bind(ScrollView.IsVisibleProperty, static src => src.Weather, mode: BindingMode.OneWay, source: WNowViewModel, convert: (model) => model?.Location != null)
                    .Apply(it =>
                    {
                        MainViewer = it;
                    })
                }
                .AppThemeBinding(RefreshView.RefreshColorProperty, LightPrimary, DarkPrimary)
                .Bind(RefreshView.IsRefreshingProperty, static src => src.UiState, mode: BindingMode.OneWay, source: WNowViewModel,
                    convert: (uiState) => uiState?.IsLoading ?? true, targetNullValue: true, fallbackValue: true
                )
                .Bind(RefreshView.IsVisibleProperty, static src => src.UiState, mode: BindingMode.OneWay, source: WNowViewModel,
                    convert: WeatherNowBinding.IsViewVisible
                )
                .Row(2)
                .Apply(it =>
                {
                    it.Refreshing += RefreshBtn_Clicked;

                    RefreshLayout = it;
                }),
                // ContentRing
                new ActivityIndicator()
                .RowSpan(3)
                .Center()
                .Bind(ActivityIndicator.IsRunningProperty, static src => src.UiState, mode: BindingMode.OneWay, source: WNowViewModel,
                    convert: WeatherNowBinding.IsLoadingRingActive, targetNullValue: true, fallbackValue: true
                )
                .Apply(it =>
                {
                    ContentRing = it;
                }),
                // BannerContainer
                new Grid()
                {
                    VerticalOptions = LayoutOptions.Start,
                    ZIndex = 0
                }
                .Row(1)
                .Apply(it =>
                {
                    BannerContainer = it;
                }),
                // SnackbarContainer
                new Grid()
                {
                    VerticalOptions = LayoutOptions.End,
                    ZIndex = 1
                }
                .Row(0)
                .RowSpan(3)
                .Apply(it =>
                {
                    SnackbarContainer = it;
                })
            }
        };
    }

    private void InitControls()
    {
        // Refresh toolbar item
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            ToolbarItems.Add(CreateRefreshToolbarButton());
        }

        // Condition Panel
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                this.Title = null;
                Shell.SetTitleView(this, CreateClockToolbar());

                ListLayout.Add(
                    CreateMobileConditionPanel()
                    .Row(0)
                );
            }
            else
            {
                if (FeatureSettings.BackgroundImage)
                {
                    MainGrid.Children.Insert(0,
                        new Image()
                        {
                            Aspect = Aspect.AspectFill,
                            IsVisible = FeatureSettings.BackgroundImage
                        }
                        .Bind(Image.SourceProperty, static src => src.ImageData, convert: imageData => imageData?.ImageSource, mode: BindingMode.OneWay, source: WNowViewModel)
                        .Row(0)
                        .RowSpan(3)
                        .Apply(it =>
                        {
                            it.Loaded += BackgroundOverlay_Loaded;
                            it.PropertyChanged += BackgroundOverlay_PropertyChanged;
                            it.PropertyChanging += BackgroundOverlay_PropertyChanging;
                        })
                    );

                    MainGrid.Insert(1,
                        new Grid()
                        {
                            IsVisible = FeatureSettings.BackgroundImage,
                            Background = new LinearGradientBrush(
                                new GradientStopCollection()
                                {
                                    new GradientStop(Color.FromRgba(0, 0, 0, 0x50), 0),
                                    new GradientStop(Color.FromRgba(0, 0, 0, 0xFF), 1),
                                },
                                new Point(0.5, 0),
                                new Point(0.5, 1)
                            )
                        }
                        .Row(0)
                        .RowSpan(3)
                        .Apply(it => GradientOverlay = it)
                    );
                }

                ListLayout.Add(
                    CreateDesktopConditionPanel()
                    .Row(0)
                );
            }
        }

        // Add Grid
        GridLayout = new Grid()
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
                // UV
                new RowDefinition(GridLength.Auto),
                // Beaufort
                new RowDefinition(GridLength.Auto),
                // AQIndex
                new RowDefinition(GridLength.Auto),
                // PollenEnabled
                new RowDefinition(GridLength.Auto),
                // MoonPhase
                new RowDefinition(GridLength.Auto),
                // Sun Phase
                new RowDefinition(GridLength.Auto),
                // Radar
                new RowDefinition(GridLength.Auto),
                // Credits
                new RowDefinition(GridLength.Auto),
            },
            Children =
            {
                // Forecast Panel
                CreateForecastPanel()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_FORECAST)
                    .Row(0),
                // HourlyForecast Panel
                CreateHourlyForecastPanel()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_HRFORECAST)
                    .Row(1),
                // Charts
                CreateChartsPanel()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_CHARTS)
                    .Row(2),
                // Details
                CreateDetailsPanel()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_WEATHERDETAILS)
                    .Row(3),
                // UV
                CreateUVControl()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_UVINDEX)
                    .Row(4),
                // Beaufort
                CreateBeaufortControl()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_BEAUFORT)
                    .Row(5),
                // AQIndex
                CreateAQIControl()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_AQINDEX)
                    .Row(6),
                // PollenEnabled
                CreatePollenCountControl()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_POLLEN)
                    .Row(7),
                // MoonPhase
                CreateMoonPhaseControl()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_MOONPHASE)
                    .Row(8),
                // Sun Phase
                CreateSunPhasePanel()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_SUNPHASE)
                    .Row(9),
                // Radar
                CreateRadarPanel()
                    .Apply(it => it.StyleId = FeatureSettings.KEY_RADAR)
                    .Row(10),
                // Weather Credit
                CreateWeatherCredit()
                    .Row(11)
            }
        }.Apply(it =>
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                it.Padding(0);
            else
                it.Paddings(16, 4, 16, 0);
        });
        var GridContainer = new Border()
        {
            Content = GridLayout,
            Stroke = null,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle()
            {
                CornerRadius = new CornerRadius(8, 8, 0, 0)
            }
        }.Apply(it =>
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                it.BackgroundColor = Colors.Transparent;
            }
            else
            {
                it.DynamicResource(Border.BackgroundColorProperty, "RegionColor");
            }
        });
        ListLayout.Add(GridContainer, row: 1);

        AdjustViewsLayout(0);
    }

    private void UpdateViewOrder()
    {
        var orderableFeatures = FeatureSettings.GetFeatureOrder();

        var index = 0;

        if (orderableFeatures?.Any() == true)
        {
            var featureViewMap = orderableFeatures.ToDictionary(f => f, f =>
            {
                return GridLayout.Cast<Element>().First(v => v.StyleId == f);
            });

            orderableFeatures.ForEach(feature =>
            {
                featureViewMap[feature].Row(index++);
            });
        }
        else
        {
            // Use default order
            GridLayout.Cast<View>().ForEach(v =>
            {
                v.Row(index++);
            });
        }
    }

    private ToolbarItem CreateRefreshToolbarButton()
    {
        App.Current.Resources.TryGetValue("inverseBoolConverter", out var inverseBoolConverter);
        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);

        return new ToolbarItem()
        {
            Text = ResStrings.action_refresh,
            IconImageSource = new MaterialIcon(MaterialSymbol.Refresh)
            {
                Size = 24,
                FontAutoScalingEnabled = true
            }.AppThemeColorBinding(MaterialIcon.ColorProperty, LightPrimary as Color, DarkPrimary as Color),
            Order = ToolbarItemOrder.Primary,
        }
        .Bind(ToolbarItem.IsEnabledProperty, static layout => !layout.IsRefreshing, mode: BindingMode.OneWay, source: RefreshLayout)
        .Apply(it =>
        {
            it.Clicked += RefreshBtn_Clicked;
        });
    }

    private View CreateClockToolbar()
    {
        App.Current.Resources.TryGetValue("LightOnSurface", out var LightOnSurface);
        App.Current.Resources.TryGetValue("DarkOnSurface", out var DarkOnSurface);

        return new AbsoluteLayout()
        {
            HeightRequest = 64,
            Children =
            {
                new Image()
                {
                    Source = new MaterialIcon(MaterialSymbol.Place)
                        .AppThemeColorBinding(MaterialIcon.ColorProperty, LightOnSurface as Color, DarkOnSurface as Color),
                    Aspect = Aspect.AspectFit,
                    HeightRequest = 24,
                    WidthRequest = 24,
                    Margin = new Thickness(16, 0)
                }
                .Bind(VisualElement.IsVisibleProperty, getter: (viewModel) => viewModel.UiState, convert: (uiState) => uiState?.IsGPSLocation ?? false,
                    mode: BindingMode.OneWay, source: WNowViewModel, targetNullValue: false, fallbackValue: false
                )
                .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                .LayoutBounds(0, 0.5),
                new VerticalStackLayout()
                {
                    Margin = new Thickness(40, 0),
                    Children =
                    {
                        // Location Name
                        new Label()
                        {
                            HorizontalOptions = LayoutOptions.Fill,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.End,
                            VerticalTextAlignment = TextAlignment.End,
                            MaxLines = 1,
                            LineBreakMode = LineBreakMode.TailTruncation,
                            FontSize = 22,
                            CharacterSpacing = 0,
                            Padding = new Thickness(8, 0)
                        }
                        .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.Location,
                            mode: BindingMode.OneWay, source: WNowViewModel, fallbackValue: ResStrings.label_nav_weathernow, targetNullValue: ResStrings.label_nav_weathernow
                        )
                        .AppThemeColorBinding(Label.TextColorProperty, LightOnSurface as Color, DarkOnSurface as Color),
                        // Clock
                        new Label()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            MaxLines = 1,
                            LineBreakMode = LineBreakMode.TailTruncation,
                            FontFamily = "OpenSansSemibold",
                            FontSize = 14,
                            CharacterSpacing = 0.00714286,
                            Text = DateTime.Now.ToString("t")
                        }
                        .AppThemeColorBinding(Label.TextColorProperty, LightOnSurface as Color, DarkOnSurface as Color)
                        .Apply(it =>
                        {
                            var timer = new System.Timers.Timer();
                            timer.AutoReset = false;

                            double GetInterval()
                            {
                                DateTime now = DateTime.Now;
                                return (60 - now.Second) * 1000 - now.Millisecond;
                            }

                            void UpdateClock()
                            {
                                var locale = LocaleUtils.GetLocale();
                                var tz = WNowViewModel?.UiState?.LocationData?.tz_offset;
                                var now = DateTimeOffset.Now;
                                string tzs;

                                if (tz.HasValue)
                                {
                                    now = now.ToOffset(tz.Value);
                                    tzs = WNowViewModel.UiState.LocationData.tz_short;
                                }
                                else
                                {
                                    tzs = now.ToString("zzz", locale);
                                }

                                it.Dispatcher.Dispatch(() =>
                                {
                                    it.Text = $"{now.ToString("t", locale)} {tzs}";
                                });
                            }

                            timer.Elapsed += (s, e) =>
                            {
                                timer.Interval = GetInterval();
                                timer.Start();
                                UpdateClock();
                            };

                            void WNow_PropertyChanged(object s, PropertyChangedEventArgs e)
                            {
                                if (e.PropertyName == nameof(WNowViewModel.Weather))
                                {
                                    UpdateClock();
                                }
                            }

                            it.ParentChanged += (s, e) =>
                            {
                                if (it.Parent != null && !timer.Enabled)
                                {
                                    timer.Interval = GetInterval();
                                    timer.Start();
                                    WNowViewModel.PropertyChanged += WNow_PropertyChanged;
                                }
                                else if (it.Parent == null)
                                {
                                    timer.Stop();
                                    WNowViewModel.PropertyChanged -= WNow_PropertyChanged;
                                }
                            };

                            if (it.IsLoaded)
                            {
                                timer.Interval = GetInterval();
                                timer.Start();
                            }
                        })
                    }
                }
                .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                .LayoutBounds(0.5, 0.5)
                .Bind(
                    VerticalStackLayout.MarginProperty, getter: static viewModel => viewModel.UiState,
                    mode: BindingMode.OneWay, convert: (uiState) =>
                    {
                        return uiState?.IsGPSLocation == true ? new Thickness(40, 0) : Thickness.Zero;
                    }, source: WNowViewModel, targetNullValue: new Thickness(40, 0), fallbackValue: new Thickness(40, 0))
            }
        };
    }

    private VerticalStackLayout CreateMobileConditionPanel()
    {
        App.Current.Resources.TryGetValue("objectBooleanConverter", out var objectBooleanConverter);
        App.Current.Resources.TryGetValue("stringBooleanConverter", out var stringBooleanConverter);
        App.Current.Resources.TryGetValue("collectionBooleanConverter", out var collectionBooleanConverter);

        this.SetAppThemeColor(ConditionPanelTextColorProperty, Colors.Black, Colors.White);

        return new VerticalStackLayout()
        {
            Children =
            {
                // Alert button
                CreateAlertButton(),
                // Image
                new AspectRatioFrame()
                {
                    Margin = new Thickness(8, 4, 8, 8),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = 0,
                    HasShadow = false,
                    CornerRadius = 8,
                    IsClippedToBounds = true,
                    BorderColor = Colors.Transparent,
                    Background = Colors.Transparent,
                    Content = new Grid()
                    {
                        Children =
                        {
                            new Image()
                            {
                                Aspect = Aspect.AspectFill
                            }
                            .Bind(Image.SourceProperty, static viewModel => viewModel.ImageData, convert: imageData => imageData?.ImageSource,
                                mode: BindingMode.OneWay, source: WNowViewModel
                            ),
                            new Label()
                                .End()
                                .Bottom()
                                .Margin(16,4)
                                .Padding(0)
                                .BackgroundColor(Colors.Transparent)
                                .Font(size: 11)
                                .TextColor(Colors.White)
                                .Text($"{ResStrings.attrib_prefix} {WNowViewModel?.ImageData?.ArtistName} ({WNowViewModel?.ImageData?.SiteName})")
                                .Behaviors(new TouchBehavior()
                                {
                                    PressedOpacity = 0.75
                                })
                                .TapGesture(() =>
                                {
                                    WNowViewModel?.ImageData?.OriginalLink?.Let(async uri =>
                                    {
                                        await Browser.Default.OpenAsync(uri);
                                    });
                                })
                                .Apply(it =>
                                {
                                    it.IsVisible = WNowViewModel.ImageData != null;

                                    WNowViewModel.PropertyChanged += (s, e) =>
                                    {
                                        if (e.PropertyName == nameof(WNowViewModel.ImageData))
                                        {
                                            it.IsVisible = WNowViewModel.ImageData != null;
                                            it.Text = $"{ResStrings.attrib_prefix} {WNowViewModel?.ImageData?.ArtistName} ({WNowViewModel?.ImageData?.SiteName})";
                                        }
                                    };
                                })
                        }
                    }
                }
                .Apply(it =>
                {
                    it.Loaded += (s, e) =>
                    {
                        it.IsVisible = FeatureSettings.BackgroundImage && WNowViewModel.ImageData != null;
                    };

                    WNowViewModel.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(WNowViewModel.ImageData))
                        {
                            it.IsVisible = FeatureSettings.BackgroundImage && WNowViewModel.ImageData != null;
                        }
                    };

                    void UpdateSize()
                    {
                        var displayInfo = DeviceDisplay.MainDisplayInfo;
                        var heightDp = displayInfo.Height / displayInfo.Density;

                        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
                        {
                            it.MaximumHeightRequest = 420;
                        }
                        else if (displayInfo.Orientation == DisplayOrientation.Landscape)
                        {
                            it.MaximumHeightRequest = Math.Min(480, MainGrid.Height - (110 * displayInfo.Density));
                        }
                        else
                        {
                            it.MaximumHeightRequest = Math.Min(420, MainGrid.Height - (110 * displayInfo.Density));
                        }

                        it.EnableAspectRatio = true;

                        if (heightDp >= 800)
                        {
                            it.AspectRatio = 1.0f;
                        }
                        else if (heightDp >= 720)
                        {
                            it.AspectRatio = 1.25f;
                        }
                        else
                        {
                            it.AspectRatio = 1.5f;
                        }
                    }

                    MainGrid.SizeChanged += (s, e) =>
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            UpdateSize();
                        });
                    };

                    UpdateSize();
                }),
                // Condition Panel
                new Grid()
                {
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
                        new RowDefinition(GridLength.Star),
                        new RowDefinition(GridLength.Auto),
                    },
                    Children =
                    {
                        // CurTemp
                        new Label()
                        {
                            Margin = new Thickness(16, 0),
                            FontFamily = "OpenSansLight",
                            FontSize = 60,
                        }
                        .Column(0)
                        .Bind(Label.TextProperty, getter: viewModel => viewModel.Weather, convert: weather => weather?.CurTemp,
                            mode: BindingMode.OneWay, source: WNowViewModel
                        )
                        .Bind(Label.TextColorProperty, getter: viewModel => viewModel.Weather,
                            mode: BindingMode.OneWay, source: WNowViewModel, convert: (p) => GetTempColor()
                        )
                        .Apply(it =>
                        {
                            CurTemp = it;
                        }),
                        // HiLoPanel
                        new Grid()
                        {
                            Padding = new Thickness(6,0,6,0),
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
                                            .Font(size: 20)
                                            .Height(30)
                                            .TextCenterHorizontal()
                                            .CenterVertical()
                                            .TextCenterVertical()
                                            .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.HiTemp,
                                                mode: BindingMode.OneWay, source: WNowViewModel, targetNullValue: "\u2022"
                                            )
                                            .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                                        new IconControl()
                                        {
                                            IconColor = Colors.OrangeRed,
                                            IconProvider = WeatherIconsEFProvider.KEY,
                                            ShowAsMonochrome = true,
                                            WeatherIcon = WeatherIcons.DIRECTION_UP,
                                            IconWidth = 25,
                                            IconHeight = 25
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
                                            .Font(size: 20)
                                            .Height(30)
                                            .TextCenterHorizontal()
                                            .CenterVertical()
                                            .TextCenterVertical()
                                            .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.LoTemp,
                                                mode: BindingMode.OneWay, source: WNowViewModel, targetNullValue: "\u2022"
                                            )
                                            .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                                        new IconControl()
                                        {
                                            IconColor = Colors.DeepSkyBlue,
                                            IconProvider = WeatherIconsEFProvider.KEY,
                                            ShowAsMonochrome = true,
                                            WeatherIcon = WeatherIcons.DIRECTION_DOWN,
                                            IconWidth = 25,
                                            IconHeight = 25
                                        }
                                    },
                                }.Row(1),
                            }
                        }
                        .Column(1)
                        .Bind(VisualElement.HeightRequestProperty, static temp => temp.Height, source: CurTemp),
                        // WeatherBox
                        new IconControl()
                        {
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.Center,
                            IconHeight = 88,
                            IconWidth = 88,
                        }
                        .Margins(right: 16)
                        .Column(3)
                        .Bind(IconControl.WeatherIconProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.WeatherIcon,
                            mode: BindingMode.OneWay, source: WNowViewModel
                        )
                        .AppThemeColorBinding(IconControl.IconColorProperty, Colors.Black, Colors.White)
                        .Apply(it =>
                        {
                            WeatherBox = it;
                        }),
                        // CurCondition
                        new Label()
                            .Margins(16, 0, 16, 4)
                            .Padding(4)
                            .Font(size: 18)
                            .Row(1)
                            .ColumnSpan(4)
                            .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.CurCondition,
                                mode: BindingMode.OneWay, source: WNowViewModel
                            )
                            .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                        // FeelsLike
                        new Label()
                        {
                            LineBreakMode = LineBreakMode.TailTruncation,
                            MaxLines = 2
                        }
                            .Margins(16, 0, 16, 0)
                            .Opacity(0.75)
                            .Padding(4)
                            .Font(size: 16)
                            .Row(2)
                            .ColumnSpan(4)
                            .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.FeelsLike,
                                mode: BindingMode.OneWay, source: WNowViewModel
                            )
                            .Bind(Label.IsVisibleProperty, getter: static viewModel => viewModel.Weather,
                                mode: BindingMode.OneWay, source: WNowViewModel, targetNullValue: false, fallbackValue: false,
                                convert: (weather) => !string.IsNullOrWhiteSpace(weather?.FeelsLike)
                            )
                            .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                        // Summary
                        new Label()
                        {
                            LineBreakMode = LineBreakMode.TailTruncation,
                            MaxLines = 3
                        }
                        .Row(3)
                        .ColumnSpan(5)
                        .Margin(16, 4)
                        .Padding(4)
                        .Font(size: 12)
                        .Opacity(0.75)
                        .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.WeatherSummary,
                            mode: BindingMode.OneWay, source: WNowViewModel
                        )
                        .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White)
                        .Apply(it =>
                        {
                            it.Loaded += (s, e) =>
                            {
                                it.IsVisible = FeatureSettings.WeatherSummary;
                            };

                            it.TapGesture(async () =>
                            {
                                if (it.IsTextTruncated())
                                {
                                    await this.DisplayAlert(
                                        title: ResStrings.pref_title_feature_summary,
                                        message: it.Text,
                                        cancel: ResStrings.ConfirmDialog_PrimaryButtonText);
                                }
                            });
                        }),
                        // Update time
                        new Label()
                        .Row(4)
                        .ColumnSpan(5)
                        .Margins(16, 4, 16, 8)
                        .Padding(4)
                        .Font(size: 12)
                        .Opacity(0.75)
                        .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.UpdateDate,
                            mode: BindingMode.OneWay, source: WNowViewModel
                        )
                        .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                    }
                }
            }
        };
    }

    private VerticalStackLayout CreateDesktopConditionPanel()
    {
        View ConditionPanelLayout = null;
        View Spacer = null;

        return new VerticalStackLayout()
        {
            Children =
            {
                // GPSIcon + Location
                new Grid()
                {
                    Margin = new Thickness(5,10,5,5),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto)
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
                                    .Bind(FontImageSource.ColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this)
                            },
                            Stroke = new SolidColorBrush(Colors.Transparent)
                        }
                        .Column(0)
                        .Bind(VisualElement.IsVisibleProperty, getter: (viewModel) => viewModel.UiState, convert: (uiState) => uiState?.IsGPSLocation ?? false,
                            mode: BindingMode.OneWay, source: WNowViewModel, targetNullValue: false, fallbackValue: false
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
                        .Bind(Label.TextProperty, static viewModel => viewModel.Weather, convert: weather => weather?.Location,
                            mode: BindingMode.OneWay, source: WNowViewModel
                        )
                        .Bind(Label.TextColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this)
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
                        .Bind(Label.TextProperty, static viewModel => viewModel.Weather, convert: weather => weather?.UpdateDate,
                            mode: BindingMode.OneWay, source: WNowViewModel
                        )
                        .Bind(Label.TextColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this)
                },
                // Alert Button
                CreateAlertButton(),
                // Spacer
                new Rectangle()
                .Margin(new Thickness(8))
                .Bind(IsVisibleProperty, static viewModel => viewModel.ImageData,
                    convert: imageData => FeatureSettings.BackgroundImage && imageData != null,
                    source: WNowViewModel)
                .Apply(it =>
                {
                    Spacer = it;
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
                        .Bind(Label.TextProperty, getter: viewModel => viewModel.Weather, convert: weather => weather?.CurTemp,
                            mode: BindingMode.OneWay, source: WNowViewModel
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
                                            .Bind(Label.TextProperty, getter: viewModel => viewModel.Weather, convert: weather => weather?.HiTemp,
                                                mode: BindingMode.OneWay, source: WNowViewModel, targetNullValue: "\u2022"
                                            )
                                            .Bind(Label.TextColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this),
                                        new IconControl()
                                        {
                                            IconColor = Colors.OrangeRed,
                                            IconProvider = WeatherIconsEFProvider.KEY,
                                            ShowAsMonochrome = true,
                                            WeatherIcon = WeatherIcons.DIRECTION_UP,
                                            IconWidth = 25,
                                            IconHeight = 25
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
                                            .Bind(Label.TextProperty, getter: viewModel => viewModel.Weather, convert: weather => weather?.LoTemp,
                                                mode: BindingMode.OneWay, source: WNowViewModel, targetNullValue: "\u2022"
                                            )
                                            .Bind(Label.TextColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this),
                                        new IconControl()
                                        {
                                            IconColor = Colors.DeepSkyBlue,
                                            IconProvider = WeatherIconsEFProvider.KEY,
                                            ShowAsMonochrome = true,
                                            WeatherIcon = WeatherIcons.DIRECTION_DOWN,
                                            IconWidth = 25,
                                            IconHeight = 25
                                        }
                                    },
                                }.Row(1),
                            }
                        }
                        .Column(1)
                        .Bind(VisualElement.HeightRequestProperty, static temp => temp.Height, source: CurTemp),
                        // WeatherBox
                        new IconControl()
                        {
                            ForceDarkTheme = FeatureSettings.BackgroundImage,
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.Center,
                            IconHeight = 108,
                            IconWidth = 108,
                        }
                        .Column(3)
                        .Bind(IconControl.WeatherIconProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.WeatherIcon,
                            mode: BindingMode.OneWay, source: WNowViewModel
                        )
                        .Bind(IconControl.IconColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this)
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
                            .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.CurCondition,
                                mode: BindingMode.OneWay, source: WNowViewModel
                            )
                            .Bind(Label.TextColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this),
                        // FeelsLike
                        new Label()
                            {
                                LineBreakMode = LineBreakMode.WordWrap,
                            }
                            .Margin(0, 5)
                            .Padding(5)
                            .Font(size: 14)
                            .Row(2)
                            .ColumnSpan(4)
                            .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.FeelsLike,
                                mode: BindingMode.OneWay, source: WNowViewModel
                            )
                            .Bind(Label.IsVisibleProperty, getter: static viewModel => viewModel.Weather,
                                mode: BindingMode.OneWay, source: WNowViewModel, targetNullValue: false, fallbackValue: false,
                                convert: (weather) => !string.IsNullOrWhiteSpace(weather?.FeelsLike)
                            )
                            .Bind(Label.TextColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this),
                        // Summary
                        new Label()
                        {
                            LineBreakMode = LineBreakMode.WordWrap
                        }
                        .Row(3)
                        .ColumnSpan(4)
                        .Margin(0, 10)
                        .Padding(5)
                        .Font(size: 12)
                        .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather, convert: weather => weather?.WeatherSummary,
                            mode: BindingMode.OneWay, source: WNowViewModel
                        )
                        .Bind(Label.TextColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this)
                        .Apply(it =>
                        {
                            it.Loaded += (s, e) =>
                            {
                                it.IsVisible = FeatureSettings.WeatherSummary;
                            };
                        }),
                        // Attribution
                        new Label()
                            .Row(4)
                            .ColumnSpan(4)
                            .End()
                            .Margin(16,4)
                            .Padding(5)
                            .BackgroundColor(Colors.Transparent)
                            .Font(size: 11)
                            .Bind(Label.IsVisibleProperty, static viewModel => viewModel.ImageData,
                                mode: BindingMode.OneWay, source: WNowViewModel, convert: imageData => FeatureSettings.BackgroundImage && imageData?.OriginalLink != null,
                                targetNullValue: false, fallbackValue: false
                            )
                            .Bind(Label.TextProperty, static viewModel => viewModel.ImageData,
                                mode: BindingMode.OneWay, source: WNowViewModel,
                                convert: imageData => $"{ResStrings.attrib_prefix} {imageData?.ArtistName} ({imageData?.SiteName})",
                                targetNullValue: null, fallbackValue: null
                            )
                            .Bind(Label.TextColorProperty, static src => src.ConditionPanelTextColor, mode: BindingMode.OneWay, source: this)
                            .TapGesture(() =>
                            {
                                WNowViewModel?.ImageData?.OriginalLink?.Let(async uri =>
                                {
                                    await Browser.Default.OpenAsync(uri);
                                });
                            })
                            .Behaviors(new TouchBehavior()
                            {
                                PressedOpacity = 0.75
                            })
                            .Apply(it =>
                            {
                                it.LineBreakMode = LineBreakMode.NoWrap;
                            })
                    }
                }
            }
        }
        .Bind(VisualElement.IsVisibleProperty, static viewModel => viewModel.Weather,
            mode: BindingMode.OneWay, source: WNowViewModel, convert: weather => !string.IsNullOrWhiteSpace(weather?.Location)
        )
        .Padding(16, 0)
        .Apply(it =>
        {
            ConditionPanelLayout = it;

            ResizeElements.Add(it);
            it.SizeChanged += (s, e) =>
            {
                if (RefreshLayout == null) return;

                double h = RefreshLayout.Height;

                if (Spacer != null)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        if (FeatureSettings.BackgroundImage && h > 0)
                        {
                            Spacer.HeightRequest = h - (ConditionPanelLayout.Height - Spacer.Height);
                        }
                        else
                        {
                            Spacer.HeightRequest = 0;
                        }
                    });
                }
            };
        });
    }

    private static Frame CreateWeatherNowFrame(View content)
    {
        App.Current.Resources.TryGetValue("SimpleBlue", out var SimpleBlue);
        App.Current.Resources.TryGetValue("SimpleBlueLight", out var SimpleBlueLight);
        App.Current.Resources.TryGetValue("LightSurface", out var LightSurface);
        App.Current.Resources.TryGetValue("DarkSurface", out var DarkSurface);

        var simpleBlueLightColor = (Color)SimpleBlueLight;
        var simpleBlueColor = (Color)SimpleBlue;
        var lightSurfaceColor = (Color)LightSurface;
        var darkSurfaceColor = (Color)DarkSurface;

        return new Frame()
        {
            Padding = new Thickness(4),
            Margin = new Thickness(8, 4),
            CornerRadius = 8,
            MinimumHeightRequest = 0,
            HasShadow = true,
            Shadow = new Shadow()
            {
                Brush = Brush.Black,
                Offset = new Point(0, 0),
                Radius = 1f,
                Opacity = 1f
            },
            BorderColor = Colors.Transparent,
            Content = content,
        }
        .Apply(it =>
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                it.AppThemeColorBinding(BackgroundColorProperty, ColorUtils.CompositeColors(lightSurfaceColor, simpleBlueColor.WithAlpha(0x10)), ColorUtils.CompositeColors(Color.FromArgb("#242424"), simpleBlueLightColor.WithAlpha(0x70)));
            }
            else
            {
                it.AppThemeColorBinding(BackgroundColorProperty, ColorUtils.CompositeColors(lightSurfaceColor, simpleBlueColor.WithAlpha(0x10)), ColorUtils.CompositeColors(darkSurfaceColor, simpleBlueLightColor.WithAlpha(0x10)));
            }
        });
    }

    private Frame CreateForecastPanel()
    {
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return CreateWeatherNowFrame(new Grid()
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
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
                .OnIdiom(Label.MarginProperty, Thickness.Zero, Phone: new Thickness(4,0,0,0), Tablet: new Thickness(4,0,0,0))
                .AppThemeColorBinding(Label.TextColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel")
                .TapGesture(() => GotoDetailsPage(false)),
                new Image()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                    {
                        Size = 24
                    }.AppThemeColorBinding(MaterialIcon.ColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                }
                .OnIdiom(Image.MarginProperty, Thickness.Zero, Phone: new Thickness(0,0,4,0), Tablet: new Thickness(0,0,4,0))
                .TapGesture(() => GotoDetailsPage(false))
                .Column(1)
                .Row(0),
                // Content
                new ForecastRangeBarGraphView()
                .Bind(BindingContextProperty, getter: forecast => forecast.ForecastGraphData, mode: BindingMode.OneWay, source: ForecastView)
                .Row(1)
                .ColumnSpan(2)
                .Apply(it =>
                {
                    it.ItemClick += (s, e) =>
                    {
                        GotoDetailsPage(false, e.ItemIndex);
                    };
                })
            }
        })
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.Bind(VisualElement.IsVisibleProperty, getter: forecast => forecast.ForecastGraphData,
                    mode: BindingMode.OneWay, source: ForecastView, convert: (data) => FeatureSettings.Forecast && data?.IsEmpty != true);
            };

            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                it.Padding(0, 4);
            }

            ResizeElements.Add(it);
        });
    }

    private VisualElement CreateAlertButton()
    {
        App.Current.Resources.TryGetValue("collectionBooleanConverter", out var collectionBooleanConverter);

        // Alert Button
        return new Grid()
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
        .Bind(VisualElement.IsVisibleProperty, getter: alerts => alerts.Alerts,
            mode: BindingMode.OneWay, source: AlertsView, convert: alerts => alerts?.Count > 0
        )
        .TapGesture(GotoAlertsPage);
    }

    private Frame CreateHourlyForecastPanel()
    {
        App.Current.Resources.TryGetValue("collectionBooleanConverter", out var collectionBooleanConverter);
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return CreateWeatherNowFrame(new Grid()
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
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
                .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel")
                .TapGesture(() => GotoDetailsPage(true)),
                new Image()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                    {
                        Size = 24
                    }.AppThemeColorBinding(MaterialIcon.ColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                }
                .TapGesture(() => GotoDetailsPage(true))
                .Column(1)
                .Row(0),
                // Content
                new HourlyForecastItemPanel()
                .Bind(HourlyForecastItemPanel.ForecastDataProperty, static forecast => forecast.HourlyForecastData,
                        mode: BindingMode.OneWay, source: ForecastView
                )
                .Row(1)
                .ColumnSpan(2)
                .Apply(it =>
                {
                    if (DeviceInfo.Idiom == DeviceIdiom.Phone)
                    {
                        it.OnDeviceWidth(VisualElement.HeightRequestProperty, 828d, Default: 200d, GreaterThanEq: 175d);
                    }
                    else
                    {
                        it.OnIdiom(VisualElement.HeightRequestProperty, Default: 250d, Phone: 175d);
                    }

                    it.ItemClick += (s, e) =>
                    {
                        AnalyticsLogger.LogEvent("WeatherNow: GraphView_Tapped");
                        GotoDetailsPage(true, e.ItemIndex);
                    };

                    it.IconProvider = SettingsManager.IconProvider;

                    HourlyForecastPanel = it;
                })
            }
        })
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.Bind(VisualElement.IsVisibleProperty, static forecast => forecast.HourlyForecastData,
                    mode: BindingMode.OneWay, source: ForecastView, convert: (collection) =>
                    {
                        return FeatureSettings.HourlyForecast && collection?.Count > 0;
                    }
                );
            };

            ResizeElements.Add(it);
        });
    }

    private Frame CreateChartsPanel()
    {
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return CreateWeatherNowFrame(new Grid()
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
                .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel")
                .TapGesture(async () =>
                {
                    await Navigation.PushAsync(new WeatherChartsPage());
                }),
                new Image()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                    {
                        Size = 24
                    }.AppThemeColorBinding(MaterialIcon.ColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                }
                .TapGesture(async () =>
                {
                    await Navigation.PushAsync(new WeatherChartsPage());
                })
                .Column(1)
                .Row(0),
                // Content
                new Grid()
                {
                    RowDefinitions =
                    {
                        new RowDefinition()
                            .Bind(RowDefinition.HeightProperty, forecast => forecast.MinutelyPrecipitationGraphData, null,
                                BindingMode.OneWay, graphDataGridLengthConv, null, ForecastView, default(LineViewData)
                            ),
                        new RowDefinition()
                            .Bind(RowDefinition.HeightProperty, forecast => forecast.HourlyPrecipitationGraphData, null,
                                BindingMode.OneWay, graphDataGridLengthConv, null, ForecastView, default(LineViewData)
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
                        .Bind(ForecastGraphPanel.GraphDataProperty, forecast => forecast.MinutelyPrecipitationGraphData,
                                mode: BindingMode.OneWay, source: ForecastView
                        )
                        .Apply(it =>
                        {
                            it.Loaded += (s,e) =>
                            {
                                it.Bind(VisualElement.IsVisibleProperty, forecast => forecast.MinutelyPrecipitationGraphData, null,
                                    BindingMode.OneWay, graphDataConv, null, ForecastView, default(LineViewData)
                                );
                            };
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
                        .Bind(ForecastGraphPanel.GraphDataProperty, forecast => forecast.HourlyPrecipitationGraphData,
                               mode: BindingMode.OneWay, source: ForecastView
                        )
                        .Apply(it =>
                        {
                            it.Loaded += (s,e) =>
                            {
                                it.Bind(VisualElement.IsVisibleProperty, forecast => forecast.HourlyPrecipitationGraphData, null, 
                                    BindingMode.OneWay, graphDataConv, null, ForecastView, default(LineViewData)
                                );
                            };
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
        })
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.Bind(
                    VisualElement.IsVisibleProperty, static forecast => forecast.IsPrecipitationDataPresent,
                    mode: BindingMode.OneWay, source: ForecastView,
                    convert: (isPresent) => FeatureSettings.Charts && isPresent);
            };

            ResizeElements.Add(it);
        });
    }

    private Frame CreateDetailsPanel()
    {
        App.Current.Resources.TryGetValue("objectBooleanConverter", out var objectBooleanConverter);
        App.Current.Resources.TryGetValue("collectionBooleanConverter", out var collectionBooleanConverter);
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return CreateWeatherNowFrame(new Grid()
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
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
                .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel")
                .Apply(it =>
                {
                    it.Loaded += (s, e) =>
                    {
                        it.IsVisible = FeatureSettings.DetailsEnabled;
                    };
                }),
                // WeatherDetails
                new FlexLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Wrap = FlexWrap.Wrap,
                    JustifyContent = FlexJustify.Center,
                }
                .Paddings(12, 8, 12, 8) // 16,8
                .Bind(BindableLayout.ItemsSourceProperty, static viewModel => viewModel.Weather,
                    convert: (weather) => detailsFilter.ConvertFrom(weather?.WeatherDetails, CultureInfo.CurrentUICulture),
                    mode: BindingMode.OneWay, source: WNowViewModel
                )                
                .ItemTemplate(new DataTemplate(() =>
                {
                    return new DetailItem()
                        .OnIdiom(VisualElement.MinimumWidthRequestProperty, 155, Phone: 155, Desktop: 250)
                        .Margin(2, 4);
                }))
                .Row(1)
                .Apply(it =>
                {
                    it.Loaded += (s, e) =>
                    {
                        it.Bind(VisualElement.IsVisibleProperty, static viewModel => viewModel.Weather,
                            mode: BindingMode.OneWay, source: WNowViewModel,
                            convert: (weather) => FeatureSettings.WeatherDetails && weather?.WeatherDetails?.Count > 0);
                    };
                })
            }
        })
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.IsVisible = FeatureSettings.DetailsEnabled;
            };

            ResizeElements.Add(it);
        });
    }

    private Frame CreateUVControl()
    {
        return CreateWeatherNowFrame(new UVControl()
            .Bind(VisualElement.BindingContextProperty, static viewModel => viewModel.Weather,
                    mode: BindingMode.OneWay, source: WNowViewModel, convert: weather => weather?.UVIndex
            ))
            .Apply(it =>
            {
                it.Loaded += (s, e) =>
                {
                    it.Bind(VisualElement.IsVisibleProperty, static viewModel => viewModel.Weather,
                        mode: BindingMode.OneWay, source: WNowViewModel, convert: (weather) =>
                        {
                            return FeatureSettings.UV && !string.IsNullOrWhiteSpace(weather?.UVIndex?.ToString());
                        }
                    );
                };

                ResizeElements.Add(it);
            });
    }

    private Frame CreateBeaufortControl()
    {
        return CreateWeatherNowFrame(new BeaufortControl()
            .Bind(VisualElement.BindingContextProperty, static viewModel => viewModel.Weather, 
                mode: BindingMode.OneWay, source: WNowViewModel, convert: weather => weather?.Beaufort
            ))
            .Apply(it =>
            {
                it.Loaded += (s, e) =>
                {
                    it.Bind(VisualElement.IsVisibleProperty, static viewModel => viewModel.Weather,
                        mode: BindingMode.OneWay, source: WNowViewModel, convert: (weather) =>
                        {
                            return FeatureSettings.Beaufort && !string.IsNullOrWhiteSpace(weather?.Beaufort?.ToString());
                        }
                    );
                };

                ResizeElements.Add(it);
            });
    }

    private Frame CreateAQIControl()
    {
        return CreateWeatherNowFrame(new AQIControl()
            .Bind(VisualElement.BindingContextProperty, static viewModel => viewModel.Weather,
                mode: BindingMode.OneWay, source: WNowViewModel, convert: weather => weather?.AirQuality
            )
            .TapGesture(async () =>
            {
                var weatherAqiPage = new WeatherAQIPage(new WeatherPageArgs() { Location = WNowViewModel?.UiState?.LocationData });
                await Navigation.PushAsync(weatherAqiPage);
            }))
            .Apply(it =>
            {
                it.Loaded += (s, e) =>
                {
                    it.Bind(VisualElement.IsVisibleProperty, static viewModel => viewModel.Weather,
                        mode: BindingMode.OneWay, source: WNowViewModel, convert: (weather) =>
                        {
                            return FeatureSettings.AQIndex && !string.IsNullOrWhiteSpace(weather?.AirQuality?.ToString());
                        }
                    );
                };

                ResizeElements.Add(it);
            });
    }

    private Frame CreatePollenCountControl()
    {
        return CreateWeatherNowFrame(new PollenCountControl()
            .Bind(VisualElement.BindingContextProperty, static viewModel => viewModel.Weather, 
                mode: BindingMode.OneWay, source: WNowViewModel, convert: (weather) => weather?.Pollen
            ))
            .Apply(it =>
            {
                it.Loaded += (s, e) =>
                {
                    it.Bind(VisualElement.IsVisibleProperty, static viewModel => viewModel.Weather,
                        mode: BindingMode.OneWay, source: WNowViewModel, convert: (weather) =>
                        {
                            return FeatureSettings.PollenEnabled && !string.IsNullOrWhiteSpace(weather?.Pollen?.ToString());
                        }
                    );
                };

                ResizeElements.Add(it);
            });
    }

    private Frame CreateMoonPhaseControl()
    {
        return CreateWeatherNowFrame(new MoonPhaseControl()
            .Bind(VisualElement.BindingContextProperty, static viewModel => viewModel.Weather,
                mode: BindingMode.OneWay, source: WNowViewModel, convert: weather => weather?.MoonPhase
            ))
            .Apply(it =>
            {
                it.Loaded += (s, e) =>
                {
                    it.Bind(VisualElement.IsVisibleProperty, static viewModel => viewModel.Weather,
                        mode: BindingMode.OneWay, source: WNowViewModel, convert: (weather) =>
                        {
                            return FeatureSettings.MoonPhase && !string.IsNullOrWhiteSpace(weather?.MoonPhase?.ToString());
                        }
                    );
                };

                ResizeElements.Add(it);
            });
    }

    private Frame CreateSunPhasePanel()
    {
        App.Current.Resources.TryGetValue("objectBooleanConverter", out var objectBooleanConverter);
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return CreateWeatherNowFrame(new Grid()
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
                    VisualElement.BindingContextProperty, static viewModel => viewModel.Weather,
                    mode: BindingMode.OneWay, source: WNowViewModel, convert: weather => weather?.SunPhase
                )
                .FillHorizontal()
                .MinWidth(350)
                .Apply(it =>
                {
                    var maxHeight = 225;

                    void ResizeSunView()
                    {
                        var parentHorizMargin = new Thickness(20, 0);

                        it.HeightRequest = maxHeight - parentHorizMargin.HorizontalThickness;
                        it.WidthRequest = Math.Min(it.HeightRequest * 2, DeviceDisplay.MainDisplayInfo.Width) - parentHorizMargin.HorizontalThickness;
                    }

                    ListLayout.SizeChanged += (s, e) =>
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            ResizeSunView();
                        });
                    };

                    ResizeSunView();
                })
            }
        }
        .FillHorizontal())
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.Bind(
                    VisualElement.IsVisibleProperty, static viewModel => viewModel.Weather,
                    mode: BindingMode.OneWay, source: WNowViewModel, convert: (weather) =>
                    {
                        return FeatureSettings.SunPhase && !string.IsNullOrWhiteSpace(weather?.SunPhase?.ToString());
                    }
                );
            };

            ResizeElements.Add(it);
        });
    }

    private Frame CreateRadarPanel()
    {
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return CreateWeatherNowFrame(new Grid()
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
                .DynamicResource(Label.StyleProperty, "WeatherNowSectionLabel")
                .TapGesture(async () =>
                {
                    await Navigation.PushAsync(new WeatherRadarPage());
                }),
                new Image()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Source = new MaterialIcon(MaterialSymbol.ChevronRight)
                    {
                        Size = 24
                    }.AppThemeColorBinding(MaterialIcon.ColorProperty, (Color)LightOnBackground, (Color)DarkOnBackground)
                }
                .Column(1)
                .Row(0)
                .TapGesture(async () =>
                {
                    await Navigation.PushAsync(new WeatherRadarPage());
                }),
                // Content
                new Border()
                {
                    BackgroundColor = Colors.Transparent,
                    Stroke = Colors.Transparent,
                    MaximumWidthRequest = 720,
                    ZIndex = 16,
                }
                .OnIdiom(Border.HeightRequestProperty, Default: 350, Phone: 200)
                .Row(1)
                .ColumnSpan(2)
                .TapGesture(async () =>
                {
                    await Navigation.PushAsync(new WeatherRadarPage());
                }),
                new Border()
                {
                    MaximumWidthRequest = 720,
                    BackgroundColor = Colors.Black
                }
                .OnIdiom(Border.HeightRequestProperty, Default: 350, Phone: 200)
                .Row(1)
                .ColumnSpan(2)
                .Apply(it =>
                {
                    RadarWebViewContainer = it;
                    it.SizeChanged += (s, e) =>
                    {
                        if (it.Width > it.MaximumWidthRequest)
                        {
                            it.WidthRequest = it.MaximumWidthRequest;
                        }
                    };
                    it.SetAppTheme(ClassIdProperty, "light", "dark");

                    radarViewProvider?.OnDestroyView();
                    radarViewProvider = RadarProvider.GetRadarViewProvider(it);
                    radarViewProvider?.EnableInteractions(false);
                })
            }
        })
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.IsVisible = FeatureSettings.WeatherRadar;
            };

            ResizeElements.Add(it);
        });
    }

    private Label CreateWeatherCredit()
    {
        return new Label()
        {
            Padding = 10,
            HorizontalOptions = LayoutOptions.Center,
            FontSize = 14
        }.Bind(
            Label.TextProperty, static viewModel => viewModel.Weather,
            mode: BindingMode.OneWay, source: WNowViewModel,
            convert: weather => weather?.WeatherCredit,
            fallbackValue: "Data from Weather Provider"
        );
    }
}