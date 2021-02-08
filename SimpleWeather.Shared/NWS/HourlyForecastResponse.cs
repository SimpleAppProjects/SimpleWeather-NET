using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.NWS.Hourly
{
    public class HourlyForecastResponse
    {
        public DateTimeOffset creationDate { get; set; }
        public Location location { get; set; }
        public List<PeriodsItem> periodsItems { get; set; }
    }

    public class Location
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class PeriodItem
    {
        public string unixTime { get; set; }
        public string windChill { get; set; }
        public string windSpeed { get; set; }
        public string cloudAmount { get; set; }
        public string pop { get; set; }
        public string relativeHumidity { get; set; }
        public string windGust { get; set; }
        public string temperature { get; set; }
        public string windDirection { get; set; }
        public string iconLink { get; set; }
        public string weather { get; set; }

        public PeriodItem(String unixTime, String windChill, String windSpeed, String cloudAmount,
                      String pop, String relativeHumidity, String windGust, String temperature,
                      String windDirection, String iconLink, String weather)
        {
            this.unixTime = unixTime;
            this.windChill = windChill;
            this.windSpeed = windSpeed;
            this.cloudAmount = cloudAmount;
            this.pop = pop;
            this.relativeHumidity = relativeHumidity;
            this.windGust = windGust;
            this.temperature = temperature;
            this.windDirection = windDirection;
            this.iconLink = iconLink;
            this.weather = weather;
        }
    }

    public class PeriodsItem
    {
        public List<string> time { get; set; }
        public List<string> unixtime { get; set; }
        public List<string> windChill { get; set; }
        public List<string> windGust { get; set; }
        public string periodName { get; set; }
        public List<string> pop { get; set; }
        public List<string> iconLink { get; set; }
        public List<string> relativeHumidity { get; set; }
        public List<string> temperature { get; set; }
        public List<string> weather { get; set; }
        public List<string> windDirection { get; set; }
        public List<string> windSpeed { get; set; }
        public List<string> cloudAmount { get; set; }
    }
}
