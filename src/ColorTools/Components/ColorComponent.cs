using System.Windows;
using System.Windows.Controls;

namespace ColorTools.Components
{
    public abstract class ColorComponent : Control
    {
        public static readonly DependencyProperty PickerProperty =
            DependencyProperty.Register(nameof(Picker), typeof(IColorPicker), typeof(ColorComponent),
                new PropertyMetadata(null, OnPickerPropertyChanged));

        private bool _isUpdating;

        public IColorPicker? Picker
        {
            get => (IColorPicker?)GetValue(PickerProperty);
            set => SetValue(PickerProperty, value);
        }

        private static void OnPickerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorComponent component)
            {
                if (args.OldValue is IColorPicker oldPicker)
                {
                    oldPicker.AlphaChanged -= component.OnAlphaChangedWrapper;
                    oldPicker.ColorChanged -= component.OnColorChangedWrapper;
                }

                if (args.NewValue is IColorPicker newPicker)
                {
                    component.OnAlphaChangedWrapper(newPicker, newPicker.Alpha);
                    component.OnColorChangedWrapper(newPicker, newPicker.Color);

                    newPicker.AlphaChanged += component.OnAlphaChangedWrapper;
                    newPicker.ColorChanged += component.OnColorChangedWrapper;
                }
            }
        }

        private void OnAlphaChangedWrapper(IColorPicker picker, double alpha)
        {
            if (_isUpdating)
            {
                return;
            }

            _isUpdating = false;
            try
            {
                OnAlphaChanged(picker, alpha);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void OnColorChangedWrapper(IColorPicker picker, IColor color)
        {
            if (_isUpdating)
            {
                return;
            }

            _isUpdating = true;
            try
            {
                OnColorChanged(picker, color);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        protected virtual void OnAlphaChanged(IColorPicker picker, double alpha)
        {
        }

        protected virtual void OnColorChanged(IColorPicker picker, IColor color)
        {
        }

        protected void TryUpdatePicker()
        {
            if (_isUpdating || Picker == null)
            {
                return;
            }

            UpdatePicker(Picker);
        }

        protected abstract void UpdatePicker(IColorPicker picker);
    }
}
