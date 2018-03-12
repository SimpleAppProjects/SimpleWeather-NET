#if __ANDROID__
using System;

using Android.Content;
using Android.Util;
using Android.Support.V7.Widget;
using Android.Support.V4.Content.Res;

namespace SimpleWeather.Droid.Controls
{
    public class WeatherIcon : AppCompatTextView
    {
        public WeatherIcon(Context context)
            : base(context)
        {
            Initialize(context);
        }

        public WeatherIcon(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context);
        }

        public WeatherIcon(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            Typeface = ResourcesCompat.GetFont(context, Resource.Font.weathericons);
        }
    }
}
#endif