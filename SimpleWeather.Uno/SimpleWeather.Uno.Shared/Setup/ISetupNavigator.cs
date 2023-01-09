using SimpleWeather.ComponentModel;

namespace SimpleWeather.Uno.Setup
{
    public interface ISetupNavigator : IViewModelProvider
    {
        void Back();
        void Next();
    }
}