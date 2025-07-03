using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SimpleWeather.Maui.Utils
{
	public static class ViewUtils
	{
		public static bool IsTextTruncated(this Label label)
		{
			var isTruncated = false;

#if __IOS__
			if (label.Handler is IPlatformViewHandler handler && handler.PlatformView is UIKit.UILabel uiLabel)
			{
				var text = uiLabel.Text;

				if (!string.IsNullOrWhiteSpace(text))
				{
					var str = new NSString(text);
					var textSize = str.GetBoundingRect(
						new CGSize(uiLabel.Bounds.Size.Width, NFloat.MaxValue),
						NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading,
						new UIStringAttributes { Font = uiLabel.Font },
						null
					).Size;

                    isTruncated = Math.Ceiling(textSize.Height) >= uiLabel.Bounds.Height - label.Padding.VerticalThickness;
                }	
			}
#endif

			return isTruncated;
		}

		public static Label TextCenterHorizontal(this Label label)
		{
			label.HorizontalTextAlignment = TextAlignment.Center;
			return label;
		}

		public static Label TextCenterVertical(this Label label)
		{
			label.VerticalTextAlignment = TextAlignment.Center;
			return label;
		}
	}
}

