using SimpleWeather.Maui.Helpers;
using SimpleWeather.NET.Controls.Graphs;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Maui.Controls.Graphs
{
    public sealed partial class BarGraphPanel : ContentView, IGraphPanel
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
            if (sender is View { IsVisible: true } graph)
            {
                UpdateScrollButtons(graph.Parent as ScrollView);
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
            typeof(BarGraphPanel), false);

        public bool CanScrollToEnd
        {
            get => (bool)GetValue(CanScrollToEndProperty);
            set => SetValue(CanScrollToEndProperty, value);
        }

        public static readonly BindableProperty CanScrollToEndProperty =
            BindableProperty.Create(nameof(CanScrollToEnd), typeof(bool),
            typeof(BarGraphPanel), false);

        private void LeftButton_Click(object sender, EventArgs e)
        {
            ScrollViewerHelper.ScrollLeft(BarChartView);
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            ScrollViewerHelper.ScrollRight(BarChartView);
        }

        public BarGraphData GraphData
        {
            get => (BarGraphData)GetValue(GraphDataProperty);
            set => SetValue(GraphDataProperty, value);
        }

        public static readonly BindableProperty GraphDataProperty =
            BindableProperty.Create(nameof(GraphData), typeof(BarGraphData),
            typeof(BarGraphPanel), null, propertyChanged: (o, _, _) => (o as BarGraphPanel)?.UpdateView(false));

        public BarGraphPanel()
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
                if (BarChartView != null)
                    await BarChartView.ScrollToAsync(0, 0, false);
            }
        }

        private void UpdateForecastGraph()
        {
            if (GraphData != null)
            {
                BarChartView.SetData(GraphData);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            GraphViewTapped?.Invoke(sender, e);
        }

        private void ClickGestureRecognizer_Clicked(object sender, EventArgs e)
        {
#if WINDOWS || MACCATALYST
            GraphViewTapped?.Invoke(sender, e);
#endif
        }

        public int GetItemPositionFromPoint(float xCoordinate) => BarChartView.GetItemPositionFromPoint(xCoordinate);
        public double GraphMaxWidth { get => BarChartView.GraphMaxWidth; set => BarChartView.GraphMaxWidth = value; }
        public bool FillParentWidth { set => BarChartView.FillParentWidth = value; }
        public Color BottomTextColor { get => BarChartView.BottomTextColor; set => BarChartView.BottomTextColor = value; }
        public double BottomTextSize { get => BarChartView.BottomTextSize; set => BarChartView.BottomTextSize = value; }
        public float IconSize { get => BarChartView.IconSize; set => BarChartView.IconSize = value; }
        public bool DrawIconLabels { set => BarChartView.DrawIconLabels = value; }
        public bool DrawDataLabels { set => BarChartView.DrawDataLabels = value; }
        public bool ScrollingEnabled { get => !BarChartView.InputTransparent; set => BarChartView.InputTransparent = !value; }
        public void RequestGraphLayout() => BarChartView.RequestGraphLayout();
    }
}