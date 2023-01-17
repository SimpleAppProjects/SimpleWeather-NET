using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.ComponentModel;

namespace SimpleWeather.NET.Setup
{
    public partial class SetupViewModel : BaseViewModel
    {
        [ObservableProperty]
        private LocationData.LocationData locationData = null;
    }
}
