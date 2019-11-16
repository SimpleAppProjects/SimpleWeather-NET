namespace SimpleWeather.WeatherUnderground
{
    public class Rootobject
    {
        public Response response { get; set; }
        public Current_Observation current_observation { get; set; }
        public Forecast forecast { get; set; }
        public Hourly_Forecast[] hourly_forecast { get; set; }
        public Moon_Phase moon_phase { get; set; }
        public Sun_Phase sun_phase { get; set; }
        public string query_zone { get; set; }
        public Alert[] alerts { get; set; }
    }

    public class Response
    {
        public string version { get; set; }
        public string termsofService { get; set; }
        public Features features { get; set; }
        public Error error { get; set; }
    }

    public class Features
    {
        public int astronomy { get; set; }
        public int conditions { get; set; }
        public int forecast10day { get; set; }
        public int hourly { get; set; }
    }

    public class Current_Observation
    {
        public Image image { get; set; }
        public Display_Location display_location { get; set; }
        public Observation_Location observation_location { get; set; }
        public Estimated estimated { get; set; }
        public string station_id { get; set; }
        public string observation_time { get; set; }
        public string observation_time_rfc822 { get; set; }
        public string observation_epoch { get; set; }
        public string local_time_rfc822 { get; set; }
        public string local_epoch { get; set; }
        public string local_tz_short { get; set; }
        public string local_tz_long { get; set; }
        public string local_tz_offset { get; set; }
        public string weather { get; set; }
        public string temperature_string { get; set; }
        public float temp_f { get; set; }
        public float temp_c { get; set; }
        public string relative_humidity { get; set; }
        public string wind_string { get; set; }
        public string wind_dir { get; set; }
        public int wind_degrees { get; set; }
        public float wind_mph { get; set; }
        public string wind_gust_mph { get; set; }
        public float wind_kph { get; set; }
        public string wind_gust_kph { get; set; }
        public string pressure_mb { get; set; }
        public string pressure_in { get; set; }
        public string pressure_trend { get; set; }
        public string dewpoint_string { get; set; }
        public string dewpoint_f { get; set; }
        public string dewpoint_c { get; set; }
        public string heat_index_string { get; set; }
        public string heat_index_f { get; set; }
        public string heat_index_c { get; set; }
        public string windchill_string { get; set; }
        public string windchill_f { get; set; }
        public string windchill_c { get; set; }
        public string feelslike_string { get; set; }
        public float feelslike_f { get; set; }
        public float feelslike_c { get; set; }
        public string visibility_mi { get; set; }
        public string visibility_km { get; set; }
        public string solarradiation { get; set; }
        public string UV { get; set; }
        public string precip_1hr_string { get; set; }
        public string precip_1hr_in { get; set; }
        public string precip_1hr_metric { get; set; }
        public string precip_today_string { get; set; }
        public string precip_today_in { get; set; }
        public string precip_today_metric { get; set; }
        public string icon { get; set; }
        public string icon_url { get; set; }
        public string forecast_url { get; set; }
        public string history_url { get; set; }
        public string ob_url { get; set; }
        public string nowcast { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
        public string title { get; set; }
        public string link { get; set; }
    }

    public class Display_Location
    {
        public string full { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string state_name { get; set; }
        public string country { get; set; }
        public string country_iso3166 { get; set; }
        public string zip { get; set; }
        public string magic { get; set; }
        public string wmo { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string elevation { get; set; }
    }

    public class Observation_Location
    {
        public string full { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string country_iso3166 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string elevation { get; set; }
    }

    public class Estimated
    {
    }

    public class Forecast
    {
        public Txt_Forecast txt_forecast { get; set; }
        public Simpleforecast simpleforecast { get; set; }
    }

    public class Txt_Forecast
    {
        public string date { get; set; }
        public Forecastday[] forecastday { get; set; }
    }

    public class Forecastday
    {
        public int period { get; set; }
        public string icon { get; set; }
        public string icon_url { get; set; }
        public string title { get; set; }
        public string fcttext { get; set; }
        public string fcttext_metric { get; set; }
        public string pop { get; set; }
    }

    public class Simpleforecast
    {
        public Forecastday1[] forecastday { get; set; }
    }

    public class Forecastday1
    {
        public Date date { get; set; }
        public int period { get; set; }
        public High high { get; set; }
        public Low low { get; set; }
        public string conditions { get; set; }
        public string icon { get; set; }
        public string icon_url { get; set; }
        public string skyicon { get; set; }
        public int pop { get; set; }
        public Qpf_Allday qpf_allday { get; set; }
        public Qpf_Day qpf_day { get; set; }
        public Qpf_Night qpf_night { get; set; }
        public Snow_Allday snow_allday { get; set; }
        public Snow_Day snow_day { get; set; }
        public Snow_Night snow_night { get; set; }
        public Maxwind maxwind { get; set; }
        public Avewind avewind { get; set; }
        public int avehumidity { get; set; }
        public int maxhumidity { get; set; }
        public int minhumidity { get; set; }
    }

    public class Date
    {
        public string epoch { get; set; }
        public string pretty { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int yday { get; set; }
        public int hour { get; set; }
        public string min { get; set; }
        public int sec { get; set; }
        public string isdst { get; set; }
        public string monthname { get; set; }
        public string monthname_short { get; set; }
        public string weekday_short { get; set; }
        public string weekday { get; set; }
        public string ampm { get; set; }
        public string tz_short { get; set; }
        public string tz_long { get; set; }
    }

    public class High
    {
        public string fahrenheit { get; set; }
        public string celsius { get; set; }
    }

    public class Low
    {
        public string fahrenheit { get; set; }
        public string celsius { get; set; }
    }

    public class Qpf_Allday
    {
        public float? _in { get; set; }
        public int? mm { get; set; }
    }

    public class Qpf_Day
    {
        public float? _in { get; set; }
        public int? mm { get; set; }
    }

    public class Qpf_Night
    {
        public float? _in { get; set; }
        public int? mm { get; set; }
    }

    public class Snow_Allday
    {
        public float? _in { get; set; }
        public float? cm { get; set; }
    }

    public class Snow_Day
    {
        public float? _in { get; set; }
        public float? cm { get; set; }
    }

    public class Snow_Night
    {
        public float? _in { get; set; }
        public float? cm { get; set; }
    }

    public class Maxwind
    {
        public int mph { get; set; }
        public int kph { get; set; }
        public string dir { get; set; }
        public int degrees { get; set; }
    }

    public class Avewind
    {
        public int mph { get; set; }
        public int kph { get; set; }
        public string dir { get; set; }
        public int degrees { get; set; }
    }

    public class Moon_Phase
    {
        public string percentIlluminated { get; set; }
        public string ageOfMoon { get; set; }
        public string phaseofMoon { get; set; }
        public string hemisphere { get; set; }
        public Current_Time current_time { get; set; }
        public Sunrise sunrise { get; set; }
        public Sunset sunset { get; set; }
        public Moonrise moonrise { get; set; }
        public Moonset moonset { get; set; }
    }

    public class Current_Time
    {
        public string hour { get; set; }
        public string minute { get; set; }
    }

    public class Sunrise
    {
        public string hour { get; set; }
        public string minute { get; set; }
    }

    public class Sunset
    {
        public string hour { get; set; }
        public string minute { get; set; }
    }

    public class Moonrise
    {
        public string hour { get; set; }
        public string minute { get; set; }
    }

    public class Moonset
    {
        public string hour { get; set; }
        public string minute { get; set; }
    }

    public class Sun_Phase
    {
        public Sunrise1 sunrise { get; set; }
        public Sunset1 sunset { get; set; }
    }

    public class Sunrise1
    {
        public string hour { get; set; }
        public string minute { get; set; }
    }

    public class Sunset1
    {
        public string hour { get; set; }
        public string minute { get; set; }
    }

    public class Error
    {
        public string type { get; set; }
        public string description { get; set; }
    }

    public class Hourly_Forecast
    {
        public FCTTIME FCTTIME { get; set; }
        public Temp temp { get; set; }
        public Dewpoint dewpoint { get; set; }
        public string condition { get; set; }
        public string icon { get; set; }
        public string icon_url { get; set; }
        public string fctcode { get; set; }
        public string sky { get; set; }
        public Wspd wspd { get; set; }
        public Wdir wdir { get; set; }
        public string wx { get; set; }
        public string uvi { get; set; }
        public string humidity { get; set; }
        public Windchill windchill { get; set; }
        public Heatindex heatindex { get; set; }
        public Feelslike feelslike { get; set; }
        public Qpf qpf { get; set; }
        public Snow snow { get; set; }
        public string pop { get; set; }
        public Mslp mslp { get; set; }
    }

    public class FCTTIME
    {
        public string hour { get; set; }
        public string hour_padded { get; set; }
        public string min { get; set; }
        public string min_unpadded { get; set; }
        public string sec { get; set; }
        public string year { get; set; }
        public string mon { get; set; }
        public string mon_padded { get; set; }
        public string mon_abbrev { get; set; }
        public string mday { get; set; }
        public string mday_padded { get; set; }
        public string yday { get; set; }
        public string isdst { get; set; }
        public string epoch { get; set; }
        public string pretty { get; set; }
        public string civil { get; set; }
        public string month_name { get; set; }
        public string month_name_abbrev { get; set; }
        public string weekday_name { get; set; }
        public string weekday_name_night { get; set; }
        public string weekday_name_abbrev { get; set; }
        public string weekday_name_unlang { get; set; }
        public string weekday_name_night_unlang { get; set; }
        public string ampm { get; set; }
        public string tz { get; set; }
        public string age { get; set; }
        public string UTCDATE { get; set; }
    }

    public class Temp
    {
        public string english { get; set; }
        public string metric { get; set; }
    }

    public class Dewpoint
    {
        public string english { get; set; }
        public string metric { get; set; }
    }

    public class Wspd
    {
        public string english { get; set; }
        public string metric { get; set; }
    }

    public class Wdir
    {
        public string dir { get; set; }
        public string degrees { get; set; }
    }

    public class Windchill
    {
        public string english { get; set; }
        public string metric { get; set; }
    }

    public class Heatindex
    {
        public string english { get; set; }
        public string metric { get; set; }
    }

    public class Feelslike
    {
        public string english { get; set; }
        public string metric { get; set; }
    }

    public class Qpf
    {
        public string english { get; set; }
        public string metric { get; set; }
    }

    public class Snow
    {
        public string english { get; set; }
        public string metric { get; set; }
    }

    public class Mslp
    {
        public string english { get; set; }
        public string metric { get; set; }
    }
}
