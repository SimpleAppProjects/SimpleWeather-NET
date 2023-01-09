﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.Uno.Controls.Graphs
{
    [TemplatePart(Name = nameof(InternalScrollViewer), Type = typeof(ScrollViewer))]
    [TemplatePart(Name = nameof(Canvas), Type = typeof(SKXamlCanvas))]
    public abstract partial class BaseGraphViewControl : Control
    {
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming have caused the view
        //     to change.
        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged;

        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming cause the view to change.
        public event EventHandler<ScrollViewerViewChangingEventArgs> ViewChanging;

        internal event EventHandler<ItemSizeChangedEventArgs> ItemWidthChanged;

        protected ScrollViewer InternalScrollViewer { get; private set; }
        protected SKXamlCanvas Canvas { get; private set; }

        protected float ViewHeight;
        protected float ViewWidth;

        public double MaxCanvasWidth
        {
            get { return (double)GetValue(MaxCanvasWidthProperty); }
            set { SetValue(MaxCanvasWidthProperty, value); InvalidateMeasure(); }
        }

        // Using a DependencyProperty as the backing store for MaxCanvasWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxCanvasWidthProperty =
            DependencyProperty.Register("MaxCanvasWidth", typeof(double), typeof(BaseGraphViewControl), new PropertyMetadata(0d));

        public BaseGraphViewControl()
        {
            this.DefaultStyleKey = typeof(BaseGraphViewControl);
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
                InternalScrollViewer.ViewChanged += InternalScrollViewer_ViewChanged;
                InternalScrollViewer.ViewChanging += InternalScrollViewer_ViewChanging;
            }
        }

        protected virtual void OnViewChanging() { }
        protected virtual void OnViewChanged() { }

        private void InternalScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            OnViewChanging();
            ViewChanging?.Invoke(sender, e);
        }

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
                InternalScrollViewer.ViewChanged += InternalScrollViewer_ViewChanged;
                InternalScrollViewer.ViewChanging += InternalScrollViewer_ViewChanging;
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
                InternalScrollViewer.ViewChanged -= InternalScrollViewer_ViewChanged;
                InternalScrollViewer.ViewChanging -= InternalScrollViewer_ViewChanging;
            }
        }

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            // get the screen density for scaling
#if WINDOWS
            var scale = (float)XamlRoot.RasterizationScale;
#elif HAS_UNO
            var display = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            var scale = display.LogicalDpi / 96.0f;
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
