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
using Android.Support.Wear.Widget;
using Android.Util;
using Android.Widget;
using Android.Views;
using SimpleWeather.Controls;
using SimpleWeather.Droid.Adapters;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using Android.Support.Wearable.Input;

namespace SimpleWeather.Droid.Wear
{
    public class WeatherForecastFragment : SwipeDismissFragment
    {
        private WeatherNowViewModel weatherView = null;

        private WearableRecyclerView recyclerView;

        private bool IsHourly = false;

        public static WeatherForecastFragment NewInstance(bool IsHourly, WeatherNowViewModel weatherViewModel)
        {
            WeatherForecastFragment fragment = new WeatherForecastFragment();
            if (weatherViewModel != null)
            {
                fragment.weatherView = weatherViewModel;
            }
            fragment.IsHourly = IsHourly;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View outerView = base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.fragment_weather_forecast, outerView as ViewGroup, true);

            recyclerView = outerView.FindViewById<WearableRecyclerView>(Resource.Id.recycler_view);
            recyclerView.RequestFocus();

            return outerView;
        }

        public override void OnResume()
        {
            base.OnResume();

            // Don't resume if fragment is hidden
            if (this.IsHidden)
                return;
            else
                Initialize();
        }

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (!hidden && this.IsVisible)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if (weatherView != null)
            {
                View?.SetBackgroundColor(weatherView.PendingBackground);
                // use this setting to improve performance if you know that changes
                // in content do not change the layout size of the RecyclerView
                recyclerView.HasFixedSize = true;
                // use a linear layout manager
                recyclerView.SetLayoutManager(new WearableLinearLayoutManager(Activity));
                recyclerView.EdgeItemsCenteringEnabled = true;
                // specify an adapter (see also next example)
                Android.Support.V7.Widget.RecyclerView.Adapter adapter = null;

                if (IsHourly)
                    adapter = new HourlyForecastItemAdapter(weatherView.Extras.HourlyForecast.ToList());
                else
                    adapter = new ForecastItemAdapter(weatherView.Forecasts.ToList());

                recyclerView.SetAdapter(adapter);
            }
        }
    }
}