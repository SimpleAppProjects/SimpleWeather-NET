using SimpleWeather.Maui.Helpers;
using SimpleWeather.NET.Controls.Graphs;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Maui.Controls.Graphs
{
    public sealed partial class ForecastGraphPanel : ContentView, IGraphPanel
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
            typeof(ForecastGraphPanel), false);

        public bool CanScrollToEnd
        {
            get => (bool)GetValue(CanScrollToEndProperty);
            set => SetValue(CanScrollToEndProperty, value);
        }

        public static readonly BindableProperty CanScrollToEndProperty =
            BindableProperty.Create(nameof(CanScrollToEnd), typeof(bool),
            typeof(ForecastGraphPanel), false);

        private void LeftButton_Click(object sender, EventArgs e)
        {
            ScrollViewerHelper.ScrollLeft(LineGraphView);
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            ScrollViewerHelper.ScrollRight(LineGraphView);
        }

        public LineViewData GraphData
        {
            get => (LineViewData)GetValue(GraphDataProperty);
            set => SetValue(GraphDataProperty, value);
        }

        // Using a DependencyProperty as the backing store for GraphData.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty GraphDataProperty =
            BindableProperty.Create(nameof(GraphData), typeof(LineViewData), typeof(ForecastGraphPanel), null, propertyChanged: (o, _, _) => (o as ForecastGraphPanel)?.UpdateView(false));

        public ForecastGraphPanel()
        {
            this.InitializeComponent();
        }

        private void GraphView_Loaded(object sender, EventArgs e)
        {
            UpdateView(true);
        }

        private async void UpdateView(bool resetOffset)
        {
            LineGraphView?.ResetData();

            UpdateForecastGraph();

            if (resetOffset)
            {
                if (LineGraphView != null)
                    await LineGraphView.ScrollToAsync(0, 0, false);
            }
        }

        private void UpdateForecastGraph()
        {
            if (GraphData != null)
            {
                LineGraphView.SetData(GraphData);
                LineGraphView.DrawSeriesLabels = GraphData?.DataSets?.Any(set => !string.IsNullOrWhiteSpace(set.SeriesLabel)) == true;
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

        public int GetItemPositionFromPoint(float xCoordinate) => LineGraphView.GetItemPositionFromPoint(xCoordinate);
        public double GraphMaxWidth { get => LineGraphView.GraphMaxWidth; set => LineGraphView.GraphMaxWidth = value; }
        public bool FillParentWidth { set => LineGraphView.FillParentWidth = value; }
        public Color BottomTextColor { get => LineGraphView.BottomTextColor; set => LineGraphView.BottomTextColor = value; }
        public double BottomTextSize { get => LineGraphView.BottomTextSize; set => LineGraphView.BottomTextSize = value; }
        public float IconSize { get => LineGraphView.IconSize; set => LineGraphView.IconSize = value; }
        public bool DrawIconLabels { set => LineGraphView.DrawIconLabels = value; }
        public bool DrawDataLabels { set => LineGraphView.DrawDataLabels = value; }
        public bool ScrollingEnabled { get => !LineGraphView.InputTransparent; set => LineGraphView.InputTransparent = !value; }
        public void RequestGraphLayout() => LineGraphView.RequestGraphLayout();
    }
}