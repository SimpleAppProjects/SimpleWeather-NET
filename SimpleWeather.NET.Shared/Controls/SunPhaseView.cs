#if WINDOWS
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endif
using SimpleWeather.Common.Controls;
using SimpleWeather.NET.Utils;
using SimpleWeather.Utils;
using SkiaSharp;
using System;
using System.Collections.Immutable;
#if WINDOWS
using SimpleWeather.NET.Helpers;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Windows.SKPaintSurfaceEventArgs;
using SKXamlCanvas = SkiaSharp.Views.Windows.SKXamlCanvas;
#else
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs;
using SKXamlCanvas = SkiaSharp.Views.Maui.Controls.SKCanvasView;
using Microsoft.Maui.Layouts;
#endif
#if !WINDOWS
using RoutedEventArgs = System.EventArgs;
#endif

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.NET.Controls
{
#if WINDOWS
    [TemplatePart(Name = nameof(Canvas), Type = typeof(SKXamlCanvas))]
    public sealed partial class SunPhaseView : Control, IDisposable
#else
    public sealed partial class SunPhaseView : TemplatedView, IDisposable
#endif
    {
#if WINDOWS
        private const string SunIconUri = "ms-appx:///Assets/FullSun.png";
#else
        private const string SunIconUri = "full_sun.png";
#endif

        private bool IsInitialized { get; set; } = false;

        private SKXamlCanvas Canvas { get; set; }

        private float ViewHeight;
        private float ViewWidth;
        private float bottomTextHeight;

        private readonly SKPaint backgroundPaint;
        private readonly SKPaint fullArcPaint;
        private readonly SKPaint pathArcPaint;
        private readonly SKPaint bottomTextPaint;
        private readonly SKFont bottomTextFont;
        private readonly SKPaint bigCirPaint;
        private float bottomTextDescent;

        private DateTime sunrise;
        private DateTime sunset;
        private TimeSpan offset = TimeSpan.Zero;

        private float sunriseX;
        private float sunsetX;

        private readonly float bottomTextTopMargin = 8f; // 8dp
        private readonly float DOT_RADIUS = 4f; // 4dp

        private float sideLineLength = 45f / 3f * 2f; // 45dp / 3 * 2
        private float backgroundGridWidth = 45f; // 45dp

        private SKImage SunIcon;

        public Color PaintColor
        {
            get => (Color)GetValue(PaintColorProperty);
            set => SetValue(PaintColorProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for PaintColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PaintColorProperty =
            DependencyProperty.Register(nameof(PaintColor), typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.Yellow, OnPaintColorChanged));
#else
        public static readonly BindableProperty PaintColorProperty =
            BindableProperty.Create(nameof(PaintColor), typeof(Color), typeof(SunPhaseView), Colors.Yellow, propertyChanged: OnPaintColorChanged);
#endif

        public Color PhaseArcColor
        {
            get => (Color)GetValue(PhaseArcColorProperty);
            set => SetValue(PhaseArcColorProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for PhaseArcColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhaseArcColorProperty =
            DependencyProperty.Register(nameof(PhaseArcColor), typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.Yellow, OnPhaseArcColorChanged));
#else
        public static readonly BindableProperty PhaseArcColorProperty =
            BindableProperty.Create(nameof(PhaseArcColor), typeof(Color), typeof(SunPhaseView), Colors.Yellow, propertyChanged: OnPhaseArcColorChanged);
#endif

        public Color BottomTextColor
        {
            get => (Color)GetValue(BottomTextColorProperty);
            set => SetValue(BottomTextColorProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register(nameof(BottomTextColor), typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.White, OnBottomTextColorChanged));
#else
        public static readonly BindableProperty BottomTextColorProperty =
            BindableProperty.Create(nameof(BottomTextColor), typeof(Color), typeof(SunPhaseView), Colors.White, propertyChanged: OnBottomTextColorChanged);
#endif

        public SunPhaseViewModel ViewModel
        {
#if WINDOWS
            get => DataContext as SunPhaseViewModel;
            set => DataContext = value;
#else
            get => BindingContext as SunPhaseViewModel;
            set => BindingContext = value;
#endif
        }

        public SunPhaseView()
        {
#if WINDOWS
            this.DefaultStyleKey = typeof(SunPhaseView);
#endif

            this.Loaded += SunPhaseView_Loaded;
            this.Unloaded += SunPhaseView_Unloaded;

            var date = DateTime.Today;
            sunrise = date;
            sunset = date;

#if WINDOWS
            DispatcherQueue.EnqueueAsync(async () =>
#else
            Dispatcher.Dispatch(async () =>
#endif
            {
#if WINDOWS
                var fs = await StorageFileHelper.GetFileStreamFromApplicationUri(SunIconUri);
#else
                var fs = await FileSystemUtils.OpenAppPackageFileAsync(SunIconUri);
#endif
                SunIcon = SKImage.FromEncodedData(fs);
#if WINDOWS
                Canvas?.Invalidate();
#else
                Canvas?.InvalidateSurface();
#endif
            });

            bottomTextPaint = new SKPaint()
            {
                IsAntialias = true,
                TextSize = 14, // 14sp
                TextAlign = SKTextAlign.Center,
                Style = SKPaintStyle.Fill,
                Color = BottomTextColor.ToSKColor()
            };

            bottomTextFont = new SKFont
            {
                Edging = SKFontEdging.SubpixelAntialias,
                Size = bottomTextPaint.TextSize
            };

            backgroundPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = ColorUtils.SetAlphaComponent(PaintColor, 0x50).ToSKColor(),
            };

            fullArcPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = ColorUtils.SetAlphaComponent(PhaseArcColor, 0x40).ToSKColor(),
                StrokeWidth = 1f // 1dp
            };

            float intervalOn = 8f; // 8dp
            float intervalOff = 10f; // 10dp

            var dashEffect = SKPathEffect.CreateDash(new float[] { intervalOn, intervalOff, intervalOn, intervalOff }, 1);
            fullArcPaint.PathEffect = dashEffect;

            pathArcPaint = fullArcPaint.Clone();
            pathArcPaint.StrokeWidth = 2f; // 2dp
            pathArcPaint.PathEffect = null;
            pathArcPaint.Color = PaintColor.ToSKColor();

            bigCirPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = PaintColor.ToSKColor()
            };

            // UpdateColors();

#if WINDOWS
            RegisterPropertyChangedCallback(FontSizeProperty, OnDependencyPropertyChanged);
            RegisterPropertyChangedCallback(FontWeightProperty, OnDependencyPropertyChanged);
            this.DataContextChanged += SunPhaseView_DataContextChanged;
#else
            this.HandlerChanged += SunPhaseView_HandlerChanged;
            this.BindingContextChanged += SunPhaseView_BindingContextChanged;
#endif

            IsInitialized = true;
        }

#if !WINDOWS
        private void SunPhaseView_HandlerChanged(object sender, EventArgs e)
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
        private static void OnPaintColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
#else
        private static void OnPaintColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue != oldValue)
#endif
            {
                (obj as SunPhaseView)?.UpdatePaintColor();
            }
        }

        private void UpdatePaintColor()
        {
            if (!IsInitialized) return;

            backgroundPaint.Color = ColorUtils.SetAlphaComponent(PaintColor, 0x50).ToSKColor();
            pathArcPaint.Color = PaintColor.ToSKColor();
            bigCirPaint.Color = PaintColor.ToSKColor();
#if WINDOWS
            Canvas?.Invalidate();
#else
            Canvas?.InvalidateSurface();
#endif
        }

#if WINDOWS
        private static void OnPhaseArcColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
#else
        private static void OnPhaseArcColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue != oldValue)
