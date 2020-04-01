using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleWeather.WeatherData.Images.Model
{
    [FirestoreData]
    [SQLite.Table("imagedata")]
    public class ImageData
    {
        [SQLite.PrimaryKey]
        [FirestoreDocumentId]
        internal string DocumentId { get; set; }

        [FirestoreProperty("artistName")]
        public string ArtistName { get; set; }
        [FirestoreProperty("color")]
        public string HexColor { get; set; }
        [FirestoreProperty("condition")]
        public string Condition { get; set; }
        [FirestoreProperty("imageURL")]
        public string ImageUrl { get; set; }
        [FirestoreProperty("location")]
        public string Location { get; set; }
        [FirestoreProperty("originalLink")]
        public string OriginalLink { get; set; }
        [FirestoreProperty("siteName")]
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
    }
}
