using System;
using System.Globalization;
using System.Runtime.Serialization;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl
    {
        // Utils Methods
        public abstract String GetBackgroundURI(Weather weather);
        public abstract Color GetWeatherBackgroundColor(Weather weather);
        public abstract String GetWeatherIconURI(string icon);
        public void SetBackground(ImageBrush bg, Weather weather)
        {
            String icon = weather.condition.icon;
            Uri imgURI = new Uri(GetBackgroundURI(weather));

            if (bg != null && bg.ImageSource != null)
            {
                // Skip re-settting bg
                if (bg.ImageSource is BitmapImage bmp && bmp.UriSource == imgURI)
                    return;
            }

            BitmapImage img = new BitmapImage(imgURI)
            {
                CreateOptions = BitmapCreateOptions.None,
                DecodePixelWidth = 960
            };
            bg.ImageSource = img;
        }
    }
}