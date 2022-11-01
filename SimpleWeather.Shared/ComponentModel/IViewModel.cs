using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using System.ComponentModel;
using System.Threading;
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

    public abstract class ScopeViewModel : BaseViewModel
    {
        private CancellationTokenSource cts;

        protected void RefreshToken()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
        }

        protected CancellationToken GetCancellationToken()
        {
            if (cts == null || cts.Token.IsCancellationRequested)
            {
                RefreshToken();
            }

            return cts.Token;
        }

        public override void OnCleared()
        {
            base.OnCleared();

            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
    }
}
