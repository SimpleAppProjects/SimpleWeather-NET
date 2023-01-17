using Windows.Foundation;
using PointF = System.Drawing.PointF;
using RectF = System.Drawing.RectangleF;

namespace SimpleWeather.NET.Helpers
{
    public static class RectHelperExtensions
    {
        public static void Set(this ref Rect rect, double left, double top, double right, double bottom)
        {
            if (left < right)
            {
                rect.X = left;
                rect.Width = right - left;
            }
            else if (double.IsNaN(left) || double.IsNaN(right))
            {
                rect.X = double.NaN;
                rect.Width = double.NaN;
            }
            else
            {
                rect.X = right;
                rect.Width = left - right;
            }

            if (top < bottom)
            {
                rect.Y = top;
                rect.Height = bottom - top;
            }
            else if (double.IsNaN(top) || double.IsNaN(bottom))
            {
                rect.Y = double.NaN;
                rect.Height = double.NaN;
            }
            else
            {
                rect.Y = bottom;
                rect.Height = top - bottom;
            }
        }

        public static void Set(this ref Rect rect, Point point1, Point point2)
        {
            if (point1.X < point2.X) // This will return false is any is NaN, and as it's the common case, we keep it first
            {
                rect.X = point1.X;
                rect.Width = point2.X - point1.X;
            }
            else if (double.IsNaN(point1.X) || double.IsNaN(point2.X))
            {
                rect.X = double.NaN;
                rect.Width = double.NaN;
            }
            else
            {
                rect.X = point2.X;
                rect.Width = point1.X - point2.X;
            }

            if (point1.Y < point2.Y) // This will return false is any is NaN, and as it's the common case, we keep it first
            {
                rect.Y = point1.Y;
                rect.Height = point2.Y - point1.Y;
            }
            else if (double.IsNaN(point1.Y) || double.IsNaN(point2.Y))
            {
                rect.Y = double.NaN;
                rect.Height = double.NaN;
            }
            else
            {
                rect.Y = point2.Y;
                rect.Height = point1.Y - point2.Y;
            }
        }

        public static void SetEmpty(this ref Rect rect)
        {
            rect.X = rect.Y = rect.Height = rect.Width = 0;
        }

        public static bool Intersects(this Rect a, Rect b)
        {
            return a.Left < b.Right && b.Left < a.Right
                && a.Top < b.Bottom && b.Top < a.Bottom;
        }
    }

    public static class RectFHelperExtensions
    {
        public static void Set(this ref RectF rect, float left, float top, float right, float bottom)
        {
            if (left < right)
            {
                rect.X = left;
                rect.Width = right - left;
            }
            else if (double.IsNaN(left) || double.IsNaN(right))
            {
                rect.X = float.NaN;
                rect.Width = float.NaN;
            }
            else
            {
                rect.X = right;
                rect.Width = left - right;
            }

            if (top < bottom)
            {
                rect.Y = top;
                rect.Height = bottom - top;
            }
            else if (double.IsNaN(top) || double.IsNaN(bottom))
            {
                rect.Y = float.NaN;
                rect.Height = float.NaN;
            }
            else
            {
                rect.Y = bottom;
                rect.Height = top - bottom;
            }
        }

        public static void Set(this ref RectF rect, PointF point1, PointF point2)
        {
            if (point1.X < point2.X) // This will return false is any is NaN, and as it's the common case, we keep it first
            {
                rect.X = point1.X;
                rect.Width = point2.X - point1.X;
            }
            else if (double.IsNaN(point1.X) || double.IsNaN(point2.X))
            {
                rect.X = float.NaN;
                rect.Width = float.NaN;
            }
            else
            {
                rect.X = point2.X;
                rect.Width = point1.X - point2.X;
            }

            if (point1.Y < point2.Y) // This will return false is any is NaN, and as it's the common case, we keep it first
            {
                rect.Y = point1.Y;
                rect.Height = point2.Y - point1.Y;
            }
            else if (double.IsNaN(point1.Y) || double.IsNaN(point2.Y))
            {
                rect.Y = float.NaN;
                rect.Height = float.NaN;
            }
            else
            {
                rect.Y = point2.Y;
                rect.Height = point1.Y - point2.Y;
            }
        }

        public static void SetEmpty(this ref RectF rect)
        {
            rect.X = rect.Y = rect.Height = rect.Width = 0;
        }

        public static bool Intersects(this RectF a, RectF b)
        {
            return a.IntersectsWith(b);
            /*
            return a.Left < b.Right && b.Left < a.Right
                && a.Top < b.Bottom && b.Top < a.Bottom;
            */
        }
    }
}