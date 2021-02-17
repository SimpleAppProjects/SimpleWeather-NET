using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.AQICN
{
    public class Rootobject
    {
        public string status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public int aqi { get; set; }
        /*
        public float idx { get; set; }
        public Attribution[] attributions { get; set; }
        public City city { get; set; }
        public string dominentpol { get; set; }
        public Iaqi iaqi { get; set; }
        public Time time { get; set; }
        */
        public Forecast forecast { get; set; }
        /*
        public Debug debug { get; set; }
        */
    }

    /*
    public class City
    {
        public float[] geo { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Iaqi
    {
        public Co co { get; set; }
        public H h { get; set; }
        public No2 no2 { get; set; }
        public O3 o3 { get; set; }
        public P p { get; set; }
        public Pm25 pm25 { get; set; }
        public T t { get; set; }
        public W w { get; set; }
        public Wg wg { get; set; }
    }

    public class Co
    {
        public float v { get; set; }
    }

    public class H
    {
        public float v { get; set; }
    }

    public class No2
    {
        public float v { get; set; }
    }

    public class O3
    {
        public float v { get; set; }
    }

    public class P
    {
        public float v { get; set; }
    }

    public class Pm25
    {
        public float v { get; set; }
    }

    public class T
    {
        public float v { get; set; }
    }

    public class W
    {
        public float v { get; set; }
    }

    public class Wg
    {
        public float v { get; set; }
    }

    public class Time
    {
        public string s { get; set; }
        public string tz { get; set; }
        public long v { get; set; }
        public DateTime iso { get; set; }
    }
    */

    public class Forecast
    {
        public Daily daily { get; set; }
    }

    public class Daily
    {
        /*
        public O31[] o3 { get; set; }
        public Pm10[] pm10 { get; set; }
        public Pm251[] pm25 { get; set; }
        */
        public Uvi[] uvi { get; set; }
    }

    /*
    public class O31
    {
        public int avg { get; set; }
        public string day { get; set; }
        public int max { get; set; }
        public int min { get; set; }
    }

    public class Pm10
    {
        public int avg { get; set; }
        public string day { get; set; }
        public int max { get; set; }
        public int min { get; set; }
    }

    public class Pm251
    {
        public int avg { get; set; }
        public string day { get; set; }
        public int max { get; set; }
        public int min { get; set; }
    }
    */

    public class Uvi
    {
        public float avg { get; set; }
        public string day { get; set; }
        public float max { get; set; }
        public float min { get; set; }
    }

    /*
    public class Debug
    {
        public DateTime sync { get; set; }
    }

    public class Attribution
    {
        public string url { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
    }
    */
}
