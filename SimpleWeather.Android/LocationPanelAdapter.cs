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

namespace SimpleWeather.Droid
{
    public class LocationPanelAdapter : RecyclerView.Adapter, IItemTouchHelperAdapter
    {
        private ObservableCollection<LocationPanelViewModel> mDataset;

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
            public ImageButton mHomeButton;
            public ViewHolder(LocationPanel v,
                Action<RecyclerClickEventArgs> clickListener, Action<RecyclerClickEventArgs> longClickListener, Action<RecyclerClickEventArgs> homeClickListener)
                : base(v)
            {
                mLocView = v;
                mHomeButton = v.FindViewById<ImageButton>(Resource.Id.home_button);

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
            Context context = vh.mLocView.Context;
            LocationPanelViewModel panelView = mDataset[position];

            vh.mLocView.SetWeather(panelView);

            // Set HomeButton visibility based on EditMode status
            if (!panelView.IsHome && !panelView.EditMode)
                vh.mHomeButton.Visibility = ViewStates.Gone;
            else if (!panelView.IsHome && panelView.EditMode)
                vh.mHomeButton.Visibility = ViewStates.Visible;
            else if (!panelView.EditMode)
                vh.mHomeButton.Visibility = panelView.IsHome ? ViewStates.Visible : ViewStates.Gone;

            if (panelView.IsHome)
            {
                vh.mHomeButton.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_home_fill_white_24dp));
                vh.mHomeButton.Enabled = false;
                vh.mHomeButton.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.mHomeButton.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_home_nofill_white_24dp));
                vh.mHomeButton.Enabled = true;
            }

            if (Settings.FollowGPS)
                vh.mHomeButton.Visibility = ViewStates.Gone;
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
                    panelView.Pair = new Utils.Pair<int, string>(Settings.FollowGPS ? index + 1 : index, panelView.Pair.Value);
                }
            }
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

        private async void RemoveLocation(int position)
        {
            // Remove location from list
            OrderedDictionary weatherData = await Settings.GetWeatherData();
            weatherData.RemoveAt(Settings.FollowGPS ? position + 1 : position);
            Settings.SaveWeatherData();

            // Remove panel
            Remove(position);

            // Reset home if necessary
            if (position == App.HomeIdx && !Settings.FollowGPS)
            {
                mDataset[App.HomeIdx].IsHome = true;
                NotifyItemChanged(App.HomeIdx);
            }
        }

        public async void OnItemMove(int fromPosition, int toPosition)
        {
            int dataFromPos = fromPosition;
            int dataToPos = toPosition;

            if (Settings.FollowGPS)
            {
                dataFromPos++;
                dataToPos++;
            }

            // Move data in both weather dictionary and local dataset
            OrderedDictionary data = await Settings.GetWeatherData();
            LocationPanelViewModel panel = mDataset[fromPosition];

            WeatherData.Weather weather = data[dataFromPos] as WeatherData.Weather;
            data.RemoveAt(dataFromPos);
            data.Insert(dataToPos, panel.Pair.Value, weather);

            mDataset.Move(fromPosition, toPosition);
            NotifyItemMoved(fromPosition, toPosition);
        }

        public void OnItemMoved(int fromPosition, int toPosition)
        {
            // Reset home if necessary
            if ((fromPosition == App.HomeIdx || toPosition == App.HomeIdx) && !Settings.FollowGPS)
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

        public void OnItemDismiss(int position)
        {
            RemoveLocation(position);
        }

        private void HomeButton_Click(object sender, RecyclerClickEventArgs args)
        {
            int index = args.Position;

            if (index == App.HomeIdx)
                return;

            OnItemMove(index, App.HomeIdx);
            OnItemMoved(index, App.HomeIdx);
        }

        protected void OnClick(RecyclerClickEventArgs args) => ItemClick?.Invoke(this, args);
        protected void OnLongClick(RecyclerClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        protected void OnHomeClick(RecyclerClickEventArgs args) => HomeButton_Click(this, args);
    }
}