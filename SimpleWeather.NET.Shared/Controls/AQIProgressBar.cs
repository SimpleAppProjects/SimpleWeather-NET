using SimpleWeather.NET.Utils;
using SimpleWeather.Utils;
using SkiaSharp;
#if WINDOWS
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endif
#if WINDOWS
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Windows.SKPaintSurfaceEventArgs;
using SKXamlCanvas = SkiaSharp.Views.Windows.SKXamlCanvas;
#else
using Font = Microsoft.Maui.Font;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs;
using SKXamlCanvas = SkiaSharp.Views.Maui.Controls.SKCanvasView;
#endif
#if !WINDOWS
using RoutedEventArgs = System.EventArgs;
#endif

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.NET.Controls
{
#if WINDOWS
    [TemplatePart(Name = nameof(Canvas), Type = typeof(SKXamlCanvas))]
    public sealed partial class AQIProgressBar : Control, IDisposable
#else
    public sealed partial class AQIProgressBar : TemplatedView, IDisposable
#endif
    {
        private bool IsInitialized { get; set; } = false;

        private SKXamlCanvas Canvas { get; set; }

        private float ViewHeight;
        private float ViewWidth;
        private float bottomTextHeight;
        private float thumbSize = 16f * 5 / 8f;

        private readonly SKPaint trackPaint;
        private readonly SKPaint bottomTextPaint;
        private readonly SKFont bottomTextFont;
        private readonly SKPaint thumbPaint;
        private float bottomTextDescent;

        private readonly float bottomTextTopMargin = 8f; // 8dp
        private readonly float bottomTextBottomMargin = 2f; // 2dp

        private float sideLineLength = 45f / 3f * 2f; // 45dp / 3 * 2
        private float backgroundGridWidth = 45f; // 45dp
        private float longestTextWidth = 0f;

        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for ThumbColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbColorProperty =
            DependencyProperty.Register(nameof(ThumbColor), typeof(Color), typeof(AQIProgressBar),
                new PropertyMetadata(Colors.GhostWhite, OnThumbColorChanged));
#else
        public static readonly BindableProperty ThumbColorProperty =
            BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(AQIProgressBar), Colors.White,
                propertyChanged: OnThumbColorChanged);
#endif

        public Color BottomTextColor
        {
            get => (Color)GetValue(BottomTextColorProperty);
            set => SetValue(BottomTextColorProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register(nameof(BottomTextColor), typeof(Color), typeof(AQIProgressBar),
                new PropertyMetadata(Colors.White, OnBottomTextColorChanged));
#else
        public static readonly BindableProperty BottomTextColorProperty =
            BindableProperty.Create(nameof(BottomTextColor), typeof(Color), typeof(AQIProgressBar), Colors.White,
                propertyChanged: OnBottomTextColorChanged);
#endif

        public int Progress
        {
            get => (int)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(nameof(Progress), typeof(int), typeof(AQIProgressBar),
                new PropertyMetadata((int)0, OnProgressChanged));
#else
        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(int), typeof(AQIProgressBar), (int)0,
                propertyChanged: OnProgressChanged);
#endif
        
#if !WINDOWS
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(AQIProgressBar), 14d, propertyChanged: OnFontChanged);

        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(FontAttributesProperty);
            set => SetValue(FontAttributesProperty, value);
        }

        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(AQIProgressBar), FontAttributes.None, propertyChanged: OnFontChanged);
