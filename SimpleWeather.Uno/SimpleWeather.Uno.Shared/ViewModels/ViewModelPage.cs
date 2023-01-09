using SimpleWeather.ComponentModel;
using SimpleWeather.Uno.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SimpleWeather.Uno.ViewModels
{
    public abstract partial class ViewModelPage : ScopePage, IViewModelProvider
    {
        private readonly string DefaultKey;

        public ViewModelStore ViewModelStore { get; } = new();

        public ViewModelPage()
        {
            DefaultKey = $"{this.GetType().FullName}.DefaultKey";
        }

        public T GetViewModel<T>() where T : IViewModel
        {
            return this.GetViewModel<T>(DefaultKey + ":" + typeof(T).FullName);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModelStore.Clear();
        }
    }
}
