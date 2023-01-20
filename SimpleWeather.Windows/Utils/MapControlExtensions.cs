using Mapsui.UI.WinUI;

namespace SimpleWeather.NET.Utils
{
    public static class MapControlExtensions
    {
        public static void NavigateHome(this MapControl mapControl)
        {
            mapControl?.Map?.Home?.Invoke(mapControl?.Navigator);
        }
    }
}
