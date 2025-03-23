using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.NET.Controls.Graphs
{
    [TemplatePart(Name = nameof(InternalScrollViewer), Type = typeof(ScrollViewer))]
    public partial class GraphScrollView : Control
    {
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(UIElement), typeof(GraphScrollView), new PropertyMetadata(null));

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            private set { SetValue(HorizontalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(GraphScrollView), new PropertyMetadata(0d));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            private set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(GraphScrollView), new PropertyMetadata(0d));

        public event EventHandler<ScrollViewerViewChangingEventArgs> ViewChanging;
        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged;

        internal Size MeasuredSize { get; set; }

        protected ScrollViewer InternalScrollViewer;

        public GraphScrollView()
        {
            this.DefaultStyleKey = typeof(GraphScrollView);
            this.Loaded += GraphScrollView_Loaded;
            this.Unloaded += GraphScrollView_Unloaded;
        }

        public void ScrollTo(double horizontalOffset, double verticalOffset)
        {
            InternalScrollViewer?.ChangeView(horizontalOffset, verticalOffset, null);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            InternalScrollViewer = GetTemplateChild(nameof(InternalScrollViewer)) as ScrollViewer;

            if (InternalScrollViewer != null)
            {
                ApplyBindings(InternalScrollViewer);
            }
        }

        private void GraphScrollView_Loaded(object sender, RoutedEventArgs e)
        {
            if (InternalScrollViewer != null)
            {
                ApplyBindings(InternalScrollViewer);
            }
        }

        private void GraphScrollView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (InternalScrollViewer != null)
            {
                RemoveBindings(InternalScrollViewer);
            }
        }

        private void ApplyBindings(ScrollViewer internalScrollViewer)
        {
            internalScrollViewer.SetBinding(ScrollViewer.ContentProperty, new Binding()
            {
                Path = new PropertyPath(nameof(Content)),
                Mode = BindingMode.OneWay,
                Source = this,
            });
            this.SetBinding(HorizontalOffsetProperty, new Binding()
            {
                Path = new PropertyPath(nameof(ScrollViewer.HorizontalOffset)),
                Mode = BindingMode.OneWay,
                Source = internalScrollViewer,
            });

            this.SetBinding(VerticalOffsetProperty, new Binding()
            {
                Path = new PropertyPath(nameof(ScrollViewer.VerticalOffset)),
                Mode = BindingMode.OneWay,
                Source = internalScrollViewer,
            });

            internalScrollViewer.ViewChanging += ViewChanging;
            internalScrollViewer.ViewChanged += ViewChanged;
        }

        private void RemoveBindings(ScrollViewer internalScrollViewer)
        {
            internalScrollViewer.ClearValue(ScrollViewer.ContentProperty);
            this.ClearValue(HorizontalOffsetProperty);
            this.ClearValue(VerticalOffsetProperty);

            internalScrollViewer.ViewChanging -= ViewChanging;
            internalScrollViewer.ViewChanged -= ViewChanged;
        }
    }
}
