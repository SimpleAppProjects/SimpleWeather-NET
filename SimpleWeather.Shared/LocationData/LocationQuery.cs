﻿using SimpleWeather.WeatherData;
using System;
using System.ComponentModel;
using System.Globalization;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.LocationData
{
    [Bindable(BindableSupport.Default)]
    public partial class LocationQuery
    {
        public string LocationName { get; set; }
        public string LocationRegion { get; set; }
        public string LocationCountry { get; set; }
        public string Location_Query { get; set; }

        public double LocationLat { get; set; }
        public double LocationLong { get; set; }

        public string LocationTZLong { get; set; }

        public string LocationSource { get; set; }
        public string WeatherSource { get; set; }

        public string LocationRegionText
        {
            get
            {
                if (String.IsNullOrWhiteSpace(LocationRegion))
                {
                    return LocationCountry;
                }
                else
                {
                    return $"{LocationRegion}, {LocationCountry}";
                }
            }
        }

        public LocationQuery()
        {
            LocationName = ResStrings.error_noresults;
            LocationCountry = string.Empty;
            Location_Query = string.Empty;
        }

        public LocationQuery(LocationData data)
        {
            Location_Query = data.query;
            LocationName = data.name;
            LocationLat = data.latitude;
            LocationLong = data.longitude;
            LocationTZLong = data.tz_long;
            WeatherSource = data.weatherSource;
            LocationSource = data.locationSource;
            LocationCountry = data.country_code;
        }

        public bool IsEmpty => String.IsNullOrEmpty(LocationCountry) && String.IsNullOrEmpty(Location_Query);

        public LocationQuery Clone()
        {
            return this.MemberwiseClone() as LocationQuery;
        }

        internal static LocationQuery BuildEmptyModel(String weatherSource)
        {
            var vm = new LocationQuery
            {
                LocationName = "" // Reset name
            };
            vm.UpdateWeatherSource(weatherSource);
            return vm;
        }

        private static readonly Lazy<LocationQuery> _empty = new Lazy<LocationQuery>(() => new LocationQuery());
        public static LocationQuery Empty => _empty.Value;

        private void UpdateLocationQuery()
        {
            Location_Query = WeatherSource switch
            {
                WeatherAPI.Here or
                WeatherAPI.WeatherUnlocked or
                WeatherAPI.WeatherApi or
                WeatherAPI.TomorrowIo or
                WeatherAPI.AccuWeather or
                WeatherAPI.ECCC => string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", LocationLat, LocationLong),
                _ => string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", LocationLat, LocationLong)
            };
        }

        public void UpdateWeatherSource(string API)
        {
            WeatherSource = API;
            UpdateLocationQuery();
        }

        public override bool Equals(object obj)
        {
            return obj is LocationQuery query &&
                   LocationName == query.LocationName &&
                   LocationRegion == query.LocationRegion &&
                   LocationCountry == query.LocationCountry &&
                   Location_Query == query.Location_Query &&
                   LocationLat == query.LocationLat &&
                   LocationLong == query.LocationLong &&
                   LocationTZLong == query.LocationTZLong &&
                   LocationSource == query.LocationSource &&
                   WeatherSource == query.WeatherSource;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(LocationName);
            hash.Add(LocationRegion);
            hash.Add(LocationCountry);
            hash.Add(Location_Query);
            hash.Add(LocationLat);
            hash.Add(LocationLong);
            hash.Add(LocationTZLong);
            hash.Add(LocationSource);
            hash.Add(WeatherSource);
            return hash.ToHashCode();
        }
    }
}