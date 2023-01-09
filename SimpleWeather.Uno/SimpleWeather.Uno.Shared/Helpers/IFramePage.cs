using Microsoft.UI.Xaml.Navigation;

namespace SimpleWeather.Uno.Helpers
{
    public interface IFrameContentPage
    {
        void OnNavigatedFromPage(NavigationEventArgs e);
        void OnNavigatedToPage(NavigationEventArgs e);
        void OnNavigatingFromPage(NavigatingCancelEventArgs e);
    }
}
