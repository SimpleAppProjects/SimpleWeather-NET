using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SimpleWeather.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using SimpleWeather.Controls;
using SimpleWeather.Droid.Helpers;
using System.Threading;
using System.Threading.Tasks;
using Android.Support.Wear.Widget;
using Android.Support.V7.Widget;
using SimpleWeather.Droid.Wear.Controls;
using Android.Text;
using Android.Views.InputMethods;
using Android.Support.Design.Widget;
using Android.Speech;
using Android.Runtime;
using SimpleWeather.Droid.Wear.Helpers;
using SimpleWeather.Droid.Adapters;

namespace SimpleWeather.Droid.Wear
{
    public class LocationSearchFragment : SwipeDismissFragment
    {
        private WearableRecyclerView mRecyclerView;
        private LocationQueryAdapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;
        private ProgressBar mProgressBar;
        private EditText mSearchView;
        private Activity mActivity;

        private FloatingActionButton keyboardButton;
        private FloatingActionButton voiceButton;
        private SwipeDismissFrameLayout swipeViewLayout;
        private SwipeDismissCallback swipeCallback;

        private CancellationTokenSource cts;

        private WeatherData.WeatherManager wm;

        private const int REQUEST_CODE_VOICE_INPUT = 0;

        public LocationSearchFragment()
        {
            // Required empty public constructor
            ClickListener = LocationSearchFragment_clickListener;
            cts = new CancellationTokenSource();
            wm = WeatherData.WeatherManager.GetInstance();
            UserVisibleHint = true;
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
            mActivity = (Activity)context;
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
            LocationQueryViewModel query_vm = null;

            if (!String.IsNullOrEmpty(mAdapter.Dataset[e.Position].LocationQuery))
                query_vm = mAdapter.Dataset[e.Position];
            else
                query_vm = new LocationQueryViewModel();

            if (String.IsNullOrWhiteSpace(query_vm.LocationQuery))
            {
                // Stop since there is no valid query
                return;
            }

            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && wm.KeyRequired)
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
            var location = new WeatherData.LocationData(query_vm);
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
                ShowLoading(false);
                return;
            }

            // We got our data so disable controls just in case
            mAdapter.Dataset.Clear();
            mAdapter.NotifyDataSetChanged();
            mRecyclerView.Enabled = false;

            // Save weather data
            Settings.SaveHomeData(location);
            if (wm.SupportsAlerts && weather.weather_alerts != null)
                await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
            await Settings.SaveWeatherData(weather);

            // If we're using search
            // make sure gps feature is off
            Settings.FollowGPS = false;
            Settings.WeatherLoaded = true;

            // Start WeatherNow Activity with weather data
            Intent intent = new Intent(mActivity, typeof(MainActivity));
            intent.PutExtra("data", location.ToJson());

            mActivity.StartActivity(intent);
            mActivity.FinishAffinity();
        }

        private void ShowLoading(bool show)
        {
            mProgressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState) as SwipeDismissFrameLayout;
            // Inflate the layout for this fragment
            inflater.Inflate(Resource.Layout.fragment_location_search, view, true);

            swipeViewLayout = view.FindViewById<SwipeDismissFrameLayout>(Resource.Id.recycler_view_layout);
            swipeCallback = new SwipeDismissCallback();
            swipeCallback.Dismissed += (layout) =>
            {
                layout.Visibility = ViewStates.Gone;
                //layout.Reset();
            };
            swipeViewLayout.AddCallback(swipeCallback);
            keyboardButton = view.FindViewById<FloatingActionButton>(Resource.Id.keyboard_button);
            keyboardButton.Click += (sender, e) =>
            {
                mSearchView.Visibility = ViewStates.Visible;
                mSearchView.RequestFocus();
                ShowInputMethod(mSearchView);
            };
            voiceButton = view.FindViewById<FloatingActionButton>(Resource.Id.voice_button);
            voiceButton.Click += (sender, e) =>
            {
                mSearchView.Visibility = ViewStates.Gone;
                mSearchView.Text = String.Empty;
                view.RequestFocus();
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech)
                    .PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm)
                    .PutExtra(RecognizerIntent.ExtraPrompt, mActivity.GetString(Resource.String.location_search_hint));
                StartActivityForResult(intent, REQUEST_CODE_VOICE_INPUT);
            };

            mProgressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            mSearchView = view.FindViewById<EditText>(Resource.Id.search_view);
            mSearchView.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                // If we're using searchfragment
                // make sure gps feature is off
                if (Settings.FollowGPS)
                    Settings.FollowGPS = false;
            };
            mSearchView.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                View v = sender as View;

                if (e.HasFocus)
                    ShowInputMethod(v.FindFocus());
                else
                    HideInputMethod(v);
            };
            mSearchView.EditorAction += (object sender, TextView.EditorActionEventArgs e) =>
            {
                if (e.ActionId == ImeAction.Search)
                {
                    DoSearchAction();

                    // If we're using searchfragment
                    // make sure gps feature is off
                    if (Settings.FollowGPS)
                        Settings.FollowGPS = false;

                    e.Handled = true;
                }
                e.Handled = false;
            };

            mRecyclerView = view.FindViewById<WearableRecyclerView>(Resource.Id.recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            mRecyclerView.HasFixedSize = true;

            // To align the edge children (first and last) with the center of the screen
            mRecyclerView.EdgeItemsCenteringEnabled = true;
            
            // use a linear layout manager
            mLayoutManager = new WearableLinearLayoutManager(mActivity);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // specify an adapter (see also next example)
            mAdapter = new LocationQueryAdapter(new List<LocationQueryViewModel>());
            mAdapter.ItemClick += ClickListener;
            mRecyclerView.SetAdapter(mAdapter);

            return view;
        }

        public override void OnDestroyView()
        {
            swipeViewLayout.RemoveCallback(swipeCallback);
            base.OnDestroyView();
        }

        private void DoSearchAction()
        {
            mProgressBar.Visibility = ViewStates.Visible;
            FetchLocations(mSearchView.Text);
            HideInputMethod(mSearchView);
            mSearchView.Visibility = ViewStates.Gone;
            swipeViewLayout.Visibility = ViewStates.Visible;
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

                    var results = await wm.GetLocations(queryString);

                    if (cts.IsCancellationRequested) return;

                    mActivity?.RunOnUiThread(() =>
                    {
                        mAdapter.SetLocations(results.ToList());
                        mProgressBar.Visibility = ViewStates.Gone;
                    });
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

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok || data == null)
                return;

            switch (requestCode)
            {
                case REQUEST_CODE_VOICE_INPUT:
                    String text = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults).FirstOrDefault();

                    if (!String.IsNullOrWhiteSpace(text))
                    {
                        mSearchView.Text = text;
                        DoSearchAction();
                    }
                    break;
            }
        }

        private void ShowInputMethod(View view)
        {
            InputMethodManager imm = (InputMethodManager)mActivity.GetSystemService(
                    Context.InputMethodService);
            imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        private void HideInputMethod(View view)
        {
            InputMethodManager imm = (InputMethodManager)mActivity.GetSystemService(
                    Context.InputMethodService);
            if (imm != null && view != null)
            {
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }
    }
}