using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Wearable.Activity;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Support.Wearable.View.Drawer;
using System.Threading.Tasks;
using System.Threading;
using Android.Gms.Location;
using Android.Locations;
using SimpleWeather.Controls;
using Android.Content.PM;
using Android.Support.V4.Content;
using Android;
using Android.Support.Wear.Widget.Drawer;
using Android.Views.InputMethods;
using Android.Text;
using Android.Util;
using Android.Support.Wearable.Views;

namespace SimpleWeather.Droid.Wear
{
    [Activity(Theme = "@style/WearAppTheme")]
    public class SetupActivity : WearableActivity, IMenuItemOnMenuItemClickListener
    {
        private FloatingActionButton searchButton;
        private FloatingActionButton locationButton;
        private WearableActionDrawerView mWearableActionDrawer;
        private ProgressBar progressBar;

        private FusedLocationProviderClient mFusedLocationClient;
        private Location mLocation;
        private LocationCallback mLocCallback;
        private Droid.Helpers.LocationListener mLocListnr;
        private const int PERMISSION_LOCATION_REQUEST_CODE = 0;

        private CancellationTokenSource cts;

        private WeatherData.WeatherManager wm;

        private const int REQUEST_CODE_SYNC_ACTIVITY = 10;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_setup);

            cts = new CancellationTokenSource();
            wm = WeatherData.WeatherManager.GetInstance();

            // Set default API to Yahoo
            // if a valid API key hasn't been entered yet
            if (wm.KeyRequired && !Settings.KeyVerified)
                Settings.API = WeatherData.WeatherAPI.Yahoo;

            // Controls
            searchButton = FindViewById<FloatingActionButton>(Resource.Id.search_button);
            searchButton.Click += (sender, e) =>
            {
                FragmentManager.BeginTransaction()
                    .Replace(Resource.Id.search_fragment_container, new LocationSearchFragment())
                    .Commit();

                mWearableActionDrawer.Controller.CloseDrawer();
            };
            locationButton = FindViewById<FloatingActionButton>(Resource.Id.location_button);
            locationButton.Click += async (sender, e) =>
            {
                await FetchGeoLocation();
            };

            mWearableActionDrawer = FindViewById<WearableActionDrawerView>(Resource.Id.bottom_action_drawer);
            mWearableActionDrawer.SetOnMenuItemClickListener(this);
            mWearableActionDrawer.LockedWhenClosed = true;
            mWearableActionDrawer.Controller.PeekDrawer();
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            progressBar.Visibility = ViewStates.Gone;

