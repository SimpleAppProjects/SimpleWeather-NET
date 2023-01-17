using CommunityToolkit.Mvvm.ComponentModel;
#if WINUI
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
#else
using Microsoft.Maui.Dispatching;
#endif
using System.ComponentModel;
using System.Threading;

namespace SimpleWeather.ComponentModel
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void OnCleared();
    }

    public abstract class BaseViewModel : ObservableObject, IViewModel
    {
#if WINUI
        protected DispatcherQueue DispatcherQueue { get; } = DispatcherQueue.GetForCurrentThread();
#else
        protected IDispatcher Dispatcher { get; } = Microsoft.Maui.Dispatching.Dispatcher.GetForCurrentThread();
#endif

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
#if WINUI
            DispatcherQueue.EnqueueAsync(() =>
#else
            Dispatcher.Dispatch(() =>
#endif
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
