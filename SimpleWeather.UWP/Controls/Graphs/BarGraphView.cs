using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
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
    public sealed partial class BarGraphView : BaseGraphView<BarGraphData, BarGraphDataSet, BarGraphEntry>
    {
        private bool disposedValue;

        private List<Bar> drawDotLists; // Y data

        private float LineStrokeWidth;
        private readonly CanvasStrokeStyle LineStrokeStyle;

        public BarGraphView() : base()
        {
            this.DefaultStyleKey = typeof(BarGraphView);

            drawDotLists = new List<Bar>();

            ResetData(false);

            LineStrokeStyle = new CanvasStrokeStyle()
            {
                StartCap = CanvasCapStyle.Round,
                EndCap = CanvasCapStyle.Round,
                LineJoin = CanvasLineJoin.Round
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
            this.drawDotLists.Clear();
            bottomTextDescent = 0;
            longestTextWidth = 0;
            horizontalGridNum = MIN_HORIZONTAL_GRID_NUM;
            //DrawIconLabels = false;
            base.ResetData(invalidate);
        }

        public override void UpdateGraph()
        {
            bottomTextDescent = 0;
            longestTextWidth = 0;
            
            if (!IsDataEmpty)
            {
                double longestWidth = 0;

                var set = Data.GetDataSet();
                foreach (var entry in set.EntryData)
                {
                    String s = entry.XLabel;
                    if (s != null)
                    {
                        using var txtLayout = new CanvasTextLayout(Canvas, s, BottomTextFormat, 0, 0);
                        if (bottomTextHeight < txtLayout.DrawBounds.Height)
                        {
                            bottomTextHeight = (float)txtLayout.DrawBounds.Height;
                        }
                        if (longestWidth < txtLayout.DrawBounds.Width)
                        {
                            longestWidth = txtLayout.DrawBounds.Width;
                        }
                        if (bottomTextDescent < Math.Abs(txtLayout.DrawBounds.Bottom))
                        {
                            bottomTextDescent = Math.Abs((float)txtLayout.DrawBounds.Bottom);
                        }
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

                // Add padding
                backgroundGridWidth = longestTextWidth + Canvas.ConvertDipsToPixels(8, CanvasDpiRounding.Floor);
            }

            UpdateHorizontalGridNum();
            RefreshXCoordinateList();
            RefreshDrawDotList();
            DrawIcons();
            InvalidateMeasure();
        }

        private void RefreshGridWidth()
        {
            // Reset the grid width
            backgroundGridWidth = longestTextWidth;
            var defaultPadding = Canvas.ConvertDipsToPixels(8, CanvasDpiRounding.Floor);

            if (GetGraphExtentWidth() < ScrollViewer.Width)
            {
                float freeSpace = (float)(ScrollViewer.Width - GetGraphExtentWidth());
                float availableAdditionalSpace = freeSpace / MaxEntryCount;
                if (HorizontalAlignment == HorizontalAlignment.Stretch)
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
                    var requestedPadding = Canvas.ConvertDipsToPixels(48, CanvasDpiRounding.Floor);
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

                float graphBottom = GraphBottom;
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
                        y = ((entry.EntryData.Y - minValue) / (maxValue - minValue)) * (graphTop - graphBottom) + graphBottom;
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

        protected override void OnCreateCanvasResources(CanvasVirtualControl canvas)
        {
            // Calculate icon height
            IconHeight = canvas.ConvertDipsToPixels(36, CanvasDpiRounding.Floor);

            iconBottomMargin = canvas.ConvertDipsToPixels(4, CanvasDpiRounding.Floor);
            bottomTextTopMargin = canvas.ConvertDipsToPixels(6, CanvasDpiRounding.Floor);

            backgroundGridWidth = canvas.ConvertDipsToPixels(45, CanvasDpiRounding.Floor);

            LineStrokeWidth = canvas.ConvertDipsToPixels(6, CanvasDpiRounding.Floor);

            UpdateGraph();
        }

        protected override void OnCanvasRegionsInvalidated(CanvasVirtualControl canvas, CanvasRegionsInvalidatedEventArgs args)
        {
            // Draw the effect to whatever regions of the CanvasVirtualControl have been invalidated.
            foreach (var InvalidatedRegion in args.InvalidatedRegions)
            {
                if (Data != null)
                {
                    using var drawingSession = canvas.CreateDrawingSession(InvalidatedRegion);
                    DrawText(InvalidatedRegion, drawingSession);
                    DrawLines(InvalidatedRegion, drawingSession);
                }
            }
        }

        private void DrawText(Rect region, CanvasDrawingSession drawingSession)
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

        private void DrawLines(Rect region, CanvasDrawingSession drawingSession)
        {
            var HiTempColor = Colors.OrangeRed;
            var LoTempColor = Colors.LightSkyBlue;

            var set = Data.GetDataSet();
            for (int i = 0; i < drawDotLists.Count; i++)
            {
                var bar = drawDotLists[i];
                var entry = set.GetEntryForIndex(i);

                using var lineBrush = new CanvasSolidColorBrush(drawingSession, entry.FillColor ?? Colors.AliceBlue);

                drawingRect.Set(bar.X - LineStrokeWidth / 2f,
                    bar.Y - bottomTextHeight - bottomTextDescent,
                    bar.X + LineStrokeWidth / 2f,
                    GraphBottom);

                if (drawingRect.Intersects(region))
                {
                    drawingSession.DrawLine(bar.X, (float)drawingRect.Bottom, bar.X, bar.Y, lineBrush, LineStrokeWidth, LineStrokeStyle);

                    if (DrawDataLabels)
                    {
                        if (entry.EntryData != null)
                        {
                            drawingSession.DrawText(entry.EntryData.YLabel, bar.X, bar.Y - bottomTextHeight - bottomTextDescent, BottomTextColor, BottomTextFormat);
                        }
                    }
                }
            }
        }

        protected override float GetGraphExtentWidth()
        {
            return longestTextWidth * MaxEntryCount;
        }

        protected override float GetPreferredWidth()
        {
            if (!xCoordinateList.Any())
            {
                return backgroundGridWidth * MaxEntryCount;
            }
            else
            {
                return xCoordinateList.Last() + sideLineLength;
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
                    LineStrokeStyle?.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}