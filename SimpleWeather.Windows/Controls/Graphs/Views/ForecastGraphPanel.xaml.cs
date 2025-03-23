using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.NET.Helpers;
using Windows.UI;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls.Graphs
{
    public sealed partial class ForecastGraphPanel : GraphPanel
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

        public LineViewData GraphData
        {
            get => (LineViewData)GetValue(GraphDataProperty);
            set => SetValue(GraphDataProperty, value);
        }

        // Using a DependencyProperty as the backing store for GraphData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphDataProperty =
            DependencyProperty.Register(nameof(GraphData), typeof(LineViewData), typeof(ForecastGraphPanel), new PropertyMetadata(null, (o, e) => (o as ForecastGraphPanel)?.UpdateView(false)));

        protected override GraphScrollView GraphScrollView => LineGraphView;
        protected override UIElement LeftScrollButton => this.LeftButton;
        protected override UIElement RightScrollButton => this.RightButton;

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
                LineGraphView?.ScrollTo(0, 0);
            }
        }

        private void UpdateGraph()
        {
            if (GraphData != null)
            {
                LineGraphView.SetData(GraphData);
                LineGraphView.DrawSeriesLabels = GraphData?.DataSets?.Any(set => !string.IsNullOrWhiteSpace(set.SeriesLabel)) == true;
            }
        }

        public override int GetItemPositionFromPoint(float xCoordinate) => LineGraphView.GetItemPositionFromPoint(xCoordinate);
        public override double GraphMaxWidth { get => LineGraphView.GraphMaxWidth; set => LineGraphView.GraphMaxWidth = value; }
        public override bool FillParentWidth { set => LineGraphView.FillParentWidth = value; }
        public override Color BottomTextColor { get => LineGraphView.BottomTextColor; set => LineGraphView.BottomTextColor = value; }
        public override double BottomTextSize { get => LineGraphView.BottomTextSize; set => LineGraphView.BottomTextSize = value; }
        public override float IconSize { get => LineGraphView.IconSize; set => LineGraphView.IconSize = value; }
        public override bool DrawIconLabels { set => LineGraphView.DrawIconLabels = value; }
        public override bool DrawDataLabels { set => LineGraphView.DrawDataLabels = value; }
        public override bool ScrollingEnabled { get => LineGraphView.IsHitTestVisible; set => LineGraphView.IsHitTestVisible = value; }
        public override void RequestGraphLayout() => LineGraphView.RequestGraphLayout();
    }
}