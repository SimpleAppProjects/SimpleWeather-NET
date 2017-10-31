using System;
using Android.Graphics;
using Android.Util;
using Android.Content.Res;
using System.IO;
using Android.Support.V4.Content.Res;
using Android.Content;

namespace SimpleWeather.Droid.Utils
{
    public static class ImageUtils
    {
        public static Bitmap BitmapFromAssets(AssetManager am, String path)
        {
            Bitmap bmp = null;

            try
            {
                using (Stream stream = am.Open(path))
                    bmp = BitmapFactory.DecodeStream(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                bmp = null;
            }

            return bmp;
        }

        public static Bitmap WeatherIconToBitmap(Context context, String text, int textSize)
        {
            return WeatherIconToBitmap(context, text, textSize, Color.White);
        }

        public static Bitmap WeatherIconToBitmap(Context context, String text, int textSize, Color textColor)
        {
            Paint paint = new Paint(PaintFlags.AntiAlias);
            Typeface weathericons = ResourcesCompat.GetFont(context, Resource.Font.weathericons);
            paint.SubpixelText = true;
            paint.SetTypeface(weathericons);
            paint.SetStyle(Paint.Style.Fill);
            paint.Color = textColor;
            paint.TextSize = textSize;
            paint.TextAlign = Paint.Align.Left;

            float baseline = -paint.Ascent();
            int width = (int)(paint.MeasureText(text) + 0.5f);
            int height = (int)(baseline + paint.Descent() + 0.5f);

            Bitmap bmp = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas myCanvas = new Canvas(bmp);
            myCanvas.DrawText(text, 0, baseline, paint);

            return bmp;
        }
    }

    public class CenterCropper
    {
        private int width = 0;
        private int height = 0;

        public CenterCropper(int newWidth, int newHeight)
        {
            width = newWidth;
            height = newHeight;
        }

        /**
         * Scales and center-crops a bitmap to the size passed in and returns the new bitmap.
         *
         * @param source Bitmap to scale and center-crop
         * @param newWidth destination width
         * @param newHeight destination height
         * @return Bitmap scaled and center-cropped bitmap
         */
        public Bitmap Process(Bitmap source)
        {
            Bitmap dest = null;
            try
            {
                int sourceWidth = source.Width;
                int sourceHeight = source.Height;

                // Compute the scaling factors to fit the new height and width, respectively.
                // To cover the final image, the final scaling will be the bigger 
                // of these two.
                float xScale = (float)width / sourceWidth;
                float yScale = (float)height / sourceHeight;
                float scale = Math.Max(xScale, yScale);

                // Now get the size of the source bitmap when scaled
                float scaledWidth = scale * sourceWidth;
                float scaledHeight = scale * sourceHeight;

                // Let's find out the upper left coordinates if the scaled bitmap
                // should be centered in the new size give by the parameters
                float left = (width - scaledWidth) / 2;
                float top = (height - scaledHeight) / 2;

                // The target rectangle for the new, scaled version of the source bitmap will now
                // be
                RectF targetRect = new RectF(left, top, left + scaledWidth, top + scaledHeight);

                // Finally, we create a new bitmap of the specified size and draw our new,
                // scaled bitmap onto it.
                dest = Bitmap.CreateBitmap(width, height, source.GetConfig());
                Canvas canvas = new Canvas(dest);
                canvas.DrawBitmap(source, null, targetRect, null);
            }
            catch (Exception ex)
            {
                dest = null;
                Log.WriteLine(LogPriority.Error, "CenterCropper", ex.StackTrace);
            }
            finally
            {
                if (dest == null)
                    dest = source;
            }

            return dest;
        }
    }
}