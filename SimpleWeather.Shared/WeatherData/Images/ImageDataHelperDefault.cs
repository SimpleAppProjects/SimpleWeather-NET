using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData.Images
{
    public sealed class ImageDataHelperDefault : BaseImageDataService
    {
        public override Task<ImageData> GetCachedImageData(string backgroundCode)
        {
            return Task.FromResult(GetDefaultImageData(backgroundCode));
        }

        public override Task<ImageData> GetRemoteImageData(string backgroundCode)
        {
            return Task.FromResult(GetDefaultImageData(backgroundCode));
        }

        public override Task<ImageData> CacheImage(ImageData imageData)
        {
            return Task.FromResult(imageData);
        }

        protected override Task<ImageData> StoreImage(Uri imageUri, ImageData imageData)
        {
            return Task.FromResult(imageData);
        }

        public override Task ClearCachedImageData()
        {
            return Task.CompletedTask;
        }

        public override Task<bool> IsEmpty()
        {
            return Task.FromResult(false);
        }
    }
}
