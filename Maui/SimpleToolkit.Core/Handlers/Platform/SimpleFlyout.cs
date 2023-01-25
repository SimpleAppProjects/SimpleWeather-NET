#if WINDOWS

using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using WindowsThickness = Microsoft.UI.Xaml.Thickness;
using XamlStyle = Microsoft.UI.Xaml.Style;

// Partially based on the .NET MAUI Community Toolkit Popup control - https://github.com/CommunityToolkit/Maui

namespace SimpleToolkit.Core.Handlers
{
    public class SimpleFlyout : Flyout
    {
        private readonly IMauiContext mauiContext;
        private Action<Panel> panelCleanUp;
        private Func<PopoverHandler, Panel> createControl;

        internal Panel Control { get; set; }
        internal XamlStyle FlyoutStyle { get; private set; } = new(typeof(FlyoutPresenter));
        public IPopover VirtualView { get; private set; }


        public SimpleFlyout(IMauiContext mauiContext)
        {
            this.mauiContext = mauiContext ?? throw new ArgumentNullException(nameof(mauiContext));
        }


        public void SetElement(IPopover element)
        {
            VirtualView = element;
        }

        public void SetUpPlatformView(Action<Panel> panelCleanUp, Func<PopoverHandler, Panel> createControl)
        {
            ArgumentNullException.ThrowIfNull(panelCleanUp);
            ArgumentNullException.ThrowIfNull(createControl);

            this.panelCleanUp = panelCleanUp;
            this.createControl = createControl;

            CreateControl();
            ConfigureControl();
        }

        public void ConfigureControl()
        {
            if (VirtualView is null)
                return;

            FlyoutStyle = new XamlStyle(typeof(FlyoutPresenter));
            SetFlyoutStyle();
            SetLayout();
            ApplyStyles();
        }

        public void Show(IElement anchor)
        {
            if (VirtualView is null)
            {
                return;
            }

            if (anchor is not null)
            {
                var frameworkElement = anchor.ToPlatform(mauiContext);

                ShowAt(frameworkElement, new FlyoutShowOptions()
                {
                    Placement = FlyoutPlacementMode.Bottom,
                    ShowMode = FlyoutShowMode.Transient,
                });
            }
            else
            {
                ArgumentNullException.ThrowIfNull(VirtualView.Parent);
                var frameworkElement = VirtualView.Parent.ToPlatform(mauiContext);
                frameworkElement.ContextFlyout = this;

                ShowAt(frameworkElement, new FlyoutShowOptions()
                {
                    Placement = FlyoutPlacementMode.Bottom,
                    ShowMode = FlyoutShowMode.Transient,
                });
            }
        }

        public void CleanUp()
        {
            Hide();

            if (Control is not null)
                panelCleanUp?.Invoke(Control);

            VirtualView = null;
            Control = null;
        }

        private void CreateControl()
        {
            if (Control is null && VirtualView?.Content is not null && createControl is not null && VirtualView.Handler is PopoverHandler handler)
            {
                Control = createControl(handler);
                Content = Control;
            }
        }

        private void SetLayout()
        {
            LightDismissOverlayMode = LightDismissOverlayMode.Off;

            if (VirtualView is not null)
            {
                this.Placement = FlyoutPlacementMode.Bottom;
                this.ShouldConstrainToRootBounds = true;
                this.ShowMode = FlyoutShowMode.Transient;
            }
        }

        private void SetFlyoutStyle()
        {
            _ = VirtualView?.Content ?? throw new NullReferenceException(nameof(IPopover.Content));

            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FlyoutPresenter.BackgroundProperty, Colors.Transparent.ToWindowsColor()));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FlyoutPresenter.IsDefaultShadowEnabledProperty, false));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FlyoutPresenter.PaddingProperty, 0));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FlyoutPresenter.BorderThicknessProperty, new WindowsThickness(0)));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FlyoutPresenter.BorderBrushProperty, Colors.Transparent.ToWindowsColor()));

            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FlyoutPresenter.MinHeightProperty, 0));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FlyoutPresenter.MinWidthProperty, 0));

            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FrameworkElement.MaxHeightProperty, double.NaN));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(FrameworkElement.MaxWidthProperty, double.NaN));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(ScrollViewer.HorizontalScrollModeProperty, Microsoft.UI.Xaml.Controls.ScrollMode.Disabled));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Disabled));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(ScrollViewer.VerticalScrollModeProperty, Microsoft.UI.Xaml.Controls.ScrollMode.Disabled));
            FlyoutStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Disabled));
        }

        private void ApplyStyles()
        {
            if (Control is null)
                return;

            FlyoutPresenterStyle = FlyoutStyle;
        }
    }
}

#endif