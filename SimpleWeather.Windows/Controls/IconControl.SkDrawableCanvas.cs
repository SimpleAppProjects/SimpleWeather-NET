using Microsoft.UI.Xaml;
using SimpleWeather.SkiaSharp;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using SKDrawable = SimpleWeather.SkiaSharp.SKDrawable;
using Thickness = Microsoft.UI.Xaml.Thickness;

namespace SimpleWeather.NET.Controls;

public partial class SkDrawableCanvas : SKXamlCanvas, IDisposable
{
    public SKDrawable? Drawable { get; private set; }

    public Thickness DrawablePadding
    {
        get => (Thickness)GetValue(DrawablePaddingProperty);
        set => SetValue(DrawablePaddingProperty, value);
    }

    public static readonly DependencyProperty DrawablePaddingProperty =
        DependencyProperty.RegisterAttached(nameof(DrawablePadding), typeof(Thickness), typeof(SkDrawableCanvas), new PropertyMetadata(default(Thickness)));

    public SkDrawableCanvas(SKDrawable drawable)
    {
        Drawable = drawable;
    }

    public void UpdateDrawable(SKDrawable? drawable, bool invalidate = true)
    {
        this.Drawable = drawable;
        
        if (invalidate)
            this.Invalidate();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        
        e.Surface.Canvas.Clear();
        
        if (Drawable is { } drawable) 
        {
            var padding = (float)Math.Max(DrawablePadding.Left +  DrawablePadding.Right, DrawablePadding.Top + DrawablePadding.Bottom) / 2;
            var bounds = new SKRect(0, 0, e.Info.Width - padding, e.Info.Height - padding);

            var cnt = e.Surface.Canvas.Save();

            drawable.Bounds = bounds;

            if (padding > 0) e.Surface.Canvas.Translate(padding / 2, padding / 2);

            drawable.Draw(e.Surface.Canvas);

            e.Surface.Canvas.RestoreToCount(cnt);
        }

        e.Surface.Flush(true);
    }

    public void Dispose()
    {
        if (Drawable is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}