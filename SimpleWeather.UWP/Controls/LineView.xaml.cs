using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    /*
     *  Multi-series line graph
     *  Based on Android implementation of LineView: https://github.com/bryan2894-playgrnd/SimpleWeather-Android
     *  Which is:
     *  Based on LineView from http://www.androidtrainee.com/draw-android-line-chart-with-animation/
     *  Graph background (under line) based on - https://github.com/jjoe64/GraphView (LineGraphSeries)
     */

    public sealed partial class LineView : UserControl, IDisposable
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

        internal event EventHandler<ItemSizeChangedEventArgs> ItemWidthChanged;

        public ScrollViewer ScrollViewer { get { return this.InternalScrollViewer; } }

        private float ViewHeight;
        private float ViewWidth;
        private int DataOfAGrid = 10;
        private float bottomTextHeight = 0;

        private List<XLabelData> dataLabels; // X
        private List<LineDataSeries> dataLists; // Y data

        private List<float> xCoordinateList;
        private List<float> yCoordinateList;

        private List<List<Dot>> drawDotLists; // Y data

        //private Paint bottomTextPaint;
        private float bottomTextDescent;

        private readonly float iconBottomMargin;
        private readonly float bottomTextTopMargin;
        private readonly float DOT_INNER_CIR_RADIUS;
        private readonly float DOT_OUTER_CIR_RADIUS;
        private readonly float MIN_TOP_LINE_LENGTH;
        private readonly float LEGEND_MARGIN_HEIGHT;
        private readonly Color BackgroundLineColor = Color.FromArgb(0x80, 0xF5, 0xF5, 0xF5); // WhiteSmoke
        private readonly Color BottomTextColor = Colors.White;
        private readonly Color LineColor = Color.FromArgb(0x80, 0x00, 0x00, 0x00); // Black

        private float topLineLength;
        private float sideLineLength;
        private float backgroundGridWidth;
        private float longestTextWidth;

        private Color[] colorArray = { Color.FromArgb(0xFF, 0x00, 0x70, 0xc0), Colors.LightSeaGreen, Colors.YellowGreen };

        public bool DrawGridLines { get; set; }
        public bool DrawDotLine { get; set; }
        public bool DrawDotPoints { get; set; }
        public bool DrawGraphBackground { get; set; }
        public bool DrawIconLabels { get; set; }
        public bool DrawDataLabels { get; set; }
        public bool DrawSeriesLabels { get; set; }

        private const float BottomTextSize = 12;
        private CanvasTextFormat BottomTextFormat;

        private CanvasTextFormat IconFormat;

        public bool ReadyToDraw => Canvas.ReadyToDraw;

        public LineView()
        {
            this.InitializeComponent();

            dataLabels = new List<XLabelData>();
            dataLists = new List<LineDataSeries>();
            xCoordinateList = new List<float>();
            yCoordinateList = new List<float>();
            drawDotLists = new List<List<Dot>>();

            BottomTextFormat = new CanvasTextFormat
            {
                FontSize = BottomTextSize,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            IconFormat = new CanvasTextFormat
            {
                FontFamily = "ms-appx:///Assets/WeatherIcons/weathericons-regular-webfont.ttf#Weather Icons",
                FontSize = 24,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            iconBottomMargin = Canvas.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
            bottomTextTopMargin = Canvas.ConvertDipsToPixels(6, CanvasDpiRounding.Floor);
            DOT_INNER_CIR_RADIUS = Canvas.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
            DOT_OUTER_CIR_RADIUS = Canvas.ConvertDipsToPixels(5, CanvasDpiRounding.Floor);
            MIN_TOP_LINE_LENGTH = Canvas.ConvertDipsToPixels(12, CanvasDpiRounding.Floor);
            LEGEND_MARGIN_HEIGHT = Canvas.ConvertDipsToPixels(20, CanvasDpiRounding.Floor);

            topLineLength = MIN_TOP_LINE_LENGTH;
            sideLineLength = Canvas.ConvertDipsToPixels(45, CanvasDpiRounding.Floor) / 3 * 2;
            backgroundGridWidth = Canvas.ConvertDipsToPixels(45, CanvasDpiRounding.Floor);
        }

        private int ADJ
        {
            get
            {
                int adj = 1;
                if (DrawIconLabels) adj = 2; // Make space for icon labels

                return adj;
            }
        }

        private float GraphHeight
        {
            get
            {
                float height = ViewHeight - bottomTextTopMargin * ADJ - bottomTextHeight * ADJ - bottomTextDescent - topLineLength;
                if (DrawIconLabels) height -= iconBottomMargin;

                return height;
            }
        }

        public void ResetData()
        {
            this.dataLists.Clear();
            this.dataLabels.Clear();
            this.xCoordinateList.Clear();
            this.yCoordinateList.Clear();
            this.drawDotLists.Clear();
            bottomTextDescent = 0;
            longestTextWidth = 0;
            DrawIconLabels = false;
            RefreshAfterDataChanged();
            InvalidateMeasure();
        }

        public void SetData(List<XLabelData> dataLabels, List<LineDataSeries> dataLists)
        {
            if (dataLabels is null)
            {
                throw new ArgumentNullException(nameof(dataLabels));
            }
            if (dataLists is null)
            {
                throw new ArgumentNullException(nameof(dataLists));
            }

            if (!Enumerable.SequenceEqual(this.dataLabels, dataLabels) &&
                !Enumerable.SequenceEqual(this.dataLists, dataLists))
            {
                SetDataLabels(dataLabels);
                SetDataList(dataLists);
            }
        }

        private void SetDataLabels(List<XLabelData> dataLabels)
        {
            this.dataLabels.Clear();

            if (dataLabels != null)
            {
                this.dataLabels.AddRange(dataLabels);
            }

            if (!DrawIconLabels && dataLabels != null && dataLabels.Count > 0 && !String.IsNullOrEmpty(dataLabels[0].XIcon))
                DrawIconLabels = true;
            else if (DrawIconLabels && (dataLabels == null || dataLabels.Count <= 0))
                DrawIconLabels = false;

            double longestWidth = 0;
            String longestStr = "";
            bottomTextDescent = 0;
            longestTextWidth = 0;
            foreach (XLabelData labelData in dataLabels)
            {
                String s = labelData.XLabel;
                using (var txtLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0))
                {
                    if (bottomTextHeight < txtLayout.LayoutBounds.Height)
                    {
                        bottomTextHeight = (float)txtLayout.LayoutBounds.Height;
                    }
                    if (longestWidth < txtLayout.LayoutBounds.Width)
                    {
                        longestWidth = txtLayout.LayoutBounds.Width;
                        longestStr = s;
                    }
                    if (bottomTextDescent < (Math.Abs(txtLayout.LayoutBounds.Bottom)))
                    {
                        bottomTextDescent = Math.Abs((float)txtLayout.LayoutBounds.Bottom);
                    }
                }
            }

            if (longestTextWidth < longestWidth)
            {
                using (var textLayout = new CanvasTextLayout(Canvas, longestStr.Substring(0, 1), BottomTextFormat, 0, 0))
                {
                    longestTextWidth = (float)longestWidth + (float)textLayout.LayoutBounds.Width;
                }
            }
            if (sideLineLength < longestWidth / 2)
            {
                sideLineLength = (float)longestWidth / 2f;
            }
            backgroundGridWidth = longestTextWidth;

            RefreshXCoordinateList();
        }

        private void SetDataList(List<LineDataSeries> dataLists)
        {
            this.dataLists.Clear();

            if (dataLists != null)
            {
                this.dataLists.AddRange(dataLists);
            }

            foreach (LineDataSeries series in dataLists)
            {
                if (series.SeriesData.Count > dataLabels.Count)
                {
                    throw new Exception("LineView error: SeriesData.Count > dataLabels.Count !!!");
                }
            }
            float biggestData = 0;
            foreach (LineDataSeries series in dataLists)
            {
                foreach (YEntryData i in series.SeriesData)
                {
                    if (biggestData < i.Y)
                        biggestData = i.Y;
                }
                DataOfAGrid = 1;
                while (biggestData / 10 > DataOfAGrid)
                    DataOfAGrid *= 10;
            }

            RefreshAfterDataChanged();
            InvalidateMeasure();
        }

        private void RefreshGridWidth()
        {
            // Reset the grid width
            backgroundGridWidth = longestTextWidth;

            if (GetPreferredWidth() < ScrollViewer.Width)
            {
                int freeSpace = (int)(ScrollViewer.Width - GetPreferredWidth());
                int additionalSpace = freeSpace / HorizontalGridNum;
                backgroundGridWidth += additionalSpace;
            }
            RefreshXCoordinateList();
        }

        private void RefreshAfterDataChanged()
        {
            float verticalGridNum = VerticalGridNum;
            RefreshTopLineLength(verticalGridNum);
            RefreshYCoordinateList();
            RefreshDrawDotList();
        }

        private float VerticalGridNum
        {
            get
            {
                float verticalGridNum = 4; // int MIN_VERTICAL_GRID_NUM = 4;
                if (dataLists != null && dataLists.Count != 0)
                {
                    foreach (LineDataSeries series in dataLists)
                    {
                        foreach (YEntryData entry in series.SeriesData)
                        {
                            if (verticalGridNum < (entry.Y + 1))
                            {
                                verticalGridNum = entry.Y + 1;
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
                const int MIN_HORIZONTAL_GRID_NUM = 1;
                int horizontalGridNum = dataLabels.Count - 1;
                if (horizontalGridNum < MIN_HORIZONTAL_GRID_NUM)
                {
                    horizontalGridNum = MIN_HORIZONTAL_GRID_NUM;
                }
                return horizontalGridNum;
            }
        }

        private void RefreshXCoordinateList()
        {
            xCoordinateList.Clear();
            for (int i = 0; i < (HorizontalGridNum + 1); i++)
            {
                xCoordinateList.Add(sideLineLength + backgroundGridWidth * i);
            }
        }

        private void RefreshYCoordinateList()
        {
            yCoordinateList.Clear();
            for (int i = 0; i < (VerticalGridNum + 1); i++)
            {
                yCoordinateList.Add(topLineLength + ((GraphHeight) * i / (VerticalGridNum)));
            }
        }

        private void RefreshDrawDotList()
        {
            if (dataLists != null && dataLists.Count > 0)
            {
                if (drawDotLists.Count == 0 || drawDotLists.Count != dataLists.Count)
                {
                    drawDotLists.Clear();
                    for (int k = 0; k < dataLists.Count; k++)
                    {
                        drawDotLists.Add(new List<Dot>());
                    }
                }
                float maxValue = 0;
                float minValue = 0;
                for (int k = 0; k < dataLists.Count; k++)
                {
                    float kMax = dataLists[k].SeriesData.Max().Y;
                    float kMin = dataLists[k].SeriesData.Min().Y;

                    if (maxValue < kMax)
                        maxValue = kMax;
                    if (minValue > kMin)
                        minValue = kMin;
                }
                for (int k = 0; k < dataLists.Count; k++)
                {
                    int drawDotSize = drawDotLists[k].Count;

                    for (int i = 0; i < dataLists[k].SeriesData.Count; i++)
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
                            y = topLineLength + (GraphHeight) * (maxValue - dataLists[k].SeriesData[i].Y) / (maxValue - minValue);
                        }

                        // Make space for each series if necessary
                        y += (topLineLength * k * 1.25f);
                        if (y >= GraphHeight)
                        {
                            y = GraphHeight;
                        }

                        if (DrawSeriesLabels) y += LEGEND_MARGIN_HEIGHT;

                        if (i > drawDotSize - 1)
                        {
                            drawDotLists[k].Add(new Dot(x, y));
                        }
                        else
                        {
                            drawDotLists[k][i] = new Dot(x, y);
                        }
                    }

                    int temp = drawDotLists[k].Count - dataLists[k].SeriesData.Count;
                    for (int i = 0; i < temp; i++)
                    {
                        drawDotLists[k].RemoveAt(drawDotLists[k].Count - 1);
                    }
                }
            }
        }

        private void RefreshTopLineLength(float verticalGridNum)
        {
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

        private void Canvas_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            // Draw the effect to whatever regions of the CanvasVirtualControl have been invalidated.
            foreach (var InvalidatedRegion in args.InvalidatedRegions)
            {
                using (var drawingSession = sender.CreateDrawingSession(InvalidatedRegion))
                {
                    DrawBackgroundLines(InvalidatedRegion, drawingSession);
                    DrawLines(InvalidatedRegion, drawingSession);
                    DrawDots(InvalidatedRegion, drawingSession);
                    DrawSeriesLegend(InvalidatedRegion, drawingSession);
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
                            if (RectHelper.Contains(region, new Point(dot.X, dot.Y)))
                            {
                                drawingSession.FillCircle(dot.X, dot.Y, DOT_OUTER_CIR_RADIUS, bigCirColor);
                                drawingSession.FillCircle(dot.X, dot.Y, DOT_INNER_CIR_RADIUS, Colors.White);
                            }
                        }
                    }
                }
            }
        }

        private void DrawLines(Rect region, CanvasDrawingSession drawingSession)
        {
            float lineStrokeWidth = drawingSession.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
            Rect drawingRect;

            for (int k = 0; k < drawDotLists.Count; k++)
            {
                var BackgroundPath = new CanvasPathBuilder(drawingSession);

                float firstX = -1;
                float firstY = -1;
                // needed to end the path for background
                float lastUsedEndY = 0;

                Color bgColor = colorArray[k % 3];
                bgColor.A = 0x50;

                for (int i = 0; i < drawDotLists[k].Count - 1; i++)
                {
                    Dot dot = drawDotLists[k][i];
                    Dot nextDot = drawDotLists[k][i + 1];
                    YEntryData entry = dataLists[k].SeriesData[i];
                    YEntryData nextEntry = dataLists[k].SeriesData[i + 1];

                    float startX = dot.X;
                    float startY = dot.Y;
                    float endX = nextDot.X;
                    float endY = nextDot.Y;

                    drawingRect = new Rect((float)region.Left, dot.Y, dot.X, dot.Y);
                    if (firstX == -1 && !RectHelper.Intersect(region, drawingRect).IsEmpty)
                    {
                        drawingSession.DrawLine((float)region.Left, dot.Y, dot.X, dot.Y, LineColor, lineStrokeWidth);
                    }

                    drawingRect = new Rect(dot.X, dot.Y, nextDot.X, nextDot.Y);
                    if (!RectHelper.Intersect(region, drawingRect).IsEmpty)
                    {
                        drawingSession.DrawLine(dot.X, dot.Y, nextDot.X, nextDot.Y, LineColor, lineStrokeWidth);
                    }

                    // Draw top label
                    if (DrawDataLabels)
                    {
                        float x = sideLineLength + backgroundGridWidth * i;
                        float y = dot.Y - bottomTextHeight;

                        using (var txtLayout = new CanvasTextLayout(drawingSession, entry.YLabel, BottomTextFormat, 0, 0))
                        {
                            Rect txtRect = RectHelper.FromPoints(
                                new Point(x - txtLayout.LayoutBounds.Width / 2, y - txtLayout.LayoutBounds.Height / 2),
                                new Point(x + txtLayout.LayoutBounds.Width / 2, y + txtLayout.LayoutBounds.Height / 2));

                            if (!RectHelper.Intersect(region, txtRect).IsEmpty)
                                drawingSession.DrawTextLayout(txtLayout, x, y - (float)txtLayout.LayoutBounds.Height, BottomTextColor);
                        }
                    }

                    if (firstX == -1)
                    {
                        firstX = (float)region.Left;
                        firstY = startY;
                        if (DrawGraphBackground)
                            BackgroundPath.BeginFigure(firstX, firstY);
                    }

                    drawingRect = new Rect(startX, startY, endX, endY);
                    if (DrawGraphBackground && !RectHelper.Intersect(region, drawingRect).IsEmpty)
                    {
                        BackgroundPath.AddLine(startX, startY);
                        BackgroundPath.AddLine(endX, endY);
                    }

                    // Draw last items
                    if (i + 1 == drawDotLists[k].Count - 1)
                    {
                        if (DrawDataLabels)
                        {
                            // Draw top label
                            float x = sideLineLength + backgroundGridWidth * (i + 1);
                            float y = nextDot.Y - bottomTextHeight;

                            using (var txtLayout = new CanvasTextLayout(drawingSession, nextEntry.YLabel, BottomTextFormat, 0, 0))
                            {
                                Rect txtRect = RectHelper.FromPoints(
                                    new Point(x - txtLayout.LayoutBounds.Width / 2, y - txtLayout.LayoutBounds.Height / 2),
                                    new Point(x + txtLayout.LayoutBounds.Width / 2, y + txtLayout.LayoutBounds.Height / 2));

                                if (!RectHelper.Intersect(region, txtRect).IsEmpty)
                                {
                                    drawingSession.DrawTextLayout(txtLayout, x, y - (float)txtLayout.LayoutBounds.Height, BottomTextColor);
                                }
                            }
                        }

                        drawingRect = new Rect(nextDot.X, nextDot.Y, region.Right, nextDot.Y);
                        if (!RectHelper.Intersect(region, drawingRect).IsEmpty)
                        {
                            drawingSession.DrawLine(nextDot.X, nextDot.Y, (float)region.Right, nextDot.Y, LineColor, lineStrokeWidth);
                        }

                        drawingRect = new Rect(endX, endY, region.Right, endY);
                        if (!RectHelper.Intersect(region, drawingRect).IsEmpty)
                        {
                            BackgroundPath.AddLine(endX, endY);
                            BackgroundPath.AddLine((float)region.Right, endY);
                        }
                    }

                    lastUsedEndY = endY;
                }

                if (DrawGraphBackground && firstX != -1)
                {
                    // end / close path
                    if (lastUsedEndY != GraphHeight + topLineLength)
                    {
                        // dont draw line to same point, otherwise the path is completely broken
                        BackgroundPath.AddLine(ViewWidth, GraphHeight + topLineLength);
                    }
                    BackgroundPath.AddLine(firstX, GraphHeight + topLineLength);
                    if (firstY != GraphHeight + topLineLength)
                    {
                        // dont draw line to same point, otherwise the path is completely broken
                        BackgroundPath.AddLine(firstX, firstY);
                    }

                    BackgroundPath.EndFigure(CanvasFigureLoop.Open);
                    var line = CanvasGeometry.CreatePath(BackgroundPath);
                    drawingSession.FillGeometry(line, bgColor);
                    line.Dispose();
                }

                BackgroundPath.Dispose();
            }
        }

        private void DrawBackgroundLines(Rect region, CanvasDrawingSession drawingSession)
        {
            if (DrawGridLines)
            {
                float BackgroundLineStrokeWidth = drawingSession.ConvertDipsToPixels(1, CanvasDpiRounding.Floor);

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
                            float y = yCoordinateList[i];

                            if (y <= region.Bottom && y >= region.Top)
                                drawingSession.DrawLine((float)region.Left, y, (float)region.Right, y, BackgroundLineColor);
                        }
                    }
                }
                else
                {
                    var BGLineDashStroke = new CanvasStrokeStyle()
                    {
                        CustomDashStyle = new float[] { 4, 2, 4, 2 },
                        DashOffset = 1,
                        LineJoin = CanvasLineJoin.Round
                    };

                    for (int i = 0; i < yCoordinateList.Count; i++)
                    {
                        if ((yCoordinateList.Count - 1 - i) % DataOfAGrid == 0)
                        {
                            float y = yCoordinateList[i];

                            if (y <= region.Bottom && y >= region.Top)
                                drawingSession.DrawLine((float)region.Left, y, (float)region.Right, y, BackgroundLineColor, BackgroundLineStrokeWidth, BGLineDashStroke);
                        }
                    }

                    BGLineDashStroke.Dispose();
                }
            }

            // Draw Bottom Text
            if (dataLabels != null)
            {
                for (int i = 0; i < dataLabels.Count; i++)
                {
                    float x = sideLineLength + backgroundGridWidth * i;
                    float y = ViewHeight - bottomTextDescent;
                    XLabelData xData = dataLabels[i];

                    if (!String.IsNullOrWhiteSpace(xData.XLabel))
                    {
                        using (var btmTxtLayout = new CanvasTextLayout(drawingSession, xData.XLabel, BottomTextFormat, 0, 0))
                        {
                            Rect btmTxtRect = RectHelper.FromPoints(
                                new Point(x - btmTxtLayout.LayoutBounds.Width / 2, y - btmTxtLayout.LayoutBounds.Height / 2),
                                new Point(x + btmTxtLayout.LayoutBounds.Width / 2, y + btmTxtLayout.LayoutBounds.Height / 2));
                            if (!RectHelper.Intersect(region, btmTxtRect).IsEmpty)
                                drawingSession.DrawTextLayout(btmTxtLayout, x, y, BottomTextColor);
                        }
                    }

                    if (DrawIconLabels && !String.IsNullOrWhiteSpace(xData.XIcon))
                    {
                        int rotation = xData.XIconRotation;
                        string icon = xData.XIcon;

                        using (var iconTxtLayout = new CanvasTextLayout(drawingSession, icon, IconFormat, 0, 0))
                        {
                            Rect iconRect = RectHelper.FromPoints(
                                new Point(x - iconTxtLayout.LayoutBounds.Width / 2, y - bottomTextHeight - iconBottomMargin * 2f - iconTxtLayout.LayoutBounds.Height / 2),
                                new Point(x + iconTxtLayout.LayoutBounds.Width / 2, y - bottomTextHeight - iconBottomMargin * 2f + iconTxtLayout.LayoutBounds.Height / 2));

                            if (!RectHelper.Intersect(region, iconRect).IsEmpty)
                            {
                                var prevTransform = drawingSession.Transform;

                                var radAngle = ConversionMethods.ToRadians(rotation);
                                var rotTransform = Matrix3x2.CreateRotation(radAngle,
                                    new Vector2(0, (float)iconTxtLayout.LayoutBounds.Height / 2f));
                                var translTransform = Matrix3x2.CreateTranslation(new Vector2(x, y - (float)(iconTxtLayout.LayoutBounds.Height / 2) - bottomTextHeight - iconBottomMargin * 2f));

                                drawingSession.Transform = Matrix3x2.Multiply(rotTransform, translTransform);

                                drawingSession.DrawTextLayout(iconTxtLayout, 0, 0, BottomTextColor);

                                drawingSession.Transform = prevTransform;
                            }
                        }
                    }
                }
            }
        }

        private void DrawSeriesLegend(Rect region, CanvasDrawingSession drawingSession)
        {
            if (DrawSeriesLabels && dataLists.Count > 0)
            {
                int seriesSize = dataLists.Count;

                double longestWidth = 0;
                String longestStr = "";
                float textWidth = 0;
                float paddingLength = 0;
                float textHeight = 0;
                float textDescent = 0;
                for (int i = 0; i < seriesSize; i++)
                {
                    String title = dataLists[i].SeriesLabel;
                    if (String.IsNullOrWhiteSpace(title))
                    {
                        title = "Series " + i;
                    }

                    using (var txtLayout = new CanvasTextLayout(Canvas, title, BottomTextFormat, 0, 0))
                    {
                        if (textHeight < txtLayout.LayoutBounds.Height)
                        {
                            textHeight = (float)txtLayout.LayoutBounds.Height;
                        }
                        if (longestWidth < txtLayout.LayoutBounds.Width)
                        {
                            longestWidth = txtLayout.LayoutBounds.Width;
                            longestStr = title;
                        }
                        if (textDescent < (Math.Abs(txtLayout.LayoutBounds.Bottom)))
                        {
                            textDescent = Math.Abs((float)txtLayout.LayoutBounds.Bottom);
                        }
                    }
                }

                if (textWidth < longestWidth)
                {
                    using (var textLayout = new CanvasTextLayout(Canvas, longestStr.Substring(0, 1), BottomTextFormat, 0, 0))
                    {
                        textWidth = (float)longestWidth + (float)textLayout.LayoutBounds.Width;
                    }
                }
                if (paddingLength < longestWidth / 2)
                {
                    paddingLength = (float)longestWidth / 2f;
                }
                textWidth += drawingSession.ConvertDipsToPixels(4, CanvasDpiRounding.Floor); // Padding

                float rectSize = textHeight;
                float rect2TextPadding = drawingSession.ConvertDipsToPixels(8, CanvasDpiRounding.Floor);

                for (int i = 0; i < seriesSize; i++)
                {
                    Color seriesColor = colorArray[i % 3];
                    String title = dataLists[i].SeriesLabel;
                    if (String.IsNullOrWhiteSpace(title))
                    {
                        title = "Series " + i;
                    }

                    using (var txtLayout = new CanvasTextLayout(drawingSession, title, BottomTextFormat, 0, 0))
                    {
                        float xRectStart = paddingLength + textWidth * i + ((rectSize + rect2TextPadding) * i);
                        float xTextStart = paddingLength + textWidth * i + rectSize + ((rectSize + rect2TextPadding) * i);

                        Rect rect = RectHelper.FromPoints(
                                new Point(xRectStart, 0),
                                new Point(xRectStart + rectSize, rectSize));
                        Rect txtRect = RectHelper.FromPoints(
                                new Point(xTextStart + rect2TextPadding, 0),
                                new Point(xTextStart + rect2TextPadding + textWidth, textHeight));

                        if (!RectHelper.Intersect(region, rect).IsEmpty)
                        {
                            drawingSession.FillRectangle(rect, seriesColor);
                        }
                        if (!RectHelper.Intersect(region, txtRect).IsEmpty)
                        {
                            drawingSession.DrawTextLayout(txtLayout,
                                xTextStart + rect2TextPadding + (float)(txtLayout.LayoutBounds.Width / 2),
                                (float)(-txtLayout.LayoutBounds.Top / 2),
                                BottomTextColor);
                        }
                    }
                }
            }
        }

        public int GetItemPositionFromPoint(float xCoordinate)
        {
            if (HorizontalGridNum <= 1)
                return 0;

            return BinarySearchPointIndex(xCoordinate);
        }

        private int BinarySearchPointIndex(float targetXPoint)
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

        private int GetPreferredWidth()
        {
            return (int)((backgroundGridWidth * HorizontalGridNum) + (sideLineLength * 2));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);

            ScrollViewer.Height = availableSize.Height;
            ScrollViewer.Width = availableSize.Width;
            RefreshGridWidth();

            Canvas.Width = Math.Max(GetPreferredWidth(), availableSize.Width);
            Canvas.Height = availableSize.Height;

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;

            // Redraw View
            RefreshAfterDataChanged();
            Canvas.Invalidate();

            // Post the event to the dispatcher to allow the method to complete first
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ItemWidthChanged?.Invoke(this, new ItemSizeChangedEventArgs()
                {
                    NewSize = new System.Drawing.Size(xCoordinateList.Count > 0 ? (int) xCoordinateList.Last() : 0, (int)Canvas.Height)
                });
            });

            return size;
        }

        internal class Dot
        {
            public float X { get; set; }
            public float Y { get; set; }

            public Dot(float x, float y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        public void Dispose()
        {
            BottomTextFormat?.Dispose();
            IconFormat?.Dispose();
        }
    }

    internal class ItemSizeChangedEventArgs : RoutedEventArgs
    {
        public System.Drawing.Size NewSize { get; set; }
    }
}