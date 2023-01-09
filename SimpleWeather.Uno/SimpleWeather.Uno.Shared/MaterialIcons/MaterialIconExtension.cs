using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace SimpleWeather.Uno.MaterialIcons
{
    [MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
    public class MaterialIconExtension : TextIconExtension
    {
        public MaterialSymbol Symbol { get; set; }

        protected override object ProvideValue()
        {
            var fontIcon = new MaterialIcon
            {
                Symbol = Symbol,
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                IsTextScaleFactorEnabled = IsTextScaleFactorEnabled,
                MirroredWhenRightToLeft = MirroredWhenRightToLeft
            };

            if (FontSize > 0)
            {
                fontIcon.FontSize = FontSize;
            }

            if (Foreground != null)
            {
                fontIcon.Foreground = Foreground;
            }

            return fontIcon;
        }
    }
}
