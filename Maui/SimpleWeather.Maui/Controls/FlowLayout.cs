using System;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Layouts;

namespace SimpleWeather.Maui.Controls.Flow
{
    public sealed partial class FlowLayout : Layout, IFlowLayout
	{
        public double LineSpacing
        {
            get => (double)GetValue(LineSpacingProperty);
            set => SetValue(LineSpacingProperty, value);
        }

        public static readonly BindableProperty LineSpacingProperty =
            BindableProperty.Create(nameof(LineSpacing), typeof(double), typeof(FlowLayout), 0d, propertyChanged: OnLayoutPropertyChanged);

        public double ItemSpacing
        {
            get => (double)GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }

        public static readonly BindableProperty ItemSpacingProperty =
            BindableProperty.Create(nameof(ItemSpacing), typeof(double), typeof(FlowLayout), 0d, propertyChanged: OnLayoutPropertyChanged);

        public StackOrientation Orientation
        {
            get => (StackOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(FlowLayout), StackOrientation.Horizontal, propertyChanged: OnLayoutPropertyChanged);

        private static void OnLayoutPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is FlowLayout layout && oldValue != newValue)
            {
                layout.InvalidateMeasure();
            }
        }

        protected sealed override ILayoutManager CreateLayoutManager() => new FlowLayoutManager(this);

        internal readonly Dictionary<IView, RowItem> _viewInfo = new();

        internal static readonly BindableProperty RowItemProperty =
            BindableProperty.Create(nameof(RowItem), typeof(RowItem), typeof(FlowLayout), null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static RowItem GetRowItem(BindableObject bindable)
            => (RowItem)bindable.GetValue(RowItemProperty);

        internal RowItem GetRowItem(IView view)
        {
            return view switch
            {
                BindableObject obj => (RowItem)obj.GetValue(RowItemProperty),
                _ => _viewInfo[view]
            };
        }

        internal void SetRowItem(IView view, RowItem rowItem)
        {
            switch (view)
            {
                case BindableObject obj:
                    obj.SetValue(RowItemProperty, rowItem);
                    break;
                default:
                    _viewInfo[view] = rowItem;
                    break;
            }
        }

        internal void AddRowItem(IView child)
        {
            if (child is BindableObject obj)
            {
                obj.SetValue(RowItemProperty, new RowItem());
            }
            else
            {
                _viewInfo.Add(child, new RowItem());
            }
        }

        internal void RemoveRowItem(IView child)
        {
            switch (child)
            {
                case BindableObject obj:
                    obj.ClearValue(RowItemProperty);
                    break;
                default:
                    _viewInfo.Remove(child);
                    break;
            }
        }

        internal void ClearLayout()
        {
            foreach (var child in Children)
                RemoveRowItem(child);
        }

        protected override void OnAdd(int index, IView view)
        {
            base.OnAdd(index, view);
            AddRowItem(view);
        }

        protected override void OnInsert(int index, IView view)
        {
            base.OnInsert(index, view);
            AddRowItem(view);
        }

        protected override void OnUpdate(int index, IView view, IView oldView)
        {
            base.OnUpdate(index, view, oldView);
            RemoveRowItem(oldView);
            AddRowItem(view);
        }

        protected override void OnRemove(int index, IView view)
        {
            base.OnRemove(index, view);
            RemoveRowItem(view);
        }

        protected override void OnClear()
        {
            base.OnClear();
            ClearLayout();
        }
    }
}

