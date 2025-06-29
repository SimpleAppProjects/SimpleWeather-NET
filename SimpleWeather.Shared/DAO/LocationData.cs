using SimpleWeather.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleWeather.LocationData
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
        [PrimaryKey]
        public string query { get; set; }

        public string name { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        [JsonIgnore]
        [Ignore]
        public TimeSpan tz_offset
        {
            get
            {
                return DateTimeUtils.TzidToOffset(tz_long);
            }
        }

        [JsonIgnore]
        [Ignore]
        public string tz_short
        {
            get
            {
                return DateTimeUtils.TzidToTzShortAbbreviation(tz_long);
            }
        }

        public string tz_long { get; set; }

        [JsonIgnore]
        [Ignore]
        public string country_code
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(tz_long))
                {
                    var tzdbLocation = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ZoneLocations
                        .FirstOrDefault(tzdbloc => Equals(tzdbloc.ZoneId, tz_long));

                    if (tzdbLocation == null)
                    {
                        var alias = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.CanonicalIdMap[tz_long];

                        tzdbLocation = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ZoneLocations
                            .FirstOrDefault(tzdbloc => Equals(tzdbloc.ZoneId, alias));
                    }

                    if (tzdbLocation != null)
                    {
                        return tzdbLocation.CountryCode;
                    }
                }
                return String.Empty;
            }
        }

        public LocationType locationType { get; set; } = LocationType.Search;

        [Column("source")]
        [JsonPropertyName("source")]
        public string weatherSource { get; set; }

        [Column("locsource")]
        [JsonPropertyName("locsource")]
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

        public override void FromJson(ref Utf8JsonReader reader)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                    reader.Read(); // StartObject

                string property = reader.GetString(); // JsonTokenType.PropertyName

                reader.Read(); // Property Value

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
                    case "weatherSource":
                        this.weatherSource = reader.GetString();
                        break;

                    case "locsource":
                    case "locationSource":
                        this.locationSource = reader.GetString();
                        break;

                    default:
                        // ignore
                        break;
                }
            }
        }

        public override string ToJson()
        {
            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            // {
            writer.WriteStartObject();

            // "query" : ""
            writer.WriteString("query", query);

            // "name" : ""
            writer.WriteString("name", name);

            // "latitude" : ""
            writer.WriteNumber("latitude", latitude);

            // "longitude" : ""
            writer.WriteNumber("longitude", longitude);

            // "tz_long" : ""
            writer.WriteString("tz_long", tz_long);

            // "locationType" : ""
            writer.WriteNumber("locationType", (int)locationType);

            // "source" : ""
            writer.WriteString("source", weatherSource);

            // "locsource" : ""
            writer.WriteString("locsource", locationSource);

            // }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public bool IsValid()
        {
            return !String.IsNullOrWhiteSpace(query) &&
                !String.IsNullOrWhiteSpace(weatherSource) &&
                !String.IsNullOrWhiteSpace(locationSource) &&
                !(latitude == 0d && longitude == 0d);
        }

        public override string ToString()
        {
            return String.Format("{0}|{1}|{2}", this.query, this.name, this.locationType);
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