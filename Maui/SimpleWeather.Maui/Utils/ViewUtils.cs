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
						new CGSize(uiLabel.Frame.Size.Width, NFloat.MaxValue),
						NSStringDrawingOptions.UsesLineFragmentOrigin,
						new UIStringAttributes { Font = uiLabel.Font },
						null
					).Size;

					isTruncated = textSize.Height > uiLabel.Bounds.Size.Height;
                }	
			}
#endif

			return isTruncated;
		}
	}
}

