using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace SimpleWeather.UWP.MaterialIcons
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
