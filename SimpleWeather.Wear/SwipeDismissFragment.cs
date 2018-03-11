using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Wear.Widget;
using Android.Views;
using Android.Widget;
using SimpleWeather.Droid.Wear.Helpers;

namespace SimpleWeather.Droid.Wear
{
    public class SwipeDismissFragment : Fragment
    {
        private SwipeDismissCallback swipeCallback;
        private SwipeDismissFrameLayout swipeLayout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            swipeLayout = (SwipeDismissFrameLayout)inflater.Inflate(Resource.Layout.activity_settings, container, false);

            swipeLayout.Swipeable = true;
            swipeCallback = new SwipeDismissCallback();
            swipeCallback.Dismissed += (layout) =>
            {
                Activity.OnBackPressed();
            };
            swipeLayout.AddCallback(swipeCallback);

            return swipeLayout;
        }

        public override void OnDestroyView()
        {
            swipeLayout.RemoveCallback(swipeCallback);
            base.OnDestroyView();
        }
    }

    public class SwipeDismissPreferenceFragment : PreferenceFragment
    {
        private SwipeDismissCallback swipeCallback;
        private SwipeDismissFrameLayout swipeLayout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            swipeLayout = (SwipeDismissFrameLayout)inflater.Inflate(Resource.Layout.activity_settings, container, false);

            View inflatedView = base.OnCreateView(inflater, container, savedInstanceState);
            swipeLayout.AddView(inflatedView);
            swipeLayout.Swipeable = true;
            swipeCallback = new SwipeDismissCallback();
            swipeCallback.Dismissed += (layout) =>
            {
                Activity.OnBackPressed();
            };
            swipeLayout.AddCallback(swipeCallback);

            return swipeLayout;
        }

        public override void OnDestroyView()
        {
            swipeLayout.RemoveCallback(swipeCallback);
            base.OnDestroyView();
        }
    }
}