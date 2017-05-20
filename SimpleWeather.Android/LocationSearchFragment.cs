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
using System.Collections.Generic;
using Android.Runtime;
using System.Collections.Specialized;
using SimpleWeather.Droid.Controls;
using SimpleWeather.Controls;
using System.Collections.ObjectModel;
using SimpleWeather.Droid.Utils;

namespace SimpleWeather.Droid
{
    public class LocationSearchFragment : Fragment
    {
        private Location mLocation;
        private String mQueryString;
        private RecyclerView mRecyclerView;
        private LocationQueryAdapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;

        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

        private String ARG_QUERY = "query";
        private String ARG_INDEX = "index";

        private String selected_query = String.Empty;

        public LocationSearchFragment()
        {
            // Required empty public constructor
            clickListener = LocationSearchFragment_clickListener;
        }

        public event EventHandler<RecyclerClickEventArgs> clickListener;

        public void SetClickListener(EventHandler<RecyclerClickEventArgs> listener)
        {
            clickListener = listener;
        }

        private async void LocationSearchFragment_clickListener(object sender, RecyclerClickEventArgs e)
        {
            LocationQuery v = (LocationQuery)e.View;
            int homeIdx = 0;
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
            Settings.saveWeatherData(weatherData);

            pair = new Pair<int, string>(homeIdx, selected_query);

            Intent intent = new Intent(Activity, typeof(MainActivity));
            intent.PutExtra(ARG_QUERY, pair.Value);
            intent.PutExtra(ARG_INDEX, pair.Key);

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
            mLocListnr.LocationChanged += (object sender, Location location) =>
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

        public async void fetchLocations(String queryString)
        {
            if (!TextUtils.Equals(mQueryString, queryString))
            {
                mQueryString = queryString;
                // Get locations
                List<LocationQueryView> locations = new List<LocationQueryView>();

                if (!TextUtils.IsEmpty(mQueryString))
                {
                    try
                    {
                        ObservableCollection<LocationQueryView> results = await WeatherData.AutoCompleteQuery.getLocations(mQueryString);

                        if (results.Count > 0)
                            locations.AddRange(results);
                    }
                    catch (Exception e)
                    {
                        //e.printStackTrace();
                    }
                }

                mAdapter.SetLocations(locations);
            }
        }

        public async void fetchGeoLocation()
        {
            if (mLocation != null)
            {
                // Get geo location
                LocationQueryView gpsLocation = null;
                List<LocationQueryView> results = new List<LocationQueryView>(1);

                try
                {
                    gpsLocation = await WeatherData.GeopositionQuery.getLocation(mLocation);

                    if (gpsLocation != null)
                    {
                        mQueryString = gpsLocation.LocationName;
                        results.Add(gpsLocation);
                        mAdapter.SetLocations(results);
                    }
                }
                catch (Exception e)
                {
                    //e.printStackTrace();
                }
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

        private LocationListener mLocListnr = new LocationListener();

        internal class LocationListener : Java.Lang.Object, ILocationListener
        {
            public event EventHandler<Location> LocationChanged;

            public void OnLocationChanged(Location location)
            {
                LocationChanged?.Invoke(this, location);
            }

            public void OnProviderDisabled(string provider)
            {
            }

            public void OnProviderEnabled(string provider)
            {
            }

            public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
            {
            }
        }
    }
}