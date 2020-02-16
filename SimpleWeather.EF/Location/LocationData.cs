using Newtonsoft.Json;
using NodaTime;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SimpleWeather.Location
{
    public enum LocationType
    {
        GPS = -1,
        Search
    }

    [JsonConverter(typeof(CustomJsonConverter))]
    [Table("locations")]
    public partial class LocationData
    {
        [JsonProperty]
        [Key]
        [Column(TypeName = "varchar")]
        public string query { get; set; }

        [JsonProperty]
        [Column(TypeName = "varchar")]
        public string name { get; set; }

        [JsonProperty]
        [Column(TypeName = "float")]
        public double latitude { get; set; }

        [JsonProperty]
        [Column(TypeName = "float")]
        public double longitude { get; set; }

        [JsonIgnore]
        [NotMapped]
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

        [JsonIgnore]
        [NotMapped]
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

        [JsonProperty]
        [Column(TypeName = "varchar")]
        public string tz_long { get; set; }

        [JsonIgnore]
        [NotMapped]
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

        [JsonProperty]
        [EnumDataType(typeof(LocationType))]
        [Column(TypeName = "integer")]
        public LocationType locationType { get; set; } = LocationType.Search;

        [JsonProperty]
        [Column("source", TypeName = "varchar")]
        public string weatherSource { get; set; }

        [JsonProperty]
        [Column("locsource", TypeName = "varchar")]
        public string locationSource { get; set; }

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
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(weatherSource);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(locationSource);
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
                            obj.query = reader.Value?.ToString();
                            break;

                        case "name":
                            obj.name = reader.Value?.ToString();
                            break;

                        case "latitude":
                            obj.latitude = double.Parse(reader.Value?.ToString());
                            break;

                        case "longitude":
                            obj.longitude = double.Parse(reader.Value?.ToString());
                            break;

                        case "tz_long":
                            obj.tz_long = reader.Value?.ToString();
                            break;

                        case "locationType":
                            obj.locationType = (LocationType)int.Parse(reader.Value?.ToString());
                            break;

                        case "source":
                            obj.weatherSource = reader.Value?.ToString();
                            break;

                        case "locsource":
                            obj.locationSource = reader.Value?.ToString();
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
            using (var sw = new System.IO.StringWriter())
            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
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
                writer.WriteValue(weatherSource);

                // "locsource" : ""
                writer.WritePropertyName("locsource");
                writer.WriteValue(locationSource);

                // }
                writer.WriteEndObject();

                return sw.ToString();
            }
        }

        public bool IsValid()
        {
            if (String.IsNullOrWhiteSpace(query) || String.IsNullOrWhiteSpace(weatherSource) || String.IsNullOrWhiteSpace(locationSource))
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
        [Key]
        [Column(TypeName = "varchar")]
        public string query { get; set; }

        [Column(TypeName = "integer")]
        public int position { get; set; }

        public Favorites()
        {
        }
    }
}