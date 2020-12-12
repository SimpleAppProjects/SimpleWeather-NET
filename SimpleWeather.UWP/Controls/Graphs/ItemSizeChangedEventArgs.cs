using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls.Graphs
{
    internal class ItemSizeChangedEventArgs : RoutedEventArgs
    {
        public System.Drawing.Size NewSize { get; set; }
    }
}