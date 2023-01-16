using Microsoft.UI.Xaml.Markup;

namespace SimpleWeather.Uno.Helpers
{
    public class OnPlatform : MarkupExtension
    {
        public object Win { get; set; }
        public object Android { get; set; }
        public object iOS { get; set; }
        public object MacCatalyst { get; set; }
        public object Skia { get; set; }
        public object Default { get; set; }

        protected override object ProvideValue()
        {
#if WINDOWS || WINDOWS_UWP || NETFX_CORE
            return Win ?? Default ?? default;
#elif ANDROID
            return Android ?? Default ?? default;
#elif MACCATALYST
            return MacCatalyst ?? Default ?? default;
#elif IOS
            return iOS ?? Default ?? default;
#elif HAS_UNO_SKIA
            return Skia ?? Default ?? default;
#else
            return Default ?? default;
#endif
        }
    }
}
