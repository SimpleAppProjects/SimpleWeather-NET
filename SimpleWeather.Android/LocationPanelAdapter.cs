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
using SimpleWeather.Droid.Helpers;
using SimpleWeather.Utils;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Android.Support.V4.Content;

namespace SimpleWeather.Droid
{
    public class LocationPanelAdapter : RecyclerView.Adapter, ItemTouchHelperAdapter
    {
        private ObservableCollection<LocationPanelViewModel> mDataset;

        public List<LocationPanelViewModel> Dataset { get { return mDataset.ToList(); } }
        public event EventHandler<RecyclerClickEventArgs> ItemClick;
        public event EventHandler<RecyclerClickEventArgs> ItemLongClick;
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

        // Provide a reference to the views for each data item
        // Complex data items may need more than one view per item, and
        // you provide access to all the views for a data item in a view holder
        class ViewHolder : RecyclerView.ViewHolder
        {
            // each data item is just a string in this case
            public LocationPanel mLocView;
            public ImageButton mHomeButton;
            public ViewHolder(LocationPanel v,
                Action<RecyclerClickEventArgs> clickListener, Action<RecyclerClickEventArgs> longClickListener, Action<RecyclerClickEventArgs> homeClickListener)
                : base(v)
            {
                mLocView = v;
                mHomeButton = (ImageButton)v.FindViewById(Resource.Id.home_button);

                mLocView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
                mLocView.LongClick += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
                mHomeButton.Click += (sender, e) => homeClickListener(new RecyclerClickEventArgs { View = mLocView, Position = AdapterPosition });
            }
        }

        // Provide a suitable constructor (depends on the kind of dataset)
        public LocationPanelAdapter(List<LocationPanelViewModel> myDataset)
        {
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
            return new ViewHolder(v, OnClick, OnLongClick, OnHomeClick);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // - get element from your dataset at this position
            // - replace the contents of the view with that element
            ViewHolder vh = holder as ViewHolder;
            LocationPanelViewModel panelView = mDataset[position];

            if (!panelView.IsHome && !panelView.EditMode)
                vh.mHomeButton.Visibility = ViewStates.Gone;
            else if (!panelView.IsHome && panelView.EditMode)
                vh.mHomeButton.Visibility = ViewStates.Visible;
            else if (!panelView.EditMode)
                vh.mHomeButton.Visibility = panelView.IsHome ? ViewStates.Visible : ViewStates.Gone;

            if (panelView.IsHome)
            {
                vh.mHomeButton.SetImageDrawable(ContextCompat.GetDrawable(vh.mLocView.Context, Resource.Drawable.ic_home_fill_white_24dp));
                vh.mHomeButton.Enabled = false;
                vh.mHomeButton.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.mHomeButton.SetImageDrawable(ContextCompat.GetDrawable(vh.mLocView.Context, Resource.Drawable.ic_home_nofill_white_24dp));
                vh.mHomeButton.Enabled = true;
            }

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

            // Update pair if necessary
            if (CollectionChanged == null)
            {
                foreach (LocationPanelViewModel panelView in mDataset)
                {
                    int index = mDataset.IndexOf(panelView);
                    panelView.Pair = new Utils.Pair<int, string>(index, panelView.Pair.Value);
                }
            }
        }

        public LocationPanelViewModel GetPanelView(int position)
        {
            return mDataset[position];
        }

        private async void RemoveLocation(int position)
        {
            // Remove location from list
            OrderedDictionary weatherData = await Settings.getWeatherData();
            weatherData.RemoveAt(position);
            Settings.saveWeatherData();

            // Remove panel
            Remove(position);

            if (position == App.HomeIdx)
            {
                mDataset[App.HomeIdx].IsHome = true;
                NotifyItemChanged(App.HomeIdx);
            }
        }

        public async void onItemMove(int fromPosition, int toPosition)
        {
            OrderedDictionary data = await Settings.getWeatherData();
            LocationPanelViewModel panel = mDataset[fromPosition];

            WeatherData.Weather weather = data[fromPosition] as WeatherData.Weather;
            data.RemoveAt(fromPosition);
            data.Insert(toPosition, panel.Pair.Value, weather);

            mDataset.Move(fromPosition, toPosition);
            NotifyItemMoved(fromPosition, toPosition);
        }

        public void onItemMoved(int fromPosition, int toPosition)
        {
            // Reset home if necessary
            if (fromPosition == App.HomeIdx || toPosition == App.HomeIdx)
            {
                foreach (LocationPanelViewModel panelView in mDataset)
                {
                    int panelIndex = mDataset.IndexOf(panelView);

                    if (panelIndex == App.HomeIdx)
                        panelView.IsHome = true;
                    else
                        panelView.IsHome = false;
                }

                NotifyDataSetChanged();
            }
            else
            {
                NotifyItemChanged(fromPosition);
                NotifyItemChanged(toPosition);
            }
        }

        public void onItemDismiss(int position)
        {
            RemoveLocation(position);
        }

        private void HomeButton_Click(object sender, RecyclerClickEventArgs args)
        {
            int index = args.Position;

            if (index == App.HomeIdx)
                return;

            onItemMove(index, App.HomeIdx);
            onItemMoved(index, App.HomeIdx);
        }

        protected void OnClick(RecyclerClickEventArgs args) => ItemClick?.Invoke(this, args);
        protected void OnLongClick(RecyclerClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        protected void OnHomeClick(RecyclerClickEventArgs args) => HomeButton_Click(this, args);
    }
}