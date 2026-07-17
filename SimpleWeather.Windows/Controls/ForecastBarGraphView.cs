using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using NetTopologySuite.Mathematics;
using SimpleWeather.Icons;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.NET.Helpers;
using SimpleWeather.Utils;
using System.Data;
using Windows.Foundation;

namespace SimpleWeather.NET.Controls
{
    public partial class ForecastBarGraphView : Grid
    {
        public int GraphHeight
        {
            get { return (int)GetValue(GraphHeightProperty); }
            set { SetValue(GraphHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphHeightProperty =
            DependencyProperty.Register(nameof(GraphHeight), typeof(int), typeof(ForecastBarGraphView), new PropertyMetadata(150));

        private double zeroValueItemHeight;
        private double bottomTextHeights = 0;

        private BarGraphData graphData = null;

        public ForecastType? ForecastType
        {
            get { return (ForecastType?)GetValue(ForecastTypeProperty); }
            set { SetValue(ForecastTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForecastType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForecastTypeProperty =
            DependencyProperty.Register(nameof(ForecastType), typeof(ForecastType?), typeof(ForecastBarGraphView), new PropertyMetadata(null, OnDataPropertyChanged));

        private readonly StackPanel InnerLayout;
        private readonly Button LeftButton;
        private readonly Button RightButton;
        private readonly ScrollViewer ScrollView;

        public event EventHandler<TappedRoutedEventArgs> ItemClick;

        public bool CanScrollToStart
        {
            get { return (bool)GetValue(CanScrollToStartProperty); }
            set { SetValue(CanScrollToStartProperty, value); }
        }

        public static readonly DependencyProperty CanScrollToStartProperty =
            DependencyProperty.Register(nameof(CanScrollToStart), typeof(bool),
            typeof(ForecastBarGraphView), new PropertyMetadata(false));

        public bool CanScrollToEnd
        {
            get { return (bool)GetValue(CanScrollToEndProperty); }
            set { SetValue(CanScrollToEndProperty, value); }
        }

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register(nameof(CanScrollToEnd), typeof(bool),
            typeof(ForecastBarGraphView), new PropertyMetadata(false));

        public ForecastBarGraphView()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Children.Clear();

            ColumnDefinitions.Clear();
            ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            Children.Add(LeftButton = new Button().Apply(it =>
            {
                it.Click += LeftButton_Click;
                it.SetBinding(Button.IsEnabledProperty, new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(nameof(CanScrollToStart)),
                    Mode = BindingMode.OneWay,
                });
                it.Style = (Style)this.FindResource("LeftArrowButton");
                SetColumn(it, 0);
            }));

            Children.Add(RightButton = new Button().Apply(it =>
            {
                it.Click += RightButton_Click;
                it.SetBinding(Button.IsEnabledProperty, new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(nameof(CanScrollToEnd)),
                    Mode = BindingMode.OneWay,
                });
                it.Style = (Style)this.FindResource("RightArrowButton");
                SetColumn(it, 2);
            }));

            Children.Add(ScrollView = new ScrollViewer()
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HorizontalScrollMode = ScrollMode.Enabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollMode = ScrollMode.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = InnerLayout = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                }
            }.Apply(it =>
            {
                it.ViewChanged += ScrollView_ViewChanged;
                it.SizeChanged += ScrollView_SizeChanged;
                SetColumn(it, 1);
            }));

            InnerLayout.SetBinding(MinHeightProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(GraphHeight)),
                Mode = BindingMode.OneWay,
            });

