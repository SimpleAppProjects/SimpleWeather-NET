using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Wear.Widget.Drawer;
using Android.Views;
using Android.Widget;

namespace SimpleWeather.Droid.Wear
{
    [Activity(Label = "MainActivity", Theme = "@style/WearAppTheme")]
    public class MainActivity : Activity, IMenuItemOnMenuItemClickListener
    {
        private WearableActionDrawerView mWearableActionDrawer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            mWearableActionDrawer = FindViewById<WearableActionDrawerView>(Resource.Id.bottom_action_drawer);
            mWearableActionDrawer.SetOnMenuItemClickListener(this);
            mWearableActionDrawer.PeekOnScrollDownEnabled = true;

            // Create your application here
            Fragment fragment = FragmentManager.FindFragmentById(Resource.Id.fragment_container);

            // Check if fragment exists
            if (fragment == null)
            {
                fragment = new WeatherNowFragment();

                // Navigate to WeatherNowFragment
                FragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, fragment, "home")
                    .Commit();
            }
        }

        public override void OnBackPressed()
        {
            Fragment current = FragmentManager.FindFragmentById(Resource.Id.fragment_container);
            // Destroy untagged fragments onbackpressed
            if (current != null && current.Tag == null)
            {
                FragmentManager.BeginTransaction()
                    .Remove(current)
                    .Commit();
                current.OnDestroy();
                current = null;
            }
            base.OnBackPressed();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_changelocation:
                    StartActivity(new Intent(this, typeof(SetupActivity)));
                    break;
                case Resource.Id.menu_settings:
                    StartActivity(new Intent(this, typeof(SettingsActivity)));
                    break;
                case Resource.Id.menu_openonphone:
                    //
                    break;
            }

            return true;
        }
    }
}