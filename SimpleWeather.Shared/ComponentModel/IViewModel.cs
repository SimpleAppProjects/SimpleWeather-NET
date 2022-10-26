using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace SimpleWeather.ComponentModel
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void OnCleared();
    }

    public abstract class BaseViewModel : ObservableObject, IViewModel
    {
        public void OnCleared()
        {
        }
    }
}