            this.SetBinding(MinHeightProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(GraphHeight)),
                Mode = BindingMode.OneWay,
            });

            this.DataContextChanged += (s, e) =>
            {
                SetData(e.NewValue as BarGraphData);
            };
        }

        private void UpdateScrollButtons(ScrollViewer scroller)
        {
            CanScrollToStart = ScrollViewerHelper.CanScrollToStart(scroller);
            CanScrollToEnd = ScrollViewerHelper.CanScrollToEnd(scroller);
            if (scroller.ExtentWidth > scroller.ViewportWidth)
            {
                LeftButton.Visibility = Visibility.Visible;
                RightButton.Visibility = Visibility.Visible;
            }
            else
            {
                LeftButton.Visibility = Visibility.Collapsed;
                RightButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollLeft(ScrollView);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollRight(ScrollView);
        }

        private void ScrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            UpdateScrollButtons(ScrollView);
        }

        private void ScrollView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScrollButtons(ScrollView);
        }

        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ForecastBarGraphView view)
            {
                view.SetData(view.graphData);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);

            var barIconHeight = 32;
            var heightChanged = false;

            InnerLayout.Children?.Cast<ForecastBar>()?.ForEachIndexed<ForecastBar>((idx, bar) =>
            {
                if (idx == 0)
                {
                    var measuredBottomTextHeights = (bar.BarValueBlock.DesiredSize.Height + bar.BarDateBlock.DesiredSize.Height);
                    if (measuredBottomTextHeights != bottomTextHeights)
                    {
                        bottomTextHeights = measuredBottomTextHeights;
                    }
                }

                graphData?.GetDataSet()?.GetEntryForIndex(idx)?.Let(data =>
                {
                    var max = graphData?.YMax;
                    var min = graphData?.YMin;
                    if (min.HasValue && max.HasValue)
                    {
                        if (UpdateInnerBarHeight(bar.InnerBarView, min.Value, max.Value, data))
                        {
                            heightChanged = true;
                        }
                    }
                });

                if (!heightChanged)
                {
                    var shouldBeGone = bar.InnerBarView.DesiredSize.Height switch
                    {
                        > 0 => bar.InnerBarView.DesiredSize.Height <= barIconHeight,
                        _ => bar.InnerBarView.Height <= barIconHeight,
                    };

                    if (shouldBeGone != bar.BarIconView.IsGone())
                    {
                        bar.BarIconView.IsGone(shouldBeGone);
                    }
                }
            });

            if (heightChanged)
            {
                size = base.MeasureOverride(availableSize);

                InnerLayout.Children?.Cast<ForecastBar>()?.ForEach<ForecastBar>((bar) =>
                {
                    var shouldBeGone = bar.InnerBarView.DesiredSize.Height switch
                    {
                        > 0 => bar.InnerBarView.DesiredSize.Height <= barIconHeight,
                        _ => bar.InnerBarView.Height <= barIconHeight,
                    };

                    if (shouldBeGone != bar.BarIconView.IsGone())
                    {
                        bar.BarIconView.IsGone(shouldBeGone);
                        bar.InnerBarView.InvalidateMeasure();
                        bar.InnerBarView.InvalidateArrange();
                    }
                });
            }

            return size;
        }

        public void SetData(BarGraphData? graphData)
        {
            this.graphData = graphData;

            var dataSet = graphData?.GetDataSet();

            if (dataSet != null && !dataSet.IsEmpty)
            {
                var itemCount = dataSet.DataCount;

                var max = dataSet.YMax;
                var min = dataSet.YMin;

                for (int i = 0; i < itemCount; i++)
                {
                    var data = dataSet.GetEntryForIndex(i);
                    var item = InnerLayout.Children.ElementAtOrDefault(i) as ForecastBar ?? new ForecastBar().Apply(it =>
                    {
                        it.Tapped += (s, e) =>
                        {
                            ItemClick?.Invoke(it, new TappedRoutedEventArgs());
                        };
                        it.VerticalAlignment = VerticalAlignment.Bottom;
                    });

                    item.DataContext = data;

                    UpdateInnerBarHeight(item.InnerBarView, min, max, data);

                    item.BarIconView.IconProvider = string.IsNullOrWhiteSpace(data.XWeatherIcon) ? "mat-icons-jdwi" : null;
                    item.BarIconView.WeatherIcon = data.XWeatherIcon ?? GetWeatherIconFromForecastType(ForecastType);

                    if (InnerLayout.Children.ElementAtOrDefault(i) == null)
                    {
                        InnerLayout.Children.Add(item);
                    }
                }

                InnerLayout.Children.RemoveRange(itemCount, InnerLayout.Children.Count - itemCount);
                this.InvalidateMeasure();
            }
            else
            {
                InnerLayout.Children.Clear();
            }
        }

        private bool UpdateInnerBarHeight(FrameworkElement innerBar, float min, float max, BarGraphEntry data)
        {
            var heightChanged = false;

            var normalizedValue = 0f;

            if (min == max)
            {
                normalizedValue = 1f;
            }
            else if (data.EntryData?.Y != null)
            {
                var dataRange = max - min;

                if (dataRange == 0f)
                {
                    normalizedValue = 0f;
                }
                else
                {
                    normalizedValue = (float)((MathUtil.Clamp(data.EntryData.Y, min, max) - min) / dataRange);
                }
            }

            var newHeight = zeroValueItemHeight + (normalizedValue * (GraphHeight - bottomTextHeights));

            if (innerBar.Height != newHeight)
            {
                innerBar.Height = newHeight;
                heightChanged = true;
            }

            return heightChanged;
        }

        private static string GetWeatherIconFromForecastType(ForecastType? forecastType) => forecastType switch
        {
            Controls.ForecastType.Temperature => null,
            Controls.ForecastType.Minutely or Controls.ForecastType.Precipitation => WeatherIcons.RAINDROP,
            Controls.ForecastType.Wind => WeatherIcons.DIRECTION_UP,
            Controls.ForecastType.Humidity => WeatherIcons.HUMIDITY,
            Controls.ForecastType.UVIndex => null,
            Controls.ForecastType.Rain => WeatherIcons.RAINDROP,
            Controls.ForecastType.Snow => WeatherIcons.SNOWFLAKE_COLD,
            Controls.ForecastType.AirQuality => null,
            _ => null,
        };
    }
}
