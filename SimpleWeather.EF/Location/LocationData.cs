using NodaTime;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleWeather.Location
{
    public enum LocationType
    {
        GPS = -1,
        Search
    }

    [JsonConverter(typeof(CustomJsonConverter<LocationData>))]
    [Table("locations")]
    public partial class LocationData : CustomJsonObject
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string query { get; set; }

        [Column(TypeName = "varchar")]
        public string name { get; set; }

        [Column(TypeName = "float")]
        public double latitude { get; set; }

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

        [EnumDataType(typeof(LocationType))]
        [Column(TypeName = "integer")]
        public LocationType locationType { get; set; } = LocationType.Search;

        [Column("source", TypeName = "varchar")]
        public string weatherSource { get; set; }

        [Column("locsource", TypeName = "varchar")]
        public string locationSource { get; set; }

        public override bool Equals(System.Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(this.GetType()))
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

        public override void FromJson(Utf8JsonReader reader)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                    reader.Read(); // StartObject

                string property = reader.GetString();
                reader.Read(); // prop value

                switch (property)
                {
                    case "query":
                        this.query = reader.GetString();
                        break;

                    case "name":
                        this.name = reader.GetString();
                        break;

                    case "latitude":
                        this.latitude = reader.GetDouble();
                        break;

                    case "longitude":
                        this.longitude = reader.GetDouble();
                        break;

                    case "tz_long":
                        this.tz_long = reader.GetString();
                        break;

                    case "locationType":
                        this.locationType = (LocationType)reader.GetInt32();
                        break;

                    case "source":
                        this.weatherSource = reader.GetString();
                        break;

                    case "locsource":
                        this.locationSource = reader.GetString();
                        break;
                }
            }
        }

        public override string ToJson()
        {
            using (var stream = new System.IO.MemoryStream())
            using (var writer = new Utf8JsonWriter(stream))
            {
                // {
                writer.WriteStartObject();

                // "query" : ""
                writer.WritePropertyName("query");
                writer.WriteStringValue(query);

                // "name" : ""
                writer.WritePropertyName("name");
                writer.WriteStringValue(name);

                // "latitude" : ""
                writer.WritePropertyName("latitude");
                writer.WriteNumberValue(latitude);

                // "longitude" : ""
                writer.WritePropertyName("longitude");
                writer.WriteNumberValue(longitude);

                // "tz_long" : ""
                writer.WritePropertyName("tz_long");
                writer.WriteStringValue(tz_long);

                // "locationType" : ""
                writer.WritePropertyName("locationType");
                writer.WriteNumberValue((int)locationType);

                // "source" : ""
                writer.WritePropertyName("source");
                writer.WriteStringValue(weatherSource);

                // "locsource" : ""
                writer.WritePropertyName("locsource");
                writer.WriteStringValue(locationSource);

                // }
                writer.WriteEndObject();

                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
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