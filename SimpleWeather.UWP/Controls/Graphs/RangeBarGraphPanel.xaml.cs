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
            typeof(RangeBarGraphPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register("CanScrollToEnd", typeof(bool),
            typeof(RangeBarGraphPanel), new PropertyMetadata(false));

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
            ScrollViewerHelper.ScrollLeft(BarChartView.ScrollViewer);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollRight(BarChartView.ScrollViewer);
        }

        public static readonly DependencyProperty ForecastDataProperty =
            DependencyProperty.Register("ForecastData", typeof(RangeBarGraphViewModel),
            typeof(RangeBarGraphPanel), new PropertyMetadata(null));

        public RangeBarGraphViewModel ForecastData
        {
            get { return (RangeBarGraphViewModel)GetValue(ForecastDataProperty); }
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
                BarChartView.ScrollViewer.ChangeView(0, null, null);
            }
        }

        private async void UpdateForecastGraph()
        {
            if (ForecastData != null)
            {
                while (BarChartView == null || !BarChartView.ReadyToDraw)
                {
                    await Task.Delay(1).ConfigureAwait(true);
                }

                BarChartView.DrawDataLabels = true;
                BarChartView.DrawIconLabels = true;
                BarChartView.CenterGraphView = true;

                if (ForecastData?.LabelData != null && ForecastData?.TempData != null)
                {
                    BarChartView.SetData(ForecastData.LabelData, ForecastData.TempData);
                }
            }
        }
    }
}