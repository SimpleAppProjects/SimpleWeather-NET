using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
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
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Controls.Graphs
{
    /*
     *  Multi-series line graph
     *  Based on Android implementation of LineView: https://github.com/SimpleAppProjects/SimpleWeather-Android
     *  Which is:
     *  Based on LineView from http://www.androidtrainee.com/draw-android-line-chart-with-animation/
     *  Graph background (under line) based on - https://github.com/jjoe64/GraphView (LineGraphSeries)
     */

    public sealed class LineView : BaseGraphView<LineViewData, LineDataSeries, LineGraphEntry>, IDisposable
    {
        private int DataOfAGrid = 10;
        private float bottomTextHeight = 0;

        private List<float> yCoordinateList;

        private List<List<Dot>> drawDotLists; // Y data

        private readonly CanvasTextFormat BottomTextFormat;
        private float bottomTextDescent;

        private float iconBottomMargin;
        private float bottomTextTopMargin;
        private float DOT_INNER_CIR_RADIUS;
        private float DOT_OUTER_CIR_RADIUS;
        private const int MIN_VERTICAL_GRID_NUM = 4;

        public Color BackgroundLineColor
        {
            get { return (Color)GetValue(BackgroundLineColorProperty); }
            set { SetValue(BackgroundLineColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundLineColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundLineColorProperty =
            DependencyProperty.Register("BackgroundLineColor", typeof(Color), typeof(LineView), new PropertyMetadata(Colors.White));

        public Color BottomTextColor
        {
            get { return (Color)GetValue(BottomTextColorProperty); }
            set { SetValue(BottomTextColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register("BottomTextColor", typeof(Color), typeof(LineView), new PropertyMetadata(Colors.White));

        public bool DrawGridLines { get; set; }
        public bool DrawDotLine { get; set; }
        public bool DrawDotPoints { get; set; }
        public bool DrawGraphBackground { get; set; }
        public bool DrawIconLabels { get; set; }
        public bool DrawDataLabels { get; set; }
        public bool DrawSeriesLabels { get; set; }

        private float LineStrokeWidth;

        public LineView() : base()
        {
            this.DefaultStyleKey = typeof(LineView);

            yCoordinateList = new List<float>();
            drawDotLists = new List<List<Dot>>();

            ResetData(false);

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

        private void OnDependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
        {
            if (property == FontSizeProperty)
            {
                this.BottomTextFormat.FontSize = (float)FontSize;
                if (ReadyToDraw)
                {
                    UpdateGraph();
                }
            }
            else if (property == FontWeightProperty)
            {
                this.BottomTextFormat.FontWeight = FontWeight;
                if (ReadyToDraw)
                {
                    UpdateGraph();
                }
            }
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

        private float GraphBottom
        {
            get
            {
                float graphBottom = ViewHeight;

                graphBottom -= (bottomTextTopMargin + bottomTextHeight + bottomTextDescent);

                if (DrawIconLabels)
                    graphBottom -= (IconHeight + iconBottomMargin);
                else
                    graphBottom -= (LineStrokeWidth + iconBottomMargin);

                return graphBottom;
            }
        }

        public override void ResetData(bool invalidate = false)
        {
            this.yCoordinateList.Clear();
            this.drawDotLists.Clear();
            bottomTextDescent = 0;
            longestTextWidth = 0;
            horizontalGridNum = MIN_HORIZONTAL_GRID_NUM;
            verticalGridNum = MIN_VERTICAL_GRID_NUM;
            base.ResetData(invalidate);
        }

        public override void UpdateGraph()
        {
            bottomTextDescent = 0;
            longestTextWidth = 0;
            verticalGridNum = MIN_VERTICAL_GRID_NUM;

            if (!IsDataEmpty)
            {
                float biggestData = 0;
                float prevLongestTextWidth = longestTextWidth;
                double longestWidth = 0;

                foreach (LineDataSeries series in Data.DataSets)
                {
                    foreach (LineGraphEntry entry in series.EntryData)
                    {
                        if (biggestData < entry.YEntryData.Y)
                        {
                            biggestData = entry.YEntryData.Y;
                        }

                        // Measure Y label
                        var s = entry.YEntryData.YLabel;
                        using (var txtLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0))
                        {
                            if (longestWidth < txtLayout.DrawBounds.Width)
                            {
                                using var textLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0);
                                longestWidth = txtLayout.DrawBounds.Width;
                            }
                        }
                        if (longestTextWidth < longestWidth)
                        {
                            longestTextWidth = (float)longestWidth;
                        }
                        if (sideLineLength < longestWidth / 1.5f)
                        {
                            sideLineLength = (float)longestWidth / 1.5f;
                        }

                        if (entry.XLabel != null)
                        {
                            s = entry.XLabel;
                            using var txtLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0);
                            if (bottomTextHeight < txtLayout.DrawBounds.Height)
                            {
                                bottomTextHeight = (float)txtLayout.DrawBounds.Height;
                            }
                            if (longestWidth < txtLayout.DrawBounds.Width)
                            {
                                longestWidth = txtLayout.DrawBounds.Width;
                            }
                            if (bottomTextDescent < (Math.Abs(txtLayout.DrawBounds.Bottom)))
                            {
                                bottomTextDescent = Math.Abs((float)txtLayout.DrawBounds.Bottom);
                            }
                        }
                    }
                    DataOfAGrid = 1;
                    while (biggestData / 10 > DataOfAGrid)
                    {
                        DataOfAGrid *= 10;
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

                // Add adequate spacing between labels
                longestTextWidth += Canvas.ConvertDipsToPixels(16, CanvasDpiRounding.Floor);
                backgroundGridWidth = longestTextWidth;
            }

            UpdateHorizontalGridNum();
            RefreshXCoordinateList();
            RefreshAfterDataChanged();
            DrawIcons();
            InvalidateMeasure();
        }

        private void RefreshGridWidth()
        {
            // Reset the grid width
            backgroundGridWidth = longestTextWidth;

            if (GetGraphExtentWidth() < ScrollViewer.Width)
            {
                float freeSpace = (float)(ScrollViewer.Width - GetGraphExtentWidth());
                float additionalSpace = freeSpace / MaxEntryCount;
                if (additionalSpace > 0)
                {
                    backgroundGridWidth += additionalSpace;
                }
            }
            RefreshXCoordinateList();
        }

        private void RefreshAfterDataChanged()
        {
            UpdateVerticalGridNum(Data?.DataSets);
            RefreshYCoordinateList();
            RefreshDrawDotList();
        }

        private void UpdateVerticalGridNum(List<LineDataSeries> dataSeriesList)
        {
            if (dataSeriesList != null && dataSeriesList.Count > 0)
            {
                foreach (LineDataSeries series in dataSeriesList)
                {
                    foreach (LineGraphEntry entry in series.EntryData)
                    {
                        verticalGridNum = Math.Max(verticalGridNum,
                            Math.Max(MIN_VERTICAL_GRID_NUM, (int) Math.Ceiling(entry.YEntryData.Y) + 1));
                    }
                }
            }
            else
            {
                verticalGridNum = MIN_VERTICAL_GRID_NUM;
            }
        }

        private void RefreshXCoordinateList()
        {
            xCoordinateList.Clear();
            xCoordinateList.EnsureCapacity(MaxEntryCount);
            for (int i = 0; i < (MaxEntryCount + 1); i++)
            {
                xCoordinateList.Add(sideLineLength + backgroundGridWidth * i);
            }
        }

        private void RefreshYCoordinateList()
        {
            yCoordinateList.Clear();
            yCoordinateList.EnsureCapacity(verticalGridNum);
            for (int i = 0; i < (verticalGridNum + 1); i++)
            {
                /*
                 * Scaling formula
                 *
                 * ((value - minValue) / (maxValue - minValue)) * (scaleMax - scaleMin) + scaleMin
                 * minValue = 0; maxValue = verticalGridNum; value = i
                 */
                yCoordinateList.Add(((float)i / (verticalGridNum)) * (GraphBottom - GraphTop) + GraphTop);
            }
        }

        private void RefreshDrawDotList()
        {
            if (!IsDataEmpty)
            {
                if (drawDotLists.Count == 0 || drawDotLists.Count != Data.DataCount)
                {
                    drawDotLists.Clear();
                    for (int k = 0; k < Data.DataCount; k++)
                    {
                        drawDotLists.Add(new List<Dot>());
                    }
                }

                float maxValue = Data.YMax;
                float minValue = Data.YMin;

                float graphBottom = GraphBottom;
                float graphTop = GraphTop;

                for (int k = 0; k < Data.DataCount; k++)
                {
                    int drawDotSize = drawDotLists[k].Count;

                    LineDataSeries series = Data.GetDataSetByIndex(k);

                    if (drawDotSize > 0)
                    {
                        drawDotLists[k].EnsureCapacity(series.DataCount);
                    }

                    for (int i = 0; i < series.DataCount; i++)
                    {
                        float x = xCoordinateList[i];
                        float y;
                        if (maxValue == minValue)
                        {
                            if (maxValue == 0)
                            {
                                y = graphBottom;
                            }
                            else if (maxValue == 100)
                            {
                                y = graphTop;
                            }
                            else
                            {
                                y = (graphBottom - graphTop) / 2f;
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
                            y = ((series.GetEntryForIndex(i).YEntryData.Y - minValue) / (maxValue - minValue)) * (graphTop - graphBottom) + graphBottom;
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

                    int temp = drawDotLists[k].Count - series.DataCount;
                    for (int i = 0; i < temp; i++)
                    {
                        drawDotLists[k].RemoveAt(drawDotLists[k].Count - 1);
                    }
                }
            }
        }

        protected override void OnCreateCanvasResources(CanvasVirtualControl canvas)
        {
            // Calculate icon height
            IconHeight = canvas.ConvertDipsToPixels(36, CanvasDpiRounding.Floor);

            iconBottomMargin = canvas.ConvertDipsToPixels(4, CanvasDpiRounding.Floor);
            bottomTextTopMargin = canvas.ConvertDipsToPixels(6, CanvasDpiRounding.Floor);
            DOT_INNER_CIR_RADIUS = canvas.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
            DOT_OUTER_CIR_RADIUS = canvas.ConvertDipsToPixels(5, CanvasDpiRounding.Floor);

            backgroundGridWidth = canvas.ConvertDipsToPixels(45, CanvasDpiRounding.Floor);

            LineStrokeWidth = canvas.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
        }

        protected override void OnCanvasRegionsInvalidated(CanvasVirtualControl canvas, CanvasRegionsInvalidatedEventArgs args)
        {
            // Draw the effect to whatever regions of the CanvasVirtualControl have been invalidated.
            foreach (var InvalidatedRegion in args.InvalidatedRegions)
            {
                if (Data != null)
                {
                    using var drawingSession = canvas.CreateDrawingSession(InvalidatedRegion);
                    DrawBackgroundLines(InvalidatedRegion, drawingSession);
                    DrawText(InvalidatedRegion, drawingSession);
                    DrawLines(InvalidatedRegion, drawingSession);
                    DrawDots(InvalidatedRegion, drawingSession);
                    DrawSeriesLegend(InvalidatedRegion, drawingSession);
                }
            }
        }

        // TODO: move to common
        private void DrawIcons()
        {
            if (DrawIconLabels)
            {
                IconCanvas.Children.Clear();
                if (!IsDataEmpty && Data.DataLabels.Count > 0)
                {
                    for (int i = 0; i < Data.DataLabels.Count; i++)
                    {
                        var entry = Data.DataLabels[i];
                        var control = CreateIconControl(entry.XIcon);
                        control.RenderTransform = new RotateTransform()
                        {
                            Angle = entry.XIconRotation,
                            CenterX = IconHeight / 2,
                            CenterY = IconHeight / 2
                        };
                        Windows.UI.Xaml.Controls.Canvas.SetLeft(control, xCoordinateList[i] - IconHeight / 2);
                        Windows.UI.Xaml.Controls.Canvas.SetTop(control, ViewHeight - IconHeight * 1.5 - bottomTextTopMargin);
                        IconCanvas.Children.Add(control);
                    }
                }
            }
        }

        // TODO: move to common
        private void RepositionIcons()
        {
            if (DrawIconLabels)
            {
                for (int i = 0; i < IconCanvas.Children.Count; i++)
                {
                    var control = IconCanvas.Children[i];
                    Windows.UI.Xaml.Controls.Canvas.SetLeft(control, xCoordinateList[i] - IconHeight / 2);
                    Windows.UI.Xaml.Controls.Canvas.SetTop(control, ViewHeight - IconHeight * 1.5 - bottomTextTopMargin);
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
                        var series = Data.GetDataSetByIndex(k);
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

                for (int k = 0; k < drawDotLists.Count; k++)
                {
                    var series = Data.GetDataSetByIndex(k);
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
                        YEntryData entry = series.GetEntryForIndex(i).YEntryData;
                        YEntryData nextEntry = series.GetEntryForIndex(i + 1).YEntryData;

                        float startX = dot.X;
                        float startY = dot.Y;
                        float endX = nextDot.X;
                        float endY = nextDot.Y;

                        drawingRect.Set(region.Left, dot.Y, dot.X, dot.Y);
                        if (firstX == -1 && drawingRect.Intersects(region))
                        {
                            drawingSession.DrawLine((float)region.Left, dot.Y, dot.X, dot.Y, lineColor, LineStrokeWidth);
                        }

                        drawingRect.Set(dot.X, dot.Y, nextDot.X, nextDot.Y);
                        if (drawingRect.Intersects(region))
                        {
                            drawingSession.DrawLine(dot.X, dot.Y, nextDot.X, nextDot.Y, lineColor, LineStrokeWidth);
                        }

                        if (DrawDataLabels)
                        {
                            // Draw top label
                            var txtLayout = new CanvasTextLayout(drawingSession, entry.YLabel, BottomTextFormat, 0, 0);
                            drawingRect.Set(dot.X, dot.Y, dot.X + txtLayout.DrawBounds.Width, dot.Y + bottomTextHeight);

                            if (drawingRect.Intersects(region))
                            {
                                textEntries.Add(new TextEntry(txtLayout, dot.X, dot.Y - bottomTextHeight - bottomTextDescent));
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
                                var txtLayout = new CanvasTextLayout(drawingSession, nextEntry.YLabel, BottomTextFormat, 0, 0);
                                drawingRect.Set(nextDot.X, nextDot.Y, nextDot.X + txtLayout.DrawBounds.Width, nextDot.Y + bottomTextHeight);

                                if (drawingRect.Intersects(region))
                                {
                                    textEntries.Add(new TextEntry(txtLayout, nextDot.X, nextDot.Y - bottomTextHeight - bottomTextDescent));
                                }
                                else
                                {
                                    txtLayout.Dispose();
                                }
                            }

                            drawingRect.Set(nextDot.X, nextDot.Y, region.Right, nextDot.Y);
                            if (drawingRect.Intersects(region))
                            {
                                drawingSession.DrawLine(nextDot.X, nextDot.Y, (float)region.Right, nextDot.Y, lineColor, LineStrokeWidth);
                            }

                            currentDot = nextDot;

                            if (drawingRect.Intersects(region))
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
                    drawingRect.Set(x, GraphTop, x, GraphBottom);
                    if (drawingRect.Intersects(region))
                        drawingSession.DrawLine(x, GraphTop, x, GraphBottom, BackgroundLineColor);
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
        }

        private void DrawText(Rect region, CanvasDrawingSession drawingSession)
        {
            // Draw Bottom Text
            if (!IsDataEmpty)
            {
                List<LineGraphEntry> dataLabels = Data.DataLabels;
                for (int i = 0; i < dataLabels.Count; i++)
                {
                    float x = xCoordinateList[i];
                    float y = ViewHeight - bottomTextDescent;
                    GraphEntry entry = dataLabels[i];

                    if (!String.IsNullOrWhiteSpace(entry.XLabel))
                    {
                        using var btmTxtLayout = new CanvasTextLayout(drawingSession, entry.XLabel, BottomTextFormat, 0, 0);
                        drawingRect.Set(x, y, x + btmTxtLayout.DrawBounds.Width, y);

                        if (drawingRect.Intersects(region))
                            drawingSession.DrawTextLayout(btmTxtLayout, x, y, BottomTextColor);
                    }
                }
            }
        }

        private void DrawSeriesLegend(Rect region, CanvasDrawingSession drawingSession)
        {
            if (DrawSeriesLabels && !IsDataEmpty)
            {
                int seriesSize = Data.DataCount;

                double longestWidth = 0;
                String longestStr = "";
                float textWidth = 0;
                float paddingLength = 0;
                float textHeight = 0;
                float textDescent = 0;
                for (int i = 0; i < seriesSize; i++)
                {
                    String title = Data.GetDataSetByIndex(i).SeriesLabel;
                    if (String.IsNullOrWhiteSpace(title))
                    {
                        title = String.Format(CultureUtils.UserCulture, "{0} {1}", App.ResLoader.GetString("label_series"), i);
                    }

                    using (var txtLayout = new CanvasTextLayout(Canvas, title, BottomTextFormat, 0, 0))
                    {
                        if (textHeight < txtLayout.DrawBounds.Height)
                        {
                            textHeight = (float)txtLayout.DrawBounds.Height;
                        }
                        if (longestWidth < txtLayout.DrawBounds.Width)
                        {
                            longestWidth = txtLayout.DrawBounds.Width;
                            longestStr = title;
                        }
                        if (textDescent < (Math.Abs(txtLayout.DrawBounds.Bottom)))
                        {
                            textDescent = Math.Abs((float)txtLayout.DrawBounds.Bottom);
                        }
                    }
                }

                if (textWidth < longestWidth)
                {
                    using var textLayout = new CanvasTextLayout(Canvas, longestStr.Substring(0, 1), BottomTextFormat, 0, 0);
                    textWidth = (float)longestWidth + (float)textLayout.DrawBounds.Width;
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
                    var series = Data.GetDataSetByIndex(i);
                    var seriesColor = series.GetColor(i);
                    var title = series.SeriesLabel;
                    if (String.IsNullOrWhiteSpace(title))
                    {
                        title = String.Format("{0} {1}", App.ResLoader.GetString("label_series"), i);
                    }

                    using var txtLayout = new CanvasTextLayout(drawingSession, title, BottomTextFormat, 0, 0);
                    float xRectStart = paddingLength + textWidth * i + ((rectSize + rect2TextPadding) * i);
                    float xTextStart = paddingLength + textWidth * i + rectSize + ((rectSize + rect2TextPadding) * i);

                    Rect rect = RectHelper.FromPoints(
                            new Point(xRectStart, 0),
                            new Point(xRectStart + rectSize, rectSize));
                    Rect txtRect = RectHelper.FromPoints(
                            new Point(xTextStart + rect2TextPadding, 0),
                            new Point(xTextStart + rect2TextPadding + textWidth, textHeight));

                    drawingSession.FillRectangle(rect, seriesColor);
                    drawingSession.DrawTextLayout(txtLayout,
                        xTextStart + rect2TextPadding + (float)(txtLayout.DrawBounds.Width / 2),
                        (float)(-txtLayout.DrawBounds.Top / 2),
                        BottomTextColor);
                }
            }
        }

        protected override float GetGraphExtentWidth()
        {
            return longestTextWidth * MaxEntryCount;
        }

        protected override float GetPreferredWidth()
        {
            return backgroundGridWidth * MaxEntryCount;
        }

        protected override void OnPreMeasure()
        {
            RefreshGridWidth();
        }

        protected override void OnPostMeasure()
        {
            // Redraw View
            RefreshAfterDataChanged();
            Canvas.Invalidate();
            RepositionIcons();
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
        }
    }
}