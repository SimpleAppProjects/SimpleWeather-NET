using SimpleWeather.Maui.MaterialIcons;
using System.ComponentModel;

namespace SimpleWeather.Maui.MarkupExtensions
{
    public class MaterialIconExtension : IMarkupExtension<MaterialIcon>
    {
        public MaterialSymbol Symbol { get; set; }
        public Color Color { get; set; } = (Color)FontImageSource.ColorProperty.DefaultValue;

        [TypeConverter(typeof(FontSizeConverter))]
        public double Size { get; set; } = (double)FontImageSource.SizeProperty.DefaultValue;

        public MaterialIcon ProvideValue(IServiceProvider serviceProvider)
        {
            return new MaterialIcon(Symbol)
            {
                Color = Color,
                Size = Size,
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<MaterialIcon>).ProvideValue(serviceProvider);
        }
    }
}
