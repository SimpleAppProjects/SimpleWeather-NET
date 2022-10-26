using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace SimpleWeather.ComponentModel
{
    public interface IViewModelProvider
    {
        public ViewModelStore ViewModelStore { get; }
        public T GetViewModel<T>() where T : IViewModel;
    }

    public class ViewModelStore
    {
        private readonly Dictionary<string, IViewModel> _viewModelStore = new();

        internal void Add(string key, IViewModel viewModel)
        {
            if (_viewModelStore.ContainsKey(key))
            {
                _viewModelStore.Remove(key, out IViewModel oldViewModel);
                oldViewModel?.OnCleared();
            }
            _viewModelStore.Add(key, viewModel);
        }

        internal IViewModel this[string key]
        {
            get
            {
                if (_viewModelStore.TryGetValue(key, out IViewModel viewModel))
                {
                    return viewModel;
                }

                return null;
            }
            set => Add(key, value);
        }

        public void Clear()
        {
            foreach (var vm in _viewModelStore.Values)
            {
                vm.OnCleared();
            }

            _viewModelStore.Clear();
        }
    }

    public static class ViewModelStoreExtensions
    {
        public static T GetViewModel<T>(this IViewModelProvider provider, string key) where T : IViewModel
        {
            var viewModel = provider.ViewModelStore[key];
            if (viewModel is T)
            {
                return (T)viewModel;
            }

            viewModel = ActivatorUtilities.CreateInstance<T>(Ioc.Default);
            provider.ViewModelStore[key] = viewModel;

            return (T)viewModel;
        }
    }
}
