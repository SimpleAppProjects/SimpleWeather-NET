#if __IOS__
using CoreFoundation;
using MapKit;

namespace SimpleWeather.Maui.Maps;

public class CustomTileOverlayRenderer : MKTileOverlayRenderer
{
    private readonly ICustomTileOverlay _overlay;

    public CustomTileOverlayRenderer(CustomTileOverlay overlay) : base(overlay)
    {
        this._overlay = overlay;
        overlay.AlphaChanged += Overlay_AlphaChanged;
    }
    
    public CustomTileOverlayRenderer(CustomWmsTileOverlay overlay) : base(overlay)
    {
        this._overlay = overlay;
        overlay.AlphaChanged += Overlay_AlphaChanged;
    }

    private void Overlay_AlphaChanged(object sender, nfloat alpha)
    {
        DispatchQueue.MainQueue.DispatchAsync(() =>
        {
            this.Alpha = alpha;
            SetNeedsDisplay();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        try
        {
            _overlay.AlphaChanged -= Overlay_AlphaChanged;
            _overlay.Dispose();
        }
        catch
        {
            // ignored
        }
    }
}
#endif