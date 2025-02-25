using System.Collections;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using MauiIcons.Core;
using SimpleWeather.Maui.Preferences;
using UIKit;

namespace SimpleWeather.Maui.Controls
{
    public class PickerButton : Button
	{
        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create(nameof(Items), typeof(IEnumerable), typeof(PickerButton), default);

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(PickerButton), null);

        public PickerButton()
        {
            ImageSource = MauiIcons.Material.MaterialIcons.UnfoldMore.ToImageSource(iconColor: Colors.White, iconSize: 24, iconAutoScaling: true)
                .Bind(FontImageSource.ColorProperty, static color => color.TextColor, mode: BindingMode.OneWay, source: this);
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            UpdateMenuItems();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (Equals(propertyName, nameof(Items)))
            {
                UpdateMenuItems();
            }
            else if (Equals(propertyName, nameof(SelectedItem)))
            {
                if (SelectedItem is PreferenceListItem item)
                {
                    Text = item.Display;
                }
                else
                {
                    item = Items?.OfType<PreferenceListItem>()?.First(it => Equals(it.Value, SelectedItem));
                    Text = item?.Display;
                }

                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(250), InvalidateMeasure);
            }
        }

        private void UpdateMenuItems()
        {
            if (Handler is IPlatformViewHandler handler && handler.PlatformView is UIButton btn)
            {
                btn.Menu = UIMenu.Create(Items?.OfType<PreferenceListItem>()?.Select(item =>
                {
                    var element = UIAction.Create(item.Display, image: null, identifier: null, (i) =>
                    {
                        SelectedItem = item.Value;
                    });

                    element.State = Equals(item.Value, SelectedItem) ? UIMenuElementState.On : UIMenuElementState.Off;

                    return element;
                })?.ToArray() ?? Array.Empty<UIMenuElement>());
                btn.ChangesSelectionAsPrimaryAction = true;
                btn.ShowsMenuAsPrimaryAction = true;
                btn.SetTitle(Text, UIControlState.Normal);
            }
        }
    }
}

