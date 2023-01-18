using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.NET.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls.Graphs
{
    public sealed partial class BarGraphPanel : UserControl
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

        public bool CanScrollToStart
        {
            get => (bool)GetValue(CanScrollToStartProperty);
            set => SetValue(CanScrollToStartProperty, value);
        }

        public static readonly DependencyProperty CanScrollToStartProperty =
            DependencyProperty.Register(nameof(CanScrollToStart), typeof(bool),
            typeof(BarGraphPanel), new PropertyMetadata(false));

        public bool CanScrollToEnd
        {
            get => (bool)GetValue(CanScrollToEndProperty);
            set => SetValue(CanScrollToEndProperty, value);
        }

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register(nameof(CanScrollToEnd), typeof(bool),
            typeof(BarGraphPanel), new PropertyMetadata(false));

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollLeft(BarChartView.ScrollViewer);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollRight(BarChartView.ScrollViewer);
        }

        public BarGraphData GraphData
        {
            get => (BarGraphData)GetValue(GraphDataProperty);
            set => SetValue(GraphDataProperty, value);
        }

        public static readonly DependencyProperty GraphDataProperty =
            DependencyProperty.Register(nameof(GraphData), typeof(BarGraphData),
            typeof(BarGraphPanel), new PropertyMetadata(null, (o, e) => (o as BarGraphPanel)?.UpdateView(false)));

        public BarGraphPanel()
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
            if (GraphData != null)
            {
                BarChartView.SetData(GraphData);
            }
        }
    }
}