using System;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Java.Lang;

namespace SimpleWeather.Droid.App
{
    /**
     * A {@link android.preference.PreferenceActivity} which implements and proxies the necessary calls
     * to be used with AppCompat.
     */
    public abstract class AppCompatPreferenceActivity : PreferenceActivity
    {
        private AppCompatDelegate mDelegate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Delegate.InstallViewFactory();
            Delegate.OnCreate(savedInstanceState);
            base.OnCreate(savedInstanceState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            Delegate.OnPostCreate(savedInstanceState);
        }

        public ActionBar SupportActionBar => Delegate.SupportActionBar;

        public void SetSupportActionBar(Toolbar toolbar)
        {
            Delegate.SetSupportActionBar(toolbar);
        }

        public override MenuInflater MenuInflater => Delegate.MenuInflater;

        public override void SetContentView(int layoutResID)
        {
            Delegate.SetContentView(layoutResID);
        }

        public override void SetContentView(View view)
        {
            Delegate.SetContentView(view);
        }

        public override void SetContentView(View view, ViewGroup.LayoutParams @params)
        {
            Delegate.SetContentView(view, @params);
        }

        public override void AddContentView(View view, ViewGroup.LayoutParams @params)
        {
            Delegate.AddContentView(view, @params);
        }

        protected override void OnPostResume()
        {
            base.OnPostResume();
            Delegate.OnPostResume();
        }

        protected override void OnTitleChanged(ICharSequence title, Color color)
        {
            base.OnTitleChanged(title, color);
            Delegate.SetTitle(title);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            Delegate.OnConfigurationChanged(newConfig);
        }

        protected override void OnStop()
        {
            base.OnStop();
            Delegate.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Delegate.OnDestroy();
        }

        public override void InvalidateOptionsMenu()
        {
            Delegate.InvalidateOptionsMenu();
        }

        private AppCompatDelegate Delegate
        {
            get
            {
                if (mDelegate == null)
                {
                    mDelegate = AppCompatDelegate.Create(this, null);
                }
                return mDelegate;
            }
        }
    }
}