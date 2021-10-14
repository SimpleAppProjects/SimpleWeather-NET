using Newtonsoft.Json;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace SimpleWeather.WeatherData.Images
{
    public sealed class ImageDataHelperRes : ImageDataHelperImpl
    {
        // Shared Settings
        private ApplicationDataContainer ImageDataContainer;

        public ImageDataHelperRes()
        {
            ImageDataContainer = ApplicationData.Current.LocalSettings.CreateContainer(
                "images", ApplicationDataCreateDisposition.Always);
        }

        public override Task<ImageData> GetCachedImageData(string backgroundCode)
        {
            if (ImageDataContainer.Values.ContainsKey(backgroundCode))
            {
                return JSONParser.DeserializerAsync<ImageData>(
                    ImageDataContainer.Values[backgroundCode]?.ToString());
            }

            return Task.FromResult<ImageData>(null);
        }

        public override async Task<ImageData> GetRemoteImageData(string backgroundCode)
        {
            var ResLoader = SimpleLibrary.GetInstance().ResLoader;
            var data = ResLoader.GetString("/SimpleWeather.Shared/Backgrounds/data");
            var imageRepo = await Task.Run(() => JsonConvert.DeserializeObject<Dictionary<string, ImageData>>(data));

            return imageRepo.FirstOrDefault(i => i.Value?.Condition == backgroundCode).Value;
        }

        public override Task<ImageData> CacheImage(ImageData imageData)
        {
            return StoreImage(new Uri(imageData.ImageUrl), imageData);
        }

        protected override async Task<ImageData> StoreImage(Uri imageUri, ImageData imageData)
        {
            if (imageData?.IsValid() == true)
            {
                ImageDataContainer.Values[imageData.Condition] = await JSONParser.SerializerAsync(imageData);
                return imageData;
            }

            return null;
        }

        public override Task ClearCachedImageData()
        {
            ImageDataContainer.Values.Clear();
            return Task.CompletedTask;
        }

        public override ImageData GetDefaultImageData(string backgroundCode, Weather weather)
        {
            var wm = WeatherManager.GetInstance();

            // Fallback to assets
            // day, night, rain, snow
            var imageData = new ImageData();
            switch (backgroundCode)
            {
                case WeatherBackground.SNOW:
                case WeatherBackground.SNOW_WINDY:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/snow.jpg";
                    imageData.HexColor = "#ffb8d0f0";
                    break;
                case WeatherBackground.RAIN:
                case WeatherBackground.RAIN_NIGHT:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/rain.jpg";
                    imageData.HexColor = "#ff102030";
                    break;
                case WeatherBackground.TSTORMS_DAY:
                case WeatherBackground.TSTORMS_NIGHT:
                case WeatherBackground.STORMS:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/storms.jpg";
                    imageData.HexColor = "#ff182830";
                    break;
                case WeatherBackground.FOG:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/fog.jpg";
                    imageData.HexColor = "#ff808080";
                    break;
                case WeatherBackground.PARTLYCLOUDY_DAY:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/day-partlycloudy.jpg";
                    imageData.HexColor = "#ff88b0c8";
                    break;
                case WeatherBackground.PARTLYCLOUDY_NIGHT:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/night-partlycloudy.jpg";
                    imageData.HexColor = "#ff182020";
                    break;
                case WeatherBackground.MOSTLYCLOUDY_DAY:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/day-cloudy.jpg";
                    imageData.HexColor = "#ff88b0c8";
                    break;
                case WeatherBackground.MOSTLYCLOUDY_NIGHT:
                    imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/night-cloudy.jpg";
                    imageData.HexColor = "#ff182020";
                    break;
                default:
                    if (wm.IsNight(weather))
                    {
                        imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/night.jpg";
                        imageData.HexColor = "#ff182020";
                    }
                    else
                    {
                        imageData.ImageUrl = "ms-appx:///SimpleWeather.Shared/Assets/Backgrounds/day.jpg";
                        imageData.HexColor = "#ff88b0c8";
                    }
                    break;
            }

            return imageData;
        }

        public override Task<bool> IsEmpty()
        {
            return Task.FromResult(ImageDataContainer.Values.Keys.Count == 0);
        }
    }
}
