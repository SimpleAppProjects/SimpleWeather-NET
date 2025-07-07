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

        public static SKBitmapDrawable ToBitmapDrawable(this SKSvgDrawable svgDrawable, double width, double height)
        {
            var bitmap = new SKBitmap((int)width, (int)height);
            using var canvas = new SKCanvas(bitmap);
            
            SKMatrix scaleMatrix = default;
            if (svgDrawable?.svg?.Picture?.CullRect is { } rect)
            {
                scaleMatrix = SKMatrix.CreateScale(bitmap.Width / rect.Width, bitmap.Height / rect.Height);
            }
            
            canvas.Clear();
            canvas.DrawPicture(svgDrawable?.svg?.Picture, in scaleMatrix);
            
            return new SKBitmapDrawable(bitmap);
        }
    }
}
