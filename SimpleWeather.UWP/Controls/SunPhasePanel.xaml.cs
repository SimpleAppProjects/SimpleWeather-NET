using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class SunPhasePanel : UserControl
    {
        private double ViewHeight;
        private double ViewWidth;
        private double bottomTextHeight = 0;

        //private Paint bottomTextPaint;
        private double bottomTextDescent;

        private TimeSpan sunrise = new TimeSpan(7, 0, 0);
        private TimeSpan sunset = new TimeSpan(19, 0, 0);
        private TimeSpan offset = TimeSpan.Zero;

        private double sunriseX;
        private double sunsetX;

        private const double bottomTextTopMargin = 5;
        private const double DOT_RADIUS = 4;

        private double sideLineLength = 45 / 3 * 2;
        private double backgroundGridWidth = 45;

        private Thickness ViewPadding = new Thickness(20, 20, 20, 20);

        private Color BottomTextColor = Colors.White;
        private double BottomTextSize = 12;
        private TextAlignment BottomTextAlignment = TextAlignment.Center;

        public SunPhasePanel()
        {
            this.InitializeComponent();
            this.SizeChanged += SunPhasePanel_SizeChanged;
            this.LayoutUpdated += SunPhasePanel_LayoutUpdated;

            // Properties are bound in XAML
            SunriseText.Foreground = new SolidColorBrush(BottomTextColor);
            SunriseText.FontSize = BottomTextSize;
            SunriseText.TextAlignment = BottomTextAlignment;

            ArcLine.Stroke = new SolidColorBrush(Colors.White);
            ArcLine.StrokeDashArray = new DoubleCollection() { 4, 2, 4, 2 };
            ArcLine.StrokeDashOffset = 1;
            ArcLine.StrokeLineJoin = PenLineJoin.Round;
            ArcLine.Fill = new SolidColorBrush(Color.FromArgb(0x50, 0xFF, 0xFF, 0x00));

            SetSunriseSetTimes(sunrise, sunset);
        }

        private double GraphHeight =>
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

        public void SetSunriseSetTimes(TimeSpan sunrise, TimeSpan sunset, TimeSpan offset)
        {
            this.sunrise = sunrise;
            this.sunset = sunset;
            this.offset = offset;

            SunriseText.Text = SunriseLabel;
            SunsetText.Text = SunsetLabel;

            double longestWidth = 0;
            String longestStr = "";
            bottomTextDescent = 0;
            foreach (String s in ImmutableList.Create<String>(SunriseLabel, SunsetLabel))
            {
                var txtblk = new TextBlock() { Text = s, FontSize = BottomTextSize, TextAlignment = BottomTextAlignment };
                txtblk.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                if (bottomTextHeight < txtblk.DesiredSize.Height)
                {
                    bottomTextHeight = txtblk.DesiredSize.Height;
                }
                if (longestWidth < txtblk.DesiredSize.Width)
                {
                    longestWidth = txtblk.DesiredSize.Width;
                    longestStr = s;
                }
                if (bottomTextDescent < (Math.Abs(txtblk.DesiredSize.Height / 2)))
                {
                    bottomTextDescent = Math.Abs(txtblk.DesiredSize.Height / 2);
                }
            }

            if (backgroundGridWidth < longestWidth)
            {
                var txtblk = new TextBlock() { Text = longestStr.Substring(0, 1), FontSize = BottomTextSize, TextAlignment = BottomTextAlignment };
                txtblk.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                backgroundGridWidth = longestWidth + txtblk.DesiredSize.Width;
            }
            if (sideLineLength < longestWidth / 2)
            {
                sideLineLength = longestWidth / 2f;
            }

            RefreshXCoordinateList();
        }

        private void RefreshXCoordinateList()
        {
            sunriseX = sideLineLength;
            sunsetX = ViewWidth - sideLineLength;
        }

        private void SunPhasePanel_LayoutUpdated(object sender, object e)
        {
            Draw();
        }

        private void Draw()
        {
            float radius = (float)(GraphHeight * 0.9f);
            float trueRadius = (float)(sunsetX - sunriseX) * 0.5f;

            float x, y;
            float centerX = (float)(ViewWidth / 2f) + (float)ViewPadding.Left;
            float centerY = (float)GraphHeight;

            SunIcon.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

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

            x = (float)(trueRadius * Math.Cos(ConversionMethods.toRadians(angle - 180))) + centerX;
            y = (float)(radius * Math.Sin(ConversionMethods.toRadians(angle - 180))) + centerY;

            float firstX = -1;
            float firstY = -1;
            // needed to end the path for background
            float lastUsedEndX = 0;
            float lastUsedEndY = 0;

            // Draw Arc
            ArcLine.Points.Clear();
            for (int i = 0; i < angle; i++)
            {
                float startX = (float)(trueRadius * Math.Cos(ConversionMethods.toRadians(i - 180))) + centerX;
                float startY = (float)(radius * Math.Sin(ConversionMethods.toRadians(i - 180))) + centerY;
                float endX = (float)(trueRadius * Math.Cos(ConversionMethods.toRadians((i + 1) - 180))) + centerX;
                float endY = (float)(radius * Math.Sin(ConversionMethods.toRadians((i + 1) - 180))) + centerY;

                if (firstX == -1)
                {
                    firstX = startX;
                    firstY = startY;
                    ArcLine.Points.Add(new Point(firstX, firstY));
                }

                ArcLine.Points.Add(new Point(startX, startY));
                ArcLine.Points.Add(new Point(endX, endY));

                lastUsedEndX = endX;
                lastUsedEndY = endY;
            }

            if (firstX != -1)
            {
                // end / close path
                if (lastUsedEndY != GraphHeight)
                {
                    // dont draw line to same point, otherwise the path is completely broken
                    ArcLine.Points.Add(new Point(lastUsedEndX, GraphHeight));
                }
                ArcLine.Points.Add(new Point(firstX, GraphHeight));
                if (firstY != GraphHeight)
                {
                    // dont draw line to same point, otherwise the path is completely broken
                    ArcLine.Points.Add(new Point(firstX, firstY));
                }
            }

            // Draw Dots
            GeometryGroup dotGrp = new GeometryGroup()
            {
                Children =
                {
                    new EllipseGeometry()
                    {
                        Center = new Point(sunriseX + ViewPadding.Left, GraphHeight - DOT_RADIUS / 2),
                        RadiusX = DOT_RADIUS,
                        RadiusY = DOT_RADIUS
                    },
                    new EllipseGeometry()
                    {
                        Center = new Point(sunsetX + ViewPadding.Right, GraphHeight - DOT_RADIUS / 2),
                        RadiusX = DOT_RADIUS,
                        RadiusY = DOT_RADIUS
                    }
                }
            };
            DotsPath.Data = dotGrp;

            if (isDay)
            {
                SunIcon.Visibility = Visibility.Visible;
                Canvas.SetLeft(SunIcon, x - SunIcon.DesiredSize.Width / 2);
                Canvas.SetTop(SunIcon, y - SunIcon.DesiredSize.Height / 2);
            }
            else
            {
                SunIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void SunPhasePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.Width = e.NewSize.Width;
            Canvas.Height = e.NewSize.Height;

            ViewHeight = Canvas.Height;
            ViewWidth = Canvas.Width - (ViewPadding.Left + ViewPadding.Right);

            RefreshXCoordinateList();

            // Draw Labels
            SunriseText.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            SunsetText.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            Canvas.SetLeft(SunriseText, sunriseX);
            Canvas.SetTop(SunriseText, GraphHeight);
            Canvas.SetLeft(SunsetText, sunsetX);
            Canvas.SetTop(SunsetText, GraphHeight);
        }
    }
}
