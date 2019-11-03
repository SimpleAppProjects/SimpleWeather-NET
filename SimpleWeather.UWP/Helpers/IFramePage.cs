using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
