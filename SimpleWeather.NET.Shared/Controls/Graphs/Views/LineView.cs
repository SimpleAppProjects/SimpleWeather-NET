#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.NET.Helpers;
using Windows.UI;
#else
using SimpleWeather.Maui.Helpers;
#endif
using SimpleWeather.NET.Utils;
using SimpleWeather.SkiaSharp;
using SimpleWeather.Utils;
using SkiaSharp;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET.Controls.Graphs
{
    /*
     *  Multi-series line graph
     *  Based on Android implementation of LineView: https://github.com/SimpleAppProjects/SimpleWeather-Android
     *  Which is:
     *  Based on LineView from http://www.androidtrainee.com/draw-android-line-chart-with-animation/
     *  Graph background (under line) based on - https://github.com/jjoe64/GraphView (LineGraphSeries)
     */

    public sealed partial class LineView : BaseGraphView<LineViewData, LineDataSeries, LineGraphEntry>
    {
        private bool disposedValue;

        private float drwTextWidth;
        private int DataOfAGrid = 10;

        private List<float> yCoordinateList;

        private List<List<Dot>> drawDotLists; // Y data

        private float DOT_INNER_CIR_RADIUS;
        private float DOT_OUTER_CIR_RADIUS;
        private float LINE_CORNER_RADIUS;
        private const int MIN_VERTICAL_GRID_NUM = 4;

        public Color BackgroundLineColor
        {
            get => (Color)GetValue(BackgroundLineColorProperty);
            set => SetValue(BackgroundLineColorProperty, value);
        }

#if WINDOWS
        // Using a DependencyProperty as the backing store for BackgroundLineColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundLineColorProperty =
            DependencyProperty.Register(nameof(BackgroundLineColor), typeof(Color), typeof(LineView), new PropertyMetadata(Colors.White, OnBackgroundLineColorChanged));
#else
        // Using a DependencyProperty as the backing store for BackgroundLineColor.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty BackgroundLineColorProperty =
            BindableProperty.Create(nameof(BackgroundLineColor), typeof(Color), typeof(LineView), Colors.White, propertyChanged: OnBackgroundLineColorChanged);
#endif

        public bool DrawGridLines { get; set; }
        public bool DrawDotLine { get; set; }
        public bool DrawDotPoints { get; set; }
        public bool DrawGraphBackground { get; set; }
        public bool DrawSeriesLabels { get; set; }

        private readonly SKPaint bigCirPaint;
        private readonly SKPaint smallCirPaint;
        private readonly SKPaint linePaint;
        private readonly SKPath linePath;
        private readonly SKPath pathBackground;
        private readonly SKPaint paintBackground;
        private readonly SKPaint bgLinesPaint;
        private readonly SKPathEffect dashEffects;
        private readonly SKPaint seriesRectPaint;

        public LineView() : base()
        {
#if WINDOWS
            this.DefaultStyleKey = typeof(LineView);
#endif

            bigCirPaint = new SKPaint();
            yCoordinateList = new List<float>();
            drawDotLists = new List<List<Dot>>();

            ResetData(false);

            bigCirPaint.IsAntialias = true;
            smallCirPaint = bigCirPaint.Clone();

            linePaint = new SKPaint()
            {
                IsAntialias = true,
                IsDither = true,
                PathEffect = SKPathEffect.CreateCorner(LINE_CORNER_RADIUS),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f, // 2dp
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };
            linePath = new SKPath();

            pathBackground = new SKPath();
            paintBackground = new SKPaint()
            {
                IsAntialias = true,
                IsDither = true,
                PathEffect = SKPathEffect.CreateCorner(LINE_CORNER_RADIUS),
                Style = SKPaintStyle.StrokeAndFill
            };

            bgLinesPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1f, // 1dp
                Color = BackgroundLineColor.ToSKColor(),
            };
            dashEffects = SKPathEffect.CreateDash(new float[] { 10, 5, 10, 5 }, 1);

            seriesRectPaint = new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
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
                    graphBottom -= (linePaint.StrokeWidth + iconBottomMargin);

                return graphBottom;
            }
        }

#if WINDOWS
        private static void OnBackgroundLineColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
#else
        private static void OnBackgroundLineColorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue != oldValue)
