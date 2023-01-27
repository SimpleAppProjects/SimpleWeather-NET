using Android.Content;
using AndroidX.Annotations;

namespace SimpleWeather.Maui.Platforms.Android
{
    public static class ContextUtils
    {
        [ColorInt]
        public static int GetAttrColor(this Context context, [AttrRes] int resId)
        {
            var array = context.Theme.ObtainStyledAttributes(new int[1] { resId });
            var color = array.GetColor(0, 0);
            array.Recycle();
            return color;
        }
    }
}
