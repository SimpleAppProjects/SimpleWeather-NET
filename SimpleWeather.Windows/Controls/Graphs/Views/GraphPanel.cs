using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.NET.Helpers;
using Windows.UI;

namespace SimpleWeather.NET.Controls.Graphs;

public abstract partial class GraphPanel : UserControl, IGraphPanel
{
    protected abstract GraphScrollView GraphScrollView { get; }
    protected abstract UIElement LeftScrollButton { get; }
    protected abstract UIElement RightScrollButton { get; }

    private ScrollViewer InternalScrollViewer { get; set; }

    public GraphPanel()
    {
        this.Loaded += GraphPanel_Loaded;
        this.Unloaded += GraphPanel_Unloaded;
    }

    private void GraphPanel_Loaded(object sender, RoutedEventArgs e)
    {
        InternalScrollViewer = this.FindChild<ScrollViewer>(includeParent: true);
        if (GraphScrollView != null)
        {
            GraphScrollView.SizeChanged += GraphScrollView_SizeChanged;
        }
    }

    private void GraphPanel_Unloaded(object sender, RoutedEventArgs e)
    {
        if (GraphScrollView != null)
        {
            GraphScrollView.SizeChanged -= GraphScrollView_SizeChanged;
        }
    }

    protected void LeftButton_Click(object sender, RoutedEventArgs e)
    {
        ScrollViewerHelper.ScrollLeft(InternalScrollViewer ?? GraphScrollView.FindChild<ScrollViewer>(includeParent: true));
    }

    protected void RightButton_Click(object sender, RoutedEventArgs e)
    {
        ScrollViewerHelper.ScrollRight(InternalScrollViewer ?? GraphScrollView.FindChild<ScrollViewer>(includeParent: true));
    }

    protected void GraphView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollv)
        {
            UpdateScrollButtons(scrollv);
        }
        else if (sender is GraphScrollView scroller)
        {
            UpdateScrollButtons(scroller.FindChild<ScrollViewer>(includeParent: true));
        }
        else
        {
            UpdateScrollButtons(InternalScrollViewer ?? GraphScrollView.FindChild<ScrollViewer>(includeParent: true));
        }
    }

    protected void GraphView_ItemWidthChanged(object sender, ItemSizeChangedEventArgs e)
    {
        if (sender is FrameworkElement graph && graph.IsVisible() && graph.FindParentOrSelf<ScrollViewer>() is ScrollViewer scrollv)
        {
            UpdateScrollButtons(scrollv);
        }
        else
        {
            UpdateScrollButtons(InternalScrollViewer ?? GraphScrollView.FindChild<ScrollViewer>(includeParent: true));
        }
    }

    private void GraphScrollView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is UIElement view)
        {
            view.InvalidateMeasure();
        }
    }

    private void UpdateScrollButtons(ScrollViewer scroller)
    {
        CanScrollToStart = ScrollViewerHelper.CanScrollToStart(scroller);
        CanScrollToEnd = ScrollViewerHelper.CanScrollToEnd(scroller);
        if (scroller.ExtentWidth > scroller.ViewportWidth)
        {
            LeftScrollButton.Visibility = Visibility.Visible;
            RightScrollButton.Visibility = Visibility.Visible;
        }
        else
        {
            LeftScrollButton.Visibility = Visibility.Collapsed;
            RightScrollButton.Visibility = Visibility.Collapsed;
        }
    }

    public abstract void RequestGraphLayout();
    public abstract int GetItemPositionFromPoint(float xCoordinate);

    public static readonly DependencyProperty CanScrollToStartProperty =
        DependencyProperty.Register(nameof(CanScrollToStart), typeof(bool),
        typeof(GraphPanel), new PropertyMetadata(false));

    public static readonly DependencyProperty CanScrollToEndProperty =
        DependencyProperty.Register(nameof(CanScrollToEnd), typeof(bool),
        typeof(GraphPanel), new PropertyMetadata(false));

    public bool CanScrollToStart
    {
        get => (bool)GetValue(CanScrollToStartProperty);
        set => SetValue(CanScrollToStartProperty, value);
    }

    public bool CanScrollToEnd
    {
        get => (bool)GetValue(CanScrollToEndProperty);
        set => SetValue(CanScrollToEndProperty, value);
    }
    public abstract double GraphMaxWidth { get; set; }
    public abstract bool FillParentWidth { set; }
    public abstract Color BottomTextColor { get; set; }
    public abstract double BottomTextSize { get; set; }
    public abstract float IconSize { get; set; }
    public abstract bool DrawIconLabels { set; }
    public abstract bool DrawDataLabels { set; }
    public abstract bool ScrollingEnabled { get; set; }
}