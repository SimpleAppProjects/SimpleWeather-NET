using System;
using SimpleWeather.SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace SimpleWeather.Maui.Controls
{
    public partial class IconControl
    {
        /* Drawables */
        private readonly Stack<SKLottieDrawable> animatedDrawables = new();

        private void RemoveAnimatedDrawables()
        {
            while (animatedDrawables.Any())
            {
                var drw = animatedDrawables.Pop();
                drw.Stop();
                drw.InvalidateDrawable -= IconControl_InvalidateDrawable;
            }
        }

        private void AddAnimatedDrawable(SKLottieDrawable drawable)
        {
            if (!animatedDrawables.Contains(drawable))
            {
                drawable.InvalidateDrawable += IconControl_InvalidateDrawable;
                drawable.Start();
                animatedDrawables.Push(drawable);
            }
        }

        private void IconControl_InvalidateDrawable(object sender, EventArgs e)
        {
            Canvas?.InvalidateSurface();
        }
    }
}

