using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Threading.Tasks;
using Windows.UI.Notifications;

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
    }
}
