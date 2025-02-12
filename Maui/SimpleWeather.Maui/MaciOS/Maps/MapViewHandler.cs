using System.Collections.Concurrent;
using MapKit;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps.Platform;

namespace SimpleWeather.Maui.Maps;

public class MapViewHandler : MapHandler
{
    protected override MauiMKMapView CreatePlatformView()
    {
        return MapPool.Get() as MauiMKMapView ?? new CustomMKMapView(this);
    }

    protected override void DisconnectHandler(MauiMKMapView platformView)
    {
        // This handler is done with the MKMapView; we can put it in the pool
        // for other renderers to use in the future
        MapPool.Add(platformView);
    }
}

internal class MapPool
{
    static MapPool s_instance;
    public static MapPool Instance => s_instance ?? (s_instance = new MapPool());

    internal readonly ConcurrentQueue<MKMapView> Maps = new ConcurrentQueue<MKMapView>();

    public static void Add(MKMapView mapView)
    {
        Instance.Maps.Enqueue(mapView);
    }

    public static MKMapView Get()
    {
        MKMapView mapView;
        return Instance.Maps.TryDequeue(out mapView) ? mapView : null;
    }
}