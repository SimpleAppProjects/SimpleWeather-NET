namespace SimpleWeather.Weather_API.Google
{
    public class PollenRootobject
    {
        public Dailyinfo[] dailyInfo { get; set; }
    }

    public class Dailyinfo
    {
        public Date date { get; set; }
        public Pollentypeinfo[] pollenTypeInfo { get; set; }
        public Plantinfo[] plantInfo { get; set; }
    }

    public class Date
    {
        public int? year { get; set; }
        public int? month { get; set; }
        public int? day { get; set; }
    }

    public class Pollentypeinfo
    {
        public string code { get; set; }
        public string displayName { get; set; }
        public bool? inSeason { get; set; }
        public Indexinfo indexInfo { get; set; }
        public string[] healthRecommendations { get; set; }
    }

    public class Indexinfo
    {
        public string code { get; set; }
        public string displayName { get; set; }
        public int? value { get; set; }
        public string category { get; set; }
        public string indexDescription { get; set; }
        public Color color { get; set; }
    }

    public class Color
    {
        public object red { get; set; }
        public object green { get; set; }
        public object blue { get; set; }
    }

    public class Plantinfo
    {
        public string code { get; set; }
        public string displayName { get; set; }
        public bool? inSeason { get; set; }
        public Indexinfo indexInfo { get; set; }
    }
}
