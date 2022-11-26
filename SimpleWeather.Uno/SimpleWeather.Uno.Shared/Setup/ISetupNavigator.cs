using SimpleWeather.ComponentModel;

namespace SimpleWeather.UWP.Setup
{
    public interface ISetupNavigator : IViewModelProvider
    {
        void Back();
        void Next();
    }
}