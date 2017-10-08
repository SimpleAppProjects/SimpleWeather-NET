using System;
using Android.Content;
using Android.Util;
using Android.Support.V7.Widget;
using Android.Content.Res;
using Android.Graphics;

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
            AssetManager am = context.ApplicationContext.Assets;

            Typeface typeface = null;

            try
            {
                typeface = Typeface.CreateFromAsset(am, "weathericons/weathericons-regular-webfont.ttf");
            }
            catch (Exception)
            {
                typeface = Typeface.Default;
            }

            Typeface = typeface;
        }
    }
}