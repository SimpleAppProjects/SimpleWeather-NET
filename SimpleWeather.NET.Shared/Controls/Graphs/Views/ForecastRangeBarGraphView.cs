﻿using SimpleWeather.Icons;
using SimpleWeather.SkiaSharp;
#if WINDOWS
using SimpleWeather.NET.Helpers;
#else
using SimpleWeather.Maui.Helpers;
#endif
using SkiaSharp;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp.Views.Windows;
using CommunityToolkit.WinUI;
#else
using SKXamlCanvas = SkiaSharp.Views.Maui.Controls.SKCanvasView;
using SimpleWeather.Utils;
#endif

namespace SimpleWeather.NET.Controls.Graphs
{
    public sealed partial class ForecastRangeBarGraphView : BaseGraphView<ForecastRangeBarGraphData, ForecastRangeBarGraphDataSet, ForecastRangeBarEntry>
    {
        private bool disposedValue;

        private List<Bar> drawDotLists; // Y data

        private readonly SKPaint linePaint;
        private readonly SKPaint popTextPaint;
        private readonly SKFont popIconFont;

#if WINDOWS
        private const string WIconFontUri = "ms-appx:///SimpleWeather.Shared/Resources/Fonts/weathericons-regular-webfont.ttf";
#else
        private const string WIconFontUri = "weathericons-regular-webfont.ttf";
#endif

        private const string PRECIP_ICON = WeatherIcons.UMBRELLA;

        public ForecastRangeBarGraphView() : base()
        {
#if WINDOWS
            this.DefaultStyleKey = typeof(ForecastRangeBarGraphView);
#endif

            drawDotLists = new List<Bar>();

            ResetData(false);

            linePaint = new SKPaint()
            {
                IsAntialias = true,
                StrokeWidth = 6f, // 6dp
                StrokeCap = SKStrokeCap.Round
            };

            popTextPaint = new SKPaint()
            {
                IsAntialias = true,
                TextSize = FontSizeToTextSize(FontSize),
                TextAlign = SKTextAlign.Center,
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0x59, 0x9e, 0xf3), // #599ef3
            };

            popIconFont = new SKFont
            {
                Edging = SKFontEdging.SubpixelAntialias,
                Size = popTextPaint.TextSize,
                EmbeddedBitmaps = true,
                Subpixel = true
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
                if (DrawIconLabels) graphHeight = graphHeight - IconHeight - iconBottomMargin;
                if (DrawDataLabels)
                {
                    graphHeight = graphHeight - bottomTextTopMargin - linePaint.StrokeWidth;
                    graphHeight = graphHeight - (bottomTextHeight * 2f + bottomTextDescent * 2f + bottomTextTopMargin); // PoP
                }

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
            bottomTextDescent = 0;
            longestTextWidth = 0;

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
            InvalidateMeasure();
        }

