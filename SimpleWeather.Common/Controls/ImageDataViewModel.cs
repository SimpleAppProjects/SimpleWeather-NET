using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using SimpleWeather.Helpers;
#if WINUI
using Microsoft.UI;
using Windows.UI;
#else
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
#endif

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
#if !WINUI
        public ImageSource ImageSource { get; protected internal set; }
#endif

        internal ImageDataViewModel() { }

        public ImageDataViewModel(ImageData imageData)
        {
            this.ArtistName = imageData.ArtistName;
            this.Color = ColorUtils.ParseColor(imageData.HexColor);
#if WINUI
            this.ImageUri = imageData.ImageUrl != null ? new Uri(imageData.ImageUrl, UriKind.Absolute) : null;
#else
            if (imageData.ImageUrl != null)
            {
                if (imageData.ImageUrl.StartsWith("maui-appx"))
                {
                    this.ImageUri = new Uri(imageData.ImageUrl.ReplaceFirst("maui-appx://", ""), UriKind.Relative);
                    this.ImageSource = ImageSource.FromStream(async (ct) =>
                    {
                        return await FileSystemUtils.OpenAppPackageFileAsync(this.ImageUri.ToString()).WaitAsync(ct);
                    });
                }
                else
                {
                    if (imageData.ImageUrl.StartsWith("ios"))
                    {
                        this.ImageUri = new Uri(imageData.ImageUrl.ReplaceFirst("ios://", Path.TrimEndingDirectorySeparator(ApplicationDataHelper.GetRootDataFolderPath())), UriKind.Absolute);
                    }
                    else
                    {
                        this.ImageUri = new Uri(imageData.ImageUrl, UriKind.Absolute);
                    }

                    if (this.ImageUri.IsFile)
                    {
                        this.ImageSource = new FileImageSource()
                        {
                            File = this.ImageUri.AbsolutePath
                        };

                    }
                    else
                    {
                        this.ImageSource = new UriImageSource()
                        {
                            Uri = this.ImageUri
                        };
                    }
                }
            }
#endif
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