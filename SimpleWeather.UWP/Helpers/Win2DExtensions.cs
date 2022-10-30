using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;

namespace SimpleWeather.UWP.Helpers
{
    public static class Win2DExtensions
    {
        // Based on SkCornerPathEffect
        public static void AddCurvedLine(this CanvasPathBuilder pathBuilder, float x1, float y1, float x2, float y2, float radius)
        {
            bool drawSegment = ComputeStep(x1, y1, x2, y2, radius, out Vector2 step);

            var controlPoint = new Vector2(x1, y1);
            var endPoint = new Vector2(x1 + step.X, y1 + step.Y);

            pathBuilder.AddQuadraticBezier(controlPoint, endPoint);
            if (drawSegment)
            {
                pathBuilder.AddLine(x2 - step.X, y2 - step.Y);
            }
        }

        private static bool ComputeStep(float x1, float y1, float x2, float y2, float radius, out Vector2 step)
        {
            var a = new Vector2(x1, y1);
            var b = new Vector2(x2, y2);

            var dist = Vector2.Distance(a, b);

            step = Vector2.Subtract(b, a);

            if (dist <= radius * 2f)
            {
                step = Vector2.Multiply(step, 0.5f);
                return false;
            }
            else
            {
                step = Vector2.Multiply(step, (radius / dist));
                return true;
            }
        }
    }
}
