using SkiaSharp;

namespace SimpleWeather.NET.Utils
{
    public static class SkiaSharpExtensions
    {
#if WINDOWS
        /// <summary>
        /// Converts a Windows color into a SkiaSharp color.
        /// </summary>
        /// <param name="color">The Windows color</param>
        /// <returns>Returns a SkiaSharp color</returns>
        public static SKColor ToSKColor(this Windows.UI.Color color)
        {
            return new SKColor(red: color.R, green: color.G, blue: color.B, alpha: color.A);
        }
#endif

#if !WINDOWS
        /// <summary>
        /// Converts a Maui color into a SkiaSharp color.
        /// </summary>
        /// <param name="color">The Windows color</param>
        /// <returns>Returns a SkiaSharp color</returns>
        public static SKColor ToSKColor(this Microsoft.Maui.Graphics.Color color)
        {
            color.ToRgba(out byte r, out byte g, out byte b, out byte a);
            return new SKColor(red: r, green: g, blue: b, alpha: a);
        }
#endif
    }
}
