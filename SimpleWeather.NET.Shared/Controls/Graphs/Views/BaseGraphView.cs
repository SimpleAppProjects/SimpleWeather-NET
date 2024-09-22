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
#else
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Primitives;
using SimpleWeather.Maui;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
#endif
using SimpleWeather.NET.Utils;
using SimpleWeather.SkiaSharp;
using SkiaSharp;
using RectF = System.Drawing.RectangleF;

namespace SimpleWeather.NET.Controls.Graphs
{
#if WINDOWS
    [TemplatePart(Name = nameof(InternalScrollViewer), Type = typeof(ScrollViewer))]
#endif
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

#if WINDOWS
        protected float IconHeight = 48f; // 30dp // 48dp
#else
        protected readonly float IconHeight;
#endif

        public BaseGraphView() : base()
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
                IconHeight = 36f;
            else
                IconHeight = 48f;
#endif
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

#if WINDOWS
        public ScrollViewer ScrollViewer => InternalScrollViewer;
        public FrameworkElement Control => this;
#else
        public ScrollView ScrollViewer => InternalScrollViewer;
        public VisualElement Control => this;
#endif

        public Color BottomTextColor
        {
            get => (Color)GetValue(BottomTextColorProperty);
            set => SetValue(BottomTextColorProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register(nameof(BottomTextColor), typeof(Color), typeof(BaseGraphView<T, S, E>), new PropertyMetadata(Colors.White, OnBottomTextColorChanged));
#else
        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
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
#if WINDOWS
            Canvas?.Invalidate();
#else
            Canvas?.InvalidateSurface();
#endif
        }

        protected IconControl CreateIconControl(string WeatherIcon)
        {
            return new IconControl()
            {
                WeatherIcon = WeatherIcon,
                ShowAsMonochrome = false,
#if WINDOWS
                Height = IconHeight,
                Width = IconHeight,
                RenderTransformOrigin = new Point(0.5, 0.5)
#else
                HeightRequest = IconHeight,
                WidthRequest = IconHeight,
                IconWidth = 30,
                IconHeight = 30,
                VerticalOptions = LayoutOptions.Center,
#endif
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
            Size size = base.MeasureOverride(availableSize);
#else
        protected sealed override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            Size size = base.MeasureOverride(widthConstraint, heightConstraint);
#endif

            if (this.ScrollViewer == null || this.Canvas == null)
            {
                return size;
            }

#if WINDOWS
            ScrollViewer.Height = double.IsInfinity(availableSize.Height) ? double.NaN : availableSize.Height;
            ScrollViewer.Width = double.IsInfinity(availableSize.Width) ? double.NaN : availableSize.Width;
#else
            ScrollViewer.HeightRequest = double.IsInfinity(heightConstraint) ? -1d : size.Height;
            ScrollViewer.WidthRequest = double.IsInfinity(widthConstraint) ? -1d : double.NaN;
#endif

            OnPreMeasure();

#if WINDOWS
            Canvas.Width =
#else
            Canvas.WidthRequest =
#endif
                MaxCanvasWidth > 0
                    ? Math.Min(MaxCanvasWidth, GetPreferredWidth())
                    : GetPreferredWidth();
#if WINDOWS
            Canvas.Height = availableSize.Height;
#else
            Canvas.HeightRequest = size.Height;
#endif

#if WINDOWS
            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;
#else
            ViewHeight = (float)Canvas.HeightRequest;
            ViewWidth = (float)Canvas.WidthRequest;
#endif

            OnPostMeasure();

            // Redraw View
#if WINDOWS
            Canvas.Invalidate();
#else
            Canvas.InvalidateSurface();
#endif

            // Post the event to the dispatcher to allow the method to complete first
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#if WINDOWS
            DispatcherQueue.EnqueueAsync(() =>
#else
            MainThread.InvokeOnMainThreadAsync(() =>
#endif
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

#if WINDOWS
        protected override void OnViewChanging()
        {
            base.OnViewChanging();
#else
        protected override void OnViewChanged()
        {
            base.OnViewChanged();
#endif
#if WINDOWS
            visibleRect.Set(
                    (float)ScrollViewer.HorizontalOffset,
                    (float)ScrollViewer.VerticalOffset,
                    (float)(ScrollViewer.HorizontalOffset + ScrollViewer.ActualWidth),
                    (float)(ScrollViewer.VerticalOffset + ScrollViewer.ActualHeight)
            );
            Canvas.Invalidate();
#else
            visibleRect.Set(
                    (float)ScrollViewer.ScrollX,
                    (float)ScrollViewer.ScrollY,
                    (float)(ScrollViewer.ScrollX + ScrollViewer.Width),
                    (float)(ScrollViewer.ScrollY + ScrollViewer.Height)
            );
            Canvas.InvalidateSurface();
#endif
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
#if WINDOWS
                Canvas?.Invalidate();
#else
                Canvas?.InvalidateSurface();
#endif
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
