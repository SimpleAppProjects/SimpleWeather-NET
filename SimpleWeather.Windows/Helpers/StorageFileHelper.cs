using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.NET.Helpers
{
    public static class StorageFileHelper
    {
        public static Task<Stream> GetFileStreamFromApplicationUri(string uri)
        {
            return GetFileStreamFromApplicationUri(new Uri(uri));
        }

        public static async Task<Stream> GetFileStreamFromApplicationUri(Uri uri)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

            return (await file.OpenReadAsync()).AsStreamForRead();
        }
    }
}
