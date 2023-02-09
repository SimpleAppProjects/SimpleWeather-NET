using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleWeather.Maui.MarkupExtensions
{
    [ContentProperty("Items")]
    [AcceptEmptyServiceProvider]
    public class ListExtension : IMarkupExtension<IList>
	{
        public ListExtension()
        {
            Items = new List<object>();
        }

        public IList Items { get; }

        public Type Type { get; set; }

        public IList ProvideValue(IServiceProvider serviceProvider)
        {
            if (Type == null)
                throw new InvalidOperationException("Type argument mandatory for List extension");

            if (Items == null)
                return null;

            return Items;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<IList>).ProvideValue(serviceProvider);
        }
    }
}

