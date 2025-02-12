using MapKit;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps.Platform;

namespace SimpleWeather.Maui.Maps;

public class CustomMKMapView(IMapHandler handler) : MauiMKMapView(handler)
{
    protected override MKOverlayRenderer GetViewForOverlayDelegate(MKMapView mapview, IMKOverlay overlay)
    {
        return overlay switch
        {
            CustomTileOverlay customTileOverlay => new CustomTileOverlayRenderer(customTileOverlay),
            CustomWmsTileOverlay customWmsTileOverlay => new CustomTileOverlayRenderer(customWmsTileOverlay),
            MKTileOverlay tileOverlay => new MKTileOverlayRenderer(tileOverlay),
            _ => base.GetViewForOverlayDelegate(mapview, overlay)
        };
    }
}