#endif
            {
                (obj as SunPhaseView)?.UpdatePhaseArcColor();
            }
        }

        private void UpdatePhaseArcColor()
        {
            if (!IsInitialized) return;

            fullArcPaint.Color = ColorUtils.SetAlphaComponent(PhaseArcColor, 0x40).ToSKColor();
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
                (obj as SunPhaseView)?.UpdateBottomTextColor();
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
        private void SunPhaseView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
#else
        private void SunPhaseView_BindingContextChanged(object sender, EventArgs args)
#endif
        {
#if WINDOWS
            if (args.NewValue is SunPhaseViewModel viewModel)
#else
            if (BindingContext is SunPhaseViewModel viewModel)
#endif
            {
                SetSunriseSetTimes(viewModel.SunriseTime, viewModel.SunsetTime, viewModel.TZOffset);
            }
        }

        private void SunPhaseView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.PaintSurface += Canvas_PaintSurface;
            }
        }

        private void SunPhaseView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.PaintSurface -= Canvas_PaintSurface;
            }
        }

#if WINDOWS
        private SKTypeface GetSKTypeface(FontFamily fontFamily, FontWeight fontWeight)
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
        private SKTypeface GetSKTypeface(string fontFamily, FontAttributes fontAttribs)
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

        private float FontSizeToTextSize(double fontSize)
        {
            return (float)fontSize /** (1f / 0.75f)*/;
        }

