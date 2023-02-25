using System.Diagnostics;
using Microsoft.Maui.Layouts;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls.Flow
{
    public sealed class FlowLayoutManager : LayoutManager
    {
        IFlowLayout FlowLayout { get; }

        public FlowLayoutManager(IFlowLayout flowLayout)
            : base(flowLayout)
        {
            FlowLayout = flowLayout;
        }

        private class Row
        {
            public double RowStart { get; set; } = 0;
            public double RowTop { get; set; } = 0;
            public double RowMaxBotton { get; set; } = 0;

            private readonly List<IView> _Items = new List<IView>();

            public IReadOnlyList<IView> Items => _Items;

            public Row(double rowStart, double rowTop)
            {
                this.RowStart = rowStart;
                this.RowTop = rowTop;
            }

            public void Add(IView child)
            {
                _Items.Add(child);
            }
        }

        private readonly List<Row> Rows = new List<Row>();

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            var width = widthConstraint;
            var height = heightConstraint;

            var maxWidth = widthConstraint - FlowLayout.Padding.HorizontalThickness;
            var maxHeight = heightConstraint - FlowLayout.Padding.VerticalThickness;

            var maxRight = maxWidth - FlowLayout.Padding.Right;

            this.FlowLayout.ForEach(view =>
            {
                if (view.Visibility is Visibility.Visible)
                {
                    view.Measure(view.MinimumWidth, maxHeight);
                }
            });

            var requiredSize = UpdateRows(FlowLayout.Padding.Left, maxRight);

            var finalHeight = ResolveConstraints(heightConstraint, FlowLayout.Height, requiredSize.Height, FlowLayout.MinimumHeight, FlowLayout.MaximumHeight);
            var finalWidth = ResolveConstraints(widthConstraint, FlowLayout.Width, requiredSize.Width, FlowLayout.MinimumWidth, FlowLayout.MaximumWidth);

            return new Size(finalWidth, finalHeight);
        }

        private Size UpdateRows(double parentStart, double parentEnd)
        {
            Rows.Clear();

            if (FlowLayout.Count == 0)
            {
                return new Size(FlowLayout.Padding.HorizontalThickness, FlowLayout.Padding.VerticalThickness);
            }

            var isRtl = FlowLayout.FlowDirection == FlowDirection.RightToLeft;

            var paddingStart = isRtl ? FlowLayout.Padding.Right : FlowLayout.Padding.Left;
            var paddingEnd = isRtl ? FlowLayout.Padding.Left : FlowLayout.Padding.Right;
            var paddingTop = FlowLayout.Padding.Top;
            var paddingBottom = FlowLayout.Padding.Bottom;

            var maxChildEnd = parentEnd - parentStart - paddingEnd;
            var maxChildWidth = maxChildEnd - (parentStart + paddingStart);

            var currentRow = new Row(paddingStart, paddingTop);
            var finalWidth = 0d;
            var finalHeight = 0d;

            void LayoutRowItems()
            {
                var initialFreeSpace = parentEnd - (FlowLayout.ItemSpacing * currentRow.Items.Count) - paddingStart;
                var freeSpace = initialFreeSpace;
                var expandingItems = currentRow.Items.Count;

                // Reset starting point
                currentRow.RowStart = paddingStart;

                if (expandingItems == 1)
                {
                    var item = currentRow.Items.First();
                    var lp = (FlowLayout as FlowLayout).GetRowItem(item);

                    var desiredWidth = (parentEnd - item.Margin.Right) - (currentRow.RowStart + item.Margin.Left);

                    item.Measure(desiredWidth, item.DesiredSize.Height - (paddingTop + paddingBottom));

                    lp.ItemStart = currentRow.RowStart + item.Margin.Left;
                    lp.ItemTop = currentRow.RowTop;
                    lp.ItemEnd = parentEnd - item.Margin.Right;
                    lp.ItemBottom = currentRow.RowTop + item.DesiredSize.Height;
                }
                else
                {
                    var maxWidth = 0d;

                    currentRow.Items.ForEach(item =>
                    {
                        var desiredWidth = item.DesiredSize.Width;
                        var lp = (FlowLayout as FlowLayout).GetRowItem(item);

                        maxWidth = Math.Max(Math.Max(desiredWidth, item.MinimumWidth), lp.ItemWidth);

                        if (desiredWidth > 0)
                        {
                            freeSpace -= desiredWidth;
                            expandingItems--;
                        }
                    });

                    var fillSpace = false;
                    var uniformWidth = false;

                    if (expandingItems <= 0 && freeSpace > 0)
                    {
                        expandingItems = currentRow.Items.Count;
                        fillSpace = true;
                        if (maxWidth * currentRow.Items.Count > 0)
                        {
                            uniformWidth = true;
                        }
                    }

                    foreach (var item in currentRow.Items)
                    {
                        var lp = (FlowLayout as FlowLayout).GetRowItem(item);
                        var startMargin = item.Margin.Left;
                        var endMargin = item.Margin.Right;
                        var desiredWidth = Math.Max(Math.Max(item.DesiredSize.Width, item.MinimumWidth), lp.ItemWidth);

                        if (desiredWidth == 0)
                            break;

                        if (fillSpace && freeSpace > 0 && expandingItems > 0)
                        {
                            if (uniformWidth)
                            {
                                desiredWidth = initialFreeSpace / currentRow.Items.Count;
                            }
                            else
                            {
                                desiredWidth += (freeSpace / expandingItems);
                            }
                        }

                        item.Measure(desiredWidth, item.DesiredSize.Height - (paddingTop + paddingBottom));

                        var childEnd = currentRow.RowStart + startMargin + desiredWidth;
                        var childBottom = currentRow.RowTop + item.DesiredSize.Height;
                        var childTop = currentRow.RowTop;

                        if (isRtl)
                        {
                            lp.ItemStart = maxChildEnd - childEnd;
                            lp.ItemTop = childTop;
                            lp.ItemEnd = maxChildEnd - currentRow.RowStart - startMargin;
                            lp.ItemBottom = childBottom;
                        }
                        else
                        {
                            lp.ItemStart = currentRow.RowStart + startMargin;
                            lp.ItemTop = childTop;
                            lp.ItemEnd = childEnd;
                            lp.ItemBottom = childBottom;
                        }

                        currentRow.RowStart += (startMargin + endMargin + desiredWidth) + FlowLayout.ItemSpacing;
                    }
                }
            }

            void LayoutChild(IView item, bool isLast = false)
            {
                if (item.Visibility is not Visibility.Visible)
                {
                    return; // if an item is collapsed, avoid adding the spacing
                }

                var startMargin = item.Margin.Left;
                var endMargin = item.Margin.Right;
                var desiredWidth = Math.Min(maxChildWidth, Math.Max(item.DesiredSize.Width, item.MinimumWidth));

                var childEnd = currentRow.RowStart + startMargin + desiredWidth;

                if (childEnd > maxChildEnd)
                {
                    LayoutRowItems();

                    // next row
                    Rows.Add(currentRow);
                    currentRow = new Row(paddingStart, currentRow.RowMaxBotton + FlowLayout.LineSpacing);
                }

                currentRow.RowMaxBotton = Math.Max(currentRow.RowMaxBotton, currentRow.RowTop + item.DesiredSize.Height);

                // Stretch the last item to fill the available space
                if (isLast)
                {
                    if (currentRow.Items.Count == 0)
                    {
                        desiredWidth = maxChildEnd - currentRow.RowStart;

                        var lp = (FlowLayout as FlowLayout).GetRowItem(item);
                        lp.ItemWidth = desiredWidth;
                    }

                    currentRow.Add(item);
                    LayoutRowItems();
                }
                else
                {
                    currentRow.Add(item);

                    // Advance position
                    currentRow.RowStart += (startMargin + endMargin + desiredWidth) + FlowLayout.ItemSpacing;
                    finalWidth = Math.Max(finalWidth, currentRow.RowStart);
                }
            }

            for (int i = 0; i < FlowLayout.Count; i++)
            {
                var child = FlowLayout[i];
                LayoutChild(child, i == FlowLayout.Count - 1);
            }

            if (currentRow.Items.Count > 0)
            {
                Rows.Add(currentRow);
            }

            if (Rows.Count == 0)
            {
                return new Size(paddingStart + paddingEnd, paddingTop + paddingBottom);
            }

            var lastRow = Rows.Last();
            finalHeight = lastRow.RowMaxBotton;
            return new Size(finalWidth + paddingEnd, finalHeight + paddingBottom);
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            if (FlowLayout.Count == 0)
            {
                // Do not re-layout when there are no children.
                return bounds.Size;
            }

            if (Rows.Count > 0)
            {
                Rows.ForEach<Row>(row =>
                {
                    row.Items.ForEach(child =>
                    {
                        if (child.Visibility is Visibility.Visible)
                        {
                            var lp = (FlowLayout as FlowLayout).GetRowItem(child);
                            child.Arrange(lp.ToRect().Offset(bounds.Left, bounds.Top));
                        }
                    });
                });
            }

            return bounds.Size;
        }
    }
}