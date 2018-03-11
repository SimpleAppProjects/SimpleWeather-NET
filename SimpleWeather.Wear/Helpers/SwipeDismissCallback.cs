using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Wear.Widget;
using Android.Views;
using Android.Widget;

namespace SimpleWeather.Droid.Wear.Helpers
{
    public class SwipeDismissCallback : SwipeDismissFrameLayout.Callback
    {
        public Action<SwipeDismissFrameLayout> Dismissed;
        public Action<SwipeDismissFrameLayout> SwipeCanceled;
        public Action<SwipeDismissFrameLayout> SwipeStarted;

        public override void OnDismissed(SwipeDismissFrameLayout layout)
        {
            Dismissed?.Invoke(layout);
        }

        public override void OnSwipeCanceled(SwipeDismissFrameLayout layout)
        {
            SwipeCanceled?.Invoke(layout);
        }

        public override void OnSwipeStarted(SwipeDismissFrameLayout layout)
        {
            SwipeStarted?.Invoke(layout);
        }
    }
}