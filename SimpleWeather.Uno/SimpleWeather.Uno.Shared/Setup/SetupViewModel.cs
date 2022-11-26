using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.ComponentModel;

namespace SimpleWeather.UWP.Setup
{
    public partial class SetupViewModel : BaseViewModel
    {
        [ObservableProperty]
        private LocationData.LocationData locationData = null;
    }
}
