namespace SimpleWeather.Maui.Controls
{
    public abstract partial class ScopePage : ContentPage, IDisposable
    {
        private CancellationTokenSource cts;
        private bool disposedValue;

        public ScopePage()
        {
            cts = new CancellationTokenSource();
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs e)
        {
            base.OnNavigatedTo(e);
            RefreshToken();
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs e)
        {
            base.OnNavigatedFrom(e);
            cts?.Cancel();
            cts = null;
        }

        protected void RefreshToken()
        {
            cts?.Cancel();
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    cts?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                cts = null;
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ViewModelPage()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
