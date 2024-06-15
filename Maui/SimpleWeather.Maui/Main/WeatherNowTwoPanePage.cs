#if false
using System;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Foldable;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Maui.Controls;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Main
{
	public class WeatherNowTwoPanePage : SplitViewPage
    {
        public WeatherNowTwoPanePage()
        {
            Title = ResStrings.label_nav_weathernow;

            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                Pane1 = new WeatherNow();

                Pane2 = new WeatherDetailsPage();
            }
            else
            {
                Pane1 = new WeatherNow();
            }
        }
    }
}
#endif