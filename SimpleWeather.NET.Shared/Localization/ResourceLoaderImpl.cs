using System;

namespace SimpleWeather.NET.Localization
{
    public class ResourceLoaderImpl : IResourceLoader
    {
        private readonly CustomStringLocalizer localizer;

        public ResourceLoaderImpl(CustomStringLocalizer localizer)
        {
            this.localizer = localizer;
        }

        public string GetString(string key)
        {
            return localizer[key];
        }

        public string GetStringForUri(Uri uri)
        {
            return localizer[uri.ToString()];
        }
    }
}
