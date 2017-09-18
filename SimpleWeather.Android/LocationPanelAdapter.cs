using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using SimpleWeather.Droid.Controls;
using SimpleWeather.Droid.Helpers;
using SimpleWeather.Utils;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Android.Support.V4.Content;
using Com.Bumptech.Glide;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace SimpleWeather.Droid
{
    public class LocationPanelAdapter : RecyclerView.Adapter, IItemTouchHelperAdapter
    {
        private ObservableCollection<LocationPanelViewModel> mDataset;
        private RequestManager Glide;

        public List<LocationPanelViewModel> Dataset { get { return mDataset.ToList(); } }

        // Events
        public event EventHandler<RecyclerClickEventArgs> ItemClick;
        public event EventHandler<RecyclerClickEventArgs> ItemLongClick;
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

        // Provide a reference to the views for each data item
        // Complex data items may need more than one view per item, and
        // you provide access to all the views for a data item in a view holder
        class ViewHolder : RecyclerView.ViewHolder
        {
            public LocationPanel mLocView;
            public ImageView mBgImageView;
            public ViewHolder(LocationPanel v,
                Action<RecyclerClickEventArgs> clickListener, Action<RecyclerClickEventArgs> longClickListener)
                : base(v)
            {
                mLocView = v;
                mBgImageView = v.FindViewById<ImageView>(Resource.Id.image_view);

                mLocView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
                mLocView.LongClick += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
            }
        }

        // Provide a suitable constructor (depends on the kind of dataset)
        public LocationPanelAdapter(RequestManager glide, List<LocationPanelViewModel> myDataset)
        {
            Glide = glide;
            mDataset = new ObservableCollection<LocationPanelViewModel>(myDataset);
            mDataset.CollectionChanged += (sender, e) => CollectionChanged?.Invoke(sender, e);
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // create a new view
            LocationPanel v = new LocationPanel(parent.Context);
            // set the view's size, margins, paddings and layout parameters
            RecyclerView.LayoutParams layoutParams = new RecyclerView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.SetMargins(0, 10, 0, 10); // l, t, r, b
            v.LayoutParameters = layoutParams;
            return new ViewHolder(v, OnClick, OnLongClick);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // - get element from your dataset at this position
            // - replace the contents of the view with that element
            ViewHolder vh = holder as ViewHolder;
            Context context = vh.mLocView.Context;
            LocationPanelViewModel panelView = mDataset[position];

            // Background
            Glide.Load(panelView.Background)
                 .CenterCrop()
                 .Placeholder(new ColorDrawable(new Color(ContextCompat.GetColor(context, Resource.Color.colorPrimary))))
                 .Into(vh.mBgImageView);

            vh.mLocView.SetWeather(panelView);
        }

        // Return the size of your dataset (invoked by the layout manager)
        public override int ItemCount => mDataset.Count;

        public void Add(int position, LocationPanelViewModel item)
        {
            mDataset.Insert(position, item);
            NotifyItemInserted(position);
        }

        public void Remove(int position)
        {
            mDataset.RemoveAt(position);
            NotifyItemRemoved(position);
        }

        public void RemoveAll()
        {
            mDataset.Clear();
            NotifyDataSetChanged();
        }

        public LocationPanelViewModel GetPanelViewModel(int position)
        {
            return mDataset[position];
        }

        private void RemoveLocation(int position)
        {
            // Remove location from list
            Settings.LocationData.RemoveAt(position);
            Settings.SaveLocationData();

            // Remove panel
            Remove(position);
        }

        public void OnItemMove(int fromPosition, int toPosition)
        {
            // Move data in both location dictionary and local dataset
            var location = Settings.LocationData[fromPosition];
            Settings.LocationData.RemoveAt(fromPosition);
            Settings.LocationData.Insert(toPosition, location);

            mDataset.Move(fromPosition, toPosition);
            NotifyItemMoved(fromPosition, toPosition);
        }

        public void OnItemMoved(int fromPosition, int toPosition)
        {
            NotifyItemChanged(fromPosition);
            NotifyItemChanged(toPosition);
        }

        public void OnItemDismiss(int position)
        {
            RemoveLocation(position);
        }

        protected void OnClick(RecyclerClickEventArgs args) => ItemClick?.Invoke(this, args);
        protected void OnLongClick(RecyclerClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }
}