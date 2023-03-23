using System;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls
{
	public class LineBreakSpan : Span
	{
		public LineBreakSpan()
		{
			Text = Environment.NewLine;
		}
	}
}