            // Location client
            if (WearableHelper.IsGooglePlayServicesInstalled && !WearableHelper.HasGPS)
            {
                mFusedLocationClient = new FusedLocationProviderClient(this);
                mLocCallback = new LocationCallback();
                mLocCallback.LocationResult += async (sender, e) =>
                {
                    if (e.Result == null)
                        mLocation = null;
                    else
                        mLocation = e.Result.LastLocation;

                    if (mLocation == null)
                    {
                        EnableControls(true);
                        Toast.MakeText(this, Resource.String.error_retrieve_location, ToastLength.Short).Show();
                    }
                    else
                    {
                        await FetchGeoLocation();
                    }

                    await mFusedLocationClient.RemoveLocationUpdatesAsync(mLocCallback);
                };
                mLocCallback.LocationAvailability += async (sender, e) =>
                {
                    await mFusedLocationClient.FlushLocationsAsync();

                    if (!e.Availability.IsLocationAvailable)
                    {
                        EnableControls(true);
                        Toast.MakeText(this, Resource.String.error_retrieve_location, ToastLength.Short).Show();
                    }
                };
            }
            else
            {
                mLocListnr = new Droid.Helpers.LocationListener();
                mLocListnr.LocationChanged += async (Location location) =>
                {
                    mLocation = location;
                    await FetchGeoLocation();
                };
            }
        }

        public override void OnBackPressed()
        {
            if (FragmentManager.FindFragmentById(Resource.Id.search_fragment_container) is LocationSearchFragment mSearchFragment)
            {
                mSearchFragment.UserVisibleHint = false;

                FragmentManager.BeginTransaction()
                    .Remove(mSearchFragment)
                    .CommitAllowingStateLoss();

                mSearchFragment = null;

                EnableControls(true);
            }
            else
                base.OnBackPressed();
        }

        public bool OnMenuItemClick(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case Resource.Id.menu_settings:
                    StartActivity(new Intent(this, typeof(SettingsActivity)));
                    break;
                case Resource.Id.menu_setupfromphone:
                    var alertBuilder = new AlertDialog.Builder(this);
                    alertBuilder.SetMessage(Resource.String.prompt_confirmsetup);
                    alertBuilder.SetNegativeButton(Resource.String.generic_cancel, (s, e) => { });
                    alertBuilder.SetPositiveButton(Resource.String.generic_yes, (sender, e) =>
                    {
                        StartActivityForResult(typeof(SetupSyncActivity), REQUEST_CODE_SYNC_ACTIVITY);
                    });
                    alertBuilder.Create().Show();
                    break;
            }

            return true;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (requestCode)
            {
                case REQUEST_CODE_SYNC_ACTIVITY:
                    if (resultCode == Result.Ok)
                    {
                        if (Settings.HomeData is WeatherData.LocationData location)
                        {
                            Settings.DataSync = WearableDataSync.WhenAvailable;
                            Settings.WeatherLoaded = true;
                            // Start WeatherNow Activity
                            StartActivity(new Intent(this, typeof(MainActivity)));
                            FinishAffinity();
                        }
                    }
                    break;
            }
        }

        private void EnableControls(bool enable)
        {
            searchButton.Enabled = enable;
            locationButton.Enabled = enable;
            if (enable)
            {
                mWearableActionDrawer.Controller.PeekDrawer();
                progressBar.Visibility = ViewStates.Gone;
            }
            else
            {
                mWearableActionDrawer.Controller.CloseDrawer();
                progressBar.Visibility = ViewStates.Visible;
            }
        }

        public async Task FetchGeoLocation()
        {
            locationButton.Enabled = false;

            if (mLocation != null)
            {
                LocationQueryViewModel view = null;

                // Cancel other tasks
                cts.Cancel();
                cts = new CancellationTokenSource();

                if (cts.IsCancellationRequested)
                {
                    EnableControls(true);
                    return;
                }

                // Show loading bar
                progressBar.Visibility = ViewStates.Visible;

                await Task.Run(async () =>
                {
                    if (cts.IsCancellationRequested)
                    {
                        EnableControls(true);
                        return;
                    }

                    // Get geo location
                    view = await wm.GetLocation(mLocation);

                    if (String.IsNullOrEmpty(view.LocationQuery))
                        view = new LocationQueryViewModel();
                });

                if (String.IsNullOrWhiteSpace(view.LocationQuery))
                {
                    // Stop since there is no valid query
                    EnableControls(true);
                    return;
                }

                if (String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
                {
                    Toast.MakeText(this.ApplicationContext, Resource.String.werror_invalidkey, ToastLength.Short).Show();
                    EnableControls(true);
                    return;
                }

                if (cts.IsCancellationRequested)
                {
                    EnableControls(true);
                    return;
                }

                // Get Weather Data
                var location = new WeatherData.LocationData(view, mLocation);
                WeatherData.Weather weather = await Settings.GetWeatherData(location.query);
                if (weather == null)
                {
                    try
                    {
                        weather = await wm.GetWeather(location);
                    }
                    catch (WeatherException wEx)
                    {
                        weather = null;
                        Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
                    }
                }

                if (weather == null)
                {
                    EnableControls(true);
                    return;
                }

                // We got our data so disable controls just in case
                EnableControls(false);

                // Save weather data
                Settings.SaveLastGPSLocData(location);
                await Settings.DeleteLocations();
                await Settings.AddLocation(new WeatherData.LocationData(view));
                if (wm.SupportsAlerts && weather.weather_alerts != null)
                    await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
                await Settings.SaveWeatherData(weather);

                Settings.FollowGPS = true;
                Settings.WeatherLoaded = true;

                // Start WeatherNow Activity with weather data
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("data", location.ToJson());

                StartActivity(intent);
                FinishAffinity();
            }
            else
            {
                await UpdateLocation();
            }
        }

        private async Task UpdateLocation()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                RequestPermissions(new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
                        PERMISSION_LOCATION_REQUEST_CODE);
                return;
            }

            Location location = null;

            if (WearableHelper.IsGooglePlayServicesInstalled && !WearableHelper.HasGPS)
            {
                location = await mFusedLocationClient.GetLastLocationAsync();

                if (location == null)
                {
                    var mLocationRequest = new LocationRequest();
                    mLocationRequest.SetInterval(10000);
                    mLocationRequest.SetFastestInterval(1000);
                    mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
                    mLocationRequest.SetNumUpdates(1);
                    await mFusedLocationClient.RequestLocationUpdatesAsync(mLocationRequest, mLocCallback, null);
                    await mFusedLocationClient.FlushLocationsAsync();
                }
            }
            else
            {
                LocationManager locMan = (LocationManager)GetSystemService(Context.LocationService);
                bool isGPSEnabled = locMan.IsProviderEnabled(LocationManager.GpsProvider);
                bool isNetEnabled = locMan.IsProviderEnabled(LocationManager.NetworkProvider);

                if (isGPSEnabled)
                {
                    location = locMan.GetLastKnownLocation(LocationManager.GpsProvider);

                    if (location == null)
                        location = locMan.GetLastKnownLocation(LocationManager.NetworkProvider);

                    if (location == null)
                        locMan.RequestSingleUpdate(LocationManager.GpsProvider, mLocListnr, null);
                }
                else if (isNetEnabled)
                {
                    location = locMan.GetLastKnownLocation(LocationManager.NetworkProvider);

                    if (location == null)
                        locMan.RequestSingleUpdate(LocationManager.NetworkProvider, mLocListnr, null);
                }
                else
                {
                    EnableControls(true);
                    Toast.MakeText(this, Resource.String.error_retrieve_location, ToastLength.Short).Show();
                }
            }

            if (location != null)
            {
                mLocation = location;
                await FetchGeoLocation();
            }
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case PERMISSION_LOCATION_REQUEST_CODE:
                    {
                        // If request is cancelled, the result arrays are empty.
                        if (grantResults.Length > 0
                                && grantResults[0] == Permission.Granted)
                        {
                            // permission was granted, yay!
                            // Do the task you need to do.
                            await FetchGeoLocation();
                        }
                        else
                        {
                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
                            EnableControls(true);
                            Toast.MakeText(this, Resource.String.error_location_denied, ToastLength.Short).Show();
                        }
                        return;
                    }
            }
        }
    }
}