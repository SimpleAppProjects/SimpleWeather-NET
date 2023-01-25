using Android.Content;
using Android.Widget;
using SupportWidgetXF.Droid.Renderers.AutoComplete;
using SupportWidgetXF.Widgets;

namespace SupportWidgetXF.Droid.Renderers
{
    public class SupportAutoCompleteRenderer : SupportBaseAutoCompleteRenderer<SupportAutoComplete, AutoCompleteTextView>
    {
        public SupportAutoCompleteRenderer(Context context) : base(context)
        {
        }

        protected override void OnInitializeOriginalView()
        {
            OriginalView = new AutoCompleteTextView(Context);
            base.OnInitializeOriginalView();
        }
    }
}