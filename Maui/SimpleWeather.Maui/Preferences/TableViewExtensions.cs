using System;
using Microsoft.Maui.Controls;

namespace SimpleWeather.Maui.Preferences
{
	public static class TableViewExtensions
	{
		public static void UpdateCellColors(
            this TableView view,
            Color lightTextColor, Color darkTextColor,
            Color lightDetailColor, Color darkDetailColor,
            Color lightPrimaryColor, Color darkPrimaryColor)
		{
			var descendants = view.GetVisualTreeDescendants();

			foreach (var element in descendants)
			{
				if (element is TableSection section)
				{
                    //section.SetAppThemeColor(TableSection.TextColorProperty, lightPrimaryColor, darkPrimaryColor);
                }
                else if (element is SwitchCell sw)
                {
                    sw.SetAppThemeColor(SwitchCell.OnColorProperty, lightPrimaryColor, darkPrimaryColor);
                    sw.SetAppThemeColor(SwitchCell.TextColorProperty, lightTextColor, darkTextColor);
                    sw.SetAppThemeColor(SwitchCell.DetailColorProperty, lightDetailColor, darkDetailColor);
                }
                else if (element is TextCell txt)
                {
                    txt.SetAppThemeColor(TextCell.TextColorProperty, lightTextColor, darkTextColor);
                    txt.SetAppThemeColor(TextCell.DetailColorProperty, lightDetailColor, darkDetailColor);
                }
                else if (element is EntryCell entry)
                {
                    entry.SetAppThemeColor(EntryCell.LabelColorProperty, lightTextColor, darkTextColor);
                }
                else if (element is CheckBoxCell chk)
                {
                    chk.SetAppThemeColor(CheckBoxCell.ColorProperty, lightPrimaryColor, darkPrimaryColor);
                    chk.SetAppThemeColor(CheckBoxCell.TextColorProperty, lightTextColor, darkTextColor);
                    chk.SetAppThemeColor(CheckBoxCell.DetailColorProperty, lightDetailColor, darkDetailColor);
                }
                else if (element is PickerViewCell pvc)
                {
                    pvc.SetAppThemeColor(PickerViewCell.ColorProperty, lightPrimaryColor, darkPrimaryColor);
                    pvc.SetAppThemeColor(PickerViewCell.TextColorProperty, lightTextColor, darkTextColor);
                    pvc.SetAppThemeColor(PickerViewCell.DetailColorProperty, lightDetailColor, darkDetailColor);
                }
                else if (element is TextViewCell tvc)
                {
                    tvc.SetAppThemeColor(TextViewCell.TextColorProperty, lightTextColor, darkTextColor);
                    tvc.SetAppThemeColor(TextViewCell.DetailColorProperty, lightDetailColor, darkDetailColor);
                }

                if (element is ViewCell vc && vc.View is View v)
                {
                    v.SetAppThemeColor(View.BackgroundColorProperty, Colors.White, Color.Parse("#1B1B1B"));
                }
            }
        }
	}
}

