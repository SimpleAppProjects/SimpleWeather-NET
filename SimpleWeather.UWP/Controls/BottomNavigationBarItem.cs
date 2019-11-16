using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.UWP.Controls
{
    public partial class BottomNavigationBarItem : ListViewItem
    {
        public BottomNavigationBarItem()
        {
            DefaultStyleKey = typeof(BottomNavigationBarItem);
        }

        /// <summary>
        /// Gets or sets the icon to appear in the navigationbar item.
        /// </summary>
        public IconElement Icon
        {
            get { return (IconElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Icon"/> dependency property.</returns>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(BottomNavigationBarItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header content for the nav item.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Label"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Label"/> dependency property.</returns>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(BottomNavigationBarItem), new PropertyMetadata(""));
    }
}
