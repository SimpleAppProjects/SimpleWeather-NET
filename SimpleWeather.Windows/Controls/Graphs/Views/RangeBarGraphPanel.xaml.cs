using SimpleWeather.NET.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls.Graphs
{
    public sealed partial class RangeBarGraphPanel : UserControl
    {
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming cause the view to change.
        public event TappedEventHandler GraphViewTapped
        {
            add
            {
                BarChartView.Tapped += value;
            }
            remove
            {
                BarChartView.Tapped -= value;
            }
        }

        private void GraphView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender is ScrollViewer scroller)
            {
                UpdateScrollButtons(scroller);
            }
        }

        private void GraphView_ItemWidthChanged(object sender, ItemSizeChangedEventArgs e)
        {
            if (sender is IGraph graph && graph.Control.Visibility == Visibility.Visible)
            {
                UpdateScrollButtons(graph.ScrollViewer);
            }
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

        public static readonly DependencyProperty CanScrollToStartProperty =
            DependencyProperty.Register("CanScrollToStart", typeof(bool),
            typeof(RangeBarGraphPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register("CanScrollToEnd", typeof(bool),
            typeof(RangeBarGraphPanel), new PropertyMetadata(false));

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

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollLeft(BarChartView.ScrollViewer);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollRight(BarChartView.ScrollViewer);
        }

        public static readonly DependencyProperty ForecastDataProperty =
            DependencyProperty.Register("ForecastData", typeof(RangeBarGraphData),
            typeof(RangeBarGraphPanel), new PropertyMetadata(null));

        public RangeBarGraphData ForecastData
        {
            get => (RangeBarGraphData)GetValue(ForecastDataProperty);
            set
            {
                SetValue(ForecastDataProperty, value);
                UpdateView(false);
            }
        }

        public RangeBarGraphPanel()
        {
            this.InitializeComponent();
        }

        private void GraphView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateView(true);
        }

        private void UpdateView(bool resetOffset)
        {
            BarChartView?.ResetData();

            UpdateForecastGraph();

            if (resetOffset)
            {
                BarChartView.ScrollViewer?.ChangeView(0, null, null);
            }
        }

        private void UpdateForecastGraph()
        {
            if (ForecastData != null)
            {
                BarChartView.SetData(ForecastData);
            }
        }
    }
}