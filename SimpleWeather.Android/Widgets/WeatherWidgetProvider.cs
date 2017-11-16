using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Appwidget;

namespace SimpleWeather.Droid.Widgets
{
    public enum WidgetType
    {
        Unknown = -1,
        Widget1x1 = 0,
        Widget2x2,
        Widget4x1,
        Widget4x2
    }

    public abstract class WeatherWidgetProvider : AppWidgetProvider
    {
        private static string TAG = "WeatherWidgetProvider";

        // Actions
        public const string ACTION_SHOWREFRESH = "SimpleWeather.Droid.action.SHOWREFRESH";
        public const string ACTION_REFRESHWIDGETS = "SimpleWeather.Droid.action.UPDATEWIDGETS";

        // Extras
        public const string EXTRA_WIDGET_ID = "SimpleWeather.Droid.extra.WIDGET_ID";
        public const string EXTRA_WIDGET_IDS = "SimpleWeather.Droid.extra.WIDGET_IDS";
        public const string EXTRA_WIDGET_OPTIONS = "SimpleWeather.Droid.extra.WIDGET_OPTIONS";
        public const string EXTRA_WIDGET_TYPE = "SimpleWeather.Droid.extra.WIDGET_TYPE";

        // Fields
        public abstract WidgetType WidgetType { get; }
        public abstract int WidgetLayoutId { get; }
        protected abstract string ClassName { get; }
        public abstract ComponentName ComponentName { get; }

        // Methods
        public abstract bool HasInstances(Context context);

        public override void OnReceive(Context context, Intent intent)
        {
            if (Intent.ActionBootCompleted.Equals(intent.Action))
            {
                // Reset weather update time
                SimpleWeather.Utils.Settings.UpdateTime = DateTime.MinValue;
                // Restart update alarm
                WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                    .SetAction(WeatherWidgetService.ACTION_STARTALARM));
            }
            else if (Intent.ActionLocaleChanged.Equals(intent.Action))
            {
                UpdateWidgets(context, null);
            }
            else if (ACTION_SHOWREFRESH.Equals(intent.Action))
            {
                // Update widgets
                int[] appWidgetIds = intent.GetIntArrayExtra(EXTRA_WIDGET_IDS);
                ShowRefresh(context, appWidgetIds);
            }
            else if (ACTION_REFRESHWIDGETS.Equals(intent.Action))
            {
                // Update widgets
                int[] appWidgetIds = intent.GetIntArrayExtra(EXTRA_WIDGET_IDS);
                UpdateWidgets(context, appWidgetIds);
            }
            else if (AppWidgetManager.ActionAppwidgetUpdate.Equals(intent.Action))
            {
                UpdateWidgets(context, null);
            }
            else
            {
                Console.WriteLine("{0}: Unhandled action: {1}", TAG, intent.Action);
                base.OnReceive(context, intent);
            }
        }

        protected void ShowRefresh(Context context, int[] appWidgetIds)
        {
            var appWidgetManager = AppWidgetManager.GetInstance(context);
            var componentname = new ComponentName(context.PackageName, ClassName);
            if (appWidgetIds == null || appWidgetIds.Length == 0)
                appWidgetIds = appWidgetManager.GetAppWidgetIds(componentname);

            RemoteViews views = new RemoteViews(context.PackageName, WidgetLayoutId);
            views.SetViewVisibility(Resource.Id.refresh_button, ViewStates.Gone);
            views.SetViewVisibility(Resource.Id.refresh_progress, ViewStates.Visible);
            appWidgetManager.PartiallyUpdateAppWidget(appWidgetIds, views);
        }

