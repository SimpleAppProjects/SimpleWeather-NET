using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SimpleWeather.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Controls.Graphs
{
    [TemplatePart(Name = nameof(InternalScrollViewer), Type = typeof(ScrollViewer))]
    public abstract class BaseGraphView<T, S, E> : BaseGraphViewControl, IGraph, IDisposable
        where T : GraphData<S, E> where S : GraphDataSet<E> where E : GraphEntry
    {
        private bool disposedValue;

        protected T Data { get; set; }
        private int MaxXEntries { get; set; }

        protected Rect drawingRect;

        protected readonly List<float> xCoordinateList;
        protected int horizontalGridNum;
        protected int verticalGridNum;
        protected const int MIN_HORIZONTAL_GRID_NUM = 1;

        protected readonly CanvasTextFormat BottomTextFormat;
        protected float bottomTextHeight;
        protected float bottomTextDescent;

        protected float iconBottomMargin;
        protected float bottomTextTopMargin;

        protected float sideLineLength;
        protected float backgroundGridWidth;
        protected float longestTextWidth;

        protected float IconHeight;
        protected readonly WeatherIconsManager wim = SharedModule.Instance.WeatherIconsManager;

        public BaseGraphView() : base()
        {
            xCoordinateList = new List<float>();

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

        public ScrollViewer ScrollViewer => InternalScrollViewer;
        public FrameworkElement Control => this;

        public Color BottomTextColor
        {
            get => (Color)GetValue(BottomTextColorProperty);
            set => SetValue(BottomTextColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for BottomTextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextColorProperty =
            DependencyProperty.Register("BottomTextColor", typeof(Color), typeof(BaseGraphView<T, S, E>), new PropertyMetadata(Colors.White));

        public bool DrawIconLabels { get; set; }
        public bool DrawDataLabels { get; set; }

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

        protected override void OnCreateCanvasResources(CanvasVirtualControl canvas)
        {
            base.OnCreateCanvasResources(canvas);

            // Calculate icon height
            IconHeight = canvas.ConvertDipsToPixels(48, CanvasDpiRounding.Floor);

            backgroundGridWidth = canvas.ConvertDipsToPixels(45, CanvasDpiRounding.Floor);

            iconBottomMargin = canvas.ConvertDipsToPixels(4, CanvasDpiRounding.Floor);
            bottomTextTopMargin = canvas.ConvertDipsToPixels(6, CanvasDpiRounding.Floor);
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

            if (this.ScrollViewer == null || this.Canvas == null)
            {
                return size;
            }

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

            // Redraw View
            Canvas.Invalidate();
            RepositionIcons();

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

        /// <summary>
        /// Places icons from data labels on canvas.
        /// Should be called in UpdateGraph method
        /// </summary>
        protected virtual void DrawIcons()
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
                        Windows.UI.Xaml.Controls.Canvas.SetTop(control, ViewHeight - IconHeight * 1.25 - bottomTextTopMargin);
                        IconCanvas.Children.Add(control);
                    }
                }
            }
        }

        /// <summary>
        /// Repositions icons on canvas.
        /// Called on MeasureOverride
        /// </summary>
        protected virtual void RepositionIcons()
        {
            if (DrawIconLabels)
            {
                for (int i = 0; i < IconCanvas.Children.Count; i++)
                {
                    var control = IconCanvas.Children[i];
                    Windows.UI.Xaml.Controls.Canvas.SetLeft(control, xCoordinateList[i] - IconHeight / 2);
                    Windows.UI.Xaml.Controls.Canvas.SetTop(control, ViewHeight - IconHeight * 1.2 - bottomTextTopMargin);
                }
            }
        }

        protected virtual void OnPreMeasure()
        {
        }

        protected virtual void OnPostMeasure()
        {
        }

        protected virtual float GetGraphExtentWidth()
        {
            return (backgroundGridWidth * horizontalGridNum) + (sideLineLength * 2);
        }

        protected virtual float GetPreferredWidth()
        {
            return GetGraphExtentWidth();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    BottomTextFormat?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseGraphView()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
