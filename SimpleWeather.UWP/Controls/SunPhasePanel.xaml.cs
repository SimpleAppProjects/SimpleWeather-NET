using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Immutable;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class SunPhasePanel : UserControl, IDisposable
    {
        private float ViewHeight;
        private float ViewWidth;
        private float bottomTextHeight = 0;

        //private Paint bottomTextPaint;
        private float bottomTextDescent;

        private TimeSpan sunrise = new TimeSpan(7, 0, 0);
        private TimeSpan sunset = new TimeSpan(19, 0, 0);
        private TimeSpan offset = TimeSpan.Zero;

        private float sunriseX;
        private float sunsetX;

        private const float bottomTextTopMargin = 5;
        private const float DOT_RADIUS = 4;

        private float sideLineLength = 45 / 3 * 2;
        private float backgroundGridWidth = 45;

        private Thickness ViewPadding = new Thickness(20, 20, 20, 20);

        private const float BottomTextSize = 12;
        private CanvasTextFormat BottomTextFormat;

        public bool ReadyToDraw => Canvas.ReadyToDraw;

        private Color PaintColor => PaintColorBrush.Color;
        private Color PhaseArcColor => PhaseArcColorBrush.Color;
        private Color BottomTextColor => BottomTextColorBrush.Color;

        public SunPhasePanel()
        {
            this.InitializeComponent();
            this.SizeChanged += SunPhasePanel_SizeChanged;

            BottomTextFormat = new CanvasTextFormat
            {
                FontSize = BottomTextSize,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                WordWrapping = CanvasWordWrapping.NoWrap
            };
        }

        private float GraphHeight =>
            ViewHeight - bottomTextTopMargin - bottomTextHeight - bottomTextDescent;

        private String SunriseLabel
        {
            get
            {
                var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
                var culture = new System.Globalization.CultureInfo(userlang);

                return DateTime.Today.Add(sunrise).ToString("t", culture);
            }
        }

        private String SunsetLabel
        {
            get
            {
                var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
                var culture = new System.Globalization.CultureInfo(userlang);

                return DateTime.Today.Add(sunset).ToString("t", culture);
            }
        }

        public void SetSunriseSetTimes(TimeSpan sunrise, TimeSpan sunset)
        {
            SetSunriseSetTimes(sunrise, sunset, TimeSpan.Zero);
        }

        public void SetSunriseSetTimes(TimeSpan sunrise, TimeSpan sunset, TimeSpan? offset)
        {
            this.sunrise = sunrise;
            this.sunset = sunset;
            this.offset = offset ?? TimeSpan.Zero;

            double longestWidth = 0;
            String longestStr = "";
            bottomTextDescent = 0;
            foreach (String s in ImmutableList.Create<String>(SunriseLabel, SunsetLabel))
            {
                using (var textLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0))
                {
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
            }

            if (backgroundGridWidth < longestWidth)
            {
                using (var textLayout = new CanvasTextLayout(Canvas, longestStr.Substring(0, 1), BottomTextFormat, 0, 0))
                {
                    backgroundGridWidth = (float)longestWidth + (float)textLayout.DrawBounds.Width;
                }
            }
            if (sideLineLength < longestWidth / 2)
            {
                sideLineLength = (float)longestWidth / 2f;
            }

            Canvas.Invalidate();
        }

        private void RefreshXCoordinateList()
        {
            sunriseX = sideLineLength;
            sunsetX = ViewWidth - sideLineLength;
        }

        private void Canvas_CreateResources(CanvasVirtualControl sender, CanvasCreateResourcesEventArgs args)
        {
            SetSunriseSetTimes(sunrise, sunset);
        }

        private void Canvas_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            // Draw the effect to whatever regions of the CanvasVirtualControl have been invalidated.
            foreach (var region in args.InvalidatedRegions)
            {
                using (var drawingSession = sender.CreateDrawingSession(region))
                {
                    DrawLabels(region, drawingSession);
                    DrawArc(region, drawingSession);
                    DrawDots(region, drawingSession);
                }
            }
        }

        private void DrawDots(Rect region, CanvasDrawingSession drawingSession)
        {
            // Draw Dots
            if (RectHelper.Contains(region, new Point(sunriseX + (float)ViewPadding.Left, GraphHeight)))
                drawingSession.FillCircle(sunriseX + (float)ViewPadding.Left, GraphHeight, DOT_RADIUS, PaintColor);
            if (RectHelper.Contains(region, new Point(sunsetX + (float)ViewPadding.Right, GraphHeight)))
                drawingSession.FillCircle(sunsetX + (float)ViewPadding.Right, GraphHeight, DOT_RADIUS, PaintColor);
        }

        private void DrawArc(Rect region, CanvasDrawingSession drawingSession)
        {
            float radius = GraphHeight * 0.9f;
            float trueRadius = (sunsetX - sunriseX) * 0.5f;

            float x, y;
            float centerX = ViewWidth / 2f + (float)ViewPadding.Left;
            float centerY = GraphHeight;

            using (var arcStroke = new CanvasStrokeStyle()
            {
                CustomDashStyle = new float[] { 4, 2, 4, 2 },
                DashOffset = 1,
                LineJoin = CanvasLineJoin.Round
            })
            using (var iconTxtFormat = new CanvasTextFormat()
            {
                FontFamily = "ms-appx:///Assets/WeatherIcons/weathericons-regular-webfont.ttf#Weather Icons",
                FontSize = 18,
                HorizontalAlignment = CanvasHorizontalAlignment.Left
            })
            using (var iconLayout = new CanvasTextLayout(drawingSession, WeatherIcons.DAY_SUNNY, iconTxtFormat, 0, 0))
            {
                var iconBounds = iconLayout.LayoutBounds;

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
                    TimeSpan now = DateTime.UtcNow.Add(offset).TimeOfDay;

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
                        int sunUpDuration = (int)(sunset.TotalSeconds - sunrise.TotalSeconds) / 60;
                        int minsAfterSunrise = (int)(now.TotalSeconds / 60) - (int)(sunrise.TotalSeconds / 60);

                        angle = (int)(((float)minsAfterSunrise / sunUpDuration) * 180);
                        isDay = true;
                    }
                }

                x = (float)(trueRadius * Math.Cos(ConversionMethods.ToRadians(angle - 180))) + centerX;
                y = (float)(radius * Math.Sin(ConversionMethods.ToRadians(angle - 180))) + centerY;

                using (var mPathBackground = new CanvasPathBuilder(drawingSession))
                {
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
                            drawingSession.DrawGeometry(arc, PhaseArcColor, 1.0f, arcStroke);
                            drawingSession.FillGeometry(arc, ColorUtils.SetAlphaComponent(PaintColor, 0x50));
                        }

                        using (var FullArcPath = new CanvasPathBuilder(drawingSession))
                        {
                            FullArcPath.BeginFigure(sunriseX, GraphHeight);
                            FullArcPath.AddArc(new System.Numerics.Vector2(sunsetX, GraphHeight), trueRadius, radius, ConversionMethods.ToRadians(180), CanvasSweepDirection.Clockwise, CanvasArcSize.Large);
                            FullArcPath.EndFigure(CanvasFigureLoop.Closed);
                            using (var fullArc = CanvasGeometry.CreatePath(FullArcPath))
                            {
                                drawingSession.DrawGeometry(fullArc, PhaseArcColor, 1.0f, arcStroke);
                            }
                        }
                    }

                    if (isDay)
                    {
                        if (!RectHelper.Intersect(region, iconLayout.DrawBounds).IsEmpty)
                        {
                            drawingSession.DrawTextLayout(iconLayout, (float)(x - iconBounds.Width / 2), (float)(y - iconBounds.Height / 2), PaintColor);
                        }
                    }
                }
            }
        }

        private void DrawLabels(Rect region, CanvasDrawingSession drawingSession)
        {
            // Draw bottom text
            float y = ViewHeight - bottomTextDescent;
            using (var sunriseTxtLayout = new CanvasTextLayout(drawingSession, SunriseLabel, BottomTextFormat, 0, 0))
            using (var sunsetTxtLayout = new CanvasTextLayout(drawingSession, SunsetLabel, BottomTextFormat, 0, 0))
            {
                if (!RectHelper.Intersect(region, sunriseTxtLayout.DrawBounds).IsEmpty)
                    drawingSession.DrawTextLayout(sunriseTxtLayout, sunriseX + (float)ViewPadding.Left, y, BottomTextColor);
                if (!RectHelper.Intersect(region, sunsetTxtLayout.DrawBounds).IsEmpty)
                    drawingSession.DrawTextLayout(sunsetTxtLayout, sunsetX + (float)ViewPadding.Right, y, BottomTextColor);
            }
        }

        private void SunPhasePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.Width = e.NewSize.Width;
            Canvas.Height = e.NewSize.Height;

            if (e.NewSize.Width <= 640)
                ViewPadding = new Thickness(0, 20, 0, 20);
            else
                ViewPadding = new Thickness(20, 20, 20, 20);

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)(Canvas.Width - (ViewPadding.Left + ViewPadding.Right));

            RefreshXCoordinateList();

            Canvas.Invalidate();
        }

        public void Dispose()
        {
            BottomTextFormat.Dispose();
        }
    }
}