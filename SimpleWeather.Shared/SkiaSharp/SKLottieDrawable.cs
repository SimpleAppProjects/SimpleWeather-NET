#if WINUI
using Microsoft.UI.Dispatching;
#else
using Microsoft.Maui.Dispatching;
#endif
using SkiaSharp;
using SkiaSharp.Skottie;
using System;
using System.Diagnostics;

namespace SimpleWeather.SkiaSharp
{
    public class SKLottieDrawable : SKDrawable, ISKAnimatable, IDisposable
    {
        private readonly Animation animation;
#if WINUI
        private readonly DispatcherQueueTimer timer;
#else
        private readonly IDispatcherTimer timer;
#endif
        private readonly Stopwatch watch = new();
        private bool disposedValue;

        public bool IsRunning => timer.IsRunning;
        public bool IsRepeating { get => timer.IsRepeating; set => timer.IsRepeating = value; }

        public event EventHandler InvalidateDrawable;

        public SKLottieDrawable(Animation animation)
        {
            this.animation = animation;

#if WINUI
            timer = SharedModule.Instance.DispatcherQueue.CreateTimer();
#else
            timer = SharedModule.Instance.Dispatcher.CreateTimer();
#endif
            timer.IsRepeating = true;
            timer.Tick += (s, e) =>
            {
                InvalidateDrawable?.Invoke(this, EventArgs.Empty);
            };
            timer.Interval = TimeSpan.FromSeconds(1 / animation.Fps);
        }

        public void Start()
        {
            timer.Start();
            watch.Start();
        }

        public void Stop()
        {
            watch.Stop();
            timer.Stop();
        }

        public override void Draw(SKCanvas canvas)
        {
            if (animation != null)
            {
                animation?.SeekFrameTime(watch.Elapsed.TotalSeconds);

                if (watch.Elapsed > animation.Duration)
                {
                    watch.Restart();
                }

                animation?.Render(canvas, Bounds);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    animation?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SKLottieDrawable()
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

    public interface ISKAnimatable
    {
        void Start();
        void Stop();
        bool IsRunning { get; }
    }
}
