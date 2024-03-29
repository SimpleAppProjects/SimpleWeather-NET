﻿using System;
using System.IO;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace SimpleWeather.Utils
{
    public static class ImageUtils
    {
        public enum ImageType
        {
            Jpeg,
            Png,
            Gif,
            Bmp,
            Webp,
            Unknown
        }

        /// <summary>
        /// Guess image file type from file header
        /// 
        /// Checks for GIF, JPEG, PNG, WEBP and BMP file formats
        /// 
        /// Sources:
        /// URLConnection.guessContentTypeFromStream
        /// https://stackoverflow.com/questions/670546/determine-if-file-is-an-image
        /// com.bumptech.glide.load.resource.bitmap.DefaultImageHeaderParser
        /// </summary>
        /// <param name="stream">The input stream to read from</param>
        /// <returns>Type supposed image type</returns>
        public static ImageType GuessImageType(Stream stream)
        {
            // If we can't read ahead safely, just give up on guessing
            if (!stream.CanRead || !stream.CanSeek)
                return ImageType.Unknown;

            var pos = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);

            int c1 = stream.ReadByte();
            int c2 = stream.ReadByte();
            int c3 = stream.ReadByte();
            int c4 = stream.ReadByte();
            int c5 = stream.ReadByte();
            int c6 = stream.ReadByte();
            int c7 = stream.ReadByte();
            int c8 = stream.ReadByte();
            int c9 = stream.ReadByte();
            int c10 = stream.ReadByte();
            int c11 = stream.ReadByte();
            int c12 = stream.ReadByte();

            stream.Seek(pos, SeekOrigin.Begin);

            if (c1 == 0xFF && c2 == 0xD8 && c3 == 0xFF)
            {
                if (c4 == 0xE0 || c4 == 0xEE)
                {
                    return ImageType.Jpeg;
                }

                /*
                 * File format used by digital cameras to store images.
                 * Exif Format can be read by any application supporting
                 * JPEG. Exif Spec can be found at:
                 * http://www.pima.net/standards/it10/PIMA15740/Exif_2-1.PDF
                 */
                if ((c4 == 0xE1) &&
                        (c7 == 'E' && c8 == 'x' && c9 == 'i' && c10 == 'f' &&
                                c11 == 0))
                {
                    return ImageType.Jpeg;
                }
            }

            if (c1 == 0x89 && c2 == 0x50 && c3 == 0x4e &&
                    c4 == 0x47 && c5 == 0x0d && c6 == 0x0a &&
                    c7 == 0x1a && c8 == 0x0a)
            {
                return ImageType.Png;
            }

            // 0x47 0x49 0x46
            if (c1 == 'G' && c2 == 'I' && c3 == 'F' && c4 == '8')
            {
                return ImageType.Gif;
            }

            // WebP - "RIFF"
            if (c1 == 0x52 && c2 == 0x49 && c3 == 0x46 && c4 == 0x46)
            {
                return ImageType.Webp;
            }

            // "BM"
            if (c1 == 0x42 && c2 == 0x4D)
            {
                return ImageType.Bmp;
            }

            return ImageType.Unknown;
        }

#if WINDOWS
        public static async Task<string> WeatherIconToBase64(string icon, bool isLight = false)
        {
            var wim = SharedModule.Instance.WeatherIconsManager;
            var iconUri = new Uri(wim.GetWeatherIconURI(icon, true, isLight));

            var file = await StorageFile.GetFileFromApplicationUriAsync(iconUri);
            using var fs = await file.OpenStreamForReadAsync();
            var imageType = GuessImageType(fs);

            var bytes = new byte[fs.Length];
            await fs.ReadAsync(bytes);

            var base64Str = Convert.ToBase64String(bytes);

            var prefix = imageType switch
            {
                ImageType.Jpeg => "data:image/jpeg;base64,",
                ImageType.Gif => "data:image/gif;base64,",
                ImageType.Bmp => "data:image/bmp;base64,",
                ImageType.Webp => "data:image/webp;base64,",
                _ => "data:image/png;base64,"
            };

            return prefix + base64Str;
        }

        public static async Task<string> ColorToBase64(Windows.UI.Color color)
        {
            var arr = new byte[1 * 1 * 4];
            for (int i = 0; i < arr.Length; i += 4)
            {
                // BGRA format
                arr[i] = color.B;
                arr[i + 1] = color.G;
                arr[i + 2] = color.R;
                arr[i + 3] = color.A;
            }

            using var ms = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, 1, 1, 96, 96, arr);
            await encoder.FlushAsync();

            var s = ms.AsStreamForRead();
            var buffer = new byte[s.Length];
            s.Read(buffer, 0, buffer.Length);

            return "data:image/png;base64," + Convert.ToBase64String(buffer);
        }
#endif
    }
}
