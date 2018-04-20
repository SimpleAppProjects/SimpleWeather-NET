#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Support.V7.Widget;
#if __ANDROID_WEAR__
using SimpleWeather.Droid.Wear.Controls;
using SimpleWeather.Droid.Wear.Helpers;
#else
using SimpleWeather.Droid.App.Controls;
using SimpleWeather.Droid.App.Helpers;
#endif
using SimpleWeather.Controls;

namespace SimpleWeather.Droid.Adapters
{
    public class WeatherAlertPanelAdapter : RecyclerView.Adapter
    {
        private List<WeatherAlertViewModel> mDataset;

        // Provide a reference to the views for each data item
        // Complex data items may need more than one view per item, and
        // you provide access to all the views for a data item in a view holder
        class ViewHolder : RecyclerView.ViewHolder
        {
            public WeatherAlertPanel mAlertPanel;
            public ViewHolder(WeatherAlertPanel v)
                : base(v)
            {
                mAlertPanel = v;
            }
        }

        // Provide a suitable constructor (depends on the kind of dataset)
        public WeatherAlertPanelAdapter(List<WeatherAlertViewModel> myDataset)
        {
            if (myDataset != null)
                mDataset = myDataset;
            else
                mDataset = new List<WeatherAlertViewModel>(0);
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // create a new view
            WeatherAlertPanel v = new WeatherAlertPanel(parent.Context)
            {
                // set the view's size, margins, paddings and layout parameters
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            return new ViewHolder(v);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // - get element from your dataset at this position
            // - replace the contents of the view with that element
            ViewHolder vh = holder as ViewHolder;
            vh.mAlertPanel.SetAlert(mDataset[position]);
        }

        // Return the size of your dataset (invoked by the layout manager)
        public override int ItemCount => mDataset.Count;
    }
}
#endif