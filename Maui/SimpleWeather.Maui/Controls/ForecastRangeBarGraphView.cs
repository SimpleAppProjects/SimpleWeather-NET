using SimpleWeather.NET.Controls.Graphs;
using System;
using System.Linq;
using SimpleWeather.Utils;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;
using SimpleWeather.Maui.Helpers;
using Microsoft.Maui.Controls;

namespace SimpleWeather.Maui.Controls;

public class ForecastRangeBarGraphView : StackLayout
{
    private int MaxItemCount = 7;
    private float scale = 1f;
    private bool IsMeasured = false;

    private double oldWidth;

    private ForecastRangeBarGraphDataSet dataSet = null;

    public event EventHandler<ItemTappedEventArgs> ItemClick;

    public ForecastRangeBarGraphView()
    {
        MinimumHeightRequest = 250;
        Orientation = StackOrientation.Horizontal;
        HorizontalOptions = LayoutOptions.CenterAndExpand;
        this.Unloaded += ForecastRangeBarGraphView_Unloaded;
        Clear();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        SetData(BindingContext as ForecastRangeBarGraphDataSet);
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var size = base.MeasureOverride(widthConstraint, heightConstraint);

        if (widthConstraint != oldWidth)
        {
            IsMeasured = false;
        }

        oldWidth = widthConstraint;

        if (size.Width > 0)
        {
            MaxItemCount = (int)Math.Min(7, widthConstraint / 70);

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
                            Dispatcher.Dispatch(() => SetData(dataSet));
                        }

                        IsMeasured = true;
                    }
                }
            });
        }

        return size;
    }

    private void ForecastRangeBarGraphView_Unloaded(object sender, EventArgs e)
    {
        IsMeasured = false;
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
                var item = Children.ElementAtOrDefault(i) as ForecastRangeBar ?? new ForecastRangeBar();

                item.BindingContext = data;
                item.GestureRecognizers.Clear();
                item.TapGesture(() =>
                {
                    ItemClick?.Invoke(this, new ItemTappedEventArgs(Children, item, Math.Max(Children.IndexOf(item), 0)));
                });

                item.InnerBarView.Margins(
                    top: (data.HiTempData?.Y)?.Let(it => (max - it) * scale) ?? 0,
                    bottom: (data.LoTempData?.Y)?.Let(it => (it - min) * scale) ?? 0
                );

                if (data.HiTempData == null || data.LoTempData == null)
                {
                    item.RangeBarView.HeightRequest = item.RangeBarView.WidthRequest;
                    FlexLayout.SetGrow(item.RangeBarView, 0);
                    FlexLayout.SetShrink(item.RangeBarView, 0);

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
                    item.RangeBarView.HeightRequest = 100 - item.InnerBarView.Margin.VerticalThickness;
                    FlexLayout.SetShrink(item.RangeBarView, 0);
                    FlexLayout.SetGrow(item.RangeBarView, 1);
                    item.SetColors();
                }

                if (Children.ElementAtOrDefault(i) == null)
                {
                    Add(item);
                }
            }

            this.RemoveChildren(itemCount, Count - itemCount);
        }
        else
        {
            Clear();
        }
    }
}

