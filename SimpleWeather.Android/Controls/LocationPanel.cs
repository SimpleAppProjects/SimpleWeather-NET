using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Support.V4.Content;
using SimpleWeather.Droid.Utils;
using Com.Nostra13.Universalimageloader.Core;

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
        private ImageButton homeButton;
        private ImageLoader loader = ImageLoader.Instance;

        public LocationPanel(Context context) :
            base(context)
        {
            Initialize(context);
        }

        public LocationPanel(Context context, LocationPanelViewModel panelView) :
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

            locationNameView = viewLayout.FindViewById<TextView>(Resource.Id.location_name);
            locationTempView = viewLayout.FindViewById<TextView>(Resource.Id.weather_temp);
            locationWeatherIcon = viewLayout.FindViewById<WeatherIcon>(Resource.Id.weather_icon);
            progressBar = viewLayout.FindViewById<ProgressBar>(Resource.Id.progressBar);
            homeButton = viewLayout.FindViewById<ImageButton>(Resource.Id.home_button);

            ShowLoading(true);
        }

        public void SetWeather(LocationPanelViewModel panelView)
        {
            this.Post(() => 
            {
                // Background
                loader.DisplayImage(panelView.Background, new CustomViewAware(mainLayout),
                    ImageUtils.CenterCropConfig(mainLayout.Width, mainLayout.Height));

                locationNameView.Text = panelView.LocationName;
                locationTempView.Text = panelView.CurrTemp;
                locationWeatherIcon.Text = panelView.WeatherIcon;
                Tag = panelView.Pair;

                if (panelView.IsHome)
                {
                    homeButton.SetImageDrawable(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_home_fill_white_24dp));
                    homeButton.Enabled = false;
                    homeButton.Visibility = ViewStates.Visible;
                }

                ShowLoading(false);
            });
        }

        public void ShowLoading(bool show)
        {
            progressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
            Enabled = show ? false : true;
        }

        public override void SetBackgroundColor(Color color)
        {
            mainLayout.SetBackgroundColor(color);
        }
    }
}