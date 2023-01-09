using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SimpleWeather.Common.Controls;
using SimpleWeather.Uno.Helpers;
using SimpleWeather.Uno.Utils;
using SimpleWeather.Utils;
using SkiaSharp;
using System;
using System.Collections.Immutable;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Windows.SKPaintSurfaceEventArgs;
using SKXamlCanvas = SkiaSharp.Views.Windows.SKXamlCanvas;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.Uno.Controls
{
    [TemplatePart(Name = nameof(Canvas), Type = typeof(SKXamlCanvas))]
    public sealed partial class SunPhaseView : Control, IDisposable
    {
        private const string SunIconUri = "ms-appx:///Assets/FullSun.png";

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
            get { return (Color)GetValue(PaintColorProperty); }
            set
            {
                SetValue(PaintColorProperty, value);
                backgroundPaint.Color = ColorUtils.SetAlphaComponent(value, 0x50).ToSKColor();
                pathArcPaint.Color = value.ToSKColor();
                bigCirPaint.Color = value.ToSKColor();
                Canvas?.Invalidate();
            }
        }

        // Using a DependencyProperty as the backing store for PaintColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PaintColorProperty =
            DependencyProperty.Register("PaintColor", typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.Yellow));

        public Color PhaseArcColor
        {
            get { return (Color)GetValue(PhaseArcColorProperty); }
            set
            {
                SetValue(PhaseArcColorProperty, value);
                fullArcPaint.Color = ColorUtils.SetAlphaComponent(value, 0x40).ToSKColor();
                Canvas?.Invalidate();
            }
        }

        // Using a DependencyProperty as the backing store for PhaseArcColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhaseArcColorProperty =
            DependencyProperty.Register("PhaseArcColor", typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.Yellow));

        public Color BottomTextColor
        {
            get { return (Color)GetValue(BottomTextColorProperty); }
            set
            {
                SetValue(BottomTextColorProperty, value);
                bottomTextPaint.Color = value.ToSKColor();
                Canvas?.Invalidate();
            }
        }

        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register("BottomTextColor", typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.White));

        public SunPhaseViewModel ViewModel
        {
            get => DataContext as SunPhaseViewModel;
            set => DataContext = value;
        }

        public SunPhaseView()
        {
            this.DefaultStyleKey = typeof(SunPhaseView);

            this.Loaded += SunPhaseView_Loaded;
            this.Unloaded += SunPhaseView_Unloaded;

            var date = DateTime.Today;
            sunrise = date;
            sunset = date;

            DispatcherQueue.EnqueueAsync(async () =>
            {
                var fs = await StorageFileHelper.GetFileStreamFromApplicationUri(SunIconUri);
                SunIcon = SKImage.FromEncodedData(fs);
                Canvas?.Invalidate();
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

            RegisterPropertyChangedCallback(FontSizeProperty, OnDependencyPropertyChanged);
            RegisterPropertyChangedCallback(FontWeightProperty, OnDependencyPropertyChanged);
            this.DataContextChanged += SunPhaseView_DataContextChanged;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Canvas = GetTemplateChild(nameof(Canvas)) as SKXamlCanvas;

            if (Canvas != null)
            {
                Canvas.PaintSurface += Canvas_PaintSurface;
            }
        }

        private void SunPhaseView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is SunPhaseViewModel viewModel)
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

        private float FontSizeToTextSize(double fontSize)
        {
            return (float)fontSize /** (1f / 0.75f)*/;
        }

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

        private float GraphHeight =>
            ViewHeight - bottomTextTopMargin - bottomTextHeight - bottomTextDescent;

        private String SunriseLabel
        {
            get
            {
                var culture = CultureUtils.UserCulture;
                return sunrise.ToString("t", culture);
            }
        }

        private String SunsetLabel
        {
            get
            {
                var culture = CultureUtils.UserCulture;
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
                Canvas?.Invalidate();
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
            Canvas?.Invalidate();
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
#elif HAS_UNO
            var display = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            var scale = display.LogicalDpi / 96.0f;
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

        protected override Size MeasureOverride(Size availableSize)
        {
            Canvas.Width = availableSize.Width;
            Canvas.Height = availableSize.Height;

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;

            RefreshXCoordinateList();

            Canvas.Invalidate();

            return availableSize;
        }

        public
#if HAS_UNO
            new
#endif
            void Dispose()
        {
#if HAS_UNO
            base.Dispose();
#endif
            backgroundPaint?.Dispose();
            fullArcPaint?.Dispose();
            pathArcPaint?.Dispose();
            bottomTextPaint?.Dispose();
            bigCirPaint?.Dispose();
        }
    }
}
