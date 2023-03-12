#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp.Views.Windows;
#else
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
#endif
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
    [TemplatePart(Name = nameof(InternalScrollViewer), Type = typeof(ScrollViewer))]
    [TemplatePart(Name = nameof(Canvas), Type = typeof(SKXamlCanvas))]
#endif
    public abstract partial class BaseGraphViewControl
#if WINDOWS
        : Control
#else
        : TemplatedView
#endif
    {
#if WINDOWS
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming have caused the view
        //     to change.
        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged;

        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming cause the view to change.
        public event EventHandler<ScrollViewerViewChangingEventArgs> ViewChanging;
#else
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming have caused the view
        //     to change.
        public event EventHandler<ScrolledEventArgs> ViewChanged;
#endif

        public event EventHandler<ItemSizeChangedEventArgs> ItemWidthChanged;

        protected ScrollViewer InternalScrollViewer { get; private set; }
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

            InternalScrollViewer = GetTemplateChild(nameof(InternalScrollViewer)) as ScrollViewer;
            Canvas = GetTemplateChild(nameof(Canvas)) as SKXamlCanvas;

            if (Canvas != null)
            {
                OnCanvasLoaded(Canvas);
                Canvas.PaintSurface += Canvas_PaintSurface;
            }

            if (InternalScrollViewer != null)
            {
#if WINDOWS
                InternalScrollViewer.ViewChanged += InternalScrollViewer_ViewChanged;
                InternalScrollViewer.ViewChanging += InternalScrollViewer_ViewChanging;
#else
                InternalScrollViewer.Scrolled += InternalScrollViewer_ViewChanged;
                InternalScrollViewer.HandlerChanged += InternalScrollViewer_HandlerChanged;
#endif
            }
        }

#if WINDOWS
        protected virtual void OnViewChanging() { }
#endif
        protected virtual void OnViewChanged() { }

#if WINDOWS
        private void InternalScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            OnViewChanging();
            ViewChanging?.Invoke(sender, e);
        }
#endif

        private void InternalScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            OnViewChanged();
            ViewChanged?.Invoke(sender, e);
        }

        private void BaseGraphViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                OnCanvasLoaded(Canvas);
                Canvas.PaintSurface += Canvas_PaintSurface;
            }
            if (InternalScrollViewer != null)
            {
#if WINDOWS
                InternalScrollViewer.ViewChanged += InternalScrollViewer_ViewChanged;
                InternalScrollViewer.ViewChanging += InternalScrollViewer_ViewChanging;
#else
                InternalScrollViewer.Scrolled += InternalScrollViewer_ViewChanged;
                InternalScrollViewer.HandlerChanged += InternalScrollViewer_HandlerChanged;
#endif
            }
        }

        private void BaseGraphViewControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.PaintSurface -= Canvas_PaintSurface;
            }
            if (InternalScrollViewer != null)
            {
#if WINDOWS
                InternalScrollViewer.ViewChanged -= InternalScrollViewer_ViewChanged;
                InternalScrollViewer.ViewChanging -= InternalScrollViewer_ViewChanging;
#else
                InternalScrollViewer.Scrolled -= InternalScrollViewer_ViewChanged;
                InternalScrollViewer.HandlerChanged -= InternalScrollViewer_HandlerChanged;
#endif
            }
        }

#if !WINDOWS
        private void InternalScrollViewer_HandlerChanged(object sender, RoutedEventArgs e)
        {
            if (sender is VisualElement element && element.Handler is IPlatformViewHandler handler)
            {
#if IOS || MACCATALYST
                if (handler.PlatformView is UIKit.UIScrollView v)
                {
                    v.Bounces = false;
                }
#endif
            }
        }
#endif

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            // get the screen density for scaling
#if WINDOWS
            var scale = (float)XamlRoot.RasterizationScale;
#elif __IOS__
            var scale = 1f;
            if (sender is VisualElement element && element.Handler?.PlatformView is UIKit.UIView v)
            {
                scale = (float)v.ContentScaleFactor;
            }
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

        protected virtual void OnCanvasLoaded(SKXamlCanvas canvas) { }
        protected virtual void OnPreDraw(SKCanvas canvas) { }
        protected virtual void OnDraw(SKCanvas canvas) { }

        internal virtual void OnItemWidthChanged(ItemSizeChangedEventArgs e)
        {
            ItemWidthChanged?.Invoke(this, e);
        }
    }
}
