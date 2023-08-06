using SimpleWeather.ComponentModel;
using SimpleWeather.Maui.Controls;

namespace SimpleWeather.Maui.ViewModels
{
    public abstract partial class ViewModelShell : ScopeShell, IViewModelProvider
    {
        private readonly string DefaultKey;

        public ViewModelStore ViewModelStore { get; } = new();

        public ViewModelShell()
        {
            DefaultKey = $"{this.GetType().FullName}.DefaultKey";
        }

        public T GetViewModel<T>() where T : IViewModel
        {
            return this.GetViewModel<T>(DefaultKey + ":" + typeof(T).FullName);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                ViewModelStore.Clear();
            }
        }
    }
}