#endif

        private static readonly List<(Range, Color)> COLOR_MAP =
        [
            (0..51, Colors.LimeGreen),
#if WINDOWS
            (51..101, Color.FromArgb(0xff, 0xff, 0xde, 0x33)),
            (101..151, Color.FromArgb(0xff, 0xff, 0x99, 0x33)),
            (151..201, Color.FromArgb(0xff, 0xcc, 0x00, 0x33)),
            (201..301, Color.FromArgb(0xff, 0xaa, 0x00, 0xff)),
            (301..501, Color.FromArgb(0xff, 0xbd, 0x00, 0x35))
#else
            (51..101, Color.FromRgba(0xff, 0xde, 0x33, 0xff)),
            (101..151, Color.FromRgba(0xff, 0x99, 0x33, 0xff)),
            (151..201, Color.FromRgba(0xcc, 0x00, 0x33, 0xff)),
            (201..301, Color.FromRgba(0xaa, 0x00, 0xff, 0xff)),
            (301..501, Color.FromRgba(0xbd, 0x00, 0x35, 0xff))
#endif
        ];

        public AQIProgressBar()
        {
#if WINDOWS
            this.DefaultStyleKey = typeof(AQIProgressBar);
#endif

            this.Loaded += AQIProgressBar_Loaded;
            this.Unloaded += AQIProgressBar_Unloaded;

#if WINDOWS
            DispatcherQueue.EnqueueAsync(() =>
#else
            Dispatcher.Dispatch(() =>
#endif
            {
#if WINDOWS
                Canvas?.Invalidate();
#else
                Canvas?.InvalidateSurface();
#endif
            });

            trackPaint = new SKPaint()
            {
                Style = SKPaintStyle.StrokeAndFill,
                IsAntialias = true,
                StrokeWidth = 2f,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

            bottomTextPaint = new SKPaint()
            {
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                Style = SKPaintStyle.Fill,
                Color = BottomTextColor.ToSKColor()
            };

            bottomTextFont = new SKFont
            {
                Edging = SKFontEdging.SubpixelAntialias,
#if WINDOWS
                Size = FontSizeToTextSize(FontSize),
                Typeface = GetSKTypeface(FontFamily, FontWeight),
#else
                Size = FontSizeToTextSize(FontSize),
                Typeface = GetSKTypeface(FontAttributes),
#endif
            };

            thumbPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = ThumbColor.ToSKColor(),
                ImageFilter = SKImageFilter.CreateDropShadow(0f, 0f, 1f, 1f, Colors.Black.WithAlpha(0x70).ToSKColor())
            };

            OnTextPaintUpdated();

#if WINDOWS
            RegisterPropertyChangedCallback(FontSizeProperty, OnDependencyPropertyChanged);
            RegisterPropertyChangedCallback(FontWeightProperty, OnDependencyPropertyChanged);
#else
            this.HandlerChanged += AQIProgressBar_HandlerChanged;
#endif

            IsInitialized = true;
        }

#if !WINDOWS
        private void AQIProgressBar_HandlerChanged(object sender, EventArgs e)
        {
#if ANDROID
            if (this.Handler?.PlatformView is Android.Views.View v)
            {
                v.SetWillNotDraw(false);
            }
#endif
        }
#endif

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Canvas = GetTemplateChild(nameof(Canvas)) as SKXamlCanvas;

            if (Canvas != null)
            {
                Canvas.PaintSurface += Canvas_PaintSurface;
            }
        }

#if WINDOWS
        private static void OnThumbColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
#else
        private static void OnThumbColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue != oldValue)
#endif
            {
                (obj as AQIProgressBar)?.UpdateThumbColor();
            }
        }

        private void UpdateThumbColor()
        {
            if (!IsInitialized) return;

            thumbPaint.Color = ThumbColor.ToSKColor();
#if WINDOWS
            Canvas?.Invalidate();
#else
            Canvas?.InvalidateSurface();
#endif
        }

#if WINDOWS
        private static void OnBottomTextColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
#else
        private static void OnBottomTextColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue != oldValue)
#endif
            {
                (obj as AQIProgressBar)?.UpdateBottomTextColor();
            }
        }

        private void UpdateBottomTextColor()
        {
            if (!IsInitialized) return;

            bottomTextPaint.Color = BottomTextColor.ToSKColor();
#if WINDOWS
            Canvas?.Invalidate();
#else
            Canvas?.InvalidateSurface();
#endif
        }

#if WINDOWS
        private static void OnProgressChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
#else
        private static void OnProgressChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue != oldValue)
