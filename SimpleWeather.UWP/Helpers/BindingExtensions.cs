using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SimpleWeather.UWP.Helpers
{
    public static class BindingExtensions
    {
        public static Visibility BoolToVisibility(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static Visibility BoolToVisibility(bool value, bool inverse = false)
        {
            return (inverse ? !value : value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public static double Multiply(double value, double ratio)
        {
            return value * ratio;
        }

        public static string GetString(string resource)
        {
            return App.ResLoader.GetString(resource);
        }

        public static string GetStringForUri(Uri uri)
        {
            return App.ResLoader.GetStringForUri(uri);
        }
    }
}