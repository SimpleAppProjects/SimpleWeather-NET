﻿using System;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.NET.Controls
{
    /// <summary>
    /// Based on: https://stackoverflow.com/a/35915640
    /// Adapted for UWP
    /// </summary>
    public partial class AlignableWrapPanel : Panel
    {
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double),
                typeof(AlignableWrapPanel), new PropertyMetadata(double.NaN, (obj, args) => (obj as Panel)?.InvalidateMeasure()));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double),
                typeof(AlignableWrapPanel), new PropertyMetadata(double.NaN, (obj, args) => (obj as Panel)?.InvalidateMeasure()));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation),
                typeof(AlignableWrapPanel), new PropertyMetadata(Orientation.Horizontal, (obj, args) => (obj as Panel)?.InvalidateMeasure()));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

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
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness),
                typeof(AlignableWrapPanel), new PropertyMetadata(default(Thickness), (obj, args) => (obj as Panel)?.InvalidateMeasure()));

        private struct UvSize
        {
            internal UvSize(Orientation orientation, double width, double height)
            {
                U = V = 0d;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UvSize(Orientation orientation)
            {
                U = V = 0d;
                _orientation = orientation;
            }

            internal double U;
            internal double V;
            private readonly Orientation _orientation;

            internal double Width
            {
                get { return (_orientation == Orientation.Horizontal ? U : V); }
                private set { if (_orientation == Orientation.Horizontal) U = value; else V = value; }
            }

            internal double Height
            {
                get { return (_orientation == Orientation.Horizontal ? V : U); }
                private set { if (_orientation == Orientation.Horizontal) V = value; else U = value; }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var curLineSize = new UvSize(Orientation);
            var panelSize = new UvSize(Orientation);
            var uvConstraint = new UvSize(Orientation, constraint.Width - Padding.Left - Padding.Right, constraint.Height - Padding.Top - Padding.Bottom);
            var itemWidth = ItemWidth;
            var itemHeight = ItemHeight;
            var itemWidthSet = !double.IsNaN(itemWidth);
            var itemHeightSet = !double.IsNaN(itemHeight);

            var childConstraint = new Size(
                    (itemWidthSet ? itemWidth : constraint.Width - Padding.Left - Padding.Right),
                    (itemHeightSet ? itemHeight : constraint.Height - Padding.Top - Padding.Bottom));

            var children = Children;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                if (child == null) continue;

                //Flow passes its own constrint to children 
                child.Measure(childConstraint);

                //this is the size of the child in UV space 
                var sz = new UvSize(
                        Orientation,
                        (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                        (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (curLineSize.U + sz.U > uvConstraint.U)
                {
                    //need to switch to another line 
                    panelSize.U = Math.Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V;
                    curLineSize = sz;

                    if (!(sz.U > uvConstraint.U)) continue;
                    //the element is wider then the constrint - give it a separate line
                    panelSize.U = Math.Max(sz.U, panelSize.U);
                    panelSize.V += sz.V;
                    curLineSize = new UvSize(Orientation);
                }
                else
                {
                    //continue to accumulate a line
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                }
            }

            //the last line size, if any should be added 
            panelSize.U = Math.Max(curLineSize.U, panelSize.U);
            panelSize.V += curLineSize.V;

            //go from UV space to W/H space
            return new Size(panelSize.Width, panelSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var firstInLine = 0;
            var itemWidth = ItemWidth;
            var itemHeight = ItemHeight;
            double accumulatedV = 0;
            var itemU = (Orientation == Orientation.Horizontal ? itemWidth : itemHeight);
            var curLineSize = new UvSize(Orientation);
            var uvFinalSize = new UvSize(Orientation, finalSize.Width, finalSize.Height);
            var itemWidthSet = !double.IsNaN(itemWidth);
            var itemHeightSet = !double.IsNaN(itemHeight);
            var useItemU = (Orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet);

            var children = Children;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                if (child == null) continue;

                var sz = new UvSize(
                        Orientation,
                        (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                        (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (curLineSize.U + sz.U > uvFinalSize.U)
                {
                    //need to switch to another line 
                    ArrangeLine(finalSize, accumulatedV, curLineSize, firstInLine, i, useItemU, itemU);

                    accumulatedV += curLineSize.V;
                    curLineSize = sz;

                    if (sz.U > uvFinalSize.U)
                    {
                        //the element is wider then the constraint - give it a separate line 
                        //switch to next line which only contain one element 
                        ArrangeLine(finalSize, accumulatedV, sz, i, ++i, useItemU, itemU);

                        accumulatedV += sz.V;
                        curLineSize = new UvSize(Orientation);
                    }

                    firstInLine = i;
                }
                else
                {
                    //continue to accumulate a line
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                }
            }

            //arrange the last line, if any
            if (firstInLine < children.Count)
            {
                ArrangeLine(finalSize, accumulatedV, curLineSize, firstInLine, children.Count, useItemU, itemU);
            }

            return finalSize;
        }

        private void ArrangeLine(Size finalSize, double v, UvSize line, int start, int end, bool useItemU, double itemU)
        {
            double u;
            var isHorizontal = Orientation == Orientation.Horizontal;

            if (Orientation == Orientation.Vertical)
            {
                switch (VerticalContentAlignment)
                {
                    case VerticalAlignment.Center:
                        u = (finalSize.Height - line.U) / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        u = finalSize.Height - line.U;
                        break;
                    default:
                        u = 0;
                        break;
                }
            }
            else
            {
                switch (HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Center:
                        u = (finalSize.Width - line.U) / 2;
                        break;
                    case HorizontalAlignment.Right:
                        u = finalSize.Width - line.U;
                        break;
                    default:
                        u = 0;
                        break;
                }
            }

            var children = Children;
            for (var i = start; i < end; i++)
            {
                var child = children[i];
                if (child == null) continue;
                var childSize = new UvSize(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                var layoutSlotU = (useItemU ? itemU : childSize.U);
                child.Arrange(new Rect(
                        isHorizontal ? u : v,
                        isHorizontal ? v : u,
                        isHorizontal ? layoutSlotU : line.V,
                        isHorizontal ? line.V : layoutSlotU));
                u += layoutSlotU;
            }
        }

        public static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register(nameof(HorizontalContentAlignment), typeof(HorizontalAlignment),
                typeof(AlignableWrapPanel), new PropertyMetadata(HorizontalAlignment.Left, (obj, args) => (obj as Panel)?.InvalidateArrange()));

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty VerticalContentAlignmentProperty = DependencyProperty.Register(nameof(VerticalContentAlignment), typeof(VerticalAlignment),
                typeof(AlignableWrapPanel), new PropertyMetadata(VerticalAlignment.Top, (obj, args) => (obj as Panel)?.InvalidateArrange()));

        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }
    }
}
