using System;

using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;

namespace SimpleWeather.Droid.Helpers
{
    public class ItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        public override bool IsLongPressDragEnabled => dragEnabled;
        public override bool IsItemViewSwipeEnabled => swipeEnabled;

        private bool dragEnabled = true;
        private bool swipeEnabled = true;

        private int fromPosition = -1;
        private int toPosition = -1;

        private ItemTouchHelperAdapter mAdapter;

        public ItemTouchHelperCallback(ItemTouchHelperAdapter adapter)
        {
            mAdapter = adapter;
        }

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            int dragFlags = 0;
            int swipeFlags = 0;

            if (dragEnabled && swipeEnabled)
            {
                dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
                swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
            }
            else if (dragEnabled)
                dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            else if (swipeEnabled)
                swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;

            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            fromPosition = viewHolder.AdapterPosition;
            toPosition = target.AdapterPosition;

            mAdapter.onItemMove(viewHolder.AdapterPosition, target.AdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            mAdapter.onItemDismiss(viewHolder.AdapterPosition);
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            if (actionState == ItemTouchHelper.ActionStateSwipe)
            {
                View itemView = viewHolder.ItemView;
                Drawable d;

                if (dX > 0)
                {
                    d = ContextCompat.GetDrawable(recyclerView.Context, Resource.Drawable.bg_swipe_right_delete);
                    d.SetBounds(itemView.Left, itemView.Top, (int)dX, itemView.Bottom);
                }
                else
                {
                    d = ContextCompat.GetDrawable(recyclerView.Context, Resource.Drawable.bg_swipe_left_delete);
                    d.SetBounds((int)dX, itemView.Top, itemView.Right, itemView.Bottom);
                }

                d.Draw(c);
            }

            base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            base.ClearView(recyclerView, viewHolder);

            if (fromPosition != -1 && toPosition != -1 && fromPosition != toPosition)
                mAdapter.onItemMoved(fromPosition, toPosition);

            fromPosition = -1;
            toPosition = -1;
        }

        public override long GetAnimationDuration(RecyclerView recyclerView, int animationType, float animateDx, float animateDy)
        {
            if (animationType == ItemTouchHelper.AnimationTypeSwipeSuccess)
                return 350;
            else
                return base.GetAnimationDuration(recyclerView, animationType, animateDx, animateDy);
        }

        public void SetLongPressDragEnabled(bool value)
        {
            dragEnabled = value;
        }

        public void SetItemViewSwipeEnabled(bool value)
        {
            swipeEnabled = value;
        }
    }
}