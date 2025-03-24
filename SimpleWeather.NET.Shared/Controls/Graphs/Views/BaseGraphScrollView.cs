#if WINDOWS
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using SimpleWeather.NET.Helpers;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using ScrollView = SimpleWeather.NET.Controls.Graphs.GraphScrollView;
using Color = Windows.UI.Color;
using ScrollViewerViewChangedEventArgs = Microsoft.UI.Xaml.Controls.ScrollViewerViewChangedEventArgs;
using Size = Windows.Foundation.Size;
#else
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Primitives;
using SimpleWeather.Maui;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using ScrollView = Microsoft.Maui.Controls.ScrollView;
using ScrollViewer = Microsoft.Maui.Controls.ScrollView;
using ScrollViewerViewChangedEventArgs = Microsoft.Maui.Controls.ScrolledEventArgs;
using SKXamlCanvas = SkiaSharp.Views.Maui.Controls.SKCanvasView;
using RoutedEventArgs = System.EventArgs;
using System.Diagnostics;
using Color = Microsoft.Maui.Graphics.Color;
#endif
using SimpleWeather.NET.Utils;
using SimpleWeather.SkiaSharp;
using SkiaSharp;
using RectF = System.Drawing.RectangleF;

namespace SimpleWeather.NET.Controls.Graphs;

public abstract class BaseGraphScrollView<T, S, E> : ScrollView, IGraph where T : GraphData<S, E> where S : GraphDataSet<E> where E : GraphEntry
{
    private ScrollView _scrollView;
    protected RectF visibleRect = new();
    private BaseGraphView<T, S, E> graph;
    private bool scrollEnabled = true;

    protected BaseGraphScrollView()
    {
#if WINDOWS
        this.ViewChanging += ScrollView_ViewChanging;
        this.ViewChanged += ScrollView_ViewChanged;
        this.HorizontalAlignment = this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        this.Height = 275;
#else
        this.Orientation = ScrollOrientation.Horizontal;
        this.VerticalScrollBarVisibility = ScrollBarVisibility.Never;
        this.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
        this.Scrolled += ScrollView_ViewChanged;
        this.HandlerChanged += ScrollView_HandlerChanged;
        this.HeightRequest = 275;
#endif
        _scrollView = this;

        Initialize();
    }

    protected ScrollView ScrollViewer => _scrollView;
    public abstract BaseGraphView<T, S, E> CreateGraphView();
    internal virtual BaseGraphView<T, S, E> Graph => graph;
    
    public event EventHandler<ItemSizeChangedEventArgs> ItemWidthChanged
    {
        add => Graph.ItemWidthChanged += value;
        remove => Graph.ItemWidthChanged -= value;
    }

    private void Initialize()
    {
        this.Content = graph = CreateGraphView();
#if WINDOWS
        Graph.SetBinding(BaseGraphViewControl.MaxCanvasWidthProperty,
            new Binding()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(GraphMaxWidth)),
                Source = this,
            });

        Graph.SetBinding(BaseGraphView<T,S,E>.BottomTextColorProperty,
            new Binding()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(BottomTextColor)),
                Source = this,
            });

        Graph.SetBinding(BaseGraphView<T, S, E>.FontSizeProperty,
            new Binding()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(BottomTextSize)),
                Source = this,
            });

        Graph.SetBinding(BaseGraphView<T, S, E>.IconSizeProperty,
            new Binding()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(IconSize)),
                Source = this,
            });
#else
        Graph.Bind(BaseGraphViewControl.MaxCanvasWidthProperty, 
            getter: view => view.GraphMaxWidth,
            setter: (view, value) => view.GraphMaxWidth = value, 
            BindingMode.OneWay, source: this);

        Graph.Bind(BaseGraphView<T,S,E>.BottomTextColorProperty, 
            getter: view => view.BottomTextColor,
            setter: (view, value) => view.BottomTextColor = value,
            mode: BindingMode.OneWay, source: this);

        Graph.Bind(BaseGraphView<T,S,E>.FontSizeProperty, 
            getter: view => view.BottomTextSize,
            setter: (view, value) => view.BottomTextSize = value, 
            BindingMode.OneWay, source: this);

        Graph.Bind(BaseGraphView<T,S,E>.IconSizeProperty, 
            getter: view => view.IconSize,
            setter: (view, value) => view.IconSize = value, 
            BindingMode.OneWay, source: this);
