using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI;

namespace SimpleWeather.Common.Controls
{
    [Bindable(true)]
    public class ImageDataViewModel
    {
        public string ArtistName { get; protected internal set; }
        public Color Color { get; protected internal set; }
        public Uri ImageUri { get; protected internal set; }
        public string SiteName { get; protected internal set; }
        public Uri OriginalLink { get; protected internal set; }

        internal ImageDataViewModel() { }

        public ImageDataViewModel(ImageData imageData)
        {
            this.ArtistName = imageData.ArtistName;
            this.Color = ColorUtils.ParseColor(imageData.HexColor);
            this.ImageUri = imageData.ImageUrl != null ? new Uri(imageData.ImageUrl, UriKind.Absolute) : null;
            this.SiteName = imageData.SiteName;
            this.OriginalLink = imageData.OriginalLink != null ? new Uri(imageData.OriginalLink) : null;
        }

        public override bool Equals(object obj)
        {
            return obj is ImageDataViewModel model &&
                   ArtistName == model.ArtistName &&
                   EqualityComparer<Color>.Default.Equals(Color, model.Color) &&
                   EqualityComparer<Uri>.Default.Equals(ImageUri, model.ImageUri) &&
                   SiteName == model.SiteName &&
                   EqualityComparer<Uri>.Default.Equals(OriginalLink, model.OriginalLink);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ArtistName, Color, ImageUri, SiteName, OriginalLink);
        }
    }
}