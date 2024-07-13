using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using CoreGraphics;
using Foundation;
using MauiIcons.Core;
using MauiIcons.Cupertino;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Platform;
using SimpleToolkit.SimpleShell.Controls;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Utils;
using UIKit;

namespace SimpleWeather.Maui.Preferences
{
    public class PickerViewCell : ViewCell
	{
        public string PreferenceKey
        {
            get => (string)GetValue(PreferenceKeyProperty);
            set => SetValue(PreferenceKeyProperty, value);
        }

        public static readonly BindableProperty PreferenceKeyProperty =
            BindableProperty.Create(nameof(PreferenceKey), typeof(string), typeof(PickerViewCell), null);

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(PickerViewCell), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(PickerViewCell), default(string));

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(PickerViewCell), null);

        public string Detail
        {
            get { return (string)GetValue(DetailProperty); }
            set { SetValue(DetailProperty, value); }
        }

        public static readonly BindableProperty DetailProperty =
            BindableProperty.Create(nameof(Detail), typeof(string), typeof(PickerViewCell), default(string));

        public Color DetailColor
        {
            get { return (Color)GetValue(DetailColorProperty); }
            set { SetValue(DetailColorProperty, value); }
        }

        public static readonly BindableProperty DetailColorProperty =
            BindableProperty.Create(nameof(DetailColor), typeof(Color), typeof(PickerViewCell), null);

        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create(nameof(Items), typeof(IEnumerable), typeof(PickerViewCell), new List<PreferenceListItem>(0));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(PickerViewCell), null);

        private Button MenuButton;

        public PickerViewCell()
        {
            this.View = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                },
                MinimumHeightRequest = 48,
                MaximumHeightRequest = double.PositiveInfinity,
                Padding = new Thickness(16, 4),
                Children =
                {
                    new Label()
                    {
                        FontSize = 17,
                        FontFamily = Microsoft.Maui.Font.Default.Family
                    }
                    .Bind(Label.TextProperty, nameof(Text), BindingMode.OneWay, source: this)
                    .Bind(Label.TextColorProperty, nameof(TextColor), BindingMode.OneWay, source: this)
                    .Bind(Label.IsEnabledProperty, nameof(IsEnabled), BindingMode.OneWay, source: this)
                    .Bind<Label, bool, double>(Label.OpacityProperty, nameof(IsEnabled), BindingMode.OneWay, source: this,
                        convert: (enabled) =>
                        {
                            return enabled ? 1.0d : 0.5d;
                        }
                     )
                    .CenterVertical()
                    .Column(0)
                    .Row(0),
                    new PickerButton()
                    {
                        HorizontalOptions = LayoutOptions.End,
                        FontSize = 15,
                    }
                    .Bind(Button.TextColorProperty, nameof(DetailColor), BindingMode.OneWay, source: this)
                    .Bind(Button.TextProperty, nameof(Detail), BindingMode.TwoWay, source: this)
                    .Bind<Button, string, bool>(Button.IsVisibleProperty, nameof(Detail), BindingMode.OneWay, source: this,
                        convert: (s) =>
                        {
                            return !string.IsNullOrWhiteSpace(s?.ToString());
                        }
                    )
                    .Bind(Button.IsEnabledProperty, nameof(IsEnabled), BindingMode.OneWay, source: this)
                    .Bind(PickerButton.SelectedItemProperty, nameof(SelectedItem), BindingMode.TwoWay, source: this)
                    .Bind(PickerButton.ItemsProperty, nameof(Items), BindingMode.OneWay, source: this)
                    .Paddings()
                    .Margins()
                    .CenterVertical()
                    .Column(1)
                    .Row(0)
                    .Let(btn => MenuButton = btn)
                }
            };

            this.PropertyChanged += (s, e) =>
            {
                if (Equals(e.PropertyName, nameof(SelectedItem)))
                {
                    if (SelectedItem is PreferenceListItem item)
                    {
                        WeakReferenceMessenger.Default.Send(new SettingsChangedMessage(new SettingsChanged(PreferenceKey, item.Value)));
                    }
                    else
                    {
                        item = Items?.OfType<PreferenceListItem>()?.First(it => Equals(it.Value, SelectedItem));
                        WeakReferenceMessenger.Default.Send(new SettingsChangedMessage(new SettingsChanged(PreferenceKey, item?.Value)));
                    }
                }
                else if (Equals(e.PropertyName, nameof(IsEnabled)))
                {
                    if (IsEnabled)
                    {
                        MenuButton.Bind(Button.TextColorProperty, nameof(DetailColor), BindingMode.OneWay, source: this);
                    }
                    else
                    {
                        MenuButton.Bind<Button, Color, Color>(
                            Button.TextColorProperty, nameof(DetailColor), BindingMode.OneWay, source: this,
                            convert: c1 =>
                            {
                                return c1?.WithAlpha((float)0x30/0xff);
                            });
                    }
                }
            };

            this.View.TapGesture(OnTapped);
        }

        protected override void OnTapped()
        {
            if (!IsEnabled) return;

            base.OnTapped();
        }
    }
}