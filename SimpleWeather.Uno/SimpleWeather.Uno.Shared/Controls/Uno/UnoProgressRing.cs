using Microsoft.UI.Xaml.Controls;
using muxc = Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.Uno.Controls
{
    /// <summary>
    /// ProgressRing control. Uses native control for all platforms except Windows
    /// </summary>
    public sealed partial class UnoProgressRing :
#if WINDOWS
        muxc.ProgressRing
#else
        ProgressRing
#endif
    {
#if !WINDOWS
        public bool IsIndeterminate { get; set; } // Does nothing
#endif

        public UnoProgressRing() : base() { }
    }
}
