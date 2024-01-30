using System.Text.Json;

namespace SimpleWeather.NET.Json
{
    public static class JsonContextExtensions
    {
        public static JsonSerializerOptions AddJsonContexts(this JsonSerializerOptions options)
        {
            options.TypeInfoResolverChain.Add(NET.Radar.RainViewer.RainViewerJsonContext.Default);

            return options;
        }
    }
}
