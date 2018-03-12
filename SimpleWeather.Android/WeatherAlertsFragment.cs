using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using SimpleWeather.Controls;
using SimpleWeather.Droid.App.Adapters;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Droid.App
{
    public class WeatherAlertsFragment : Fragment
    {
        private AppCompatActivity AppCompatActivity;
        private LocationData location = null;
        private WeatherNowViewModel weatherView = null;

        private Toolbar toolbar;
        private Android.Widget.TextView locationHeader;
        private RecyclerView recyclerView;

        public static WeatherAlertsFragment NewInstance(LocationData location)
        {
            WeatherAlertsFragment fragment = new WeatherAlertsFragment();
            if (location != null)
            {
                fragment.location = location;
            }
            return fragment;
        }

        public static WeatherAlertsFragment NewInstance(LocationData location, WeatherNowViewModel weatherViewModel)
        {
            WeatherAlertsFragment fragment = new WeatherAlertsFragment();
            if (location != null && weatherViewModel != null)
            {
                fragment.location = location;
                fragment.weatherView = weatherViewModel;
            }
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            if (location == null && savedInstanceState != null)
            {
                location = LocationData.FromJson(
                    new Newtonsoft.Json.JsonTextReader(
                        new System.IO.StringReader(savedInstanceState.GetString("data", null))));
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.fragment_weather_alerts, container, false);

            // Setup Actionbar
            toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.NavigationClick += (sender, e) =>
            {
                AppCompatActivity?.OnBackPressed();
            };

            locationHeader = view.FindViewById<Android.Widget.TextView>(Resource.Id.location_name);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);

            return view;
        }

        public override async void OnResume()
        {
            base.OnResume();

            // Don't resume if fragment is hidden
            if (this.IsHidden)
                return;
            else
                await Initialize();
        }

        public override async void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (!hidden && this.IsVisible)
            {
                await Initialize();
            }
        }

        private async Task Initialize()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                AppCompatActivity?.Window.SetStatusBarColor(
                    new Color(ContextCompat.GetColor(AppCompatActivity, Resource.Color.colorPrimaryDark)));
            }

            if (weatherView == null)
            {
                if (location == null)
                    location = Settings.HomeData;

                var weather = await Settings.GetWeatherData(location.query);
                if (weather != null)
                {
                    weather.weather_alerts = await Settings.GetWeatherAlertData(location.query);
                }

                weatherView = new WeatherNowViewModel(weather);
            }

            if (weatherView != null)
            {
                locationHeader.Text = weatherView.Location;
                // use this setting to improve performance if you know that changes
                // in content do not change the layout size of the RecyclerView
                recyclerView.HasFixedSize = true;
                // use a linear layout manager
                recyclerView.SetLayoutManager(new LinearLayoutManager(AppCompatActivity));
                // specify an adapter (see also next example)
                recyclerView.SetAdapter(new WeatherAlertPanelAdapter(weatherView.Extras.Alerts.ToList()));
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            // Save data
            outState.PutString("data", location.ToJson());

            base.OnSaveInstanceState(outState);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            AppCompatActivity = context as AppCompatActivity;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            AppCompatActivity = null;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            AppCompatActivity = null;
        }
    }
}