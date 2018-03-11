using System;
using System.Collections.Generic;
using Android.Views;
using Android.Support.V7.Widget;
#if __ANDROID_WEAR__
using SimpleWeather.Droid.Wear.Controls;
#else
using SimpleWeather.Droid.Controls;
#endif
using SimpleWeather.Controls;
using SimpleWeather.Droid.Helpers;

namespace SimpleWeather.Droid
{
    public class LocationQueryAdapter : RecyclerView.Adapter
    {
        private List<LocationQueryViewModel> mDataset;

        public List<LocationQueryViewModel> Dataset { get { return mDataset; } }
        public event EventHandler<RecyclerClickEventArgs> ItemClick;

        // Provide a reference to the views for each data item
        // Complex data items may need more than one view per item, and
        // you provide access to all the views for a data item in a view holder
        class ViewHolder : RecyclerView.ViewHolder
        {
            public LocationQuery mLocView;
            public ViewHolder(LocationQuery v, Action<RecyclerClickEventArgs> clickListener)
                : base(v)
            {
                mLocView = v;
                mLocView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
            }
        }

        // Provide a suitable constructor (depends on the kind of dataset)
        public LocationQueryAdapter(List<LocationQueryViewModel> myDataset)
        {
            mDataset = myDataset;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // create a new view
            LocationQuery v = new LocationQuery(parent.Context)
            {
                // set the view's size, margins, paddings and layout parameters
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            return new ViewHolder(v, OnClick);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // - get element from your dataset at this position
            // - replace the contents of the view with that element
            ViewHolder vh = holder as ViewHolder;
            vh.mLocView.SetLocation(mDataset[position]);
        }

        // Return the size of your dataset (invoked by the layout manager)
        public override int ItemCount => mDataset.Count;

        public void SetLocations(List<LocationQueryViewModel> myDataset)
        {
            mDataset.Clear();
            mDataset.AddRange(myDataset);
            NotifyDataSetChanged();
        }

        protected void OnClick(RecyclerClickEventArgs args) => ItemClick?.Invoke(this, args);
    }
}