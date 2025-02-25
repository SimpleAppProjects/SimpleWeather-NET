using System;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Markup;
using Foundation;
using MauiIcons.Core;
using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell.Controls;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls
{
	public class SideBar : ContentView
	{
        public IEnumerable<BaseShellItem> Items
        {
            get => (IEnumerable<BaseShellItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create(nameof(Items), typeof(IEnumerable<BaseShellItem>), typeof(SideBar), default, propertyChanged: OnItemsChanged);

        public IEnumerable<BaseShellItem> FooterItems
        {
            get => (IEnumerable<BaseShellItem>)GetValue(FooterItemsProperty);
            set => SetValue(FooterItemsProperty, value);
        }

        public static readonly BindableProperty FooterItemsProperty =
            BindableProperty.Create(nameof(FooterItems), typeof(IEnumerable<BaseShellItem>), typeof(SideBar), default, propertyChanged: OnItemsChanged);

        public BaseShellItem SelectedItem
        {
            get => (BaseShellItem)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(BaseShellItem), typeof(SideBar), default, propertyChanged: OnSelectedItemChanged);

        public Color IconColor
        {
            get => (Color)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        public static readonly BindableProperty IconColorProperty =
			BindableProperty.Create(nameof(IconColor), typeof(Color), typeof(SideBar), propertyChanged: OnIconColorChanged, defaultValue: Colors.Black);

        public Color IconSelectionColor
        {
            get => (Color)GetValue(IconSelectionColorProperty);
            set => SetValue(IconSelectionColorProperty, value);
        }

        public static readonly BindableProperty IconSelectionColorProperty =
			BindableProperty.Create(nameof(IconSelectionColor), typeof(Color), typeof(SideBar), propertyChanged: OnIconSelectionColorChanged, defaultValue: Colors.Black);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SideBar), propertyChanged: OnTextColorChanged, defaultValue: Colors.Black);

        public Color TextSelectionColor
        {
            get => (Color)GetValue(TextSelectionColorProperty);
            set => SetValue(TextSelectionColorProperty, value);
        }

        public static readonly BindableProperty TextSelectionColorProperty =
			BindableProperty.Create(nameof(TextSelectionColor), typeof(Color), typeof(SideBar), propertyChanged: OnTextSelectionColorChanged, defaultValue: Colors.Black);

        public Color SelectionBackgroundColor
        {
            get => (Color)GetValue(SelectionBackgroundColorProperty);
            set => SetValue(SelectionBackgroundColorProperty, value);
        }

        public static readonly BindableProperty SelectionBackgroundColorProperty =
            BindableProperty.Create(nameof(SelectionBackgroundColor), typeof(Color), typeof(SideBar), propertyChanged: OnSelectionBackgroundColorPropertyChanged, defaultValue: Colors.Transparent);

        public bool IsExpanded
		{
			get => (bool)GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value);
		}

		public static readonly BindableProperty IsExpandedProperty =
			BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(SideBar), false, propertyChanged: OnExpandedChanged);

		public double CollapsedWidth
		{
			get => (double)GetValue(CollapsedWidthProperty);
			set => SetValue(CollapsedWidthProperty, value);
		}

		public static readonly BindableProperty CollapsedWidthProperty =
			BindableProperty.Create(nameof(CollapsedWidth), typeof(double), typeof(SideBar), 48d, propertyChanged: OnRequestedWidthChanged);

		public double ExpandedWidth
		{
			get => (double)GetValue(ExpandedWidthProperty);
			set => SetValue(ExpandedWidthProperty, value);
		}

		public static readonly BindableProperty ExpandedWidthProperty =
			BindableProperty.Create(nameof(ExpandedWidth), typeof(double), typeof(SideBar), 240d, propertyChanged: OnRequestedWidthChanged);

        public double ExpandButtonHeight
        {
            get => (double)GetValue(ExpandButtonHeightProperty);
            set => SetValue(ExpandButtonHeightProperty, value);
        }

        public static readonly BindableProperty ExpandButtonHeightProperty =
            BindableProperty.Create(nameof(ExpandButtonHeight), typeof(double), typeof(SideBar), 48d);

        public event SideBarItemSelectedEventHandler ItemSelected;

        private VerticalStackLayout ListContainer;
        private VerticalStackLayout FooterContainer;

        private DataTemplate ItemTemplate;

        private IEnumerable<IView> SideBarViews
		{
			get => Enumerable.Concat(ListContainer.Children, FooterContainer.Children);
        }

        public IEnumerable<BaseShellItem> AllItems
        {
            get
            {
                if (Items?.Any() == true && FooterItems?.Any() == true)
                {
                    return Enumerable.Concat(Items, FooterItems);
                }
                else
                {
                    return Items ?? FooterItems;
                }
            }
        }

        public SideBar()
		{
			Initialize();
		}

        private void Initialize()
        {
			ItemTemplate = new DataTemplate(() =>
			{
				return new Frame()
				{
					CornerRadius = 8,
					BackgroundColor = Colors.Transparent,
					Margin = new Thickness(4, 2),
					Padding = new Thickness(0, 8),
					BorderColor = Colors.Transparent,
					Content = new Grid()
					{
						ColumnDefinitions =
						{
							new ColumnDefinition(new GridLength(CollapsedWidth))
								.Bind(ColumnDefinition.WidthProperty, static src => src.CollapsedWidth, mode: BindingMode.OneWay, source: this),
							new ColumnDefinition(GridLength.Star)
						},
                        Margin = new Thickness(-4, 0, 0, 0),
						Children =
						{
							new Icon()
							{
								HeightRequest = 24,
								WidthRequest = 24
							}
							.Bind(Icon.SourceProperty, static (BaseShellItem item) => item.Icon)
							.Apply(it =>
							{
								it.Bind(Icon.TintColorProperty, static src => src.SelectedItem, mode: BindingMode.OneWay, source: this,
									convert: (_) =>
									{
										return IsSelected(it) ? IconSelectionColor : IconColor;
									}
								);
                            })
                            .Column(0),
							new Label()
							{
								Margin = new Thickness(16, 0, 0, 0),
								FontSize = 16,
								VerticalOptions = LayoutOptions.Center,
								LineBreakMode = LineBreakMode.NoWrap
							}
							.Bind(Label.TextProperty, static (BaseShellItem item) => item.Title)
                            .Column(1)
						}
					}
				}
                .Behaviors(
                    new TouchBehavior()
                        .Bind(TouchBehavior.HoveredBackgroundColorProperty, static src => src.SelectionBackgroundColor, mode: BindingMode.OneWay, source: this)
                        .Bind(
                            TouchBehavior.PressedBackgroundColorProperty, static src => src.SelectionBackgroundColor, mode: BindingMode.OneWay, source: this,
                            convert: (color) =>
                            {
                                return color.WithLuminosity(0.2f);
                            }
                        )
                )
                .Apply(it =>
				{
					it.TapGesture(() =>
					{
						ItemClicked(it, EventArgs.Empty);
					});
				});
			});

            Content = new Grid()
            {
                MinimumWidthRequest = 48,
                WidthRequest = IsExpanded ? ExpandedWidth : CollapsedWidth,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star)
                },
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                },
                Children =
                {
                    new Frame()
                    {
                        CornerRadius = 8,
                        BackgroundColor = Colors.Transparent,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(4, 6),
                        Padding = new Thickness(0, 8),
                        BorderColor = Colors.Transparent,
                        WidthRequest = 44,
                        Content = new Grid()
                        {
                            Margin = new Thickness(0, 0, 0, 0),
                            Children =
                            {
                                new Icon()
                                {
                                    HeightRequest = 24,
                                    WidthRequest = 24,
                                    Source = MauiIcons.Cupertino.CupertinoIcons.SidebarLeft.ToImageSource(iconSize: 24)
                                }
                                .Bind(Icon.TintColorProperty, static src => src.IconColor, mode: BindingMode.OneWay, source: this),
                            }
                        },
                    }
                    .Behaviors(
                        new TouchBehavior()
                            .Bind(TouchBehavior.HoveredBackgroundColorProperty, static src => src.SelectionBackgroundColor, mode: BindingMode.OneWay, source: this)
                            .Bind(
                                TouchBehavior.PressedBackgroundColorProperty, static src => src.SelectionBackgroundColor, mode: BindingMode.OneWay, source: this,
                                convert: (color) =>
                                {
                                    return color.WithLuminosity(0.2f);
                                }
                            )
                    )
                    .TapGesture(() =>
                    {
                        IsExpanded = !IsExpanded;
                    }),
                    new VerticalStackLayout()
                        .Bind(BindableLayout.ItemsSourceProperty, static src => src.Items, mode: BindingMode.OneWay, source: this)
                        .ItemTemplate(ItemTemplate)
                        .Apply(it => ListContainer = it)
                        .Row(1),
                    new VerticalStackLayout()
						.Bind(BindableLayout.ItemsSourceProperty, static src => src.FooterItems, mode: BindingMode.OneWay, source: this)
						.ItemTemplate(ItemTemplate)
                        .Margins(bottom: 8)
                        .Apply(it => FooterContainer = it)
                        .Row(2)
                }
            }
			.Bind(MinimumWidthRequestProperty, static src => src.CollapsedWidth, mode: BindingMode.OneWay, source: this);
        }

        private void ShellItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != BaseShellItem.TitleProperty.PropertyName && e.PropertyName != BaseShellItem.IconProperty.PropertyName)
                return;

            var shellItem = sender as BaseShellItem;

			foreach (var item in SideBarViews.Cast<Frame>())
            {
                var grid = item.Content as Grid;
                var icon = grid.Children[0] as Icon;
                var label = grid.Children[1] as Label;

                label.Text = shellItem.Title;
                icon.Source = shellItem.Icon;

				if (IsSelected(item))
				{
					icon.Bind(Icon.TintColorProperty, static src => src.IconSelectionColor, mode: BindingMode.OneWay, source: this);
                }
				else
				{
                    icon.Bind(Icon.TintColorProperty, static src => src.IconColor, mode: BindingMode.OneWay, source: this);
                }
            }
        }

        private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var oldItems = oldValue as IEnumerable<BaseShellItem>;
            var newItems = newValue as IEnumerable<BaseShellItem>;
            var sideBar = bindable as SideBar;

            if (oldItems is not null)
            {
                foreach (var item in oldItems)
                {
                    item.PropertyChanged -= sideBar.ShellItemPropertyChanged;
                }
            }
            if (newItems is not null)
            {
                foreach (var item in newItems)
                {
                    item.PropertyChanged += sideBar.ShellItemPropertyChanged;
                }
            }

            sideBar.UpdateItems();
        }

        private static void OnIconColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sideBar = bindable as SideBar;

            foreach (var item in sideBar.SideBarViews.Cast<Frame>())
            {
                if (sideBar.IsSelected(item))
                    continue;

                var grid = item.Content as Grid;
                var icon = grid.Children[0] as Icon;

                if (newValue is not null)
                    icon.TintColor = newValue as Color;
            }
        }

        private static void OnIconSelectionColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sideBar = bindable as SideBar;

            foreach (var item in sideBar.SideBarViews.Cast<Frame>())
            {
                if (!sideBar.IsSelected(item))
                    continue;

                var grid = item.Content as Grid;
                var icon = grid.Children[0] as Icon;

                if (newValue is not null)
                    icon.TintColor = newValue as Color;
            }
        }

        private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sideBar = bindable as SideBar;

            foreach (var item in sideBar.SideBarViews.Cast<Frame>())
            {
                if (sideBar.IsSelected(item))
                    continue;

                var grid = item.Content as Grid;
                var label = grid.Children[1] as Label;

                if (newValue is not null)
                    label.TextColor = newValue as Color;
            }
        }

        private static void OnTextSelectionColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sideBar = bindable as SideBar;

            foreach (var item in sideBar.SideBarViews.Cast<Frame>())
            {
                if (!sideBar.IsSelected(item))
                    continue;

                var grid = item.Content as Grid;
                var label = grid.Children[1] as Label;

                if (newValue is not null)
                    label.TextColor = newValue as Color;
            }
        }

        private static void OnSelectionBackgroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sideBar = bindable as SideBar;

            foreach (var item in sideBar.SideBarViews.Cast<Frame>())
            {
                item.BackgroundColor = sideBar.IsSelected(item) ? sideBar.SelectionBackgroundColor : Colors.Transparent;
                if (item.Behaviors?.FirstOrDefault() is TouchBehavior touchBehavior)
                {
                    touchBehavior.DefaultBackgroundColor = sideBar.IsSelected(item) ? sideBar.SelectionBackgroundColor : Colors.Transparent;
                }
            }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sideBar = bindable as SideBar;

            if (sideBar.ListContainer is VerticalStackLayout stackLayout && stackLayout.Parent is ScrollView scrollView)
            {
                var element = stackLayout.Children.Cast<Element>().FirstOrDefault(e => e.BindingContext == newValue);

                if (element is not null)
                    _ = scrollView.ScrollToAsync(element, ScrollToPosition.MakeVisible, true);
            }

            sideBar.UpdateItems();
        }

        private static void OnExpandedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sideBar = bindable as SideBar;
            sideBar.Content.WidthRequest = (bool)newValue ? sideBar.ExpandedWidth : sideBar.CollapsedWidth;
        }

        private static void OnRequestedWidthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sideBar = bindable as SideBar;
            sideBar.Content.WidthRequest = sideBar.IsExpanded ? sideBar.ExpandedWidth : sideBar.CollapsedWidth;
        }

        private void UpdateItems()
        {
            foreach (var item in SideBarViews.Cast<Frame>())
            {
                var grid = item.Content as Grid;
                var icon = grid.Children[0] as Icon;
                var label = grid.Children[1] as Label;

                icon.TintColor = IsSelected(item) ? IconSelectionColor : IconColor;
                label.TextColor = IsSelected(item) ? TextSelectionColor : TextColor;
                item.BackgroundColor = IsSelected(item) ? SelectionBackgroundColor : Colors.Transparent;
                if (item.Behaviors?.FirstOrDefault() is TouchBehavior touchBehavior)
                {
                    touchBehavior.DefaultBackgroundColor = IsSelected(item) ? SelectionBackgroundColor : Colors.Transparent;
                }
            }
        }

        private bool IsSelected(BindableObject bindable)
		{
			return bindable.BindingContext == SelectedItem;
		}

		private void ItemClicked(object sender, EventArgs e)
		{
			var element = sender as BindableObject;

			ItemSelected?.Invoke(sender, new SideBarItemSelectedEventArgs()
			{
				ShellItem = element.BindingContext as BaseShellItem
			});
		}
    }

    public delegate void SideBarItemSelectedEventHandler(object sender, SideBarItemSelectedEventArgs e);

    public class SideBarItemSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// The selected <see cref="BaseShellItem"/> item.
        /// </summary>
        public BaseShellItem ShellItem { get; internal set; }
    }
}

