using Newtonsoft.Json;
using SimpleWeather.Firebase;
using SimpleWeather.Helpers;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.WeatherData.Images;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SimpleWeather.Maui.Images;

public sealed class ImageDataServiceImpl : BaseImageDataService
{
    private readonly string ImageDataFolder;
    private readonly string CacheDataFolder;

    public ImageDataServiceImpl() : base()
    {
        CacheDataFolder = ApplicationDataHelper.GetLocalCacheFolderPath();
        ImageDataFolder = Path.Combine(CacheDataFolder, "images");
        Directory.CreateDirectory(ImageDataFolder);
    }

    public override Task<ImageData> GetCachedImageData(string backgroundCode)
    {
        if (ImageDataContainer.ContainsKey(backgroundCode))
        {
            return JSONParser.DeserializerAsync<ImageData>(
                ImageDataContainer.GetValue<string>(backgroundCode)?.ToString());
        }

        return Task.FromResult<ImageData>(null);
    }

    public override async Task<ImageData> GetRemoteImageData(string backgroundCode)
    {
        ImageData imageData;

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            imageData = await ImageDatabase.GetRandomImageForCondition(backgroundCode);
        }
        else
        {
            Logger.WriteLine(LoggerLevel.Info, "Network not connected!!");
            return null;
        }

        if (imageData != null && await imageData.IsValidAsync())
        {
            return await CacheImage(imageData);
        }

        return null;
    }

    public override async Task<ImageData> CacheImage(ImageData imageData)
    {
        var imageUri = new Uri(imageData.ImageUrl);

        if (Equals("gs", imageUri.Scheme) || Equals("https", imageUri.Scheme) || Equals("http", imageUri.Scheme))
        {
            // Download image to storage
            // and image metadata to settings
            return await StoreImage(new Uri(imageData.ImageUrl), imageData);
        }
        else
        {
            return null;
        }
    }

    protected override async Task<ImageData> StoreImage(Uri imageUri, ImageData imageData)
    {
        if (!Directory.Exists(ImageDataFolder))
        {
            Directory.CreateDirectory(ImageDataFolder);
        }

        var imageFile = Path.Combine(ImageDataFolder, $"{imageData.Condition}-{Guid.NewGuid()}");

        try
        {
            AnalyticsLogger.LogEvent("ImageDataService: storeImage", new Dictionary<string, string>()
            {
                { "imageData", imageData.ToString() }
            });

            if (Equals(imageUri.Scheme, "gs") || imageUri.ToString().Contains("firebasestorage"))
            {
                var storage = await FirebaseHelper.GetFirebaseStorage();
                var storageRef = storage.GetReferenceFromUrl(imageUri.ToString());

                using var cts = new CancellationTokenSource(SettingsManager.CONNECTION_TIMEOUT);

                try
                {
                    await storageRef.WriteToFileAsync(imageFile).WaitAsync(cts.Token);
                }
                catch (OperationCanceledException) { }
            }
            else
            {
                var client = SharedModule.Instance.WebClient;
                var request = new HttpRequestMessage(HttpMethod.Get, imageData.ImageUrl);
                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromDays(1)
                };
                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    throw await response.CreateException();
                }
                else
                {
                    using var stream = await response.Content.ReadAsStreamAsync();

                    var fs = File.OpenWrite(imageFile);
                    fs.SetLength(0);
                    await stream.CopyToAsync(fs);
                    await fs.FlushAsync();
                }
            }
        }
        catch (Exception e)
        {
            Logger.WriteLine(LoggerLevel.Error, e, "ImageDataService: Error retrieving download url");

            if (File.Exists(imageFile))
            {
                File.Delete(imageFile);
            }

            return null;
        }

#if IOS
        var rootDataFolder = ApplicationDataHelper.GetRootDataFolderPath();
        var newImageData = ImageData.CopyWithNewImageUrl(imageData, imageFile.ReplaceFirst(rootDataFolder, "ios://"));
#else
        var newImageData = ImageData.CopyWithNewImageUrl(imageData, imageFile);
#endif

#if WINUI
        if (!newImageData.IsValid())
#else
        if (!await newImageData.IsValidAsync())
#endif
        {
            if (File.Exists(imageFile))
            {
                File.Delete(imageFile);
            }
            return null;
        }

        ImageDataContainer.SetValue(imageData.Condition, await JSONParser.SerializerAsync(newImageData));

        return newImageData;
    }

    public override Task ClearCachedImageData()
    {
        if (Directory.Exists(ImageDataFolder))
        {
            Directory.Delete(ImageDataFolder, true);
        }

        ImageDataContainer.Clear();
        return Task.CompletedTask;
    }

    public override ImageData GetDefaultImageData(string backgroundCode)
    {
        return ImageDataUtils.GetDefaultImageData(backgroundCode, null);
    }

    public override ImageData GetDefaultImageData(string backgroundCode, Weather weather)
    {
        return ImageDataUtils.GetDefaultImageData(backgroundCode, weather);
    }

    public override Task<bool> IsEmpty()
    {
        return Task.FromResult(ImageDataContainer.Count == 0);
    }
}