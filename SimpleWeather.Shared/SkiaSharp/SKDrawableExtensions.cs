using SkiaSharp;
using SkiaSharp.Skottie;
using Svg.Skia;

namespace SimpleWeather.SkiaSharp
{
    public static class SKDrawableExtensions
    {
        public static SKDrawable ToDrawable(this SKBitmap bitmap)
        {
            return new SKBitmapDrawable(bitmap);
        }

        public static SKDrawable ToDrawable(this SKSvg svg)
        {
            return new SKSvgDrawable(svg);
        }

        public static SKLottieDrawable ToDrawable(this Animation animation)
        {
            return new SKLottieDrawable(animation);
        }
    }
}
