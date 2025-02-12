#if WINDOWS
using MapControl = Mapsui.UI.WinUI.MapControl;
using Border = Microsoft.UI.Xaml.Controls.Border;
#elif __IOS__
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Extras;
using SimpleWeather.Maui.Radar;
using SimpleWeather.Utils;
using MapControl = Microsoft.Maui.Controls.Maps.Map;
#else
using SimpleWeather.Maui.Radar;
using MapControl = Mapsui.UI.Maui.MapControl;
#endif

namespace SimpleWeather.NET.Radar
{
    public partial class MapTileRadarViewProvider
    {
        protected const double MIN_ZOOM_LEVEL = 2d;
        protected const double MAX_ZOOM_LEVEL = 18d;
        protected const double DEFAULT_ZOOM_LEVEL = 6d;

        private MapControl mapControl;
        private WeatherUtils.Coordinate locationCoords;
        protected RadarToolbar RadarMapContainer { get; private set; }

        protected readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

        protected bool IsViewAlive { get; private set; }

        public MapTileRadarViewProvider(Border container) : base(container)
        {
        }
        
        public override void UpdateCoordinates(WeatherUtils.Coordinate coordinates, bool updateView = false)
        {
            locationCoords = coordinates;
            if (updateView) UpdateRadarView();
        }
        
        private void RadarMapContainer_OnPlayAnimation(object sender, EventArgs e)
        {
            OnPlayRadarAnimation();
        }

        private void RadarMapContainer_OnPauseAnimation(object sender, EventArgs e)
        {
            OnPauseRadarAnimation();
        }

        protected virtual void OnPlayRadarAnimation() { }
        protected virtual void OnPauseRadarAnimation() { }

        public virtual partial void UpdateMap(MapControl mapControl);

        public override void OnDestroyView()
        {
            IsViewAlive = false;
#if WINDOWS
            RadarContainer.Child = null;
#else
            RadarContainer.Content = null;
#endif
            if (RadarMapContainer != null)
            {
                RadarMapContainer.OnPauseAnimation -= RadarMapContainer_OnPauseAnimation;
                RadarMapContainer.OnPlayAnimation -= RadarMapContainer_OnPlayAnimation;
                RadarMapContainer.MapContainerChild = null;
                RadarMapContainer = null;
            }

#if WINDOWS || !__IOS__
            mapControl?.Map?.Layers?.Clear();
            mapControl = null;
            markerLayer?.Dispose();
            markerLayer = null;
#else
            mapControl?.MapElements?.Clear();
            mapControl?.Pins?.Clear();
            mapControl = null;
            locationMarker = null;
#endif
        }
    }
}