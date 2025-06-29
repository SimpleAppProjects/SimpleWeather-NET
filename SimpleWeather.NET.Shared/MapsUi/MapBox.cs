#if !__IOS__
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Rendering.Skia;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui.UI.WinUI;
using Mapsui.Widgets;
using Mapsui.Widgets.ButtonWidgets;
using SimpleWeather.Helpers;
using SimpleWeather.Preferences;
using SimpleWeather.Weather_API.Keys;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Windows.Storage;
#if WINDOWS
using MapControl = Mapsui.UI.WinUI.MapControl;
#else
using MapControl = Mapsui.UI.Maui.MapControl;
#endif

namespace SimpleWeather.NET.MapsUi
{
    public static class MapBox
    {
        public static TileLayer CreateMapBoxLayer(bool isDarkMode = false)
        {
            return new TileLayer(GetMapBoxTileSource(isDarkMode))
            {
                Name = "Root"
            };
        }

        private static HttpTileSource GetMapBoxTileSource(bool isDarkMode = false)
        {
            string tileUrlTemplate;

            if (!string.IsNullOrWhiteSpace(MapBoxConfig.GetMapBoxMapStyle()))
            {
                tileUrlTemplate = $"https://api.mapbox.com/styles/v1/{MapBoxConfig.GetMapBoxMapStyle()}/tiles/256/{{z}}/{{x}}/{{y}}?access_token={{k}}";
            }
            else
            {
                tileUrlTemplate = $"https://api.mapbox.com/v4/mapbox.satellite/{{z}}/{{x}}/{{y}}.jpg90?access_token={{k}}";
            }

            return new CustomHttpTileSource(
                new GlobalSphericalMercator(name: "MapBox", yAxis: YAxis.OSM, minZoomLevel: 2, maxZoomLevel: 18, format: "jpeg"),
                urlBuilder: new BasicUrlBuilder(tileUrlTemplate, apiKey: MapBoxConfig.GetMapBoxKey()),
                name: "MapBox",
                attribution: null,
                persistentCache: new FileCache(
                            Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), Constants.TILE_CACHE_DIR, "MapBox"), "tile.jpeg"),
                configureHttpRequestMessage: request =>
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(30)
                    };
                }
            );
        }

        public static async Task CreateLayersAndWidgets(MapControl mapControl)
        {
            mapControl?.Map?.Layers?.Insert(1, new Layer("Root0")
            {
                Attribution = new Mapsui.Widgets.ButtonWidgets.HyperlinkWidget()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Url = "https://www.mapbox.com/about/maps/",
                    Text = "© Mapbox |",
                    BackColor = Color.Transparent,
                    TextColor = Color.LightSkyBlue
                }
            });
            mapControl?.Map?.Layers?.Insert(1, new Layer("Root1")
            {
                Attribution = new Mapsui.Widgets.ButtonWidgets.HyperlinkWidget()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Url = "https://www.openstreetmap.org/about/",
                    Text = "© OpenStreetMap |",
                    BackColor = Color.Transparent,
                    TextColor = Color.LightSkyBlue
                }
            });
            mapControl?.Map?.Layers?.Insert(1, new Layer("Root2")
            {
                Attribution = new Mapsui.Widgets.ButtonWidgets.HyperlinkWidget()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Url = "https://www.mapbox.com/contribute/",
                    Text = "Improve this map |",
                    BackColor = Color.Transparent,
                    TextColor = Color.LightSkyBlue
                }
            });
            mapControl?.Map?.Layers?.Insert(1, new Layer("Root3")
            {
                Attribution = new Mapsui.Widgets.ButtonWidgets.HyperlinkWidget()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Url = "https://www.maxar.com/",
                    Text = "Maxar",
                    BackColor = Color.Transparent,
                    TextColor = Color.LightSkyBlue
                }
            });

#if WINDOWS
            var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SimpleWeather.Shared/Resources/Images/Providers/mapbox_logo_white.svg"));
            var path = new Uri(logoFile.Path).ToString();
            mapControl?.Map?.Widgets?.Add(new Mapsui.Widgets.ButtonWidgets.ImageButtonWidget()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Mapsui.MRect(5),
                Image = path,
                Width = 100,
                Height = 22.5,
            });

            MapRenderer.RegisterWidgetRenderer(typeof(ImageButtonWidget), new ImageButtonWidgetRenderer());
#endif
        }
    }
}
#endif