#endif
    }

    public bool FillParentWidth { set => graph.FillParentWidth = value; }

    public double GraphMaxWidth
    {
        get => (double)GetValue(GraphMaxWidthProperty);
        set => SetValue(GraphMaxWidthProperty, value);
    }

    public Color BottomTextColor
    {
        get => (Color)GetValue(BottomTextColorProperty);
        set => SetValue(BottomTextColorProperty, value);
    }

    public double BottomTextSize
    {
        get => (double)GetValue(BottomTextSizeProperty);
        set => SetValue(BottomTextSizeProperty, value);
    }

    public float IconSize
    {
        get => (float)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

#if WINDOWS
    public static readonly DependencyProperty GraphMaxWidthProperty =
        DependencyProperty.Register(nameof(GraphMaxWidth), typeof(double), typeof(BaseGraphScrollView<T, S, E>), new PropertyMetadata(0d));

    public static readonly DependencyProperty BottomTextColorProperty =
        DependencyProperty.Register(nameof(BottomTextColor), typeof(Color), typeof(BaseGraphScrollView<T, S, E>), new PropertyMetadata(Colors.White));

    public static readonly DependencyProperty BottomTextSizeProperty =
        DependencyProperty.Register(nameof(BottomTextSize), typeof(double), typeof(BaseGraphScrollView<T, S, E>), new PropertyMetadata(13d));

    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(nameof(IconSize), typeof(float), typeof(BaseGraphScrollView<T, S, E>), new PropertyMetadata(48f));

    internal static readonly DependencyProperty MeasureModeProperty =
        DependencyProperty.Register(nameof(MeasureMode), typeof(MeasureMode), typeof(BaseGraphScrollView<T, S, E>), new PropertyMetadata(MeasureMode.Undefined));
#else
    public static readonly BindableProperty GraphMaxWidthProperty =
        BindableProperty.Create(nameof(GraphMaxWidth), typeof(double), typeof(BaseGraphScrollView<T,S,E>), 0d);

    public static readonly BindableProperty BottomTextColorProperty =
        BindableProperty.Create(nameof(BottomTextColor), typeof(Color), typeof(BaseGraphScrollView<T,S,E>), Colors.White);

    public static readonly BindableProperty BottomTextSizeProperty =
        BindableProperty.Create(nameof(BottomTextSize), typeof(double), typeof(BaseGraphScrollView<T,S,E>), 13d);

    public static readonly BindableProperty IconSizeProperty =
        BindableProperty.Create(nameof(IconSize), typeof(float), typeof(BaseGraphScrollView<T,S,E>), 48f);
    
    internal static readonly BindableProperty MeasureModeProperty =
        BindableProperty.Create(nameof(MeasureMode), typeof(MeasureMode), typeof(BaseGraphScrollView<T,S,E>), MeasureMode.Undefined);
#endif

    internal enum MeasureMode
    {
        Undefined,
        AtMost,
        Exactly,
    }

    public bool DrawIconLabels { set => graph.DrawIconLabels = value; }
    public bool DrawDataLabels { set => graph.DrawDataLabels = value; }
    
#if !WINDOWS
    public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged;
#endif

#if WINDOWS
    protected virtual void OnViewChanging(ScrollViewerViewChangingEventArgs e) 
    {
        Invalidate();
    }
#endif
    protected virtual void OnViewChanged(ScrollViewerViewChangedEventArgs e)
    {
        Invalidate();
    }

#if WINDOWS
    private void ScrollView_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
    {
        OnViewChanging(e);
    }
#endif

    private void ScrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        OnViewChanged(e);
    }
#if !WINDOWS
    private void ScrollView_HandlerChanged(object sender, RoutedEventArgs e)
    {
        if (sender is VisualElement { Handler: IPlatformViewHandler handler })
        {
#if IOS || MACCATALYST
            if (handler.PlatformView is UIKit.UIScrollView v)
            {
                v.Bounces = false;
            }
#endif
        }
    }
