using System;
using Foundation;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Platform;
using NodaTime.TimeZones.Cldr;
using UIKit;
using TimePicker = Microsoft.Maui.Controls.TimePicker;

namespace SimpleWeather.Maui
{
    public class NativeTimePickerHandler : ViewHandler<ITimePicker, UIDatePicker>
    {
        private readonly UIDatePickerProxy _proxy = new();

        public static IPropertyMapper<ITimePicker, NativeTimePickerHandler> Mapper = new PropertyMapper<ITimePicker, NativeTimePickerHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ITimePicker.FlowDirection)] = MapFlowDirection,
            [nameof(ITimePicker.Format)] = MapFormat,
            [nameof(ITimePicker.TextColor)] = MapTextColor,
            [nameof(ITimePicker.Time)] = MapTime,
            [Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.TimePicker.UpdateModeProperty.PropertyName] = MapUpdateMode
        };

        public static CommandMapper<ITimePicker, NativeTimePickerHandler> CommandMapper = new(ViewCommandMapper)
        {
        };

        public NativeTimePickerHandler() : base(Mapper, CommandMapper)
        {
        }

        public NativeTimePickerHandler(IPropertyMapper? mapper)
            : base(mapper ?? Mapper, CommandMapper)
        {
        }

        public NativeTimePickerHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
            : base(mapper ?? Mapper, commandMapper ?? CommandMapper)
        {
        }

        internal bool UpdateImmediately { get; set; } = true;

        protected override UIDatePicker CreatePlatformView()
        {
            return new UIDatePicker
            {
                Mode = UIDatePickerMode.Time,
                TimeZone = new NSTimeZone("UTC"),
                PreferredDatePickerStyle = DeviceInfo.Idiom == DeviceIdiom.Desktop ? UIDatePickerStyle.Inline : UIDatePickerStyle.Compact
            };
        }

        protected override void ConnectHandler(UIDatePicker platformView)
        {
            base.ConnectHandler(platformView);

            _proxy.Connect(this, VirtualView, platformView);
        }

        protected override void DisconnectHandler(UIDatePicker platformView)
        {
            base.DisconnectHandler(platformView);

            _proxy.Disconnect(platformView);
        }

        public static void MapFormat(NativeTimePickerHandler handler, ITimePicker timePicker)
        {
            handler.PlatformView?.UpdateFormat(timePicker);
        }

        public static void MapTime(NativeTimePickerHandler handler, ITimePicker timePicker)
        {
            handler.PlatformView?.UpdateTime(timePicker);
        }

        public static void MapTextColor(NativeTimePickerHandler handler, ITimePicker timePicker)
        {
            handler.PlatformView?.SetValueForKeyPath(handler.VirtualView.TextColor.ToPlatform(), NSObject.FromObject("textColor") as NSString);
            handler.PlatformView?.SetValueForKeyPath(NSObject.FromObject(false), NSObject.FromObject("highlightsToday") as NSString);
        }

        public static void MapFlowDirection(NativeTimePickerHandler handler, ITimePicker timePicker)
        {
            handler.PlatformView?.UpdateFlowDirection(timePicker);
        }

        public static void MapUpdateMode(NativeTimePickerHandler handler, ITimePicker timePicker)
        {
            if (timePicker is TimePicker t)
                handler.UpdateImmediately = t.OnThisPlatform().UpdateMode() == UpdateMode.Immediately;
        }

        void SetVirtualViewTime()
        {
            if (VirtualView == null || PlatformView == null)
                return;

            var datetime = PlatformView.Date.ToDateTime();
            VirtualView.Time = new TimeSpan(datetime.Hour, datetime.Minute, 0);
        }

        class UIDatePickerProxy
        {
            WeakReference<NativeTimePickerHandler>? _handler;
            WeakReference<ITimePicker>? _virtualView;

            ITimePicker? VirtualView => _virtualView is not null && _virtualView.TryGetTarget(out var v) ? v : null;

            public void Connect(NativeTimePickerHandler handler, ITimePicker virtualView, UIDatePicker platformView)
            {
                _handler = new(handler);
                _virtualView = new(virtualView);

                platformView.EditingDidBegin += OnStarted;
                platformView.EditingDidEnd += OnEnded;
                platformView.ValueChanged += OnValueChanged;
            }

            public void Disconnect(UIDatePicker platformView)
            {
                _virtualView = null;

                platformView.EditingDidBegin -= OnStarted;
                platformView.EditingDidEnd -= OnEnded;
                platformView.ValueChanged -= OnValueChanged;
                platformView.RemoveFromSuperview();
            }

            void OnStarted(object? sender, EventArgs eventArgs)
            {
                if (VirtualView is ITimePicker virtualView)
                    virtualView.IsFocused = true;
            }

            void OnEnded(object? sender, EventArgs eventArgs)
            {
                if (VirtualView is ITimePicker virtualView)
                    virtualView.IsFocused = false;

                if (_handler is not null && _handler.TryGetTarget(out var handler))
                {
                    handler.SetVirtualViewTime();
                }
            }

            void OnValueChanged(object? sender, EventArgs e)
            {
                if (_handler is not null && _handler.TryGetTarget(out var handler) && handler.UpdateImmediately)
                {
                    handler.SetVirtualViewTime();
                }
            }
        }
    }
}

