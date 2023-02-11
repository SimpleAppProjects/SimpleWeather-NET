#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace SimpleWeather.NET.Controls.Graphs
{
    public interface IIconCacheGraph
    {
        void ClearIconCache();
    }

    public interface IGraph
    {
        int GetItemPositionFromPoint(float xCoordinate);
#if WINDOWS
        FrameworkElement Control { get; }
        ScrollViewer ScrollViewer { get; }
#else
        VisualElement Control { get; }
        ScrollView ScrollViewer { get; }
#endif
    }
}
