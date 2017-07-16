using SimpleWeather.Utils;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            RestoreSettings();
        }

        private void RestoreSettings()
        {
            // Temperature
            if (Settings.Unit == Settings.Fahrenheit)
            {
                Fahrenheit.IsChecked = true;
                Celsius.IsChecked = false;
            }
            else
            {
                Fahrenheit.IsChecked = false;
                Celsius.IsChecked = true;
            }

            Version.Text = string.Format("v{0}.{1}.{2}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

            OSSLicenseWebview.NavigationStarting += OSSLicenseWebview_NavigationStarting;
        }

        private void OSSLicenseWebview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // Cancel navigation and open link in ext. browser
            args.Cancel = true;
            Windows.Foundation.IAsyncOperation<bool> b = Windows.System.Launcher.LaunchUriAsync(args.Uri);
        }

        private void Fahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Unit = Settings.Fahrenheit;
        }

        private void Celsius_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Unit = Settings.Celsius;
        }
    }
}
