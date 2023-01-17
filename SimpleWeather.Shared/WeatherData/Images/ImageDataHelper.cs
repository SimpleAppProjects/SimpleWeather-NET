using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData.Images
{
    public abstract class ImageDataHelperImpl
    {
        public abstract Task<ImageData> GetCachedImageData(String backgroundCode);

        public abstract Task<ImageData> GetRemoteImageData(String backgroundCode);

        public abstract Task<ImageData> CacheImage(ImageData imageData);

        protected abstract Task<ImageData> StoreImage(Uri imageUri, ImageData imageData);

        public abstract Task ClearCachedImageData();

        public abstract ImageData GetDefaultImageData(String backgroundCode);

        public abstract Task<bool> IsEmpty();
    }
}
