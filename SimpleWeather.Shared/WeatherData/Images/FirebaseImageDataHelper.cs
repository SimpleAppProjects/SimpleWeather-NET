using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData.Images
{
    public abstract class FirebaseImageDataHelper : ImageDataHelperImpl
    {
        public override async Task<ImageData> GetRemoteImageData(String backgroundCode)
        {
            var imageData = await FirebaseImageDatabase.GetRandomImageForCondition(backgroundCode);

            if (imageData?.IsValid() == true)
            {
                var cachedImage = await CacheImage(imageData);
                return cachedImage;
            }

            return null;
        }

        public override async Task<ImageData> CacheImage(ImageData imageData)
        {
            if (imageData?.IsValid() == true)
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
            }

            // Invalid image uri
            return null;
        }
    }
}
