using BruTile.Predefined;
using BruTile.Web;
using CacheCow.Client.Headers;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
#if WINDOWS
using Mapsui.UI.WinUI;
using Microsoft.UI.Xaml.Controls;
#else
using Mapsui.UI.Maui;
#endif
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using System.Net.Http.Headers;

namespace SimpleWeather.NET.Radar.OpenWeather
{
    public partial class OWMRadarViewProvider : MapTileRadarViewProvider
    {
        private HttpTileSource TileSource;

        public OWMRadarViewProvider(Border container) : base(container)
        {
        }

        public override void UpdateMap(MapControl mapControl)
        {
            if (TileSource == null)
            {
                TileSource = CreateTileSource();
                mapControl.Map.Layers.Add(new TileLayer(TileSource, dataFetchStrategy: new MinimalDataFetchStrategy(), renderFetchStrategy: new MinimalRenderFetchStrategy()));
            }
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            TileSource = null;
        }

        private static readonly BruTile.Attribution OWMAttribution = new("OpenWeatherMap", "https://openweathermap.org/");

        private HttpTileSource CreateTileSource()
        {
            return new HttpTileSource(new GlobalSphericalMercator(),
                "https://tile.openweathermap.org/map/precipitation_new/{z}/{x}/{y}.png?appid={k}",
                apiKey: APIKeys.GetOWMKey(), name: OWMAttribution.Text,
                tileFetcher: FetchTileAsync,
                attribution: OWMAttribution, userAgent: Constants.GetUserAgentString());
        }

        private async Task<byte[]> FetchTileAsync(Uri arg)
        {
            byte[] arr = null;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, arg);
                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromMinutes(15)
                };

                using var response = await WebClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

#if DEBUG
                var cacheHeader = response.Headers.GetCacheCowHeader();
                if (cacheHeader?.RetrievedFromCache == true)
                {
                    Logger.WriteLine(LoggerLevel.Debug, $"{nameof(OWMRadarViewProvider)}: tile fetched from cache");
                }
                else
                {
                    Logger.WriteLine(LoggerLevel.Debug, $"{nameof(OWMRadarViewProvider)}: tile fetched from web");
                }
#endif

                arr = await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }

            return arr;
        }
    }
}
