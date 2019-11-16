namespace SimpleWeather.HERE
{
    public partial class Alerts
    {
        public Alert[] alerts { get; set; }
    }

    public class Alert
    {
        public Timesegment[] timeSegment { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }

    public class Timesegment
    {
        public string value { get; set; }
        public string segment { get; set; }
        public Otherattributes otherAttributes { get; set; }
        public string day_of_week { get; set; }
    }

    public class Otherattributes
    {
    }
}
