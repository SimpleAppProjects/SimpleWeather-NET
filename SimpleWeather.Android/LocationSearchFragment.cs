using Android;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using SimpleWeather.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using Android.Runtime;
using System.Collections.Specialized;
using SimpleWeather.Droid.Controls;
using SimpleWeather.Controls;
using System.Collections.ObjectModel;
using SimpleWeather.Droid.Utils;
using SimpleWeather.Droid.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Droid
{
    public class LocationSearchFragment : Fragment
    {
        private Location mLocation;
        private RecyclerView mRecyclerView;
        private LocationQueryAdapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;

        private LocationListener mLocListnr;

        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

        private String selected_query = String.Empty;

        private CancellationTokenSource cts;

        public LocationSearchFragment()
        {
            // Required empty public constructor
            clickListener = LocationSearchFragment_clickListener;
            cts = new CancellationTokenSource();
        }

        public event EventHandler<RecyclerClickEventArgs> clickListener;

        public void SetClickListener(EventHandler<RecyclerClickEventArgs> listener)
        {
            clickListener = listener;
        }

        private async void LocationSearchFragment_clickListener(object sender, RecyclerClickEventArgs e)
        {
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

            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && Settings.API == "WUnderground")
            {
                // TODO: replace with string resource
                String errorMsg = new WeatherException(WeatherUtils.ErrorStatus.INVALIDAPIKEY).Message;
                Toast.MakeText(Activity.ApplicationContext, errorMsg, ToastLength.Short).Show();
                return;
            }

            Pair<int, string> pair;
            
            // Weather Data
            OrderedDictionary weatherData = await Settings.getWeatherData();

            WeatherData.Weather weather = await WeatherData.WeatherLoaderTask.getWeather(selected_query);

            if (weather == null)
                return;

            // Save weather data
            if (weatherData.Contains(selected_query))
                weatherData[selected_query] = weather;
            else
                weatherData.Add(selected_query, weather);
            Settings.saveWeatherData();

            pair = new Pair<int, string>(App.HomeIdx, selected_query);

            Intent intent = new Intent(Activity, typeof(MainActivity));
            intent.PutExtra("pair", JSONParser.Serializer(pair, typeof(Pair<int, string>)));

            Settings.WeatherLoaded = true;
            Activity.StartActivity(intent);
            Activity.FinishAffinity();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.fragment_location_search, container, false);
            setupView(view);
            return view;
        }

        public override void OnDetach()
        {
            base.OnDetach();
        }

        private void setupView(View view)
        {
            mRecyclerView = (RecyclerView)view.FindViewById(Resource.Id.recycler_view);

            // Location Listener
            mLocListnr = new LocationListener();
            mLocListnr.LocationChanged += (Location location) =>
            {
                mLocation = location;
                fetchGeoLocation();
            };

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            mRecyclerView.HasFixedSize = true;

            // use a linear layout manager
            mLayoutManager = new LinearLayoutManager(Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // specify an adapter (see also next example)
            mAdapter = new LocationQueryAdapter(new List<LocationQueryView>());
            mAdapter.ItemClick += clickListener;
            mRecyclerView.SetAdapter(mAdapter);
        }

        public void fetchLocations(String queryString)
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

                    var results = await WeatherData.AutoCompleteQuery.getLocations(queryString);

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

        public void fetchGeoLocation()
        {
            // Cancel pending searches
            cts.Cancel();
            cts = new CancellationTokenSource();

            if (mLocation != null)
            {
                Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested) return;

                    // Get geo location
                    LocationQueryView gpsLocation = await WeatherData.GeopositionQuery.getLocation(mLocation);

                    if (cts.IsCancellationRequested) return;

                    this.Activity.RunOnUiThread(() => mAdapter.SetLocations(new List<LocationQueryView>() { gpsLocation }));
                });
            }
            else
            {
                updateLocation();
            }
        }

        private void updateLocation()
        {
            if (ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) != Permission.Granted && ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                RequestPermissions(new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
                        PERMISSION_LOCATION_REQUEST_CODE);
                return;
            }

            LocationManager locMan = (LocationManager)Activity.GetSystemService(Context.LocationService);
            bool isGPSEnabled = locMan.IsProviderEnabled(LocationManager.GpsProvider);
            bool isNetEnabled = locMan.IsProviderEnabled(LocationManager.NetworkProvider);

            Location location = null;

            if (isGPSEnabled)
            {
                location = locMan.GetLastKnownLocation(LocationManager.GpsProvider);

                if (location == null)
                    location = locMan.GetLastKnownLocation(LocationManager.NetworkProvider);

                if (location == null)
                    locMan.RequestSingleUpdate(LocationManager.GpsProvider, mLocListnr, null);
                else
                {
                    mLocation = location;
                    fetchGeoLocation();
                }
            }
            else if (isNetEnabled)
            {
                location = locMan.GetLastKnownLocation(LocationManager.NetworkProvider);

                if (location == null)
                    locMan.RequestSingleUpdate(LocationManager.NetworkProvider, mLocListnr, null);
                else
                {
                    mLocation = location;
                    fetchGeoLocation();
                }
            }
            else
            {
                Toast.MakeText(Activity, "Unable to get location", ToastLength.Short).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case PERMISSION_LOCATION_REQUEST_CODE:
                    {
                        // If request is cancelled, the result arrays are empty.
                        if (grantResults.Length > 0
                                && grantResults[0] == Permission.Granted)
                        {

                            // permission was granted, yay! Do the
                            // contacts-related task you need to do.

                            fetchGeoLocation();
                        }
                        else
                        {
                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
                            Toast.MakeText(Activity, "Location access denied", ToastLength.Short).Show();
                        }
                        return;
                    }
            }
        }
    }
}