using System.Collections.Generic;

namespace SimpleWeather.Weather_API.Google
{
    public class MinutelyResponse
    {
        public Overallpredictiontimeframe overallPredictionTimeframe { get; set; }
        public TimeZone timeZone { get; set; }
        public List<Segment> segments { get; set; }
        public string nextPageToken { get; set; }
    }

    public class Overallpredictiontimeframe
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
    }

    public class Segment
    {
        public Timeframe timeFrame { get; set; }
        public string type { get; set; }
        public int? probability { get; set; }
        public Qpf qpf { get; set; }
        public Snowfallamount snowfallAmount { get; set; }
        public string intensity { get; set; }
    }

    public class Timeframe
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
    }

    public class Snowfallamount
    {
        public float? quantity { get; set; }
        public string unit { get; set; }
    }
}
