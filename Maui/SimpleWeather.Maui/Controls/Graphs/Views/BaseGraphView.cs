using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.Utils;
using SimpleWeather.SkiaSharp;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using RectF = System.Drawing.RectangleF;

namespace SimpleWeather.Maui.Controls.Graphs
{
    public abstract partial class BaseGraphView<T, S, E> : BaseGraphViewControl, IGraph, IDisposable
        where T : GraphData<S, E> where S : GraphDataSet<E> where E : GraphEntry
    {
        private bool disposedValue;

        protected RectF visibleRect = new();

        protected T Data { get; set; }
        private int MaxXEntries { get; set; }

        protected RectF drawingRect;

        protected readonly List<float> xCoordinateList;
        protected int horizontalGridNum;
        protected int verticalGridNum;
        protected const int MIN_HORIZONTAL_GRID_NUM = 1;

        protected readonly SKPaint bottomTextPaint;
        protected readonly SKFont bottomTextFont;
        protected float bottomTextHeight = 0;
        protected float bottomTextDescent;

        protected readonly float iconBottomMargin = 4f; // 2dp // 4dp
        protected readonly float bottomTextTopMargin = 6f; // 6dp

        protected float sideLineLength = 0;
        protected float backgroundGridWidth = 45f; // 45dp
        protected float longestTextWidth;

        protected float IconHeight = 48f; // 30dp // 48dp

        public BaseGraphView() : base()
        {
            xCoordinateList = new List<float>();

            bottomTextPaint = new SKPaint()
            {
                IsAntialias = false,
                TextSize = FontSizeToTextSize(FontSize),
                Typeface = GetSKTypeface(FontFamily, FontAttributes),
                TextAlign = SKTextAlign.Center,
                Style = SKPaintStyle.Fill,
                Color = BottomTextColor.ToSKColor(),
            };

            bottomTextFont = new SKFont
            {
                Edging = SKFontEdging.SubpixelAntialias,
                Size = bottomTextPaint.TextSize
            };

            this.HandlerChanged += BaseGraphView_HandlerChanged;
            this.Loaded += BaseGraphView_Loaded;
            this.Unloaded += BaseGraphView_Unloaded;
        }

        private void BaseGraphView_HandlerChanged(object sender, EventArgs e)
        {
#if ANDROID
            if (this.Handler?.PlatformView is Android.Views.View v)
            {
                v.SetWillNotDraw(false);
            }
#endif
        }

        private void BaseGraphView_Loaded(object sender, EventArgs e)
        {
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
        }

        private void BaseGraphView_Unloaded(object sender, EventArgs e)
        {
            App.Current.RequestedThemeChanged -= Current_RequestedThemeChanged;
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            visibleRect.SetEmpty();
        }

        public ScrollView ScrollViewer => InternalScrollViewer;
        public VisualElement Control => this;

        public Color BottomTextColor
        {
            get => (Color)GetValue(BottomTextColorProperty);
            set => SetValue(BottomTextColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty BottomTextColorProperty =
            BindableProperty.Create(nameof(BottomTextColor), typeof(Color), typeof(BaseGraphView<T, S, E>), Colors.White, propertyChanged: OnBottomTextColorChanged);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(BaseGraphView<T, S, E>), 14d, propertyChanged: (obj, _, _) => (obj as BaseGraphView<T,S,E>)?.UpdateFontSize());

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(BaseGraphView<T, S, E>), null, propertyChanged: (obj, _, _) => (obj as BaseGraphView<T, S, E>)?.UpdateFontFamily());

        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(FontAttributesProperty);
            set => SetValue(FontAttributesProperty, value);
        }

        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(BaseGraphView<T, S, E>), FontAttributes.None, propertyChanged: (obj, _, _) => (obj as BaseGraphView<T, S, E>)?.UpdateFontFamily());

        public bool DrawIconLabels { get; set; }
        public bool DrawDataLabels { get; set; }

        protected SKTypeface GetSKTypeface(string fontFamily, FontAttributes fontAttribs)
        {
            if (fontAttribs == FontAttributes.Bold)
            {
                return SKTypeface.FromFamilyName(fontFamily, SKFontStyle.Bold);
            }
            else
            {
                return SKTypeface.FromFamilyName(fontFamily, SKFontStyle.Normal);
            }
        }

        protected float FontSizeToTextSize(double fontSize)
        {
            return (float)fontSize/* * (1f / 0.75f)*/;
        }

        private void UpdateFontSize()
        {
            this.bottomTextPaint.TextSize = FontSizeToTextSize(FontSize);
            this.bottomTextFont.Size = this.bottomTextPaint.TextSize;
            UpdateGraph();
        }

        private void UpdateFontFamily()
        {
            this.bottomTextPaint.Typeface = GetSKTypeface(FontFamily, FontAttributes);
            UpdateGraph();
        }

        private static void OnBottomTextColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue != oldValue)
            {
                (obj as BaseGraphView<T, S, E>)?.UpdateBottomTextColor();
            }
        }

        private void UpdateBottomTextColor()
        {
            bottomTextPaint.Color = BottomTextColor.ToSKColor();
            Canvas?.InvalidateSurface();
        }

        protected IconControl CreateIconControl(string WeatherIcon)
        {
            return new IconControl()
            {
                HeightRequest = IconHeight,
                WidthRequest = IconHeight,
                WeatherIcon = WeatherIcon,
                ShowAsMonochrome = false,
                //RenderTransformOrigin = new Point(0.5, 0.5)
            };
        }

        public void SetData(T data)
        {
            this.Data = data;
            NotifyDataSetChanged();
            UpdateGraph();
        }

        public bool IsDataEmpty => Data == null || Data.IsEmpty;
        public int DataCount => Data?.DataCount ?? 0;

        protected int MaxEntryCount => MaxXEntries;

        public virtual void ResetData(bool invalidate = false)
        {
            this.Data = null;
            this.xCoordinateList.Clear();
            RemoveAnimatedDrawables();
            if (invalidate)
            {
                InvalidateMeasure();
            }
        }

        public abstract void UpdateGraph();

        protected virtual void NotifyDataSetChanged()
        {
            CalcMaxX();
        }

        private void CalcMaxX()
        {
            var count = 0;

            if (!IsDataEmpty)
            {
                foreach (var set in Data.DataSets)
                {
                    if (set.DataCount > count)
                    {
                        count = set.DataCount;
                    }
                }
            }

            MaxXEntries = count;
        }

        protected void UpdateHorizontalGridNum()
        {
            horizontalGridNum = Math.Max(horizontalGridNum,
                Math.Max(MIN_HORIZONTAL_GRID_NUM, MaxEntryCount - 1));
        }

        public int GetItemPositionFromPoint(float xCoordinate)
        {
            if (horizontalGridNum <= 1)
                return 0;

            return BinarySearchPointIndex(xCoordinate);
        }

        protected int BinarySearchPointIndex(float targetXPoint)
        {
            int l = 0;
            int r = xCoordinateList.Count - 1;
            while (l <= r)
            {
                int midPt = (int)Math.Floor((l + r) / 2f);
                if (targetXPoint == xCoordinateList[midPt] - backgroundGridWidth / 2f ||
                    (targetXPoint > xCoordinateList[midPt] - backgroundGridWidth / 2f && targetXPoint < xCoordinateList[midPt] + backgroundGridWidth / 2f))
                {
                    return midPt;
                }
                else if (targetXPoint < xCoordinateList[midPt] - backgroundGridWidth / 2f)
                {
                    r = midPt - 1;
                }
                else if (targetXPoint > xCoordinateList[midPt] + backgroundGridWidth / 2f)
                {
                    l = midPt + 1;
                }
            }

            return 0;
        }

        protected sealed override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            Size size = base.MeasureOverride(widthConstraint, heightConstraint);

            if (this.ScrollViewer == null || this.Canvas == null)
            {
                return size;
            }

            ScrollViewer.HeightRequest = double.IsInfinity(heightConstraint) ? double.NaN : heightConstraint;
            ScrollViewer.WidthRequest = double.IsInfinity(widthConstraint) ? double.NaN : widthConstraint;

            OnPreMeasure();

            Canvas.WidthRequest = MaxCanvasWidth > 0
                ? Math.Min(MaxCanvasWidth, GetPreferredWidth())
                : GetPreferredWidth();
            Canvas.HeightRequest = heightConstraint;

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;

            OnPostMeasure();

            // Redraw View
            Canvas.InvalidateSurface();

            // Post the event to the dispatcher to allow the method to complete first
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnItemWidthChanged(new ItemSizeChangedEventArgs()
                {
                    NewSize = new System.Drawing.Size(xCoordinateList.Count > 0 ? (int)xCoordinateList.Last() : 0, (int)Canvas.Height)
                });
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return size;
        }

        protected virtual void OnPreMeasure()
        {
        }

        protected virtual void OnPostMeasure()
        {
        }

        protected virtual float GetGraphExtentWidth()
        {
            return (backgroundGridWidth * horizontalGridNum) + (sideLineLength * 2);
        }

        protected virtual float GetPreferredWidth()
        {
            return GetGraphExtentWidth();
        }

        protected override void OnPreDraw(SKCanvas canvas)
        {
            base.OnPreDraw(canvas);
            visibleRect.SetEmpty();
        }

        protected override void OnViewChanging()
        {
            base.OnViewChanging();
            visibleRect.Set(
                    (float)ScrollViewer.ScrollX,
                    (float)ScrollViewer.ScrollY,
                    (float)(ScrollViewer.ScrollX + ScrollViewer.Width),
                    (float)(ScrollViewer.ScrollY + ScrollViewer.Height)
            );
            Canvas.InvalidateSurface();
        }

        /* Drawables */
        private readonly Stack<SKLottieDrawable> animatedDrawables = new();

        protected void RemoveAnimatedDrawables()
        {
            while (animatedDrawables.Any())
            {
                var drw = animatedDrawables.Pop();
                drw.Stop();
                drw.InvalidateDrawable -= BaseGraphView_InvalidateDrawable;
            }
        }

        protected void AddAnimatedDrawable(SKLottieDrawable drawable)
        {
            if (!animatedDrawables.Contains(drawable))
            {
                drawable.InvalidateDrawable += BaseGraphView_InvalidateDrawable;
                drawable.Start();
                animatedDrawables.Push(drawable);
            }
        }

        private void BaseGraphView_InvalidateDrawable(object sender, EventArgs e)
        {
            Canvas?.InvalidateSurface();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    bottomTextFont?.Dispose();
                    bottomTextPaint?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseGraphView()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
