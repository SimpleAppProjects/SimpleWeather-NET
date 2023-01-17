using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.Utils;
using SimpleWeather.SkiaSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using RectF = System.Drawing.RectangleF;

namespace SimpleWeather.NET.Controls.Graphs
{
    [TemplatePart(Name = nameof(InternalScrollViewer), Type = typeof(ScrollViewer))]
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
#if ANDROID
            SetWillNotDraw(false);
#endif

            xCoordinateList = new List<float>();

            bottomTextPaint = new SKPaint()
            {
                IsAntialias = false,
                TextSize = FontSizeToTextSize(FontSize),
                Typeface = GetSKTypeface(FontFamily, FontWeight),
                TextAlign = SKTextAlign.Center,
                Style = SKPaintStyle.Fill,
                Color = BottomTextColor.ToSKColor(),
            };

            bottomTextFont = new SKFont
            {
                Edging = SKFontEdging.SubpixelAntialias,
                Size = bottomTextPaint.TextSize
            };

            RegisterPropertyChangedCallback(FontSizeProperty, OnDependencyPropertyChanged);
            RegisterPropertyChangedCallback(FontWeightProperty, OnDependencyPropertyChanged);
        }

        public ScrollViewer ScrollViewer => InternalScrollViewer;
        public FrameworkElement Control => this;

        public Color BottomTextColor
        {
            get => (Color)GetValue(BottomTextColorProperty);
            set
            {
                SetValue(BottomTextColorProperty, value);
                bottomTextPaint.Color = value.ToSKColor();
            }
        }

        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register("BottomTextColor", typeof(Color), typeof(BaseGraphView<T, S, E>), new PropertyMetadata(Colors.White));

        public bool DrawIconLabels { get; set; }
        public bool DrawDataLabels { get; set; }

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

        protected float FontSizeToTextSize(double fontSize)
        {
            return (float)fontSize/* * (1f / 0.75f)*/;
        }

        private void OnDependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
        {
            if (property == FontSizeProperty)
            {
                this.bottomTextPaint.TextSize = FontSizeToTextSize(FontSize);
                this.bottomTextFont.Size = this.bottomTextPaint.TextSize;
                UpdateGraph();
            }
            else if (property == FontWeightProperty)
            {
                this.bottomTextPaint.Typeface = GetSKTypeface(FontFamily, FontWeight);
                UpdateGraph();
            }
        }

        protected IconControl CreateIconControl(string WeatherIcon)
        {
            return new IconControl()
            {
                Height = IconHeight,
                Width = IconHeight,
                WeatherIcon = WeatherIcon,
                ShowAsMonochrome = false,
                RenderTransformOrigin = new Point(0.5, 0.5)
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

        protected sealed override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);

            if (this.ScrollViewer == null || this.Canvas == null)
            {
                return size;
            }

            ScrollViewer.Height = double.IsInfinity(availableSize.Height) ? double.NaN : availableSize.Height;
            ScrollViewer.Width = double.IsInfinity(availableSize.Width) ? double.NaN : availableSize.Width;

            OnPreMeasure();

            Canvas.Width = MaxCanvasWidth > 0
                ? Math.Min(MaxCanvasWidth, GetPreferredWidth())
                : GetPreferredWidth();
            Canvas.Height = availableSize.Height;

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;

            OnPostMeasure();

            // Redraw View
            Canvas.Invalidate();

            // Post the event to the dispatcher to allow the method to complete first
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            DispatcherQueue.EnqueueAsync(() =>
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
                    (float)ScrollViewer.HorizontalOffset,
                    (float)ScrollViewer.VerticalOffset,
                    (float)(ScrollViewer.HorizontalOffset + ScrollViewer.ActualWidth),
                    (float)(ScrollViewer.VerticalOffset + ScrollViewer.ActualHeight)
            );
            Canvas.Invalidate();
        }

#if ANDROID
        protected override void OnConfigurationChanged(Android.Content.Res.Configuration? newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            visibleRect.SetEmpty();
        }
#endif

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
            Canvas?.Invalidate();
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
