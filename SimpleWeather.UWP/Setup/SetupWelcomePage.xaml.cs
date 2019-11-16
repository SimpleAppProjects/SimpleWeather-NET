using SimpleWeather.UWP.Helpers;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupWelcomePage : Page, IPageVerification
    {
        public SetupWelcomePage()
        {
            this.InitializeComponent();
        }

        public bool CanContinue()
        {
            return true;
        }
    }
}
