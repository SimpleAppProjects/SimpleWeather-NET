using Microsoft.Maui.Layouts;
using System;

namespace SimpleWeather.Maui.Controls
{
	public interface IFlowLayout : Microsoft.Maui.ILayout
    {
		double LineSpacing { get; }
		double ItemSpacing { get; }
		StackOrientation Orientation { get; }
	}
}

