using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.WeatherData.Images
{
    // TODO:
    // Make this a singleton class
    // Have UWP extend this and create its own implementation
    // Move platform specific code to its own implementation
    // Ex: caching image data and default data from assets
    public static partial class ImageDataHelper
    {
        private static ImageDataHelperImpl sImageDataHelperImpl;
        internal static ImageDataHelperImpl ImageDataHelperImpl
        {
            get
            {
                if (sImageDataHelperImpl == null)
                {
#if WINDOWS_UWP
                    sImageDataHelperImpl = new UWP.Shared.WeatherData.Images.ImageDataHelperImplUWP();
#else
                    throw new NotImplementedException();
#endif
                }

                return sImageDataHelperImpl;
            }
        }

        public static long ImageDBUpdateTime { get { return GetImageDBUpdateTime(); } set { SetImageDBUpdateTime(value); } }

#if WINDOWS_UWP
        private static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        private static long GetImageDBUpdateTime()
        {
            if (LocalSettings.Containers.ContainsKey("ImageDB_LastUpdated"))
            {
                return (long)LocalSettings.Values["ImageDB_LastUpdated"];
            }

            return 0;
        }

        private static void SetImageDBUpdateTime(long value)
        {
            LocalSettings.Values["ImageDB_LastUpdated"] = value;
        }
#endif
    }

    public abstract class ImageDataHelperImpl
    {
        public abstract Task<ImageData> GetCachedImageData(String backgroundCode);

        public Task<ImageData> GetRemoteImageData(String backgroundCode)
        {
            return Task.Run(async () =>
            {
                var imageData = await ImageDatabase.GetRandomImageForCondition(backgroundCode);

                if (imageData?.IsValid() == true)
                {
                    var cachedImage = await CacheImage(imageData);
                    return cachedImage;
                }

                return null;
            });
        }

        public Task<ImageData> CacheImage(ImageData imageData)
        {
            return Task.Run(async () =>
            {
                // Check if image url is valid
                Uri imageUri = new Uri(imageData.ImageUrl);
                if (imageUri.IsWellFormedOriginalString() &&
                    (imageUri.Scheme.Equals("gs") || imageUri.Scheme.Equals("https") || imageUri.Scheme.Equals("http")))
                {
                    // Download image to storage
                    // and image metadata to settings
                    var cachedImage = await StoreImage(imageUri, imageData);
                    return cachedImage;
                }

                // Invalid image uri
                return null;
            });
        }

        protected abstract Task<ImageData> StoreImage(Uri imageUri, ImageData imageData);

        public abstract Task ClearCachedImageData();

        public abstract ImageData GetDefaultImageData(String backgroundCode, Weather weather);

        public abstract Task<bool> IsEmpty();
    }
}
