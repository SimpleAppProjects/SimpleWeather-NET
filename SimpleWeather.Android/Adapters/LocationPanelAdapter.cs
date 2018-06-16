using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using SimpleWeather.Droid.Helpers;
using SimpleWeather.Droid.App.Controls;
using SimpleWeather.Droid.App.Helpers;
using SimpleWeather.Utils;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Android.Support.V4.Content;
using Com.Bumptech.Glide;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using SimpleWeather.Droid.Utils;
using Android.Util;

namespace SimpleWeather.Droid.App.Adapters
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
        private class ViewHolder : RecyclerView.ViewHolder
        {
            public ImageView mBgImageView;
            public LocationPanel mLocView;
            public ViewHolder(LocationPanel v,
                Action<RecyclerClickEventArgs> clickListener, Action<RecyclerClickEventArgs> longClickListener)
                : base(v)
            {
                mLocView = v;
                mBgImageView = v.FindViewById<ImageView>(Resource.Id.image_view);

                mLocView.Click += (sender, e) => clickListener?.Invoke(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
                mLocView.LongClick += (sender, e) => longClickListener?.Invoke(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
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
            int margin = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, parent.Context.Resources.DisplayMetrics);
            layoutParams.SetMargins(0, margin, 0, margin); // l, t, r, b
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
                 .AsBitmap()
                 .CenterCrop()
                 .Error(vh.mLocView.colorDrawable)
                 .Placeholder(vh.mLocView.colorDrawable)
                 .Into(new GlideBitmapViewTarget(vh.mBgImageView, () => vh.mLocView.SetWeather(panelView)));
        }

        // Return the size of your dataset (invoked by the layout manager)
        public override int ItemCount => mDataset.Count;

        public void Add(LocationPanelViewModel item)
        {
            mDataset.Add(item);
            NotifyItemInserted(mDataset.IndexOf(item));
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

        private async Task RemoveLocation(int position)
        {
            // Remove location from list
            await Settings.DeleteLocation(mDataset[position].LocationData.query);

            // Remove panel
            Remove(position);
        }

        public void OnItemMove(int fromPosition, int toPosition)
        {
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
            Task.Run(async () => await RemoveLocation(position))
                .ContinueWith((t) => Task.Run(Shortcuts.ShortcutCreator.UpdateShortcuts));
        }

        protected void OnClick(RecyclerClickEventArgs args) => ItemClick?.Invoke(this, args);
        protected void OnLongClick(RecyclerClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }
}