#endif
            {
                if (obj is AQIProgressBar aqiBar)
                {
                    if (!aqiBar.IsInitialized) return;

#if WINDOWS
                    aqiBar?.Canvas?.Invalidate();
#else
                    aqiBar?.Canvas?.InvalidateSurface();
#endif
                }
            }
        }

        private void AQIProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.PaintSurface += Canvas_PaintSurface;
            }
        }

        private void AQIProgressBar_Unloaded(object sender, RoutedEventArgs e)
        {
            Canvas?.Let(canvas =>
            {
                try
                {
                    canvas.PaintSurface -= Canvas_PaintSurface;
                }
                catch
                {
                }
            });
        }

#if WINDOWS
        private static SKTypeface GetSKTypeface(FontFamily fontFamily, FontWeight fontWeight)
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
        private static SKTypeface GetSKTypeface(FontAttributes fontAttribs)
        {
            return GetSKTypeface(Font.Default.Family, fontAttribs);
        }

        private static SKTypeface GetSKTypeface(string fontFamily, FontAttributes fontAttribs)
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

        private static float FontSizeToTextSize(double fontSize)
        {
            return (float)fontSize;
        }

#if WINDOWS
        private void OnDependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
        {
            if (property == FontSizeProperty)
            {
                this.bottomTextFont.Size = FontSizeToTextSize(FontSize);
                OnTextPaintUpdated();
                Canvas?.Invalidate();
            }
            else if (property == FontWeightProperty)
            {
                this.bottomTextFont.Typeface = GetSKTypeface(FontFamily, FontWeight);
                OnTextPaintUpdated();
                Canvas?.Invalidate();
            }
        }
#else
        private static void OnFontChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                if (obj is AQIProgressBar aqiBar)
                {
                    if (!aqiBar.IsInitialized) return;

                    aqiBar.bottomTextFont.Size = FontSizeToTextSize(aqiBar.FontSize);
                    aqiBar.bottomTextFont.Typeface = GetSKTypeface(aqiBar.FontAttributes);
                    aqiBar.OnTextPaintUpdated();
                    aqiBar.Canvas?.InvalidateSurface();
                }
            }
        }
#endif

        private void OnTextPaintUpdated()
        {
            bottomTextFont.MeasureText("T", out var r);
            bottomTextHeight = r.Height;
            bottomTextDescent = Math.Abs(r.Bottom);
        }

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            // get the screen density for scaling
#if WINDOWS
            var scale = 1f;
            //var scale = (float)XamlRoot.RasterizationScale;
#else
            var scale = 1f;
#endif

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(SKColors.Transparent);

            OnDraw(canvas);
        }

#if !WINDOWS
        private float GraphTop => bottomTextTopMargin + bottomTextBottomMargin + bottomTextHeight + bottomTextDescent;
