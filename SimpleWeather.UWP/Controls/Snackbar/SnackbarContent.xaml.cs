using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class SnackbarContent : UserControl
    {
        public Snackbar SnackbarModel
        {
            get { return this.DataContext as Snackbar; }
        }

        public SnackbarContent()
        {
            this.InitializeComponent();
        }
    }
}
