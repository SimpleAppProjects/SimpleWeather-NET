using SimpleWeather.Maui.Helpers;
using SimpleWeather.NET.Controls.Graphs;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Maui.Controls.Graphs
{
    public sealed partial class ForecastRangeBarGraphPanel : ContentView
    {
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming cause the view to change.
        public event EventHandler GraphViewTapped;

        private void GraphView_ViewChanged(object sender, ScrolledEventArgs e)
        {
            if (sender is ScrollView scroller)
            {
                UpdateScrollButtons(scroller);
            }
        }

        private void GraphView_ItemWidthChanged(object sender, ItemSizeChangedEventArgs e)
        {
            if (sender is IGraph graph && graph.Control.IsVisible)
            {
                UpdateScrollButtons(graph.ScrollViewer);
            }
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

        public bool CanScrollToStart
        {
            get => (bool)GetValue(CanScrollToStartProperty);
            set => SetValue(CanScrollToStartProperty, value);
        }

        public static readonly BindableProperty CanScrollToStartProperty =
            BindableProperty.Create(nameof(CanScrollToStart), typeof(bool),
            typeof(ForecastRangeBarGraphPanel), false);

        public bool CanScrollToEnd
        {
            get => (bool)GetValue(CanScrollToEndProperty);
            set => SetValue(CanScrollToEndProperty, value);
        }

        public static readonly BindableProperty CanScrollToEndProperty =
            BindableProperty.Create(nameof(CanScrollToEnd), typeof(bool),
            typeof(ForecastRangeBarGraphPanel), false);

        private void LeftButton_Click(object sender, EventArgs e)
        {
            ScrollViewerHelper.ScrollLeft(BarChartView.ScrollViewer);
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            ScrollViewerHelper.ScrollRight(BarChartView.ScrollViewer);
        }

        public ForecastRangeBarGraphData ForecastData
        {
            get => (ForecastRangeBarGraphData)GetValue(ForecastDataProperty);
            set => SetValue(ForecastDataProperty, value);
        }

        public static readonly BindableProperty ForecastDataProperty =
            BindableProperty.Create(nameof(ForecastData), typeof(ForecastRangeBarGraphData),
            typeof(ForecastRangeBarGraphPanel), null, propertyChanged: (o, _, _) => (o as ForecastRangeBarGraphPanel)?.UpdateView(false));

        public ForecastRangeBarGraphPanel()
        {
            this.InitializeComponent();
        }

        private void GraphView_Loaded(object sender, EventArgs e)
        {
            UpdateView(true);
        }

        private async void UpdateView(bool resetOffset)
        {
            BarChartView?.ResetData();

            UpdateForecastGraph();

            if (resetOffset)
            {
                await BarChartView.ScrollViewer?.ScrollToAsync(0, 0, false);
            }
        }

        private void UpdateForecastGraph()
        {
            if (ForecastData != null)
            {
                BarChartView.SetData(ForecastData);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            GraphViewTapped?.Invoke(sender, e);
        }
    }
}