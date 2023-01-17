using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData.Images
{
    public sealed class ImageDataHelperDefault : ImageDataHelperImpl
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

        public override ImageData GetDefaultImageData(string backgroundCode)
        {
            // Fallback to assets
            // day, night, rain, snow
            var imageData = new ImageData();
            switch (backgroundCode)
            {
                case WeatherBackground.SNOW:
                case WeatherBackground.SNOW_WINDY:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/snow.jpg";
                    imageData.HexColor = "#ffb8d0f0";
                    break;
                case WeatherBackground.RAIN:
                case WeatherBackground.RAIN_NIGHT:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/rain.jpg";
                    imageData.HexColor = "#ff102030";
                    break;
                case WeatherBackground.TSTORMS_DAY:
                case WeatherBackground.TSTORMS_NIGHT:
                case WeatherBackground.STORMS:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/storms.jpg";
                    imageData.HexColor = "#ff182830";
                    break;
                case WeatherBackground.FOG:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/fog.jpg";
                    imageData.HexColor = "#ff808080";
                    break;
                case WeatherBackground.PARTLYCLOUDY_DAY:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/day-partlycloudy.jpg";
                    imageData.HexColor = "#ff88b0c8";
                    break;
                case WeatherBackground.PARTLYCLOUDY_NIGHT:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/night-partlycloudy.jpg";
                    imageData.HexColor = "#ff182020";
                    break;
                case WeatherBackground.MOSTLYCLOUDY_DAY:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/day-cloudy.jpg";
                    imageData.HexColor = "#ff88b0c8";
                    break;
                case WeatherBackground.MOSTLYCLOUDY_NIGHT:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/night-cloudy.jpg";
                    imageData.HexColor = "#ff182020";
                    break;
                case WeatherBackground.NIGHT:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/night.jpg";
                    imageData.HexColor = "#ff182020";
                    break;
                case WeatherBackground.DAY:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/day.jpg";
                    imageData.HexColor = "#ff88b0c8";
                    break;
                default:
                    return null;
            }

            return imageData;
        }

        public override Task<bool> IsEmpty()
        {
            return Task.FromResult(false);
        }
    }
}
