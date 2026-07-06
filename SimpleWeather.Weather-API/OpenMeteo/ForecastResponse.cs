namespace SimpleWeather.Weather_API.OpenMeteo
{
    public class ForecastResponse
    {
        public float? latitude { get; set; }
        public float? longitude { get; set; }
        public float? generationtime_ms { get; set; }
        public int? utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public float? elevation { get; set; }
        public Current_Units current_units { get; set; }
        public Current current { get; set; }
        public Minutely_15_Units minutely_15_units { get; set; }
        public Minutely_15 minutely_15 { get; set; }
        public Hourly_Units hourly_units { get; set; }
        public Hourly hourly { get; set; }
        public Daily_Units daily_units { get; set; }
        public Daily daily { get; set; }
    }

    public class Current_Units
    {
        public string time { get; set; }
        public string interval { get; set; }
        public string temperature_2m { get; set; }
        public string relative_humidity_2m { get; set; }
        public string dew_point_2m { get; set; }
        public string apparent_temperature { get; set; }
        public string precipitation_probability { get; set; }
        public string weather_code { get; set; }
        public string pressure_msl { get; set; }
        public string cloud_cover { get; set; }
        public string visibility { get; set; }
        public string wind_speed_10m { get; set; }
        public string wind_gusts_10m { get; set; }
        public string wind_direction_10m { get; set; }
        public string rain { get; set; }
        public string snowfall { get; set; }
        public string uv_index { get; set; }
        public string is_day { get; set; }
    }

    public class Current
    {
        public long? time { get; set; }
        public int? interval { get; set; }
        public float? temperature_2m { get; set; }
        public int? relative_humidity_2m { get; set; }
        public float? dew_point_2m { get; set; }
        public float? apparent_temperature { get; set; }
        public int? precipitation_probability { get; set; }
        public int? weather_code { get; set; }
        public float? pressure_msl { get; set; }
        public int? cloud_cover { get; set; }
        public float? visibility { get; set; }
        public float? wind_speed_10m { get; set; }
        public float? wind_gusts_10m { get; set; }
        public int? wind_direction_10m { get; set; }
        public float? rain { get; set; }
        public float? snowfall { get; set; }
        public float? uv_index { get; set; }
        public int? is_day { get; set; }
    }

    public class Minutely_15_Units
    {
        public string time { get; set; }
        public string rain { get; set; }
        public string snowfall { get; set; }
    }

    public class Minutely_15
    {
        public long[] time { get; set; }
        public float[] rain { get; set; }
        public float[] snowfall { get; set; }
    }

    public class Hourly_Units
    {
        public string time { get; set; }
        public string temperature_2m { get; set; }
        public string relative_humidity_2m { get; set; }
        public string dew_point_2m { get; set; }
        public string apparent_temperature { get; set; }
        public string precipitation_probability { get; set; }
        public string weather_code { get; set; }
        public string pressure_msl { get; set; }
        public string cloud_cover { get; set; }
        public string visibility { get; set; }
        public string wind_speed_10m { get; set; }
        public string wind_gusts_10m { get; set; }
        public string wind_direction_10m { get; set; }
        public string rain { get; set; }
        public string snowfall { get; set; }
        public string uv_index { get; set; }
        public string is_day { get; set; }
    }

    public class Hourly
    {
        public long[] time { get; set; }
        public float[] temperature_2m { get; set; }
        public int[] relative_humidity_2m { get; set; }
        public float[] dew_point_2m { get; set; }
        public float[] apparent_temperature { get; set; }
        public int[] precipitation_probability { get; set; }
        public int[] weather_code { get; set; }
        public float[] pressure_msl { get; set; }
        public int[] cloud_cover { get; set; }
        public float[] visibility { get; set; }
        public float[] wind_speed_10m { get; set; }
        public float[] wind_gusts_10m { get; set; }
        public int[] wind_direction_10m { get; set; }
        public float[] rain { get; set; }
        public float[] snowfall { get; set; }
        public float[] uv_index { get; set; }
        public int[] is_day { get; set; }
    }

    public class Daily_Units
    {
        public string time { get; set; }
        public string weather_code { get; set; }
        public string temperature_2m_max { get; set; }
        public string temperature_2m_min { get; set; }
        public string sunrise { get; set; }
        public string sunset { get; set; }
        public string uv_index_max { get; set; }
        public string precipitation_probability_max { get; set; }
        public string wind_speed_10m_max { get; set; }
        public string wind_gusts_10m_max { get; set; }
        public string wind_direction_10m_dominant { get; set; }
        public string cloud_cover_mean { get; set; }
        public string apparent_temperature_mean { get; set; }
        public string dew_point_2m_mean { get; set; }
        public string pressure_msl_mean { get; set; }
        public string relative_humidity_2m_mean { get; set; }
        public string visibility_mean { get; set; }
    }

    public class Daily
    {
        public long[] time { get; set; }
        public int[] weather_code { get; set; }
        public float[] temperature_2m_max { get; set; }
        public float[] temperature_2m_min { get; set; }
        public long[] sunrise { get; set; }
        public long[] sunset { get; set; }
        public float[] uv_index_max { get; set; }
        public int[] precipitation_probability_max { get; set; }
        public float[] wind_speed_10m_max { get; set; }
        public float[] wind_gusts_10m_max { get; set; }
        public int[] wind_direction_10m_dominant { get; set; }
        public int[] cloud_cover_mean { get; set; }
        public float[] apparent_temperature_mean { get; set; }
        public float[] dew_point_2m_mean { get; set; }
        public float[] pressure_msl_mean { get; set; }
        public int[] relative_humidity_2m_mean { get; set; }
        public float[] visibility_mean { get; set; }
    }
}
