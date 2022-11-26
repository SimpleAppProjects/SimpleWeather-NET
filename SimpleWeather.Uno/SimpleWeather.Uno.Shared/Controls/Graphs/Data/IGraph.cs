using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Controls.Graphs
{
    public interface IIconCacheGraph
    {
        void ClearIconCache();
    }

    public interface IGraph
    {
        int GetItemPositionFromPoint(float xCoordinate);
        FrameworkElement Control { get; }
        ScrollViewer ScrollViewer { get; }
    }
}
