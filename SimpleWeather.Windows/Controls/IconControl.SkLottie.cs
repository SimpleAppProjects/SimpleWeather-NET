using SimpleWeather.SkiaSharp;
using SkiaSharp.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWeather.NET.Controls
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
            if (IconBox.Child is SKXamlCanvas canvas)
            {
                canvas.Invalidate();
            }
        }
    }
}
