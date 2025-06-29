using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public static class CustomException
    {
        public static Exception CreateUnsupportedLocationException(string weatherApi, LocationData.LocationData location)
        {
            return new Exception($"Unsupported location - weatherapi: {weatherApi}, countryCode: {location.country_code}, query: [{location.query}]");
        }
    }
}