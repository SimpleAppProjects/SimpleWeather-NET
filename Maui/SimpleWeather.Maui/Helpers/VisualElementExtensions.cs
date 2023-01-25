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
    }
}