#endif
            {
                (obj as LineView)?.UpdateBackgroundLineColor();
            }
        }

        private void UpdateBackgroundLineColor()
        {
            bgLinesPaint.Color = BackgroundLineColor.ToSKColor();
#if WINDOWS
            Canvas?.Invalidate();
#else
            Canvas?.InvalidateSurface();
#endif
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
                var r = new SKRect();
                float biggestData = 0;

                float longestWidth = 0;

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
                        bottomTextPaint.MeasureText(s, ref r);
                        if (longestWidth < r.Width)
                        {
                            longestWidth = r.Width;
                        }
                        if (longestTextWidth < longestWidth)
                        {
                            longestTextWidth = longestWidth;
                        }
                        if (sideLineLength < longestWidth / 1.5f)
                        {
                            sideLineLength = longestWidth / 1.5f;
                        }

                        if (entry.XLabel != null)
                        {
                            s = entry.XLabel;
                            bottomTextPaint.MeasureText(s, ref r);
                            if (bottomTextHeight < r.Height)
                            {
                                bottomTextHeight = r.Height;
                            }
                            if (longestWidth < r.Width)
                            {
                                longestWidth = r.Width;
                            }
                            if (bottomTextDescent < Math.Abs(r.Bottom))
                            {
                                bottomTextDescent = Math.Abs(r.Bottom);
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
                    longestTextWidth = longestWidth;
                }
                if (sideLineLength < longestWidth / 1.5f)
                {
                    sideLineLength = longestWidth / 1.5f;
                }

                // Add adequate spacing between labels
                longestTextWidth += 16f; // 16dp
                backgroundGridWidth = longestTextWidth;
            }
            else
            {
                bottomTextDescent = 0;
                longestTextWidth = 0;
                verticalGridNum = MIN_VERTICAL_GRID_NUM;
            }

            UpdateHorizontalGridNum();
            RefreshXCoordinateList();
            RefreshAfterDataChanged();
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
#if WINDOWS
                if (HorizontalAlignment == HorizontalAlignment.Stretch)
#else
                if (HorizontalOptions.Alignment == LayoutAlignment.Fill)
#endif
                {
                    if (additionalSpace > 0)
                    {
                        backgroundGridWidth += additionalSpace;
                    }
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
                            Math.Max(MIN_VERTICAL_GRID_NUM, (int)Math.Ceiling(entry.YEntryData.Y) + 1));
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

        protected override void OnDraw(SKCanvas canvas)
        {
            if (visibleRect.IsEmpty)
            {
#if WINDOWS
                visibleRect.Set(
                    (float)ScrollViewer.HorizontalOffset,
                    (float)ScrollViewer.VerticalOffset,
                    (float)(ScrollViewer.HorizontalOffset + ScrollViewer.ActualWidth),
                    (float)(ScrollViewer.VerticalOffset + ScrollViewer.ActualHeight)
                );
#else
                visibleRect.Set(
                    (float)ScrollViewer.ScrollX,
                    (float)ScrollViewer.ScrollY,
                    (float)(ScrollViewer.ScrollX + ScrollViewer.Width),
                    (float)(ScrollViewer.ScrollY + ScrollViewer.Height)
                );
#endif
            }

            if (!IsDataEmpty)
            {
                DrawBackgroundLines(canvas);
                DrawTextAndIcons(canvas);
                DrawLines(canvas);
                DrawDots(canvas);
                DrawSeriesLegend(canvas);
            }
        }

        private void DrawDots(SKCanvas canvas)
        {
            if (DrawDotPoints)
            {
                if (drawDotLists != null && drawDotLists.Count > 0)
                {
                    for (int k = 0; k < drawDotLists.Count; k++)
                    {
                        var series = Data.GetDataSetByIndex(k);
                        var color = series.GetColor(k);

                        bigCirPaint.Color = color.ToSKColor();
                        smallCirPaint.Color = ColorUtils.SetAlphaComponent(color, 0x99).ToSKColor();

                        foreach (Dot dot in drawDotLists[k])
                        {
                            // Draw Dots
                            if (visibleRect.Contains(dot.X, dot.Y))
                            {
                                canvas.DrawCircle(dot.X, dot.Y, DOT_OUTER_CIR_RADIUS, bigCirPaint);
                                canvas.DrawCircle(dot.X, dot.Y, DOT_INNER_CIR_RADIUS, smallCirPaint);
                            }
                        }
                    }
                }
            }
        }

        private void DrawLines(SKCanvas canvas)
        {
            if (drawDotLists.Any())
            {
                float graphHeight = GraphHeight;

                for (int k = 0; k < drawDotLists.Count; k++)
                {
                    var series = Data.GetDataSetByIndex(k);
                    ICollection<TextEntry> textEntries = new LinkedList<TextEntry>();

                    float firstX = -1;
                    // needed to end the path for background
                    Dot currentDot = null;

                    linePath.Rewind();
                    pathBackground.Rewind();
                    linePaint.Color = series.GetColor(k).ToSKColor();
                    paintBackground.Color = ColorUtils.SetAlphaComponent(series.GetColor(k), 0x99).ToSKColor();

                    linePath.MoveTo((float)(visibleRect.Left - LINE_CORNER_RADIUS), graphHeight);
                    if (DrawGraphBackground)
                    {
                        pathBackground.MoveTo(visibleRect.Left - LINE_CORNER_RADIUS, graphHeight);
                    }

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

                        if (DrawDataLabels)
                        {
                            // Draw top label
                            drwTextWidth = bottomTextPaint.MeasureText(entry.YLabel);
                            drawingRect.Set(dot.X, dot.Y, dot.X + drwTextWidth, dot.Y + bottomTextHeight);
                            if (drawingRect.Intersects(visibleRect))
                            {
                                textEntries.Add(new TextEntry(entry.YLabel, dot.X, dot.Y - bottomTextHeight - bottomTextDescent));
                            }
                        }

                        if (firstX == -1)
                        {
                            firstX = visibleRect.Left;

                            linePath.MoveTo(firstX - LINE_CORNER_RADIUS, startY);
                            if (DrawGraphBackground)
                            {
                                pathBackground.LineTo(firstX - LINE_CORNER_RADIUS, startY);
                            }
                        }

                        linePath.LineTo(startX, startY);
                        if (DrawGraphBackground)
                        {
                            pathBackground.LineTo(startX, startY);
                        }

                        currentDot = dot;

                        // Draw last items
                        if (i + 1 == drawDotLists[k].Count - 1)
                        {
                            if (DrawDataLabels)
                            {
                                // Draw top label
                                drwTextWidth = bottomTextPaint.MeasureText(nextEntry.YLabel);
                                drawingRect.Set(nextDot.X, nextDot.Y, nextDot.X + drwTextWidth, nextDot.Y + bottomTextHeight);

                                if (drawingRect.Intersects(visibleRect))
                                {
                                    textEntries.Add(new TextEntry(nextEntry.YLabel, nextDot.X, nextDot.Y - bottomTextHeight - bottomTextDescent));
                                }
                            }

                            currentDot = nextDot;

                            linePath.LineTo(endX, endY);
                            if (DrawGraphBackground)
                            {
                                pathBackground.LineTo(endX, endY);
                            }
                        }
                    }

                    if (currentDot != null)
                    {
                        linePath.LineTo(visibleRect.Right + LINE_CORNER_RADIUS, currentDot.Y);
                    }

                    canvas.DrawPath(linePath, linePaint);

                    if (DrawGraphBackground)
                    {
                        if (currentDot != null)
                        {
                            pathBackground.LineTo(visibleRect.Right + LINE_CORNER_RADIUS, currentDot.Y);
                        }
                        if (firstX != -1)
                        {
                            pathBackground.LineTo(visibleRect.Right + LINE_CORNER_RADIUS, graphHeight);
                        }

                        pathBackground.Close();

                        canvas.DrawPath(pathBackground, paintBackground);
                    }

                    if (DrawDataLabels)
                    {
                        foreach (var entry in textEntries)
                        {
                            canvas.DrawText(entry.Text, entry.X, entry.Y, bottomTextFont, bottomTextPaint);
                        }
                    }
                }
            }
        }

        private void DrawBackgroundLines(SKCanvas canvas)
        {
            if (DrawGridLines)
            {
                // Draw vertical lines
                for (int i = 0; i < xCoordinateList.Count; i++)
                {
                    float x = xCoordinateList[i];
                    drawingRect.Set(x, GraphTop, x, GraphBottom);

                    if (drawingRect.Intersects(visibleRect))
                    {
                        canvas.DrawLine(x, GraphTop, x, GraphBottom, bgLinesPaint);
                    }
                }

                // draw dotted lines
                bgLinesPaint.PathEffect = DrawDotLine ? dashEffects : null;

                // Draw horizontal lines
                for (int i = 0; i < yCoordinateList.Count; i++)
                {
                    if ((yCoordinateList.Count - 1 - i) % DataOfAGrid == 0)
                    {
                        float y = yCoordinateList[i];

                        if (y <= visibleRect.Bottom && y >= visibleRect.Top)
                            canvas.DrawLine(visibleRect.Left, y, visibleRect.Right, y, bgLinesPaint);
                    }
                }
            }
        }

        private void DrawTextAndIcons(SKCanvas canvas)
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

                    if (!string.IsNullOrWhiteSpace(entry.XLabel))
                    {
                        var drwTextRect = new SKRect();
                        float drwTextWidth = bottomTextPaint.MeasureText(entry.XLabel, ref drwTextRect);
                        drawingRect.X = x - drwTextWidth / 2;
                        drawingRect.Y = y - drwTextRect.Height / 2;

                        if (drawingRect.Intersects(visibleRect))
                            canvas.DrawText(entry.XLabel, x, y, bottomTextFont, bottomTextPaint);
                    }

                    if (DrawIconLabels && entry.XIcon != null)
                    {
                        var rotation = entry.XIconRotation;

                        var bounds = new SKRect(0, 0, IconHeight, IconHeight);
                        entry.XIcon.Bounds = bounds;
                        drawingRect.Set(x - bounds.Width / 2, y - bounds.Height / 2, x + bounds.Width / 2, y + bounds.Height / 2);

                        if (drawingRect.Intersects(visibleRect))
                        {
                            if (entry.XIcon is SKLottieDrawable animatable)
                            {
                                AddAnimatedDrawable(animatable);
                            }

                            canvas.Save();
                            canvas.Translate(x - bounds.Width / 2f, y - bounds.Height - bottomTextHeight - bottomTextDescent - iconBottomMargin);
                            canvas.RotateDegrees(rotation, bounds.Width / 2f, bounds.Height / 2f);
                            entry.XIcon.Draw(canvas);
                            canvas.Restore();
                        }
                    }
                }
            }
        }

        private void DrawSeriesLegend(SKCanvas canvas)
        {
            if (DrawSeriesLabels && !IsDataEmpty)
            {
                int seriesSize = Data.DataCount;

                var r = new SKRect();
                float longestWidth = 0;
                string longestStr = "";
                float textWidth = 0;
                float paddingLength = 0;
                float textHeight = 0;
                float textDescent = 0;
                for (int i = 0; i < seriesSize; i++)
                {
                    String title = Data.GetDataSetByIndex(i).SeriesLabel;
                    if (String.IsNullOrWhiteSpace(title))
                    {
                        title = String.Format(CultureUtils.UserCulture, "{0} {1}", ResStrings.label_series, i);
                    }

                    bottomTextPaint.MeasureText(title, ref r);
                    if (textHeight < r.Height)
                    {
                        textHeight = r.Height;
                    }
                    if (longestWidth < r.Width)
                    {
                        longestWidth = r.Width;
                        longestStr = title;
                    }
                    if (textDescent < (Math.Abs(r.Bottom)))
                    {
                        textDescent = Math.Abs(r.Bottom);
                    }
                }

                if (textWidth < longestWidth)
                {
                    textWidth = longestWidth + bottomTextPaint.MeasureText(longestStr.Substring(0, 1));
                }
                if (paddingLength < longestWidth / 2)
                {
                    paddingLength = longestWidth / 2f;
                }
                textWidth += 4f; // 4dp

                float rectSize = textHeight;
                float rect2TextPadding = 8f; // 8dp

                for (int i = 0; i < seriesSize; i++)
                {
                    var series = Data.GetDataSetByIndex(i);
                    var seriesColor = series.GetColor(i);
                    var title = series.SeriesLabel;
                    if (String.IsNullOrWhiteSpace(title))
                    {
                        title = String.Format("{0} {1}", ResStrings.label_series, i);
                    }

                    var textBounds = new SKRect();
                    bottomTextPaint.MeasureText(title, ref textBounds);

                    float xRectStart = paddingLength + textWidth * i + ((rectSize + rect2TextPadding) * i);
                    float xTextStart = paddingLength + textWidth * i + rectSize + ((rectSize + rect2TextPadding) * i);

                    var rectF = new SKRect(xRectStart, bottomTextTopMargin + textDescent, xRectStart + rectSize, rectSize + bottomTextTopMargin + textDescent);
                    seriesRectPaint.Color = seriesColor.ToSKColor();

                    canvas.DrawRect(rectF, seriesRectPaint);
                    canvas.DrawText(title, xTextStart + textWidth / 2f, textHeight + bottomTextTopMargin + textDescent, bottomTextFont, bottomTextPaint);
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
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposedValue)
            {
                if (disposing)
                {
                    bigCirPaint?.Dispose();
                    smallCirPaint?.Dispose();
                    linePaint?.Dispose();
                    linePath?.Dispose();
                    pathBackground?.Dispose();
                    paintBackground?.Dispose();
                    bgLinesPaint?.Dispose();
                    dashEffects?.Dispose();
                    seriesRectPaint?.Dispose();
                }

                disposedValue = true;
            }
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

            public override string ToString()
            {
                return "Dot{" + "x=" + X + ", y=" + Y + '}';
            }
        }

        internal class TextEntry
        {
            public string Text { get; set; }
            public float X { get; set; }
            public float Y { get; set; }

            public TextEntry(string text, float x, float y)
            {
                this.Text = text;
                this.X = x;
                this.Y = y;
            }
        }
    }
}