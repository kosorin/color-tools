using System;
using System.Windows;
using System.Windows.Media;

namespace ColorTools
{
    public class ColorComponent
    {
        private readonly Action _valueChangedCallback;

        private double _value;
        private ColorSlider? _slider;

        public ColorComponent(Action valueChangedCallback)
        {
            _valueChangedCallback = valueChangedCallback;
        }

        public double Value
        {
            get => _value;
            private set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_value != value)
                {
                    _value = value;
                    _valueChangedCallback.Invoke();
                }

                if (_slider != null)
                {
                    _slider.Value = value;
                }
            }
        }

        public void SetValue(double value, Func<double, Color>? gradientColorFactory = null)
        {
            Value = value;

            if (_slider != null && gradientColorFactory != null)
            {
                _slider.UpdateGradient(gradientColorFactory);
            }
        }

        public void AttachSlider(ColorSlider? slider)
        {
            if (_slider != null)
            {
                _slider.ValueChanged -= Slider_ValueChanged;
            }

            _slider = slider;

            if (_slider != null)
            {
                _slider.Value = Value;
                _slider.ValueChanged += Slider_ValueChanged;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            Value = args.NewValue;
        }
    }
}
