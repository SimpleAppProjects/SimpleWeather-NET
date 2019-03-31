using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using NodaTime;

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
        [Newtonsoft.Json.JsonIgnore]
        [Ignore]
        public TimeSpan tz_offset
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(tz_long))
                {
                    var nodaTz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tz_long);
                    if (nodaTz != null)
                        return nodaTz.GetUtcOffset(SystemClock.Instance.GetCurrentInstant()).ToTimeSpan();
                }
                return TimeSpan.Zero;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        [Ignore]
        public string tz_short
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(tz_long))
                {
                    var nodaTz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tz_long);
                    if (nodaTz != null)
                        return nodaTz.GetZoneInterval(SystemClock.Instance.GetCurrentInstant()).Name;
                }
                return "UTC";
            }
        }
        [Newtonsoft.Json.JsonProperty]
        public string tz_long { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [Ignore]
        public string country_code
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(tz_long))
                {
                    var tzdbLocation = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ZoneLocations
                        .First(tzdbloc => tzdbloc.ZoneId.Equals(tz_long));
                    return tzdbLocation.CountryCode;
                }
                return String.Empty;
            }
        }
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
            latitude = query_vm.LocationLat;
            longitude = query_vm.LocationLong;
            tz_long = query_vm.LocationTZ_Long;
            source = Settings.API;
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
            tz_long = query_vm.LocationTZ_Long;
            locationType = LocationType.GPS;
            source = Settings.API;
        }

        public override bool Equals(System.Object obj)
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
            var hashCode = -19042156;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(query);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + latitude.GetHashCode();
            hashCode = hashCode * -1521134295 + longitude.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(tz_long);
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
                        case "tz_long":
                            obj.tz_long = reader.Value.ToString();
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

            // "tz_long" : ""
            writer.WritePropertyName("tz_long");
            writer.WriteValue(tz_long);

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

        public bool IsValid()
        {
            if (String.IsNullOrWhiteSpace(query) || String.IsNullOrWhiteSpace(source))
                return false;
            else
                return true;
        }

        public override string ToString()
        {
            return String.Format("{0}|{1}|{2}", this.query, this.name, this.locationType.ToString());
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
