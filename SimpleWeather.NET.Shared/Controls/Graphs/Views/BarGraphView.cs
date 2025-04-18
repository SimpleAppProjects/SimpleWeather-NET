﻿#if WINDOWS
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.NET.Helpers;
using Windows.Foundation;
using ScrollView = SimpleWeather.NET.Controls.Graphs.GraphScrollView;
#else
using SimpleWeather.Maui.Helpers;
using ScrollView = Microsoft.Maui.Controls.ScrollView;
#endif
using SimpleWeather.SkiaSharp;
using SimpleWeather.NET.Utils;
using SkiaSharp;

namespace SimpleWeather.NET.Controls.Graphs
{
    public partial class BarGraphView : BaseGraphScrollView<BarGraphData, BarGraphDataSet, BarGraphEntry>
    {
        public override BaseGraphView<BarGraphData, BarGraphDataSet, BarGraphEntry> CreateGraphView()
        {
            return new BarChartGraph(ScrollViewer);
        }

        internal override BarChartGraph Graph => (BarChartGraph)base.Graph;

        internal sealed partial class BarChartGraph : BaseGraphView<BarGraphData, BarGraphDataSet, BarGraphEntry>
        {
            private bool disposedValue;

            private readonly List<Bar> drawDotLists; // Y data

            private readonly SKPaint linePaint;

            public BarChartGraph(ScrollView scrollViewer) : base(scrollViewer)
            {
#if WINDOWS
                //this.Style = this.TryFindResource("BarChartGraphStyle") as Style;
                this.DefaultStyleKey = typeof(BarChartGraph);
#endif

                drawDotLists = new List<Bar>();

                ResetData(false);

                linePaint = new SKPaint()
                {
                    IsAntialias = true,
                    StrokeWidth = 6f, // 6dp
                    StrokeCap = SKStrokeCap.Round
                };
            }

            private float GraphTop
            {
                get
                {
                    float graphTop = (float)Padding.Top;
                    graphTop += bottomTextTopMargin + bottomTextHeight * 2f + bottomTextDescent * 2f;

                    return graphTop;
                }
            }

            private float GraphHeight
            {
                get
                {
                    float graphHeight = ViewHeight - bottomTextTopMargin - bottomTextHeight - bottomTextDescent - linePaint.StrokeWidth;
                    if (DrawIconLabels) graphHeight = graphHeight - IconSize - iconBottomMargin;
                    if (DrawDataLabels)
                        graphHeight = graphHeight - bottomTextTopMargin - linePaint.StrokeWidth;

                    return graphHeight;
                }
            }

            public override void ResetData(bool invalidate = false)
            {
                this.drawDotLists.Clear();
                bottomTextDescent = 0;
                longestTextWidth = 0;
                horizontalGridNum = MIN_HORIZONTAL_GRID_NUM;
                base.ResetData(invalidate);
            }

