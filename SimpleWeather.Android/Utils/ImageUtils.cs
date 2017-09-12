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
using Android.Util;

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
        public static DisplayImageOptions CenterCropConfig()
        {
            return new DisplayImageOptions.Builder()
                    .ImageScaleType(ImageScaleType.InSamplePowerOf2)
                    .CacheOnDisk(true)
                    .CacheInMemory(true)
                    .BitmapConfig(Bitmap.Config.Rgb565)
                    .ResetViewBeforeLoading(false)
                    .Displayer(new RoundedBitmapDisplayer(0, 0))
                    .Build();
        }

        // UniversalImageLoader DisplayImageOptions for Center Cropping an Image
        public static DisplayImageOptions CenterCropAlpha(int Alpha)
        {
            return new DisplayImageOptions.Builder()
                    .ImageScaleType(ImageScaleType.InSamplePowerOf2)
                    .CacheOnDisk(true)
                    .CacheInMemory(true)
                    .BitmapConfig(Bitmap.Config.Rgb565)
                    .ResetViewBeforeLoading(false)
                    .Displayer(new RoundedBitmapDisplayer(0, 0, Alpha))
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

    public class RoundedBitmapDisplayer : Java.Lang.Object, IBitmapDisplayer
    {
        protected int cornerRadius;
        protected int margin;
        private int alpha;

        public RoundedBitmapDisplayer(int cornerRadiusPixels)
        {
            this.cornerRadius = cornerRadiusPixels;
            this.margin = 0;
            this.alpha = 255;
        }

        public RoundedBitmapDisplayer(int cornerRadiusPixels, int marginPixels)
        {
            this.cornerRadius = cornerRadiusPixels;
            this.margin = marginPixels;
            this.alpha = 255;
        }

        public RoundedBitmapDisplayer(int cornerRadiusPixels, int marginPixels, int alpha)
        {
            this.cornerRadius = cornerRadiusPixels;
            this.margin = marginPixels;
            this.alpha = alpha;
        }

        public void Display(Bitmap bitmap, IImageAware imageAware, LoadedFrom loadedFrom)
        {
            if (!(imageAware is IImageAware))
            {
                throw new Java.Lang.IllegalArgumentException("ImageAware should wrap ImageView. ImageViewAware is expected.");
            }

            var drawable = new RoundCornerDrawable(bitmap, cornerRadius, margin, RoundCornerDrawable.Type.centerCrop)
            {
                Alpha = alpha
            };
            imageAware.SetImageDrawable(drawable);
        }

        /**
          * Created by yijie.ma on 2016/7/20.
          */
        public class RoundCornerDrawable : Drawable
        {

            protected float cornerRadius;
            protected int margin;

            protected RectF mRect = new RectF();
            protected BitmapShader bitmapShader;
            protected Paint paint;
            protected Bitmap mBitmap;

            private Type mType = Type.fitXY;

            public enum Type
            {
                center, fitXY, centerCrop
            }

            public RoundCornerDrawable(Bitmap bitmap, int cornerRadius, int margin, Type type)
            {
                this.cornerRadius = cornerRadius;
                this.margin = margin;
                mBitmap = bitmap;
                mType = type;

                bitmapShader = new BitmapShader(bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

                paint = new Paint();
                paint.AntiAlias = (true);
                paint.SetShader(bitmapShader);
            }

            protected override void OnBoundsChange(Rect bounds)
            {
                base.OnBoundsChange(bounds);
                mRect.Set(margin, margin, bounds.Width() - margin, bounds.Height() - margin);

                Matrix shaderMatrix = new Matrix();
                int width = bounds.Width();
                int height = bounds.Height();

                switch (mType)
                {
                    case Type.centerCrop:
                        float scale = width * 1.0f / mBitmap.Width;
                        if (scale * mBitmap.Height < height)
                        {
                            scale = height * 1.0f / mBitmap.Height;
                        }
                        int outWidth = Java.Lang.Math.Round(scale * mBitmap.Width);
                        int outHeight = Java.Lang.Math.Round(scale * mBitmap.Height);

                        shaderMatrix.PostScale(scale, scale);

                        int left = 0;
                        int top = 0;
                        if (outWidth == width)
                        {
                            top = (outHeight - height) * -1 / 2;
                        }
                        else
                        {
                            left = (outWidth - width) * -1 / 2;
                        }
                        shaderMatrix.PostTranslate(left, top);
                        break;
                    case Type.fitXY:
                        float wScale = width * 1.0f / mBitmap.Width;
                        float hScale = height * 1.0f / mBitmap.Height;
                        shaderMatrix.PostScale(wScale, hScale);
                        break;
                    case Type.center:
                        int moveleft;
                        int movetop;
                        moveleft = (width - mBitmap.Width) / 2;
                        movetop = (height - mBitmap.Height) / 2;
                        shaderMatrix.PostTranslate(moveleft, movetop);
                        break;
                }

                bitmapShader.SetLocalMatrix(shaderMatrix);
            }

            public override void Draw(Canvas canvas)
            {
                canvas.DrawRoundRect(mRect, cornerRadius, cornerRadius, paint);
            }

            public override int Opacity
            {
                get { return (int)Format.Translucent; }
            }

            public override void SetAlpha(int alpha)
            {
                paint.Alpha = alpha;
            }

            public override void SetColorFilter(ColorFilter cf)
            {
                paint.SetColorFilter(cf);
            }
        }
    }
}