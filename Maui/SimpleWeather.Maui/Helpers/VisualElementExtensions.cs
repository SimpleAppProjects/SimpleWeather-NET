using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Helpers
{
    public static class VisualElementExtensions
    {
        public static void SetVisibility(this VisualElement element, Visibility visibility)
        {
            switch (visibility)
            {
                case Visibility.Visible:
                    element.IsVisible = true;
                    if (element.Opacity == 0)
                    {
                        element.Opacity = 1;
                    }
                    break;
                case Visibility.Hidden:
                    element.IsVisible = true;
                    element.Opacity = 0;
                    break;
                case Visibility.Collapsed:
                    element.IsVisible = false;
                    break;
            }
        }

        public static T GetParent<T>(this VisualElement depObj) where T : VisualElement
        {
            if (depObj != null)
            {
                var parent = depObj?.Parent;

                while (parent is VisualElement element && !(parent is T))
                {
                    parent = element.Parent;
                }

                if (parent is T)
                    return (T)parent;
            }

            return null;
        }

        public static void RemoveChildren(this Layout layout, int start, int count)
        {
            layout.RemoveRange(start, count);
        }
    }
}
