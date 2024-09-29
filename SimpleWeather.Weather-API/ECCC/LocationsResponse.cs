using System;

namespace SimpleWeather.Weather_API.ECCC
{
    public class LocationsItem
    {
        //public string cgndb { get; set; }
        public string displayName { get; set; }
        //public string zonePoly { get; set; }
        public long lastUpdated { get; set; }
        public Alert alert { get; set; }
        public Observation observation { get; set; }
        public Hourlyfcst hourlyFcst { get; set; }
        public Dailyfcst dailyFcst { get; set; }
        //public Aqhi aqhi { get; set; }
        public Riseset riseSet { get; set; }
        //public Risesetnextday riseSetNextDay { get; set; }
        //public Risedata[] riseData { get; set; }
        //public object[] metNotes { get; set; }
        //public Pasthourly pastHourly { get; set; }
    }

    public class Alert
    {
        //public string zoneId { get; set; }
        //public string uuid { get; set; }
        //public string mostSevere { get; set; }
        //public string hwyMostSevere { get; set; }
        public AlertsItem[] alerts { get; set; }
        //public object[] hwyAlerts { get; set; }
    }

    public class AlertsItem
    {
        public string type { get; set; }
        public int sequence { get; set; }
        public string status { get; set; }
        public string transitionStatus { get; set; }
        public DateTimeOffset issueTime { get; set; }
        public string timezone { get; set; }
        public string issueTimeText { get; set; }
        public string issuingOfficeTZ { get; set; }
        public DateTimeOffset expiryTime { get; set; }
        public DateTimeOffset? eventOnsetTime { get; set; }
        public DateTimeOffset eventEndTime { get; set; }
        public string alertId { get; set; }
        public string alertCode { get; set; }
        public string program { get; set; }
        public string alertBannerText { get; set; }
        public string alertHeaderText { get; set; }
        public string bannerColour { get; set; }
        public string zoneId { get; set; }
        public string[] zones { get; set; }
        public string text { get; set; }
        //public Special_Text[] special_text { get; set; }
        public string tcisURL { get; set; }
    }

    /*
    public class Special_Text
    {
        public string type { get; set; }
        public string link { get; set; }
    }
    */

    public class Observation
    {
        public string observedAt { get; set; }
        public string provinceCode { get; set; }
        public string climateId { get; set; }
        public string tcid { get; set; }
        public DateTimeOffset timeStamp { get; set; }
        public string timeStampText { get; set; }
        public string iconCode { get; set; }
        public string condition { get; set; }
        public Temperature temperature { get; set; }
        public Temperature dewpoint { get; set; }
        public ItemValue feelsLike { get; set; }
        public ItemValue pressure { get; set; }
        public string tendency { get; set; }
        public ItemValue visibility { get; set; }
        public string humidity { get; set; }
        //public int humidityQaValue { get; set; }
        public ItemValue windSpeed { get; set; }
        public ItemValue windGust { get; set; }
        public string windDirection { get; set; }
        //public int windDirectionQAValue { get; set; }
        public string windBearing { get; set; }
    }

    public class Temperature
    {
        public string imperial { get; set; }
        public string imperialUnrounded { get; set; }
        public string metric { get; set; }
        public string metricUnrounded { get; set; }
    }

    public class Hourlyfcst
    {
        public string hourlyIssuedTimeShrt { get; set; }
        public Hourly[] hourly { get; set; }
    }

    public class Hourly
    {
        public string date { get; set; }
        public int periodID { get; set; }
        public ItemValue windGust { get; set; }
        public string windDir { get; set; }
        public ItemValue feelsLike { get; set; }
        public string condition { get; set; }
        public string precip { get; set; }
        public ItemValue temperature { get; set; }
        public string iconCode { get; set; }
        public string time { get; set; }
        public ItemValue windSpeed { get; set; }
        public long? epochTime { get; set; }
        public string dateShrt { get; set; }
        public Uv uv { get; set; }
    }

    public class ItemValue
    {
        public string metric { get; set; }
        public string imperial { get; set; }
    }