        private void RefreshGridWidth()
        {
            // Reset the grid width
            backgroundGridWidth = longestTextWidth;
#if __MACCATALYST__
            var defaultPadding = 32f; // 32dp
#else
            var defaultPadding = 8f; // 8dp
#endif

            if (GetGraphExtentWidth() < ScrollViewer.Width)
            {
                float freeSpace = (float)(ScrollViewer.Width - GetGraphExtentWidth());
                float availableAdditionalSpace = freeSpace / MaxEntryCount;
#if WINDOWS
                if (HorizontalAlignment == HorizontalAlignment.Stretch)
#else
                if (HorizontalOptions.Alignment == LayoutAlignment.Fill)
#endif
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
                    float? hiY = null, loY = null;

                    /*
                     * Scaling formula
                     *
                     * ((value - minValue) / (maxValue - minValue)) * (scaleMax - scaleMin) + scaleMin
                     * graphTop is scaleMax & graphHeight is scaleMin due to View coordinate system
                     */
                    if (entry.HiTempData != null)
                    {
                        hiY = ((entry.HiTempData.Y - minValue) / (maxValue - minValue)) * (graphTop - graphHeight) + graphHeight;
                    }

                    if (entry.LoTempData != null)
                    {
                        loY = ((entry.LoTempData.Y - minValue) / (maxValue - minValue)) * (graphTop - graphHeight) + graphHeight;
                    }

                    // Skip empty entry
                    if (hiY == null && loY == null)
                    {
                        continue;
                    }

                    if (hiY == null)
                    {
                        hiY = loY;
                    }

                    if (loY == null)
                    {
                        loY = hiY;
                    }

                    if (i > drawDotSize - 1)
                    {
                        drawDotLists.Add(new Bar(x, hiY.Value, loY.Value));
                    }
                    else
                    {
                        drawDotLists[i] = new Bar(x, hiY.Value, loY.Value);
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

        private void DrawLines(SKCanvas canvas)
        {
            if (drawDotLists.Any() && !IsDataEmpty)
            {
                var HiTempColor = SKColors.OrangeRed;
                var LoTempColor = SKColors.LightSkyBlue;

                ForecastRangeBarGraphDataSet set = Data.GetDataSet();
                for (int i = 0; i < drawDotLists.Count; i++)
                {
                    var bar = drawDotLists[i];
                    var entry = set.GetEntryForIndex(i);
                    var drawLine = true;

                    if (entry.HiTempData != null && entry.LoTempData != null)
                    {
                        var shader = SKShader.CreateLinearGradient(new SKPoint(0, bar.HiY), new SKPoint(0, bar.LoY), new SKColor[] { HiTempColor, LoTempColor }, SKShaderTileMode.Clamp);
                        linePaint.Shader = shader;
                    }
                    else if (entry.HiTempData != null)
                    {
                        linePaint.Shader = null;
                        linePaint.Color = HiTempColor;
                        drawLine = false;
                    }
                    else if (entry.LoTempData != null)
                    {
                        linePaint.Shader = null;
                        linePaint.Color = LoTempColor;
                        drawLine = false;
                    }

                    drawingRect.Set(bar.X - linePaint.StrokeWidth / 2f,
                        bar.HiY - bottomTextHeight - bottomTextDescent,
                        bar.X + linePaint.StrokeWidth / 2f,
                        bar.LoY + bottomTextHeight + bottomTextDescent
                    );

                    if (drawingRect.Intersects(visibleRect))
                    {
                        if (drawLine)
                        {
                            canvas.DrawLine(bar.X, bar.HiY, bar.X, bar.LoY, linePaint);
                        }
                        else
                        {
                            canvas.DrawLine(bar.X, bar.HiY - linePaint.StrokeWidth / 4f, bar.X, bar.HiY, linePaint);
                        }

                        if (DrawDataLabels)
                        {
                            if (entry.HiTempData != null)
                            {
                                canvas.DrawText(entry.HiTempData.YLabel, bar.X, bar.HiY - bottomTextHeight - bottomTextDescent, bottomTextFont, bottomTextPaint);
                            }
                            if (entry.LoTempData != null)
                            {
#if WINDOWS
                                canvas.DrawText(entry.LoTempData.YLabel, bar.X, bar.LoY + bottomTextHeight + bottomTextDescent + linePaint.StrokeWidth * 1.5f, bottomTextFont, bottomTextPaint);
#else
                                canvas.DrawText(entry.LoTempData.YLabel, bar.X, bar.LoY + bottomTextHeight + bottomTextDescent + linePaint.StrokeWidth, bottomTextFont, bottomTextPaint);
#endif
                            }
                            if (entry.PoP != null)
                            {
                                var popIconGlyphs = PRECIP_ICON.Select(c => popIconFont.GetGlyph(c)).ToArray();
                                var popString = $" {entry.PoP.GetValueOrDefault()}%";
                                var popTxtGlyphs = popString.Select(c => bottomTextFont.GetGlyph(c)).ToArray();

                                var positions = new float[popString.Length];
                                var popIconBounds = popIconFont.MeasureText(popIconGlyphs, popTextPaint);

                                for (int pos = 0; pos < positions.Length; pos++)
                                {
                                    if (pos == 0)
                                        positions[pos] = popIconBounds;
                                    else
                                        positions[pos] = positions[pos - 1] + bottomTextFont.MeasureText([popTxtGlyphs[pos - 1]], bottomTextPaint);
                                }

                                var textBlob = new SKTextBlobBuilder();
                                textBlob.AddHorizontalRun(popIconGlyphs, popIconFont, [0f], 0);
                                textBlob.AddHorizontalRun(popTxtGlyphs, bottomTextFont, positions, 0);

                                canvas.DrawText(textBlob.Build(), bar.X - (positions[^1] / 2f) - popIconBounds / 2f, ViewHeight - bottomTextHeight - bottomTextDescent - linePaint.StrokeWidth * 2f - IconHeight, popTextPaint);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnCanvasLoaded(SKXamlCanvas canvas)
        {
            base.OnCanvasLoaded(canvas);

            // Post the event to the dispatcher to allow the method to complete first
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#if WINDOWS
            DispatcherQueue.EnqueueAsync(async () =>
#else
            MainThread.InvokeOnMainThreadAsync(async () =>
#endif
            {
#if WINDOWS
                var fs = await StorageFileHelper.GetFileStreamFromApplicationUri(WIconFontUri);
#else
                var fs = await FileSystemUtils.OpenAppPackageFileAsync(WIconFontUri);
#endif
                try
                {
                    popIconFont.Typeface = SKTypeface.FromStream(fs);
                }
                catch { }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        protected override void UpdateFontSize()
        {
            popIconFont.Size = popTextPaint.TextSize = FontSizeToTextSize(FontSize);
            base.UpdateFontSize();
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
            public float HiY { get; set; }
            public float LoY { get; set; }

            public Bar(float x, float hiY, float loY)
            {
                this.X = x;
                this.HiY = hiY;
                this.LoY = loY;
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
                    popTextPaint?.Dispose();
                    popIconFont?.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}