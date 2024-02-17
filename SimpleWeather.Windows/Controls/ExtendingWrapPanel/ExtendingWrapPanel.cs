using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace SimpleWeather.NET.Controls
{
    // Based upon the WrapPanel control from the Microsoft Community Toolkit
    // https://github.com/windows-toolkit/WindowsCommunityToolkit/

    /// <summary>
    /// WrapPanel is a panel that position child control vertically or horizontally based on the orientation and when max width / max height is reached a new row (in case of horizontal) or column (in case of vertical) is created to fit new controls.
    /// </summary>
    public partial class ExtendingWrapPanel : Panel
    {
        /// <summary>
        /// Gets or sets a uniform Horizontal distance (in pixels) between items when <see cref="Orientation"/> is set to Horizontal,
        /// or between columns of items when <see cref="Orientation"/> is set to Vertical.
        /// </summary>
        public double HorizontalSpacing
        {
            get { return (double)GetValue(HorizontalSpacingProperty); }
            set { SetValue(HorizontalSpacingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalSpacingProperty =
            DependencyProperty.Register(
                nameof(HorizontalSpacing),
                typeof(double),
                typeof(ExtendingWrapPanel),
                new PropertyMetadata(0d, LayoutPropertyChanged));

        /// <summary>
        /// Gets or sets a uniform Vertical distance (in pixels) between items when <see cref="Orientation"/> is set to Vertical,
        /// or between rows of items when <see cref="Orientation"/> is set to Horizontal.
        /// </summary>
        public double VerticalSpacing
        {
            get { return (double)GetValue(VerticalSpacingProperty); }
            set { SetValue(VerticalSpacingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VerticalSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalSpacingProperty =
            DependencyProperty.Register(
                nameof(VerticalSpacing),
                typeof(double),
                typeof(ExtendingWrapPanel),
                new PropertyMetadata(0d, LayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the orientation of the WrapPanel.
        /// Horizontal means that child controls will be added horizontally until the width of the panel is reached, then a new row is added to add new child controls.
        /// Vertical means that children will be added vertically until the height of the panel is reached, then a new column is added.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(ExtendingWrapPanel),
                new PropertyMetadata(Orientation.Horizontal, LayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        /// <returns>
        /// The dimensions of the space between the border and its child as a Thickness value.
        /// Thickness is a structure that stores dimension values using pixel measures.
        /// </returns>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Identifies the Padding dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Padding"/> dependency property.</returns>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(ExtendingWrapPanel),
                new PropertyMetadata(default(Thickness), LayoutPropertyChanged));

        private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExtendingWrapPanel wp)
            {
                wp.InvalidateMeasure();
                wp.InvalidateArrange();
            }
        }

        private readonly List<Row> _rows = new List<Row>();

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            var childAvailableSize = new Size(
                availableSize.Width - Padding.Left - Padding.Right,
                availableSize.Height - Padding.Top - Padding.Bottom);
            foreach (var child in Children)
            {
                child.Measure(childAvailableSize);
            }

            var requiredSize = UpdateRows(availableSize);

            if (_rows.Count > 0)
            {
                // Need to re-measure row children with preferred size
                var childIndex = 0;
                foreach (var row in _rows)
                {
                    var rectCount = row.ChildrenRects.Count;
                    var rectIdx = 0;

                    foreach (var rect in row.ChildrenRects)
                    {
                        var child = Children[childIndex++];
                        while (child.Visibility == Visibility.Collapsed)
                        {
                            // Collapsed children are not added into the rows,
                            // we skip them.
                            child = Children[childIndex++];
                        }

                        // Re-measure child with desired size
                        child.Measure(rect.Size.ToSize(Orientation));

                        rectIdx++;
                    }
                }
            }

            return requiredSize;
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            if ((Orientation == Orientation.Horizontal && finalSize.Width < DesiredSize.Width) ||
                (Orientation == Orientation.Vertical && finalSize.Height < DesiredSize.Height))
            {
                // We haven't received our desired size. We need to refresh the rows.
                UpdateRows(finalSize);
            }

            if (_rows.Count > 0)
            {
                // Now that we have all the data, we do the actual arrange pass
                var childIndex = 0;
                foreach (var row in _rows)
                {
                    foreach (var rect in row.ChildrenRects)
                    {
                        var child = Children[childIndex++];
                        while (child.Visibility == Visibility.Collapsed)
                        {
                            // Collapsed children are not added into the rows,
                            // we skip them.
                            child = Children[childIndex++];
                        }

                        var arrangeRect = new UvRect
                        {
                            Position = rect.Position,
                            Size = new UvMeasure { U = rect.Size.U, V = row.Size.V },
                        };

                        var finalRect = arrangeRect.ToRect(Orientation);
                        child.Arrange(finalRect);
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("ExtendingWrapPanel: RowItem: {0}", finalRect);
#endif
                    }
                }
            }

            return finalSize;
        }

        private Size UpdateRows(Size availableSize)
        {
            _rows.Clear();

            var paddingStart = new UvMeasure(Orientation, Padding.Left, Padding.Top);
            var paddingEnd = new UvMeasure(Orientation, Padding.Right, Padding.Bottom);

            if (Children.Count == 0)
            {
                var emptySize = paddingStart.Add(paddingEnd).ToSize(Orientation);
                return emptySize;
            }

            var parentMeasure = new UvMeasure(Orientation, availableSize.Width, availableSize.Height);
            var spacingMeasure = new UvMeasure(Orientation, HorizontalSpacing, VerticalSpacing);
            var position = new UvMeasure(Orientation, Padding.Left, Padding.Top);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("ExtendingWrapPanel: Parent Measure: {0}", parentMeasure);
#endif

            var currentRow = new Row(new List<UvRect>(), default);
            var finalMeasure = new UvMeasure(Orientation, width: 0.0, height: 0.0);
            void Arrange(UIElement child, bool isLast = false)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    return; // if an item is collapsed, avoid adding the spacing
                }

                var desiredMeasure = new UvMeasure(Orientation, child.DesiredSize);
                if ((desiredMeasure.U + position.U + paddingEnd.U) > parentMeasure.U)
                {
                    ArrangeRowItems();

                    // next row!
                    position.U = paddingStart.U;
                    position.V += currentRow.Size.V + spacingMeasure.V;

                    _rows.Add(currentRow);
                    currentRow = new Row(new List<UvRect>(), default);
                }

                // Stretch the last item to fill the available space
                if (isLast)
                {
                    if (currentRow.ChildrenRects.Count > 0)
                    {
                        currentRow.Add(position, desiredMeasure);
                        ArrangeRowItems();
                    }
                    else
                    {
                        desiredMeasure.U = parentMeasure.U - position.U;
                        currentRow.Add(position, desiredMeasure);
                    }
                }
                else
                {
                    currentRow.Add(position, desiredMeasure);

                    // adjust the location for the next items
                    position.U += desiredMeasure.U + spacingMeasure.U;
                    finalMeasure.U = Math.Max(finalMeasure.U, position.U);
                }
            }
            void ArrangeRowItems()
            {
                double freeSpace = parentMeasure.U - (spacingMeasure.U * currentRow.ChildrenRects.Count) - paddingStart.U;
                double initFreeSpace = freeSpace;
                int expandingItems = currentRow.ChildrenRects.Count;
                if (currentRow.ChildrenRects.Count == 1)
                {
                    // Row item is already of desired size
                    var rowItem = currentRow.ChildrenRects[0];
                    rowItem.Position = new UvMeasure(Orientation, paddingStart.U, rowItem.Position.V);
                    rowItem.Size = new UvMeasure(Orientation, parentMeasure.U - paddingStart.U, rowItem.Size.V);
                    currentRow.ChildrenRects[0] = rowItem;
                }
                else
                {
                    var newPostition = UvMeasure.Zero; newPostition.U = paddingStart.U; newPostition.V = position.V;

                    double maxWidth = 0;
                    foreach (var rowItem in currentRow.ChildrenRects)
                    {
                        var desiredWidth = rowItem.ToRect(Orientation).Width;
                        maxWidth = Math.Max(maxWidth, desiredWidth);

                        if (desiredWidth > 0)
                        {
                            freeSpace -= desiredWidth;
                            expandingItems--;
                        }
                    }

                    bool fillSpace = false;
                    bool uniformWidth = false;
                    if (expandingItems <= 0 && freeSpace > 0)
                    {
                        expandingItems = currentRow.ChildrenRects.Count;
                        fillSpace = true;
                        if (maxWidth * currentRow.ChildrenRects.Count > 0)
                        {
                            uniformWidth = true;
                        }
                    }

                    for (int i = 0; i < currentRow.ChildrenRects.Count; i++)
                    {
                        var rowItem = currentRow.ChildrenRects[i];

                        var desiredMeasure = rowItem.Size;
                        if (desiredMeasure.U == 0)
                        {
                            return; // if an item is collapsed, avoid adding the spacing
                        }

                        if (fillSpace && freeSpace > 0 && expandingItems > 0)
                        {
                            if (uniformWidth)
                            {
                                desiredMeasure.U = initFreeSpace / currentRow.ChildrenRects.Count;
                            }
                            else
                            {
                                desiredMeasure.U += (freeSpace / expandingItems);
                            }
                        }

                        rowItem.Position = newPostition;
                        rowItem.Size = desiredMeasure;

                        // adjust the location for the next items
                        newPostition.U += desiredMeasure.U + spacingMeasure.U;

                        currentRow.ChildrenRects[i] = rowItem;
                    }
                }
            }

            for (var i = 0; i < Children.Count; i++)
            {
                Arrange(Children[i], i == Children.Count - 1);
            }

            if (currentRow.ChildrenRects.Count > 0)
            {
                _rows.Add(currentRow);
            }

            if (_rows.Count == 0)
            {
                var emptySize = paddingStart.Add(paddingEnd).ToSize(Orientation);
                return emptySize;
            }

            // Get max V here before computing final rect
            var lastRowRect = _rows.Last().Rect;
            finalMeasure.V = lastRowRect.Position.V + lastRowRect.Size.V;
            var finalRect = finalMeasure.Add(paddingEnd).ToSize(Orientation);
            return finalRect;
        }
    }
}