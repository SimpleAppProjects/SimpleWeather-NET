using System;
namespace SimpleWeather.Maui.Helpers
{
	public interface IBackRequestedPage
	{
		Task<bool> OnBackRequested();
	}
}

