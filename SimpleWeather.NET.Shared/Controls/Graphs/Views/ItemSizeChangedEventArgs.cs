#if WINDOWS
using Microsoft.UI.Xaml;
#endif

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls.Graphs
{
    public class ItemSizeChangedEventArgs :
#if WINDOWS
        RoutedEventArgs
#else
        EventArgs
#endif
    {
        public System.Drawing.Size NewSize { get; set; }
    }
}