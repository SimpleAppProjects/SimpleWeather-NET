﻿using SimpleWeather.Location;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SimpleWeather.UWP.Helpers
{
    public class LocationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = (LocationType)value;

            switch (type)
            {
                case LocationType.GPS:
                    return App.ResLoader.GetString("Header_GPSLocation");
                case LocationType.Search:
                    return App.ResLoader.GetString("Header_FavLocations");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}