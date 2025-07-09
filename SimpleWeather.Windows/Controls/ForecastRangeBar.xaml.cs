using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.NET.Controls
{
    public sealed partial class ForecastRangeBar : UserControl
    {
        private readonly Color[] DEFAULT_COLORS = { Colors.OrangeRed, Colors.LightSkyBlue };

        public Grid InnerBarView => InnerBar;
        public FrameworkElement RangeBarView => RangeBar;

        public ForecastRangeBarEntry Data
        {
            get => DataContext as ForecastRangeBarEntry;
        }

        public ForecastRangeBar()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        public void SetColors() => SetColors(DEFAULT_COLORS);

        public void SetColors(params Color[] colors)
        {
            RangeBar.Fill = new LinearGradientBrush()
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
                GradientStops = new GradientStopCollection().Apply(c =>
                {
                    colors.ForEachIndexed((index, color) =>
                    {
                        c.Add(new GradientStop() 
                        {
                            Color = color,
                            Offset = index / MathF.Max(1, colors.Length - 1)
                        });
                    });
                })
            };
        }
    }
}
