using Windows.UI.Xaml.Navigation;

namespace SimpleWeather.UWP.Helpers
{
    public interface IFrameContentPage
    {
        void OnNavigatedFromPage(NavigationEventArgs e);
        void OnNavigatedToPage(NavigationEventArgs e);
        void OnNavigatingFromPage(NavigatingCancelEventArgs e);
    }
}
