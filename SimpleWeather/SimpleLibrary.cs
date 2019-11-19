using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace SimpleWeather
{
    public sealed class SimpleLibrary
    {
        private ResourceLoader ResourceLoader;

        private static SimpleLibrary sSimpleLib;

        private SimpleLibrary()
        {
            ResourceLoader = GetResourceLoader();
        }

        public static ResourceLoader ResLoader
        {
            get
            {
                Init();
                return sSimpleLib.ResourceLoader;
            }
        }

        private static void Init()
        {
            if (sSimpleLib == null)
                sSimpleLib = new SimpleLibrary();
        }

        private ResourceLoader GetResourceLoader()
        {
            if (Windows.UI.Core.CoreWindow.GetForCurrentThread() != null)
            {
                return ResourceLoader.GetForCurrentView();
            }
            else
            {
                return ResourceLoader.GetForViewIndependentUse();
            }
        }
    }
}