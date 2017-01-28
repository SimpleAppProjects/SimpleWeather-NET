using System;
using System.Runtime.Serialization;

namespace SimpleWeather.WeatherUnderground
{
    [DataContract]
    public class Weather
    {
        [DataMember]
        public Location location { get; set; }
        [DataMember]
        public DateTime update_time { get; set; }
        [DataMember]
        public Current_Observation condition { get; set; }
        [DataMember]
        public Simpleforecast forecast { get; set; }
        [DataMember]
        public Sun_Phase sun_phase { get; set; }

        public Weather(Rootobject root)
        {
            condition = root.current_observation;
            forecast = root.forecast.simpleforecast;
            sun_phase = root.sun_phase;
            update_time = DateTime.Parse(root.current_observation.local_time_rfc822);

            location = new Location();
            location.full_name = condition.display_location.full;
            location.city = condition.display_location.city;
            location.state = condition.display_location.state;
            location.state_name = condition.display_location.state_name;
            location.country = condition.display_location.country;
            location.zip = condition.display_location.zip;
            location.latitude = condition.display_location.latitude;
            location.longitude = condition.display_location.longitude;
            location.tz_offset = condition.local_tz_offset;
    }
}

    public class Location
    {
        public string full_name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string state_name { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string tz_offset { get; set; }
    }

    public class Rootobject
    {
        public Response response { get; set; }
        public Current_Observation current_observation { get; set; }
        public Forecast forecast { get; set; }
        public Moon_Phase moon_phase { get; set; }
        public Sun_Phase sun_phase { get; set; }
    }

    public class Response
    {
        public string version { get; set; }
        public string termsofService { get; set; }
        public Features features { get; set; }
    }

    public class Features
    {
        public int astronomy { get; set; }
        public int conditions { get; set; }
        public int forecast10day { get; set; }
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
        public int dewpoint_f { get; set; }
        public int dewpoint_c { get; set; }
        public string heat_index_string { get; set; }
        public string heat_index_f { get; set; }
        public string heat_index_c { get; set; }
        public string windchill_string { get; set; }
        public string windchill_f { get; set; }
        public string windchill_c { get; set; }
        public string feelslike_string { get; set; }
        public string feelslike_f { get; set; }
        public string feelslike_c { get; set; }
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
        [IgnoreDataMember]
        public float _in { get; set; }
        [IgnoreDataMember]
        public int mm { get; set; }
    }

    public class Qpf_Day
    {
        [IgnoreDataMember]
        public float _in { get; set; }
        [IgnoreDataMember]
        public int mm { get; set; }
    }

    public class Qpf_Night
    {
        [IgnoreDataMember]
        public float _in { get; set; }
        [IgnoreDataMember]
        public int mm { get; set; }
    }

    public class Snow_Allday
    {
        [IgnoreDataMember]
        public float _in { get; set; }
        [IgnoreDataMember]
        public float cm { get; set; }
    }

    public class Snow_Day
    {
        [IgnoreDataMember]
        public float _in { get; set; }
        [IgnoreDataMember]
        public float cm { get; set; }
    }

    public class Snow_Night
    {
        [IgnoreDataMember]
        public float _in { get; set; }
        [IgnoreDataMember]
        public float cm { get; set; }
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
}
