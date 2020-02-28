using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SimpleWeather.UWP.Helpers
{
    public class ControlSizeTrigger : StateTriggerBase
    {
        private FrameworkElement _targetElement;
        private double _currentHeight, _currentWidth;

        //public properties to set from XAML
        public double MinHeight { get; set; } = -1;

        public double MinWidth { get; set; } = -1;

        public FrameworkElement TargetElement
        {
            get
            {
                return _targetElement;
            }
            set
            {
                if (_targetElement != null)
                {
                    _targetElement.SizeChanged -= _targetElement_SizeChanged;
                }
                _targetElement = value;
                if (_targetElement != null)
                {
                    _targetElement.SizeChanged += _targetElement_SizeChanged;
                    _currentHeight = _targetElement.ActualHeight;
                    _currentWidth = _targetElement.ActualWidth;
                    UpdateTrigger();
                }
            }
        }

        //Handle event to get current values
        private void _targetElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _currentHeight = e.NewSize.Height;
            _currentWidth = e.NewSize.Width;
            UpdateTrigger();
        }

        //Logic to evaluate and apply trigger value
        private void UpdateTrigger()
        {
            //if target is set and either minHeight or minWidth is set, proceed
            if (_targetElement != null && (MinWidth >= 0 || MinHeight >= 0))
            {
                //if both minHeight and minWidth are set, then both conditions must be satisfied
                if (MinHeight >= 0 && MinWidth >= 0)
                {
                    SetActive((_currentHeight >= MinHeight) && (_currentWidth >= MinWidth));
                }
                //if only one of them is set, then only that condition needs to be satisfied
                else if (MinHeight >= 0)
                {
                    SetActive(_currentHeight >= MinHeight);
                }
                else
                {
                    SetActive(_currentWidth >= MinWidth);
                }
            }
            else
            {
                SetActive(false);
            }
        }
    }
}