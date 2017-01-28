namespace SimpleWeather.Controls
{
    public class ForecastItemView
    {
        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string LoTemp { get; set; }

        public ForecastItemView(WeatherYahoo.Forecast forecast)
        {
            WeatherIcon = WeatherUtils.GetWeatherIcon(int.Parse(forecast.code));
            Date = forecast.date;
            Condition = forecast.text;
            HiTemp = forecast.high + "º ";
            LoTemp = forecast.low + "º";
        }

        public ForecastItemView(WeatherUnderground.Forecastday1 forecast)
        {
            WeatherIcon = WeatherUtils.GetWeatherIcon(forecast.icon_url);
            Date = string.Format("{0} {1}", forecast.date.weekday, forecast.date.day);
            Condition = forecast.conditions;
            HiTemp = (Settings.Unit == "F" ? 
                forecast.high.fahrenheit : forecast.high.celsius) + "º ";
            LoTemp = (Settings.Unit == "F" ?
                forecast.low.fahrenheit : forecast.low.celsius) + "º ";
        }
    }
}
