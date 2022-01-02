using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Controls.Graphs
{
    [TemplatePart(Name = nameof(InternalScrollViewer), Type = typeof(ScrollViewer))]
    public abstract class BaseGraphView<T, S, E> : BaseGraphViewControl, IGraph
        where T : GraphData<S, E> where S : GraphDataSet<E> where E : GraphEntry
    {
        protected T Data { get; set; }
        private int MaxXEntries { get; set; }

        protected Rect drawingRect;

        protected readonly List<float> xCoordinateList;
        protected int horizontalGridNum;
        protected int verticalGridNum;
        protected const int MIN_HORIZONTAL_GRID_NUM = 1;

        protected float sideLineLength = 0f;
        protected float backgroundGridWidth;
        protected float longestTextWidth;

        protected float IconHeight;

        protected readonly WeatherIconsManager wim = WeatherIconsManager.GetInstance();

        public BaseGraphView() : base()
        {
            xCoordinateList = new List<float>();
        }

        public ScrollViewer ScrollViewer => InternalScrollViewer;
        public FrameworkElement Control => this;

        protected override void OnCreateCanvasResources(CanvasVirtualControl canvas)
        {
            backgroundGridWidth = canvas.ConvertDipsToPixels(45, CanvasDpiRounding.Floor);
        }

        protected IconControl CreateIconControl(string WeatherIcon)
        {
            return new IconControl()
            {
                Height = IconHeight,
                Width = IconHeight,
                WeatherIcon = WeatherIcon,
                ShowAsMonochrome = false,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
        }

        public void SetData(T data)
        {
            this.Data = data;
            NotifyDataSetChanged();
            UpdateGraph();
        }

        public bool IsDataEmpty => Data == null || Data.IsEmpty;
        public int DataCount => Data?.DataCount ?? 0;

        protected int MaxEntryCount => MaxXEntries;

        public virtual void ResetData(bool invalidate = false)
        {
            this.Data = null;
            this.xCoordinateList.Clear();
            // Remove animated drawables
            if (invalidate)
            {
                IconCanvas.Children.Clear();
                InvalidateMeasure();
            }
        }

        public abstract void UpdateGraph();

        protected virtual void NotifyDataSetChanged()
        {
            CalcMaxX();
        }

        private void CalcMaxX()
        {
            var count = 0;

            if (!IsDataEmpty)
            {
                foreach (var set in Data.DataSets)
                {
                    if (set.DataCount > count)
                    {
                        count = set.DataCount;
                    }
                }
            }

            MaxXEntries = count;
        }

        protected void UpdateHorizontalGridNum()
        {
            horizontalGridNum = Math.Max(horizontalGridNum,
                Math.Max(MIN_HORIZONTAL_GRID_NUM, MaxEntryCount - 1));
        }

        public int GetItemPositionFromPoint(float xCoordinate)
        {
            if (horizontalGridNum <= 1)
                return 0;

            return BinarySearchPointIndex(xCoordinate);
        }

        protected int BinarySearchPointIndex(float targetXPoint)
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

        protected sealed override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);

            ScrollViewer.Height = double.IsInfinity(availableSize.Height) ? double.NaN : availableSize.Height;
            ScrollViewer.Width = double.IsInfinity(availableSize.Width) ? double.NaN : availableSize.Width;

            OnPreMeasure();

            Canvas.Width = MaxCanvasWidth > 0
                ? Math.Min(MaxCanvasWidth, GetPreferredWidth())
                : GetPreferredWidth();
            Canvas.Height = availableSize.Height;

            ViewHeight = (float)Canvas.Height;
            ViewWidth = (float)Canvas.Width;

            OnPostMeasure();

            // Post the event to the dispatcher to allow the method to complete first
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OnItemWidthChanged(new ItemSizeChangedEventArgs()
                {
                    NewSize = new System.Drawing.Size(xCoordinateList.Count > 0 ? (int)xCoordinateList.Last() : 0, (int)Canvas.Height)
                });
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return size;
        }

        protected virtual void OnPreMeasure()
        {
            
        }

        protected virtual void OnPostMeasure()
        {
            // Redraw View
            Canvas.Invalidate();
        }

        protected virtual float GetGraphExtentWidth()
        {
            return (backgroundGridWidth * horizontalGridNum) + (sideLineLength * 2);
        }

        protected virtual float GetPreferredWidth()
        {
            return GetGraphExtentWidth();
        }
    }
}
