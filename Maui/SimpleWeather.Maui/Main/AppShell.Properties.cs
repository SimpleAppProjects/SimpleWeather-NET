using System;
namespace SimpleWeather.Maui.Main
{
	public partial class AppShell
	{
		public static readonly BindableProperty AppBarIsVisibleProperty =
			BindableProperty.Create("AppBarIsVisible", typeof(bool), typeof(AppShell), true);

        public static readonly BindableProperty BottomNavBarIsVisibleProperty =
            BindableProperty.Create("BottomNavBarIsVisible", typeof(bool), typeof(AppShell), true);

		public static bool GetAppBarIsVisible(BindableObject obj) => (bool)obj.GetValue(AppBarIsVisibleProperty);
        public static void SetAppBarIsVisible(BindableObject obj, bool value) => obj.SetValue(AppBarIsVisibleProperty, value);

        public static bool GetBottomNavBarIsVisible(BindableObject obj) => (bool)obj.GetValue(BottomNavBarIsVisibleProperty);
        public static void SetBottomNavBarIsVisible(BindableObject obj, bool value) => obj.SetValue(BottomNavBarIsVisibleProperty, value);
    }
}

