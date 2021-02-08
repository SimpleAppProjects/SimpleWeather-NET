using NodaTime;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SimpleWeather.Location
{
    public partial class LocationData
    {
        public LocationData()
        {
            weatherSource = Settings.API;
        }

        public LocationData(Controls.LocationQueryViewModel query_vm)
        {
            query = query_vm.LocationQuery;
            name = query_vm.LocationName;
            latitude = query_vm.LocationLat;
            longitude = query_vm.LocationLong;
            tz_long = query_vm.LocationTZLong;
            weatherSource = query_vm.WeatherSource;
            locationSource = query_vm.LocationSource;
        }

        public LocationData(Controls.LocationQueryViewModel query_vm, Windows.Devices.Geolocation.Geoposition geoPos)
        {
            SetData(query_vm, geoPos);
        }

        public void SetData(Controls.LocationQueryViewModel query_vm, Windows.Devices.Geolocation.Geoposition geoPos)
        {
            query = query_vm.LocationQuery;
            name = query_vm.LocationName;
            latitude = geoPos.Coordinate.Point.Position.Latitude;
            longitude = geoPos.Coordinate.Point.Position.Longitude;
            tz_long = query_vm.LocationTZLong;
            locationType = LocationType.GPS;
            weatherSource = query_vm.WeatherSource;
            locationSource = query_vm.LocationSource;
        }
    }
}