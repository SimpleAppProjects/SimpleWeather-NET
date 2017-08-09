using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Controls
{
    public enum SnackbarDuration
    {
        Short = 3500,
        Long = 5000
    }

    public class Snackbar
    {
        private Kimono.Controls.SnackBar.SnackBarManager snackMan;
        private String Message;
        private int Duration;
        private Action<Kimono.Controls.SnackBar.SnackBarMessage> ButtonAction;
        private String ButtonLabel;

        public static Snackbar Make(Grid Grid, String Message, SnackbarDuration Length)
        {
            Snackbar snack = new Snackbar()
            {
                snackMan = new Kimono.Controls.SnackBar.SnackBarManager(Grid),
                Message = Message,
                Duration = (int)Length
            };
            return snack;
        }

        public void SetAction(String ButtonLabel, Action<Kimono.Controls.SnackBar.SnackBarMessage> Action)
        {
            this.ButtonLabel = ButtonLabel;
            this.ButtonAction = Action;
        }

        public void Show()
        {
            if (!String.IsNullOrWhiteSpace(ButtonLabel) || ButtonAction != null)
            {
                snackMan.ShowMessageWithCallbackAsync(Message, ButtonLabel, ButtonAction, Duration);
            }
            else
            {
                snackMan.ShowMessageAsync(Message, Duration);
            }
        }
    }
}
