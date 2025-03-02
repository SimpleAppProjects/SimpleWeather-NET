﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Maui.Behaviors;

namespace SimpleWeather.Maui.Controls.AppBar
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class AppBarItem : Grid
    {
        ToolbarItemOrder _toolbarItemOrder;
        Image _appbarItemIcon;
        Label _appbarItemText;
        string _originalText;
        RowDefinition _appbarItemIconRow;
        RowDefinition _appbarItemTextRow;

        public AppBarItem()
        {
            Initialize();
        }

        public ToolbarItem ToolbarItem { get; internal set; }

        public static readonly BindableProperty IconImageSourceProperty =
           BindableProperty.Create(nameof(IconImageSource), typeof(ImageSource), typeof(AppBarItem), null,
               propertyChanged: OnIconImageSourceChanged);

        public ImageSource IconImageSource
        {
            get => (ImageSource)GetValue(IconImageSourceProperty);
            set => SetValue(IconImageSourceProperty, value);
        }

        static void OnIconImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((AppBarItem)bindable).UpdateIconImageSource((ImageSource)newValue);
        }

        public static readonly BindableProperty TextProperty =
          BindableProperty.Create(nameof(Text), typeof(string), typeof(AppBarItem), string.Empty,
              propertyChanged: OnTextChanged);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((AppBarItem)bindable).UpdateText((string)newValue);
        }

        public static readonly BindableProperty TextColorProperty =
         BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(AppBarItem), Colors.Transparent,
             propertyChanged: OnTextColorChanged);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((AppBarItem)bindable).UpdateTextColor((Color)newValue);
        }

        public static readonly BindableProperty OrderProperty =
          BindableProperty.Create(nameof(Order), typeof(ToolbarItemOrder), typeof(AppBarItem), ToolbarItemOrder.Default,
              propertyChanged: OnOrderChanged);

        public ToolbarItemOrder Order
        {
            get => (ToolbarItemOrder)GetValue(OrderProperty);
            set => SetValue(OrderProperty, value);
        }

        static void OnOrderChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((AppBarItem)bindable).UpdateOrder((ToolbarItemOrder)newValue);
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(AppBarItem), null);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(AppBarItem), null);

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsEnabledProperty.PropertyName)
                UpdateIsEnabled();
        }

        void Initialize()
        {
            VerticalOptions = LayoutOptions.Center;
            Margin = new Thickness(6, 0);

            _appbarItemIconRow = new RowDefinition { Height = HeightRequest = AppBarSizes.GetToolBarItemSize() };
            RowDefinitions.Add(_appbarItemIconRow);

            _appbarItemTextRow = new RowDefinition { Height = GridLength.Auto };
            RowDefinitions.Add(_appbarItemTextRow);

            _appbarItemIcon = new Image
            {
                Aspect = Aspect.AspectFit,
                Source = IconImageSource,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            _appbarItemText = new Label
            {
                TextColor = TextColor,
                Text = Text,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            Children.Add(_appbarItemIcon);
            SetRow((IView)_appbarItemIcon, 0);

            Children.Add(_appbarItemText);
            SetRow((IView)_appbarItemText, 1);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnAppBarItemTapped;
            GestureRecognizers.Add(tapGestureRecognizer);

            Behaviors.Add(new TouchBehavior()
            {
                PressedScale = 0.95
            });
        }

        void UpdateIsEnabled()
        {
            if (IsEnabled)
                Opacity = 1d;
            else
                Opacity = 0.8d;
        }

        void UpdateIconImageSource(ImageSource icon)
        {
            _appbarItemIcon.Source = icon;
            UpdateLayout();
        }

        void UpdateText(string text)
        {
            _originalText = text;
            _appbarItemText.Text = _originalText;
            UpdateLayout();
        }

        void UpdateTextColor(Color textColor)
        {
            _appbarItemText.TextColor = textColor;
        }

        void UpdateOrder(ToolbarItemOrder toolbarItemOrder)
        {
            _toolbarItemOrder = toolbarItemOrder;
            UpdateLayout();
        }

        void UpdateLayout()
        {
            if (IconImageSource == null || IconImageSource.IsEmpty)
            {
                _appbarItemText.IsVisible = true;
                _appbarItemIconRow.Height = GridLength.Auto;
                _appbarItemTextRow.Height = AppBarSizes.GetToolBarItemSize();

                if (_toolbarItemOrder == ToolbarItemOrder.Secondary)
                    _appbarItemText.Text = _originalText;
                else
                {
                    if (DeviceInfo.Platform == DevicePlatform.Android && !string.IsNullOrEmpty(_originalText))
                        _appbarItemText.Text = _originalText.ToUpper();
                }
            }
            else
            {
                _appbarItemText.IsVisible = false;
                _appbarItemIconRow.Height = AppBarSizes.GetToolBarItemSize();
                _appbarItemTextRow.Height = GridLength.Auto;
            }
        }

        void OnAppBarItemTapped(object sender, EventArgs e)
        {
            OnTapped();
        }

        internal virtual void OnTapped()
        {
            // TODO: Add Ripple Effect on Android and animated highlighted view on iOS.

            if (ToolbarItem != null)
                ((IMenuItemController)ToolbarItem).Activate();

            if (Command != null)
                Command.Execute(CommandParameter);
        }
    }
}

