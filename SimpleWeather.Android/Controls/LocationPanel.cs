using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Com.Bumptech.Glide;
using Android.OS;

namespace SimpleWeather.Droid.Controls
{
    public class LocationPanel : CardView
    {
        private View viewLayout;
        private ImageView bgImageView;
        private TextView locationNameView;
        private TextView locationTempView;
        private TextView locationWeatherIcon;
        private ProgressBar progressBar;
        public Drawable colorDrawable { get; private set; }

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

            bgImageView = viewLayout.FindViewById<ImageView>(Resource.Id.image_view);
            locationNameView = viewLayout.FindViewById<TextView>(Resource.Id.location_name);
            locationTempView = viewLayout.FindViewById<TextView>(Resource.Id.weather_temp);
            locationWeatherIcon = viewLayout.FindViewById<TextView>(Resource.Id.weather_icon);
            progressBar = viewLayout.FindViewById<ProgressBar>(Resource.Id.progressBar);
            colorDrawable = new ColorDrawable(new Color(ContextCompat.GetColor(context, Resource.Color.colorPrimary)));

            // NOTE: Bug: Explicitly set tintmode on Lollipop devices
            if (Build.VERSION.SdkInt == BuildVersionCodes.Lollipop)
                progressBar.IndeterminateTintMode = PorterDuff.Mode.SrcIn;

            // Remove extra (compat) padding on pre-Lollipop devices
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                PreventCornerOverlap = false;
                CardElevation = 0;
                MaxCardElevation = 0;
            }

            ShowLoading(true);
        }

        public void SetWeatherBackground(LocationPanelViewModel panelView)
        {
            // Background
            Glide.With(Context)
                 .Load(panelView.Background)
                 .CenterCrop()
                 .Error(colorDrawable)
                 .Placeholder(colorDrawable)
                 .Into(bgImageView);
        }

        public void SetWeather(LocationPanelViewModel panelView)
        {
            locationNameView.Text = panelView.LocationName;
            locationTempView.Text = panelView.CurrTemp;
            locationWeatherIcon.Text = panelView.WeatherIcon;
            Tag = panelView.LocationData;

            if (panelView.LocationName != null && Tag != null)
                ShowLoading(false);
        }

        public void ShowLoading(bool show)
        {
            progressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
            Clickable = show ? false : true;
        }
    }
}