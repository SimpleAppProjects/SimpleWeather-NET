#if __ANDROID__
using Android.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Droid.Helpers
{
    public class IntentEqualityComparer : IEqualityComparer<Intent>
    {
        public bool Equals(Intent x, Intent y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else if (x.FilterEquals(y))
                return true;
            else
                return false;
        }

        public int GetHashCode(Intent obj)
        {
            return obj.FilterHashCode();
        }
    }
}
#endif