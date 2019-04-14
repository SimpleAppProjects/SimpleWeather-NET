using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Controls
{
    public enum ToastDuration
    {
        Short = 2000,
        Long = 3500
    }

    public static class Toast
    {
        private static async Task ShowToastNotifcation(String Message, ToastDuration Length)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = Message
                            }
                        }
                    }
                },
                Audio = new ToastAudio()
                {
                    Silent = true
                }
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml())
            {
                ExpirationTime = DateTimeOffset.Now.AddMilliseconds((double)Length),
                Tag = toastContent.GetHashCode().ToString()
            };

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);

            // Remove notification after delay
            await Task.Delay((int)Length);
            ToastNotificationManager.History.Remove(toastNotif.Tag);
        }

        /* NOT USING THIS ANYMORE
        public static async Task ShowToastNotifcation(String Message, String ActionLabel, String ActionTag, String PageTag, ToastDuration Length)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = Message
                            }
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton(ActionLabel, string.Format("action={0}&page={1}", ActionTag, PageTag))
                        {
                            ActivationType = ToastActivationType.Foreground
                        }
                    }
                },
                Audio = new ToastAudio()
                {
                    Silent = true
                }
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml())
            {
                ExpirationTime = DateTimeOffset.Now.AddMilliseconds((double)Length),
                Tag = toastContent.GetHashCode().ToString()
            };

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);

            // Remove notification after delay
            await Task.Delay((int)Length);
            ToastNotificationManager.History.Remove(toastNotif.Tag);
        }
        */

        public static async Task ShowToastAsync(String Message, ToastDuration Length)
        {
            Panel Root = null;
            if (await DispatcherHelper.ExecuteOnUIThreadAsync(() => 
            {
                if (App.RootFrame.Content is Shell)
                    Root = Shell.Instance.Content as Panel;
                else
                    Root = (App.RootFrame.Content as UserControl)?.Content as Panel;

                return !App.IsInBackground && Root != null;
            }))
            {
                CoreDispatcher Dispatcher;

                if (CoreWindow.GetForCurrentThread() == null)
                    Dispatcher = CoreApplication.MainView.Dispatcher;
                else
                    Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Snackbar.Make(Root, Message, (SnackbarDuration)Length, SnackBarStackMode.Replace).Show();
                });
            }
            else
            {
                await ShowToastNotifcation(Message, Length);
            }
        }
    }
}
