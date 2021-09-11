using Newtonsoft.Json;
using System;
using System.IO;

namespace SimpleWeather.WeatherData.Images.Model
{
    [SQLite.Table("imagedata")]
    public class ImageData
    {
        [SQLite.PrimaryKey]
        internal string DocumentId { get; set; }

        [JsonProperty("artistName")]
        public string ArtistName { get; set; }
        [JsonProperty("color")]
        public string HexColor { get; set; }
        [JsonProperty("condition")]
        public string Condition { get; set; }
        [JsonProperty("imageURL")]
        public string ImageUrl { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("originalLink")]
        public string OriginalLink { get; set; }
        [JsonProperty("siteName")]
        public string SiteName { get; set; }

        public bool IsValid()
        {
            return ImageUrl != null &&
                (File.Exists(ImageUrl) || Uri.IsWellFormedUriString(ImageUrl, UriKind.Absolute)) &&
                !String.IsNullOrWhiteSpace(HexColor);
        }

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
