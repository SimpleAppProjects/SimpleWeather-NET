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
    public sealed partial class ForecastGraphPanel : UserControl
    {
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming cause the view to change.
        public event TappedEventHandler GraphViewTapped
        {
            add
            {
                LineGraphView.Tapped += value;
            }
            remove
            {
                LineGraphView.Tapped -= value;
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
            if (sender is IGraph graph && graph.GetControl().Visibility == Visibility.Visible)
            {
                UpdateScrollButtons(graph.GetScrollViewer());
            }
        }

        private void UpdateScrollButtons(ScrollViewer scroller)
        {
            CanScrollToStart = ScrollViewerHelper.CanScrollToStart(scroller);
            CanScrollToEnd = ScrollViewerHelper.CanScrollToEnd(scroller);
        }

        public static readonly DependencyProperty CanScrollToStartProperty =
            DependencyProperty.Register("CanScrollToStart", typeof(bool),
            typeof(ForecastGraphPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register("CanScrollToEnd", typeof(bool),
            typeof(ForecastGraphPanel), new PropertyMetadata(false));

        public bool CanScrollToStart
        {
            get { return (bool)GetValue(CanScrollToStartProperty); }
            set { SetValue(CanScrollToStartProperty, value); }
        }

        public bool CanScrollToEnd
        {
            get { return (bool)GetValue(CanScrollToEndProperty); }
            set { SetValue(CanScrollToEndProperty, value); }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollLeft(LineGraphView.ScrollViewer);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollRight(LineGraphView.ScrollViewer);
        }

        public ForecastGraphViewModel GraphData
        {
            get { return (ForecastGraphViewModel)GetValue(GraphDataProperty); }
            set { SetValue(GraphDataProperty, value); UpdateView(false); }
        }

        // Using a DependencyProperty as the backing store for GraphData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphDataProperty =
            DependencyProperty.Register("GraphData", typeof(ForecastGraphViewModel), typeof(ForecastGraphPanel), new PropertyMetadata(null));

        public ForecastGraphPanel()
        {
            this.InitializeComponent();
        }

        private void GraphView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateView(true);
        }

        private void UpdateView(bool resetOffset)
        {
            LineGraphView.ResetData();

            UpdateGraph();

            if (resetOffset)
            {
                LineGraphView.ScrollViewer.ChangeView(0, null, null);
            }
        }

        private async void UpdateGraph()
        {
            if (GraphData != null)
            {
                while (LineGraphView == null || !LineGraphView.ReadyToDraw)
                {
                    await Task.Delay(1).ConfigureAwait(true);
                }

                LineGraphView.DrawGridLines = false;
                LineGraphView.DrawDotLine = false;
                LineGraphView.DrawDataLabels = true;
                LineGraphView.DrawIconLabels = false;
                LineGraphView.DrawGraphBackground = true;
                LineGraphView.DrawDotPoints = false;
                LineGraphView.DrawSeriesLabels = false;

                LineGraphView.SetData(GraphData.LabelData, GraphData.SeriesData);
            }
        }
    }
}