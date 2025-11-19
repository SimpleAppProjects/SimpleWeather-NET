using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SimpleWeather.NET.Controls.Graphs;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.NET.Controls
{
    public sealed partial class ForecastBar : UserControl
    {
        public Grid InnerBarView => InnerBar;
        public FrameworkElement BarView => Bar;
        public IconControl BarIconView => BarIcon;
        public TextBlock BarDateBlock => BarDate;
        public TextBlock BarValueBlock => BarValue;

        public BarGraphEntry Data
        {
            get => DataContext as BarGraphEntry;
        }

        public ForecastBar()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
            this.Bar.SizeChanged += ForecastBar_Bar_SizeChanged;
        }

        private void ForecastBar_Bar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var largestSize = Math.Min(Bar.ActualHeight, Bar.ActualWidth);
            Bar.RadiusX = Bar.RadiusY = largestSize / 2;
        }

        public void SetColors(Color color)
        {
            Bar.Fill = new SolidColorBrush(color);
        }
    }
}