#if WINDOWS
        private void OnDependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
        {
            if (property == FontSizeProperty)
            {
                this.bottomTextPaint.TextSize = FontSizeToTextSize(FontSize);
                CalculateBottomTextSize();
                Canvas?.Invalidate();
            }
            else if (property == FontWeightProperty)
            {
                this.bottomTextPaint.Typeface = GetSKTypeface(FontFamily, FontWeight);
                CalculateBottomTextSize();
                Canvas?.Invalidate();
            }
        }
#endif

        private float GraphHeight =>
            ViewHeight - bottomTextTopMargin - bottomTextHeight - bottomTextDescent;

        private String SunriseLabel
        {
            get
            {
                var culture = LocaleUtils.GetLocale();
                return sunrise.ToString("t", culture);
            }
        }

        private String SunsetLabel
        {
            get
            {
                var culture = LocaleUtils.GetLocale();
                return sunset.ToString("t", culture);
            }
        }

        public void SetSunriseSetTimes(TimeSpan sunrise, TimeSpan sunset, TimeSpan offset = default)
        {
            var date = DateTime.UtcNow.Add(offset).Date;
            SetSunriseSetTimes(date.Add(sunrise), date.Add(sunset), offset);
        }

        public void SetSunriseSetTimes(DateTime sunrise, DateTime sunset, TimeSpan offset = default)
        {
            if (!Equals(this.sunrise, sunrise) || !Equals(this.sunset, sunset) || !Equals(this.offset, offset))
            {
                this.sunrise = sunrise;
                this.sunset = sunset;
                this.offset = offset;

                CalculateBottomTextSize();
#if WINDOWS
                Canvas?.Invalidate();
#else
                Canvas?.InvalidateSurface();
#endif
            }
        }

        private void CalculateBottomTextSize()
        {
            SKRect r = new();
            float longestWidth = 0;
            string longestStr = "";
            bottomTextDescent = 0;
            foreach (var s in ImmutableList.Create(SunriseLabel, SunsetLabel))
            {
                bottomTextPaint.MeasureText(s, ref r);
                if (bottomTextHeight < r.Height)
                {
                    bottomTextHeight = r.Height;
                }
                if (longestWidth < r.Width)
                {
                    longestWidth = r.Width;
                    longestStr = s;
                }
                if (bottomTextDescent < (Math.Abs(r.Bottom)))
                {
                    bottomTextDescent = Math.Abs(r.Bottom);
                }
            }

            if (backgroundGridWidth < longestWidth)
            {
                backgroundGridWidth = longestWidth + bottomTextPaint.MeasureText(longestStr.Substring(0, 1));
            }
            if (sideLineLength < longestWidth / 2f)
            {
                sideLineLength = longestWidth / 2f;
            }

            RefreshXCoordinateList();
#if WINDOWS
            Canvas?.Invalidate();
#else
            Canvas?.InvalidateSurface();
#endif
        }

        private void RefreshXCoordinateList()
        {
            sunriseX = sideLineLength;
            sunsetX = ViewWidth - sideLineLength;
        }

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            // get the screen density for scaling
#if WINDOWS
            var scale = (float)XamlRoot.RasterizationScale;
#else
            var scale = 1f;
