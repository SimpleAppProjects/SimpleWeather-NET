#if WINDOWS
using Microsoft.UI.Xaml.Controls;
using MapControl = Mapsui.UI.WinUI.MapControl;
#else
using Mapsui.UI.Maui;
using MapControl = Mapsui.UI.Maui.MapControl;
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
