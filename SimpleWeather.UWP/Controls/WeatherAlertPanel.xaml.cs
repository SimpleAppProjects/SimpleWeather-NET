using SimpleWeather.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class WeatherAlertPanel : UserControl
    {
        public WeatherAlertViewModel WeatherAlert
        {
            get { return (this.DataContext as WeatherAlertViewModel); }
        }

        public WeatherAlertPanel()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();

                AlertHeader.Background = new SolidColorBrush(GetColorFromAlertType(WeatherAlert.AlertType));
                AlertIcon.Source = GetAssetFromAlertType(WeatherAlert.AlertType);
            };
        }

        private string GetAssetFromAlertType(WeatherData.WeatherAlertType type)
        {
            string baseuri = "ms-appx:///Assets/WeatherIcons/png/";
            string fileIcon = string.Empty;

            switch (type)
            {
                case WeatherData.WeatherAlertType.DenseFog:
                    fileIcon = "fog.png";
                    break;
                case WeatherData.WeatherAlertType.Fire:
                    fileIcon = "fire.png";
                    break;
                case WeatherData.WeatherAlertType.FloodWarning:
                case WeatherData.WeatherAlertType.FloodWatch:
                    fileIcon = "flood.png";
                    break;
                case WeatherData.WeatherAlertType.Heat:
                    fileIcon = "hot.png";
                    break;
                case WeatherData.WeatherAlertType.HighWind:
                    fileIcon = "strong_wind.png";
                    break;
                case WeatherData.WeatherAlertType.HurricaneLocalStatement:
                case WeatherData.WeatherAlertType.HurricaneWindWarning:
                    fileIcon = "hurricane.png";
                    break;
                case WeatherData.WeatherAlertType.SevereThunderstormWarning:
                case WeatherData.WeatherAlertType.SevereThunderstormWatch:
                    fileIcon = "thunderstorm.png";
                    break;
                case WeatherData.WeatherAlertType.TornadoWarning:
                case WeatherData.WeatherAlertType.TornadoWatch:
                    fileIcon = "tornado.png";
                    break;
                case WeatherData.WeatherAlertType.Volcano:
                    fileIcon = "volcano.png";
                    break;
                case WeatherData.WeatherAlertType.WinterWeather:
                    fileIcon = "snowflake_cold.png";
                    break;
                case WeatherData.WeatherAlertType.SevereWeather:
                case WeatherData.WeatherAlertType.SpecialWeatherAlert:
                default:
                    fileIcon = "ic_error_white.png";
                    break;
            }

            return baseuri + fileIcon;
        }

        private Color GetColorFromAlertType(WeatherData.WeatherAlertType type)
        {
            Color color = Colors.OrangeRed;

            switch (type)
            {
                case WeatherData.WeatherAlertType.DenseFog:
                case WeatherData.WeatherAlertType.FloodWatch:
                case WeatherData.WeatherAlertType.SevereWeather:
                case WeatherData.WeatherAlertType.SpecialWeatherAlert:
                case WeatherData.WeatherAlertType.WinterWeather:
                // Unsure
                case WeatherData.WeatherAlertType.Volcano:
                    color = Colors.OrangeRed;
                    break;
                case WeatherData.WeatherAlertType.HighWind:
                case WeatherData.WeatherAlertType.Fire:
                // Unsure
                case WeatherData.WeatherAlertType.Heat:
                case WeatherData.WeatherAlertType.HurricaneLocalStatement:
                case WeatherData.WeatherAlertType.SevereThunderstormWatch:
                case WeatherData.WeatherAlertType.TornadoWatch:
                    color = Colors.Orange;
                    break;
                case WeatherData.WeatherAlertType.FloodWarning:
                // Unsure
                case WeatherData.WeatherAlertType.HurricaneWindWarning:
                case WeatherData.WeatherAlertType.SevereThunderstormWarning:
                case WeatherData.WeatherAlertType.TornadoWarning:
                    color = Colors.Red;
                    break;
            }

            return color;
        }
    }
}
