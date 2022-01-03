using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.UWP.Controls
{
    [TemplatePart(Name = nameof(Canvas), Type = typeof(CanvasVirtualControl))]
    public sealed class SunPhaseView : Control, IDisposable
    {
        private const string SunIconUri = "ms-appx:///Assets/FullSun.png";

        private CanvasVirtualControl Canvas { get; set; }

        private float ViewHeight;
        private float ViewWidth;
        private float bottomTextHeight;

        private float bottomTextDescent;

        private DateTime sunrise;
        private DateTime sunset;
        private TimeSpan offset = TimeSpan.Zero;

        private float sunriseX;
        private float sunsetX;

        private float bottomTextTopMargin;
        private float DOT_RADIUS;

        private float sideLineLength;
        private float backgroundGridWidth;

        private readonly CanvasTextFormat BottomTextFormat;

        private float IconSize;
        private CanvasBitmap SunIcon;

        public bool ReadyToDraw => Canvas?.ReadyToDraw ?? false;

        public Color PaintColor
        {
            get { return (Color)GetValue(PaintColorProperty); }
            set { SetValue(PaintColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PaintColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PaintColorProperty =
            DependencyProperty.Register("PaintColor", typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.Yellow));

        public Color PhaseArcColor
        {
            get { return (Color)GetValue(PhaseArcColorProperty); }
            set { SetValue(PhaseArcColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PhaseArcColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhaseArcColorProperty =
            DependencyProperty.Register("PhaseArcColor", typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.Yellow));

        public Color BottomTextColor
        {
            get { return (Color)GetValue(BottomTextColorProperty); }
            set { SetValue(BottomTextColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register("BottomTextColor", typeof(Color), typeof(SunPhaseView), new PropertyMetadata(Colors.White));

        public SunPhaseView()
        {
            this.DefaultStyleKey = typeof(SunPhaseView);

            this.Loaded += SunPhaseView_Loaded;
            this.Unloaded += SunPhaseView_Unloaded;

            var date = DateTime.Today;
            sunrise = date.AddHours(7);
            sunset = date.AddHours(19);

            BottomTextFormat = new CanvasTextFormat
            {
                FontSize = (float)FontSize,
                FontWeight = FontWeight,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            RegisterPropertyChangedCallback(FontSizeProperty, OnDependencyPropertyChanged);
            RegisterPropertyChangedCallback(FontWeightProperty, OnDependencyPropertyChanged);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Canvas = GetTemplateChild(nameof(Canvas)) as CanvasVirtualControl;

            Canvas.CreateResources += Canvas_CreateResources;
            Canvas.RegionsInvalidated += Canvas_RegionsInvalidated;
        }

        private void SunPhaseView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.CreateResources += Canvas_CreateResources;
                Canvas.RegionsInvalidated += Canvas_RegionsInvalidated;
            }
        }

        private void SunPhaseView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.CreateResources -= Canvas_CreateResources;
                Canvas.RegionsInvalidated -= Canvas_RegionsInvalidated;
            }
        }

        private void OnDependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
        {
            if (property == FontSizeProperty)
            {
                this.BottomTextFormat.FontSize = (float)FontSize;
                if (ReadyToDraw)
                {
                    CalculateBottomTextSize();
                    Canvas.Invalidate();
                }
            }
            else if (property == FontWeightProperty)
            {
                this.BottomTextFormat.FontWeight = FontWeight;
                if (ReadyToDraw)
                {
                    CalculateBottomTextSize();
                    Canvas.Invalidate();
                }
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
            var date = DateTime.Today;
            SetSunriseSetTimes(date.Add(sunrise), date.Add(sunset), offset);
        }

        public void SetSunriseSetTimes(DateTime sunrise, DateTime sunset, TimeSpan offset = default)
        {
            if (!Equals(this.sunrise, sunrise) || !Equals(this.sunset, sunset) || !Equals(this.offset, offset))
            {
                this.sunrise = sunrise;
                this.sunset = sunset;
                this.offset = offset;

                if (ReadyToDraw)
                {
                    CalculateBottomTextSize();
                    Canvas.Invalidate();
                }
            }
        }

        private void CalculateBottomTextSize()
        {
            double longestWidth = 0;
            String longestStr = "";
            bottomTextDescent = 0;
            foreach (var s in ImmutableList.Create(SunriseLabel, SunsetLabel))
            {
                using var textLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0);
                if (bottomTextHeight < textLayout.DrawBounds.Height)
                {
                    bottomTextHeight = (float)textLayout.DrawBounds.Height;
                }
                if (longestWidth < textLayout.DrawBounds.Width)
                {
                    longestWidth = textLayout.DrawBounds.Width;
                    longestStr = s;
                }
                if (bottomTextDescent < (Math.Abs(textLayout.DrawBounds.Bottom)))
                {
                    bottomTextDescent = Math.Abs((float)textLayout.DrawBounds.Bottom);
                }
            }

            if (backgroundGridWidth < longestWidth)
            {
                using var textLayout = new CanvasTextLayout(Canvas, longestStr.Substring(0, 1), BottomTextFormat, 0, 0);
                backgroundGridWidth = (float)longestWidth + (float)textLayout.DrawBounds.Width;
            }
            if (sideLineLength < longestWidth / 2f)
            {
                sideLineLength = (float)longestWidth / 2f;
            }

            RefreshXCoordinateList();
        }

        private void RefreshXCoordinateList()
        {
            sunriseX = sideLineLength;
            sunsetX = ViewWidth - sideLineLength;
        }

        private void Canvas_CreateResources(CanvasVirtualControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            IconSize = sender.ConvertDipsToPixels(28, CanvasDpiRounding.Floor);
            CanvasBitmap.LoadAsync(sender, new Uri(SunIconUri)).AsTask().ContinueWith((t) =>
            {
                if (t.IsCompletedSuccessfully && t.Result != null)
                {
                    SunIcon = t.Result;
                }
            });

            bottomTextTopMargin = sender.ConvertDipsToPixels(8, CanvasDpiRounding.Floor);
            DOT_RADIUS = sender.ConvertDipsToPixels(4, CanvasDpiRounding.Floor);

            sideLineLength = sender.ConvertDipsToPixels(45, CanvasDpiRounding.Floor) / 3 * 2;
            backgroundGridWidth = sender.ConvertDipsToPixels(45, CanvasDpiRounding.Floor);

            CalculateBottomTextSize();
        }

        private void Canvas_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            // Draw the effect to whatever regions of the CanvasVirtualControl have been invalidated.
            foreach (var region in args.InvalidatedRegions)
            {
                using var drawingSession = sender.CreateDrawingSession(region);
                DrawLabels(region, drawingSession);
                DrawArc(region, drawingSession);
                DrawDots(region, drawingSession);
            }
        }

        private void DrawDots(Rect region, CanvasDrawingSession drawingSession)
        {
            // Draw Dots
            if (RectHelper.Contains(region, new Point(sunriseX, GraphHeight)))
            {
                drawingSession.FillCircle(sunriseX, GraphHeight, DOT_RADIUS, PaintColor);
            }

            if (RectHelper.Contains(region, new Point(sunsetX, GraphHeight)))
            {
                drawingSession.FillCircle(sunsetX, GraphHeight, DOT_RADIUS, PaintColor);
            }
        }

        private void DrawArc(Rect region, CanvasDrawingSession drawingSession)
        {
            float radius = GraphHeight * 0.9f;
            float trueRadius = (sunsetX - sunriseX) * 0.5f;

            float x, y;
            float centerX = ViewWidth / 2f;
            float centerY = GraphHeight;

            float intervalOn = drawingSession.ConvertDipsToPixels(8, CanvasDpiRounding.Floor);
            float intervalOff = drawingSession.ConvertDipsToPixels(10, CanvasDpiRounding.Floor);

            using var arcStroke = new CanvasStrokeStyle()
            {
                CustomDashStyle = new float[] { intervalOn, intervalOff, intervalOn, intervalOff },
                DashOffset = 1,
                LineJoin = CanvasLineJoin.Round
            };
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
                    int sunUpDuration = (int)((sunset - sunrise).TotalSeconds / 60);
                    int minsAfterSunrise = (int)((now - sunrise).TotalSeconds / 60);

                    angle = (int)(((float)minsAfterSunrise / sunUpDuration) * 180);
                    isDay = true;
                }
            }

            x = (float)(trueRadius * Math.Cos(ConversionMethods.ToRadians(angle - 180))) + centerX;
            y = (float)(radius * Math.Sin(ConversionMethods.ToRadians(angle - 180))) + centerY;

            using var mPathBackground = new CanvasPathBuilder(drawingSession);
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

                if (RectHelper.Contains(region, new Point(startX, startY)) ||
                    RectHelper.Contains(region, new Point(endX, endY)))
                {
                    if (firstX == -1)
                    {
                        firstX = startX;
                        firstY = startY;
                        mPathBackground.BeginFigure(startX, startY);
                    }

                    mPathBackground.AddLine(startX, startY);
                    mPathBackground.AddLine(endX, endY);

                    lastUsedEndX = endX;
                    lastUsedEndY = endY;
                }
            }

            using var FullArcPath = new CanvasPathBuilder(drawingSession);
            FullArcPath.BeginFigure(sunriseX, GraphHeight);
            FullArcPath.AddArc(new System.Numerics.Vector2(centerX, GraphHeight), trueRadius, radius, ConversionMethods.ToRadians(-180), ConversionMethods.ToRadians(180));
            FullArcPath.EndFigure(CanvasFigureLoop.Closed);

            using var fullArc = CanvasGeometry.CreatePath(FullArcPath);
            drawingSession.DrawGeometry(fullArc, PhaseArcColor, 1.0f, arcStroke);

            if (firstX != -1)
            {
                // end / close path
                if (lastUsedEndY != GraphHeight)
                {
                    // dont draw line to same point, otherwise the path is completely broken
                    mPathBackground.AddLine(lastUsedEndX, GraphHeight);
                }
                mPathBackground.AddLine(firstX, GraphHeight);
                if (firstY != GraphHeight)
                {
                    // dont draw line to same point, otherwise the path is completely broken
                    mPathBackground.AddLine(firstX, firstY);
                }
                mPathBackground.EndFigure(CanvasFigureLoop.Closed);
                using (var arc = CanvasGeometry.CreatePath(mPathBackground))
                {
                    drawingSession.FillGeometry(arc, ColorUtils.SetAlphaComponent(PaintColor, 0x50));
                }

                using var PathArcPath = new CanvasPathBuilder(drawingSession);
                PathArcPath.BeginFigure(sunriseX, GraphHeight);
                PathArcPath.AddArc(new System.Numerics.Vector2(centerX, GraphHeight), trueRadius, radius, ConversionMethods.ToRadians(-180), ConversionMethods.ToRadians(angle));
                PathArcPath.EndFigure(CanvasFigureLoop.Open);

                using var pathArc = CanvasGeometry.CreatePath(PathArcPath);
                drawingSession.DrawGeometry(pathArc, PaintColor, 2.0f);
            }

            if (isDay)
            {
                var iconRect = new Rect(x - IconSize / 2, y - IconSize / 2, IconSize, IconSize);

                if (SunIcon == null)
                {
                    CanvasBitmap.LoadAsync(Canvas, new Uri(SunIconUri)).AsTask().ContinueWith((t) =>
                    {
                        if (t.IsCompletedSuccessfully && t.Result != null)
                        {
                            SunIcon = t.Result;
                            Dispatcher.RunOnUIThread(() => Canvas.Invalidate(iconRect));
                        }
                    });

                    return;
                }

                drawingSession.DrawImage(new TintEffect()
                {
                    Source = SunIcon,
                    Color = PaintColor
                }, iconRect, SunIcon.Bounds);
            }
        }

        private void DrawLabels(Rect region, CanvasDrawingSession drawingSession)
        {
            // Draw bottom text
            float y = ViewHeight - bottomTextDescent;

            using var sunriseTxtLayout = new CanvasTextLayout(drawingSession, SunriseLabel, BottomTextFormat, 0, 0);
            using var sunsetTxtLayout = new CanvasTextLayout(drawingSession, SunsetLabel, BottomTextFormat, 0, 0);

            if (sunriseTxtLayout.DrawBounds.Intersects(region))
                drawingSession.DrawTextLayout(sunriseTxtLayout, sunriseX, y, BottomTextColor);
            if (sunsetTxtLayout.DrawBounds.Intersects(region))
                drawingSession.DrawTextLayout(sunsetTxtLayout, sunsetX, y, BottomTextColor);
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

        public void Dispose()
        {
            BottomTextFormat?.Dispose();
        }
    }
}
