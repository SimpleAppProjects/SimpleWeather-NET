﻿using CommunityToolkit.Maui.Converters;
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
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Main;

public partial class WeatherNow
{
    // Views
    private Label CurTemp { get; set; }
    private IconControl WeatherBox { get; set; }
    private Border RadarWebViewContainer { get; set; }
    private VisualElement GradientOverlay { get; set; }
    private VisualElement HourlyForecastPanel { get; set; }

    private readonly HashSet<VisualElement> ResizeElements = new HashSet<VisualElement>();

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
        .Bind(ToolbarItem.IsEnabledProperty, nameof(RefreshLayout.IsRefreshing), BindingMode.OneWay, inverseBoolConverter as IValueConverter, source: RefreshLayout)
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
                .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.UiState)}.{nameof(WNowViewModel.UiState.IsGPSLocation)}",
                    BindingMode.OneWay, source: WNowViewModel, targetNullValue: false, fallbackValue: false
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
                        .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Location)}",
                            BindingMode.OneWay, source: WNowViewModel, fallbackValue: ResStrings.label_nav_weathernow, targetNullValue: ResStrings.label_nav_weathernow
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
                .Bind<VerticalStackLayout, bool, Thickness>(
                    VerticalStackLayout.MarginProperty, $"{nameof(WNowViewModel.UiState)}.{nameof(WNowViewModel.UiState.IsGPSLocation)}",
                    BindingMode.OneWay, convert: (isGPS) =>
                    {
                        return isGPS ? new Thickness(40, 0) : Thickness.Zero;
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
                            .Bind(Image.SourceProperty, $"{nameof(WNowViewModel.ImageData)}.{nameof(WNowViewModel.ImageData.ImageSource)}",
                                BindingMode.OneWay, source: WNowViewModel
                            ),
                            new Button()
                                .End()
                                .Bottom()
                                .Margin(16,0)
                                .Padding(0)
                                .BackgroundColor(Colors.Transparent)
                                .Font(size: 11)
                                .TextColor(Colors.White)
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
                                })
                        }
                    }
                }
                .Apply(it =>
                {
                    it.Loaded += (s, e) =>
                    {
                        it.IsVisible = Utils.FeatureSettings.BackgroundImage && WNowViewModel.ImageData != null;
                    };

                    WNowViewModel.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(WNowViewModel.ImageData))
                        {
                            it.IsVisible = Utils.FeatureSettings.BackgroundImage && WNowViewModel.ImageData != null;
                        }
                    };

                    void UpdateSize()
                    {
                        var displayInfo = DeviceDisplay.MainDisplayInfo;
                        var heightDp = displayInfo.Height / displayInfo.Density;

                        if (DeviceInfo.Idiom == DeviceIdiom.Tablet || displayInfo.Orientation == DisplayOrientation.Landscape)
                        {
                            it.MaximumHeightRequest = 560;
                        }
                        else
                        {
                            it.MaximumHeightRequest = 420;
                        }

                        it.EnableAspectRatio = displayInfo.Orientation != DisplayOrientation.Landscape;

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
                        .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.CurTemp)}",
                            BindingMode.OneWay, source: WNowViewModel
                        )
                        .Bind<Label, string, Color>(Label.TextColorProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.CurTemp)}",
                            BindingMode.OneWay, source: WNowViewModel, convert: (p) => GetTempColor()
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
                                            .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.HiTemp)}",
                                                BindingMode.OneWay, source: WNowViewModel, targetNullValue: '\u2022'
                                            )
                                            .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
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
                                            .Font(size: 20)
                                            .Height(30)
                                            .TextCenterHorizontal()
                                            .CenterVertical()
                                            .TextCenterVertical()
                                            .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.LoTemp)}",
                                                BindingMode.OneWay, source: WNowViewModel, targetNullValue: '\u2022'
                                            )
                                            .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
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
                            HeightRequest = 88,
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.Center,
                            IconHeight = 88,
                            IconWidth = 88,
                            WidthRequest = 88
                        }
                        .Margins(right: 16)
                        .Column(3)
                        .Bind(IconControl.WeatherIconProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherIcon)}",
                            BindingMode.OneWay, source: WNowViewModel
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
                            .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.CurCondition)}",
                                BindingMode.OneWay, source: WNowViewModel
                            )
                            .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                        // Summary
                        new Label()
                        {
                            LineBreakMode = LineBreakMode.TailTruncation,
                            MaxLines = 3
                        }
                        .Row(2)
                        .ColumnSpan(5)
                        .Margin(16, 4)
                        .Padding(4)
                        .Font(size: 12)
                        .Opacity(0.75)
                        .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherSummary)}",
                            BindingMode.OneWay, source: WNowViewModel
                        )
                        .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White)
                        .Apply(it =>
                        {
                            it.Loaded += (s, e) =>
                            {
                                it.IsVisible = Utils.FeatureSettings.WeatherSummary;
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
                        .Row(3)
                        .ColumnSpan(5)
                        .Margins(16, 4, 16, 8)
                        .Padding(4)
                        .Font(size: 12)
                        .Opacity(0.75)
                        .Bind(Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.UpdateDate)}",
                            BindingMode.OneWay, source: WNowViewModel
                        )
                        .AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                    }
                }
            }
        };
    }

    private VerticalStackLayout CreateDesktopConditionPanel()
    {
        App.Current.Resources.TryGetValue("objectBooleanConverter", out var objectBooleanConverter);
        App.Current.Resources.TryGetValue("stringBooleanConverter", out var stringBooleanConverter);
        App.Current.Resources.TryGetValue("collectionBooleanConverter", out var collectionBooleanConverter);

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
                                    .Bind(FontImageSource.ColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this)
                            },
                            Stroke = new SolidColorBrush(Colors.Transparent)
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
                CreateAlertButton(),
                // Spacer
                new Rectangle()
                .Margin(new Thickness(8))
                .Apply(it =>
                {
                    Spacer = it;

                    WNowViewModel.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(WNowViewModel.ImageData))
                        {
                            it.IsVisible = Utils.FeatureSettings.BackgroundImage && WNowViewModel.ImageData != null;
                        }
                    };

                    it.Loaded += (s, e) =>
                    {
                        it.IsVisible = Utils.FeatureSettings.BackgroundImage && WNowViewModel.ImageData != null;
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
                        .Bind(Label.TextColorProperty, nameof(ConditionPanelTextColor), BindingMode.OneWay, source: this)
                        .Apply(it =>
                        {
                            it.Loaded += (s, e) =>
                            {
                                it.IsVisible = Utils.FeatureSettings.WeatherSummary;
                            };
                        }),
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
                                it.Loaded += (s, e) =>
                                {
                                     it.IsVisible = Utils.FeatureSettings.BackgroundImage;
                                };

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
                            })
                    }
                }
            }
        }
        .Bind(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Location)}",
            BindingMode.OneWay, source: WNowViewModel, converter: stringBooleanConverter as IValueConverter
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
                        if (Utils.FeatureSettings.BackgroundImage && h > 0)
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

    private Grid CreateForecastPanel()
    {
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);
        Resources.TryGetValue("graphDataConv", out var graphDataConv);

        return new Grid()
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
                .Bind(RangeBarGraphPanel.ForecastDataProperty, $"{nameof(ForecastView.ForecastGraphData)}", BindingMode.OneWay, source: ForecastView)
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
        .OnIdiom(Grid.MarginProperty, Default: new Thickness(0, 0, 0, 20), Phone: new Thickness(0), Tablet: new Thickness(0))
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.Bind<Grid, object, bool>(
                    VisualElement.IsVisibleProperty, $"{nameof(ForecastView.ForecastGraphData)}", BindingMode.OneWay, source: ForecastView,
                    convert: (data) =>
                    {
                        return Utils.FeatureSettings.Forecast && (bool)(graphDataConv as IValueConverter).Convert(data, typeof(bool), null, CultureInfo.InvariantCulture);
                    }
                 );
            };

            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                it.Padding(8, 0);
            else
                it.Padding(16, 0);

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
        .Bind(VisualElement.IsVisibleProperty, $"{nameof(AlertsView.Alerts)}",
            BindingMode.OneWay, source: AlertsView, converter: collectionBooleanConverter as IValueConverter
        )
        .TapGesture(GotoAlertsPage);
    }

    private Grid CreateHourlyForecastPanel()
    {
        App.Current.Resources.TryGetValue("collectionBooleanConverter", out var collectionBooleanConverter);
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return new Grid()
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
                .Bind(HourlyForecastItemPanel.ForecastDataProperty, $"{nameof(ForecastView.HourlyForecastData)}",
                        BindingMode.OneWay, source: ForecastView
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
        }
        .OnIdiom(Grid.MarginProperty, Default: new Thickness(0, 0, 0, 20), Phone: new Thickness(0), Tablet: new Thickness(0))
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.Bind<Grid, IEnumerable, bool>(VisualElement.IsVisibleProperty, $"{nameof(ForecastView.HourlyForecastData)}",
                    BindingMode.OneWay, source: ForecastView, convert: (collection) =>
                    {
                        return Utils.FeatureSettings.HourlyForecast && (bool)((IValueConverter)collectionBooleanConverter).Convert(collection, typeof(bool), null, CultureInfo.InvariantCulture);
                    }
                );
            };

            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                it.Padding(8, 0);
            else
                it.Padding(16, 0);

            ResizeElements.Add(it);
        });
    }

    private Grid CreateChartsPanel()
    {
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);
        Resources.TryGetValue("graphDataConv", out var graphDataConv);
        Resources.TryGetValue("graphDataGridLengthConv", out var graphDataGridLengthConv);

        return new Grid()
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
                        .Apply(it =>
                        {
                            it.Loaded += (s,e) =>
                            {
                                it.Bind(VisualElement.IsVisibleProperty, $"{nameof(ForecastView.MinutelyPrecipitationGraphData)}",
                                        BindingMode.OneWay, graphDataConv as IValueConverter, source: ForecastView
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
                        .Bind(ForecastGraphPanel.GraphDataProperty, $"{nameof(ForecastView.HourlyPrecipitationGraphData)}",
                                BindingMode.OneWay, source: ForecastView
                        )
                        .Apply(it =>
                        {
                            it.Loaded += (s,e) =>
                            {
                                it.Bind(VisualElement.IsVisibleProperty, $"{nameof(ForecastView.HourlyPrecipitationGraphData)}",
                                        BindingMode.OneWay, graphDataConv as IValueConverter, source: ForecastView
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
        }
        .OnIdiom(Grid.MarginProperty, Default: new Thickness(0, 0, 0, 20), Phone: new Thickness(0), Tablet: new Thickness(0))
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.Bind<Grid, bool, bool>(
                    VisualElement.IsVisibleProperty, $"{nameof(ForecastView.IsPrecipitationDataPresent)}", BindingMode.OneWay, source: ForecastView,
                    convert: (isPresent) =>
                    {
                        return Utils.FeatureSettings.Charts && isPresent;
                    }
                 );
            };

            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                it.Padding(8, 0);
            else
                it.Padding(16, 0);

            ResizeElements.Add(it);
        });
    }

    private Grid CreateDetailsPanel()
    {
        App.Current.Resources.TryGetValue("objectBooleanConverter", out var objectBooleanConverter);
        App.Current.Resources.TryGetValue("collectionBooleanConverter", out var collectionBooleanConverter);
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);
        Resources.TryGetValue("detailsFilter", out var detailsFilter);

        return new Grid()
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
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
                        it.IsVisible = Utils.FeatureSettings.DetailsEnabled;
                    };
                }),
                // WeatherDetails
                new FlexLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Wrap = FlexWrap.Wrap,
                    JustifyContent = FlexJustify.Center,
                }
                .Bind(BindableLayout.ItemsSourceProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherDetails)}",
                        BindingMode.OneWay, detailsFilter as IValueConverter, source: WNowViewModel
                )                
                .DynamicResource(BindableLayout.ItemTemplateProperty, "DetailItemTemplate")
                .Row(1)
                .Apply(it =>
                {
                    it.Loaded += (s, e) =>
                    {
                        it.Bind<VisualElement, IEnumerable, bool>(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherDetails)}",
                            BindingMode.OneWay, source: WNowViewModel, convert: (collection) =>
                            {
                                return Utils.FeatureSettings.WeatherDetails && (bool)(collectionBooleanConverter as IValueConverter).Convert(collection, typeof(bool), null, CultureInfo.InvariantCulture);
                            }
                        );
                    };
                }),
                // ExtraDetailsEnabled
                new WrapGrid()
                {
                    MaxColumns = 3,
                    MinItemWidth = 340
                }
                .OnIdiom(WrapGrid.MaxColumnsProperty, Phone: 1, Default: 3)
                .Apply(wrapLayout =>
                {
                    // UV
                    wrapLayout.Add(
                        new UVControl()
                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.UVIndex)}",
                                    BindingMode.OneWay, source: WNowViewModel
                            )
                            .Apply(it =>
                            {
                                it.Loaded += (s, e) =>
                                {
                                    it.Bind<VisualElement, object, bool>(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.UVIndex)}",
                                        BindingMode.OneWay, source: WNowViewModel, convert: (value) =>
                                        {
                                            return Utils.FeatureSettings.UV && !string.IsNullOrWhiteSpace(value?.ToString());
                                        }
                                    );
                                };
                            })
                    );
                    // Beaufort
                    wrapLayout.Add(
                        new BeaufortControl()
                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Beaufort)}",
                                    BindingMode.OneWay, source: WNowViewModel
                            )
                            .Apply(it =>
                            {
                                it.Loaded += (s, e) =>
                                {
                                    it.Bind<VisualElement, object, bool>(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Beaufort)}",
                                        BindingMode.OneWay, source: WNowViewModel, convert: (value) =>
                                        {
                                            return Utils.FeatureSettings.Beaufort && !string.IsNullOrWhiteSpace(value?.ToString());
                                        }
                                    );
                                };
                            })
                    );
                    // AQIndex
                    wrapLayout.Add(
                        new AQIControl()
                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.AirQuality)}",
                                    BindingMode.OneWay, source: WNowViewModel
                            )
                            .TapGesture(async () =>
                            {
                                await Navigation.PushAsync(new WeatherAQIPage(new WeatherPageArgs() { Location = WNowViewModel?.UiState?.LocationData }));
                            })
                            .Apply(it =>
                            {
                                it.Loaded += (s, e) =>
                                {
                                    it.Bind<VisualElement, object, bool>(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.AirQuality)}",
                                        BindingMode.OneWay, source: WNowViewModel, convert: (value) =>
                                        {
                                            return Utils.FeatureSettings.AQIndex && !string.IsNullOrWhiteSpace(value?.ToString());
                                        }
                                    );
                                };
                            })
                    );
                    // PollenEnabled
                    wrapLayout.Add(
                        new PollenCountControl()
                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Pollen)}",
                                    BindingMode.OneWay, source: WNowViewModel
                            )
                            .Apply(it =>
                            {
                                it.Loaded += (s, e) =>
                                {
                                    it.Bind<VisualElement, object, bool>(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.Pollen)}",
                                        BindingMode.OneWay, source: WNowViewModel, convert: (value) =>
                                        {
                                            return Utils.FeatureSettings.PollenEnabled && !string.IsNullOrWhiteSpace(value?.ToString());
                                        }
                                    );
                                };
                            })
                    );
                    // MoonPhase
                    wrapLayout.Add(
                        new MoonPhaseControl()
                            .Bind(VisualElement.BindingContextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.MoonPhase)}",
                                    BindingMode.OneWay, source: WNowViewModel
                            )
                            .Apply(it =>
                            {
                                it.Loaded += (s, e) =>
                                {
                                    it.Bind<VisualElement, object, bool>(VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.MoonPhase)}",
                                        BindingMode.OneWay, source: WNowViewModel, convert: (value) =>
                                        {
                                            return Utils.FeatureSettings.MoonPhase && !string.IsNullOrWhiteSpace(value?.ToString());
                                        }
                                    );
                                };
                            })
                    );
                })
                .Row(2)
                .Apply(it =>
                {
                    it.Loaded += (s, e) =>
                    {
                        it.IsVisible = Utils.FeatureSettings.ExtraDetailsEnabled;
                    };
                })
            }
        }
        .OnIdiom(Grid.MarginProperty, Default: new Thickness(0, 0, 0, 20), Phone: new Thickness(0), Tablet: new Thickness(0))
        .Apply(it =>
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                it.Padding(8, 0);
            else
                it.Padding(16, 0);

            it.Loaded += (s, e) =>
            {
                it.IsVisible = Utils.FeatureSettings.DetailsEnabled;
            };

            ResizeElements.Add(it);
        });
    }

    private Grid CreateSunPhasePanel()
    {
        App.Current.Resources.TryGetValue("objectBooleanConverter", out var objectBooleanConverter);
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return new Grid()
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
                    .FillHorizontal()
                    .Apply(it =>
                    {
                        var maxHeight = DeviceInfo.Idiom == DeviceIdiom.Phone ? 200 : 250;

                        void ResizeSunView()
                        {
                            Thickness parentHorizMargin;

                            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                            {
                                parentHorizMargin = new Thickness(4, 0);
                            }
                            else
                            {
                                parentHorizMargin = new Thickness(0);
                            }

                            it.HeightRequest = Math.Min(maxHeight, Math.Max(0, ListLayout.Width / 2)) - parentHorizMargin.HorizontalThickness;
                            it.WidthRequest = Math.Min(it.HeightRequest * 2, DeviceDisplay.MainDisplayInfo.Width);
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
        .FillHorizontal()
        .OnIdiom(Grid.MarginProperty, Default: new Thickness(0, 0, 0, 20), Phone: new Thickness(0), Tablet: new Thickness(0))
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.Bind<Grid, object, bool>(
                    VisualElement.IsVisibleProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.SunPhase)}",
                    BindingMode.OneWay, source: WNowViewModel, convert: (value) =>
                    {
                        return Utils.FeatureSettings.SunPhase && !string.IsNullOrWhiteSpace(value?.ToString());
                    }
                );
            };

            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                it.Padding(8, 0);
            else
                it.Padding(16, 0);

            ResizeElements.Add(it);
        });
    }

    private Grid CreateRadarPanel()
    {
        App.Current.Resources.TryGetValue("LightOnBackground", out var LightOnBackground);
        App.Current.Resources.TryGetValue("DarkOnBackground", out var DarkOnBackground);

        return new Grid()
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
        }
        .OnIdiom(Grid.MarginProperty, Default: new Thickness(0, 0, 0, 20), Phone: new Thickness(0), Tablet: new Thickness(0))
        .Apply(it =>
        {
            it.Loaded += (s, e) =>
            {
                it.IsVisible = Utils.FeatureSettings.WeatherRadar;
            };

            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                it.Padding(8, 0);
            else
                it.Padding(16, 0);

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
            Label.TextProperty, $"{nameof(WNowViewModel.Weather)}.{nameof(WNowViewModel.Weather.WeatherCredit)}",
            BindingMode.OneWay, source: WNowViewModel, fallbackValue: "Data from Weather Provider"
        );
    }
}