using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.MaterialIcons
{
    public sealed partial class MaterialIcon : FontIcon
    {
        public const string FONT_PATH =
#if WINDOWS_UWP || HAS_UNO_SKIA || __ANDROID__
            "ms-appx:///Assets/Fonts/MaterialIcons-Regular.ttf#" +
#endif
            "Material Icons";

        public MaterialSymbol Symbol
        {
            get { return (MaterialSymbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(MaterialSymbol), typeof(MaterialIcon), new PropertyMetadata(default, OnSymbolPropertyChanged));

        public MaterialIcon() : this(MaterialSymbol.ChevronLeft)
        {
        }

        public MaterialIcon(MaterialSymbol symbol)
        {
            FontFamily = new FontFamily(FONT_PATH);

            this.Glyph = new string((char)symbol, 1);
        }

        private static void OnSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MaterialIcon matIcon)
            {
                matIcon.Glyph = new string((char)matIcon.Symbol, 1);
            }
        }
    }
}
