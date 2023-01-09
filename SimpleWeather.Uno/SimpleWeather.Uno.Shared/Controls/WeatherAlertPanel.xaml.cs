using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Uno.Controls
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
        }
    }

    internal class AlertSeverityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is WeatherData.WeatherAlertSeverity severity)
            {
                return WeatherUtils.GetColorFromAlertSeverity(severity);
            }
            else
            {
                return WeatherUtils.GetColorFromAlertSeverity(WeatherData.WeatherAlertSeverity.Minor);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal class AlertTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is WeatherData.WeatherAlertType type)
            {
                return new Uri(WeatherUtils.GetAssetFromAlertType(type));
            }
            else
            {
                return new Uri(WeatherUtils.GetAssetFromAlertType(WeatherData.WeatherAlertType.SpecialWeatherAlert));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
