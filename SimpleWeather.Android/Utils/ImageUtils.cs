using System;
using Android.Views;
using Android.Graphics;
using Android.Graphics.Drawables;
using Com.Nostra13.Universalimageloader.Core;
using Com.Nostra13.Universalimageloader.Core.Imageaware;
using Com.Nostra13.Universalimageloader.Core.Process;
using Com.Nostra13.Universalimageloader.Core.Assist;
using Android.Support.V4.Content;
using Com.Nostra13.Universalimageloader.Core.Display;

namespace SimpleWeather.Droid.Utils
{
    public static class ImageUtils
    {
        // Default UniversalImageLoader config
        public static DisplayImageOptions DefaultDisplayConfig()
        {
            return new DisplayImageOptions.Builder()
                    .ImageScaleType(ImageScaleType.InSamplePowerOf2)
                    .CacheOnDisk(true)
                    .CacheInMemory(true)
                    .BitmapConfig(Bitmap.Config.Rgb565)
                    .ShowImageOnLoading(new ColorDrawable(new Color(ContextCompat.GetColor(App.Context, Resource.Color.colorPrimary))))
                    .Build();
        }

        // UniversalImageLoader DisplayImageOptions for Center Cropping an Image
        public static DisplayImageOptions CenterCropConfig(int Width, int Height)
        {
            return new DisplayImageOptions.Builder()
                    .ImageScaleType(ImageScaleType.InSamplePowerOf2)
                    .CacheOnDisk(true)
                    .CacheInMemory(true)
                    .PreProcessor(new CenterCropper(Width, Height))
                    .BitmapConfig(Bitmap.Config.Rgb565)
                    .ResetViewBeforeLoading(false)
                    .Displayer(new FadeInBitmapDisplayer(500))
                    .Build();
        }
    }

    public class CenterCropper : Java.Lang.Object, IBitmapProcessor
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
            catch (Exception)
            {
                dest = null;
            }
            finally
            {
                if (dest == null)
                    dest = source;
            }

            return dest;
        }
    }

    public class CustomViewAware : ViewAware
    {
        public CustomViewAware(View view) :
            base(view)
        {
        }

        protected override void SetImageBitmapInto(Bitmap bmp, View view)
        {
            view.Background = new BitmapDrawable(bmp);
        }

        protected override void SetImageDrawableInto(Drawable drawable, View view)
        {
            view.Background = drawable;
        }
    }
}