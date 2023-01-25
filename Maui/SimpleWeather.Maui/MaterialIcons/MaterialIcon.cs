namespace SimpleWeather.Maui.MaterialIcons
{
    public sealed class MaterialIcon : FontImageSource
    {
        public const string FONT_ALIAS = "MaterialIcons";

        public MaterialSymbol Symbol
        {
            get { return (MaterialSymbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        public static readonly BindableProperty SymbolProperty =
            BindableProperty.Create("Symbol", typeof(MaterialSymbol), typeof(MaterialIcon), MaterialSymbol.ChevronLeft, propertyChanged: OnBindingPropertyChanged);

        public MaterialIcon() : this(MaterialSymbol.ChevronLeft)
        {
        }

        public MaterialIcon(MaterialSymbol symbol)
        {
            FontFamily = FONT_ALIAS;

            this.Glyph = new string((char)symbol, 1);
        }

        private static void OnBindingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MaterialIcon matIcon)
            {
                matIcon.Glyph = new string((char)matIcon.Symbol, 1);
            }
        }
    }
}
