using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using SupportWidgetXF.Models.Widgets;
using SupportWidgetXF.Widgets;
using SupportWidgetXF.Widgets.Interface;
using System.ComponentModel;

namespace SupportWidgetXF.Droid.Renderers
{
    public class SupportDropRenderer<TSupport, TOrignal> : ViewRenderer<TSupport, Android.Views.View>, IDropItemSelected where TSupport : SupportViewDrop where TOrignal : Android.Views.View
    {
        protected TSupport SupportView;
        protected TOrignal OriginalView;
        protected GradientDrawable gradientDrawable;
        protected ArrayAdapter dropItemAdapter;
        protected List<IAutoDropItem> SupportItemList = new List<IAutoDropItem>();

        public SupportDropRenderer(Context context) : base(context)
        {

        }

        protected virtual void OnInitializeAdapter()
        {

        }

        protected virtual void SyncItemSource()
        {
            SupportItemList.Clear();
            if (SupportView.ItemsSource != null)
            {
                SupportItemList.AddRange(SupportView.ItemsSource.ToList());
            }
        }

        protected virtual void NotifyAdapterChanged()
        {
            if (dropItemAdapter != null)
                dropItemAdapter.NotifyDataSetChanged();
        }

        protected virtual void OnInitializeOriginalView()
        {

        }

        protected virtual void OnInitializeBorderView()
        {
            gradientDrawable = new GradientDrawable();
            gradientDrawable.SetStroke((int)SupportView.CornerWidth, SupportView.CornerColor.ToPlatform());
            gradientDrawable.SetShape(ShapeType.Rectangle);
            gradientDrawable.SetCornerRadius((float)SupportView.CornerRadius);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TSupport> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && e.NewElement is TSupport)
            {
                SupportView = e.NewElement as TSupport;
                OnInitializeBorderView();
                OnInitializeOriginalView();
                SyncItemSource();
                OnInitializeAdapter();
                NotifyAdapterChanged();
                SetNativeControl(OriginalView);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName.Equals(nameof(SupportViewDrop.ItemsSource)))
            {
                SyncItemSource();
                NotifyAdapterChanged();
            }
            else if (e.PropertyName.Equals(nameof(SupportViewDrop.RefreshList)))
            {
                NotifyAdapterChanged();
            }
            else if (e.PropertyName.Equals(SupportViewDrop.RefreshListProperty.PropertyName))
            {
                NotifyAdapterChanged();
            }
        }

        public virtual void IF_ItemSelectd(int position)
        {

        }
    }
}