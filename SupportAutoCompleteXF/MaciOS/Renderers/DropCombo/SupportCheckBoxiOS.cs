﻿#if IOS || MACCATALYST
using Foundation;
using System;
using System.ComponentModel;
using UIKit;

namespace SupportWidgetXF.iOS.Renderers.DropCombo
{
    [Register("SupportCheckBoxiOS"), DesignTimeVisible(true)]
    public partial class SupportCheckBoxiOS : UIButton, INotifyPropertyChanged
    {
        public SupportCheckBoxiOS() : base() { }

        public SupportCheckBoxiOS(IntPtr handle) : base(handle) { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _Checked;
        [Export("Checked")]
        public bool Checked
        {
            get => _Checked;
            set
            {
                _Checked = value;
                OnPropertyChanged("IsCheck");
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            if (TitleLabel.Text != null && TitleLabel.Text.Length > 0)
                CenterImageAndTitle(7f);
            else
                CenterImageAndTitle(7f);
            HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            //LineBreakMode = UILineBreakMode.TailTruncation;
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals("IsCheck"))
                {
                    if (Checked)
                    {
                        //Font = UIFont.BoldSystemFontOfSize(13f);
                        SetImage(UIImage.FromBundle("checkbox_selected").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                    }
                    else
                    {
                        //Font = UIFont.SystemFontOfSize(13f);
                        SetImage(UIImage.FromBundle("checkbox_empty").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                    }
                }
            };

            Checked = false;
        }
        public void CenterImageAndTitle(nfloat spacing)
        {
            ImageEdgeInsets = new UIEdgeInsets(5, 0, 5, spacing);
            TitleEdgeInsets = new UIEdgeInsets(0, spacing, 0, 0);
        }
    }
}
#endif