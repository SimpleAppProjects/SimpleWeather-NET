using System;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace SimpleWeather.Maui.Utils
{
	public static class PageUtils
	{
		public static async Task RefreshPage(this Page page)
		{
			var navigation = page.Navigation;

			// Create a new instance of page
            var instance = ActivatorUtilities.CreateInstance(Ioc.Default, page.GetType()) as Page;
			// Push page to stack
            await navigation.PushAsync(instance);

			// Remove old instance
			navigation.RemovePage(page);
        }
	}
}

