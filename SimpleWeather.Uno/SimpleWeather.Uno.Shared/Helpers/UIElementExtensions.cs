using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace SimpleWeather.Uno.Helpers
{
	public static class UIElementExtensions
	{
        /// <summary>
        /// Gets the coordinate of the top-left point of the view
        /// </summary>
        /// <param name="element">The element to locate</param>
        /// <returns>The top-left point of the element</returns>
        public static Point GetCoordinates(this UIElement element)
		{
			var windowContent = (MainWindow.Current ?? Window.Current)?.Content;

			if (windowContent != null)
			{
				var visual = element.TransformToVisual(windowContent);
				return visual.TransformPoint(new Point(0, 0));
			}
			else if (element.XamlRoot?.Content != null)
			{
				var rootView = element.XamlRoot?.Content;
                var visual = element.TransformToVisual(rootView);
                return visual.TransformPoint(new Point(0, 0));
            }
			else
			{
				return element.GetScreenCoords();
			}
		}

        /// <summary>
        /// A view's default X- and Y-coordinates are LOCAL with respect to the boundaries of its parent,
        /// and NOT with respect to the screen. This method calculates the SCREEN coordinates of a view.
        /// The coordinates returned refer to the top left corner of the view.
        /// </summary>
        public static Point GetScreenCoords(this UIElement view)
        {
            var result = new Point(0, 0);
            while (VisualTreeHelper.GetParent(view) is UIElement parent)
            {
                var visual = view.TransformToVisual(parent);
                var relPoint = visual.TransformPoint(new Point(0, 0));

                result = new Point(result.X + relPoint.X, result.Y + relPoint.Y);
                view = parent;
            }
            return result;
        }

        public static double GetBestPopupHeight(this UIElement element, Window window)
        {
            var coords = element.GetCoordinates();

            var distToTop = (coords.Y + element.ActualSize.Y) - window.Bounds.Top;
            var distToBottom = window.Bounds.Height - (coords.Y + element.ActualSize.Y);

            return Math.Min(distToTop, distToBottom);
        }
    }
}

