using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
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
                MessageSpan.Inlines.Clear();

                if (WeatherAlert?.Message?.StartsWith("https://weatherkit.apple.com") == true)
                {
                    try
                    {
                        MessageSpan.Inlines.Add(new Hyperlink()
                        {
                            NavigateUri = new Uri(WeatherAlert.Message),
                            Inlines =
                            {
                                new Run()
                                {
                                    Text = ResStrings.label_moreinfo
                                }
                            }
                        });
                    }
                    catch
                    {
                        MessageSpan.Inlines.Add(new Run()
                        {
                            Text = WeatherAlert.Message
                        });
                    }
                }
                else
                {
                    MessageSpan.Inlines.Add(new Run()
                    {
                        Text = WeatherAlert?.Message
                    });
                }

                MessageSpan.Inlines.Add(new LineBreak());
            };
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
