#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp.Views.Windows;
#else
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
#endif
using SimpleWeather.Utils;
using SkiaSharp;
#if WINDOWS
#else
using ScrollViewer = Microsoft.Maui.Controls.ScrollView;
using ScrollViewerViewChangedEventArgs = Microsoft.Maui.Controls.ScrolledEventArgs;
using SKXamlCanvas = SkiaSharp.Views.Maui.Controls.SKCanvasView;
using RoutedEventArgs = System.EventArgs;
using System.Diagnostics;
#endif

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.NET.Controls.Graphs
{
#if WINDOWS
    [TemplatePart(Name = nameof(Canvas), Type = typeof(SKXamlCanvas))]
#endif
    public abstract partial class BaseGraphViewControl
#if WINDOWS
        : Control
#else
        : TemplatedView
#endif
    {
        public event EventHandler<ItemSizeChangedEventArgs> ItemWidthChanged;

        protected SKXamlCanvas Canvas { get; private set; }

        protected float ViewHeight;
        protected float ViewWidth;

        public double MaxCanvasWidth
        {
            get => (double)GetValue(MaxCanvasWidthProperty);
            set => SetValue(MaxCanvasWidthProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for MaxCanvasWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxCanvasWidthProperty =
            DependencyProperty.Register(nameof(MaxCanvasWidth), typeof(double), typeof(BaseGraphViewControl), new PropertyMetadata(0d, (o, e) => (o as UIElement)?.InvalidateMeasure()));
#else
        // Using a DependencyProperty as the backing store for MaxCanvasWidth.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty MaxCanvasWidthProperty =
            BindableProperty.Create(nameof(MaxCanvasWidth), typeof(double), typeof(BaseGraphViewControl), 0d, propertyChanged: (o, _, _) => (o as BaseGraphViewControl)?.InvalidateMeasure());
#endif

        public BaseGraphViewControl()
        {
#if WINDOWS
            this.DefaultStyleKey = typeof(BaseGraphViewControl);
#endif
            this.Loaded += BaseGraphViewControl_Loaded;
            this.Unloaded += BaseGraphViewControl_Unloaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Canvas = GetTemplateChild(nameof(Canvas)) as SKXamlCanvas;

            if (Canvas != null)
            {
                OnCanvasLoaded(Canvas);
                Canvas.PaintSurface += Canvas_PaintSurface;
            }
        }

        private void BaseGraphViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                OnCanvasLoaded(Canvas);
                Canvas.PaintSurface += Canvas_PaintSurface;
            }
        }

        private void BaseGraphViewControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Canvas?.Let(canvas =>
            {
                try
                {
                    canvas.PaintSurface -= Canvas_PaintSurface;
                }
                catch { }
            });
        }

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            // get the screen density for scaling
#if WINDOWS
            var scale = 1f;
            //var scale = (float)XamlRoot.RasterizationScale;
#elif __IOS__
            var scale = 1f;
            /*
            if (sender is VisualElement element && element.Handler?.PlatformView is UIKit.UIView v)
            {
                scale = (float)v.ContentScaleFactor;
            }
            */
#else
            var scale = 1f;
#endif

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(SKColors.Transparent);

            OnPreDraw(canvas);
            OnDraw(canvas);
        }

        public void Invalidate()
        {
#if WINDOWS
            Canvas?.Invalidate();
#else
            Canvas?.InvalidateSurface();
#endif
        }

        protected virtual void OnCanvasLoaded(SKXamlCanvas canvas) { }
        protected virtual void OnPreDraw(SKCanvas canvas) { }
        protected virtual void OnDraw(SKCanvas canvas) { }

        internal virtual void OnItemWidthChanged(ItemSizeChangedEventArgs e)
        {
            ItemWidthChanged?.Invoke(this, e);
        }
    }
}
