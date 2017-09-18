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

namespace SimpleWeather.Droid
{
    public class LocationSearchFragment : Fragment
    {
        private RecyclerView mRecyclerView;
        private LocationQueryAdapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;

        private String selected_query = String.Empty;

        private CancellationTokenSource cts;

        public LocationSearchFragment()
        {
            // Required empty public constructor
            ClickListener = LocationSearchFragment_clickListener;
            cts = new CancellationTokenSource();
        }

        public event EventHandler<RecyclerClickEventArgs> ClickListener;

        public void SetClickListener(EventHandler<RecyclerClickEventArgs> listener)
        {
            ClickListener = listener;
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
                Toast.MakeText(Activity.ApplicationContext, errorMsg, ToastLength.Short).Show();
                return;
            }

            // Show loading dialog
            LoadingDialog progDialog = new LoadingDialog(Activity);
            progDialog.Show();

            // Get Weather Data
            var locData = Settings.LocationData;
            var weatherData = Settings.WeatherData;

            WeatherData.Weather weather = weatherData[selected_query] as WeatherData.Weather;
            if (weather == null)
                weather = await WeatherData.WeatherLoaderTask.GetWeather(selected_query);

            if (weather == null)
            {
                // Hide dialog
                progDialog.Dismiss();
                return;
            }

            // Save weather data
            var location = new WeatherData.LocationData(selected_query);
            locData.Clear();
            locData.Add(location);
            weatherData[selected_query] = weather;
            Settings.SaveLocationData();
            Settings.SaveWeatherData();

            // Start WeatherNow Activity with weather data
            Intent intent = new Intent(Activity, typeof(MainActivity));
            intent.PutExtra("data", await JSONParser.SerializerAsync(location, typeof(WeatherData.LocationData)));

            // If we're using search
            // make sure gps feature is off
            Settings.FollowGPS = false;
            Settings.WeatherLoaded = true;
            // Hide dialog
            progDialog.Dismiss();

            Activity.StartActivity(intent);
            Activity.FinishAffinity();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.fragment_location_search, container, false);
            SetupView(view);

            Activity.Window.SetSoftInputMode(SoftInput.AdjustResize);
            return view;
        }

        private void SetupView(View view)
        {
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            mRecyclerView.HasFixedSize = true;

            // use a linear layout manager
            mLayoutManager = new LinearLayoutManager(Activity);
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

                    this.Activity.RunOnUiThread(() => mAdapter.SetLocations(results.ToList()));
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