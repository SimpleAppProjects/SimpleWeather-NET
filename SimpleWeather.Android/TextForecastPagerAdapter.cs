using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Java.Lang;
using SimpleWeather.Controls;
using SimpleWeather.Droid.Controls;
using System.Collections.ObjectModel;

namespace SimpleWeather.Droid
{
    public class TextForecastPagerAdapter : PagerAdapter
    {
        private Context context;
        private List<TextForecastItemViewModel> txt_forecast;

        public TextForecastPagerAdapter(Context context, List<TextForecastItemViewModel> txt_forecast)
        {
            this.context = context;
            this.txt_forecast = txt_forecast;
        }

        public override int Count => txt_forecast.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            TextForecastItem fctItem = new TextForecastItem(context);
            fctItem.SetForecast(txt_forecast[position]);
            container.AddView(fctItem);
            return fctItem;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView(@object as View);
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(txt_forecast[position].Title);
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return PagerAdapter.PositionNone;
        }

        public void UpdateDataset(List<TextForecastItemViewModel> dataset)
        {
            txt_forecast.Clear();
            txt_forecast.AddRange(dataset);
            NotifyDataSetChanged();
        }
    }
}