using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Helpers
{
    public interface ICommandBarPage
    {
        String CommandBarLabel { get; set; }
        List<ICommandBarElement> PrimaryCommands { get; set; }
    }
}
