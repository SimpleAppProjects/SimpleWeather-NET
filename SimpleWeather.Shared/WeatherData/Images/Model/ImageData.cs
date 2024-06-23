#if !WINUI
using Microsoft.Maui.Storage;
#endif
using Newtonsoft.Json;
using SimpleWeather.Helpers;
using SimpleWeather.Utils;
using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData.Images.Model
{
    [SQLite.Table("imagedata")]
    public class ImageData
    {
        [SQLite.PrimaryKey]
        internal string DocumentId { get; set; }

        [JsonProperty("artistName")]
        [JsonPropertyName("artistName")]
        public string ArtistName { get; set; }
        [JsonProperty("color")]
        [JsonPropertyName("color")]
        public string HexColor { get; set; }
        [JsonProperty("condition")]
        [JsonPropertyName("condition")]
        public string Condition { get; set; }
        [JsonProperty("imageURL")]
        [JsonPropertyName("imageURL")]
        public string ImageUrl { get; set; }
        [JsonProperty("location")]
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonProperty("originalLink")]
        [JsonPropertyName("originalLink")]
        public string OriginalLink { get; set; }
        [JsonProperty("siteName")]
        [JsonPropertyName("siteName")]
        public string SiteName { get; set; }

#if WINUI
        public bool IsValid()
        {
            if (ImageUrl != null && !String.IsNullOrWhiteSpace(HexColor))
            {
                if (Uri.IsWellFormedUriString(ImageUrl, UriKind.Absolute))
                {
                    var uri = new Uri(ImageUrl);

                    if (Equals("file", uri.Scheme))
                    {
                        var fileInfo = new FileInfo(uri.ToString());
                        return !string.IsNullOrEmpty(uri.AbsolutePath) && fileInfo.Exists && fileInfo.Length > 0;
                    }
                    else
                    {
                        return uri.IsAbsoluteUri;
                    }
                }
            }

            return false;
        }
#else
        public async Task<bool> IsValidAsync()
        {
            if (ImageUrl != null && !string.IsNullOrWhiteSpace(HexColor))
            {
                Uri uri = new Uri(ImageUrl);

                if (Equals("maui-appx", uri.Scheme))
                {
                    return await FileSystemUtils.FileExistsAsync(ImageUrl);
                }
                else if (Equals("file", uri.Scheme))
                {
                    return !string.IsNullOrEmpty(uri.AbsolutePath) && FileUtils.IsValid(ImageUrl);
                }
                else if (Equals("ios", uri.Scheme))
                {
                    return !string.IsNullOrEmpty(uri.AbsolutePath) && FileUtils.IsValid(Path.Combine(ApplicationDataHelper.GetRootDataFolderPath(), uri.AbsolutePath.TrimStart('/')));
                }
                else
                {
                    return uri.IsAbsoluteUri;
                }
            }

            return false;
        }
#endif

        public static ImageData CopyWithNewImageUrl(ImageData @old, String newImagePath)
        {
            return new ImageData()
            {
                ArtistName = old.ArtistName,
                Condition = old.Condition,
                DocumentId = old.DocumentId,
                HexColor = old.HexColor,
                ImageUrl = newImagePath,
                Location = old.Location,
                OriginalLink = old.OriginalLink,
                SiteName = old.SiteName
            };
        }

        public override string ToString()
        {
            return "ImageData{" +
                "documentId='" + DocumentId + '\'' +
                ", condition='" + Condition + '\'' +
                ", imageURL='" + ImageUrl + '\'' +
                '}';
        }
    }
}
