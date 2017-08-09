using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Kimono.Controls.SnackBar
{
    public class SnackBarManager
    {
        private class SnackBarMessageRequest
        {
            public Action<SnackBarMessage> ButtonCallback { get; internal set; }
            public string ButtonText { get; internal set; }
            public Visibility ButtonVisibility { get; internal set; }
            public TaskCompletionSource<object> TaskSource { get; internal set; }
            public string Text { get; internal set; }
            public int TimeToShow { get; internal set; }
        }

        public event EventHandler NotificationShown;

        private Grid desiredSnackBarAreaGrid  = null;
        private ConcurrentQueue<SnackBarMessageRequest> snackQueue = null;
        private volatile bool queueRunning = false;
        public SnackBarManager(Grid snackBarAreaGrid)
        {
            if (snackBarAreaGrid == null) throw new ArgumentNullException(nameof(snackBarAreaGrid));

            desiredSnackBarAreaGrid = snackBarAreaGrid;
            snackQueue = new ConcurrentQueue<SnackBarMessageRequest>();
        }
        public Task ShowMessageAsync(string message, int msToBeVisible = 10000)
        {
            TaskCompletionSource<object> taskSource = new TaskCompletionSource<object>();

            var request = new SnackBarMessageRequest()
            {
                Text = message,
                TimeToShow = msToBeVisible,
                TaskSource = taskSource,
                ButtonVisibility = Visibility.Collapsed
            };
            snackQueue.Enqueue(request);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            RunQueueAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return taskSource.Task;
        }

        public Task ShowMessageWithCallbackAsync(string message, string buttonText, Action<SnackBarMessage> callback, int msToBeVisible = 10000)
        {
            TaskCompletionSource<object> taskSource = new TaskCompletionSource<object>();

            var request = new SnackBarMessageRequest()
            {
                Text = message,
                TimeToShow = msToBeVisible,
                TaskSource = taskSource,

                //too lazy for dependency properties
                ButtonVisibility = Visibility.Visible,
                ButtonText = buttonText,
                ButtonCallback = callback
            };
            snackQueue.Enqueue(request);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            RunQueueAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return taskSource.Task;
        }

        private async Task RunQueueAsync()
        {
            if (queueRunning) return;

            queueRunning = true;

            while(snackQueue.Count > 0)
            {
                SnackBarMessageRequest request = null;

                if (snackQueue.TryDequeue(out request))
                {
                    var popupTask = DoPopupAsync(request);

                    if (NotificationShown != null)
                        NotificationShown(this, EventArgs.Empty);

                    await popupTask;
                }
            }

            queueRunning = false;
        }

        private Task DoPopupAsync(SnackBarMessageRequest snackRequest)
        {
            TaskCompletionSource<object> popupTask = new TaskCompletionSource<object>();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(async () =>
            {
                var msgControl = new SnackBarMessage()
                {
                    Text = snackRequest.Text,
                    ButtonVisibility = snackRequest.ButtonVisibility,
                    ButtonText = snackRequest.ButtonText,
                    ButtonCallback = snackRequest.ButtonCallback,
                    TimeToShow = snackRequest.TimeToShow
                };

                /* Addition: set snackbar width based on screen width */
                var ViewWidth = ApplicationView.GetForCurrentView().VisibleBounds.Width;

                if (ViewWidth <= 640)
                    msgControl.Width = ViewWidth;
                else if (ViewWidth <= 1080)
                    msgControl.Width = ViewWidth * (0.75);
                else
                    msgControl.Width = ViewWidth * (0.50);

                desiredSnackBarAreaGrid.Children.Add(msgControl);

                /* Addition: move snackbar to bottom of page */
                Grid.SetRowSpan(msgControl, desiredSnackBarAreaGrid.RowDefinitions.Count);
                msgControl.VerticalAlignment = VerticalAlignment.Bottom;

                if (msgControl.TimeToShow > 0)
                {
                    var timeDelay = Task.Delay(msgControl.TimeToShow);
                    if (msgControl.ButtonVisibility == Visibility.Collapsed)
                    {
                        await timeDelay;
                    }
                    else
                    {
                        var buttonTask = msgControl.WaitForButtonClickAsync();
                        if (await Task.WhenAny(timeDelay, buttonTask) == timeDelay)
                        {
                            //todo break down buttonTask to prevent memory leaks
                        }
                    }
                }

                desiredSnackBarAreaGrid.Children.Remove(msgControl);

                snackRequest.TaskSource.SetResult(null); //allow the requester to continue
                popupTask.SetResult(null); //allow the queue to continue
            }));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return popupTask.Task;
        }

        private T FindChildOfType<T>(UIElement element) where T : UIElement
        {
            if (element is T) return (T)element;

            if (element is ContentControl)
            {
                if (((ContentControl)element).Content != null)
                    return FindChildOfType<T>(((ContentControl)element).Content as UIElement);
            }
            else if (element is Page)
            {
                if (((Page)element).Content != null)
                    return FindChildOfType<T>(((Page)element).Content as UIElement);
            }

            return null;
        }
    }
}
