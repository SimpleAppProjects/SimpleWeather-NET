using CommunityToolkit.Maui.Markup;
using SimpleWeather.Icons;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls;

public class ForecastBarGraphView : Grid
{
    public double GraphHeight
    {
        get => (double)GetValue(GraphHeightProperty);
        set => SetValue(GraphHeightProperty, value);
    }

    public static readonly BindableProperty GraphHeightProperty =
        BindableProperty.Create(nameof(GraphHeight), typeof(double), typeof(ForecastBarGraphView), 150d);

    private double zeroValueItemHeight;
    private double bottomTextHeights = 0;

    private BarGraphData graphData = null;

    public ForecastType? ForecastType
    {
        get => (ForecastType?)GetValue(ForecastTypeProperty);
        set => SetValue(ForecastTypeProperty, value);
    }

    public static readonly BindableProperty ForecastTypeProperty =
        BindableProperty.Create(nameof(ForecastType), typeof(ForecastType?), typeof(ForecastBarGraphView), null);

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
            typeof(ForecastBarGraphView), false);

    public bool CanScrollToEnd
    {
        get => (bool)GetValue(CanScrollToEndProperty);
        set => SetValue(CanScrollToEndProperty, value);
    }

    public static readonly BindableProperty CanScrollToEndProperty =
        BindableProperty.Create(nameof(CanScrollToEnd), typeof(bool),
            typeof(ForecastBarGraphView), false);

    public ForecastBarGraphView()
    {
        HorizontalOptions = LayoutOptions.CenterAndExpand;
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
            static (ForecastBarGraphView view) => view.GraphHeight, BindingMode.OneWay,
            source: this);

        this.SetBinding(MinimumHeightRequestProperty,
            static (ForecastBarGraphView view) => view.GraphHeight, BindingMode.OneWay,
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

        SetData(BindingContext as BarGraphData);
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var size = base.MeasureOverride(widthConstraint, heightConstraint);

        var barIconHeight = 32;
        var heightChanged = false;

        InnerLayout.Children?.Cast<ForecastBar>()?.ForEachIndexed<ForecastBar>((idx, bar) =>
        {
            if (idx == 0)
            {
                var measuredBottomTextHeights =
                    (bar.BarValueBlock.DesiredSize.Height + bar.BarDateBlock.DesiredSize.Height);
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
            size = base.MeasureOverride(widthConstraint, heightConstraint);

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
                    (bar.InnerBarView as IView)?.InvalidateArrange();
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
                    it.GestureRecognizers.Clear();
                    it.TapGesture(() =>
                    {
                        ItemClick?.Invoke(this, new ItemTappedEventArgs(Children, it, Math.Max(Children.IndexOf(it), 0)));
                    });
                    it.VerticalOptions = LayoutOptions.End;
                });

                item.BindingContext = data;

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

    private bool UpdateInnerBarHeight(View innerBar, float min, float max, BarGraphEntry data)
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
                var y = data.EntryData.Y;
                if (y < min) y = min; else if (y > max) y = max;
                normalizedValue = (y - min) / dataRange;
            }
        }

        var newHeight = zeroValueItemHeight + (normalizedValue * (GraphHeight - bottomTextHeights));

        if (innerBar.HeightRequest != newHeight)
        {
            innerBar.HeightRequest = newHeight;
            heightChanged = true;
        }

        return heightChanged;
    }

    private static string GetWeatherIconFromForecastType(ForecastType? forecastType) => forecastType switch
    {
        NET.Controls.ForecastType.Temperature => null,
        NET.Controls.ForecastType.Minutely or NET.Controls.ForecastType.Precipitation => WeatherIcons.RAINDROP,
        NET.Controls.ForecastType.Wind => WeatherIcons.DIRECTION_UP,
        NET.Controls.ForecastType.Humidity => WeatherIcons.HUMIDITY,
        NET.Controls.ForecastType.UVIndex => null,
        NET.Controls.ForecastType.Rain => WeatherIcons.RAINDROP,
        NET.Controls.ForecastType.Snow => WeatherIcons.SNOWFLAKE_COLD,
        NET.Controls.ForecastType.AirQuality => null,
        _ => null,
    };
}

