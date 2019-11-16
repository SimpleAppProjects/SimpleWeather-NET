using SimpleWeather.Controls;
using SimpleWeather.Utils;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class DetailItem : UserControl
    {
        public DetailItemViewModel Details
        {
            get { return (this.DataContext as DetailItemViewModel); }
        }

        public static readonly DependencyProperty ItemColorProperty =
            DependencyProperty.Register("ItemColor", typeof(Color),
            typeof(DetailItem), new PropertyMetadata(Color.FromArgb(0xB3, 0xff, 0xff, 0xff)));
        public Color ItemColor
        {
            get
            {
                Color color = (Color)GetValue(ItemColorProperty);
                color.A = 0xB3;
                return color;
            }
            set { SetValue(ItemColorProperty, value); }
        }

        public DetailItem()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();
            };
            RegisterPropertyChangedCallback(ItemColorProperty, DetailItem_PropertyChangedCallback);

            if (ItemBorder.Background == null) ItemBorder.Background = new SolidColorBrush();
            if (ItemBorder.BorderBrush == null) ItemBorder.BorderBrush = new SolidColorBrush();
        }

        private void DetailItem_PropertyChangedCallback(DependencyObject sender, DependencyProperty property)
        {
            if (property == ItemColorProperty)
            {
                bool isLightBackground = ColorUtils.IsSuperLight(ItemColor);
                var colorBrush = ItemBorder.Background as SolidColorBrush;
                var borderBrush = ItemBorder.BorderBrush as SolidColorBrush;

                switch (Settings.UserTheme)
                {
                    case Helpers.UserThemeMode.System:
                        if (App.IsSystemDarkTheme)
                            goto case Helpers.UserThemeMode.Dark;
                        else
                            goto case Helpers.UserThemeMode.Light;
                    case Helpers.UserThemeMode.Light:
                        colorBrush.Color = isLightBackground ? ItemColor : ColorUtils.BlendColor(ItemColor, Colors.White, 0.25f);
                        Control.Foreground = new SolidColorBrush(isLightBackground ? Colors.Black : Colors.White);
                        borderBrush.Color = ColorUtils.SetAlphaComponent(isLightBackground ? Colors.Black : Colors.LightGray, 0x40);
                        break;
                    case Helpers.UserThemeMode.Dark:
                        colorBrush.Color = ColorUtils.BlendColor(ItemColor, Colors.Black, 0.25f);
                        Control.Foreground = new SolidColorBrush(isLightBackground ? Colors.Black : Colors.White);
                        borderBrush.Color = ColorUtils.SetAlphaComponent(isLightBackground ? Colors.Black : Colors.LightGray, 0x40);
                        break;
                }
            }
        }
    }
}
