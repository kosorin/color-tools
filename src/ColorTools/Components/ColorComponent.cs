using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorTools.Components
{
    public abstract class ColorComponent : Control
    {
        public static readonly DependencyProperty PickerProperty =
            DependencyProperty.Register(nameof(Picker), typeof(IColorPicker), typeof(ColorComponent),
                new PropertyMetadata(null, OnPickerPropertyChanged));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ColorComponent), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ColorComponent), new PropertyMetadata(null));

        public static readonly DependencyProperty AlphaBrushProperty =
            DependencyProperty.Register(nameof(AlphaBrush), typeof(Brush), typeof(ColorComponent), new PropertyMetadata(Brushes.Transparent));

        private bool _isUpdating;

        protected virtual ColorPickerParts PickerParts => ColorPickerParts.Color;

        public IColorPicker? Picker
        {
            get => (IColorPicker?)GetValue(PickerProperty);
            set => SetValue(PickerProperty, value);
        }

        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public DataTemplate? HeaderTemplate
        {
            get => (DataTemplate?)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public Brush AlphaBrush
        {
            get => (Brush)GetValue(AlphaBrushProperty);
            set => SetValue(AlphaBrushProperty, value);
        }

        private static void OnPickerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorComponent component)
            {
                if (args.OldValue is IColorPicker oldPicker)
                {
                    oldPicker.EndUpdate();
                    oldPicker.Changed -= component.OnPickerChangedCore;
                }

                if (args.NewValue is IColorPicker newPicker)
                {
                    component.OnPickerChangedCore(newPicker, ColorPickerParts.All);
                    newPicker.Changed += component.OnPickerChangedCore;
                }
            }
        }

        private void OnPickerChangedCore(IColorPicker picker, ColorPickerParts parts)
        {
            if (_isUpdating)
            {
                return;
            }

            if ((parts & PickerParts) == ColorPickerParts.None)
            {
                return;
            }

            _isUpdating = true;
            try
            {
                OnPickerChanged(picker, parts);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        protected virtual void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
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
