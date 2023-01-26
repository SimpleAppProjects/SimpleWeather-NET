﻿#if IOS || MACCATALYST
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using SupportWidgetXF.Widgets;
using System;
using System.ComponentModel;
using System.Linq;
using UIKit;

namespace SupportWidgetXF.iOS.Renderers.AutoComplete
{
    public class SupportBaseAutoCompleteRenderer<TSupportAutoComplete> : SupportDropRenderer<TSupportAutoComplete>
        where TSupportAutoComplete : SupportAutoComplete

    {
        public SupportBaseAutoCompleteRenderer()
        {
        }

        protected virtual void RunFilterAutocomplete(string text)
        {
            SupportItemList.Clear();
            if (text != null && text.Length > 1 && SupportView.ItemsSource != null)
            {
                var key = text.ToLower();
                var result = SupportView.ItemsSource.ToList().Where(x => x.IF_GetTitle().ToLower().Contains(key)).Take(30);
                SupportItemList.AddRange(result);

                var Count = SupportItemList.Count;
                if (Count > 0)
                {
                    FlagShow = false;
                    tableView.ReloadData();
                    ShowData();
                }
            }
            else
            {
                HideData();
            }
        }

        protected virtual void OnInitializePlaceHolderTextField()
        {
            if (textField != null)
            {
                textField.AttributedPlaceholder = new NSAttributedString(SupportView.Placeholder, font: UIFont.FromName(SupportView.FontFamily, size: (float)SupportView.FontSize), foregroundColor: SupportView.PlaceHolderColor.ToPlatform());
                textField.Placeholder = SupportView.Placeholder;
            }
        }

        public override void OnInitializeTextField()
        {
            base.OnInitializeTextField();
            OnInitializePlaceHolderTextField();
            textField.LeftView = new UIView(new CGRect(0, 0, SupportView.PaddingInside, 0));
            textField.LeftViewMode = UITextFieldViewMode.Always;

            textField.EditingChanged += Wrapper_EditingChanged; ;
            textField.ShouldEndEditing += Wrapper_ShouldEndEditing;
            textField.ShouldBeginEditing += Wrapper_ShouldBeginEditing;
            textField.ShouldReturn += (textField) =>
            {
                SupportView.SendOnReturnKeyClicked();
                return true;
            };
            textField.InitlizeReturnKey(SupportView.ReturnType);
        }

        protected virtual void Wrapper_EditingChanged(object sender, EventArgs e)
        {
            var textFieldInput = sender as UITextField;
            SupportView.SendOnTextChanged(textFieldInput.Text);
            RunFilterAutocomplete(textFieldInput.Text);
        }

        protected virtual bool Wrapper_ShouldBeginEditing(UITextField textFieldInput)
        {
            SupportView.SendOnTextFocused(true);
            return true;
        }

        protected virtual bool Wrapper_ShouldEndEditing(UITextField textFieldInput)
        {
            HideData();
            SupportView.SendOnTextFocused(false);
            return true;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName.Equals(SupportAutoComplete.CurrentCornerColorProperty.PropertyName))
            {
                textField.Layer.BorderColor = SupportView.CurrentCornerColor.ToPlatform();
            }
            else if (e.PropertyName.Equals(SupportAutoComplete.PlaceHolderColorProperty.PropertyName))
            {
                OnInitializePlaceHolderTextField();
            }
        }
    }
}
#endif