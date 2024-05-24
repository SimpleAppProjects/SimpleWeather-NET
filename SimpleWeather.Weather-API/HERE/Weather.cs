using System;

namespace SimpleWeather.Weather_API.HERE
{
    public class Rootobject
    {
        public Place[] places { get; set; }
        public string status { get; set; }
        public string error { get; set; }
    }

    public class Place
    {
        public Observation[] observations { get; set; }
        public Dailyforecast[] dailyForecasts { get; set; }
        public Hourlyforecast[] hourlyForecasts { get; set; }
        public Astronomyforecast[] astronomyForecasts { get; set; }
        public Alert[] alerts { get; set; }
        public Nwsalerts nwsAlerts { get; set; }
    }

    public class Nwsalerts
    {
        public Warning[] warnings { get; set; }
        public Watch[] watches { get; set; }
    }

    public class Warning
    {
        public Zone[] counties { get; set; }
        public Zone[] zones { get; set; }
        public Province[] provinces { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public int? severity { get; set; }
        public string message { get; set; }
        public string name { get; set; }
        public DateTimeOffset validFromTimeLocal { get; set; }
        public DateTimeOffset validUntilTimeLocal { get; set; }
    }

    public class Zone
    {
        public string value { get; set; }
        public string country { get; set; }
        public string countryName { get; set; }
        public string state { get; set; }
        public string stateName { get; set; }
        public string name { get; set; }
        public Location location { get; set; }
    }

    public class Location
    {
        public float? lat { get; set; }
        public float? lng { get; set; }
    }

    public class Watch
    {
        public Zone[] counties { get; set; }
        public Zone[] zones { get; set; }
        public Province[] provinces { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public int? severity { get; set; }
        public string message { get; set; }
        public string name { get; set; }
        public DateTimeOffset validFromTimeLocal { get; set; }
        public DateTimeOffset validUntilTimeLocal { get; set; }
    }

    public class Province
    {
        public string value { get; set; }
        public string country { get; set; }
        public string countryName { get; set; }
        public string province { get; set; }
        public string provinceName { get; set; }
        public string name { get; set; }
        public Location location { get; set; }
    }

    public class Observation
    {
        public Place1 place { get; set; }
        public string daylight { get; set; }
        public string description { get; set; }
        public string skyInfo { get; set; }
        public string skyDesc { get; set; }
        public string temperature { get; set; }
        public string temperatureDesc { get; set; }
        public string comfort { get; set; }
        public string highTemperature { get; set; }
        public string lowTemperature { get; set; }
        public string humidity { get; set; }
        public string dewPoint { get; set; }
        public string precipitation1H { get; set; }
        public string precipitation3H { get; set; }
        public string precipitation6H { get; set; }
        public string precipitation12H { get; set; }
        public string precipitation24H { get; set; }
        public string precipitationProbability { get; set; }
        public string precipitationDesc { get; set; }
        public string rainFall { get; set; }
        public string snowFall { get; set; }
        public string airInfo { get; set; }
        public string airDesc { get; set; }
        public string windSpeed { get; set; }
        public string windDirection { get; set; }
        public string windDesc { get; set; }
        public string windDescShort { get; set; }
        public string barometerPressure { get; set; }
        public string barometerTrend { get; set; }
        public string visibility { get; set; }
        public string snowCover { get; set; }
        public string iconId { get; set; }
        public string iconName { get; set; }
        public string iconLink { get; set; }
        public string ageMinutes { get; set; }
        public string activeAlerts { get; set; }
        public DateTimeOffset time { get; set; }
    }

    public class Place1
    {
        public Address address { get; set; }
        public Location location { get; set; }
        public string distance { get; set; }
    }

    public class Address
    {
        public string countryName { get; set; }
        public string state { get; set; }
        public string city { get; set; }
    }

    public class Dailyforecast
    {
        public Place1 place { get; set; }
        public Forecast[] forecasts { get; set; }
    }

    public class Forecast
    {
        public string daylight { get; set; }
        public string description { get; set; }
        public string skyInfo { get; set; }
        public string skyDesc { get; set; }
        public string temperature { get; set; }
        public string temperatureDesc { get; set; }
        public string comfort { get; set; }
        public string highTemperature { get; set; }
        public string lowTemperature { get; set; }
        public string humidity { get; set; }
        public string dewPoint { get; set; }
        public string precipitationProbability { get; set; }
        public string precipitationDesc { get; set; }
        public string rainFall { get; set; }
        public string snowFall { get; set; }
        public string airInfo { get; set; }
        public string airDesc { get; set; }
        public string windSpeed { get; set; }
        public string windDirection { get; set; }
        public string windDesc { get; set; }
        public string windDescShort { get; set; }
        public string beaufortScale { get; set; }
        public string beaufortDesc { get; set; }
        public string uvIndex { get; set; }
        public string uvDesc { get; set; }
        public string barometerPressure { get; set; }
        public string visibility { get; set; }
        public string icon { get; set; }
        public string iconName { get; set; }
        public string iconId { get; set; }
        public string iconLink { get; set; }
        public string weekday { get; set; }
        public DateTimeOffset time { get; set; }
        public string sunRise { get; set; }
        public string sunSet { get; set; }
        public string moonRise { get; set; }
        public string moonSet { get; set; }
        public string moonPhase { get; set; }
        public string moonPhaseDescription { get; set; }
    }

    public class Hourlyforecast
    {
        public Place1 place { get; set; }
        public Forecast[] forecasts { get; set; }
    }

    public class Astronomyforecast
    {
        public Place1 place { get; set; }
        public AstronomyItem[] forecasts { get; set; }
    }

    public class AstronomyItem
    {
        public string sunRise { get; set; }
        public string sunSet { get; set; }
        public string moonRise { get; set; }
        public string moonSet { get; set; }
        public string moonPhase { get; set; }
        public string moonPhaseDescription { get; set; }
        public string iconName { get; set; }
        public DateTimeOffset time { get; set; }
    }

    public class Alert
    {
        public Place1 place { get; set; }
        public Timesegment[] timeSegments { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }

    public class Timesegment
    {
        public string segment { get; set; }
        public string weekday { get; set; }
    }
}
