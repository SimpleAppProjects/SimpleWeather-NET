using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData.Images
{
    public abstract class BaseImageDataService : IImageDataService
    {
        // Shared Settings
        protected readonly SettingsContainer ImageDataContainer = new("images");
        protected readonly SettingsContainer ImageDBSettingsContainer = new("imageDB");

        public abstract Task<ImageData> GetCachedImageData(string backgroundCode);

        public abstract Task<ImageData> GetRemoteImageData(string backgroundCode);

        public abstract Task<ImageData> CacheImage(ImageData imageData);

        protected abstract Task<ImageData> StoreImage(Uri imageUri, ImageData imageData);

        public abstract Task ClearCachedImageData();

        public virtual ImageData GetDefaultImageData(string backgroundCode)
        {
            return GetDefaultImageData(backgroundCode, null);
        }

        public virtual ImageData GetDefaultImageData(string backgroundCode, Weather weather)
        {
            // Fallback to assets
            // day, night, rain, snow
            var imageData = new ImageData();

#if WINDOWS
            var basePath = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/";
#else
            var basePath = "maui-appx://Images/Backgrounds/";
#endif

            switch (backgroundCode)
            {
                case WeatherBackground.SNOW:
                case WeatherBackground.SNOW_WINDY:
                    imageData.ImageUrl = $"{basePath}snow.jpg";
                    imageData.HexColor = "#ffb8d0f0";
                    break;
                case WeatherBackground.RAIN:
                case WeatherBackground.RAIN_NIGHT:
                    imageData.ImageUrl = $"{basePath}rain.jpg";
                    imageData.HexColor = "#ff102030";
                    break;
                case WeatherBackground.TSTORMS_DAY:
                case WeatherBackground.TSTORMS_NIGHT:
                case WeatherBackground.STORMS:
                    imageData.ImageUrl = $"{basePath}storms.jpg";
                    imageData.HexColor = "#ff182830";
                    break;
                case WeatherBackground.FOG:
                    imageData.ImageUrl = $"{basePath}fog.jpg";
                    imageData.HexColor = "#ff808080";
                    break;
                case WeatherBackground.PARTLYCLOUDY_DAY:
                    imageData.ImageUrl = $"{basePath}day_partlycloudy.jpg";
                    imageData.HexColor = "#ff88b0c8";
                    break;
                case WeatherBackground.PARTLYCLOUDY_NIGHT:
                    imageData.ImageUrl = $"{basePath}night_partlycloudy.jpg";
                    imageData.HexColor = "#ff182020";
                    break;
                case WeatherBackground.MOSTLYCLOUDY_DAY:
                    imageData.ImageUrl = $"{basePath}day_cloudy.jpg";
                    imageData.HexColor = "#ff88b0c8";
                    break;
                case WeatherBackground.MOSTLYCLOUDY_NIGHT:
                    imageData.ImageUrl = $"{basePath}night_cloudy.jpg";
                    imageData.HexColor = "#ff182020";
                    break;
                case WeatherBackground.NIGHT:
                    imageData.ImageUrl = $"{basePath}night.jpg";
                    imageData.HexColor = "#ff182020";
                    break;
                case WeatherBackground.DAY:
                    imageData.ImageUrl = $"{basePath}day.jpg";
                    imageData.HexColor = "#ff88b0c8";
                    break;
                default:
                    if (weather != null)
                    {
                        if (weather.IsNight())
                        {
                            imageData.ImageUrl = $"{basePath}night.jpg";
                            imageData.HexColor = "#ff182020";
                        }
                        else
                        {
                            imageData.ImageUrl = $"{basePath}day.jpg";
                            imageData.HexColor = "#ff88b0c8";
                        }
                    }
                    else
                    {
                        return null;
                    }
                    break;
            }

            return imageData;
        }

        public abstract Task<bool> IsEmpty();

        public long GetImageDBUpdateTime()
        {
            return ImageDBSettingsContainer.GetValue("ImageDB_LastUpdated", 0L);
        }

        public void SetImageDBUpdateTime(long value)
        {
            ImageDBSettingsContainer.SetValue("ImageDB_LastUpdated", value);
        }

        public bool ShouldInvalidateCache()
        {
            return ImageDBSettingsContainer.GetValue("ImageDB_Invalidate", false);
        }

        public void InvalidateCache(bool value)
        {
            ImageDBSettingsContainer.SetValue("ImageDB_Invalidate", value);
        }
    }
}
