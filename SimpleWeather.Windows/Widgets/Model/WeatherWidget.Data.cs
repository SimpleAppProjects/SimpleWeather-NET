namespace SimpleWeather.NET.Widgets.Model
{
    public class WeatherWidgetData
    {
        public Current current { get; set; }
        public Forecast[] forecast { get; set; } = Array.Empty<Forecast>();
        public bool showForecast { get; set; }
        public HourlyForecast[] hr_forecast { get; set; } = Array.Empty<HourlyForecast>();
        public bool showHrForecast { get; set; }
        public WeatherIcon chanceIcon { get; set; }
    }

    public class Current
    {
        public WeatherIcon weatherIcon { get; set; }
        public string temp { get; set; }
        public string feelsLike { get; set; }
        public string windSpeed { get; set; }
        public string chance { get; set; }
        public string location { get; set; }
        public string background { get; set; }
    }

    public class Forecast
    {
        public string date { get; set; }
        public WeatherIcon weatherIcon { get; set; }
        public string hi { get; set; }
        public string lo { get; set; }
    }

    public class HourlyForecast
    {
        public string date { get; set; }
        public WeatherIcon weatherIcon { get; set; }
        public string hi { get; set; }
        public string chance { get; set; }
    }

    public class WeatherIcon
    {
        public string light { get; set; }
        public string dark { get; set; }
    }
}
