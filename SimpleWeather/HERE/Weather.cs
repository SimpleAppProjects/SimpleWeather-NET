using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.HERE
{
    public class Rootobject
    {
        public Observations observations { get; set; }
        public Dailyforecasts dailyForecasts { get; set; }
        public Hourlyforecasts hourlyForecasts { get; set; }
        public Alerts alerts { get; set; }
        public Astronomy astronomy { get; set; }
        public DateTimeOffset feedCreation { get; set; }
        public bool metric { get; set; }
        public string Type { get; set; }
        public string[] Message { get; set; }
    }

    public class Observations
    {
        public Location[] location { get; set; }
    }

    public class Location
    {
        public Observation[] observation { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float distance { get; set; }
        public int timezone { get; set; }
    }

    public class Observation
    {
        public string daylight { get; set; }
        public string description { get; set; }
        public string skyInfo { get; set; }
        public string skyDescription { get; set; }
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
        public string precipitationDesc { get; set; }
        public string airInfo { get; set; }
        public string airDescription { get; set; }
        public string windSpeed { get; set; }
        public string windDirection { get; set; }
        public string windDesc { get; set; }
        public string windDescShort { get; set; }
        public string barometerPressure { get; set; }
        public string barometerTrend { get; set; }
        public string visibility { get; set; }
        public string snowCover { get; set; }
        public string icon { get; set; }
        public string iconName { get; set; }
        public string iconLink { get; set; }
        public string ageMinutes { get; set; }
        public string activeAlerts { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float distance { get; set; }
        public float elevation { get; set; }
        public DateTimeOffset utcTime { get; set; }
    }

    public class Dailyforecasts
    {
        public Forecastlocation forecastLocation { get; set; }
    }

    public class Forecastlocation
    {
        public Forecast[] forecast { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float distance { get; set; }
        public int timezone { get; set; }
    }

    public class Forecast
    {
        public string daylight { get; set; }
        public string description { get; set; }
        public string skyInfo { get; set; }
        public string skyDescription { get; set; }
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
        public string airDescription { get; set; }
        public string windSpeed { get; set; }
        public string windDirection { get; set; }
        public string windDesc { get; set; }
        public string windDescShort { get; set; }
        public string beaufortScale { get; set; }
        public string beaufortDescription { get; set; }
        public string uvIndex { get; set; }
        public string uvDesc { get; set; }
        public string barometerPressure { get; set; }
        public string icon { get; set; }
        public string iconName { get; set; }
        public string iconLink { get; set; }
        public string dayOfWeek { get; set; }
        public string weekday { get; set; }
        public DateTimeOffset utcTime { get; set; }
    }

    public class Hourlyforecasts
    {
        public Forecastlocation1 forecastLocation { get; set; }
    }

    public class Forecastlocation1
    {
        public Forecast1[] forecast { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float distance { get; set; }
        public int timezone { get; set; }
    }

    public class Forecast1
    {
        public string daylight { get; set; }
        public string description { get; set; }
        public string skyInfo { get; set; }
        public string skyDescription { get; set; }
        public string temperature { get; set; }
        public string temperatureDesc { get; set; }
        public string comfort { get; set; }
        public string humidity { get; set; }
        public string dewPoint { get; set; }
        public string precipitationProbability { get; set; }
        public string precipitationDesc { get; set; }
        public string rainFall { get; set; }
        public string snowFall { get; set; }
        public string airInfo { get; set; }
        public string airDescription { get; set; }
        public string windSpeed { get; set; }
        public string windDirection { get; set; }
        public string windDesc { get; set; }
        public string windDescShort { get; set; }
        public string beaufortScale { get; set; }
        public string beaufortDescription { get; set; }
        public string uvIndex { get; set; }
        public string uvDesc { get; set; }
        public string visibility { get; set; }
        public string barometerPressure { get; set; }
        public string icon { get; set; }
        public string iconName { get; set; }
        public string iconLink { get; set; }
        public string dayOfWeek { get; set; }
        public string weekday { get; set; }
        public DateTimeOffset utcTime { get; set; }
        public string localTime { get; set; }
        public string localTimeFormat { get; set; }
    }

    public partial class Alerts
    {
        //public object[] alerts { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int timezone { get; set; }
    }

    public class Astronomy
    {
        public Astronomy1[] astronomy { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int timezone { get; set; }
    }

    public class Astronomy1
    {
        public string sunrise { get; set; }
        public string sunset { get; set; }
        public string moonrise { get; set; }
        public string moonset { get; set; }
        public float moonPhase { get; set; }
        public string moonPhaseDesc { get; set; }
        public string iconName { get; set; }
        public string city { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public DateTimeOffset utcTime { get; set; }
    }
}
