using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.NET.Helpers
{
    public interface ICommandBarPage
    {
        String CommandBarLabel { get; set; }
        List<ICommandBarElement> PrimaryCommands { get; set; }
    }
}