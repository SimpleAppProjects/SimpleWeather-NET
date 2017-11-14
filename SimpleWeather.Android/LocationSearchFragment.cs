using Android;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SimpleWeather.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using SimpleWeather.Droid.Controls;
using SimpleWeather.Controls;
using SimpleWeather.Droid.Utils;
using SimpleWeather.Droid.Helpers;
using System.Threading;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Android.Graphics;
using Android.Support.V4.Graphics.Drawable;
using Android.Util;

namespace SimpleWeather.Droid
{
    public class LocationSearchFragment : Fragment
    {
        private RecyclerView mRecyclerView;
        private LocationQueryAdapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;
        private ProgressBar mProgressBar;
        private View mClearButton;
        private EditText mSearchView;
        private FragmentActivity mActivity; 

        private String selected_query = String.Empty;

        private CancellationTokenSource cts;

        public LocationSearchFragment()
        {
            // Required empty public constructor
            ClickListener = LocationSearchFragment_clickListener;
            cts = new CancellationTokenSource();
        }

        public void CtsCancel()
        {
            cts.Cancel();
            cts = new CancellationTokenSource();
        }

        public bool CtsCancelRequested()
        {
            return (bool)cts?.IsCancellationRequested;
        }

        public event EventHandler<RecyclerClickEventArgs> ClickListener;

        public void SetClickListener(EventHandler<RecyclerClickEventArgs> listener)
        {
            ClickListener = listener;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            mActivity = (FragmentActivity)context;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            mActivity = null;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            mActivity = null;
        }

        private async void LocationSearchFragment_clickListener(object sender, RecyclerClickEventArgs e)
        {
            // Get selected query view
            LocationQuery v = (LocationQuery)e.View;

            if (!String.IsNullOrEmpty(mAdapter.Dataset[e.Position].LocationQuery))
                selected_query = mAdapter.Dataset[e.Position].LocationQuery;
            else
                selected_query = string.Empty;

            if (String.IsNullOrWhiteSpace(selected_query))
            {
                // Stop since there is no valid query
                return;
            }

            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && Settings.API == Settings.API_WUnderground)
            {
                String errorMsg = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey).Message;
                Toast.MakeText(App.Context, errorMsg, ToastLength.Short).Show();
                return;
            }

            // Cancel pending searches
            cts.Cancel();
            cts = new CancellationTokenSource();

            ShowLoading(true);

            if (cts.IsCancellationRequested)
            {
                ShowLoading(false);
                return;
            }

            // Get Weather Data
            WeatherData.Weather weather = await Settings.GetWeatherData(selected_query);
            if (weather == null)
                weather = await WeatherData.WeatherLoaderTask.GetWeather(selected_query);

            if (weather == null)
            {
                ShowLoading(false);
                return;
            }

            // We got our data so disable controls just in case
            mAdapter.Dataset.Clear();
            mAdapter.NotifyDataSetChanged();
            mRecyclerView.Enabled = false;

            // Save weather data
            var location = new WeatherData.LocationData(selected_query);
            await Settings.DeleteLocations();
            await Settings.AddLocation(location);
            Settings.SaveWeatherData(weather);

            // Start WeatherNow Activity with weather data
            Intent intent = new Intent(mActivity, typeof(MainActivity));
            intent.PutExtra("data", location.ToJson());

            // If we're using search
            // make sure gps feature is off
            Settings.FollowGPS = false;
            Settings.WeatherLoaded = true;

            mActivity.StartActivity(intent);
            mActivity.FinishAffinity();
        }

        private void ShowLoading(bool show)
        {
            mProgressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;

            if (show || (!show && String.IsNullOrEmpty(mSearchView.Text)))
                mClearButton.Visibility = ViewStates.Gone;
            else
                mClearButton.Visibility = ViewStates.Visible;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.fragment_location_search, container, false);
            SetupView(view);

            mActivity.Window.SetSoftInputMode(SoftInput.AdjustResize);
            return view;
        }

        private void SetupView(View view)
        {
            mProgressBar = mActivity.FindViewById<ProgressBar>(Resource.Id.search_progressBar);
            mClearButton = mActivity.FindViewById(Resource.Id.search_close_button);
            mSearchView = mActivity.FindViewById<EditText>(Resource.Id.search_view);

            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                mProgressBar.IndeterminateDrawable = 
                    ContextCompat.GetDrawable(mActivity, Resource.Drawable.progressring);
            }

            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            mRecyclerView.HasFixedSize = true;

            // use a linear layout manager
            mLayoutManager = new LinearLayoutManager(mActivity);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // specify an adapter (see also next example)
            mAdapter = new LocationQueryAdapter(new List<LocationQueryViewModel>());
            mAdapter.ItemClick += ClickListener;
            mRecyclerView.SetAdapter(mAdapter);
        }

        public void FetchLocations(String queryString)
        {
            // Cancel pending searches
            cts.Cancel();
            cts = new CancellationTokenSource();

            // Get locations
            if (!String.IsNullOrWhiteSpace(queryString))
            {
                Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested) return;

                    var results = await WeatherData.AutoCompleteQuery.GetLocations(queryString);

                    if (cts.IsCancellationRequested) return;

                    mActivity?.RunOnUiThread(() => mAdapter.SetLocations(results.ToList()));
                });
            }
            else if (String.IsNullOrWhiteSpace(queryString))
            {
                // Cancel pending searches
                cts.Cancel();
                // Hide flyout if query is empty or null
                mAdapter.Dataset.Clear();
                mAdapter.NotifyDataSetChanged();
            }
        }
    }
}