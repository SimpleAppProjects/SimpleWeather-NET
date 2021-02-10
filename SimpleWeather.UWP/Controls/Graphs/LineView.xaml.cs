using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls.Graphs
{
    /*
     *  Multi-series line graph
     *  Based on Android implementation of LineView: https://github.com/SimpleAppProjects/SimpleWeather-Android
     *  Which is:
     *  Based on LineView from http://www.androidtrainee.com/draw-android-line-chart-with-animation/
     *  Graph background (under line) based on - https://github.com/jjoe64/GraphView (LineGraphSeries)
     */

    public sealed partial class LineView : UserControl, IDisposable, IGraph
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

        private float sideLineLength = 0f;
        private float backgroundGridWidth;
        private float longestTextWidth;

        private Color BackgroundLineColor => BackgroundLineColorBrush.Color;
        private Color BottomTextColor => BottomTextColorBrush.Color;

        public bool DrawGridLines { get; set; }
        public bool DrawDotLine { get; set; }
        public bool DrawDotPoints { get; set; }
        public bool DrawGraphBackground { get; set; }
        public bool DrawIconLabels { get; set; }
        public bool DrawDataLabels { get; set; }
        public bool DrawSeriesLabels { get; set; }

        private const float BottomTextSize = 12;
        private readonly CanvasTextFormat BottomTextFormat;

        private double IconHeight;
        private Dictionary<String, CanvasBitmap> IconCache;

        public bool ReadyToDraw => Canvas.ReadyToDraw;

        private readonly WeatherIconsManager wim = WeatherIconsManager.GetInstance();

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

            iconBottomMargin = Canvas.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
            bottomTextTopMargin = Canvas.ConvertDipsToPixels(6, CanvasDpiRounding.Floor);
            DOT_INNER_CIR_RADIUS = Canvas.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
            DOT_OUTER_CIR_RADIUS = Canvas.ConvertDipsToPixels(5, CanvasDpiRounding.Floor);

            backgroundGridWidth = Canvas.ConvertDipsToPixels(45, CanvasDpiRounding.Floor);
        }

        private float GraphTop
        {
            get
            {
                double graphTop = Padding.Top;
                if (DrawSeriesLabels) graphTop += LegendHeight;
                graphTop += bottomTextTopMargin + bottomTextHeight * 2f + bottomTextDescent * 2f;

                return (float)graphTop;
            }
        }

        private float GraphHeight
        {
            get
            {
                float graphHeight = ViewHeight - bottomTextTopMargin - bottomTextHeight - bottomTextDescent;
                if (DrawIconLabels) graphHeight -= (float)IconHeight;

                return graphHeight;
            }
        }

        private float LegendHeight
        {
            get
            {
                return bottomTextTopMargin + bottomTextHeight * 2f + bottomTextDescent * 2f;
            }
        }

        public FrameworkElement GetControl()
        {
            return this;
        }

        public ScrollViewer GetScrollViewer()
        {
            return ScrollViewer;
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

            double longestWidth = 0;
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
                        using (var textLayout = new CanvasTextLayout(Canvas, s.Substring(0, 1), BottomTextFormat, 0, 0))
                        {
                            longestWidth = txtLayout.LayoutBounds.Width + (textLayout.LayoutBounds.Width * 2.25f);
                        }
                    }
                    if (bottomTextDescent < (Math.Abs(txtLayout.LayoutBounds.Bottom)))
                    {
                        bottomTextDescent = Math.Abs((float)txtLayout.LayoutBounds.Bottom);
                    }
                }
            }

            if (longestTextWidth < longestWidth)
            {
                longestTextWidth = (float)longestWidth;
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

            float biggestData = 0;
            float prevLongestTextWidth = longestTextWidth;
            double longestWidth = 0;

            foreach (LineDataSeries series in dataLists)
            {
                if (series.SeriesData.Count > dataLabels.Count)
                {
                    throw new Exception("LineView error: SeriesData.Count > dataLabels.Count !!!");
                }

                foreach (YEntryData i in series.SeriesData)
                {
                    if (biggestData < i.Y)
                    {
                        biggestData = i.Y;
                    }

                    // Measure Y label
                    var s = i.YLabel;
                    using (var txtLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0))
                    {
                        if (longestWidth < txtLayout.LayoutBounds.Width)
                        {
                            using (var textLayout = new CanvasTextLayout(Canvas, s.Substring(0, 1), BottomTextFormat, 0, 0))
                            {
                                longestWidth = txtLayout.LayoutBounds.Width + (textLayout.LayoutBounds.Width * 2.25f);
                            }
                        }
                    }
                    if (longestTextWidth < longestWidth)
                    {
                        longestTextWidth = (float)longestWidth;
                    }
                    if (sideLineLength < longestWidth / 2)
                    {
                        sideLineLength = (float)longestWidth / 2f;
                    }
                }
                DataOfAGrid = 1;
                while (biggestData / 10 > DataOfAGrid)
                {
                    DataOfAGrid *= 10;
                }
            }

            backgroundGridWidth = longestTextWidth;

            if (prevLongestTextWidth != longestTextWidth)
            {
                RefreshXCoordinateList();
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
                float freeSpace = (float)(ScrollViewer.Width - GetPreferredWidth());
                float additionalSpace = freeSpace / HorizontalGridNum;
                backgroundGridWidth += additionalSpace;
            }
            RefreshXCoordinateList();
        }

        private void RefreshAfterDataChanged()
        {
            RefreshYCoordinateList();
            RefreshDrawDotList();
        }

        private int VerticalGridNum
        {
            get
            {
                int verticalGridNum = 4; // int MIN_VERTICAL_GRID_NUM = 4;
                if (dataLists != null && dataLists.Count != 0)
                {
                    foreach (LineDataSeries series in dataLists)
                    {
                        foreach (YEntryData entry in series.SeriesData)
                        {
                            if (verticalGridNum < (entry.Y + 1))
                            {
                                verticalGridNum = (int)Math.Ceiling(entry.Y) + 1;
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
            xCoordinateList.EnsureCapacity(HorizontalGridNum);
            for (int i = 0; i < (HorizontalGridNum + 1); i++)
            {
                xCoordinateList.Add(sideLineLength + backgroundGridWidth * i);
            }
        }

        private void RefreshYCoordinateList()
        {
            yCoordinateList.Clear();
            yCoordinateList.EnsureCapacity(VerticalGridNum);
            for (int i = 0; i < (VerticalGridNum + 1); i++)
            {
                /*
                 * Scaling formula
                 *
                 * ((value - minValue) / (maxValue - minValue)) * (scaleMax - scaleMin) + scaleMin
                 * minValue = 0; maxValue = verticalGridNum; value = i
                 */
                yCoordinateList.Add(((float)i / (VerticalGridNum)) * (GraphHeight - GraphTop) + GraphTop);
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
                    float kMax = 0;
                    float kMin = 0;

                    foreach (var seriesData in dataLists[k].SeriesData)
                    {
                        if (kMax < seriesData.Y)
                            kMax = seriesData.Y;
                        if (kMin > seriesData.Y)
                            kMin = seriesData.Y;
                    }

                    if (maxValue < kMax)
                        maxValue = kMax;
                    if (minValue > kMin)
                        minValue = kMin;
                }

                float graphHeight = GraphHeight;
                float graphTop = GraphTop;

                for (int k = 0; k < dataLists.Count; k++)
                {
                    int drawDotSize = drawDotLists[k].Count;

                    if (drawDotSize > 0)
                    {
                        drawDotLists[k].EnsureCapacity(dataLists[k].SeriesData.Count);
                    }

                    for (int i = 0; i < dataLists[k].SeriesData.Count; i++)
                    {
                        float x = xCoordinateList[i];
                        float y;
                        if (maxValue == minValue)
                        {
                            if (maxValue == 0)
                            {
                                y = graphHeight;
                            }
                            else if (maxValue == 100)
                            {
                                y = graphTop;
                            }
                            else
                            {
                                y = graphHeight / 2f;
                            }
                        }
                        else
                        {
                            /*
                             * Scaling formula
                             *
                             * ((value - minValue) / (maxValue - minValue)) * (scaleMax - scaleMin) + scaleMin
                             * graphTop is scaleMax & graphHeight is scaleMin due to View coordinate system
                             */
                            y = ((dataLists[k].SeriesData[i].Y - minValue) / (maxValue - minValue)) * (graphTop - graphHeight) + graphHeight;
                        }

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

        private void Canvas_CreateResources(CanvasVirtualControl sender, CanvasCreateResourcesEventArgs args)
        {
            // Calculate icon height
            IconHeight = sender.ConvertDipsToPixels(30, CanvasDpiRounding.Floor);

            IconCache = new Dictionary<string, CanvasBitmap>();

            // Initialize common icons
            CanvasBitmap.LoadAsync(sender, wim.GetWeatherIconURI(WeatherIcons.RAINDROP)).AsTask().ContinueWith((t) =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    IconCache.TryAdd(WeatherIcons.RAINDROP, t.Result);
                }
            });
            CanvasBitmap.LoadAsync(sender, wim.GetWeatherIconURI(WeatherIcons.WIND_DIRECTION)).AsTask().ContinueWith((t) =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    IconCache.TryAdd(WeatherIcons.WIND_DIRECTION, t.Result);
                }
            });
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
                        var series = dataLists[k];
                        var color = series.GetColor(k);

                        var bigCirColor = color;
                        var smallCirColor = ColorUtils.SetAlphaComponent(color, 0x99);

                        foreach (Dot dot in drawDotLists[k])
                        {
                            // Draw Dots
                            if (RectHelper.Contains(region, new Point(dot.X, dot.Y)))
                            {
                                drawingSession.FillCircle(dot.X, dot.Y, DOT_OUTER_CIR_RADIUS, bigCirColor);
                                drawingSession.FillCircle(dot.X, dot.Y, DOT_INNER_CIR_RADIUS, smallCirColor);
                            }
                        }
                    }
                }
            }
        }

        private void DrawLines(Rect region, CanvasDrawingSession drawingSession)
        {
            if (drawDotLists.Any())
            {
                float graphHeight = GraphHeight;
                float lineStrokeWidth = drawingSession.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
                Rect drawingRect;

                for (int k = 0; k < drawDotLists.Count; k++)
                {
                    var series = dataLists[k];
                    ICollection<TextEntry> textEntries = new LinkedList<TextEntry>();

                    var BackgroundPath = new CanvasPathBuilder(drawingSession);

                    float firstX = -1;
                    // needed to end the path for background
                    Dot currentDot = null;

                    var lineColor = series.GetColor(k);
                    var backgroundColor = ColorUtils.SetAlphaComponent(series.GetColor(k), 0x99);

                    if (DrawGraphBackground)
                        BackgroundPath.BeginFigure((float)region.Left, graphHeight);

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
                            drawingSession.DrawLine((float)region.Left, dot.Y, dot.X, dot.Y, lineColor, lineStrokeWidth);
                        }

                        drawingRect = new Rect(dot.X, dot.Y, nextDot.X, nextDot.Y);
                        if (!RectHelper.Intersect(region, drawingRect).IsEmpty)
                        {
                            drawingSession.DrawLine(dot.X, dot.Y, nextDot.X, nextDot.Y, lineColor, lineStrokeWidth);
                        }

                        if (DrawDataLabels)
                        {
                            // Draw top label
                            float x = dot.X;
                            float y = dot.Y - bottomTextHeight;

                            var txtLayout = new CanvasTextLayout(drawingSession, entry.YLabel, BottomTextFormat, 0, 0);

                            Rect txtRect = RectHelper.FromPoints(
                                new Point(x - txtLayout.LayoutBounds.Width / 2, y - txtLayout.LayoutBounds.Height / 2),
                                new Point(x + txtLayout.LayoutBounds.Width / 2, y + txtLayout.LayoutBounds.Height / 2));

                            if (!RectHelper.Intersect(region, txtRect).IsEmpty)
                            {
                                textEntries.Add(new TextEntry(txtLayout, x, y - (float)txtLayout.LayoutBounds.Height));
                            }
                            else
                            {
                                txtLayout.Dispose();
                            }
                        }

                        if (firstX == -1)
                        {
                            firstX = (float)region.Left;
                            if (DrawGraphBackground)
                                BackgroundPath.AddLine(firstX, startY);
                        }

                        if (DrawGraphBackground)
                        {
                            BackgroundPath.AddLine(startX, startY);
                        }

                        currentDot = dot;

                        // Draw last items
                        if (i + 1 == drawDotLists[k].Count - 1)
                        {
                            if (DrawDataLabels)
                            {
                                // Draw top label
                                float x = nextDot.X;
                                float y = nextDot.Y - bottomTextHeight;

                                var txtLayout = new CanvasTextLayout(drawingSession, nextEntry.YLabel, BottomTextFormat, 0, 0);

                                Rect txtRect = RectHelper.FromPoints(
                                    new Point(x - txtLayout.LayoutBounds.Width / 2, y - txtLayout.LayoutBounds.Height / 2),
                                    new Point(x + txtLayout.LayoutBounds.Width / 2, y + txtLayout.LayoutBounds.Height / 2));

                                if (!RectHelper.Intersect(region, txtRect).IsEmpty)
                                {
                                    textEntries.Add(new TextEntry(txtLayout, x, y - (float)txtLayout.LayoutBounds.Height));
                                }
                                else
                                {
                                    txtLayout.Dispose();
                                }
                            }

                            drawingRect = new Rect(nextDot.X, nextDot.Y, region.Right, nextDot.Y);
                            if (!RectHelper.Intersect(region, drawingRect).IsEmpty)
                            {
                                drawingSession.DrawLine(nextDot.X, nextDot.Y, (float)region.Right, nextDot.Y, lineColor, lineStrokeWidth);
                            }

                            currentDot = nextDot;

                            if (!RectHelper.Intersect(region, drawingRect).IsEmpty)
                            {
                                BackgroundPath.AddLine(endX, endY);
                            }
                        }
                    }

                    if (DrawGraphBackground)
                    {
                        if (currentDot != null)
                        {
                            BackgroundPath.AddLine((float)region.Right, currentDot.Y);
                        }
                        if (firstX != -1)
                        {
                            BackgroundPath.AddLine((float)region.Right, graphHeight);
                        }

                        BackgroundPath.EndFigure(CanvasFigureLoop.Closed);
                        var line = CanvasGeometry.CreatePath(BackgroundPath);
                        drawingSession.FillGeometry(line, backgroundColor);
                        line.Dispose();
                    }
                    BackgroundPath?.Dispose();

                    if (DrawDataLabels)
                    {
                        foreach (var entry in textEntries)
                        {
                            drawingSession.DrawTextLayout(entry.TextLayout, entry.X, entry.Y, BottomTextColor);
                            entry.TextLayout.Dispose();
                        }
                    }
                }
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
                    float y1 = GraphTop;
                    float y2 = GraphHeight;

                    if (!RectHelper.Intersect(region, RectHelper.FromPoints(new Point(x, y1), new Point(x, y2))).IsEmpty)
                        drawingSession.DrawLine(x, y1, x, y2, BackgroundLineColor);
                }

                if (!DrawDotLine)
                {
                    // Draw horizontal lines
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
                    float x = xCoordinateList[i];
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

                        Rect iconRect = RectHelper.FromPoints(
                            new Point(x - IconHeight / 2, y - bottomTextHeight - iconBottomMargin - IconHeight / 2),
                            new Point(x + IconHeight / 2, y - bottomTextHeight - iconBottomMargin + IconHeight / 2));

                        if (!RectHelper.Intersect(region, iconRect).IsEmpty)
                        {
                            CanvasBitmap icon = IconCache.GetValueOrDefault(xData.XIcon, null);

                            if (icon == null)
                            {
                                var task = CanvasBitmap.LoadAsync(Canvas, wim.GetWeatherIconURI(xData.XIcon)).AsTask();
                                task.ContinueWith((t) =>
                                {
                                    if (t.IsCompletedSuccessfully)
                                    {
                                        IconCache.TryAdd(xData.XIcon, t.Result);
                                        Dispatcher.RunOnUIThread(() => Canvas.Invalidate(iconRect));
                                    }
                                });
                                continue;
                            }

                            DrawIcon(region, drawingSession, icon, x, y, rotation);
                        }
                    }
                }
                // END_LOOP
            }
        }

        private void DrawIcon(Rect region, CanvasDrawingSession drawingSession, CanvasBitmap icon, float x, float y, int rotation = 0)
        {
            var prevTransform = drawingSession.Transform;

            var radAngle = ConversionMethods.ToRadians(rotation);
            var rotTransform = Matrix3x2.CreateRotation(radAngle,
                new Vector2((float)IconHeight / 2f, (float)IconHeight / 2f));
            var translTransform = Matrix3x2.CreateTranslation(new Vector2(x - (float)(IconHeight / 2), y - (float)(IconHeight / 2) - bottomTextHeight - iconBottomMargin * 2f));

            drawingSession.Transform = Matrix3x2.Multiply(rotTransform, translTransform);

            ICanvasImage finalIcon;
            if (!wim.IsFontIcon)
            {
                finalIcon = new TintEffect()
                {
                    Source = icon,
                    Color = BottomTextColor
                };
            }
            else
            {
                finalIcon = icon;
            }

            drawingSession.DrawImage(finalIcon, new Rect(0, 0, IconHeight, IconHeight), icon.Bounds);

            drawingSession.Transform = prevTransform;
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
                        title = String.Format(CultureInfo.CurrentCulture, "{0} {1}", App.ResLoader.GetString("Label_Series"), i);
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
                    var series = dataLists[i];
                    var seriesColor = series.GetColor(i);
                    var title = series.SeriesLabel;
                    if (String.IsNullOrWhiteSpace(title))
                    {
                        title = String.Format("{0} {1}", App.ResLoader.GetString("Label_Series"), i);
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

        private float GetPreferredWidth()
        {
            return (backgroundGridWidth * HorizontalGridNum) + (sideLineLength * 2);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);

            ScrollViewer.Height = double.IsInfinity(availableSize.Height) ? double.NaN : availableSize.Height;
            ScrollViewer.Width = double.IsInfinity(availableSize.Width) ? double.NaN : availableSize.Width;
            RefreshGridWidth();

            Canvas.Width = Math.Max(GetPreferredWidth(), ScrollViewer.Width);
            Canvas.Height = availableSize.Height;

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;

            // Redraw View
            RefreshAfterDataChanged();
            Canvas.Invalidate();

            // Post the event to the dispatcher to allow the method to complete first
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ItemWidthChanged?.Invoke(this, new ItemSizeChangedEventArgs()
                {
                    NewSize = new System.Drawing.Size(xCoordinateList.Count > 0 ? (int) xCoordinateList.Last() : 0, (int)Canvas.Height)
                });
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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

        internal class TextEntry
        {
            public CanvasTextLayout TextLayout { get; set; }
            public float X { get; set; }
            public float Y { get; set; }

            public TextEntry(CanvasTextLayout text, float x, float y)
            {
                this.TextLayout = text;
                this.X = x;
                this.Y = y;
            }
        }

        public void Dispose()
        {
            BottomTextFormat?.Dispose();
            IconCache?.Clear();
        }
    }
}