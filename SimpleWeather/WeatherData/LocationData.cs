using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SimpleWeather.WeatherData
{
    public enum LocationType
    {
        GPS = -1,
        Search
    }

    [Newtonsoft.Json.JsonConverter(typeof(CustomJsonConverter))]
    [Table("locations")]
    public class LocationData
#if __ANDROID__
        : Java.Lang.Object
#endif
    {
        [Newtonsoft.Json.JsonProperty]
        [PrimaryKey]
        public string query { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public string name { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public double latitude { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public double longitude { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public TimeSpan tz_offset { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public string tz_short { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public LocationType locationType { get; set; } = LocationType.Search;
        [Newtonsoft.Json.JsonProperty]
        public string source { get; set; }

        public LocationData()
        {
            source = Settings.API;
        }

        public LocationData(Controls.LocationQueryViewModel query_vm)
        {
            query = query_vm.LocationQuery;
            name = query_vm.LocationName;
            tz_offset = query_vm.LocationTZ_Offset;
            tz_short = query_vm.LocationTZ_Short;
            source = Settings.API;
        }

#if WINDOWS_UWP
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
            tz_offset = query_vm.LocationTZ_Offset;
            tz_short = query_vm.LocationTZ_Short;
            locationType = LocationType.GPS;
            source = Settings.API;
        }
#elif __ANDROID__
        public LocationData(Controls.LocationQueryViewModel query_vm, Android.Locations.Location location)
        {
            SetData(query_vm, location);
        }

        public void SetData(Controls.LocationQueryViewModel query_vm, Android.Locations.Location location)
        {
            query = query_vm.LocationQuery;
            name = query_vm.LocationName;
            latitude = location.Latitude;
            longitude = location.Longitude;
            tz_offset = query_vm.LocationTZ_Offset;
            tz_short = query_vm.LocationTZ_Short;
            locationType = LocationType.GPS;
            source = Settings.API;
        }
#endif

#if WINDOWS_UWP
        public override bool Equals(System.Object obj)
#elif __ANDROID__
        public override bool Equals(Java.Lang.Object obj)
#endif
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                LocationData locData = (LocationData)obj;
                return this.GetHashCode() == locData.GetHashCode();
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -671766369;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(query);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + latitude.GetHashCode();
            hashCode = hashCode * -1521134295 + longitude.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(tz_offset);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(tz_short);
            hashCode = hashCode * -1521134295 + locationType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(source);
            return hashCode;
        }

        public static LocationData FromJson(Newtonsoft.Json.JsonReader reader)
        {
            LocationData obj = null;

            try
            {
                obj = new LocationData();

                while (reader.Read() && reader.TokenType != Newtonsoft.Json.JsonToken.EndObject)
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.StartObject)
                        reader.Read(); // StartObject

                    string property = reader.Value.ToString();
                    reader.Read(); // prop value

                    switch (property)
                    {
                        case "query":
                            obj.query = reader.Value.ToString();
                            break;
                        case "name":
                            obj.name = reader.Value.ToString();
                            break;
                        case "latitude":
                            obj.latitude = double.Parse(reader.Value.ToString());
                            break;
                        case "longitude":
                            obj.longitude = double.Parse(reader.Value.ToString());
                            break;
                        case "tz_offset":
                            obj.tz_offset = TimeSpan.Parse(reader.Value.ToString());
                            break;
                        case "tz_short":
                            obj.tz_short = reader.Value.ToString();
                            break;
                        case "locationType":
                            obj.locationType = (LocationType)int.Parse(reader.Value.ToString());
                            break;
                        case "source":
                            obj.source = reader.Value.ToString();
                            break;
                    }
                }
            }
            catch (Exception)
            {
                obj = null;
            }

            return obj;
        }

        public string ToJson()
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw);

            // {
            writer.WriteStartObject();

            // "query" : ""
            writer.WritePropertyName("query");
            writer.WriteValue(query);

            // "name" : ""
            writer.WritePropertyName("name");
            writer.WriteValue(name);

            // "latitude" : ""
            writer.WritePropertyName("latitude");
            writer.WriteValue(latitude);

            // "longitude" : ""
            writer.WritePropertyName("longitude");
            writer.WriteValue(longitude);

            // "tz_offset" : ""
            writer.WritePropertyName("tz_offset");
            writer.WriteValue(tz_offset);

            // "tz_short" : ""
            writer.WritePropertyName("tz_short");
            writer.WriteValue(tz_short);

            // "locationType" : ""
            writer.WritePropertyName("locationType");
            writer.WriteValue((int)locationType);

            // "source" : ""
            writer.WritePropertyName("source");
            writer.WriteValue(source);

            // }
            writer.WriteEndObject();

            return sw.ToString();
        }
    }

    [Table("favorites")]
    public class Favorites
    {
        [PrimaryKey]
        public string query { get; set; }
        public int position { get; set; }

        public Favorites()
        {
        }
    }
}
