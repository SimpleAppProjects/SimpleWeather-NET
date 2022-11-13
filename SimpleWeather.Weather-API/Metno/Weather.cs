using System;

namespace SimpleWeather.Weather_API.Metno
{
    public class Rootobject
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public float[] coordinates { get; set; }
    }

    public class Properties
    {
        public Meta meta { get; set; }
        public Timesery[] timeseries { get; set; }
    }

    public class Meta
    {
        public DateTime updated_at { get; set; }
        public Units units { get; set; }
    }

    public class Units
    {
        public string air_pressure_at_sea_level { get; set; }
        public string air_temperature { get; set; }
        public string air_temperature_max { get; set; }
        public string air_temperature_min { get; set; }
        public string cloud_area_fraction { get; set; }
        public string cloud_area_fraction_high { get; set; }
        public string cloud_area_fraction_low { get; set; }
        public string cloud_area_fraction_medium { get; set; }
        public string dew_point_temperature { get; set; }
        public string fog_area_fraction { get; set; }
        public string precipitation_amount { get; set; }
        public string relative_humidity { get; set; }
        public string ultraviolet_index_clear_sky { get; set; }
        public string wind_from_direction { get; set; }
        public string wind_speed { get; set; }
    }

    public class Timesery
    {
        public DateTime time { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Instant instant { get; set; }
        public Next_12_Hours next_12_hours { get; set; }
        public Next_1_Hours next_1_hours { get; set; }
        public Next_6_Hours next_6_hours { get; set; }
    }

    public class Instant
    {
        public Details details { get; set; }
    }

    public class Details
    {
        public float? air_pressure_at_sea_level { get; set; }
        public float? air_temperature { get; set; }
        public float? cloud_area_fraction { get; set; }
        public float? cloud_area_fraction_high { get; set; }
        public float? cloud_area_fraction_low { get; set; }
        public float? cloud_area_fraction_medium { get; set; }
        public float? dew_point_temperature { get; set; }
        public float? fog_area_fraction { get; set; }
        public float? probability_of_precipitation { get; set; }
        public float? relative_humidity { get; set; }
        public float? ultraviolet_index_clear_sky { get; set; }
        public float? wind_from_direction { get; set; }
        public float? wind_speed { get; set; }
        public float? wind_speed_of_gust { get; set; }
    }

    public class Next_12_Hours
    {
        public Summary summary { get; set; }
        public Details1 details { get; set; }
    }

    public class Summary
    {
        public string symbol_code { get; set; }
    }

    public class Details1
    {
        public float? probability_of_precipitation { get; set; }
    }

    public class Next_1_Hours
    {
        public Summary1 summary { get; set; }
        public Details2 details { get; set; }
    }

    public class Summary1
    {
        public string symbol_code { get; set; }
    }

    public class Details2
    {
        public float? precipitation_amount { get; set; }
        public float? precipitation_amount_max { get; set; }
        public float? precipitation_amount_min { get; set; }
        public float? probability_of_precipitation { get; set; }
        public float? probability_of_thunder { get; set; }
    }

    public class Next_6_Hours
    {
        public Summary2 summary { get; set; }
        public Details3 details { get; set; }
    }

    public class Summary2
    {
        public string symbol_code { get; set; }
    }

    public class Details3
    {
        public float? air_temperature_max { get; set; }
        public float? air_temperature_min { get; set; }
        public float? precipitation_amount { get; set; }
        public float? precipitation_amount_max { get; set; }
        public float? precipitation_amount_min { get; set; }
        public float? probability_of_precipitation { get; set; }
    }
}