using System.Threading.Tasks;
using SimpleWeather.WeatherData.Images.Model;

namespace SimpleWeather.WeatherData.Images
{
    public interface IImageDataService
    {
        Task<ImageData> GetCachedImageData(string backgroundCode);
        Task<ImageData> GetRemoteImageData(string backgroundCode);
        Task<ImageData> CacheImage(ImageData imageData);
        Task ClearCachedImageData();
        ImageData GetDefaultImageData(string backgroundCode);
        ImageData GetDefaultImageData(string backgroundCode, Weather weather);
        Task<bool> IsEmpty();

        long GetImageDBUpdateTime();
        void SetImageDBUpdateTime(long value);
        bool ShouldInvalidateCache();
        void InvalidateCache(bool value);
    }
}