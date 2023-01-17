using SimpleWeather.ComponentModel;

namespace SimpleWeather.NET.Setup
{
    public interface ISetupNavigator : IViewModelProvider
    {
        void Back();
        void Next();
    }
}