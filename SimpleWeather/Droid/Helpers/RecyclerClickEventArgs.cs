#if __ANDROID__
using System;
using Android.Views;

namespace SimpleWeather.Droid.Helpers
{
    public class RecyclerClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
#endif