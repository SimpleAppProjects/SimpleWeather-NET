using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Controls
{
    public class Toast
    {
        public enum ToastDuration
        {
            Short = 3500,
            Long = 5000
        }

        public static async Task ShowToastNotifcation(String Message, ToastDuration Length)
        {
            var xmlDoc = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            xmlDoc.GetElementsByTagName("text")[0]
                .AppendChild(xmlDoc.CreateTextNode(Message));
            var toastNode = xmlDoc.SelectSingleNode("/toast");
            var audioNode = xmlDoc.CreateElement("audio");
            audioNode.SetAttribute("silent", "true");
            toastNode.AppendChild(audioNode);

            ToastNotification not = new ToastNotification(xmlDoc);
            not.Tag = not.Content.GetHashCode().ToString();

            ToastNotificationManager.CreateToastNotifier().Show(not);
            not.ExpirationTime = DateTimeOffset.Now.AddMilliseconds((double)Length);

            await Task.Delay((int)Length);

            ToastNotificationManager.History.Remove(not.Tag);
        }

        public static void ShowToast(String Message, ToastDuration Length)
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
        }
    }
}
