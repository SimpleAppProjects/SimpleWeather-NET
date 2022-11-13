using System;

namespace SimpleWeather.Weather_API.TomorrowIO
{
    public class Rootobject
    {
        public Data data { get; set; }
        //public Warning[] warnings { get; set; }
    }

    public class Data
    {
        public Timeline[] timelines { get; set; }
    }

    public class Timeline
    {
        public string timestep { get; set; }
        public DateTimeOffset startTime { get; set; }
        public DateTimeOffset endTime { get; set; }
        public Interval[] intervals { get; set; }
    }

    public class Interval
    {
        public DateTimeOffset startTime { get; set; }
        public Values values { get; set; }
    }

    public class Values
    {
        public float? snowAccumulation { get; set; }
        public float? temperature { get; set; }
        public float? temperatureApparent { get; set; }
        public float? temperatureMin { get; set; }
        public float? temperatureMax { get; set; }
        public float? dewPoint { get; set; }
        public float? humidity { get; set; }
        public float? windSpeed { get; set; }
        public float? windDirection { get; set; }
        public float? windGust { get; set; }
        public float? pressureSeaLevel { get; set; }
        public float? precipitationIntensity { get; set; }
        public float? precipitationProbability { get; set; }
        public DateTimeOffset sunriseTime { get; set; }
        public DateTimeOffset sunsetTime { get; set; }
        public float? visibility { get; set; }
        public float? cloudCover { get; set; }
        public int? moonPhase { get; set; }
        public int? weatherCode { get; set; }
        public int? treeIndex { get; set; }
        public int? grassIndex { get; set; }
        public int? weedIndex { get; set; }
        public int? epaIndex { get; set; }
        public int? weatherCodeDay { get; set; }
        public int? weatherCodeFullDay { get; set; }
        public int? weatherCodeNight { get; set; }
        public double? particulateMatter25 { get; set; }
        public double? pollutantCO { get; set; }
        public double? pollutantNO2 { get; set; }
        public double? pollutantSO2 { get; set; }
        public double? pollutantO3 { get; set; }
        public double? particulateMatter10 { get; set; }
    }

    /*
    public class Warning
    {
        public int code { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public Meta meta { get; set; }
    }
    */

    public class Meta
    {
        public string timestep { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string field { get; set; }
        public string[] timesteps { get; set; }
    }
}
