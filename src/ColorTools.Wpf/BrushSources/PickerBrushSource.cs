using System.Windows;

namespace Koda.ColorTools.Wpf.BrushSources
{
    public abstract class PickerBrushSource : BrushSource
    {
        public static readonly DependencyProperty PickerProperty =
            DependencyProperty.Register(nameof(Picker), typeof(IColorPicker), typeof(BrushSource),
                new PropertyMetadata(null, OnPickerPropertyChanged));

        protected virtual ColorPickerParts PickerParts => ColorPickerParts.Color;

        public IColorPicker? Picker
        {
            get => (IColorPicker?)GetValue(PickerProperty);
            set => SetValue(PickerProperty, value);
        }

        private static void OnPickerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is PickerBrushSource brushSource)
            {
                if (args.OldValue is IColorPicker oldPicker)
                {
                    oldPicker.Changed -= brushSource.OnPickerChangedCore;
                }

                if (args.NewValue is IColorPicker newPicker)
                {
                    brushSource.OnPickerChangedCore(newPicker, ColorPickerParts.All);
                    newPicker.Changed += brushSource.OnPickerChangedCore;
                }
            }
        }

        private void OnPickerChangedCore(IColorPicker picker, ColorPickerParts parts)
        {
            if ((parts & PickerParts) == ColorPickerParts.None)
            {
                return;
            }

            OnPickerChanged(picker, parts);
        }

        protected abstract void OnPickerChanged(IColorPicker picker, ColorPickerParts parts);
    }
}
