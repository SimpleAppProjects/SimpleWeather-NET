using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.ComponentModel;
using SimpleWeather.Location;

namespace SimpleWeather.UWP.Setup
{
    public partial class SetupViewModel : BaseViewModel
    {
        [ObservableProperty]
        private LocationData locationData = null;
    }
}
