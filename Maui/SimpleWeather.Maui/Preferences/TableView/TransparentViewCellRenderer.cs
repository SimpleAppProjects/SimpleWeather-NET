#if __IOS__
using System;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using SimpleWeather.Maui.Preferences;
using UIKit;

[assembly: ExportCell(typeof(TransparentViewCell), typeof(TransparentViewCellRenderer))]
namespace SimpleWeather.Maui.Preferences
{
	public class TransparentViewCellRenderer : ViewCellRenderer
	{
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }
    }
}
#endif
