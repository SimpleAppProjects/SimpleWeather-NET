using SimpleWeather.NET.Controls.Graphs;
using System;
using System.Linq;
using CommunityToolkit.Maui.Core.Extensions;
using SimpleWeather.Utils;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;
using SimpleWeather.Maui.Helpers;
using Microsoft.Maui.Controls;
using SimpleWeather.NET.Controls;

namespace SimpleWeather.Maui.Controls;

public class ForecastRangeBarGraphView : Grid
{
    private int MaxItemCount = 7;

    public double GraphHeight
    {
        get => (double)GetValue(GraphHeightProperty);
        set => SetValue(GraphHeightProperty, value);
    }

    public static readonly BindableProperty GraphHeightProperty =
        BindableProperty.Create(nameof(GraphHeight), typeof(double), typeof(ForecastRangeBarGraphView), 150d, propertyChanged:
            (o, _, _) => (o as  ForecastRangeBarGraphView)?.RefreshLayout());

    private ForecastRangeBarGraphData GraphData = null;
    public ForecastType? ForecastType { get; set; } = null;
    
    private readonly HorizontalStackLayout InnerLayout;
    private readonly ImageButton LeftButton;
    private readonly ImageButton RightButton;
    private readonly ScrollView ScrollView;

    public event EventHandler<ItemTappedEventArgs> ItemClick;

    public bool CanScrollToStart
    {
        get => (bool)GetValue(CanScrollToStartProperty);
        set => SetValue(CanScrollToStartProperty, value);
    }

    public static readonly BindableProperty CanScrollToStartProperty =
        BindableProperty.Create(nameof(CanScrollToStart), typeof(bool),
            typeof(ForecastRangeBarGraphView), false);

    public bool CanScrollToEnd
    {
        get => (bool)GetValue(CanScrollToEndProperty);
        set => SetValue(CanScrollToEndProperty, value);
    }

    public static readonly BindableProperty CanScrollToEndProperty =
        BindableProperty.Create(nameof(CanScrollToEnd), typeof(bool),
            typeof(ForecastRangeBarGraphView), false);

    public ForecastRangeBarGraphView()
    {
        HorizontalOptions = LayoutOptions.FillAndExpand;
        Clear();

        ColumnDefinitions =
        [
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Star },
            new ColumnDefinition { Width = GridLength.Auto }
        ];
        
        Children.Add(LeftButton = new ImageButton().Apply(it =>
        {
            it.Clicked += LeftButton_Click;
            it.Bind(IsEnabledProperty, static view => view.CanScrollToStart, mode: BindingMode.OneWay, source: this);
            it.DynamicResource(StyleProperty, "LeftArrowButton");
            Grid.SetColumn(it, 0);
        }));
        
        Children.Add(RightButton = new ImageButton().Apply(it =>
        {
            it.Clicked += RightButton_Click;
            it.Bind(IsEnabledProperty, static view => view.CanScrollToEnd, mode: BindingMode.OneWay, source: this);
            it.DynamicResource(StyleProperty, "RightArrowButton");
            Grid.SetColumn(it, 2);
        }));
        