    public class Uv
    {
        public string index { get; set; }
    }

    public class Dailyfcst
    {
        public string dailyIssuedTimeShrt { get; set; }
        //public Regionalnormals regionalNormals { get; set; }
        public Daily[] daily { get; set; }
        public string dailyIssuedTime { get; set; }
        public string dailyIssuedTimeEpoch { get; set; }
    }

    /*
    public class Regionalnormals
    {
        public Metric metric { get; set; }
        public Imperial imperial { get; set; }
    }

    public class Metric
    {
        public int highTemp { get; set; }
        public int lowTemp { get; set; }
        public string text { get; set; }
    }

    public class Imperial
    {
        public int highTemp { get; set; }
        public int lowTemp { get; set; }
        public string text { get; set; }
    }
    */

    public class Daily
    {
        public string date { get; set; }
        public string summary { get; set; }
        public int periodID { get; set; }
        public string periodLabel { get; set; }
        //public Windchill windChill { get; set; }
        //public object sun { get; set; }
        public string temperatureText { get; set; }
        //public object humidex { get; set; }
        public string precip { get; set; }
        //public Frost frost { get; set; }
        public string titleText { get; set; }
        public DailyTemperature temperature { get; set; }
        public string iconCode { get; set; }
        public string text { get; set; }
        //public Visibility1 visibility { get; set; }
    }

    /*
    public class Windchill
    {
        public object[] calculated { get; set; }
        public string textSummary { get; set; }
    }

    public class Frost
    {
        public string textSummary { get; set; }
    }
    */

    public class DailyTemperature
    {
        public string metric { get; set; }
        public string imperial { get; set; }
        public int? periodLow { get; set; }
        public int? periodHigh { get; set; }
    }

    /*
    public class Visibility1
    {
        public Othervisib otherVisib { get; set; }
    }

    public class Othervisib
    {
        public string cause { get; set; }
        public string textSummary { get; set; }
    }

    public class Aqhi
    {
        public string url { get; set; }
    }
    */

    public class Riseset
    {
        public Set set { get; set; }
        public string timeZone { get; set; }
        public Rise rise { get; set; }
    }

    public class Set
    {
        public string time12h { get; set; }
        public string epochTimeRounded { get; set; }
        public string time { get; set; }
    }

    public class Rise
    {
        public string time12h { get; set; }
        public string epochTimeRounded { get; set; }
        public string time { get; set; }
    }

    /*
    public class Risesetnextday
    {
        public Set1 set { get; set; }
        public string timeZone { get; set; }
        public Rise1 rise { get; set; }
    }


    public class Set1
    {
        public string time12h { get; set; }
        public string epochTimeRounded { get; set; }
        public string time { get; set; }
    }

    public class Rise1
    {
        public string time12h { get; set; }
        public string epochTimeRounded { get; set; }
        public string time { get; set; }
    }

    public class Pasthourly
    {
        public string observedAt { get; set; }
        public string provinceCode { get; set; }
        public Hour[] hours { get; set; }
    }

    public class Hour
    {
        public DateTimeOffset timeStamp { get; set; }
        public float temperature { get; set; }
        public int temperatureQaValue { get; set; }
        public float dewpoint { get; set; }
        public float pressure { get; set; }
        public int humidity { get; set; }
        public int humidityQaValue { get; set; }
        public int windSpeed { get; set; }
        public int windSpeedQaValue { get; set; }
        public string windDirection { get; set; }
        public int windDirectionQaValue { get; set; }
        public int windGust { get; set; }
        public int windGustQaValue { get; set; }
    }

    public class Risedata
    {
        public Set2 set { get; set; }
        public string timeZone { get; set; }
        public Rise2 rise { get; set; }
    }

    public class Set2
    {
        public string time12h { get; set; }
        public string epochTimeRounded { get; set; }
        public string time { get; set; }
    }

    public class Rise2
    {
        public string time12h { get; set; }
        public string epochTimeRounded { get; set; }
        public string time { get; set; }
    }
    */
}
