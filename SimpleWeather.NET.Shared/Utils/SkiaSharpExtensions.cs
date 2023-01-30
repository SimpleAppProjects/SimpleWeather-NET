using SkiaSharp;

namespace SimpleWeather.NET.Utils
{
    public static class SkiaSharpExtensions
    {
        /// <summary>
        /// Converts a Windows color into a SkiaSharp color.
        /// </summary>
        /// <param name="color">The Windows color</param>
        /// <returns>Returns a SkiaSharp color</returns>
        public static SKColor ToSKColor(this Windows.UI.Color color)
        {
            return new SKColor(red: color.R, green: color.G, blue: color.B, alpha: color.A);
        }
    }
}
