using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.Uno.Controls
{
    public sealed partial class LocationPanel : UserControl
    {
        public LocationPanelUiModel ViewModel
        {
            get { return this.DataContext as LocationPanelUiModel; }
        }

        private double ControlShadowOpacity
        {
            get { return (double)GetValue(ControlShadowOpacityProperty); }
            set { SetValue(ControlShadowOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlShadowOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlShadowOpacityProperty =
            DependencyProperty.Register("ControlShadowOpacity", typeof(double), typeof(LocationPanel), new PropertyMetadata(0d));

        private ElementTheme ControlTheme
        {
            get { return (ElementTheme)GetValue(ControlThemeProperty); }
            set { SetValue(ControlThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlThemeProperty =
            DependencyProperty.Register("ControlTheme", typeof(ElementTheme), typeof(LocationPanel), new PropertyMetadata(ElementTheme.Default));

        public LocationPanel()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                UpdateControlTheme();
                //this.Bindings.Update(); // Update control theme updates bindings
            };
        }

        private void BackgroundOverlay_ImageOpened(object sender, RoutedEventArgs e)
        {
            UpdateControlTheme(true);
        }

        private void BackgroundOverlay_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            UpdateControlTheme(false);
        }

        private void UpdateControlTheme()
        {
            UpdateControlTheme(Utils.FeatureSettings.LocationPanelBackgroundImage);
        }

        private void UpdateControlTheme(bool backgroundEnabled)
        {
            if (backgroundEnabled)
            {
                if (GradientOverlay != null)
                {
                    GradientOverlay.Visibility = Visibility.Visible;
                }
                ControlTheme = ElementTheme.Dark;
                ControlShadowOpacity = 1;
            }
            else
            {
                if (GradientOverlay != null)
                {
                    GradientOverlay.Visibility = Visibility.Collapsed;
                }
                ControlTheme = ElementTheme.Default;
                ControlShadowOpacity = 0;
            }
            this.Bindings.Update();
        }
    }
}
