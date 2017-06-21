using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SimpleWeather.Droid.Helpers
{
    public interface ItemTouchHelperAdapter
    {
        void onItemMove(int fromPosition, int toPosition);

        void onItemMoved(int fromPosition, int toPosition);

        void onItemDismiss(int position);
    }
}