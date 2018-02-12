using System;
using System.Collections.Generic;
using System.Linq;

using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Content;
using System.Threading.Tasks;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.Droid.Widgets;

namespace SimpleWeather.Droid
{
    [Android.App.Activity(Label = "@string/title_activity_weather_now", Theme = "@style/AppTheme",
        ClearTaskOnLaunch = true, FinishOnTaskLaunch = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Create your application here
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            Android.Support.V7.App.ActionBarDrawerToggle toggle = new Android.Support.V7.App.ActionBarDrawerToggle(
                    this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            // Back stack listener
            SupportFragmentManager.BackStackChanged += delegate { RefreshNavViewCheckedItem(); };

            Fragment fragment = SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container);

            // Alerts
            if (WeatherWidgetService.ACTION_SHOWALERTS.Equals(Intent?.Action))
            {
                Fragment newFragment = WeatherNowFragment.NewInstance(Intent.Extras);

                if (fragment == null)
                {
                    fragment = newFragment;
                    // Navigate to WeatherNowFragment
                    // Make sure we exit if location is not home
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.fragment_container, fragment, "notification")
                        .Commit();
                }
                else
                {
                    // Navigate to WeatherNowFragment
                    // Make sure we exit if location is not home
                    SupportFragmentManager.BeginTransaction()
                        .Add(Resource.Id.fragment_container, newFragment)
                        .AddToBackStack(null)
                        .Commit();
                }
            }

            // Check if fragment exists
            if (fragment == null)
            {
                if (Intent.HasExtra("data"))
                    fragment = WeatherNowFragment.NewInstance(Intent.Extras);
                else
                    fragment = new WeatherNowFragment();

                // Navigate to WeatherNowFragment
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, fragment, "home")
                    .Commit();
            }

            if ((bool)Intent?.HasExtra("shortcut-data"))
            {
                var locData = LocationData.FromJson(
                    new Newtonsoft.Json.JsonTextReader(
                        new System.IO.StringReader(Intent.GetStringExtra("shortcut-data"))));

                // Navigate to WeatherNowFragment
                Fragment newFragment = WeatherNowFragment.NewInstance(locData);

                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, newFragment, "shortcut")
                    .Commit();

                // Disable navigation
                toggle.DrawerIndicatorEnabled = false;
                drawer.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }

            navigationView.SetCheckedItem(Resource.Id.nav_weathernow);

            await Task.Run(() => Shortcuts.ShortcutCreator.UpdateShortcuts());
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                Fragment current = SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container);
                // Destroy untagged fragments onbackpressed
                if (current != null && current.Tag == null)
                {
                    SupportFragmentManager.BeginTransaction()
                        .Remove(current)
                        .Commit();
                    current.OnDestroy();
                    current = null;
                }
                base.OnBackPressed();
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            // Handle navigation view item clicks here.
            int id = item.ItemId;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
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
                    if (fragment is WeatherNowFragment)
                    {
                        // Pop all since we're going home
                        fragment = null;
                        transaction.Commit();
                        SupportFragmentManager.PopBackStackImmediate(null, (int)Android.App.PopBackStackFlags.Inclusive);
                    }
                    else if (fragment is LocationsFragment)
                    {
                        Fragment current = SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container);

                        if (current is WeatherNowFragment)
                        {
                            // Hide home frag
                            if (current.Tag == "home")
                                transaction.Hide(current);
                            else
                            {
                                // Destroy lingering WNow frag
                                SupportFragmentManager.BeginTransaction()
                                    .Remove(current)
                                    .Commit();
                                current.OnDestroy();
                                current = null;
                                SupportFragmentManager.PopBackStack();
                            }
                        }

                        if (SupportFragmentManager.FindFragmentByTag("locations") != null)
                        {
                            // Pop all frags if LocFrag in backstack
                            fragment = null;
                            transaction.Commit();
                        }
                        else
                        {
                            // Add LocFrag if not in backstack
                            // Commit the transaction
                            transaction
                                .Add(Resource.Id.fragment_container, fragment, "locations")
                                .AddToBackStack(null)
                                .Commit();
                        }
                    }
                }
            }

            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        protected override void OnResumeFragments()
        {
            base.OnResumeFragments();
            RefreshNavViewCheckedItem();
        }

        private void RefreshNavViewCheckedItem()
        {
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            Fragment fragment = SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container);

            if (fragment is WeatherNowFragment)
            {
                navigationView.SetCheckedItem(Resource.Id.nav_weathernow);
                SupportActionBar.Title = GetString(Resource.String.title_activity_weather_now);
            }
            else if (fragment is LocationsFragment)
            {
                navigationView.SetCheckedItem(Resource.Id.nav_locations);
                SupportActionBar.Title = GetString(Resource.String.label_nav_locations);
            }
            else if (fragment is WeatherAlertsFragment)
            {
                navigationView.SetCheckedItem(Resource.Id.nav_weathernow);
                SupportActionBar.Title = GetString(Resource.String.title_fragment_alerts);
            }

            if (fragment is WeatherAlertsFragment)
            {
                SupportActionBar.Hide();
                drawer.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            else
            {
                if (!SupportActionBar.IsShowing)
                    SupportActionBar.Show();

                if ((bool)Intent?.HasExtra("shortcut-data"))
                    drawer.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
                else
                    drawer.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            }
        }
    }
}