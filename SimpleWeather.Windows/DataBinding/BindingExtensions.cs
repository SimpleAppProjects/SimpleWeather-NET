﻿using System;
using Microsoft.UI.Xaml;

namespace SimpleWeather.NET.DataBinding
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
            return App.Current.ResLoader.GetString(resource);
        }

        public static string GetStringForUri(Uri uri)
        {
            return App.Current.ResLoader.GetStringForUri(uri);
        }

        public static bool IsNotNull(object obj)
        {
            return obj != null;
        }

        public static bool AreBothTrue(bool value1, bool value2)
        {
            return value1 && value2;
        }

        public static bool AreBothFalse(bool value1, bool value2)
        {
            return !value1 && !value2;
        }

        public static Visibility IsVisibleIfBothTrue(bool value1, bool value2)
        {
            return BoolToVisibility(AreBothTrue(value1, value2));
        }

        public static Visibility IsVisibleIfBothFalse(bool value1, bool value2)
        {
            return BoolToVisibility(AreBothFalse(value1, value2));
        }
    }
}