using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.WeatherData.Images
{
    public static partial class ImageDataHelper
    {
        private static ImageDataHelperImpl sImageDataHelperImpl;
        public static ImageDataHelperImpl ImageDataHelperImpl
        {
            get
            {
                if (sImageDataHelperImpl == null)
                {
#if WINDOWS_UWP
                    sImageDataHelperImpl = new ImageDataHelperRes();
#else
                    throw new NotImplementedException();
#endif
                }

                return sImageDataHelperImpl;
            }
        }
    }

    public abstract class ImageDataHelperImpl
    {
        public abstract Task<ImageData> GetCachedImageData(String backgroundCode);

        public abstract Task<ImageData> GetRemoteImageData(String backgroundCode);

        public abstract Task<ImageData> CacheImage(ImageData imageData);

        protected abstract Task<ImageData> StoreImage(Uri imageUri, ImageData imageData);

        public abstract Task ClearCachedImageData();

        public abstract ImageData GetDefaultImageData(String backgroundCode, Weather weather);

        public abstract Task<bool> IsEmpty();
    }
}
