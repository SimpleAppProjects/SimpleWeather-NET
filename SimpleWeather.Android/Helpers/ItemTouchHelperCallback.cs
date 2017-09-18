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

        private IItemTouchHelperAdapter mAdapter;

        Drawable deleteIcon;
        Drawable deleteBackground;
        int iconMargin;

        public ItemTouchHelperCallback(IItemTouchHelperAdapter adapter)
        {
            mAdapter = adapter;

            deleteIcon = ContextCompat.GetDrawable(App.Context, Resource.Drawable.ic_delete_white_24dp);
            deleteBackground = new ColorDrawable(new Color(ContextCompat.GetColor(App.Context, Resource.Color.bg_swipe_delete)));
            iconMargin = (int)App.Context.Resources.GetDimension(Resource.Dimension.delete_icon_margin);
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

            mAdapter.OnItemMove(viewHolder.AdapterPosition, target.AdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            mAdapter.OnItemDismiss(viewHolder.AdapterPosition);
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            if (viewHolder.AdapterPosition == -1)
                return;

            if (actionState == ItemTouchHelper.ActionStateSwipe)
            {
                View itemView = viewHolder.ItemView;

                int iconLeft;
                int iconRight;
                int iconTop = itemView.Top + (itemView.Bottom - itemView.Top - deleteIcon.IntrinsicHeight) / 2;
                int iconBottom = iconTop + deleteIcon.IntrinsicHeight;

                if (dX > 0)
                {
                    deleteBackground.SetBounds(itemView.Left, itemView.Top, itemView.Left + (int)dX, itemView.Bottom);

                    iconLeft = itemView.Left + iconMargin;
                    iconRight = itemView.Left + iconMargin + deleteIcon.IntrinsicWidth;
                }
                else
                {
                    deleteBackground.SetBounds(itemView.Right + (int)dX, itemView.Top, itemView.Right, itemView.Bottom);

                    iconLeft = itemView.Right - iconMargin - deleteIcon.IntrinsicWidth;
                    iconRight = itemView.Right - iconMargin;
                }

                deleteBackground.Draw(c);

                deleteIcon.SetBounds(iconLeft, iconTop, iconRight, iconBottom);
                deleteIcon.Draw(c);
            }

            base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            base.ClearView(recyclerView, viewHolder);

            if (fromPosition != -1 && toPosition != -1 && fromPosition != toPosition)
                mAdapter.OnItemMoved(fromPosition, toPosition);

            // Reset positions
            fromPosition = -1;
            toPosition = -1;
        }

        public override long GetAnimationDuration(RecyclerView recyclerView, int animationType, float animateDx, float animateDy)
        {
            if (animationType == ItemTouchHelper.AnimationTypeSwipeSuccess)
                return 350; // Default is 250
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