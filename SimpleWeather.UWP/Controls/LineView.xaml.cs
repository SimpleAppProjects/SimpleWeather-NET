using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    /*
     *  Single series line graph
     *  Based on Android implementation of LineView: https://github.com/bryan2894-playgrnd/SimpleWeather-Android
     *  Which is:
     *  Based on LineView from http://www.androidtrainee.com/draw-android-line-chart-with-animation/
     *  Graph background (under line) based on - https://github.com/jjoe64/GraphView (LineGraphSeries)
     */
    public sealed partial class LineView : UserControl
    {
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming have caused the view
        //     to change.
        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged
        {
            add
            {
                ScrollViewer.ViewChanged += value;
            }
            remove
            {
                ScrollViewer.ViewChanged -= value;
            }
        }
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming cause the view to change.
        public event EventHandler<ScrollViewerViewChangingEventArgs> ViewChanging
        {
            add
            {
                ScrollViewer.ViewChanging += value;
            }
            remove
            {
                ScrollViewer.ViewChanging -= value;
            }
        }

        public ScrollViewer ScrollViewer { get { return this.InternalScrollViewer; } }

        private float ViewHeight;
        private float ViewWidth;
        private int DataOfAGrid = 10;
        private float bottomTextHeight = 0;

        private List<KeyValuePair<String, String>> dataLabels; // X, Y Labels
        private List<KeyValuePair<String, int>> iconLabels; // X-axis icon Labels

        private List<List<float>> dataLists; // Y data

        private List<float> xCoordinateList;
        private List<float> yCoordinateList;

        private List<List<Dot>> drawDotLists; // Y data

        //private Paint bottomTextPaint;
        private float bottomTextDescent;

        private const float iconBottomMargin = 8;
        private const float bottomTextTopMargin = 5;
        private const float bottomLineLength = 25;
        private const float DOT_INNER_CIR_RADIUS = 2;
        private const float DOT_OUTER_CIR_RADIUS = 4;
        private const float MIN_TOP_LINE_LENGTH = 12;
        private const int MIN_VERTICAL_GRID_NUM = 4;
        private const int MIN_HORIZONTAL_GRID_NUM = 1;
        private readonly Color BackgroundLineColor = Colors.WhiteSmoke;
        private readonly Color BottomTextColor = Colors.White;

        private float topLineLength = MIN_TOP_LINE_LENGTH;
        private float sideLineLength = 45 / 3 * 2;
        private float backgroundGridWidth = 45;

        private Color[] colorArray = { /*Colors.SIMPLEBLUE*/Color.FromArgb(0xFF, 0x00, 0x70, 0xc0), Colors.Red, Colors.LightSeaGreen };

        public bool DrawGridLines { get; set; }
        public bool DrawDotLine { get; set; }
        public bool DrawDotPoints { get; set; }
        public bool DrawGraphBackground { get; set; }
        public bool DrawDataLabels { get; set; }
        public bool DrawIconsLabels { get; set; }

        private Thickness ViewPadding = new Thickness(20, 20, 20, 20);

        private const float BottomTextSize = 12;
        private CanvasTextFormat BottomTextFormat;

        private DispatchedHandler animator;
        public bool ReadyToDraw => Canvas.ReadyToDraw;

        public LineView()
        {
            this.InitializeComponent();

            dataLabels = new List<KeyValuePair<string, string>>();
            iconLabels = new List<KeyValuePair<string, int>>();
            xCoordinateList = new List<float>();
            yCoordinateList = new List<float>();
            drawDotLists = new List<List<Dot>>();

            BottomTextFormat = new CanvasTextFormat
            {
                FontSize = BottomTextSize,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            animator = new DispatchedHandler(async () =>
            {
                bool needNewFrame = false;
                foreach (List<Dot> data in drawDotLists)
                {
                    foreach (Dot dot in data)
                    {
                        dot.Update();
                        if (!dot.IsAtRest())
                        {
                            needNewFrame = true;
                        }
                    }
                }
                if (needNewFrame)
                {
                    await Task.Delay(25).ContinueWith(async (t) =>
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, animator);
                    });
                }
                Canvas.Invalidate();
            });
        }

        private bool IsDrawIcons => DrawIconsLabels && iconLabels != null & iconLabels.Count > 0;

        private int ADJ
        {
            get
            {
                int adj = 1;
                if (DrawIconsLabels) adj = 2; // Make space for icon labels

                return adj;
            }
        }

        private float GraphHeight
        {
            get
            {
                float height = ViewHeight - bottomTextTopMargin - bottomTextHeight - bottomLineLength - bottomTextDescent - topLineLength;
                if (DrawIconsLabels) height -= iconBottomMargin;

                return height;
            }
        }

        public void SetDataLabels(List<KeyValuePair<String, String>> dataLabels)
        {
            SetDataLabels(null, dataLabels);
        }

        public void SetDataLabels(List<KeyValuePair<String, int>> iconLabels, List<KeyValuePair<String, String>> dataLabels)
        {
            this.iconLabels = iconLabels;
            this.dataLabels = dataLabels;

            if (!DrawIconsLabels && iconLabels != null && iconLabels.Count > 0)
                DrawIconsLabels = true;
            else if (DrawIconsLabels && (iconLabels == null || iconLabels.Count <= 0))
                DrawIconsLabels = false;

            double longestWidth = 0;
            String longestStr = "";
            bottomTextDescent = 0;
            foreach (KeyValuePair<String, String> p in dataLabels)
            {
                String s = p.Key;
                String s2 = p.Value;
                var txtLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0);
                var txtLayout2 = new CanvasTextLayout(Canvas, s2, BottomTextFormat, 0, 0);

                if (bottomTextHeight < txtLayout.DrawBounds.Height)
                {
                    bottomTextHeight = (float)txtLayout.DrawBounds.Height;
                }
                if (bottomTextHeight < txtLayout2.DrawBounds.Height)
                {
                    bottomTextHeight = (float)txtLayout2.DrawBounds.Height;
                }
                if (longestWidth < txtLayout.DrawBounds.Width)
                {
                    longestWidth = txtLayout.DrawBounds.Width;
                    longestStr = s;
                }
                if (longestWidth < txtLayout2.DrawBounds.Width)
                {
                    longestWidth = txtLayout2.DrawBounds.Width;
                    longestStr = s2;
                }
                if (bottomTextDescent < (Math.Abs(txtLayout.DrawBounds.Bottom)))
                {
                    bottomTextDescent = Math.Abs((float)txtLayout.DrawBounds.Bottom);
                }
                if (bottomTextDescent < (Math.Abs(txtLayout2.DrawBounds.Bottom)))
                {
                    bottomTextDescent = Math.Abs((float)txtLayout2.DrawBounds.Bottom);
                }
            }

            if (backgroundGridWidth < longestWidth)
            {
                var textLayout = new CanvasTextLayout(Canvas, longestStr.Substring(0, 1), BottomTextFormat, 0, 0);
                backgroundGridWidth = (float)longestWidth + (float)textLayout.DrawBounds.Width;
            }
            if (sideLineLength < longestWidth / 2)
            {
                sideLineLength = (float)longestWidth / 2f;
            }

            RefreshXCoordinateList(HorizontalGridNum);
        }

        public void SetDataList(List<List<float>> dataLists)
        {
            this.dataLists = dataLists;
            foreach (List<float> list in dataLists)
            {
                if (list.Count > dataLabels.Count)
                {
                    throw new Exception("LineView error: dataList.Count > bottomTextList.Count !!!");
                }
            }
            float biggestData = 0;
            foreach (List<float> list in dataLists)
            {
                foreach (float i in list)
                {
                    if (biggestData < i)
                        biggestData = i;
                }
                DataOfAGrid = 1;
                while (biggestData / 10 > DataOfAGrid)
                    DataOfAGrid *= 10;
            }

            RefreshAfterDataChanged();
            InvalidateMeasure();
            Canvas.Invalidate();
        }

        private void RefreshAfterDataChanged()
        {
            float verticalGridNum = VerticalGridNum;
            RefreshTopLineLength(verticalGridNum);
            RefreshYCoordinateList(verticalGridNum);
            RefreshDrawDotList(verticalGridNum);
        }

        private float VerticalGridNum
        {
            get
            {
                float verticalGridNum = MIN_VERTICAL_GRID_NUM;
                if (dataLists != null && dataLists.Count != 0)
                {
                    foreach (List<float> list in dataLists)
                    {
                        foreach (float number in list)
                        {
                            if (verticalGridNum < (number + 1))
                            {
                                verticalGridNum = number + 1;
                            }
                        }
                    }
                }
                return verticalGridNum;
            }
        }

        private int HorizontalGridNum
        {
            get
            {
                int horizontalGridNum = dataLabels.Count - 1;
                if (horizontalGridNum < MIN_HORIZONTAL_GRID_NUM)
                {
                    horizontalGridNum = MIN_HORIZONTAL_GRID_NUM;
                }
                return horizontalGridNum;
            }
        }

        private void RefreshXCoordinateList(float horizontalGridNum)
        {
            xCoordinateList.Clear();
            for (int i = 0; i < (horizontalGridNum + 1); i++)
            {
                xCoordinateList.Add(sideLineLength + backgroundGridWidth * i);
            }
        }

        private void RefreshYCoordinateList(float horizontalGridNum)
        {
            yCoordinateList.Clear();
            for (int i = 0; i < (VerticalGridNum + 1); i++)
            {
                yCoordinateList.Add(topLineLength + ((GraphHeight) * i / (VerticalGridNum)));
            }
        }

        private async void RefreshDrawDotList(float verticalGridNum)
        {
            if (dataLists != null && dataLists.Count > 0)
            {
                if (drawDotLists.Count == 0)
                {
                    for (int k = 0; k < dataLists.Count; k++)
                    {
                        drawDotLists.Add(new List<Dot>());
                    }
                }
                float maxValue = 0;
                float minValue = 0;
                for (int k = 0; k < dataLists.Count; k++)
                {
                    float kMax = dataLists[k].Max();
                    float kMin = dataLists[k].Min();

                    if (maxValue < kMax)
                        maxValue = kMax;
                    if (minValue > kMin)
                        minValue = kMin;
                }
                for (int k = 0; k < dataLists.Count; k++)
                {
                    int drawDotSize = drawDotLists[k].Count;

                    for (int i = 0; i < dataLists[k].Count; i++)
                    {
                        float x = xCoordinateList[i];
                        // Make space for y data labels
                        float y;
                        if (maxValue == minValue)
                        {
                            y = topLineLength + (GraphHeight) / 2f;
                        }
                        else
                        {
                            y = topLineLength + (GraphHeight) * (maxValue - dataLists[k][i]) / (maxValue - minValue);
                        }
                        // int y = yCoordinateList.get(verticalGridNum - dataLists[k].get(i)) + bottomTextHeight + bottomTextTopMargin + bottomTextDescent;

                        if (i > drawDotSize - 1)
                        {
                            drawDotLists[k].Add(new Dot(x, 0, x, y, dataLists[k][i], k));
                        }
                        else
                        {
                            drawDotLists[k][i] = drawDotLists[k][i].SetTargetData(x, y, dataLists[k][i], k);
                        }
                    }

                    int temp = drawDotLists[k].Count - dataLists[k].Count;
                    for (int i = 0; i < temp; i++)
                    {
                        drawDotLists[k].RemoveAt(drawDotLists[k].Count - 1);
                    }
                }
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, animator);
        }

        private void RefreshTopLineLength(float verticalGridNum)
        {
            // For prevent popup can't be completely showed when backgroundGridHeight is too small.
            // But this code not so good.
            float labelsize = bottomTextHeight * 2 + bottomTextTopMargin;

            if (DrawDataLabels && (GraphHeight) /
                    (verticalGridNum + 2) < labelsize)
            {
                topLineLength = labelsize + 2;
            }
            else
            {
                topLineLength = MIN_TOP_LINE_LENGTH;
            }
        }

        private void Canvas_CreateResources(CanvasVirtualControl sender, CanvasCreateResourcesEventArgs args)
        {
            //
        }

        private void Canvas_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            // Draw the effect to whatever regions of the CanvasVirtualControl have been invalidated.
            foreach (var region in args.InvalidatedRegions)
            {
                using (var drawingSession = sender.CreateDrawingSession(region))
                {
                    DrawBackgroundLines(region, drawingSession);
                    DrawLines(region, drawingSession);
                    DrawDots(region, drawingSession);
                }
            }
        }

        private void DrawDots(Rect region, CanvasDrawingSession drawingSession)
        {
            if (DrawDotPoints)
            {
                if (drawDotLists != null && drawDotLists.Count > 0)
                {
                    for (int k = 0; k < drawDotLists.Count; k++)
                    {
                        Color bigCirColor = colorArray[k % 3];

                        foreach (Dot dot in drawDotLists[k])
                        {
                            // Draw Dots
                            if (RectHelper.Contains(region, new Point(dot.x, dot.y)))
                            {
                                drawingSession.FillCircle(dot.x, dot.y, DOT_OUTER_CIR_RADIUS, Colors.White);
                                drawingSession.DrawCircle(dot.x, dot.y, DOT_OUTER_CIR_RADIUS, bigCirColor, 2);
                            }
                        }
                    }
                }
            }
        }

        private void DrawLines(Rect region, CanvasDrawingSession drawingSession)
        {
            Color lineColor = Colors.White;
            float lineStrokeWidth = 2;

            var LinePath = new CanvasPathBuilder(drawingSession);

            for (int k = 0; k < drawDotLists.Count; k++)
            {
                float firstX = -1;
                float firstY = -1;
                // needed to end the path for background
                float lastUsedEndX = 0;
                float lastUsedEndY = 0;

                Color bgColor = colorArray[k % 3];
                bgColor.A = 0x50;

                for (int i = 0; i < drawDotLists[k].Count - 1; i++)
                {
                    Dot dot = drawDotLists[k][i];
                    Dot nextDot = drawDotLists[k][i + 1];

                    float startX = dot.x;
                    float startY = dot.y;
                    float endX = nextDot.x;
                    float endY = nextDot.y;

                    // Draw Dots
                    if (firstX == -1)
                    {
                        firstX = 0;
                        firstY = dot.y;
                        LinePath.BeginFigure(firstX, firstY);
                    }

                    LinePath.AddLine(startX, startY);
                    LinePath.AddLine(endX, endY);

                    // Draw top label
                    if (k == 0 && DrawDataLabels)
                    {
                        float x = sideLineLength + backgroundGridWidth * i;
                        float y = dot.y - bottomTextHeight * 2.5f;

                        var txtLayout = new CanvasTextLayout(drawingSession, dataLabels[i].Value, BottomTextFormat, 0, 0);
                        Rect txtRect = RectHelper.FromPoints(
                            new Point(x - txtLayout.DrawBounds.Width / 2, y - txtLayout.DrawBounds.Height / 2),
                            new Point(x + txtLayout.DrawBounds.Width / 2, y + txtLayout.DrawBounds.Height / 2));

                        if (!RectHelper.Intersect(region, txtRect).IsEmpty)
                            drawingSession.DrawTextLayout(txtLayout, x, y, BottomTextColor);
                    }

                    // Draw last items
                    if (i + 1 == drawDotLists[k].Count - 1)
                    {
                        if (k == 0 && DrawDataLabels)
                        {
                            float x = sideLineLength + backgroundGridWidth * (i + 1);
                            float y = nextDot.y - bottomTextHeight * 3;

                            var txtLayout = new CanvasTextLayout(drawingSession, dataLabels[i + 1].Value, BottomTextFormat, 0, 0);
                            Rect txtRect = RectHelper.FromPoints(
                                new Point(x - txtLayout.DrawBounds.Width / 2, y - txtLayout.DrawBounds.Height / 2),
                                new Point(x + txtLayout.DrawBounds.Width / 2, y + txtLayout.DrawBounds.Height / 2));

                            if (!RectHelper.Intersect(region, txtRect).IsEmpty)
                            {
                                drawingSession.DrawTextLayout(txtLayout, x, y, BottomTextColor);
                            }
                        }

                        LinePath.AddLine(endX, endY);
                        LinePath.AddLine(ViewWidth, endY);
                    }

                    lastUsedEndY = endY;
                }

                if (k == 0 && DrawGraphBackground && firstX != -1)
                {
                    // end / close path
                    if (lastUsedEndY != GraphHeight + topLineLength)
                    {
                        // dont draw line to same point, otherwise the path is completely broken
                        LinePath.AddLine(ViewWidth, GraphHeight + topLineLength);
                    }
                    LinePath.AddLine(firstX, GraphHeight + topLineLength);
                    if (firstY != GraphHeight + topLineLength)
                    {
                        // dont draw line to same point, otherwise the path is completely broken
                        LinePath.AddLine(firstX, firstY);
                    }
                }

                LinePath.EndFigure(CanvasFigureLoop.Open);
                var line = CanvasGeometry.CreatePath(LinePath);
                drawingSession.DrawGeometry(line, lineColor, lineStrokeWidth);
                drawingSession.FillGeometry(line, bgColor);
            }
        }

        private void DrawBackgroundLines(Rect region, CanvasDrawingSession drawingSession)
        {
            Color bgColor = Colors.Yellow;
            bgColor.A = 0x50;

            var iconTxtFormat = new CanvasTextFormat()
            {
                FontFamily = "ms-appx:///Assets/WeatherIcons/weathericons-regular-webfont.ttf#Weather Icons",
                FontSize = 24,
                HorizontalAlignment = CanvasHorizontalAlignment.Center
            };
            Color iconColor = Colors.Yellow;

            if (DrawGridLines)
            {
                float BackgroundLineStrokeWidth = 1f;
                var BGLineDashStroke = new CanvasStrokeStyle()
                {
                    CustomDashStyle = new float[] { 4, 2, 4, 2 },
                    DashOffset = 1,
                    LineJoin = CanvasLineJoin.Round
                };

                // Draw vertical lines
                for (int i = 0; i < xCoordinateList.Count; i++)
                {
                    float x = xCoordinateList[i];
                    float y1 = 0;
                    float y2 = GraphHeight + topLineLength;

                    if (!RectHelper.Intersect(region, RectHelper.FromPoints(new Point(x, y1), new Point(x, y2))).IsEmpty)
                        drawingSession.DrawLine(x, y1, x, y2, BackgroundLineColor);
                }

                if (!DrawDotLine)
                {
                    // Draw solid lines
                    for (int i = 0; i < yCoordinateList.Count; i++)
                    {
                        if ((yCoordinateList.Count - 1 - i) % DataOfAGrid == 0)
                        {
                            float x1 = 0;
                            float x2 = ViewWidth;
                            float y = yCoordinateList[i];

                            if (y <= region.Bottom && y >= region.Top)
                                drawingSession.DrawLine((float)region.Left, y, (float)region.Right, y, BackgroundLineColor);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < yCoordinateList.Count; i++)
                    {
                        if ((yCoordinateList.Count - 1 - i) % DataOfAGrid == 0)
                        {
                            float x1 = 0;
                            float x2 = ViewWidth;
                            float y = yCoordinateList[i];

                            if (y <= region.Bottom && y >= region.Top)
                                drawingSession.DrawLine((float)region.Left, y, (float)region.Right, y, BackgroundLineColor, BackgroundLineStrokeWidth, BGLineDashStroke);
                        }
                    }
                }
            }

            // Draw Bottom Text
            if (dataLabels != null)
            {
                bool drawIcons = iconLabels != null && iconLabels.Count > 0;

                for (int i = 0; i < dataLabels.Count; i++)
                {
                    float x = sideLineLength + backgroundGridWidth * i;
                    float y = ViewHeight - bottomTextDescent;

                    var btmTxtLayout = new CanvasTextLayout(drawingSession, dataLabels[i].Key, BottomTextFormat, 0, 0);
                    Rect btmTxtRect = RectHelper.FromPoints(
                        new Point(x - btmTxtLayout.DrawBounds.Width / 2, y - btmTxtLayout.DrawBounds.Height / 2),
                        new Point(x + btmTxtLayout.DrawBounds.Width / 2, y + btmTxtLayout.DrawBounds.Height / 2));
                    if (!RectHelper.Intersect(region, btmTxtRect).IsEmpty)
                        drawingSession.DrawTextLayout(btmTxtLayout, x, y, BottomTextColor);

                    if (drawIcons)
                    {
                        int rotation = iconLabels[iconLabels.Count == 1 ? 0 : i].Value;
                        string icon = iconLabels[iconLabels.Count == 1 ? 0 : i].Key;

                        var iconTxtLayout = new CanvasTextLayout(drawingSession, icon, iconTxtFormat, 0, 0);
                        Rect iconRect = RectHelper.FromPoints(
                            new Point(x - iconTxtLayout.DrawBounds.Width / 2, y - bottomTextHeight - iconBottomMargin * 3f - iconTxtLayout.DrawBounds.Height / 2),
                            new Point(x + iconTxtLayout.DrawBounds.Width / 2, y - bottomTextHeight - iconBottomMargin * 3f + iconTxtLayout.DrawBounds.Height / 2));

                        if (!RectHelper.Intersect(region, iconRect).IsEmpty)
                        {
                            var prevTransform = drawingSession.Transform;

                            var radAngle = ConversionMethods.ToRadians(rotation);
                            var rotTransform = Matrix3x2.CreateRotation(radAngle,
                                new Vector2(0, (float)iconTxtLayout.LayoutBounds.Height / 2f));
                            var translTransform = Matrix3x2.CreateTranslation(new Vector2(x, y - bottomTextHeight - iconBottomMargin * 3f));

                            drawingSession.Transform = Matrix3x2.Multiply(rotTransform, translTransform);

                            drawingSession.DrawTextLayout(iconTxtLayout,
                                0,
                                0,
                                BottomTextColor);

                            drawingSession.Transform = prevTransform;
                        }
                    }
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);

            Canvas.Width = backgroundGridWidth * HorizontalGridNum + sideLineLength * 2;
            Canvas.Height = availableSize.Height;
            ScrollViewer.Height = availableSize.Height;
            ScrollViewer.Width = availableSize.Width;

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;

            // Redraw View
            RefreshAfterDataChanged();

            return size;
        }

        internal class Dot
        {
            public float x;
            public float y;
            float data;
            float targetX;
            float targetY;
            int linenumber;
            const float velocity = 24;

            public Dot(float x, float y, float targetX, float targetY, float data, int linenumber)
            {
                this.x = x;
                this.y = y;
                this.linenumber = linenumber;
                SetTargetData(targetX, targetY, data, linenumber);
            }

            public Dot SetTargetData(float targetX, float targetY, float data, int linenumber)
            {
                this.targetX = targetX;
                this.targetY = targetY;
                this.data = data;
                this.linenumber = linenumber;
                return this;
            }

            public bool IsAtRest()
            {
                return (x == targetX) && (y == targetY);
            }

            public void Update()
            {
                x = UpdateSelf(x, targetX, velocity);
                y = UpdateSelf(y, targetY, velocity);
            }

            private float UpdateSelf(float origin, float target, float velocity)
            {
                if (origin < target)
                {
                    origin += velocity;
                }
                else if (origin > target)
                {
                    origin -= velocity;
                }
                if (Math.Abs(target - origin) < velocity)
                {
                    origin = target;
                }
                return origin;
            }
        }
    }
}
