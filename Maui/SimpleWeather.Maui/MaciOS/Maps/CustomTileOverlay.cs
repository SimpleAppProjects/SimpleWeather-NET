using MapKit;
using MapKitProxy;

#if __IOS__

namespace SimpleWeather.Maui.Maps;

public class CustomTileOverlay(string name, string urlTemplate, int cacheTimeSeconds) : CustomMKTileOverlay(name, urlTemplate, cacheTimeSeconds), ICustomTileOverlay
{
    private nfloat _alpha = 1f;

    public nfloat Alpha
    {
        get => _alpha;
        set
        {
            _alpha = value;
            AlphaChanged?.Invoke(this, value);
        }
    }
    
    public event EventHandler<nfloat> AlphaChanged;
}

public class CustomWmsTileOverlay(string name, string urlTemplate, int cacheTimeSeconds) : WmsMKTileOverlay(name, urlTemplate, cacheTimeSeconds), ICustomTileOverlay
{
    private nfloat _alpha = 1f;

    public nfloat Alpha
    {
        get => _alpha;
        set
        {
            _alpha = value;
            AlphaChanged?.Invoke(this, value);
        }
    }

    public event EventHandler<nfloat> AlphaChanged;
}

public interface ICustomTileOverlay : IMKOverlay
{
    nfloat Alpha { get; set; }
    event EventHandler<nfloat> AlphaChanged;
}
#endif