#endif

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(SKColors.Transparent);

            OnDraw(canvas);
        }

        private void OnDraw(SKCanvas canvas)
        {
            DrawLabels(canvas);
            DrawArc(canvas);
            DrawDots(canvas);
        }

        private void DrawDots(SKCanvas canvas)
        {
            canvas.DrawCircle(sunriseX, GraphHeight, DOT_RADIUS, bigCirPaint);
            canvas.DrawCircle(sunsetX, GraphHeight, DOT_RADIUS, bigCirPaint);
        }

        private void DrawArc(SKCanvas canvas)
        {
            float radius = GraphHeight * 0.9f;
            float trueRadius = (sunsetX - sunriseX) * 0.5f;

            float x, y;
            float centerX = ViewWidth * 0.5f;
            float centerY = GraphHeight;

            /*
                Point on circle (width = height = r)
                x(t) = r cos(t) + j
                y(t) = r sin(t) + k
                Point on ellipse
                int ePX = X + (int) (width  * Math.cos(Math.toRadians(t)));
                int ePY = Y + (int) (height * Math.sin(Math.toRadians(t)));
            */

            bool isDay = false;
            int angle = 0;

            {
                DateTime now = DateTime.UtcNow.Add(offset);

                if (now.CompareTo(sunrise) < 0)
                {
                    angle = 0;
                    isDay = false;
                }
                else if (now.CompareTo(sunset) > 0)
                {
                    angle = 180;
                    isDay = false;
                }
                else if (now.CompareTo(sunrise) > 0)
                {
                    double sunUpDuration = ((sunset - sunrise).TotalSeconds / 60);
                    double minsAfterSunrise = ((now - sunrise).TotalSeconds / 60);

                    angle = (int)((minsAfterSunrise / sunUpDuration) * 180);
                    isDay = true;
                }
            }

            x = (float)(trueRadius * Math.Cos(ConversionMethods.ToRadians(angle - 180))) + centerX;
            y = (float)(radius * Math.Sin(ConversionMethods.ToRadians(angle - 180))) + centerY;

            using var mPathBackground = new SKPath();
            float firstX = -1;
            float firstY = -1;
            // needed to end the path for background
            float lastUsedEndX = 0;
            float lastUsedEndY = 0;

            // Draw Arc
            for (int i = 0; i < angle; i++)
            {
                float startX = (float)(trueRadius * Math.Cos(ConversionMethods.ToRadians(i - 180))) + centerX;
                float startY = (float)(radius * Math.Sin(ConversionMethods.ToRadians(i - 180))) + centerY;
                float endX = (float)(trueRadius * Math.Cos(ConversionMethods.ToRadians((i + 1) - 180))) + centerX;
                float endY = (float)(radius * Math.Sin(ConversionMethods.ToRadians((i + 1) - 180))) + centerY;

                if (firstX == -1)
                {
                    firstX = sunriseX;
                    firstY = GraphHeight;
                    mPathBackground.MoveTo(firstX, firstY);
                }

                mPathBackground.LineTo(startX, startY);
                mPathBackground.LineTo(endX, endY);

                lastUsedEndX = endX;
                lastUsedEndY = endY;
            }

            var oval = new SKRect(sunriseX, GraphHeight - radius, sunsetX, GraphHeight + radius);
            canvas.DrawArc(oval, -180f, 180f, true, fullArcPaint);

            if (firstX != -1)
            {
                // end / close path
                if (lastUsedEndY != GraphHeight)
                {
                    // dont draw line to same point, otherwise the path is completely broken
                    mPathBackground.LineTo(lastUsedEndX, GraphHeight);
                }
                mPathBackground.LineTo(firstX, GraphHeight);
                if (firstY != GraphHeight)
                {
                    // dont draw line to same point, otherwise the path is completely broken
                    mPathBackground.LineTo(firstX, firstY);
                }

                canvas.DrawPath(mPathBackground, backgroundPaint);
                canvas.DrawArc(oval, -180f, angle, false, pathArcPaint);
            }

            if (isDay && SunIcon != null)
            {
                var iconSize = 28f; // 28dp // old: 24dp
                var iconBounds = new SKRect(0, 0, iconSize, iconSize);

                canvas.Save();
                canvas.Translate(x - iconSize / 2f, y - iconSize / 2f);
                using var iconPaint = new SKPaint()
                {
                    ColorFilter = SKColorFilter.CreateBlendMode(PaintColor.ToSKColor(), SKBlendMode.SrcIn),
                };
                canvas.DrawImage(SunIcon, iconBounds, iconPaint);
                canvas.Restore();
            }
        }

        private void DrawLabels(SKCanvas canvas)
        {
            // Draw bottom text
            float y = ViewHeight - bottomTextDescent;
            canvas.DrawText(SunriseLabel, sunriseX, y, bottomTextFont, bottomTextPaint);
            canvas.DrawText(SunsetLabel, sunsetX, y, bottomTextFont, bottomTextPaint);
        }

#if WINDOWS
        protected sealed override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);

            Canvas.Width = availableSize.Width;
            Canvas.Height = availableSize.Height;

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;
#else
        protected sealed override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            Size availableSize = base.MeasureOverride(widthConstraint, heightConstraint);
            availableSize.Width = MeasureWidth(widthConstraint, availableSize.Width);

            if (Canvas != null)
            {
                Canvas.WidthRequest = availableSize.Width;
                Canvas.HeightRequest = availableSize.Height;

                ViewHeight = (float)Canvas.Height;
                ViewWidth = (float)Canvas.Width;
            }
#endif

            RefreshXCoordinateList();

            // Redraw View
#if WINDOWS
            Canvas?.Invalidate();
#else
            Canvas?.InvalidateSurface();
#endif

            return availableSize;
        }

        private double MeasureWidth(double widthConstraint, double measuredWidth)
        {
            int MIN_HORIZONTAL_GRID_NUM = 2;
            double preferred = backgroundGridWidth * MIN_HORIZONTAL_GRID_NUM + sideLineLength * 2;
            return LayoutManager.ResolveConstraints(widthConstraint, this.Width, measuredWidth, min: preferred);
        }

        public void Dispose()
        {
            backgroundPaint?.Dispose();
            fullArcPaint?.Dispose();
            pathArcPaint?.Dispose();
            bottomTextPaint?.Dispose();
            bigCirPaint?.Dispose();
        }
    }
}
