using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Helpers
{
    public static class VisualTreeHelperExtensions
    {
        public static T FindChild<T>(FrameworkElement depObj, String Name = null, bool includeParent = false) where T : FrameworkElement
        {
            if (includeParent && depObj != null && depObj is T && (String.IsNullOrWhiteSpace(Name) || String.Equals(Name, depObj.Name)))
            {
                return (T)depObj;
            }

            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    FrameworkElement child = VisualTreeHelper.GetChild(depObj, i) as FrameworkElement;
                    if (child != null && child is T && (String.IsNullOrWhiteSpace(Name) || String.Equals(Name, child.Name)))
                    {
                        return (T)child;
                    }

                    if (FindChild<T>(child, Name) is FrameworkElement childOfChild)
                    {
                        return (T)childOfChild;
                    }
                }
            }

            return null;
        }

        public static T GetParent<T>(FrameworkElement depObj) where T : FrameworkElement
        {
            if (depObj != null)
            {
                var parent = depObj?.Parent;

                while (parent is FrameworkElement element && !(parent is T))
                {
                    parent = element.Parent;
                }

                if (parent is T)
                    return (T)parent;
            }

            return null;
        }
    }
}