#endif

    public void Invalidate()
    {
        visibleRect.SetEmpty();
        this.graph.Invalidate();
    }

    public int GetItemPositionFromPoint(float xCoordinate)
    {
        return this.graph.GetItemPositionFromPoint(xCoordinate);
    }
    
    public T GetData() => graph.Data;
    public void SetData(T data) => graph.SetData(data);
    public void ResetData(bool invalidate = false) => graph.ResetData(invalidate);
    public void RequestGraphLayout() => graph.InvalidateMeasure();

#if WINDOWS
    protected override Size MeasureOverride(Size availableSize)
    {
        this.MeasuredSize = Size.Empty;
        base.MeasureOverride(availableSize);
        var size = availableSize;
        this.MeasuredSize = size;
#else
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        this.DesiredSize = Size.Zero;
        var size = base.MeasureOverride(widthConstraint, heightConstraint);
        if (size.Width <= 0) size.Width = widthConstraint;
        this.DesiredSize = size;
#endif

        double widthPadding =
#if WINDOWS
            Padding.Left + Padding.Right;
#else
                Padding.HorizontalThickness;
#endif
        double heightPadding =
#if WINDOWS
            Padding.Top + Padding.Bottom;
#else
                Padding.VerticalThickness;
#endif

#if WINDOWS
        if (Content is FrameworkElement child)
#else
        if (Content is { } child)
#endif
        {
            double desiredWidth = size.Width - widthPadding;

            Content.ClearValue(MeasureModeProperty);
#if WINDOWS
            child.Measure(size);
            var childMeasuredSize = child.DesiredSize;
            if (childMeasuredSize.Width < desiredWidth)
            {
                Content.SetValue(MeasureModeProperty, MeasureMode.Undefined);
                Content.Measure(new Size(desiredWidth, size.Height));
            }
            else
            {
                Content.SetValue(MeasureModeProperty, MeasureMode.AtMost);
                Content.Measure(new Size(childMeasuredSize.Width, size.Height));
            }
#else
            var childMeasuredSize = child.Measure(widthConstraint, heightConstraint);
            if (childMeasuredSize.Width < desiredWidth)
            {
                Content.SetValue(MeasureModeProperty, MeasureMode.Undefined);
                Content.Measure(desiredWidth, size.Height);
            }
            else
            {
                Content.SetValue(MeasureModeProperty, MeasureMode.AtMost);
                Content.Measure(childMeasuredSize.Width, size.Height);
            }
#endif
        }

        Invalidate();
        
        return new Size(Content.DesiredSize.Width + widthPadding, Content.DesiredSize.Height + heightPadding);
    }

#if WINDOWS
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
#else
    protected override Size ArrangeOverride(Rect bounds)
    {
        var size = base.ArrangeOverride(bounds);
#endif

#if WINDOWS
        if (Content is FrameworkElement child && child.Visibility == Visibility.Visible)
#else
        if (Content is View { IsVisible: true })
#endif
        {
#if WINDOWS
            double parentLeft = Padding.Left;
            double parentRight = size.Width - Padding.Right;

            double width = child.ActualWidth;
            double height = child.ActualHeight;

            if (width < size.Width)
            {
                double childLeft = parentLeft + (parentRight - parentLeft - width) / 2 + (Margin.Left + Margin.Right);
                
                Content.Arrange(new Rect(childLeft, Margin.Top, width, height));
            }
#else
            double parentLeft = Padding.Left;
            double parentRight = bounds.Right - bounds.Left - Padding.Right;

            double width = Content.Width;
            double height = Content.Height;

            if (width < bounds.Width)
            {
                double childLeft = parentLeft + (parentRight - parentLeft - width) / 2 + Margin.HorizontalThickness;
                
                Content.Arrange(Rect.FromLTRB(childLeft, bounds.Top, childLeft + width, bounds.Top + height));
            }
#endif
        }

        return size;
    }

#if !WINDOWS
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        InvalidateMeasure();
    }
#endif
}