#if WINDOWS
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SimpleWeather.NET.Helpers;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using ScrollView = SimpleWeather.NET.Controls.Graphs.GraphScrollView;
using Size = Windows.Foundation.Size;
using FontWeight = Windows.UI.Text.FontWeight;
using FontWeights = Microsoft.UI.Text.FontWeights;
#else
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Primitives;
using SimpleWeather.Maui;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using ScrollView = Microsoft.Maui.Controls.ScrollView;
#endif
using SimpleWeather.NET.Utils;
using SimpleWeather.SkiaSharp;
using SkiaSharp;
using RectF = System.Drawing.RectangleF;

namespace SimpleWeather.NET.Controls.Graphs
{
    public abstract partial class BaseGraphView<T, S, E> : BaseGraphViewControl, IGraph, IDisposable
        where T : GraphData<S, E> where S : GraphDataSet<E> where E : GraphEntry
    {
        private bool disposedValue;

        protected RectF visibleRect = new();
        protected readonly ScrollView ScrollViewer;

        internal T Data { get; set; }
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
        protected readonly float bottomTextTopMargin = 12f; // 6dp

        protected float sideLineLength = 0;
        protected float backgroundGridWidth = 45f; // 45dp
        protected float longestTextWidth;

        protected BaseGraphView(ScrollView scrollViewer) : base()
        {
            xCoordinateList = new List<float>();

            bottomTextPaint = new SKPaint()
            {
                IsAntialias = false,
                TextSize = FontSizeToTextSize(FontSize),
#if WINDOWS
                Typeface = GetSKTypeface(FontFamily, FontWeight),
#else

                Typeface = GetSKTypeface(FontFamily, FontAttributes),
#endif
                TextAlign = SKTextAlign.Center,
                Style = SKPaintStyle.Fill,
                Color = BottomTextColor.ToSKColor(),
            };

            bottomTextFont = new SKFont
            {
                Edging = SKFontEdging.SubpixelAntialias,
                Size = bottomTextPaint.TextSize
            };

#if WINDOWS
            RegisterPropertyChangedCallback(FontSizeProperty, OnDependencyPropertyChanged);
            RegisterPropertyChangedCallback(FontWeightProperty, OnDependencyPropertyChanged);
#endif

#if !WINDOWS
            this.HandlerChanged += BaseGraphView_HandlerChanged;
            this.Loaded += BaseGraphView_Loaded;
            this.Unloaded += BaseGraphView_Unloaded;
#endif

#if !WINDOWS
            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
                IconSize = 36f;
            else
                IconSize = 48f;
#endif

            this.ScrollViewer = scrollViewer;
        }

#if !WINDOWS
        private void BaseGraphView_HandlerChanged(object sender, EventArgs e)
        {
#if ANDROID
            if (this.Handler?.PlatformView is Android.Views.View v)
            {
                v.SetWillNotDraw(false);
            }
#elif IOS || MACCATALYST
            if (this.Handler?.PlatformView is UIKit.UIView v)
            {
                v.ClearsContextBeforeDrawing = false;
                v.ContentMode = UIKit.UIViewContentMode.Redraw;
            }
#endif
        }

        private void BaseGraphView_Loaded(object sender, EventArgs e)
        {
            if (App.Current != null)
            {
                App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
            }
        }

        private void BaseGraphView_Unloaded(object sender, EventArgs e)
        {
            if (App.Current != null)
            {
                App.Current.RequestedThemeChanged -= Current_RequestedThemeChanged;
            }
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            visibleRect.SetEmpty();
        }
#endif

        public Color BottomTextColor
        {
            get => (Color)GetValue(BottomTextColorProperty);
            set => SetValue(BottomTextColorProperty, value);
        }

        public float IconSize
        {
            get => (float)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public bool FillParentWidth
        {
            get => (bool)GetValue(FillParentWidthProperty);
            set => SetValue(FillParentWidthProperty, value);
        }
#if WINDOWS
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register(nameof(BottomTextColor), typeof(Color), typeof(BaseGraphView<T, S, E>), new PropertyMetadata(Colors.White, OnBottomTextColorChanged));

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(BaseGraphView<T, S, E>), new PropertyMetadata(48f));

        public static readonly DependencyProperty FillParentWidthProperty =
            DependencyProperty.Register(nameof(FillParentWidth), typeof(bool), typeof(BaseGraphView<T, S, E>), new PropertyMetadata(false, (obj, args) => (obj as FrameworkElement)?.InvalidateMeasure()));
#else
        public static readonly BindableProperty BottomTextColorProperty =
            BindableProperty.Create(nameof(BottomTextColor), typeof(Color), typeof(BaseGraphView<T, S, E>), Colors.White, propertyChanged: OnBottomTextColorChanged);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(BaseGraphView<T, S, E>), 14d, propertyChanged: (obj, _, _) => (obj as BaseGraphView<T, S, E>)?.UpdateFontSize());

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

        public static readonly BindableProperty IconSizeProperty =
            BindableProperty.Create(nameof(IconSize), typeof(float), typeof(BaseGraphView<T, S, E>), 48f);

        public static readonly BindableProperty FillParentWidthProperty =
            BindableProperty.Create(nameof(FillParentWidth), typeof(bool), typeof(BaseGraphView<T,S,E>), false, propertyChanged: (obj, _, _) => (obj as BaseGraphView<T, S, E>)?.InvalidateMeasure());
#endif

