#if !__IOS__
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Rendering.Skia.Cache;
using Mapsui.Rendering.Skia.Extensions;
using Mapsui.Rendering.Skia.Images;
using Mapsui.Rendering.Skia.SkiaWidgets;
using Mapsui.Styles;
using Mapsui.Widgets;
using Mapsui.Widgets.ButtonWidgets;
using SimpleWeather.SkiaSharp;
using SkiaSharp;
using Svg.Skia;

namespace SimpleWeather.NET.MapsUi
{
    public class ImageButtonWidgetRenderer : ISkiaWidgetRenderer
    {
        public void Draw(SKCanvas canvas, Viewport viewport, IWidget widget, RenderService renderService, float layerOpacity)
        {
            var button = (ImageButtonWidget)widget;

            if (button.Image == null)
                throw new InvalidOperationException("ImageSource is not set");

            var drawableImage = renderService.DrawableImageCache.GetOrCreate(button.Image.SourceId,
                () => TryCreateDrawableImage(button.Image, renderService.ImageSourceCache));
            if (drawableImage == null)
                return;

            button.UpdateEnvelope(
                button.Width != 0 ? button.Width : drawableImage.Width + button.Padding.Left + button.Padding.Right,
                button.Height != 0 ? button.Height : drawableImage.Height + button.Padding.Top + button.Padding.Bottom,
                viewport.Width,

                viewport.Height);

            if (button.Envelope == null)
                return;

            using var backPaint = new SKPaint { Color = button.BackColor.ToSkia(layerOpacity), IsAntialias = true };
            canvas.DrawRoundRect(button.Envelope.ToSkia(), (float)button.CornerRadius, (float)button.CornerRadius, backPaint);

            // Get the scale for picture in each direction
            var scaleX = (button.Envelope.Width - button.Padding.Left - button.Padding.Right) / drawableImage.Width;
            var scaleY = (button.Envelope.Height - button.Padding.Top - button.Padding.Bottom) / drawableImage.Height;


            using var skPaint = new SKPaint { IsAntialias = true, FilterQuality = SKFilterQuality.High, IsDither = true };
            if (drawableImage is SKBitmapDrawableImage bitmapImage)
            {
                int cnt = canvas.Save();

                // Rotate picture
                var matrix = SKMatrix.CreateRotationDegrees((float)button.Rotation, drawableImage.Width / 2f, drawableImage.Height / 2f);
                // Create a scale matrix
                matrix = matrix.PostConcat(SKMatrix.CreateScale((float)scaleX, (float)scaleY));
                // Translate picture to right place
                matrix = matrix.PostConcat(SKMatrix.CreateTranslation((float)(button.Envelope.MinX + button.Padding.Left), (float)(button.Envelope.MinY + button.Padding.Top)));
                // Draw picture
                canvas.SetMatrix(matrix);
                canvas.DrawBitmap(bitmapImage.Bitmap, 0, 0, skPaint);

                canvas.RestoreToCount(cnt);
            }
            else if (drawableImage is SvgDrawableImage svgImage)
            {
                // Rotate picture
                var matrix = SKMatrix.CreateRotationDegrees((float)button.Rotation, drawableImage.Width / 2f, drawableImage.Height / 2f);
                // Create a scale matrix
                matrix = matrix.PostConcat(SKMatrix.CreateScale((float)scaleX, (float)scaleY));
                // Translate picture to right place
                matrix = matrix.PostConcat(SKMatrix.CreateTranslation((float)(button.Envelope.MinX + button.Padding.Left), (float)(button.Envelope.MinY + button.Padding.Top)));
                // Draw picture
                canvas.DrawPicture(svgImage.Picture, in matrix, skPaint);
            }
            else
                throw new NotSupportedException("DrawableImage type not supported");
        }

        private static IDrawableImage? TryCreateDrawableImage(Image image, ImageSourceCache imageSourceCache)
        {
            byte[] array = imageSourceCache.Get(image.Source);
            if (array == null)
            {
                return null;
            }

            return ToDrawableImage(array);
        }

        private static IDrawableImage ToDrawableImage(byte[] bytes)
        {
            if (bytes.IsSvg())
            {
                return new SvgDrawableImage(SKSvg.CreateFromStream(new MemoryStream(bytes)));
            }

            return new SKBitmapDrawableImage(SKBitmap.Decode(bytes));
        }

        private class SvgDrawableImage(SKSvg svg) : SKSvgDrawable(svg), IDrawableImage
        {
            public float Width => svg.Picture.CullRect.Width;

            public float Height => svg.Picture.CullRect.Height;

            public SKPicture Picture => svg.Picture;
        }

        private class SKBitmapDrawableImage(SKBitmap bitmap) : SKBitmapDrawable(bitmap), IDrawableImage
        {
            public float Width => bitmap.Width;

            public float Height => bitmap.Height;

            public SKBitmap Bitmap => bitmap;
        }
    }
}
#endif