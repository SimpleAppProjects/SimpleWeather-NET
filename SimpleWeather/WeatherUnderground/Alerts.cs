namespace SimpleWeather.WeatherUnderground
{
    public class AlertRootobject
    {
        public Response response { get; set; }
        public Alert[] alerts { get; set; }
    }

    public class AlertFeatures
    {
        public int alerts { get; set; }
    }

    // NWS Alerts
    public partial class Alert
    {
        public string type { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public string date_epoch { get; set; }
        public string expires { get; set; }
        public string expires_epoch { get; set; }
        public string message { get; set; }
        public string phenomena { get; set; }
        public string significance { get; set; }
        //public AlertZONE[] ZONES { get; set; }
        //public Stormbased StormBased { get; set; }
    }

    public class Stormbased
    {
        public Vertex[] vertices { get; set; }
        public int Vertex_count { get; set; }
        public Storminfo stormInfo { get; set; }
    }

    public class Storminfo
    {
        public int time_epoch { get; set; }
        public int Motion_deg { get; set; }
        public int Motion_spd { get; set; }
        public float position_lat { get; set; }
        public float position_lon { get; set; }
    }

    public class Vertex
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }

    public class AlertZONE
    {
        public string state { get; set; }
        public string ZONE { get; set; }
    }

    // Meteoalarm.eu Alerts
    public partial class Alert
    {
        public string wtype_meteoalarm { get; set; }
        public string wtype_meteoalarm_name { get; set; }
        public string level_meteoalarm { get; set; }
        public string level_meteoalarm_name { get; set; }
        public string level_meteoalarm_description { get; set; }
        public string attribution { get; set; }
    }
}
