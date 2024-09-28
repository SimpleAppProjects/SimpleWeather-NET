using System;

namespace SimpleWeather.Weather_API.BrightSky
{
    public class AlertsRootobject
    {
        public Alert[] alerts { get; set; }
        public Location location { get; set; }
    }

    public class Location
    {
        public int? warn_cell_id { get; set; }
        public string name { get; set; }
        public string name_short { get; set; }
        public string district { get; set; }
        public string state { get; set; }
        public string state_short { get; set; }
    }

    public class Alert
    {
        public int id { get; set; }
        public string alert_id { get; set; }
        public string status { get; set; }
        public DateTimeOffset effective { get; set; }
        public DateTimeOffset onset { get; set; }
        public DateTimeOffset? expires { get; set; }
        public string category { get; set; }
        public string response_type { get; set; }
        public string urgency { get; set; }
        public string severity { get; set; }
        public string certainty { get; set; }
        public int? event_code { get; set; }
        public string event_en { get; set; }
        public string event_de { get; set; }
        public string headline_en { get; set; }
        public string headline_de { get; set; }
        public string description_en { get; set; }
        public string description_de { get; set; }
        public string instruction_en { get; set; }
        public string instruction_de { get; set; }
    }

}
