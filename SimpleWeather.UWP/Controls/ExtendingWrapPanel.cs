using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Controls
{
    // Based upon the WrapPanel control from the Microsoft Community Toolkit
    // https://github.com/windows-toolkit/WindowsCommunityToolkit/

    /// <summary>
    /// WrapPanel is a panel that position child control vertically or horizontally based on the orientation and when max width / max height is reached a new row (in case of horizontal) or column (in case of vertical) is created to fit new controls.
    /// </summary>
    public class ExtendingWrapPanel : Panel
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

        [System.Diagnostics.DebuggerDisplay("U = {U} V = {V}")]
        private struct UvMeasure
        {
            internal static readonly UvMeasure Zero = default(UvMeasure);

            internal double U { get; set; }

            internal double V { get; set; }

            public UvMeasure(Orientation orientation, double width, double height)
            {
                if (orientation == Orientation.Horizontal)
                {
                    U = width;
                    V = height;
                }
                else
                {
                    U = height;
                    V = width;
                }
            }
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize.Width = availableSize.Width - Padding.Left - Padding.Right;
            availableSize.Height = availableSize.Height - Padding.Top - Padding.Bottom;
            var totalMeasure = UvMeasure.Zero;
            var parentMeasure = new UvMeasure(Orientation, availableSize.Width, availableSize.Height);
            var spacingMeasure = new UvMeasure(Orientation, HorizontalSpacing, VerticalSpacing);
            var lineMeasure = UvMeasure.Zero;

            foreach (var child in Children)
            {
                child.Measure(availableSize);

                var currentMeasure = new UvMeasure(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                if (currentMeasure.U == 0)
                {
                    continue; // ignore collapsed items
                }

                // if this is the first item, do not add spacing. Spacing is added to the "left"
                double uChange = lineMeasure.U == 0
                    ? currentMeasure.U
                    : currentMeasure.U + spacingMeasure.U;
                if (parentMeasure.U >= uChange + lineMeasure.U)
                {
                    lineMeasure.U += uChange;
                    lineMeasure.V = Math.Max(lineMeasure.V, currentMeasure.V);
                }
                else
                {
                    // new line should be added
                    // to get the max U to provide it correctly to ui width ex: ---| or -----|
                    totalMeasure.U = Math.Max(lineMeasure.U, totalMeasure.U);
                    totalMeasure.V += lineMeasure.V + spacingMeasure.V;

                    // if the next new row still can handle more controls
                    if (parentMeasure.U > currentMeasure.U)
                    {
                        // set lineMeasure initial values to the currentMeasure to be calculated later on the new loop
                        lineMeasure = currentMeasure;
                    }

                    // the control will take one row alone
                    else
                    {
                        // validate the new control measures
                        totalMeasure.U = Math.Max(currentMeasure.U, totalMeasure.U);
                        totalMeasure.V += currentMeasure.V;

                        // add new empty line
                        lineMeasure = UvMeasure.Zero;
                    }
                }
            }

            // update value with the last line
            // if the the last loop is(parentMeasure.U > currentMeasure.U + lineMeasure.U) the total isn't calculated then calculate it
            // if the last loop is (parentMeasure.U > currentMeasure.U) the currentMeasure isn't added to the total so add it here
            // for the last condition it is zeros so adding it will make no difference
            // this way is faster than an if condition in every loop for checking the last item
            totalMeasure.U = Math.Max(lineMeasure.U, totalMeasure.U);
            totalMeasure.V += lineMeasure.V;

            totalMeasure.U = Math.Ceiling(totalMeasure.U);

            return Orientation == Orientation.Horizontal ? new Size(totalMeasure.U, totalMeasure.V) : new Size(totalMeasure.V, totalMeasure.U);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count > 0)
            {
                var parentMeasure = new UvMeasure(Orientation, finalSize.Width, finalSize.Height);
                var spacingMeasure = new UvMeasure(Orientation, HorizontalSpacing, VerticalSpacing);
                var paddingStart = new UvMeasure(Orientation, Padding.Left, Padding.Top);
                var paddingEnd = new UvMeasure(Orientation, Padding.Right, Padding.Bottom);
                var position = new UvMeasure(Orientation, Padding.Left, Padding.Top);

                double currentV = 0;
                var rowItems = new LinkedList<FrameworkElement>();
                void Arrange(FrameworkElement child, bool isLast = false)
                {
                    var desiredMeasure = new UvMeasure(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                    if (desiredMeasure.U == 0)
                    {
                        return; // if an item is collapsed, avoid adding the spacing
                    }

                    if ((desiredMeasure.U + position.U + paddingEnd.U) > parentMeasure.U)
                    {
                        ArrangeRowItems();

                        // next row!
                        position.U = paddingStart.U;
                        position.V += currentV + spacingMeasure.V;
                        currentV = 0;
                        rowItems.Clear();
                    }

                    if (isLast)
                    {
                        if (rowItems.Count > 0)
                        {
                            rowItems.AddLast(child);
                            ArrangeRowItems();
                        }
                        else
                        {
                            desiredMeasure.U = parentMeasure.U - position.U;
                            if (Orientation == Orientation.Horizontal)
                            {
                                child.Arrange(new Rect(position.U, position.V, desiredMeasure.U, desiredMeasure.V));
                            }
                            else
                            {
                                child.Arrange(new Rect(position.V, position.U, desiredMeasure.V, desiredMeasure.U));
                            }
                        }
                    }
                    else
                    {
                        rowItems.AddLast(child);

                        // adjust the location for the next items
                        position.U += desiredMeasure.U + spacingMeasure.U;
                        currentV = Math.Max(desiredMeasure.V, currentV);
                    }
                }
                void ArrangeRowItems()
                {
                    double freeSpace = parentMeasure.U - (spacingMeasure.U * rowItems.Count) - paddingStart.U;
                    double initFreeSpace = freeSpace;
                    int expandingItems = rowItems.Count;
                    if (rowItems.Count == 1)
                    {
                        var rowItem = rowItems.First.Value;
                        var desiredMeasure = new UvMeasure(Orientation, rowItem.DesiredSize.Width, rowItem.DesiredSize.Height);
                        // place the item
                        if (Orientation == Orientation.Horizontal)
                        {
                            rowItem.Arrange(new Rect(paddingStart.U, position.V, parentMeasure.U - paddingStart.U, desiredMeasure.V));
                        }
                        else
                        {
                            rowItem.Arrange(new Rect(position.V, paddingStart.U, desiredMeasure.V, parentMeasure.U - paddingStart.U));
                        }
                    }
                    else
                    {
                        var newPostition = UvMeasure.Zero; newPostition.U = paddingStart.U; newPostition.V = position.V;

                        double maxWidth = 0;
                        foreach (var rowItem in rowItems)
                        {
                            maxWidth = Math.Max(maxWidth, rowItem.DesiredSize.Width);

                            if (rowItem.DesiredSize.Width > 0)
                            {
                                freeSpace -= rowItem.DesiredSize.Width;
                                expandingItems--;
                            }
                        }

                        bool fillSpace = false;
                        bool uniformWidth = false;
                        if (expandingItems <= 0 && freeSpace > 0)
                        {
                            expandingItems = rowItems.Count;
                            fillSpace = true;
                            if (maxWidth * rowItems.Count > 0)
                            {
                                uniformWidth = true;
                            }
                        }

                        foreach (var rowItem in rowItems)
                        {
                            var desiredMeasure = new UvMeasure(Orientation, rowItem.DesiredSize.Width, rowItem.DesiredSize.Height);
                            if (desiredMeasure.U == 0)
                            {
                                return; // if an item is collapsed, avoid adding the spacing
                            }

                            if (fillSpace && freeSpace > 0 && expandingItems > 0)
                            {
                                if (uniformWidth)
                                {
                                    desiredMeasure.U = initFreeSpace / rowItems.Count;
                                }
                                else
                                {
                                    desiredMeasure.U += (freeSpace / expandingItems);
                                }
                            }

                            // place the item
                            if (Orientation == Orientation.Horizontal)
                            {
                                rowItem.Arrange(new Rect(newPostition.U, newPostition.V, desiredMeasure.U, desiredMeasure.V));
                            }
                            else
                            {
                                rowItem.Arrange(new Rect(newPostition.V, newPostition.U, desiredMeasure.V, desiredMeasure.U));
                            }

                            // adjust the location for the next items
                            newPostition.U += desiredMeasure.U + spacingMeasure.U;
                        }
                    }
                }

                for (var i = 0; i < Children.Count; i++)
                {
                    Arrange(Children[i] as FrameworkElement, i == Children.Count - 1);
                }
            }

            return finalSize;
        }
    }
}