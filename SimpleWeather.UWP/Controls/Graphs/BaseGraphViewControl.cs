using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.UWP.Controls.Graphs
{
    [TemplatePart(Name = nameof(InternalScrollViewer), Type = typeof(ScrollViewer))]
    [TemplatePart(Name = nameof(Canvas), Type = typeof(CanvasVirtualControl))]
    [TemplatePart(Name = nameof(IconCanvas), Type = typeof(Canvas))]
    public abstract class BaseGraphViewControl : Control
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
        protected CanvasVirtualControl Canvas { get; private set; }
        protected Canvas IconCanvas { get; private set; }

        protected float ViewHeight;
        protected float ViewWidth;

        public bool ReadyToDraw => Canvas?.ReadyToDraw ?? false;

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
            Canvas = GetTemplateChild(nameof(Canvas)) as CanvasVirtualControl;
            IconCanvas = GetTemplateChild(nameof(IconCanvas)) as Canvas;

            Canvas.CreateResources += Canvas_CreateResources;
            Canvas.RegionsInvalidated += Canvas_RegionsInvalidated;

            InternalScrollViewer.ViewChanged += InternalScrollViewer_ViewChanged;
            InternalScrollViewer.ViewChanging += InternalScrollViewer_ViewChanging;
        }

        private void InternalScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            ViewChanging?.Invoke(sender, e);
        }

        private void InternalScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ViewChanged?.Invoke(sender, e);
        }

        private void BaseGraphViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.CreateResources += Canvas_CreateResources;
                Canvas.RegionsInvalidated += Canvas_RegionsInvalidated;
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
                Canvas.CreateResources -= Canvas_CreateResources;
                Canvas.RegionsInvalidated -= Canvas_RegionsInvalidated;
            }
            if (InternalScrollViewer != null)
            {
                InternalScrollViewer.ViewChanged -= InternalScrollViewer_ViewChanged;
                InternalScrollViewer.ViewChanging -= InternalScrollViewer_ViewChanging;
            }
        }

        private void Canvas_CreateResources(CanvasVirtualControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            OnCreateCanvasResources(sender);
        }

        private void Canvas_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            OnCanvasRegionsInvalidated(sender, args);
        }

        protected virtual void OnCreateCanvasResources(CanvasVirtualControl canvas) { }
        protected virtual void OnCanvasRegionsInvalidated(CanvasVirtualControl canvas, CanvasRegionsInvalidatedEventArgs args) { }

        internal virtual void OnItemWidthChanged(ItemSizeChangedEventArgs e)
        {
            ItemWidthChanged?.Invoke(this, e);
        }
    }
}
