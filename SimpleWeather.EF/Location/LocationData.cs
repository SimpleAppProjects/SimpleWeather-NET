using NodaTime;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Utf8Json;

namespace SimpleWeather.Location
{
    public enum LocationType
    {
        GPS = -1,
        Search
    }

    [JsonFormatter(typeof(CustomJsonConverter<LocationData>))]
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

        [IgnoreDataMember]
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

        [IgnoreDataMember]
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

        [IgnoreDataMember]
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

        public override void FromJson(ref JsonReader reader)
        {
            var count = 0; // managing array-count state in outer(this is count, not index(index is always count - 1)
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                reader.ReadIsBeginObject(); // StartObject

                string property = reader.ReadPropertyName();
                //reader.ReadNext(); // prop value

                switch (property)
                {
                    case "query":
                        this.query = reader.ReadString();
                        break;

                    case "name":
                        this.name = reader.ReadString();
                        break;

                    case "latitude":
                        this.latitude = reader.ReadDouble();
                        break;

                    case "longitude":
                        this.longitude = reader.ReadDouble();
                        break;

                    case "tz_long":
                        this.tz_long = reader.ReadString();
                        break;

                    case "locationType":
                        this.locationType = (LocationType)reader.ReadInt32();
                        break;

                    case "source":
                        this.weatherSource = reader.ReadString();
                        break;

                    case "locsource":
                        this.locationSource = reader.ReadString();
                        break;
                }
            }
        }

        public override string ToJson()
        {
            var writer = new JsonWriter();

            // {
            writer.WriteBeginObject();

            // "query" : ""
            writer.WritePropertyName("query");
            writer.WriteString(query);

            writer.WriteValueSeparator();

            // "name" : ""
            writer.WritePropertyName("name");
            writer.WriteString(name);

            writer.WriteValueSeparator();

            // "latitude" : ""
            writer.WritePropertyName("latitude");
            writer.WriteDouble(latitude);

            writer.WriteValueSeparator();

            // "longitude" : ""
            writer.WritePropertyName("longitude");
            writer.WriteDouble(longitude);

            writer.WriteValueSeparator();

            // "tz_long" : ""
            writer.WritePropertyName("tz_long");
            writer.WriteString(tz_long);

            writer.WriteValueSeparator();

            // "locationType" : ""
            writer.WritePropertyName("locationType");
            writer.WriteInt32((int)locationType);

            writer.WriteValueSeparator();

            // "source" : ""
            writer.WritePropertyName("source");
            writer.WriteString(weatherSource);

            writer.WriteValueSeparator();

            // "locsource" : ""
            writer.WritePropertyName("locsource");
            writer.WriteString(locationSource);

            // }
            writer.WriteEndObject();

            return writer.ToString();
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