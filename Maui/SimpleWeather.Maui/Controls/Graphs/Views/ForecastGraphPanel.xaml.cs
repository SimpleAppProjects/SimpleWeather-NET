using SimpleWeather.Maui.Helpers;
using SimpleWeather.NET.Controls.Graphs;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Maui.Controls.Graphs
{
    public sealed partial class ForecastGraphPanel : ContentView
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
            ScrollViewerHelper.ScrollLeft(LineGraphView.ScrollViewer);
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            ScrollViewerHelper.ScrollRight(LineGraphView.ScrollViewer);
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
                await LineGraphView.ScrollViewer?.ScrollToAsync(0, 0, false);
            }
        }

        private void UpdateForecastGraph()
        {
            if (GraphData != null)
            {
                LineGraphView.SetData(GraphData);
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
    }
}