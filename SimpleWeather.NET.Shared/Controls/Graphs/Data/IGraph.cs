#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScrollView = Microsoft.UI.Xaml.Controls.ScrollView;
using Color = Windows.UI.Color;
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
    }

    public interface IGraphPanel : IGraph
    {
        public double GraphMaxWidth { get; set; }
        public bool FillParentWidth { set; }
        public Color BottomTextColor { get; set; }
        public double BottomTextSize { get; set; }
        public float IconSize { get; set; }
        public bool DrawIconLabels { set; }
        public bool DrawDataLabels { set; }
        public bool ScrollingEnabled { get; set; }
        public void RequestGraphLayout();
    }
}