        public bool DrawIconLabels { get; set; }
        public bool DrawDataLabels { get; set; }

#if WINDOWS
        protected SKTypeface GetSKTypeface(FontFamily fontFamily, FontWeight fontWeight)
        {
            if (fontWeight.Weight >= FontWeights.Medium.Weight)
            {
                return SKTypeface.FromFamilyName(fontFamily.Source, SKFontStyle.Bold);
            }
            else
            {
                return SKTypeface.FromFamilyName(fontFamily.Source, SKFontStyle.Normal);
            }
        }
#else
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
#endif

        protected float FontSizeToTextSize(double fontSize)
        {
            return (float)fontSize/* * (1f / 0.75f)*/;
        }

#if WINDOWS
        private void OnDependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
        {
            if (property == FontSizeProperty)
            {
                (obj as BaseGraphView<T, S, E>)?.UpdateFontSize();
            }
            else if (property == FontWeightProperty)
            {
                (obj as BaseGraphView<T, S, E>)?.UpdateFontFamily();
            }
        }

        private static void OnBottomTextColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (obj as BaseGraphView<T, S, E>)?.UpdateBottomTextColor();
            }
        }
#else
        private static void OnBottomTextColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue != oldValue)
            {
                (obj as BaseGraphView<T, S, E>)?.UpdateBottomTextColor();
            }
        }
#endif

        protected virtual void UpdateFontSize()
        {
            this.bottomTextPaint.TextSize = FontSizeToTextSize(FontSize);
            this.bottomTextFont.Size = this.bottomTextPaint.TextSize;
            UpdateGraph();
        }

        protected virtual void UpdateFontFamily()
        {
#if WINDOWS
            this.bottomTextPaint.Typeface = GetSKTypeface(FontFamily, FontWeight);
#else
            this.bottomTextPaint.Typeface = GetSKTypeface(FontFamily, FontAttributes);
#endif
            UpdateGraph();
        }

        private void UpdateBottomTextColor()
        {
            bottomTextPaint.Color = BottomTextColor.ToSKColor();
            Invalidate();
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
                Invalidate();
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

            if (Data is { IsEmpty: false })
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

#if !WINDOWS
        protected double GetMeasurement(double constraint, double desiredSize, double measuredSize, double maxSize)
        {
            if (desiredSize > 0)
            {
                return maxSize > 0 ? Math.Min(desiredSize, maxSize) : desiredSize;
            }

            return measuredSize;
        }
#endif

#if WINDOWS
        protected sealed override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            size.Width = Math.Floor(size.Width <= 0 ? availableSize.Width : size.Width);
            size.Height = Math.Floor(size.Height <= 0 ? availableSize.Height : size.Height);
            availableSize = size;
#else
        protected sealed override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            var availableSize = base.MeasureOverride(widthConstraint, heightConstraint);
            availableSize.Width = Math.Floor(availableSize.Width <= 0 ? widthConstraint : availableSize.Width);
            availableSize.Height = Math.Floor(availableSize.Height <= 0 ? heightConstraint : availableSize.Height);
#endif
            OnPreMeasure();

            var desiredWidth = GetPreferredWidth();

            var measureMode = (BaseGraphScrollView<T, S, E>.MeasureMode)(GetValue(BaseGraphScrollView<T, S, E>.MeasureModeProperty) ?? BaseGraphScrollView<T, S, E>.MeasureMode.Undefined);
            var resolvedWidth = measureMode switch
            {
                BaseGraphScrollView<T, S, E>.MeasureMode.AtMost => MaxCanvasWidth > 0 ? Math.Min(Math.Min(desiredWidth, availableSize.Width), MaxCanvasWidth) : desiredWidth,
                BaseGraphScrollView<T, S, E>.MeasureMode.Exactly => MaxCanvasWidth > 0 ? Math.Min(MaxCanvasWidth, availableSize.Width) : availableSize.Width,
                _ => MaxCanvasWidth > 0 ? Math.Min(desiredWidth, MaxCanvasWidth) : desiredWidth
            };

            ViewWidth = MathF.Floor((float)resolvedWidth);
#if WINDOWS
            ViewHeight = (float)availableSize.Height;
            Canvas.Width = float.IsInfinity(ViewWidth) ? -1 : ViewWidth;
            Canvas.Height = float.IsInfinity(ViewHeight) ? -1 : ViewHeight;
#else
            ViewHeight = MathF.Floor((float)availableSize.Height);
            Canvas.WidthRequest = float.IsInfinity(ViewWidth) ? -1 : ViewWidth;
            Canvas.HeightRequest = float.IsInfinity(ViewHeight) ? -1 : ViewHeight;
#endif

            OnPostMeasure();

            // Redraw View
            Invalidate();

            // Post the event to the dispatcher to allow the method to complete first
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#if WINDOWS
            DispatcherQueue.EnqueueAsync(() =>
#else
            Dispatcher.Dispatch(() =>
#endif
            {
                OnItemWidthChanged(new ItemSizeChangedEventArgs()
                {
                    NewSize = new System.Drawing.Size(xCoordinateList.Count > 0 ? (int)xCoordinateList.Last() : 0,
                        (int)(Canvas?.Height ?? 0))
                });
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

#if WINDOWS
            return new Size(resolvedWidth, availableSize.Height);
#else
            return new Size(ViewWidth + Margin.HorizontalThickness, availableSize.Height/* + Margin.VerticalThickness*/);
#endif
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

        /* Drawables */
        private readonly Stack<SKLottieDrawable> animatedDrawables = new();

        protected void RemoveAnimatedDrawables()
        {
            while (animatedDrawables.Count != 0)
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
            try
            {
                Invalidate();
            }
            // Note: May occur if window is closed and canvas not disposed?
            catch (NullReferenceException)
            {
                RemoveAnimatedDrawables();
            }
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
                    RemoveAnimatedDrawables();
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
