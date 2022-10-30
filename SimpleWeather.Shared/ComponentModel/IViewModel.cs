using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using System.ComponentModel;
using Windows.System;

namespace SimpleWeather.ComponentModel
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void OnCleared();
    }

    public abstract class BaseViewModel : ObservableObject, IViewModel
    {
        protected DispatcherQueue DispatcherQueue { get; } = DispatcherQueue.GetForCurrentThread();

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            DispatcherQueue.EnqueueAsync(() =>
            {
                base.OnPropertyChanged(e);
            });
        }

        public virtual void OnCleared()
        {
        }
    }
}
