using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.NET.Helpers;
using SimpleWeather.Utils;
using Windows.Foundation;

namespace SimpleWeather.NET.Controls
{
    public partial class ForecastRangeBarGraphView : Grid
    {
        private int MaxItemCount = 7;

        public int GraphHeight
        {
            get { return (int)GetValue(GraphHeightProperty); }
            set { SetValue(GraphHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphHeightProperty =
            DependencyProperty.Register(nameof(GraphHeight), typeof(int), typeof(ForecastRangeBarGraphView), new PropertyMetadata(150));

        private ForecastRangeBarGraphData GraphData = null;
        public ForecastType? ForecastType { get; set; } = null;

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
            typeof(ForecastRangeBarGraphView), new PropertyMetadata(false));

        public bool CanScrollToEnd
        {
            get { return (bool)GetValue(CanScrollToEndProperty); }
            set { SetValue(CanScrollToEndProperty, value); }
        }

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register(nameof(CanScrollToEnd), typeof(bool),
            typeof(ForecastRangeBarGraphView), new PropertyMetadata(false));

        public ForecastRangeBarGraphView()
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
                SetData(e.NewValue as ForecastRangeBarGraphData);
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

        public int GetItemPosition(object item)
        {
            var index = Children.IndexOf(item);
            return index >= 0 ? index : 0;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            RefreshLayout();
            return size;
        }

        public void SetData(ForecastRangeBarGraphData graphData)
        {
            this.GraphData = graphData;

            var dataSet = graphData?.GetDataSet();

            if (dataSet != null && !dataSet.IsEmpty)
            {
                var itemCount = Math.Min(dataSet.DataCount, MaxItemCount);

                var max = dataSet.YMax;
                var min = dataSet.YMin;

                for (int i = 0; i < itemCount; i++)
                {
                    var data = dataSet.GetEntryForIndex(i);
                    var item = InnerLayout.Children.ElementAtOrDefault(i) as ForecastRangeBar ?? new ForecastRangeBar().Apply(it =>
                    {
                        it.Tapped += (s, e) =>
                        {
                            ItemClick?.Invoke(it, new TappedRoutedEventArgs());
                        };
                        VerticalAlignment = VerticalAlignment.Bottom;
                    });

                    item.ForecastType = this.ForecastType;
                    item.DataContext = data;

                    UpdateBarSizing(item, min, max, data);

                    if (data.FillColors != null)
                    {
                        item.SetColors(data.FillColors.Value);
                    }
                    else
                    {
                        item.SetColors();
                    }

                    var barSize = ForecastType switch
                    {
                        Controls.ForecastType.Temperature => 6,
                        _ => 24,
                    };

                    if (item.RangeBarView.Width != barSize)
                    {
                        item.RangeBarView.Width = barSize;
                    }

                    if (InnerLayout.Children.ElementAtOrDefault(i) == null)
                    {
                        InnerLayout.Children.Add(item);
                    }
                }

                InnerLayout.Children.RemoveRange(itemCount, InnerLayout.Children.Count - itemCount);
            }
            else
            {
                InnerLayout.Children.Clear();
            }
        }

        private void RefreshLayout()
        {
            // Recalculate margins/size for existing children using current GraphData
            var dataSet = this.GraphData?.GetDataSet();
            if (dataSet == null || dataSet.IsEmpty) return;

            var max = GraphData.YMax;
            var min = GraphData.YMin;

            InnerLayout.Children?.Cast<ForecastRangeBar>()?.ForEachIndexed<ForecastRangeBar>((idx, bar) =>
            {
                dataSet.GetEntryForIndex(idx)?.Let(data =>
                {
                    UpdateBarSizing(bar, min, max, data);
                });
            });
        }

        private void UpdateBarSizing(ForecastRangeBar bar, float min, float max, ForecastRangeBarEntry entry)
        {
            if (bar == null || entry == null) return;

            // We compute margins as a proportion of available graphable height.
            // Reserve a space for labels/icons inside the control
            var bottomTextHeights = bar.BarHiLoHeight + bar.BarPoPView.Height + bar.BarIconView.Height + bar.BarDateBlock.Height;
            if (bottomTextHeights <= 0 || double.IsNaN(bottomTextHeights)) bottomTextHeights = 180; // placeholder height
            var availableHeight = GraphHeight - bottomTextHeights;

            var range = max - min;
            if (range <= 0) range = 1;

            var topPx = (entry.HiTempData?.Y)?.Let(it => ((max - it) / range) * availableHeight) ?? 0;
            var bottomPx = (entry.LoTempData?.Y)?.Let(it => ((it - min) / range) * availableHeight) ?? 0;

            // apply margins on the InnerBarView so the inner range gradient stretches between hi and lo
            if (bar.InnerBarView is { } inner)
            {
                inner.Margin = new Thickness(inner.Margin.Left, topPx, inner.Margin.Right, bottomPx);
            }

            // Set the RangeBarView.Height so the visible gradient is the remaining space
            if (bar.RangeBarView != null)
            {
                if (entry.HiTempData?.Y == null || entry.HiTempData?.Y == null)
                {
                    bar.RangeBarView.Height = bar.RangeBarView.Width;
                }
                else
                {
                    var height = Math.Max(bar.RangeBarView.Width, availableHeight - (topPx + bottomPx));
                    // RangeBarView is expressed as percent in the original control (0..100),
                    // but here RangeBarView.Height expects device pixels. To preserve the
                    // visual size relationship, clamp it and let layout do the rest.
                    bar.RangeBarView.Height = height;
                }
            }
        }
    }
}
