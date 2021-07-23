using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
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
    public sealed partial class RangeBarGraphView : UserControl, IGraph, IIconCacheGraph, IDisposable
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
        private double drwTextWidth;
        private float bottomTextHeight = 0;

        private List<XLabelData> dataLabels; // X
        private List<GraphTemperature> dataLists; // Y data

        private List<float> xCoordinateList;

        private List<Bar> drawDotLists; // Y data

        private float bottomTextDescent;

        private readonly float iconBottomMargin;
        private readonly float bottomTextTopMargin;

        private float sideLineLength = 0f;
        private float backgroundGridWidth;
        private float longestTextWidth;

        private Color BottomTextColor => BottomTextColorBrush.Color;

        public bool DrawIconLabels { get; set; }
        public bool DrawDataLabels { get; set; }
        public bool CenterGraphView { get; set; }

        private readonly CanvasTextFormat BottomTextFormat;

        private double IconHeight;
        private Dictionary<String, CanvasBitmap> IconCache;
        private readonly object iconCacheLock = new object();

        private readonly float LineStrokeWidth;
        private readonly CanvasStrokeStyle LineStrokeStyle;

        public bool ReadyToDraw => Canvas.ReadyToDraw;

        private readonly WeatherIconsManager wim = WeatherIconsManager.GetInstance();

        public RangeBarGraphView()
        {
            this.InitializeComponent();

            dataLabels = new List<XLabelData>();
            dataLists = new List<GraphTemperature>();
            xCoordinateList = new List<float>();
            drawDotLists = new List<Bar>();

            BottomTextFormat = new CanvasTextFormat
            {
                FontSize = (float)FontSize,
                FontWeight = FontWeight,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            iconBottomMargin = Canvas.ConvertDipsToPixels(2, CanvasDpiRounding.Floor);
            bottomTextTopMargin = Canvas.ConvertDipsToPixels(6, CanvasDpiRounding.Floor);

            backgroundGridWidth = Canvas.ConvertDipsToPixels(45, CanvasDpiRounding.Floor);

            LineStrokeWidth = Canvas.ConvertDipsToPixels(8, CanvasDpiRounding.Floor);
            LineStrokeStyle = new CanvasStrokeStyle()
            {
                StartCap = CanvasCapStyle.Round,
                EndCap = CanvasCapStyle.Round,
                LineJoin = CanvasLineJoin.Round
            };

            RegisterPropertyChangedCallback(FontSizeProperty, OnDependencyPropertyChanged);
            RegisterPropertyChangedCallback(FontWeightProperty, OnDependencyPropertyChanged);
        }

        private void OnDependencyPropertyChanged(DependencyObject obj, DependencyProperty property)
        {
            if (property == FontSizeProperty)
            {
                this.BottomTextFormat.FontSize = (float)FontSize;
                CalculateBottomTextSize();
                RefreshDrawDotList();
                if (Canvas.ReadyToDraw)
                {
                    Canvas.Invalidate();
                }
            }
            else if (property == FontWeightProperty)
            {
                this.BottomTextFormat.FontWeight = FontWeight;
                CalculateBottomTextSize();
                RefreshDrawDotList();
                if (Canvas.ReadyToDraw)
                {
                    Canvas.Invalidate();
                }
            }
        }

        private float GraphTop
        {
            get
            {
                return bottomTextTopMargin + bottomTextHeight / 2f + bottomTextDescent;
            }
        }

        private float GraphHeight
        {
            get
            {
                float graphHeight = ViewHeight - bottomTextTopMargin - bottomTextHeight - bottomTextDescent - LineStrokeWidth;
                if (DrawIconLabels) graphHeight = (float)(graphHeight - IconHeight - iconBottomMargin);
                if (DrawDataLabels) graphHeight = graphHeight - bottomTextTopMargin - LineStrokeWidth;

                return graphHeight;
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
            this.drawDotLists.Clear();
            bottomTextDescent = 0;
            longestTextWidth = 0;
            DrawIconLabels = false;
            RefreshDrawDotList();
            InvalidateMeasure();
        }

        public void SetData(List<XLabelData> dataLabels, List<GraphTemperature> dataLists)
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

            CalculateBottomTextSize();
        }

        private void CalculateBottomTextSize()
        {
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
                    if (bottomTextDescent < Math.Abs(txtLayout.LayoutBounds.Bottom))
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

        private void SetDataList(List<GraphTemperature> dataLists)
        {
            this.dataLists.Clear();

            if (dataLists != null)
            {
                this.dataLists.AddRange(dataLists);
            }

            RefreshDrawDotList();
            InvalidateMeasure();
        }

        private void RefreshGridWidth()
        {
            // Reset the grid width
            backgroundGridWidth = longestTextWidth * 1.1f;

            if (!CenterGraphView && GetPreferredWidth() < ScrollViewer.Width)
            {
                float freeSpace = (float)(ScrollViewer.Width - GetPreferredWidth());
                float additionalSpace = freeSpace / HorizontalGridNum;
                backgroundGridWidth += additionalSpace;
            }
            RefreshXCoordinateList();
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
            xCoordinateList.EnsureCapacity(dataLabels.Count);

            var IsWithinViewport = GetPreferredWidth() < ScrollViewer.Width;

            for (int i = 0; i < dataLabels.Count; i++)
            {
                float x;
                if (CenterGraphView && IsWithinViewport)
                {
                    x = (float)((ScrollViewer.Width / (dataLabels.Count + 1)) * (i + 1));
                }
                else
                {
                    x = sideLineLength + backgroundGridWidth * i;
                }
                xCoordinateList.Add(x);
            }
        }

        private void RefreshDrawDotList()
        {
            if (dataLists != null && dataLists.Count > 0)
            {
                drawDotLists.Clear();
                float maxValue = float.MinValue;
                float minValue = float.MaxValue;
                foreach (var tempData in dataLists) 
                {
                    if (tempData.HiTempData != null)
                    {
                        maxValue = Math.Max(maxValue, tempData.HiTempData.Y);
                    }
                    if (tempData.LoTempData != null)
                    {
                        minValue = Math.Min(minValue, tempData.LoTempData.Y);
                    }
                }

                float graphHeight = GraphHeight;
                float graphTop = GraphTop;

                int drawDotSize = drawDotLists.Count > 0 ? drawDotLists.Count : 0;

                if (drawDotSize > 0)
                {
                    drawDotLists.EnsureCapacity(dataLists.Count);
                }

                for (int i = 0; i < dataLists.Count; i++)
                {
                    var entry = dataLists[i];
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

        private string GetThemeSuffix(bool isLight)
        {
            return isLight ? "_light" : "_dark";
        }

        public void ClearIconCache()
        {
            IconCache?.Clear();
        }

        private void Canvas_CreateResources(CanvasVirtualControl sender, CanvasCreateResourcesEventArgs args)
        {
            // Calculate icon height
            IconHeight = sender.ConvertDipsToPixels(36, CanvasDpiRounding.Floor);

            IconCache = new Dictionary<string, CanvasBitmap>();
        }

        private void Canvas_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            // Draw the effect to whatever regions of the CanvasVirtualControl have been invalidated.
            foreach (var InvalidatedRegion in args.InvalidatedRegions)
            {
                using (var drawingSession = sender.CreateDrawingSession(InvalidatedRegion))
                {
                    DrawText(InvalidatedRegion, drawingSession);
                    DrawLines(InvalidatedRegion, drawingSession);
                }
            }
        }

        private void DrawText(Rect region, CanvasDrawingSession drawingSession)
        {
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
                            drwTextWidth = btmTxtLayout.LayoutBounds.Width;
                            Rect btmTxtRect = RectHelper.FromPoints(
                                new Point(x - drwTextWidth / 2, y - btmTxtLayout.LayoutBounds.Height / 2),
                                new Point(x + drwTextWidth / 2, y + btmTxtLayout.LayoutBounds.Height / 2));
                            if (!RectHelper.Intersect(region, btmTxtRect).IsEmpty)
                                drawingSession.DrawTextLayout(btmTxtLayout, x, y, BottomTextColor);
                        }
                    }

                    if (DrawIconLabels && !String.IsNullOrWhiteSpace(xData.XIcon))
                    {
                        int rotation = xData.XIconRotation;

                        Rect iconRect = RectHelper.FromPoints(
                            new Point(x - IconHeight / 2, y - bottomTextHeight - iconBottomMargin * 2f - IconHeight / 2),
                            new Point(x + IconHeight / 2, y - bottomTextHeight - iconBottomMargin * 2f + IconHeight / 2));

                        if (!RectHelper.Intersect(region, iconRect).IsEmpty)
                        {
                            var isLightObj = this.Resources["IsLight"] as Helpers.ObjectContainer;
                            bool isLight = false;

                            if (isLightObj?.Value is Color paramColor)
                            {
                                isLight = paramColor == Colors.Black;
                            }
                            else if (isLightObj?.Value is bool)
                            {
                                isLight = (bool)isLightObj.Value;
                            }

                            CanvasBitmap icon = IconCache.GetValueOrDefault(xData.XIcon + GetThemeSuffix(isLight), null);

                            if (icon == null)
                            {
                                var task = CanvasBitmap.LoadAsync(Canvas, new Uri(wim.GetWeatherIconURI(xData.XIcon, true, isLight))).AsTask();
                                task.ContinueWith((t) =>
                                {
                                    if (t.IsCompletedSuccessfully)
                                    {
                                        lock (iconCacheLock)
                                        {
                                            IconCache.TryAdd(xData.XIcon + GetThemeSuffix(isLight), t.Result);
                                        }
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
            if (wim.IsFontIcon)
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

        private void DrawLines(Rect region, CanvasDrawingSession drawingSession)
        {
            var HiTempColor = Colors.OrangeRed;
            var LoTempColor = Colors.LightSkyBlue;
            Rect drawingRect;

            for (int i = 0; i < drawDotLists.Count; i++)
            {
                var bar = drawDotLists[i];
                var entry = dataLists[i];
                var drawLine = true;

                ICanvasBrush lineBrush;
                if (entry.HiTempData != null && entry.LoTempData != null)
                {
                    var gradientStops = new CanvasGradientStop[2]
                    {
                        new CanvasGradientStop()
                        {
                            Color = HiTempColor,
                            Position = 0f
                        },
                        new CanvasGradientStop()
                        {
                            Color = LoTempColor,
                            Position = 1f
                        }
                    };
                    lineBrush = new CanvasLinearGradientBrush(drawingSession, gradientStops, CanvasEdgeBehavior.Clamp, CanvasAlphaMode.Premultiplied)
                    {
                        StartPoint = new Vector2(bar.X, bar.HiY),
                        EndPoint = new Vector2(bar.X, bar.LoY)
                    };
                }
                else if (entry.HiTempData != null)
                {
                    lineBrush = new CanvasSolidColorBrush(drawingSession, HiTempColor);
                    drawLine = false;
                }
                else/* if (entry.LoTempData != null)*/
                {
                    lineBrush = new CanvasSolidColorBrush(drawingSession, LoTempColor);
                    drawLine = false;
                }

                drawingRect = new Rect(bar.X - LineStrokeWidth / 2f,
                    bar.HiY - bottomTextHeight - bottomTextDescent,
                    bar.X + drwTextWidth + LineStrokeWidth / 2f,
                    bar.LoY + bottomTextHeight + bottomTextDescent);

                if (!RectHelper.Intersect(region, drawingRect).IsEmpty)
                {
                    if (drawLine)
                    {
                        drawingSession.DrawLine(bar.X, bar.HiY, bar.X, bar.LoY, lineBrush, LineStrokeWidth, LineStrokeStyle);
                    }
                    else
                    {
                        drawingSession.DrawLine(bar.X, bar.HiY - LineStrokeWidth / 4f, bar.X, bar.HiY, lineBrush, LineStrokeWidth, LineStrokeStyle);
                    }

                    if (DrawDataLabels)
                    {
                        if (entry.HiTempData != null)
                        {
                            drawingSession.DrawText(entry.HiTempData.YLabel, bar.X, bar.HiY - bottomTextHeight - bottomTextDescent + LineStrokeWidth / 2, BottomTextColor, BottomTextFormat);
                        }
                        if (entry.LoTempData != null)
                        {
                            drawingSession.DrawText(entry.LoTempData.YLabel, bar.X, bar.LoY + LineStrokeWidth * 1.5f, BottomTextColor, BottomTextFormat);
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
            RefreshDrawDotList();
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

        public void Dispose()
        {
            BottomTextFormat?.Dispose();
            LineStrokeStyle?.Dispose();
            IconCache?.Clear();
        }
    }
}