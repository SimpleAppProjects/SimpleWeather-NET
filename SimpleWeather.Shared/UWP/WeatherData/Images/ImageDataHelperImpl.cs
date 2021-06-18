#if WINDOWS_UWP
using SimpleWeather.Controls;
using SimpleWeather.Firebase;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.WeatherData.Images;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Web.Http;
using static SimpleWeather.Utils.APIRequestUtils;

namespace SimpleWeather.UWP.Shared.WeatherData.Images
{
    public sealed class ImageDataHelperImplUWP : ImageDataHelperImpl
    {
        // Shared Settings
        private ApplicationDataContainer ImageDataContainer;

        // App data files
        private StorageFolder CacheDataFolder;
        private StorageFolder ImageDataFolder;

        public ImageDataHelperImplUWP()
        {
            ImageDataContainer = ApplicationData.Current.LocalSettings.CreateContainer(
                "images", ApplicationDataCreateDisposition.Always);

            CacheDataFolder = ApplicationData.Current.LocalCacheFolder;
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

        private async Task CreateDataFolderIfNeeded()
        {
            if (ImageDataFolder == null)
            {
                ImageDataFolder = await CacheDataFolder.CreateFolderAsync(
                    "images", CreationCollisionOption.OpenIfExists);
            }
        }

        protected override async Task<ImageData> StoreImage(Uri imageUri, ImageData imageData)
        {
            // If download has failed recently; don't allow any downloads until specified timespan
            if ((DateTime.UtcNow - FirebaseStorageHelper.LastDownloadFailTimestamp).TotalHours >= 1)
            {
                StorageFile imageFile = null;

                try
                {
                    // Download image to local cache folder
                    await CreateDataFolderIfNeeded();

                    imageFile = await ImageDataFolder.CreateFileAsync(
                        String.Format("{0}-{1}", imageData.Condition, Guid.NewGuid().ToString()), CreationCollisionOption.ReplaceExisting);

                    var storage = await FirebaseStorageHelper.GetFirebaseStorage();
                    var storageRef = storage.GetReferenceFromUrl(imageUri);

                    var imageDownloadUri = new Uri(await storageRef.GetDownloadUrlAsync());

                    AnalyticsLogger.LogEvent("ImageDataHelperImplUWP: StoreImage",
                        new Dictionary<string, string>()
                        {
                                { "imageData", imageData.ToString() }
                        });

                    using (var fStream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                    using (var request = new HttpRequestMessage(HttpMethod.Get, imageDownloadUri))
                    {
                        request.Headers.CacheControl.MaxAge = TimeSpan.FromMinutes(15);

                        // Connect to webstream
                        var webClient = SimpleLibrary.GetInstance().WebClient;
                        using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                        {
                            response.EnsureSuccessStatusCode();

                            // Download content to file
                            await response.Content.WriteToStreamAsync(fStream);
                            await fStream.FlushAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "ImageDataHelper: error storing image data");
                    FirebaseStorageHelper.LastDownloadFailTimestamp = DateTime.UtcNow;

                    if (imageFile != null)
                    {
                        try
                        {
                            await imageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                        catch (Exception e1)
                        {
                            Logger.WriteLine(LoggerLevel.Error, e1);
                        }
                    }

                    return null;
                }

                ImageData newImageData = ImageData.CopyWithNewImageUrl(imageData, imageFile.Path);

                ImageDataContainer.Values[imageData.Condition] = await JSONParser.SerializerAsync(newImageData);

                return newImageData;
            }

            return null;
        }

        public override async Task ClearCachedImageData()
        {
            if (ImageDataFolder != null)
            {
                try
                {
                    await ImageDataFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e);
                }
            }

            ImageDataContainer.Values.Clear();
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
#endif