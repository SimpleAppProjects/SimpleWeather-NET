using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.NET.Helpers;
using Windows.UI;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls.Graphs
{
    public sealed partial class RangeBarGraphPanel : GraphPanel
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

        public RangeBarGraphData ForecastData
        {
            get => (RangeBarGraphData)GetValue(ForecastDataProperty);
            set => SetValue(ForecastDataProperty, value);
        }

        public static readonly DependencyProperty ForecastDataProperty =
            DependencyProperty.Register(nameof(ForecastData), typeof(RangeBarGraphData),
            typeof(RangeBarGraphPanel), new PropertyMetadata(null, (o, e) => (o as RangeBarGraphPanel)?.UpdateView(false)));

        protected override GraphScrollView GraphScrollView => BarChartView;
        protected override UIElement LeftScrollButton => this.LeftButton;
        protected override UIElement RightScrollButton => this.RightButton;

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
                BarChartView?.ScrollTo(0, 0);
            }
        }

        private void UpdateForecastGraph()
        {
            if (ForecastData != null)
            {
                BarChartView.SetData(ForecastData);
            }
        }

        public override int GetItemPositionFromPoint(float xCoordinate) => BarChartView.GetItemPositionFromPoint(xCoordinate);
        public override double GraphMaxWidth { get => BarChartView.GraphMaxWidth; set => BarChartView.GraphMaxWidth = value; }
        public override bool FillParentWidth { set => BarChartView.FillParentWidth = value; }
        public override Color BottomTextColor { get => BarChartView.BottomTextColor; set => BarChartView.BottomTextColor = value; }
        public override double BottomTextSize { get => BarChartView.BottomTextSize; set => BarChartView.BottomTextSize = value; }
        public override float IconSize { get => BarChartView.IconSize; set => BarChartView.IconSize = value; }
        public override bool DrawIconLabels { set => BarChartView.DrawIconLabels = value; }
        public override bool DrawDataLabels { set => BarChartView.DrawDataLabels = value; }
        public override bool ScrollingEnabled { get => BarChartView.IsHitTestVisible; set => BarChartView.IsHitTestVisible = value; }
        public override void RequestGraphLayout() => BarChartView.RequestGraphLayout();
    }
}