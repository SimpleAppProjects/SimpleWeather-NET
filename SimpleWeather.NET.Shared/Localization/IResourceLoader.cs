using System;

namespace SimpleWeather.NET.Localization
{
    public interface IResourceLoader
    {
        string GetString(string key);
        string GetStringForUri(Uri uri);
    }
}
