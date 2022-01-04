using SimpleWeather.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class AQIForecastControl : UserControl
    {
        public AirQualityViewModel ViewModel
        {
            get { return (this.DataContext as AirQualityViewModel); }
        }

        public AQIForecastControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
            };

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                DataContext = new AirQualityViewModel(new WeatherData.AirQuality()
                {
                    attribution = "World Air Quality Index Project",
                    co = 9,
                    date = DateTime.Today,
                    index = 101,
                    no2 = 32,
                    o3 = 9,
                    pm10 = 68,
                    pm25 = 154,
                    so2 = 5
                });
            }
        }
    }
}
