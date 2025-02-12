#if WINDOWS
using Border = Microsoft.UI.Xaml.Controls.Border;
using MapControl = Mapsui.UI.WinUI.MapControl;
#else
using MapControl = Microsoft.Maui.Controls.Maps.Map;
#endif

namespace SimpleWeather.NET.Radar
{
    public partial class EmptyRadarViewProvider(Border container) : MapTileRadarViewProvider(container)
    {
        public override void UpdateRadarView()
        {
            base.UpdateRadarView();
        }

        public override void UpdateMap(MapControl mapControl)
        {
            base.UpdateMap(mapControl);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }
    }
}
