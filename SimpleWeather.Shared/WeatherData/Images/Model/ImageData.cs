#if !WINUI
using Microsoft.Maui.Storage;
#endif
using Newtonsoft.Json;
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
            return ImageUrl != null &&
                (File.Exists(ImageUrl) || Uri.IsWellFormedUriString(ImageUrl, UriKind.Absolute)) &&
                !String.IsNullOrWhiteSpace(HexColor);
        }
#else
        public async Task<bool> IsValidAsync()
        {
            return ImageUrl != null &&
                ((
                ImageUrl?.StartsWith("maui-appx") != true &&
                await FileSystemUtils.FileExistsAsync(ImageUrl)) ||
                Uri.IsWellFormedUriString(ImageUrl, UriKind.Absolute)) &&
                !String.IsNullOrWhiteSpace(HexColor);
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
