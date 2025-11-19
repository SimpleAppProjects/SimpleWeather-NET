using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.NET.Controls.Graphs;
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

        private double bottomTextHeights = 0;
        private float scale = 1f;

        private ForecastRangeBarGraphData GraphData = null;
        public ForecastType? ForecastType { get; set; } = null;

        private readonly StackPanel InnerLayout;

        public event EventHandler<TappedRoutedEventArgs> ItemClick;

        public ForecastRangeBarGraphView()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Children.Clear();

            Children.Add(new ScrollViewer()
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
            });

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

        public int GetItemPosition(object item)
        {
            var index = Children.IndexOf(item);
            return index >= 0 ? index : 0;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);

            var measuredBottomTextHeights = double.MinValue;
            var newScale = 0f;

            InnerLayout.Children?.Cast<ForecastRangeBar>()?.ForEach<ForecastRangeBar>(bar =>
            {
                measuredBottomTextHeights = Math.Max(measuredBottomTextHeights, (bar.BarPoPView.DesiredSize.Height + bar.BarIconView.DesiredSize.Height + bar.BarDateBlock.DesiredSize.Height));

                if (measuredBottomTextHeights != bottomTextHeights)
                {
                    bottomTextHeights = measuredBottomTextHeights;
                }

                if (GraphData != null)
                {
                    var dataBarHeight = GraphData.YMax - GraphData.YMin;

                    newScale = MathF.Max(
                        dataBarHeight == 0 ? 1f : MathF.Min((float)(GraphHeight - bottomTextHeights - (bar.BarHiLoHeight)) / dataBarHeight, 1f),
                        newScale);
                }
            });

            if (newScale != scale && newScale != 0f)
            {
                scale = newScale;
            }

            InnerLayout.Children?.Cast<ForecastRangeBar>()?.ForEachIndexed<ForecastRangeBar>((idx, bar) =>
            {
                // Update bar measurement
                GraphData?.GetDataSet()?.GetEntryForIndex(idx)?.Let(data =>
                {
                    var max = GraphData?.YMax;
                    var min = GraphData?.YMin;
                    if (min.HasValue && max.HasValue)
                    {
                        UpdateInnerBarMargins(bar.InnerBarView, min.Value, max.Value, data);
                    }
                });
            });

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

                    if (data.HiTempData == null || data.LoTempData == null)
                    {
                        item.RangeBarView.Height = item.RangeBarView.Width;
                    }
                    else
                    {
                        item.RangeBarView.Height = 100 - (item.InnerBarView.Margin.Top + item.InnerBarView.Margin.Bottom);
                    }

                    if (data.FillColors != null)
                    {
                        item.SetColors(data.FillColors.Value);
                    }
                    else
                    {
                        item.SetColors();
                    }

                    UpdateInnerBarMargins(item.InnerBarView, min, max, data);

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

        private void UpdateInnerBarMargins(FrameworkElement innerBar, float min, float max, ForecastRangeBarEntry data)
        {
            innerBar.Margin = new Thickness(
                        innerBar.Margin.Left,
                        (data.HiTempData?.Y)?.Let(it => (max - it) * scale) ?? 0,
                        innerBar.Margin.Right,
                        (data.LoTempData?.Y)?.Let(it => (it - min) * scale) ?? 0);
        }
    }
}
