using System;

namespace SimpleWeather.Weather_API.NWS
{
    public class PointsRootobject
    {
        //[DataMember(Name = "@context")]
        //public Context context { get; set; }
        //[DataMember(Name = "@id")]
        //public string id { get; set; }
        //[DataMember(Name = "@type")]
        //public string type { get; set; }
        //public string geometry { get; set; }
        //public string cwa { get; set; }
        //public string forecastOffice { get; set; }
        //public int gridX { get; set; }
        //public int gridY { get; set; }
        public string forecast { get; set; }
        public string forecastHourly { get; set; }
        public string forecastGridData { get; set; }
        public string observationStations { get; set; }
        //public Relativelocation relativeLocation { get; set; }
        //public string forecastZone { get; set; }
        //public string county { get; set; }
        //public string fireWeatherZone { get; set; }
        public string timeZone { get; set; }
        //public string radarStation { get; set; }
    }

    /*
    public partial class Context
    {
        public string s { get; set; }
        public string geo { get; set; }
        public string unit { get; set; }
        public Geometry geometry { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public Distance distance { get; set; }
        public Bearing bearing { get; set; }
        public Value value { get; set; }
        public Unitcode unitCode { get; set; }
        public Forecastoffice forecastOffice { get; set; }
        public Forecastgriddata forecastGridData { get; set; }
        public Publiczone publicZone { get; set; }
        public County county { get; set; }
    }
    */

    /*
    public class Geometry
    {
        [DataMember(Name = "@id")]
        public string id { get; set; }
        [DataMember(Name = "@type")]
        public string type { get; set; }
    }
    public class Distance
    {
        [DataMember(Name = "@id")]
        public string id { get; set; }
        [DataMember(Name = "@type")]
        public string type { get; set; }
    }
    public class Bearing
    {
        [DataMember(Name = "@type")]
        public string type { get; set; }
    }
    public class Value
    {
        [DataMember(Name = "@id")]
        public string id { get; set; }
    }
    public class Unitcode
    {
        [DataMember(Name = "@id")]
        public string id { get; set; }
        [DataMember(Name = "@type")]
        public string type { get; set; }
    }
    public class Forecastoffice
    {
        [DataMember(Name = "@type")]
        public string type { get; set; }
    }
    public class Forecastgriddata
    {
        [DataMember(Name = "@type")]
        public string type { get; set; }
    }
    public class Publiczone
    {
        [DataMember(Name = "@type")]
        public string type { get; set; }
    }
    public class County
    {
        [DataMember(Name = "@type")]
        public string type { get; set; }
    }
    public class Relativelocation
    {
        public string city { get; set; }
        public string state { get; set; }
        public string geometry { get; set; }
        public Distance1 distance { get; set; }
        public Bearing1 bearing { get; set; }
    }
    public class Distance1
    {
        public float value { get; set; }
        public string unitCode { get; set; }
    }
    public class Bearing1
    {
        public int value { get; set; }
        public string unitCode { get; set; }
    }
    */

    public class HourlyPointsRootobject
    {
        //public Context context { get; set; }
        //public string geometry { get; set; }
        public DateTimeOffset updated { get; set; }
        public string units { get; set; }
        //public string forecastGenerator { get; set; }
        public DateTimeOffset generatedAt { get; set; }
        public DateTimeOffset updateTime { get; set; }
        public string validTimes { get; set; }
        //public Elevation elevation { get; set; }
        public Period[] periods { get; set; }
    }

    /*
    public class Elevation
    {
        public float value { get; set; }
        public string unitCode { get; set; }
    }
    */

    public class Period
    {
        public int number { get; set; }
        public string name { get; set; }
        public DateTimeOffset startTime { get; set; }
        public DateTimeOffset endTime { get; set; }
        public bool isDaytime { get; set; }
        public int? temperature { get; set; }
        public string temperatureUnit { get; set; }
        public string temperatureTrend { get; set; }
        public string windSpeed { get; set; }
        public string windDirection { get; set; }
        public string icon { get; set; }
        public string shortForecast { get; set; }
        public string detailedForecast { get; set; }
        public Dewpoint dewpoint { get; set; }
        public Probabilityofprecipitation probabilityOfPrecipitation { get; set; }
        public Relativehumidity relativeHumidity { get; set; }
    }

    public class ObservationsStationsRootobject
    {
        //public Context context { get; set; }
        //public ObsGraph[] graph { get; set; }
        public string[] observationStations { get; set; }
    }

    /*
    public class Observationstations
    {
        public string container { get; set; }
        public string type { get; set; }
    }
    public class ObsGraph
    {
        public Elevation elevation { get; set; }
        public string stationIdentifier { get; set; }
        public string name { get; set; }
        public string timeZone { get; set; }
    }
    */

    public class ObservationsCurrentRootobject
    {
        //public Context context { get; set; }
        //[DataMember(Name = "@id")]
        //public string id { get; set; }
        //[DataMember(Name = "@type")]
        //public string type { get; set; }
        //public string geometry { get; set; }
        //public Elevation elevation { get; set; }
        //public string station { get; set; }
        public DateTimeOffset timestamp { get; set; }
        //public string rawMessage { get; set; }
        public string textDescription { get; set; }
        public string icon { get; set; }
        //public object[] presentWeather { get; set; }
        public Temperature temperature { get; set; }
        public Dewpoint dewpoint { get; set; }
        public Winddirection windDirection { get; set; }
        public Windspeed windSpeed { get; set; }
        public Windgust windGust { get; set; }
        public Barometricpressure barometricPressure { get; set; }
        //public Sealevelpressure seaLevelPressure { get; set; }
        public Visibility visibility { get; set; }
        //public Maxtemperaturelast24hours maxTemperatureLast24Hours { get; set; }
        //public Mintemperaturelast24hours minTemperatureLast24Hours { get; set; }
        public Precipitationlasthour precipitationLastHour { get; set; }
        public Precipitationlast3hours precipitationLast3Hours { get; set; }
        public Precipitationlast6hours precipitationLast6Hours { get; set; }
        public Relativehumidity relativeHumidity { get; set; }
        public Windchill windChill { get; set; }
        public Heatindex heatIndex { get; set; }
        //public Cloudlayer[] cloudLayers { get; set; }
    }

    public class Temperature
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Dewpoint
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Winddirection
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Windspeed
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Windgust
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Barometricpressure
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    /*
    public class Sealevelpressure
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }
    */

    public class Visibility
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    /*
    public class Maxtemperaturelast24hours
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }
    public class Mintemperaturelast24hours
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }
    */

    public class Precipitationlasthour
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Precipitationlast3hours
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Precipitationlast6hours
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Relativehumidity
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Windchill
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    public class Heatindex
    {
        public float? value { get; set; }
        public string unitCode { get; set; }
        public string qualityControl { get; set; }
    }

    /*
    public class Cloudlayer
    {
        public Base _base { get; set; }
        public string amount { get; set; }
    }
    public class Base
    {
        public int value { get; set; }
        public string unitCode { get; set; }
    }
    */

    public class Probabilityofprecipitation
    {
        public string unitCode { get; set; }
        public float? value { get; set; }
    }
}
