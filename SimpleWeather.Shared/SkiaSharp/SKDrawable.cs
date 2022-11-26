using SkiaSharp;
using Svg.Skia;
using System;

namespace SimpleWeather.SkiaSharp
{
    public abstract class SKDrawable
    {
        public SKRect Bounds { get; set; }

        public abstract void Draw(SKCanvas canvas);
    }

    public class SKBitmapDrawable : SKDrawable, IDisposable
    {
        private readonly SKBitmap _bitmap;
        private bool disposedValue;

        public SKBitmapDrawable(SKBitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawBitmap(_bitmap, Bounds);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _bitmap?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SKBitmapDrawable()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class SKSvgDrawable : SKDrawable, IDisposable
    {
        private readonly SKSvg svg;
        private bool disposedValue;

        public SKSvgDrawable(SKSvg svg)
        {
            this.svg = svg;
        }

        public override void Draw(SKCanvas canvas)
        {
            SKMatrix scaleMatrix = default;
            if (svg.Picture?.CullRect is { } rect)
            {
                scaleMatrix = SKMatrix.CreateScale(Bounds.Width / rect.Width, Bounds.Height / rect.Height);
            }
            canvas.DrawPicture(svg.Picture, ref scaleMatrix);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    svg?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SKSvgDrawable()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
