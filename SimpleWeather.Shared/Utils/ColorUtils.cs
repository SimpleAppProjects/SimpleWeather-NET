using System;
using System.Globalization;
using System.Threading;
#if WINUI
using Microsoft.UI;
using Windows.UI;
using Windows.UI.ViewManagement;
#else
using Microsoft.Maui.Graphics;
#endif

namespace SimpleWeather.Utils
{
    /* Collection of color related methods ported from Android */
    public static class ColorUtils
    {
        private const float MIN_CONTRAST_RATIO = 4.5f;
        private static ThreadLocal<double[]> TEMP_ARRAY = new ThreadLocal<double[]>();

        public static Color ParseColor(String colorString)
        {
            if (colorString[0] == '#')
            {
                // Use a long to avoid rollovers on #ffXXXXXX
                long color = long.Parse(colorString.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                if (colorString.Length == 7)
                {
                    // Set the alpha value
                    color |= 0x00000000ff000000;
                }
                else if (colorString.Length != 9)
                {
                    throw new ArgumentException("Unknown color");
                }
                return ValueOf(color);
            }
            throw new ArgumentException("Unknown color");
        }

        public static Color ValueOf(long color)
        {
            byte r = (byte)((color >> 16) & 0xff);
            byte g = (byte)((color >> 8) & 0xff);
            byte b = (byte)((color) & 0xff);
            byte a = (byte)((color >> 24) & 0xff);

#if WINUI
            return Color.FromArgb(a, r, g, b);
#else
            return Color.FromRgba(r, g, b, a);
#endif
        }

        public static Color GetAccentColor()
        {
#if WINUI
            var uiSettings = new UISettings();
            return uiSettings.GetColorValue(UIColorType.Accent);
#else
            return Microsoft.Maui.Controls.Application.AccentColor;
#endif
        }

        public static bool IsSuperLight(Color color)
        {
            return !IsLegible(Colors.White, color);
        }

        public static bool IsSuperDark(Color color)
        {
            return !IsLegible(Colors.Black, color);
        }

        /**
         * @return Whether the foreground color is legible on the background color.
         */
        private static bool IsLegible(Color foreground, Color background)
        {
            background = background.WithAlpha(0xFF);
            return CalculateContrast(foreground, background) >= MIN_CONTRAST_RATIO;
        }

        public static Color WithAlpha(this Color color, byte alpha)
        {
#if WINUI
            return new Color() { A = alpha, R = color.R, G = color.G, B = color.B };
#else
            return color.WithAlpha(Math.Clamp(alpha / 255f, 0f, 1f));
#endif
        }

        public static Color SetAlphaComponent(Color color, byte alpha)
        {
            return WithAlpha(color, alpha);
        }

        /**
         * Blend between two ARGB colors using the given ratio.
         *
         * <p>A blend ratio of 0.0 will result in {@code color1}, 0.5 will give an even blend,
         * 1.0 will result in {@code color2}.</p>
         *
         * @param color1 the first ARGB color
         * @param color2 the second ARGB color
         * @param ratio  the blend ratio of {@code color1} to {@code color2}
         */
        public static Color BlendColor(Color color1, Color color2, float ratio)
        {
            float inverseRatio = 1 - ratio;
#if WINUI
            float a = color1.A * inverseRatio + color2.A * ratio;
            float r = color1.R * inverseRatio + color2.R * ratio;
            float g = color1.G * inverseRatio + color2.G * ratio;
            float b = color1.B * inverseRatio + color2.B * ratio;
            return new Color() { A = (byte)a, R = (byte)r, G = (byte)g, B = (byte)b };
#else
            float a = color1.Alpha * inverseRatio + color2.Alpha * ratio;
            float r = color1.Red * inverseRatio + color2.Red * ratio;
            float g = color1.Green * inverseRatio + color2.Green * ratio;
            float b = color1.Blue * inverseRatio + color2.Blue * ratio;
            return new Color(r, g, b, a);
#endif
        }

        /**
         * Convert the ARGB color to its CIE XYZ representative components.
         *
         * <p>The resulting XYZ representation will use the D65 illuminant and the CIE
         * 2° Standard Observer (1931).</p>
         *
         * <ul>
         * <li>outXyz[0] is X [0 ...95.047)</li>
         * <li>outXyz[1] is Y [0...100)</li>
         * <li>outXyz[2] is Z [0...108.883)</li>
         * </ul>
         *
         * @param color  the ARGB color to convert. The alpha component is ignored
         * @param outXyz 3-element array which holds the resulting LAB components
         */
        public static void ColorToXYZ(Color color, in double[] outXyz)
        {
#if WINUI
            double sr = color.R / 255.0;
            sr = sr < 0.04045 ? sr / 12.92 : Math.Pow((sr + 0.055) / 1.055, 2.4);
            double sg = color.G / 255.0;
            sg = sg < 0.04045 ? sg / 12.92 : Math.Pow((sg + 0.055) / 1.055, 2.4);
            double sb = color.B / 255.0;
            sb = sb < 0.04045 ? sb / 12.92 : Math.Pow((sb + 0.055) / 1.055, 2.4);
#else
            double sr = color.Red;
            sr = sr < 0.04045 ? sr / 12.92 : Math.Pow((sr + 0.055) / 1.055, 2.4);
            double sg = color.Green;
            sg = sg < 0.04045 ? sg / 12.92 : Math.Pow((sg + 0.055) / 1.055, 2.4);
            double sb = color.Blue;
            sb = sb < 0.04045 ? sb / 12.92 : Math.Pow((sb + 0.055) / 1.055, 2.4);
#endif

            outXyz[0] = 100 * (sr * 0.4124 + sg * 0.3576 + sb * 0.1805);
            outXyz[1] = 100 * (sr * 0.2126 + sg * 0.7152 + sb * 0.0722);
            outXyz[2] = 100 * (sr * 0.0193 + sg * 0.1192 + sb * 0.9505);
        }

        /**
         * Returns the luminance of a color as a float between {@code 0.0} and {@code 1.0}.
         * <p>Defined as the Y component in the XYZ representation of {@code color}.</p>
         */
        public static double CalculateLuminance(Color color)
        {
            double[] result = GetTempDouble3Array();
            ColorToXYZ(color, result);
            // Luminance is the Y component
            return result[1] / 100;
        }

        /**
         * Composite two potentially translucent colors over each other and returns the result.
         */
        public static Color CompositeColors(Color foreground, Color background)
        {
#if WINUI
            int bgAlpha = background.A;
            int fgAlpha = foreground.A;
            int a = CompositeAlpha(fgAlpha, bgAlpha);

            int r = CompositeComponent(foreground.R, fgAlpha,
                    background.R, bgAlpha, a);
            int g = CompositeComponent(foreground.G, fgAlpha,
                    background.G, bgAlpha, a);
            int b = CompositeComponent(foreground.B, fgAlpha,
                    background.B, bgAlpha, a);

            return new Color() { A = (byte)a, R = (byte)r, G = (byte)g, B = (byte)b };
#else
            int bgAlpha = (int)(background.Alpha * 255f);
            int fgAlpha = (int)(foreground.Alpha * 255f);
            int a = CompositeAlpha(fgAlpha, bgAlpha);

            int r = CompositeComponent((int)(foreground.Red * 255f), fgAlpha,
                    (int)(background.Red * 255f), bgAlpha, a);
            int g = CompositeComponent((int)(foreground.Green * 255f), fgAlpha,
                    (int)(background.Green * 255f), bgAlpha, a);
            int b = CompositeComponent((int)(foreground.Blue * 255f), fgAlpha,
                    (int)(background.Blue * 255f), bgAlpha, a);

            return new Color(r, g, b, a);
#endif
        }

        private static int CompositeAlpha(int foregroundAlpha, int backgroundAlpha)
        {
            return 0xFF - (((0xFF - backgroundAlpha) * (0xFF - foregroundAlpha)) / 0xFF);
        }

        private static int CompositeComponent(int fgC, int fgA, int bgC, int bgA, int a)
        {
            if (a == 0) return 0;
            return ((0xFF * fgC * fgA) + (bgC * bgA * (0xFF - fgA))) / (a * 0xFF);
        }

        /**
         * Returns the contrast ratio between {@code foreground} and {@code background}.
         * {@code background} must be opaque.
         * <p>
         * Formula defined
         * <a href="http://www.w3.org/TR/2008/REC-WCAG20-20081211/#contrast-ratiodef">here</a>.
         */
        public static double CalculateContrast(Color foreground, Color background)
        {
#if WINUI
            if (background.A != 255)
            {
                throw new ArgumentException("background can not be translucent: #"
                        + background.ToString());
            }
            if (foreground.A < 255)
            {
                // If the foreground is translucent, composite the foreground over the background
                foreground = CompositeColors(foreground, background);
            }
#else
            if (background.Alpha != 1f)
            {
                throw new ArgumentException("background can not be translucent: #"
                        + background.ToString());
            }
            if (foreground.Alpha < 1f)
            {
                // If the foreground is translucent, composite the foreground over the background
                foreground = CompositeColors(foreground, background);
            }
#endif

            double luminance1 = CalculateLuminance(foreground) + 0.05;
            double luminance2 = CalculateLuminance(background) + 0.05;

            // Now return the lighter luminance divided by the darker luminance
            return Math.Max(luminance1, luminance2) / Math.Min(luminance1, luminance2);
        }

        private static double[] GetTempDouble3Array()
        {
            double[] result = TEMP_ARRAY.Value;
            if (result == null)
            {
                result = new double[3];
                TEMP_ARRAY.Value = result;
            }
            return result;
        }
    }
}