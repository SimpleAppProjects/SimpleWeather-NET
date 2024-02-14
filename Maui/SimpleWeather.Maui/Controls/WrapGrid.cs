using System;
using Microsoft.Maui.Layouts;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls
{
	public class WrapGrid : Grid
	{
        public int MaxColumns
        {
            get => (int)GetValue(MaxColumnsProperty);
            set => SetValue(MaxColumnsProperty, value);
        }

        public static readonly BindableProperty MaxColumnsProperty =
            BindableProperty.Create(nameof(MaxColumns), typeof(int), typeof(WrapGrid), 1,
                validateValue: (obj, newValue) =>
                {
                    return newValue is int number && number >= 1;
                },
                propertyChanged: (obj, oldValue, newValue) =>
                {
                    (obj as WrapGrid)?.InvalidateMeasure();
                }
            );

        public double MinItemWidth
        {
            get => (double)GetValue(MinItemWidthProperty);
            set => SetValue(MinItemWidthProperty, value);
        }

        public static readonly BindableProperty MinItemWidthProperty =
            BindableProperty.Create(nameof(MinItemWidth), typeof(double), typeof(WrapGrid), double.NaN,
                propertyChanged: (obj, oldValue, newValue) =>
                {
                    (obj as WrapGrid)?.InvalidateMeasure();
                }
            );

        public WrapGrid()
        {
            MaxColumns = 3;
            RowDefinitions = new RowDefinitionCollection(Enumerable.Repeat(new RowDefinition(), 5).ToArray());
        }

        protected override ILayoutManager CreateLayoutManager() => new WrapGridLayoutManager(this);

        private class WrapGridLayoutManager : GridLayoutManager
        {
            private WrapGrid WrapGrid => Grid as WrapGrid;
            private double MinItemWidth => WrapGrid.MinItemWidth;
            private int MaxColumns => WrapGrid.MaxColumns;
            private RowDefinitionCollection RowDefinitions => WrapGrid.RowDefinitions;
            private ColumnDefinitionCollection ColumnDefinitions => WrapGrid.ColumnDefinitions;

            public WrapGridLayoutManager(IGridLayout layout) : base(layout) { }

            public override Size Measure(double widthConstraint, double heightConstraint)
            {
                if (!double.IsPositiveInfinity(widthConstraint))
                {
                    int row = 0;
                    int col = 0;
                    int availableColumns;

                    var visibleChildren = Grid.Where(v => v is View view && view.IsVisible).Cast<View>().ToList();
                    var visibleCount = visibleChildren.Count;

                    if (double.IsNaN(MinItemWidth) || double.IsInfinity(MinItemWidth))
                    {
                        availableColumns = 1;
                    }
                    else
                    {
                        availableColumns = Math.Max(1, Math.Min(visibleCount, Math.Min(MaxColumns, (int)Math.Floor(widthConstraint / MinItemWidth))));
                    }

                    if (ColumnDefinitions == null || ColumnDefinitions.Count != availableColumns)
                    {
                        WrapGrid.ColumnDefinitions = new ColumnDefinitionCollection(Enumerable.Repeat(new ColumnDefinition(GridLength.Star), availableColumns).ToArray());
                    }

                    for (int i = 0; i < visibleChildren.Count; i++)
                    {
                        if (visibleChildren[i] is View view)
                        {
                            view.ClearValue(RowProperty);
                            view.ClearValue(ColumnProperty);
                            view.ClearValue(RowSpanProperty);
                            view.ClearValue(ColumnSpanProperty);

                            if (view.IsVisible)
                            {
                                // | 0 | 1 | 2 |
                                view.SetValue(RowProperty, row);
                                view.SetValue(ColumnProperty, col);
                                view.SetValue(RowSpanProperty, 1);
                                view.SetValue(ColumnSpanProperty, 1);

                                // islast - fill remaining space
                                if (i == visibleChildren.Count - 1)
                                {
                                    view.SetValue(RowSpanProperty, 1);
                                    view.SetValue(ColumnSpanProperty, Math.Max(1, availableColumns - col));
                                }
                                else if (col + 1 >= availableColumns)
                                {
                                    view.SetValue(ColumnSpanProperty, Math.Max(1, availableColumns - col));

                                    row++;
                                    col = 0;
                                }
                                else
                                {
                                    col++;
                                }
                            }
                        }
                    }

                    if (RowDefinitions == null || RowDefinitions.Count != (row + 1))
                    {
                        WrapGrid.RowDefinitions = new RowDefinitionCollection(Enumerable.Repeat(new RowDefinition(), row + 1).ToArray());
                    }
                }

                return base.Measure(widthConstraint, heightConstraint);
            }
        }
    }
}

