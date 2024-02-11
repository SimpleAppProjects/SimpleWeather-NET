#if __IOS__
using System;
using System.ComponentModel;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Platform;
using SimpleWeather.Maui.Preferences;
using UIKit;

[assembly: ExportCell(typeof(TextCell), typeof(NonReusableTextCellRenderer))]
namespace SimpleWeather.Maui.Preferences
{
	public class NonReusableTextCellRenderer : CellRenderer
	{
        private readonly Color DefaultDetailColor = SecondaryLabelColor.ToColor();
        private readonly Color DefaultTextColor = LabelColor.ToColor();

        private static UIColor LabelColor
        {
            get
            {
                if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsTvOSVersionAtLeast(13))
                    return UIColor.Label;

                return UIColor.Black;
            }
        }

        private static UIColor SecondaryLabelColor
        {
            get
            {

                if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsTvOSVersionAtLeast(13))
                    return UIColor.SecondaryLabel;

                return new Color(.32f, .4f, .57f).ToPlatform();
            }
        }

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var textCell = (TextCell)item;

            var tvc = new CellTableViewCell(UITableViewCellStyle.Subtitle, item.GetType().FullName);

            SetRealCell(item, tvc);

            tvc.Cell = textCell;
            tvc.PropertyChanged = HandleCellPropertyChanged;

#pragma warning disable CA1416, CA1422 // TODO: 'UITableViewCell.TextLabel', DetailTextLabel is unsupported on: 'ios' 14.0 and later
            tvc.TextLabel.Text = textCell.Text;
            tvc.DetailTextLabel.Text = textCell.Detail;
            tvc.TextLabel.TextColor = (textCell.TextColor ?? DefaultTextColor).ToPlatform();
            tvc.DetailTextLabel.TextColor = (textCell.DetailColor ?? DefaultDetailColor).ToPlatform();

            WireUpForceUpdateSizeRequested(item, tvc, tv);

            UpdateIsEnabled(tvc, textCell);
#pragma warning restore CA1416, CA1422

            UpdateBackground(tvc, item);

            SetAccessibility(tvc, item);
            UpdateAutomationId(tvc, textCell);

            return tvc;
        }

        protected virtual void HandleCellPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var textCell = (TextCell)sender;
            var tvc = (CellTableViewCell)GetRealCell(textCell);

#pragma warning disable CA1416, CA1422 // TODO: 'UITableViewCell.TextLabel', DetailTextLabel is unsupported on: 'ios' 14.0 and later
            if (args.PropertyName == TextCell.TextProperty.PropertyName)
            {
                tvc.TextLabel.Text = ((TextCell)tvc.Cell).Text;
                tvc.TextLabel.SizeToFit();
            }
            else if (args.PropertyName == TextCell.DetailProperty.PropertyName)
            {
                tvc.DetailTextLabel.Text = ((TextCell)tvc.Cell).Detail;
                tvc.DetailTextLabel.SizeToFit();
            }
            else if (args.PropertyName == TextCell.TextColorProperty.PropertyName)
                tvc.TextLabel.TextColor = textCell.TextColor?.ToPlatform() ?? DefaultTextColor.ToPlatform();
            else if (args.PropertyName == TextCell.DetailColorProperty.PropertyName)
                tvc.DetailTextLabel.TextColor = textCell.DetailColor?.ToPlatform() ?? DefaultTextColor.ToPlatform();
            else if (args.PropertyName == Cell.IsEnabledProperty.PropertyName)
                UpdateIsEnabled(tvc, textCell);
            else if (args.PropertyName == TextCell.AutomationIdProperty.PropertyName)
                UpdateAutomationId(tvc, textCell);
#pragma warning restore CA1416, CA1422

            HandlePropertyChanged(tvc, args);
        }

        protected virtual void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            //keeping this method for backwards compatibility 
            //as the the sender for this method is a CellTableViewCell
        }

        internal static readonly BindableProperty RealCellProperty = BindableProperty.CreateAttached("RealCell", typeof(UITableViewCell), typeof(Cell), null);

        internal static UITableViewCell GetRealCell(BindableObject cell)
        {
            return (UITableViewCell)cell.GetValue(RealCellProperty);
        }

        internal static void SetRealCell(BindableObject cell, UITableViewCell renderer)
        {
            cell.SetValue(RealCellProperty, renderer);
        }

        private void UpdateAutomationId(CellTableViewCell tvc, TextCell cell)
        {
            tvc.AccessibilityIdentifier = cell.AutomationId;
        }

        [System.Runtime.Versioning.UnsupportedOSPlatform("ios14.0")]
        [System.Runtime.Versioning.UnsupportedOSPlatform("tvos14.0")]
        private static void UpdateIsEnabled(CellTableViewCell cell, TextCell entryCell)
        {
            cell.UserInteractionEnabled = entryCell.IsEnabled;
            cell.TextLabel.Enabled = entryCell.IsEnabled;
            cell.DetailTextLabel.Enabled = entryCell.IsEnabled;
        }
    }
}
#endif
