using SQLite;

namespace SimpleWeather.TZDB
{
    [Table("tzdb")]
    public class TZDB
    {
        [Indexed(Name = "tz_latidx", Order = 1)]
        public double latitude { get; set; }
        [Indexed(Name = "tz_longidx", Order = 2)]
        public double longitude { get; set; }
        public string tz_long { get; set; }
    }
}