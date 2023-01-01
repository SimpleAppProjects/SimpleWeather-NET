using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using muxc = Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.UWP.Controls.Uno
{
    public sealed partial class UnoProgressRing : Control
    {
#if WINDOWS_UWP
        private muxc.ProgressRing ProgressControl;
#else
        private ProgressRing ProgressControl;
#endif

        public static DependencyProperty IsActiveProperty { get; } = DependencyProperty.Register(
            nameof(IsActive), typeof(bool), typeof(ProgressRing), new PropertyMetadata(true));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public UnoProgressRing()
        {
            this.DefaultStyleKey = typeof(UnoProgressRing);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ProgressControl = GetTemplateChild(nameof(ProgressControl)) as
#if WINDOWS_UWP
                muxc.ProgressRing;
#else
                ProgressRing;
#endif
        }
    }
}
