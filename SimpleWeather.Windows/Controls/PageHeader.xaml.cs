using SimpleWeather.NET.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    public sealed partial class PageHeader : UserControl
    {
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(PageHeader), new PropertyMetadata(null));

        public ICollection<ICommandBarElement> Commands
        {
            get { return (ICollection<ICommandBarElement>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); UpdateCommandBar(); }
        }

        // Using a DependencyProperty as the backing store for Commands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandsProperty =
            DependencyProperty.Register("Commands", typeof(ICollection<ICommandBarElement>), typeof(PageHeader), new PropertyMetadata(null));

        public PageHeader()
        {
            this.InitializeComponent();
        }

        private void UpdateCommandBar()
        {
            CommandBar.PrimaryCommands.Clear();

            if (Commands != null)
            {
                foreach (var cmd in Commands)
                {
                    CommandBar.PrimaryCommands.Add(cmd);
                }
            }
        }
    }
}
