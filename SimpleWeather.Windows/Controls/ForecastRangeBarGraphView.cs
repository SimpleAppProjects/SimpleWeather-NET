using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace SimpleWeather.NET.Controls
{
    public partial class ForecastRangeBarGraphView : StackPanel
    {
        private int MaxItemCount = 7;
        private float scale = 1f;
        private bool IsMeasured = false;

        private double oldWidth;

        private ForecastRangeBarGraphDataSet dataSet = null;

        public event EventHandler<TappedRoutedEventArgs> ItemClick;

        public ForecastRangeBarGraphView()
        {
            this.MinHeight = 250;
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Center;
            this.Unloaded += ForecastRangeBarGraphView_Unloaded;
            Children.Clear();

            this.DataContextChanged += (s, e) =>
            {
                SetData(e.NewValue as ForecastRangeBarGraphDataSet);
            };
        }

        private void ForecastRangeBarGraphView_Unloaded(object sender, RoutedEventArgs e)
        {
            IsMeasured = false;
        }

        public int GetItemPosition(object item)
        {
            var index = Children.IndexOf(item);
            return index >= 0 ? index : 0;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);

            if (availableSize.Width != oldWidth)
            {
                IsMeasured = false;
            }

            oldWidth = availableSize.Width;

            if (size.Width > 0)
            {
                MaxItemCount = (int)Math.Min(7, availableSize.Width / 70);

                Children?.Cast<ForecastRangeBar>()?.FirstOrDefault()?.Apply(v =>
                {
                    if (dataSet != null)
                    {
                        var remainingSpace = v.Height - v.InnerBarView.Height;
                        if (remainingSpace > 0)
                        {
                            var dataBarHeight = dataSet.YMax - dataSet.YMin;
                            scale = MathF.Min((float)remainingSpace / dataBarHeight, 1f) * 2;

                            if (!IsMeasured)
                            {
                                DispatcherQueue.TryEnqueue(() => SetData(dataSet));
                            }

                            IsMeasured = true;
                        }
                    }
                });
            }

            return size;
        }

        public void SetData(ForecastRangeBarGraphDataSet dataSet)
        {
            this.dataSet = dataSet;

            if (dataSet != null && !dataSet.IsEmpty)
            {
                var itemCount = Math.Min(dataSet.DataCount, MaxItemCount);

                var max = dataSet.YMax;
                var min = dataSet.YMin;

                for (int i = 0; i < itemCount; i++)
                {
                    var data = dataSet.GetEntryForIndex(i);
                    var item = Children.ElementAtOrDefault(i) as ForecastRangeBar ?? new ForecastRangeBar().Apply(it => 
                    {
                        it.Tapped += (s, e) =>
                        {
                            ItemClick?.Invoke(it, new TappedRoutedEventArgs());
                        };
                    });

                    item.DataContext = data;

                    item.InnerBarView.Margin = new Thickness(
                        item.InnerBarView.Margin.Left,
                        (data.HiTempData?.Y)?.Let(it => (max - it) * scale) ?? 0,
                        item.InnerBarView.Margin.Right,
                        (data.LoTempData?.Y)?.Let(it => (it - min) * scale) ?? 0);

                    if (data.HiTempData == null || data.LoTempData == null)
                    {
                        item.RangeBarView.Height = item.RangeBarView.Width;

                        if (data.HiTempData == null)
                        {
                            item.SetColors(Colors.LightSkyBlue, Colors.LightSkyBlue);
                        }
                        else if (data.LoTempData == null)
                        {
                            item.SetColors(Colors.OrangeRed, Colors.OrangeRed);
                        }
                    }
                    else
                    {
                        item.RangeBarView.Height = 100 - (item.InnerBarView.Margin.Top + item.InnerBarView.Margin.Bottom);
                        item.SetColors();
                    }

                    if (Children.ElementAtOrDefault(i) == null)
                    {
                        Children.Add(item);
                    }
                }

                Children.RemoveRange(itemCount, Children.Count - itemCount);
            }
            else
            {
                Children.Clear();
            }
        }
    }
}