#endif

        private void OnDraw(SKCanvas canvas)
        {
            longestTextWidth = 0f;
            DrawLabels(canvas);
            DrawTrack(canvas);
            DrawThumb(canvas);
        }

        private void DrawLabels(SKCanvas canvas)
        {
            // Draw bottom text
#if WINDOWS
            float y = ViewHeight - bottomTextBottomMargin;
#else
            float y = GraphTop + trackPaint.StrokeWidth + bottomTextTopMargin + bottomTextHeight + bottomTextDescent;
#endif

            for (int i = 0; i < COLOR_MAP.Count; i++)
            {
                var pair = COLOR_MAP[i];

                if (i == 0)
                {
                    var text = pair.Item1.Start.Value.ToString();
                    var bounds = bottomTextFont.MeasureText(text);
                    canvas.DrawText(text, bounds / 2 + thumbSize + (float)Margin.Left, y, bottomTextFont,
                        bottomTextPaint);
                    longestTextWidth = Math.Max(longestTextWidth, bounds);
                }
                else
                {
                    var text = (pair.Item1.Start.Value - 1).ToString();
                    var bounds = bottomTextFont.MeasureText(text);
                    var pct = (pair.Item1.Start.Value - 1) / 500f;

                    canvas.DrawText(
                        text,
                        ViewWidth * pct - trackPaint.StrokeWidth / 2f -
                        (pct >= 1f ? (float)Margin.Right + longestTextWidth / 2 : 0),
                        y,
                        bottomTextFont, bottomTextPaint
                    );
                    longestTextWidth = Math.Max(longestTextWidth, bounds);
                }

                if (i == COLOR_MAP.Count - 1)
                {
                    var text = (pair.Item1.End.Value - 1).ToString();
                    var bounds = bottomTextFont.MeasureText(text);

                    canvas.DrawText(
                        text,
                        ViewWidth - bounds / 2 - longestTextWidth / 2,
                        y,
                        bottomTextFont, bottomTextPaint
                    );
                    longestTextWidth = Math.Max(longestTextWidth, bounds);
                }
            }
        }

        private void DrawTrack(SKCanvas canvas)
        {
#if WINDOWS
            var y =
                ViewHeight - bottomTextTopMargin - bottomTextBottomMargin - bottomTextHeight - bottomTextDescent -
                trackPaint.StrokeWidth;
#else
            var y = GraphTop;
#endif

            COLOR_MAP.Reverse<(Range, Color)>().ForEach(pair =>
            {
                trackPaint.Color = pair.Item2.ToSKColor();
                var pct = Math.Min(pair.Item1.End.Value / 500f, 1f);
                canvas.DrawRoundRect(
                    new SKRect(
                        trackPaint.StrokeWidth / 2f + longestTextWidth / 2 + (float)Margin.Left,
#if WINDOWS
                        y + trackPaint.StrokeWidth / 2f,
#else
                        y - trackPaint.StrokeWidth / 2f,
#endif
                        ViewWidth * pct - trackPaint.StrokeWidth / 2f -
                        (pct >= 1f ? (float)Margin.Right + longestTextWidth / 2 : 0),
#if WINDOWS
                        y - trackPaint.StrokeWidth / 2f
#else
                        y + trackPaint.StrokeWidth / 2f
#endif
                    ),
                    trackPaint.StrokeWidth / 2f,
                    trackPaint.StrokeWidth / 2f,
                    trackPaint
                );
            });
        }

        private void DrawThumb(SKCanvas canvas)
        {
            var progress = Math.Min(Progress, 500);
            var x = ViewWidth * (progress / 500f) +
                    (progress == 0 ? thumbSize / 2f + longestTextWidth + (float)Margin.Left : 0f) -
                    (progress >= 500 ? thumbSize / 1.75f + longestTextWidth / 2 : 0);
#if WINDOWS
            var y =
                ViewHeight - bottomTextTopMargin - bottomTextBottomMargin - bottomTextHeight - bottomTextDescent -
                trackPaint.StrokeWidth / 2f;
#else
            var y = GraphTop + trackPaint.StrokeWidth / 2f;
#endif

            canvas.DrawCircle(x, y, thumbSize / 2f, thumbPaint);
        }

#if WINDOWS
        protected sealed override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);

            if (this.Canvas == null)
            {
                return size;
            }

            Canvas.Width = availableSize.Width;
            Canvas.Height =
                double.IsFinite(availableSize.Height)
                    ? availableSize.Height
                    : Math.Max(MinHeight,
                        ((bottomTextTopMargin + bottomTextHeight + bottomTextDescent)) + trackPaint.StrokeWidth * 3 +
                        thumbSize);

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;

            // Redraw View
            Canvas?.Invalidate();

            return new Size(ViewWidth, ViewHeight);
        }
#else
        protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            var desiredHeight = Math.Max(MinimumHeightRequest,
                GraphTop + trackPaint.StrokeWidth + bottomTextTopMargin + bottomTextHeight * 2 + bottomTextDescent +
                bottomTextBottomMargin) + Margin.VerticalThickness;

            var size = base.MeasureOverride(widthConstraint, desiredHeight);

            return new Size(size.Width, desiredHeight);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (this.Canvas == null)
            {
                return;
            }

            ViewWidth = (float)(Canvas.WidthRequest = width);
            ViewHeight = (float)(Canvas.HeightRequest = height);

            // Redraw View
            Canvas?.InvalidateSurface();
        }
#endif

        public void Dispose()
        {
            trackPaint?.Dispose();
            bottomTextPaint?.Dispose();
            bottomTextFont?.Dispose();
            thumbPaint?.Dispose();
        }
    }
}