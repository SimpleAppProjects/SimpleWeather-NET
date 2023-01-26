﻿#if IOS || MACCATALYST
using CoreGraphics;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using SupportWidgetXF.iOS.Renderers.DropCombo;
using SupportWidgetXF.Models.Widgets;
using SupportWidgetXF.Widgets;
using SupportWidgetXF.Widgets.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UIKit;

namespace SupportWidgetXF.iOS.Renderers
{
    public class SupportDropRenderer<TSupport> : ViewRenderer<TSupport, UIView>, IDropItemSelected where TSupport : SupportViewDrop
    {
        protected TSupport SupportView;
        protected int HeightOfRow = 40;

        protected UITableView tableView;
        protected UITextField textField;
        protected bool FlagShow = false;

        protected List<IAutoDropItem> SupportItemList = new List<IAutoDropItem>();
        protected DropItemSource dropSource;

        public virtual void SyncItemSource()
        {
            SupportItemList.Clear();
            if (SupportView.ItemsSource != null)
            {
                SupportItemList.AddRange(SupportView.ItemsSource.ToList());
            }
            NotifyAdapterChanged();
        }

        public virtual void NotifyAdapterChanged()
        {
            if (tableView != null)
                tableView.ReloadData();
        }

        public virtual void OnInitialize()
        {

        }

        public virtual void IF_ItemSelectd(int position)
        {
            ShowData();
            if (position >= 0 && position < SupportItemList.Count)
            {
                var text = SupportItemList[position].IF_GetTitle();
                SupportView.Text = text;
            }
        }

        public virtual void OnInitializeTextField()
        {
            textField = new UITextField();
            textField.Frame = this.Frame;
            textField.Layer.CornerRadius = (float)SupportView.CornerRadius;
            textField.Layer.BorderWidth = (float)SupportView.CornerWidth;

            textField.Layer.BackgroundColor = SupportView.BackgroundColor.ToPlatform();
            //textField.BackgroundColor = UIColor.Clear;
            SupportView.BackgroundColor = Colors.Transparent;

            textField.Layer.BorderColor = SupportView.CornerColor.ToCGColor();
            textField.Font = UIFont.FromName(SupportView.FontFamily, (float)SupportView.FontSize);
            textField.Text = SupportView.Text;
        }

        public virtual void OnInitializeTableView()
        {
            tableView = new UITableView();
            tableView.AutoresizingMask = UIViewAutoresizing.All;
            tableView.Frame = textField.Frame;
            tableView.SeparatorColor = UIColor.Clear;
        }

        public virtual void OnInitializeTableSource()
        {
            dropSource = new DropItemSource(SupportItemList, SupportView, HeightOfRow, this);
            tableView.Source = dropSource;
        }

        public virtual void ShowData()
        {
            if (textField == null)
                return;

            FlagShow = !FlagShow;
            if (FlagShow)
            {
                var rect = textField.ConvertRectToView(textField.Frame, Window);
                nfloat height = Window.Bounds.Height - rect.Y - 10;
                CGRect r = new CGRect(rect.X, rect.Y, rect.Width, height);

                ShowSubviewAt(r, tableView, () =>
                {
                    tableView.Layer.MasksToBounds = !SupportView.HasShadow;
                });
            }
            else
            {
                HideData();
            }
        }

        public virtual void HideData()
        {
            if (tableView != null)
                tableView.RemoveFromSuperview();
        }

        public UIWindow GetCurrentWindow(UIView view)
        {
            if (view.Superview is UIWindow)
                return (UIWindow)view.Superview;
            else return GetCurrentWindow(view.Superview);
        }

        public virtual void ShowSubviewAt(CGRect rect, UIView subView, Action didFinishAnimation)
        {
            float height = HeightOfRow * SupportItemList.Count();
            var y = rect.Y + textField.Frame.Height + 2;
            if (height > rect.Height / 2)
                height = (float)rect.Height / 2;

            subView.Frame = new CGRect(rect.X, y, rect.Width, 0);
            UIView.Animate(0.2, () =>
            {
                subView.Frame = new CGRect(rect.X, y, rect.Width, height);
                subView.SetShadow(2f, 2, 0.8f);
                Window.AddSubview(subView);
            }, didFinishAnimation);
        }

        protected virtual void OnSetNativeControl()
        {
            SetNativeControl(textField);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TSupport> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && e.NewElement is TSupport)
            {
                SupportView = e.NewElement as TSupport;
                if (Control == null)
                {
                    OnInitializeTextField();
                    OnInitializeTableView();
                    SyncItemSource();
                    OnInitializeTableSource();
                    OnSetNativeControl();
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName.Equals(SupportViewBase.TextProperty.PropertyName))
            {
                if (textField != null && !textField.Text.Equals(SupportView.Text))
                {
                    textField.Text = SupportView.Text;
                }
            }
            else if (e.PropertyName.Equals(SupportViewDrop.ItemsSourceProperty.PropertyName))
            {
                SyncItemSource();
            }
            else if (e.PropertyName.Equals(SupportViewDrop.RefreshListProperty.PropertyName))
            {
                NotifyAdapterChanged();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && tableView != null) HideData();
            base.Dispose(disposing);
        }
    }
}
#endif