        protected void UpdateWidgets(Context context, int[] appWidgetIds)
        {
            ShowRefresh(context, appWidgetIds);

            WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                .SetAction(WeatherWidgetService.ACTION_REFRESHWIDGET)
                .PutExtra(EXTRA_WIDGET_IDS, appWidgetIds)
                .PutExtra(EXTRA_WIDGET_TYPE, (int)WidgetType));
        }

        public override void OnUpdate(Context context,
            AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            UpdateWidgets(context, appWidgetIds);
        }

        public override void OnEnabled(Context context)
        {
            // Schedule alarms/updates
            WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                .SetAction(WeatherWidgetService.ACTION_STARTALARM));
        }

        public override void OnDisabled(Context context)
        {
            // Remove alarms/updates
            WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                .SetAction(WeatherWidgetService.ACTION_CANCELALARM));
        }

        public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                .SetAction(WeatherWidgetService.ACTION_RESIZEWIDGET)
                .PutExtra(EXTRA_WIDGET_ID, appWidgetId)
                .PutExtra(EXTRA_WIDGET_OPTIONS, newOptions)
                .PutExtra(EXTRA_WIDGET_TYPE, (int)WidgetType));
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            base.OnDeleted(context, appWidgetIds);
        }
    }

    [BroadcastReceiver(Label = "Weather (1x1)")]
    [IntentFilter(new string[]
    {
        AppWidgetManager.ActionAppwidgetUpdate,
        Intent.ActionBootCompleted,
        Intent.ActionLocaleChanged,
        ACTION_REFRESHWIDGETS
    })]
    [MetaData("android.appwidget.provider", Resource = "@xml/app_widget_1x1_info")]
    public class WeatherWidgetProvider1x1 : WeatherWidgetProvider
    {
        // Overrides
        public override WidgetType WidgetType => WidgetType.Widget1x1;
        public override int WidgetLayoutId => Resource.Layout.app_widget_1x1;
        protected override string ClassName => Java.Lang.Class.FromType(this.GetType()).Name;
        public override ComponentName ComponentName => new ComponentName(App.Context.PackageName, ClassName);

        private static WeatherWidgetProvider1x1 Instance;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static WeatherWidgetProvider1x1 GetInstance()
        {
            if (Instance == null)
                Instance = new WeatherWidgetProvider1x1();

            return Instance;
        }

        public override bool HasInstances(Context context)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(
                new ComponentName(context, ClassName));
            return (appWidgetIds.Length > 0);
        }
    }

    [BroadcastReceiver(Label = "Weather (2x2)")]
    [IntentFilter(new string[]
    {
        AppWidgetManager.ActionAppwidgetUpdate,
        Intent.ActionBootCompleted,
        Intent.ActionLocaleChanged,
        ACTION_REFRESHWIDGETS
    })]
    [MetaData("android.appwidget.provider", Resource = "@xml/app_widget_2x2_info")]
    public class WeatherWidgetProvider2x2 : WeatherWidgetProvider
    {
        // Overrides
        public override WidgetType WidgetType => WidgetType.Widget2x2;
        public override int WidgetLayoutId => Resource.Layout.app_widget_2x2;
        protected override string ClassName => Java.Lang.Class.FromType(this.GetType()).Name;
        public override ComponentName ComponentName => new ComponentName(App.Context.PackageName, ClassName);

        private static WeatherWidgetProvider2x2 Instance;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static WeatherWidgetProvider2x2 GetInstance()
        {
            if (Instance == null)
                Instance = new WeatherWidgetProvider2x2();

            return Instance;
        }

        public override bool HasInstances(Context context)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(
                new ComponentName(context, ClassName));
            return (appWidgetIds.Length > 0);
        }
    }

    [BroadcastReceiver(Label = "Weather (4x1)")]
    [IntentFilter(new string[]
    {
        AppWidgetManager.ActionAppwidgetUpdate,
        Intent.ActionBootCompleted,
        Intent.ActionLocaleChanged,
        ACTION_REFRESHWIDGETS
    })]
    [MetaData("android.appwidget.provider", Resource = "@xml/app_widget_4x1_info")]
    public class WeatherWidgetProvider4x1 : WeatherWidgetProvider
    {
        // Overrides
        public override WidgetType WidgetType => WidgetType.Widget4x1;
        public override int WidgetLayoutId => Resource.Layout.app_widget_4x1;
        protected override string ClassName => Java.Lang.Class.FromType(this.GetType()).Name;
        public override ComponentName ComponentName => new ComponentName(App.Context.PackageName, ClassName);

        private static WeatherWidgetProvider4x1 Instance;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static WeatherWidgetProvider4x1 GetInstance()
        {
            if (Instance == null)
                Instance = new WeatherWidgetProvider4x1();

            return Instance;
        }

        public override bool HasInstances(Context context)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(
                new ComponentName(context, ClassName));
            return (appWidgetIds.Length > 0);
        }
    }

    [BroadcastReceiver(Label = "Weather + Clock (4x2)")]
    [IntentFilter(new string[]
    {
        AppWidgetManager.ActionAppwidgetUpdate,
        Intent.ActionBootCompleted,
        Intent.ActionTimeChanged,
        Intent.ActionLocaleChanged,
        Intent.ActionTimezoneChanged,
        ACTION_REFRESHWIDGETS
    })]
    [MetaData("android.appwidget.provider", Resource = "@xml/app_widget_4x2_info")]
    public class WeatherWidgetProvider4x2 : WeatherWidgetProvider
    {
        // Overrides
        public override WidgetType WidgetType => WidgetType.Widget4x2;
        public override int WidgetLayoutId => Resource.Layout.app_widget_4x2;
        protected override string ClassName => Java.Lang.Class.FromType(this.GetType()).Name;
        public override ComponentName ComponentName => new ComponentName(App.Context.PackageName, ClassName);

        private static WeatherWidgetProvider4x2 Instance;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static WeatherWidgetProvider4x2 GetInstance()
        {
            if (Instance == null)
                Instance = new WeatherWidgetProvider4x2();

            return Instance;
        }

        public override bool HasInstances(Context context)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(
                new ComponentName(context, ClassName));
            return (appWidgetIds.Length > 0);
        }

        public override void OnEnabled(Context context)
        {
            // Register tick receiver
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr1)
            {
                WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                    .SetAction(WeatherWidgetService.ACTION_STARTCLOCK));
            }

            base.OnEnabled(context);
        }

        public override void OnDisabled(Context context)
        {
            // Unregister tick receiver
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr1)
            {
                WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                    .SetAction(WeatherWidgetService.ACTION_CANCELCLOCK));
            }

            base.OnDisabled(context);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            if (Intent.ActionTimeChanged.Equals(action)
                || Intent.ActionTimezoneChanged.Equals(action))
            {
                // Update clock widget
                var appWidgetManager = AppWidgetManager.GetInstance(context);
                var componentname = new ComponentName(context.PackageName, ClassName);
                int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(componentname);

                WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                    .SetAction(WeatherWidgetService.ACTION_UPDATECLOCK)
                    .PutExtra(EXTRA_WIDGET_IDS, appWidgetIds)
                    .PutExtra(EXTRA_WIDGET_TYPE, (int)WidgetType));
            }
            else if (Intent.ActionDateChanged.Equals(action))
            {
                // Update clock widget
                var appWidgetManager = AppWidgetManager.GetInstance(context);
                var componentname = new ComponentName(context.PackageName, ClassName);
                int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(componentname);

                WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                    .SetAction(WeatherWidgetService.ACTION_UPDATEDATE)
                    .PutExtra(EXTRA_WIDGET_IDS, appWidgetIds)
                    .PutExtra(EXTRA_WIDGET_TYPE, (int)WidgetType));
            }
            else
                base.OnReceive(context, intent);
        }
    }
}