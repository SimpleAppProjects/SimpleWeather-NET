using SimpleWeather.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Main
{
    internal class DetailsPageArgs
    {
        public WeatherNowViewModel WeatherNowView { get; set; }
        public bool IsHourly { get; set; }
        public int ScrollToPosition { get; set; }
    }
}