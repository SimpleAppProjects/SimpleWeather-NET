using System.Linq;

using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.Wear.Widget;
using Android.Views;

using SimpleWeather.Controls;
using SimpleWeather.Droid.Adapters;

namespace SimpleWeather.Droid.Wear
{
    public enum WeatherListType
    {
        Forecast = 0,
        HourlyForecast,
        Alerts
    }

    public class WeatherListFragment : SwipeDismissFragment
    {
        private WeatherNowViewModel weatherView = null;

        private WearableRecyclerView recyclerView;

        private WeatherListType weatherType;

        public static WeatherListFragment NewInstance(WeatherListType weatherType, WeatherNowViewModel weatherViewModel)
        {
            WeatherListFragment fragment = new WeatherListFragment();
            if (weatherViewModel != null)
            {
                fragment.weatherView = weatherViewModel;
            }

            Bundle args = new Bundle();
            args.PutInt("WeatherListType", (int)weatherType);
            fragment.Arguments = args;

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
            View view = inflater.Inflate(Resource.Layout.fragment_weather_list, outerView as ViewGroup, true);

            recyclerView = outerView.FindViewById<WearableRecyclerView>(Resource.Id.recycler_view);
            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            recyclerView.HasFixedSize = true;
            recyclerView.EdgeItemsCenteringEnabled = true;

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

        public void Initialize()
        {
            if (weatherView != null)
            {
                View?.SetBackgroundColor(weatherView.PendingBackground);
                // specify an adapter (see also next example)
                RecyclerView.Adapter adapter = null;

                weatherType = (WeatherListType)Arguments?.GetInt("WeatherListType", 0);

                switch (weatherType)
                {
                    default:
                    case WeatherListType.Forecast:
                        recyclerView.SetLayoutManager(new WearableLinearLayoutManager(Activity));
                        adapter = new ForecastItemAdapter(weatherView.Forecasts.ToList());
                        break;
                    case WeatherListType.HourlyForecast:
                        recyclerView.SetLayoutManager(new WearableLinearLayoutManager(Activity));
                        adapter = new HourlyForecastItemAdapter(weatherView.Extras.HourlyForecast.ToList());
                        break;
                    case WeatherListType.Alerts:
                        recyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                        adapter = new WeatherAlertPanelAdapter(weatherView.Extras.Alerts.ToList());
                        break;
                }

                recyclerView.SetAdapter(adapter);
            }
        }
    }
}