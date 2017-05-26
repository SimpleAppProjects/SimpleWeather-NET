using Android.Support.V7.View;
using System;

namespace SimpleWeather.Droid.Helpers
{
    public class ActionModeCallback : Java.Lang.Object, ActionMode.ICallback
    {
        public Func<ActionMode, Android.Views.IMenu, bool> CreateActionMode;
        public Func<ActionMode, Android.Views.IMenu, bool> PrepareActionMode;
        public Func<ActionMode, Android.Views.IMenuItem, bool> ActionItemClicked;
        public Action<ActionMode> DestroyActionMode;

        // Called when the action mode is created; startActionMode() was called
        public bool OnCreateActionMode(ActionMode mode, Android.Views.IMenu menu)
        {
            if (CreateActionMode != null)
                return CreateActionMode.Invoke(mode, menu);
            else
                return false; // Return false if nothing is done
        }

        // Called each time the action mode is shown. Always called after onCreateActionMode, but
        // may be called multiple times if the mode is invalidated.
        public bool OnPrepareActionMode(ActionMode mode, Android.Views.IMenu menu)
        {
            if (PrepareActionMode != null)
                return PrepareActionMode.Invoke(mode, menu);
            else
                return false; // Return false if nothing is done
        }

        // Called when the user selects a contextual menu item
        public bool OnActionItemClicked(ActionMode mode, Android.Views.IMenuItem item)
        {
            if (ActionItemClicked != null)
                return ActionItemClicked.Invoke(mode, item);
            else
                return false; // Return false if nothing is done
        }

        // Called when the user exits the action mode
        public void OnDestroyActionMode(ActionMode mode)
        {
            if (DestroyActionMode != null)
                DestroyActionMode.Invoke(mode);
        }
    }
}