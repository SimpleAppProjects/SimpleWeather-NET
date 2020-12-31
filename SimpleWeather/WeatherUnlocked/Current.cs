namespace SimpleWeather.WeatherUnlocked
{
    public class CurrentRootobject
    {
        public float lat { get; set; }
        public float lon { get; set; }
        public float alt_m { get; set; }
        public float alt_ft { get; set; }
        public string wx_desc { get; set; }
        public int wx_code { get; set; }
        public string wx_icon { get; set; }
        public float temp_c { get; set; }
        public float temp_f { get; set; }
        public float feelslike_c { get; set; }
        public float feelslike_f { get; set; }
        public float humid_pct { get; set; }
        public float windspd_mph { get; set; }
        public float windspd_kmh { get; set; }
        public float windspd_kts { get; set; }
        public float windspd_ms { get; set; }
        public float winddir_deg { get; set; }
        public string winddir_compass { get; set; }
        public float cloudtotal_pct { get; set; }
        public float vis_km { get; set; }
        public float vis_mi { get; set; }
        //public object vis_desc { get; set; }
        public float slp_mb { get; set; }
        public float slp_in { get; set; }
        public float dewpoint_c { get; set; }
        public float dewpoint_f { get; set; }
    }
}