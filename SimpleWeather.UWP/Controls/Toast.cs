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
        Short = 3500,
        Long = 5000
    }

    public class Toast
    {
        public static async Task ShowToastNotifcation(String Message, ToastDuration Length)
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

        public static async Task ShowToastAsync(String Message, ToastDuration Length)
        {
            CoreDispatcher Dispatcher;

            if (CoreWindow.GetForCurrentThread() == null)
                Dispatcher = CoreApplication.MainView.Dispatcher;
            else
                Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                new Coding4Fun.Toolkit.Controls.ToastPrompt()
                {
                    Message = Message,
                    MillisecondsUntilHidden = (int)Length,
                    Background = new SolidColorBrush(Colors.SlateGray),
                    Foreground = new SolidColorBrush(Colors.White),
                    IsHitTestVisible = false,
                    Margin = new Thickness(0, 0, 0, 50),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Stretch = Stretch.Uniform,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Template = App.Current.Resources["ToastPromptStyle"] as ControlTemplate
                }.Show();
            });
        }
    }
}
