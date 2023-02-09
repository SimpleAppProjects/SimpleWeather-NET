using System;

namespace SimpleWeather.Maui.Main
{
	internal class WeatherPageArgs
	{
		public LocationData.LocationData Location { get; set; }
	}

	internal class DetailsPageArgs : WeatherPageArgs
	{
		public bool IsHourly { get; set; }
		public int ScrollToPosition { get; set; }
	}

	internal class WeatherNowArgs : WeatherPageArgs
	{
		public bool IsHome { get; set; }
	}
}

