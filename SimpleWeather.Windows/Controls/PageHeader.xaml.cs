using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    public sealed partial class PageHeader : UserControl
    {
        public object Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(object), typeof(PageHeader), new PropertyMetadata(null));

        public ICollection<ICommandBarElement> Commands
        {
            get => (ICollection<ICommandBarElement>)GetValue(CommandsProperty);
            set => SetValue(CommandsProperty, value);
        }

        // Using a DependencyProperty as the backing store for Commands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandsProperty =
            DependencyProperty.Register(nameof(Commands), typeof(ICollection<ICommandBarElement>), typeof(PageHeader), new PropertyMetadata(null, (o, e) => (o as PageHeader)?.UpdateCommandBar()));

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
