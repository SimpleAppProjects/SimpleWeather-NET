namespace SimpleWeather.NET.Widgets.Model
{
    public class WeatherWidgetCustomizeData
    {
        public string label { get; set; }
        public string selectedLocation { get; set; }
        public LocationChoice[] locations { get; set; } = Array.Empty<LocationChoice>();
    }

    public class LocationChoice
    {
        public string title { get; set; }
        public string value { get; set; }
    }

}
