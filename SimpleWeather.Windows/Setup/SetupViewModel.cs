using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.ComponentModel;

namespace SimpleWeather.NET.Setup
{
#if WINDOWS
    [WinRT.GeneratedBindableCustomProperty]
#endif
    public partial class SetupViewModel : BaseViewModel
    {
        [ObservableProperty]
        public partial LocationData.LocationData LocationData { get; set; } = null;
    }
}
