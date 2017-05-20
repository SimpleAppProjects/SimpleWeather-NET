using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;

namespace SimpleWeather.Droid.Controls
{
    public class LocationPanel : CardView
    {
        private View viewLayout;
        private View mainLayout;
        private TextView locationNameView;
        private TextView locationTempView;
        private WeatherIcon locationWeatherIcon;
        private ProgressBar progressBar;

        public LocationPanel(Context context) :
            base(context)
        {
            Initialize(context);
        }

        public LocationPanel(Context context, LocationPanelView panelView) :
            base(context)
        {
            Initialize(context);
            SetWeather(panelView);
        }

        public LocationPanel(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context);
        }

        public LocationPanel(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            LayoutInflater inflater = LayoutInflater.From(context);
            viewLayout = inflater.Inflate(Resource.Layout.location_panel, this);
            mainLayout = viewLayout.FindViewById(Resource.Id.main_layout);

            locationNameView = (TextView)viewLayout.FindViewById(Resource.Id.location_name);
            locationTempView = (TextView)viewLayout.FindViewById(Resource.Id.weather_temp);
            locationWeatherIcon = (WeatherIcon)viewLayout.FindViewById(Resource.Id.weather_icon);
            progressBar = (ProgressBar)viewLayout.FindViewById(Resource.Id.progressBar);

            Enabled = false;
            ShowLoading(true);
        }

        public async void SetWeather(LocationPanelView panelView)
        {
            // Background
            try
            {
                mainLayout.Background = new BitmapDrawable(App.Context.Resources, ThumbnailUtils.ExtractThumbnail(
                    await BitmapFactory.DecodeStreamAsync(panelView.Background), Width, Height, ThumnailExtractOptions.RecycleInput));
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }

            locationNameView.Text = panelView.LocationName;
            locationTempView.Text = panelView.CurrTemp;
            locationWeatherIcon.Text = panelView.WeatherIcon;
            Tag = panelView.Pair;

            Enabled = true;
            ShowLoading(false);
        }

        public void ShowLoading(bool show)
        {
            progressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
        }

        public override void SetBackgroundColor(Color color)
        {
            mainLayout.SetBackgroundColor(color);
        }
    }
}