﻿using SimpleWeather.LocationData;
using System;
using Windows.UI.Xaml.Data;

namespace SimpleWeather.UWP.Converters
{
    public class LocationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = (LocationType)value;

            switch (type)
            {
                case LocationType.GPS:
                    return App.Current.ResLoader.GetString("label_currentlocation");
                case LocationType.Search:
                    return App.Current.ResLoader.GetString("label_favoritelocations");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}