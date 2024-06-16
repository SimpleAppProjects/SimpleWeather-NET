using System;
using CommunityToolkit.Maui.Core;
using Foundation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Items;
using UIKit;

namespace SimpleWeather.Maui.Controls
{
	public class ReorderableCollectionView : CollectionView
	{
        public event EventHandler<LongPressEventArgs> LongPressed;

        internal void OnItemLongPressed(UICollectionView collectionView, int section, int index)
        {
            var args = new LongPressEventArgs(section, index);
            LongPressed?.Invoke(this, args);

            if (!args.Cancel)
            {
                collectionView.BeginInteractiveMovementForItem(NSIndexPath.FromItemSection(index, section));
            }
        }
    }

    public class LongPressEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
        public int Section { get; }
        public int Item { get; }

        internal LongPressEventArgs(int section, int index)
        {
            Section = section;
            Item = index;
        }
    }

	public class ReorderableCollectionViewHandler<TItemsView> : ReorderableItemsViewHandler<TItemsView> where TItemsView : ReorderableItemsView
    {
		public ReorderableCollectionViewHandler() : base(Mapper) { }

		public ReorderableCollectionViewHandler(PropertyMapper mapper = null) : base(mapper ?? ReorderableItemsViewMapper) { }

        protected override ItemsViewController<TItemsView> CreateController(TItemsView itemsView, ItemsViewLayout layout)
        {
            return new ReorderableItemsViewControllerEx<TItemsView>(itemsView, layout);
        }

        public static PropertyMapper<ReorderableCollectionView, ReorderableCollectionViewHandler<TItemsView>> Mapper = new(ReorderableItemsViewMapper)
		{
            [ReorderableItemsView.CanReorderItemsProperty.PropertyName] = MapCanReorderItems,
        };

        public static void MapCanReorderItems(ReorderableCollectionViewHandler<TItemsView> handler, ReorderableItemsView itemsView)
        {
            (handler.Controller as ReorderableItemsViewControllerEx<TItemsView>)?.UpdateCanReorderItemsEx();
        }
    }

    public class ReorderableItemsViewControllerEx<TItemsView> : ReorderableItemsViewController<TItemsView> where TItemsView : ReorderableItemsView
    {
        bool _disposed;
        UILongPressGestureRecognizer _longPressGestureRecognizer;

        public ReorderableItemsViewControllerEx(TItemsView reorderableItemsView, ItemsViewLayout layout) : base(reorderableItemsView, layout)
        {
        }

        private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
        {
            var collectionView = CollectionView;
            if (collectionView == null)
                return;

            var location = gestureRecognizer.LocationInView(collectionView);

            // We are updating "CancelsTouchesInView" so views can still receive touch events when this gesture runs.
            // Those events shouldn't be aborted until they've actually moved the position of the CollectionView item.
            switch (gestureRecognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    var indexPath = collectionView?.IndexPathForItemAtPoint(location);
                    if (indexPath == null)
                    {
                        return;
                    }
                    gestureRecognizer.CancelsTouchesInView = false;
                    (ItemsView as ReorderableCollectionView)?.OnItemLongPressed(collectionView, indexPath.Section, (int)indexPath.Item);
                    break;
                case UIGestureRecognizerState.Changed:
                    gestureRecognizer.CancelsTouchesInView = true;
                    collectionView.UpdateInteractiveMovement(location);
                    break;
                case UIGestureRecognizerState.Ended:
                    collectionView.EndInteractiveMovement();
                    break;
                default:
                    collectionView.CancelInteractiveMovement();
                    break;
            }
        }

        public void UpdateCanReorderItemsEx()
        {
            if (ItemsView.CanReorderItems)
            {
                if (_longPressGestureRecognizer == null)
                {
                    _longPressGestureRecognizer = new UILongPressGestureRecognizer(HandleLongPress);
                    CollectionView.AddGestureRecognizer(_longPressGestureRecognizer);
                }
            }
            else
            {
                if (_longPressGestureRecognizer != null)
                {
                    CollectionView.RemoveGestureRecognizer(_longPressGestureRecognizer);
                    _longPressGestureRecognizer.Dispose();
                    _longPressGestureRecognizer = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_longPressGestureRecognizer != null)
                {
                    CollectionView.RemoveGestureRecognizer(_longPressGestureRecognizer);
                    _longPressGestureRecognizer.Dispose();
                    _longPressGestureRecognizer = null;
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}

