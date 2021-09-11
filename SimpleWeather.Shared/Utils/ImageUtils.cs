using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
