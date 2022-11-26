#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class MathF
    {
        public const float E = 2.71828183f;
        public const float PI = 3.14159265f;

        //
        // Parameters:
        //   x:
        public static float Abs(float x)
        {
            return Math.Abs(x);
        }
        //
        // Parameters:
        //   x:
        public static float Acos(float x)
        {
            return (float)Math.Acos(x);
        }
        //
        // Parameters:
        //   x:
        public static float Asin(float x)
        {
            return (float)Math.Asin(x);
        }
        //
        // Parameters:
        //   x:
        public static float Atan(float x)
        {
            return (float)Math.Atan(x);
        }
        //
        // Parameters:
        //   y:
        //
        //   x:
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(x, y);
        }
        //
        // Parameters:
        //   x:
        public static float Ceiling(float x)
        {
            return (float)Math.Ceiling(x);
        }
        //
        // Parameters:
        //   x:
        public static float Cos(float x)
        {
            return (float)Math.Cos(x);
        }
        //
        // Parameters:
        //   x:
        public static float Cosh(float x)
        {
            return (float)Math.Cosh(x);
        }
        //
        // Parameters:
        //   x:
        public static float Exp(float x)
        {
            return (float)Math.Exp(x);
        }
        //
        // Parameters:
        //   x:
        public static float Floor(float x)
        {
            return (float)Math.Floor(x);
        }
        //
        // Parameters:
        //   x:
        //
        //   y:
        public static float IEEERemainder(float x, float y)
        {
            return (float)Math.IEEERemainder(x, y);
        }
        //
        // Parameters:
        //   x:
        //
        //   y:
        public static float Log(float x, float y)
        {
            return (float)Math.Log(x, y);
        }
        //
        // Parameters:
        //   x:
        public static float Log(float x)
        {
            return (float)Math.Log(x);
        }
        //
        // Parameters:
        //   x:
        public static float Log10(float x)
        {
            return (float)Math.Log10(x);
        }
        //
        // Parameters:
        //   x:
        //
        //   y:
        public static float Max(float x, float y)
        {
            return Math.Max(x, y);
        }
        //
        // Parameters:
        //   x:
        //
        //   y:
        public static float Min(float x, float y)
        {
            return Math.Min(x, y);
        }
        //
        // Parameters:
        //   x:
        //
        //   y:
        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }
        //
        // Parameters:
        //   x:
        public static float Round(float x)
        {
            return (float)Math.Round(x);
        }
        //
        // Parameters:
        //   x:
        //
        //   digits:
        public static float Round(float x, int digits)
        {
            return (float)Math.Round(x, digits);
        }
        //
        // Parameters:
        //   x:
        //
        //   digits:
        //
        //   mode:
        public static float Round(float x, int digits, MidpointRounding mode)
        {
            return (float)Math.Round(x, digits, mode);
        }
        //
        // Parameters:
        //   x:
        //
        //   mode:
        public static float Round(float x, MidpointRounding mode)
        {
            return (float)Math.Round(x, mode);
        }
        //
        // Parameters:
        //   x:
        public static int Sign(float x)
        {
            return Math.Sign(x);
        }
        //
        // Parameters:
        //   x:
        public static float Sin(float x)
        {
            return (float)Math.Sin(x);
        }
        //
        // Parameters:
        //   x:
        public static float Sinh(float x)
        {
            return (float)Math.Sinh(x);
        }
        //
        // Parameters:
        //   x:
        public static float Sqrt(float x)
        {
            return (float)Math.Sqrt(x);
        }
        //
        // Parameters:
        //   x:
        public static float Tan(float x)
        {
            return (float)Math.Tan(x);
        }
        //
        // Parameters:
        //   x:
        public static float Tanh(float x)
        {
            return (float)Math.Tanh(x);
        }
        //
        // Parameters:
        //   x:
        public static float Truncate(float x)
        {
            return (float)Math.Truncate(x);
        }
    }
}
#endif