            public override void UpdateGraph()
            {
                if (!IsDataEmpty)
                {
                    var r = new SKRect();
                    float longestWidth = 0;

                    var set = Data.GetDataSet();
                    foreach (var entry in set.EntryData)
                    {
                        String s = entry.XLabel;
                        if (s != null)
                        {
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

                    if (longestTextWidth < longestWidth)
                    {
                        longestTextWidth = longestWidth;
                    }
                    if (sideLineLength < longestWidth / 1.5f)
                    {
                        sideLineLength = longestWidth / 1.5f;
                    }

                    // Add padding
                    backgroundGridWidth = longestTextWidth + 8f; // 8dp
                }
                else
                {
                    bottomTextDescent = 0;
                    longestTextWidth = 0;
                }

                UpdateHorizontalGridNum();
                RefreshXCoordinateList();
                RefreshDrawDotList();
                Invalidate();
            }

            private void RefreshGridWidth()
            {
                // Reset the grid width
                backgroundGridWidth = longestTextWidth;
                var defaultPadding = 8f; // 8dp
#if WINDOWS
                var parentWidth = Math.Floor(ScrollViewer.MeasuredSize.IsEmpty ? (ScrollViewer.DesiredSize.IsEmpty ? ScrollViewer.Width : ScrollViewer.DesiredSize.Width) : ScrollViewer.MeasuredSize.Width);
#else
                var parentWidth = Math.Floor(ScrollViewer.DesiredSize.IsZero ? ScrollViewer.Width : ScrollViewer.DesiredSize.Width);
#endif

                if (GetGraphExtentWidth() < parentWidth)
                {
                    float freeSpace = (float)(parentWidth - GetGraphExtentWidth());
                    float availableAdditionalSpace = freeSpace / MaxEntryCount;

                    if (FillParentWidth)
                    {
                        if (availableAdditionalSpace > 0)
                        {
                            backgroundGridWidth += availableAdditionalSpace;
                        }
                        else
                        {
                            backgroundGridWidth += defaultPadding;
                        }
                    }
                    else
                    {
                        var requestedPadding = 48f; // 48dp
                        if (availableAdditionalSpace > 0 && requestedPadding < availableAdditionalSpace)
                        {
                            backgroundGridWidth += requestedPadding;
                        }
                        else
                        {
                            backgroundGridWidth += defaultPadding;
                        }
                    }
                }
                else
                {
                    backgroundGridWidth += defaultPadding;
                }
            }

            private void RefreshXCoordinateList()
            {
                xCoordinateList.Clear();
                xCoordinateList.EnsureCapacity(MaxEntryCount);

                for (int i = 0; i < MaxEntryCount; i++)
                {
                    float x = sideLineLength + backgroundGridWidth * i;
                    xCoordinateList.Add(x);
                }
            }

            private void RefreshDrawDotList()
            {
                if (!IsDataEmpty)
                {
                    drawDotLists.Clear();

                    float maxValue = Data.YMax;
                    float minValue = Data.YMin;

                    float graphHeight = GraphHeight;
                    float graphTop = GraphTop;

                    int drawDotSize = drawDotLists.Count;

                    if (drawDotSize > 0)
                    {
                        drawDotLists.EnsureCapacity(Data.GetDataSet().DataCount);
                    }

                    for (int i = 0; i < Data.GetDataSet().DataCount; i++)
                    {
                        var entry = Data.GetDataSet().GetEntryForIndex(i);
                        float x = xCoordinateList[i];
                        float y = 0;

                        /*
                         * Scaling formula
                         *
                         * ((value - minValue) / (maxValue - minValue)) * (scaleMax - scaleMin) + scaleMin
                         * graphTop is scaleMax & graphHeight is scaleMin due to View coordinate system
                         */
                        if (entry.EntryData != null)
                        {
                            y = ((entry.EntryData.Y - minValue) / (maxValue - minValue)) * (graphTop - graphHeight) + graphHeight;
                        }

                        if (i > drawDotSize - 1)
                        {
                            drawDotLists.Add(new Bar(x, y));
                        }
                        else
                        {
                            drawDotLists[i] = new Bar(x, y);
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
                    DrawTextAndIcons(canvas);
                    DrawLines(canvas);
                }
            }

            private void DrawTextAndIcons(SKCanvas canvas)
            {
                // Draw Bottom Text
                if (!IsDataEmpty)
                {
                    var dataLabels = Data.DataLabels;
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

                            var bounds = new SKRect(0, 0, IconSize, IconSize);
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

            private void DrawLines(SKCanvas canvas)
            {
                if (drawDotLists.Any() && !IsDataEmpty)
                {
                    var set = Data.GetDataSet();
                    for (int i = 0; i < drawDotLists.Count; i++)
                    {
                        var bar = drawDotLists[i];
                        var entry = set.GetEntryForIndex(i);

                        linePaint.Color = (entry.FillColor ?? Colors.AliceBlue).ToSKColor();

                        drawingRect.Set(
                            bar.X - linePaint.StrokeWidth / 2f,
                            bar.Y - bottomTextHeight - bottomTextDescent,
                            bar.X + linePaint.StrokeWidth / 2f,
                            GraphHeight
                        );

                        if (drawingRect.Intersects(visibleRect))
                        {
                            canvas.DrawLine(bar.X, drawingRect.Bottom, bar.X, bar.Y, linePaint);

                            if (DrawDataLabels)
                            {
                                if (entry.EntryData != null)
                                {
                                    canvas.DrawText(entry.EntryData.YLabel, bar.X, bar.Y - bottomTextHeight - bottomTextDescent, bottomTextFont, bottomTextPaint);
                                }
                            }
                        }
                    }
                }
            }

            protected override float GetGraphExtentWidth()
            {
                return MathF.Round(longestTextWidth * MaxEntryCount + sideLineLength);
            }

            protected override float GetPreferredWidth()
            {
                if (!xCoordinateList.Any())
                {
                    return MathF.Round(backgroundGridWidth * MaxEntryCount);
                }
                else
                {
                    return MathF.Floor(xCoordinateList.Last() + sideLineLength);
                }
            }

            protected override void OnPreMeasure()
            {
                RefreshGridWidth();
                RefreshXCoordinateList();
            }

            protected override void OnPostMeasure()
            {
                // Redraw View
                RefreshDrawDotList();
            }

            internal class Bar
            {
                public float X { get; set; }
                public float Y { get; set; }

                public Bar(float x, float y)
                {
                    this.X = x;
                    this.Y = y;
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        linePaint?.Dispose();
                    }

                    disposedValue = true;
                }
            }
        }
    }
}