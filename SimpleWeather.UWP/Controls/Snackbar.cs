using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Controls
{
    public enum SnackbarDuration
    {
        Short = 3500,
        Long = 5000
    }

    public class Snackbar
    {
        private String Message;
        private int Duration;
        private Grid RootGrid;
        private InAppNotification InAppNotification;
        private Action ButtonAction;
        private String ButtonLabel;

        public static Snackbar Make(Grid Grid, String Message, SnackbarDuration Length)
        {
            Snackbar snack = new Snackbar()
            {
                InAppNotification = new InAppNotification()
                {
                    Style = Application.Current.Resources["MSEdgeNotificationStyle"] as Style
                },
                RootGrid = Grid,
                Message = Message,
                Duration = (int)Length
            };
            snack.InAppNotification.Closed += (sender, e) =>
            {
                snack.RootGrid.Children.Remove(snack.InAppNotification);
            };

            return snack;
        }

        public void SetAction(String ButtonLabel, Action Action)
        {
            this.ButtonLabel = ButtonLabel;
            this.ButtonAction = Action;
        }

        public void Show()
        {
            RootGrid.Children.Add(InAppNotification);
            Grid.SetRow(InAppNotification, 1);

            /* Addition: set snackbar width based on screen width */
            var ViewWidth = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            if (ViewWidth <= 640)
                InAppNotification.MinWidth = ViewWidth;
            else if (ViewWidth <= 1080)
                InAppNotification.MinWidth = ViewWidth * (0.75);
            else
                InAppNotification.MinWidth = ViewWidth * (0.50);

            if (!String.IsNullOrWhiteSpace(ButtonLabel) || ButtonAction != null)
            {
                var grid = new Grid();

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Text part
                var textBlock = new TextBlock
                {
                    Text = Message,
                    VerticalAlignment = VerticalAlignment.Center,
                    Style = (Application.Current.Resources["CaptionTextBlockStyle"] as Style),
                    TextWrapping = TextWrapping.WrapWholeWords
                };
                grid.Children.Add(textBlock);

                // Buttons part
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(4, 0, 4, 0)
                };

                var actionButton = new Button
                {
                    Content = ButtonLabel.ToUpper(),
                    FontWeight = Windows.UI.Text.FontWeights.SemiBold,
                    Height = 30,
                    Background = new SolidColorBrush(Colors.Transparent),
                    Foreground = InAppNotification.BorderBrush,
                    Style = (Application.Current.Resources["TextButtonStyle"] as Style),
                };
                actionButton.Click += (s, e) => ButtonAction?.Invoke();
                stackPanel.Children.Add(actionButton);

                Grid.SetColumn(stackPanel, 1);
                grid.Children.Add(stackPanel);

                InAppNotification?.Show(grid, Duration);
            }
            else
            {
                InAppNotification?.Show(Message, Duration);
            }
        }
    }
}
