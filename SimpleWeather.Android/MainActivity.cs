using System;

using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Content;
using SimpleWeather.Droid.Utils;
using SimpleWeather.Utils;

namespace SimpleWeather.Droid
{
    [Android.App.Activity(Label = "@string/title_activity_weather_now",
        Name = "SimpleWeather.Droid.MainActivity", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Create your application here
            Toolbar toolbar = (Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = (DrawerLayout)FindViewById(Resource.Id.drawer_layout);
            Android.Support.V7.App.ActionBarDrawerToggle toggle = new Android.Support.V7.App.ActionBarDrawerToggle(
                    this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.SetDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = (NavigationView)FindViewById(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            // Back stack listener
            SupportFragmentManager.BackStackChanged += delegate { refreshNavViewCheckedItem(); };

            Fragment fragment = SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container);

            // Check if fragment exists
            if (fragment == null)
            {
                if (Intent.HasExtra("pair"))
                {
                    Pair<int, string> pair = JSONParser.Deserializer(Intent.GetStringExtra("pair"), typeof(Pair<int, string>)) as Pair<int, string>;
                    fragment = WeatherNowFragment.NewInstance(pair);
                }
                else
                    fragment = new WeatherNowFragment();

                // Navigate to WeatherNowFragment
                SupportFragmentManager.BeginTransaction().Replace(
                        Resource.Id.fragment_container, fragment).Commit();
            }

            navigationView.SetCheckedItem(Resource.Id.nav_weathernow);
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = (DrawerLayout)FindViewById(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            // Handle navigation view item clicks here.
            int id = item.ItemId;

            DrawerLayout drawer = (DrawerLayout)FindViewById(Resource.Id.drawer_layout);
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            Fragment fragment = null;

            if (id == Resource.Id.nav_weathernow)
            {
                fragment = new WeatherNowFragment();
            }
            else if (id == Resource.Id.nav_locations)
            {
                fragment = new LocationsFragment();
            }
            else if (id == Resource.Id.nav_settings)
            {
                StartActivity(new Intent(this, typeof(SettingsActivity)));
                drawer.CloseDrawer(GravityCompat.Start);
                return false;
            }

            if (fragment != null)
            {
                if (fragment.Class != SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container).Class)
                {
                    // Replace whatever is in the fragment_container view with this fragment,
                    // and add the transaction to the back stack so the user can navigate back
                    transaction.Replace(Resource.Id.fragment_container, fragment);

                    if ((fragment as WeatherNowFragment) != null)
                    {
                        // Pop all since we're going home
                        transaction.Commit();
                        SupportFragmentManager.PopBackStackImmediate(null, (int)Android.App.PopBackStackFlags.Inclusive);
                    }
                    else
                    {
                        // Commit the transaction
                        transaction.AddToBackStack(null).Commit();
                    }
                }
            }

            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        protected override void OnResumeFragments()
        {
            base.OnResumeFragments();
            refreshNavViewCheckedItem();
        }

        private void refreshNavViewCheckedItem()
        {
            NavigationView navigationView = (NavigationView)FindViewById(Resource.Id.nav_view);
            Fragment fragment = SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container);

            if ((fragment as WeatherNowFragment) != null)
            {
                navigationView.SetCheckedItem(Resource.Id.nav_weathernow);
                SupportActionBar.Title = GetString(Resource.String.title_activity_weather_now);
            }
            else if ((fragment as LocationsFragment) != null)
            {
                navigationView.SetCheckedItem(Resource.Id.nav_locations);
                SupportActionBar.Title = GetString(Resource.String.label_nav_locations);
            }
        }
    }
}