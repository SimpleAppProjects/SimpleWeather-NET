using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls.Graphs
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

        public static readonly DependencyProperty CanScrollToStartProperty =
            DependencyProperty.Register("CanScrollToStart", typeof(bool),
            typeof(BarGraphPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register("CanScrollToEnd", typeof(bool),
            typeof(BarGraphPanel), new PropertyMetadata(false));

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

        public static readonly DependencyProperty GraphDataProperty =
            DependencyProperty.Register("GraphData", typeof(BarGraphData),
            typeof(BarGraphPanel), new PropertyMetadata(null));

        public BarGraphData GraphData
        {
            get { return (BarGraphData)GetValue(GraphDataProperty); }
            set
            {
                SetValue(GraphDataProperty, value);
                UpdateView(false);
            }
        }

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

        private async void UpdateForecastGraph()
        {
            if (GraphData != null)
            {
                while (BarChartView == null || !BarChartView.ReadyToDraw)
                {
                    await Task.Delay(1).ConfigureAwait(true);
                }

                BarChartView.SetData(GraphData);
            }
        }
    }
}