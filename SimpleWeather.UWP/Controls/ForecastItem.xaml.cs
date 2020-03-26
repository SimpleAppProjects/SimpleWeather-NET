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
    public sealed partial class ForecastItem : UserControl
    {
        public ForecastItemViewModel Forecasts
        {
            get { return (this.DataContext as ForecastItemViewModel); }
        }

        public ForecastItem()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();

                if (Utils.Settings.API.Equals(WeatherData.WeatherAPI.OpenWeatherMap) ||
                    Utils.Settings.API.Equals(WeatherData.WeatherAPI.MetNo))
                {
                    // Use cloudiness
                    PoPIcon.Text = "\uf013";
                }
                else
                {
                    PoPIcon.Text = "\uf078";
                }
            };
        }
    }
}
