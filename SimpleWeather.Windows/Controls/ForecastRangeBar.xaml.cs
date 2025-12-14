using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;
using Windows.Foundation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.NET.Controls
{
    public sealed partial class ForecastRangeBar : UserControl
    {
        public Grid InnerBarView => InnerBar;
        public FrameworkElement RangeBarView => RangeBar;
        public IconControl BarIconView => BarIcon;
        public TextBlock BarDateBlock => BarDate;
        public FrameworkElement BarPoPView => BarPoP;

        public double BarHiLoHeight => BarHi.DesiredSize.Height + BarLo.DesiredSize.Height;

        public ForecastRangeBarEntry Data
        {
            get => DataContext as ForecastRangeBarEntry;
        }

        private ForecastType? _forecastType = null;
        public ForecastType? ForecastType
        {
            get => _forecastType;
            set
            {
                _forecastType = value;
                BarPoP.Visibility = _forecastType == Controls.ForecastType.Temperature ? Visibility.Visible : Visibility.Collapsed;
                BarDate.FontSize = BarHi.FontSize = BarLo.FontSize = _forecastType switch
                {
                    Controls.ForecastType.Temperature => 16,
                    _ => 14
                };
                BarIcon.IconHeight = BarIcon.IconWidth = _forecastType switch
                {
                    Controls.ForecastType.Temperature => 50,
                    _ => 30
                };
            }
        }

        public ForecastRangeBar()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
            this.RangeBar.SizeChanged += RangeBar_SizeChanged;
        }

        private void RangeBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var largestSize = Math.Min(RangeBar.ActualHeight, RangeBar.ActualWidth);
            RangeBar.RadiusX = RangeBar.RadiusY = largestSize / 2;
        }

        public void SetColors() => SetColors((Colors.OrangeRed, Colors.LightSkyBlue));

        public void SetColors((Color hiColor, Color loColor) colors)
        {
            RangeBar.Fill = new LinearGradientBrush()
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
                GradientStops = new GradientStopCollection().Apply(c =>
                {
                    c.Add(new GradientStop()
                    {
                        Color = colors.hiColor,
                        Offset = 0
                    });
                    c.Add(new GradientStop()
                    {
                        Color = colors.loColor,
                        Offset = 1
                    });
                })
            };
        }
    }
}
