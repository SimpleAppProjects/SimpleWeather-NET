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
using Android.Support.V7.Widget;
using SimpleWeather.Droid.Controls;

namespace SimpleWeather.Droid
{
    public class LocationPanelAdapter : RecyclerView.Adapter
    {
        private List<LocationPanelView> mDataset;

        public List<LocationPanelView> Dataset { get { return mDataset; } }
        public event EventHandler<RecyclerClickEventArgs> ItemClick;
        //public event EventHandler<RecyclerClickEventArgs> ItemLongClick;

        // Provide a reference to the views for each data item
        // Complex data items may need more than one view per item, and
        // you provide access to all the views for a data item in a view holder
        class ViewHolder : RecyclerView.ViewHolder
        {
            // each data item is just a string in this case
            public LocationPanel mLocView;
            public ViewHolder(LocationPanel v, Action<RecyclerClickEventArgs> clickListener)
                : base(v)
            {
                mLocView = v;
                mLocView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
                mLocView.ContextMenuCreated += (object sender, View.CreateContextMenuEventArgs e) =>
                {
                    e.Menu.Add(Menu.None, AdapterPosition, 0, "Delete Location");
                };
            }
        }

        // Provide a suitable constructor (depends on the kind of dataset)
        public LocationPanelAdapter(List<LocationPanelView> myDataset)
        {
            mDataset = myDataset;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // create a new view
            LocationPanel v = new LocationPanel(parent.Context);
            // set the view's size, margins, paddings and layout parameters
            RecyclerView.LayoutParams layoutParams = new RecyclerView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.SetMargins(0, 5, 0, 5); // l, t, r, b
            v.LayoutParameters = layoutParams;
            return new ViewHolder(v, OnClick);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // - get element from your dataset at this position
            // - replace the contents of the view with that element
            ViewHolder vh = holder as ViewHolder;
            vh.mLocView.SetWeather(mDataset[position]);
        }

        // Return the size of your dataset (invoked by the layout manager)
        public override int ItemCount => mDataset.Count;

        public void Add(int position, LocationPanelView item)
        {
            mDataset.Add(/*position, */item);
            NotifyDataSetChanged();
        }

        public void Remove(int position)
        {
            mDataset.RemoveAt(position);
            NotifyItemRemoved(position);

            // Update pair
        }

        public LocationPanelView Get(int position)
        {
            return mDataset[position];
        }

        protected void OnClick(RecyclerClickEventArgs args) => ItemClick?.Invoke(this, args);
        //protected void OnLongClick(RecyclerClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }
}