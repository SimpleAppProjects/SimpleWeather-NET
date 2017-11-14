using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Kimono.Controls.SnackBar
{
    public sealed partial class SnackBarMessage : UserControl
    {
        public SnackBarMessage()
        {
            this.InitializeComponent();

            this.Opacity = SnackBarAppearance.Opacity;
            PART_MessageBlock.FontSize = SnackBarAppearance.MessageFontSize;
            PART_CommandButton.FontSize = SnackBarAppearance.MessageFontSize;

            var transColl = new TransitionCollection();
            if (SnackBarAppearance.Transition != null)
            {
                transColl.Add(SnackBarAppearance.Transition);
            }
            else
            {
                transColl.Add(new AddDeleteThemeTransition());
            }
            this.Transitions = transColl;
        }

        public string Text
        {
            get { return PART_MessageBlock.Text; }
            set { PART_MessageBlock.Text = value; }
        }

        public int TimeToShow { get; internal set; }

        public Action<SnackBarMessage> ButtonCallback { get; internal set; }

        public string ButtonText
        {
            get { return PART_CommandButton.Content as string; }
            set { PART_CommandButton.Content = value?.ToUpper(); }
        }

        public Visibility ButtonVisibility
        {
            get { return PART_CommandButton.Visibility; }
            set { PART_CommandButton.Visibility = value; }
        }

        //Inception
        internal SnackBarMessage NextSnackBarMessage { get; set; }

        internal Task WaitForButtonClickAsync()
        {
            TaskCompletionSource<object> taskSource = new TaskCompletionSource<object>();

            RoutedEventHandler clickHandler = null;
            clickHandler = new RoutedEventHandler((sender, args) =>
            {
                PART_CommandButton.Click -= clickHandler;

                taskSource.SetResult(null);

                ButtonCallback?.Invoke(this);
            });

            PART_CommandButton.Click += clickHandler;

            return taskSource.Task;
        }
    }
}