        Children.Add(ScrollView = new ScrollView()
        {
            Orientation = ScrollOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            VerticalScrollBarVisibility = ScrollBarVisibility.Never,
            Content = InnerLayout = new HorizontalStackLayout()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.End,
            }
        }.Apply(it =>
        {
            it.Scrolled += ScrollView_OnScrolled;
            it.SizeChanged += ScrollView_OnSizeChanged;
            Grid.SetColumn(it, 1);
        }));
        
        InnerLayout.SetBinding(MinimumHeightRequestProperty,
            static (ForecastRangeBarGraphView view) => view.GraphHeight, BindingMode.OneWay,
            source: this);
        
        this.SetBinding(MinimumHeightRequestProperty,
            static (ForecastRangeBarGraphView view) => view.GraphHeight, BindingMode.OneWay,
            source: this);
    }

    private void UpdateScrollButtons(ScrollView scroller)
    {
        CanScrollToStart = ScrollViewerHelper.CanScrollToStart(scroller);
        CanScrollToEnd = ScrollViewerHelper.CanScrollToEnd(scroller);

        if (DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
        {
            if (scroller.ContentSize.Width > scroller.Width)
            {
                LeftButton.IsVisible = RightButton.IsVisible = true;
            }
            else
            {
                LeftButton.IsVisible = RightButton.IsVisible = false;
            }
        }
        else
        {
            LeftButton.IsVisible = RightButton.IsVisible = false;
        }
    }
    
    private void LeftButton_Click(object sender, EventArgs e)
    {
        ScrollViewerHelper.ScrollLeft(ScrollView);
    }

    private void RightButton_Click(object sender, EventArgs e)
    {
        ScrollViewerHelper.ScrollRight(ScrollView);
    }

    private void ScrollView_OnScrolled(object sender, ScrolledEventArgs e)
    {
        if (sender is ScrollView scroller)
        {
            UpdateScrollButtons(scroller);
        }
    }

    private void ScrollView_OnSizeChanged(object sender, EventArgs e)
    {
        if (sender is ScrollView scroller)
        {
            UpdateScrollButtons(scroller);
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        SetData(BindingContext as ForecastRangeBarGraphData);
    }

    public void SetData(ForecastRangeBarGraphData graphData)
    {
        this.GraphData = graphData;

        var dataSet = graphData?.GetDataSet();

        if (dataSet is { IsEmpty: false })
        {
            var itemCount = Math.Min(dataSet.DataCount, MaxItemCount);

            var max = dataSet.YMax;
            var min = dataSet.YMin;

            for (int i = 0; i < itemCount; i++)
            {
                var data = dataSet.GetEntryForIndex(i);
                var item = Children.ElementAtOrDefault(i) as ForecastRangeBar ?? new ForecastRangeBar();

                item.BindingContext = data;
                item.ForecastType = this.ForecastType;
                item.GestureRecognizers.Clear();
                item.TapGesture(() =>
                {
                    ItemClick?.Invoke(this, new ItemTappedEventArgs(Children, item, Math.Max(Children.IndexOf(item), 0)));
                });
                item.VerticalOptions = LayoutOptions.Fill;

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
                    NET.Controls.ForecastType.Temperature => 8,
                    _ => 24,
                };

                if (item.RangeBarView.Width != barSize)
                {
                    item.RangeBarView.WidthRequest = barSize;
                }

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

    private void RefreshLayout()
    {
        var dataSet = GraphData?.GetDataSet();
        if (dataSet is not { IsEmpty: true }) return;
        
        var max = GraphData.YMax;
        var min = GraphData.YMin;

        InnerLayout.Children?.Cast<ForecastRangeBar>()?.ForEachIndexed<ForecastRangeBar>((idx, bar) =>
        {
            // Update bar measurement
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
        if (bottomTextHeights <= 0) bottomTextHeights = 180; // placeholder height
        var availableHeight = GraphHeight - bottomTextHeights;

        var range = max - min;
        if (range <= 0) range = 1;

        var topPx = (entry.HiTempData?.Y)?.Let(it => ((max - it) / range) * availableHeight) ?? 0;
        var bottomPx = (entry.LoTempData?.Y)?.Let(it => ((it - min) / range) * availableHeight) ?? 0;

        // apply margins on the InnerBarView so the inner range gradient stretches between hi and lo
        if (bar.InnerBarView is { } inner)
        {
            inner.Margins(inner.Margin.Left, topPx, inner.Margin.Right, bottomPx);
        }

        // Set the RangeBarView.Height so the visible gradient is the remaining space
        if (bar.RangeBarView != null)
        {
            if (entry.HiTempData?.Y == null || entry.HiTempData?.Y == null)
            {
                bar.RangeBarView.HeightRequest = bar.RangeBarView.WidthRequest;
            }
            else
            {
                var height = Math.Max(bar.RangeBarView.WidthRequest, availableHeight - (topPx + bottomPx));
                // RangeBarView is expressed as percent in the original control (0..100),
                // but here RangeBarView.Height expects device pixels. To preserve the
                // visual size relationship, clamp it and let layout do the rest.
                bar.RangeBarView.HeightRequest = height;
            }
        }
    }
}

