using System.Windows;
using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public abstract class BrushSource : FrameworkElement
    {
        protected static readonly Color DefaultColor = Colors.Transparent;

        private static readonly DependencyPropertyKey ValuePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Value), typeof(Brush), typeof(BrushSource), new PropertyMetadata(new SolidColorBrush(DefaultColor)));

        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        public static readonly DependencyProperty PickerProperty =
            DependencyProperty.Register(nameof(Picker), typeof(IColorPicker), typeof(BrushSource),
                new PropertyMetadata(null, OnPickerPropertyChanged));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.RegisterAttached("Background", typeof(BrushSource), typeof(BrushSource), new PropertyMetadata(null));

        protected virtual ColorPickerParts PickerParts => ColorPickerParts.Color;

        public Brush Value
        {
            get => (Brush)GetValue(ValueProperty);
            protected set => SetValue(ValuePropertyKey, value);
        }

        public IColorPicker? Picker
        {
            get => (IColorPicker?)GetValue(PickerProperty);
            set => SetValue(PickerProperty, value);
        }

        public static BrushSource GetBackground(DependencyObject obj)
        {
            return (BrushSource)obj.GetValue(BackgroundProperty);
        }

        public static void SetBackground(DependencyObject obj, BrushSource value)
        {
            obj.SetValue(BackgroundProperty, value);
        }

        private static void OnPickerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is BrushSource brushSource)
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
