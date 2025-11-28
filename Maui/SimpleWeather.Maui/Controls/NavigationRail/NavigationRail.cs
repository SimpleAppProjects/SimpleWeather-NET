using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using SimpleToolkit.Core;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls
{
	public class NavigationRail : ContentView
	{
        public IEnumerable<BaseShellItem> Items
        {
            get => (IEnumerable<BaseShellItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create(nameof(Items), typeof(IEnumerable<BaseShellItem>), typeof(NavigationRail), default, propertyChanged: OnItemsChanged);

        public BaseShellItem SelectedItem
        {
            get => (BaseShellItem)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(BaseShellItem), typeof(NavigationRail), default, propertyChanged: OnSelectedItemChanged);

        public Color IconColor
        {
            get => (Color)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        public static readonly BindableProperty IconColorProperty =
			BindableProperty.Create(nameof(IconColor), typeof(Color), typeof(NavigationRail), propertyChanged: OnIconColorChanged, defaultValue: Colors.Black);

        public Color IconSelectionColor
        {
            get => (Color)GetValue(IconSelectionColorProperty);
            set => SetValue(IconSelectionColorProperty, value);
        }

        public static readonly BindableProperty IconSelectionColorProperty =
			BindableProperty.Create(nameof(IconSelectionColor), typeof(Color), typeof(NavigationRail), propertyChanged: OnIconSelectionColorChanged, defaultValue: Colors.Black);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(NavigationRail), propertyChanged: OnTextColorChanged, defaultValue: Colors.Black);

        public Color TextSelectionColor
        {
            get => (Color)GetValue(TextSelectionColorProperty);
            set => SetValue(TextSelectionColorProperty, value);
        }

        public static readonly BindableProperty TextSelectionColorProperty =
			BindableProperty.Create(nameof(TextSelectionColor), typeof(Color), typeof(NavigationRail), propertyChanged: OnTextSelectionColorChanged, defaultValue: Colors.Black);

        public Color SelectionBackgroundColor
        {
            get => (Color)GetValue(SelectionBackgroundColorProperty);
            set => SetValue(SelectionBackgroundColorProperty, value);
        }

        public static readonly BindableProperty SelectionBackgroundColorProperty =
            BindableProperty.Create(nameof(SelectionBackgroundColor), typeof(Color), typeof(NavigationRail), propertyChanged: OnSelectionBackgroundColorPropertyChanged, defaultValue: Colors.Transparent);

        public event NavigationRailItemSelectedEventHandler ItemSelected;

        private VerticalStackLayout ListContainer;

        private DataTemplate ItemTemplate;

        private IEnumerable<IView> NavigationRailViews
		{
			get => ListContainer.Children;
        }

        public IEnumerable<BaseShellItem> AllItems
        {
            get => Items;
        }

        public NavigationRail()
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
                    MinimumHeightRequest = 60,
					BackgroundColor = Colors.Transparent,
					Margin = new Thickness(4, 2),
					Padding = new Thickness(0, 8),
					BorderColor = Colors.Transparent,
					Content = new Grid()
					{
						RowDefinitions = 
						{
                            new RowDefinition(GridLength.Star),
                            new RowDefinition(GridLength.Star),
						},
						Children =
						{
							new Icon()
							{
								HeightRequest = 24,
								WidthRequest = 24
							}
                            .CenterHorizontal()
                            .CenterVertical()
							.Bind(Icon.SourceProperty, static (BaseShellItem item) => item.Icon)
							.Apply(it =>
							{
								it.Bind(Icon.TintColorProperty, static src => src.SelectedItem, mode: BindingMode.OneWay, source: this,
									convert: (_) =>
									{
										return IsSelected(it) ? IconSelectionColor : IconColor;
									}
								);
                                it.Bind(Grid.RowSpanProperty, static src => src.SelectedItem, mode: BindingMode.OneWay, source: this,
                                    convert: (_) =>
                                    {
                                        return IsSelected(it) ? 1 : 2;
                                    }
                                );
                            })
                            .Row(0),
							new Label()
							{
								FontSize = 12,
								VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
								LineBreakMode = LineBreakMode.CharacterWrap,
							}
                            .CenterHorizontal()
                            .CenterVertical()
							.Bind(Label.TextProperty, static (BaseShellItem item) => item.Title)
                            .Apply(it =>
                            {
                                it.Bind(IsVisibleProperty, static src => src.SelectedItem, mode: BindingMode.OneWay, source: this,
                                    convert: _ => IsSelected(it));
                            })
                            .Row(1)
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
                WidthRequest = WidthRequest,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star)
                },
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                },
                Children =
                {
                    new VerticalStackLayout()
                        .Bind(BindableLayout.ItemsSourceProperty, static src => src.Items, mode: BindingMode.OneWay,
                            source: this)
                        .ItemTemplate(ItemTemplate)
                        .Apply(it => ListContainer = it)
                        .Row(0),
                }
            };
        }

        private void ShellItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != BaseShellItem.TitleProperty.PropertyName && e.PropertyName != BaseShellItem.IconProperty.PropertyName)
                return;

            var shellItem = sender as BaseShellItem;

			foreach (var item in NavigationRailViews.Cast<Frame>())
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
            var sideBar = bindable as NavigationRail;

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
            var sideBar = bindable as NavigationRail;

            foreach (var item in sideBar.NavigationRailViews.Cast<Frame>())
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
            var sideBar = bindable as NavigationRail;

            foreach (var item in sideBar.NavigationRailViews.Cast<Frame>())
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
            var sideBar = bindable as NavigationRail;

            foreach (var item in sideBar.NavigationRailViews.Cast<Frame>())
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
            var sideBar = bindable as NavigationRail;

            foreach (var item in sideBar.NavigationRailViews.Cast<Frame>())
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
            var sideBar = bindable as NavigationRail;

            foreach (var item in sideBar.NavigationRailViews.Cast<Frame>())
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
            var sideBar = bindable as NavigationRail;

            if (sideBar.ListContainer is VerticalStackLayout stackLayout && stackLayout.Parent is ScrollView scrollView)
            {
                var element = stackLayout.Children.Cast<Element>().FirstOrDefault(e => e.BindingContext == newValue);

                if (element is not null)
                    _ = scrollView.ScrollToAsync(element, ScrollToPosition.MakeVisible, true);
            }

            sideBar.UpdateItems();
        }

        private void UpdateItems()
        {
            foreach (var item in NavigationRailViews.Cast<Frame>())
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

			ItemSelected?.Invoke(sender, new NavigationRailItemSelectedEventArgs()
			{
				ShellItem = element.BindingContext as BaseShellItem
			});
		}
    }

    public delegate void NavigationRailItemSelectedEventHandler(object sender, NavigationRailItemSelectedEventArgs e);

    public class NavigationRailItemSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// The selected <see cref="BaseShellItem"/> item.
        /// </summary>
        public BaseShellItem ShellItem { get; internal set; }
    }
}

