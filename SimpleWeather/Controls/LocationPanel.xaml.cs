﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather
{
    public sealed partial class LocationPanel : Button
    {
        public LocationPanel()
        {
            this.InitializeComponent();

            LoadIt();
        }

        private async void LoadIt()
        {
            LoadingRing.IsActive = true;

            while (this.LocalName.Text == null || this.LocalName.Text == string.Empty)
            {
                await System.Threading.Tasks.Task.Delay(200);
            }

            LoadingRing.IsActive = false;
        }
    }
}
