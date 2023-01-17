using SimpleWeather.Common.Controls;
using System;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
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
