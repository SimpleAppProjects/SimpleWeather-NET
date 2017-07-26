using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SimpleWeather.Droid.Controls
{
    public class LoadingDialog : ProgressDialog
    {
        public LoadingDialog(Context context) :
            base(context)
        {
            Initialize();
        }

        public LoadingDialog(Context context, int theme) :
            base(context, theme)
        {
            Initialize();
        }

        private void Initialize()
        {
            SetMessage(Context.Resources.GetString(Resource.String.message_loading));
        }
    }
}