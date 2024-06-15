#if false
using System;
using System.ComponentModel;
using System.Reflection;
using BingMapsRESTToolkit;
using Microsoft.Maui;

namespace SimpleWeather.Maui.Controls
{
	public class SplitViewPage : Page
	{
		private Page _pane1;
        private Rect _pane1Bounds;

        private Page _pane2;
        private Rect _pane2Bounds;

        public SplitViewPage()
        {
            DeviceDisplay.MainDisplayInfoChanged += (s, e) =>
            {
                bool isPortrait = e.DisplayInfo.Height > e.DisplayInfo.Width;

                if (isPortrait)
                    FlyoutLayoutBehavior = FlyoutLayoutBehavior.Default;
                else
                    FlyoutLayoutBehavior = FlyoutLayoutBehavior.Split;
            };
        }

        public FlyoutLayoutBehavior FlyoutLayoutBehavior
        {
            get { return (FlyoutLayoutBehavior)GetValue(FlyoutLayoutBehaviorProperty); }
            set { SetValue(FlyoutLayoutBehaviorProperty, value); }
        }

        public static readonly BindableProperty FlyoutLayoutBehaviorProperty =
            BindableProperty.Create(nameof(FlyoutLayoutBehavior), typeof(FlyoutLayoutBehavior), typeof(SplitViewPage), default(FlyoutLayoutBehavior));

        public Page Pane1
		{
			get { return _pane1; }
            set
			{
                if (_pane1 != null && value == null)
                    throw new ArgumentNullException(nameof(value), "Flyout cannot be set to null once a value is set");

                if (_pane1 == value)
                    return;

                if (value.RealParent != null)
                    throw new InvalidOperationException("Flyout must not already have a parent.");

                // TODO MAUI refine this to fire earlier
                var previousPane = _pane1;
                // TODO MAUI refine this to fire earlier
                _pane1?.SendNavigatingFrom(new NavigatingFromEventArgs());

                OnPropertyChanging();
                if (_pane1 != null)
                    InternalChildren.Remove(_pane1);
                _pane1 = value;
                InternalChildren.Add(_pane1);
                OnPropertyChanged();

                if (this.HasAppeared())
                {
                    previousPane?.SendDisappearing();
                    _pane1?.SendAppearing();
                }

                previousPane?.SendNavigatedFrom(_pane1.CreateNavigatedFromEventArgs());
                _pane1?.SendNavigatedTo(previousPane.CreateNavigatedToEventArgs());
            }
		}

		public Page Pane2
		{
			get { return _pane2; }
            set
            {
                if (_pane2 != null && value == null)
                    throw new ArgumentNullException(nameof(value), "Detail cannot be set to null once a value is set.");

                if (_pane2 == value)
                    return;

                if (value.RealParent != null)
                    throw new InvalidOperationException("Detail must not already have a parent.");

                var previousPane = _pane2;
                // TODO MAUI refine this to fire earlier
                _pane2?.SendNavigatingFrom(new NavigatingFromEventArgs());

                OnPropertyChanging();
                if (_pane2 != null)
                    InternalChildren.Remove(_pane2);
                _pane2 = value;
                InternalChildren.Add(_pane2);
                OnPropertyChanged();

                if (this.HasAppeared())
                {
                    previousPane?.SendDisappearing();
                    _pane2?.SendAppearing();
                }

                previousPane?.SendNavigatedFrom(_pane2.CreateNavigatedFromEventArgs());
                _pane2?.SendNavigatedTo(previousPane.CreateNavigatedToEventArgs());
            }
		}

        internal Rect Pane1Bounds
        {
            get { return _pane1Bounds; }
            set
            {
                _pane1Bounds = value;
                if (_pane1 == null)
                    throw new InvalidOperationException("Pane1 must be set before using a FlyoutPage");
                _pane1.Layout(value);
            }
        }

        internal Rect Pane2Bounds
        {
            get { return _pane2Bounds; }
            set
            {
                _pane2Bounds = value;
                if (_pane2 == null)
                    throw new InvalidOperationException("Pane2 must be set before using a FlyoutPage");
                _pane2.Layout(value);
            }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (Pane1 == null || Pane2 == null)
                throw new InvalidOperationException("Pane1 and Pane2 must be set before using a FlyoutPage");

            _pane1.Layout(_pane1Bounds);
            _pane2.Layout(_pane2Bounds);
        }

        protected override void OnAppearing()
        {
            Pane1?.SendAppearing();
            Pane2?.SendAppearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            Pane1?.SendDisappearing();
            Pane2?.SendDisappearing();
            base.OnDisappearing();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            bool isPortrait = height > width;

            if (isPortrait)
                FlyoutLayoutBehavior = FlyoutLayoutBehavior.Default;
            else
                FlyoutLayoutBehavior = FlyoutLayoutBehavior.Split;
        }
    }

    internal static class PageExtensions
    {
        internal static void SendNavigatingFrom(this Page page, NavigatingFromEventArgs e)
        {
            var t = typeof(Page);

            try
            {
                var methods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                var method = methods.FirstOrDefault(m => m.Name == "SendNavigatingFrom");
                method?.Invoke(page, new[] { e });
            }
            catch { }
        }

        internal static void SendNavigatedFrom(this Page page, NavigatedFromEventArgs e)
        {
            var t = typeof(Page);

            try
            {
                var methods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                var method = methods.FirstOrDefault(m => m.Name == "SendNavigatedFrom");
                method?.Invoke(page, new[] { e });
            }
            catch { }
        }

        internal static void SendNavigatedTo(this Page page, NavigatedToEventArgs e)
        {
            var t = typeof(Page);

            try
            {
                var methods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                var method = methods.FirstOrDefault(m => m.Name == "SendNavigatedTo");
                method?.Invoke(page, new[] { e });
            }
            catch { }
        }

        internal static bool HasAppeared(this Page page)
        {
            var t = typeof(Page);

            try
            {
                var member = t.GetMember("HasAppeared", MemberTypes.Field | MemberTypes.Property, BindingFlags.NonPublic | BindingFlags.Instance);
                if (member.FirstOrDefault() is FieldInfo field)
                {
                    return (bool) field.GetValue(page);
                }
                else if (member.FirstOrDefault() is PropertyInfo prop)
                {
                    return (bool) prop.GetValue(page);
                }
            }
            catch { }

            return true;
        }

#nullable enable
        internal static NavigatedFromEventArgs? CreateNavigatedFromEventArgs(this Page? page)
        {
            return Activator.CreateInstance(typeof(NavigatedFromEventArgs), BindingFlags.NonPublic | BindingFlags.Instance, null, args: new[] { page }, null) as NavigatedFromEventArgs;
        }

        internal static NavigatedToEventArgs? CreateNavigatedToEventArgs(this Page? page)
        {
            return Activator.CreateInstance(typeof(NavigatedToEventArgs), BindingFlags.NonPublic | BindingFlags.Instance, null, args: new[] { page }, null) as NavigatedToEventArgs;
        }
#nullable restore
    }
}
#endif
