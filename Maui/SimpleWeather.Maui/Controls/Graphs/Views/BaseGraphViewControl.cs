using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.Maui.Controls.Graphs
{
    public abstract partial class BaseGraphViewControl : TemplatedView
    {
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming have caused the view
        //     to change.
        public event EventHandler<ScrolledEventArgs> ViewChanged;

        internal event EventHandler<ItemSizeChangedEventArgs> ItemWidthChanged;

        protected ScrollView InternalScrollViewer { get; private set; }
        protected SKCanvasView Canvas { get; private set; }

        protected float ViewHeight;
        protected float ViewWidth;

        public double MaxCanvasWidth
        {
            get => (double)GetValue(MaxCanvasWidthProperty);
            set => SetValue(MaxCanvasWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for MaxCanvasWidth.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty MaxCanvasWidthProperty =
            BindableProperty.Create(nameof(MaxCanvasWidth), typeof(double), typeof(BaseGraphViewControl), 0d, propertyChanged: (o, _, _) => (o as BaseGraphViewControl)?.InvalidateMeasure());

        public BaseGraphViewControl()
        {
            this.Loaded += BaseGraphViewControl_Loaded;
            this.Unloaded += BaseGraphViewControl_Unloaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            InternalScrollViewer = GetTemplateChild(nameof(InternalScrollViewer)) as ScrollView;
            Canvas = GetTemplateChild(nameof(Canvas)) as SKCanvasView;

            if (Canvas != null)
            {
                OnCanvasLoaded(Canvas);
                Canvas.PaintSurface += Canvas_PaintSurface;
            }

            if (InternalScrollViewer != null)
            {
                InternalScrollViewer.Scrolled += InternalScrollViewer_ViewChanged;
            }
        }

        protected virtual void OnViewChanging() { }
        protected virtual void OnViewChanged() { }

        private void InternalScrollViewer_ViewChanged(object sender, ScrolledEventArgs e)
        {
            OnViewChanged();
            ViewChanged?.Invoke(sender, e);
        }

        private void BaseGraphViewControl_Loaded(object sender, EventArgs e)
        {
            if (Canvas != null)
            {
                OnCanvasLoaded(Canvas);
                Canvas.PaintSurface += Canvas_PaintSurface;
            }
            if (InternalScrollViewer != null)
            {
                InternalScrollViewer.Scrolled += InternalScrollViewer_ViewChanged;
            }
        }

        private void BaseGraphViewControl_Unloaded(object sender, EventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.PaintSurface -= Canvas_PaintSurface;
            }
            if (InternalScrollViewer != null)
            {
                InternalScrollViewer.Scrolled -= InternalScrollViewer_ViewChanged;
            }
        }

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            // get the screen density for scaling
            var scale = 1f;//(float)XamlRoot.RasterizationScale;

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(SKColors.Transparent);

            OnPreDraw(canvas);
            OnDraw(canvas);
        }

        protected virtual void OnCanvasLoaded(SKCanvasView canvas) { }
        protected virtual void OnPreDraw(SKCanvas canvas) { }
        protected virtual void OnDraw(SKCanvas canvas) { }

        internal virtual void OnItemWidthChanged(ItemSizeChangedEventArgs e)
        {
            ItemWidthChanged?.Invoke(this, e);
        }
    }
}
