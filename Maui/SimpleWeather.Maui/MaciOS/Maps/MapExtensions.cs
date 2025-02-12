using CoreLocation;
using MapKit;
using Map = Microsoft.Maui.Controls.Maps.Map;

#if __IOS__
namespace SimpleWeather.Maui.Maps;

public static class MapExtensions
{
    private const double MERCATOR_OFFSET = 268435456;
    private const double MERCATOR_RADIUS = 85445659.44705395; 
    
    public static void AddOverlay(this Map map, IMKOverlay overlay)
    {
        if (map.Handler is IPlatformViewHandler { PlatformView: MKMapView mapView })
        {
            mapView.AddOverlay(overlay, MKOverlayLevel.AboveLabels);
        }
    }
    
    public static void RemoveOverlay(this Map map, IMKOverlay overlay)
    {
        if (map.Handler is IPlatformViewHandler { PlatformView: MKMapView mapView })
        {
            mapView.RemoveOverlay(overlay);
        }
    }
    
    
    public static void SetCameraPosition(this Map map, CameraPosition position, bool animated = false)
    {
        if (map.Handler is IPlatformViewHandler { PlatformView: MKMapView mapView })
        {
            if (!mapView.Bounds.IsEmpty)
            {
                var centerCoord = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
                mapView.SetCenterCoordinate(centerCoord, position.ZoomLevel, animated);
            }
            else
            {
                void DidChangeVisibleRegion(object sender, EventArgs e)
                {
                    mapView.DidChangeVisibleRegion -= DidChangeVisibleRegion;
                    map.SetCameraPosition(position, animated);
                }

                mapView.DidChangeVisibleRegion += DidChangeVisibleRegion;
            }
        }
        else
        {
            void OnHandlerChanged(object sender, EventArgs e)
            {
                map.HandlerChanged -= OnHandlerChanged;
                map.SetCameraPosition(position, animated);
            }

            map.HandlerChanged += OnHandlerChanged;
        }
    }

    #region Zoom Level
    /**
     * Zoom Level methods
     *
     * Source:
     * https://stackoverflow.com/a/2121869
     * http://troybrant.net/blog/2010/01/set-the-zoom-level-of-an-mkmapview/
     * 
     */
    private static double LongitudeToPixelSpaceX(double longitude)
    {
        return double.Round(MERCATOR_OFFSET + MERCATOR_RADIUS * longitude * double.Pi / 180.0);
    }
    
    private static double LatitudeToPixelSpaceY(double latitude)
    {
        return double.Round(MERCATOR_OFFSET - MERCATOR_RADIUS * double.Log((1 + double.Sin(latitude * double.Pi / 180.0)) / (1 - double.Sin(latitude * double.Pi / 180.0))) / 2.0);
    }
    
    private static double PixelSpaceXToLongitude(double pixelX)
    {
        return ((double.Round(pixelX) - MERCATOR_OFFSET) / MERCATOR_RADIUS) * 180.0 / double.Pi;
    }
    
    private static double PixelSpaceYToLatitude(double pixelY)
    {
        return (double.Pi / 2.0 - 2.0 * double.Atan(double.Exp((double.Round(pixelY) - MERCATOR_OFFSET) / MERCATOR_RADIUS))) * 180.0 / double.Pi;
    }

    public static MKCoordinateSpan CoordinateSpan(this MKMapView mapView, CLLocationCoordinate2D centerCoordinate,
        nuint zoomLevel)
    {
        // convert center coordinate to pixel space
        double centerPixelX = LongitudeToPixelSpaceX(centerCoordinate.Longitude);
        double centerPixelY = LatitudeToPixelSpaceY(centerCoordinate.Latitude);
            
        // determine the scale value from the zoom level
        double zoomExponent = 20 - zoomLevel;
        double zoomScale = double.Pow(2, zoomExponent);
            
        // scale the map’s size in pixel space
        var mapSizeInPixels = mapView.Bounds.Size;
        double scaledMapWidth = mapSizeInPixels.Width * zoomScale;
        double scaledMapHeight = mapSizeInPixels.Height * zoomScale;
            
        // figure out the position of the top-left pixel
        double topLeftPixelX = centerPixelX - (scaledMapWidth / 2);
        double topLeftPixelY = centerPixelY - (scaledMapHeight / 2);
            
        // find delta between left and right longitudes
        double minLng = PixelSpaceXToLongitude(topLeftPixelX);
        double maxLng = PixelSpaceXToLongitude(topLeftPixelX + scaledMapWidth);
        double longitudeDelta = maxLng - minLng;
            
        // find delta between top and bottom latitudes
        double minLat = PixelSpaceYToLatitude(topLeftPixelY);
        double maxLat = PixelSpaceYToLatitude(topLeftPixelY + scaledMapHeight);
        double latitudeDelta = -1 * (maxLat - minLat);
        
        // create and return the lat/lng span
        MKCoordinateSpan span = new MKCoordinateSpan(latitudeDelta, longitudeDelta);
        return span;
    }

    public static void SetCenterCoordinate(this MKMapView mapView, CLLocationCoordinate2D centerCoordinate,
        nuint zoomLevel, bool animated = false)
    {
        // clamp large numbers to 28
        zoomLevel = nuint.Min(zoomLevel, 28);
        
        // use the zoom level to compute the region
        MKCoordinateSpan span = mapView.CoordinateSpan(centerCoordinate, zoomLevel);
        MKCoordinateRegion region = new MKCoordinateRegion(centerCoordinate, span);
        
        // set the region like normal
        mapView.SetRegion(region, animated);
    }
    #endregion
}

public record CameraPosition(double Latitude, double Longitude, nuint ZoomLevel);
#endif