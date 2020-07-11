using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Helpers
{
    public interface ICommandBarPage
    {
        String CommandBarLabel { get; set; }
        List<muxc.NavigationViewItemBase> PrimaryCommands { get; set; }
    }
}