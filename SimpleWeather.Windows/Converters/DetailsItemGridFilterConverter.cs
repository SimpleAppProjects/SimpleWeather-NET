using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Data;
using FeatureSettings = SimpleWeather.NET.Utils.FeatureSettings;

namespace SimpleWeather.NET.Converters
{
    public partial class DetailsItemGridFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is IEnumerable<DetailItemViewModel> detailItems
                ? detailItems.WhereNot(it => ((it.DetailsType == WeatherDetailsType.Sunrise || it.DetailsType == WeatherDetailsType.Sunset) && FeatureSettings.SunPhase) ||
                        ((it.DetailsType == WeatherDetailsType.Moonrise || it.DetailsType == WeatherDetailsType.Moonset || it.DetailsType == WeatherDetailsType.MoonPhase) && FeatureSettings.MoonPhase))
                : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
