
using Android.Content;
using Android.OS;
using Android.Content.Res;
using Android.Support.V7.App;
using Android.Views;
using Android.Support.V4.App;
using Android.Preferences;

namespace SimpleWeather.Droid
{
    [Android.App.Activity(Label = "@string/title_activity_settings", Theme = "@style/SettingsTheme",
        ParentActivity = typeof(MainActivity))]
    public class SettingsActivity : AppCompatPreferenceActivity
    {
        /**
         * Helper method to determine if the device has an extra-large screen. For
         * example, 10" tablets are extra-large.
         */
        private static bool IsXLargeTablet(Context context)
        {
            return (context.Resources.Configuration.ScreenLayout
                    & ScreenLayout.SizeMask) >= ScreenLayout.SizeXlarge;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetupActionBar();

            // Display the fragment as the main content.
            FragmentManager.BeginTransaction()
                    .Replace(Android.Resource.Id.Content, new SettingsFragment())
                    .Commit();
        }

        /**
         * Set up the {@link android.app.ActionBar}, if the API is available.
         */
        private void SetupActionBar()
        {
            ActionBar actionBar = SupportActionBar;
            if (actionBar != null)
            {
                // Show the Up button in the action bar.
                actionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Android.Resource.Id.Home)
            {
                if (!base.OnMenuItemSelected(featureId, item))
                {
                    Intent intent = NavUtils.GetParentActivityIntent(this);
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    NavUtils.NavigateUpTo(this, intent);
                }
                return true;
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        /**
         * {@inheritDoc}
         */
        public override bool OnIsMultiPane()
        {
            return IsXLargeTablet(this);
        }

        /**
         * This method stops fragment injection in malicious applications.
         * Make sure to deny any unknown fragments here.
         */
        protected override bool IsValidFragment(string fragmentName)
        {
            return typeof(PreferenceFragment).FullName.Equals(fragmentName)
                || typeof(SettingsFragment).FullName.Equals(fragmentName);
        }

        /**
         * This fragment shows general preferences only. It is used when the
         * activity is showing a two-pane settings UI.
         */
        [Android.Annotation.TargetApi(Value = (int)BuildVersionCodes.Honeycomb)]
        public class SettingsFragment : PreferenceFragment
        {
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.pref_general);
                SetHasOptionsMenu(true);
            }
        }
    }
}