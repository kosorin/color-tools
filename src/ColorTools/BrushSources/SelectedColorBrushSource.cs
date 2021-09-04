using System.Windows;
using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public class SelectedColorBrushSource : BrushSource
    {
        public static readonly DependencyProperty EmptyBrushProperty =
            DependencyProperty.Register(nameof(EmptyBrush), typeof(Brush), typeof(SelectedColorBrushSource),
                new PropertyMetadata(Brushes.White, OnEmptyBrushPropertyChanged));

        private readonly SolidColorBrush _brush;

        public SelectedColorBrushSource()
        {
            _brush = new SolidColorBrush(DefaultColor);

            Value = _brush;
        }

        protected override ColorPickerParts PickerParts => ColorPickerParts.All;

        public Brush EmptyBrush
        {
            get => (Brush)GetValue(EmptyBrushProperty);
            set => SetValue(EmptyBrushProperty, value);
        }

        private static void OnEmptyBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is SelectedColorBrushSource brushSource)
            {
                brushSource.OnEmptyBrushChanged();
            }
        }

        private void OnEmptyBrushChanged()
        {
            if (Picker?.IsSet == false)
            {
                Value = EmptyBrush;
            }
        }

        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            if ((parts & ColorPickerParts.Color) != ColorPickerParts.None)
            {
                _brush.Color = picker.Color.ToColor();
            }

            if ((parts & ColorPickerParts.Alpha) != ColorPickerParts.None)
            {
                _brush.Opacity = picker.Alpha;
            }

            if ((parts & ColorPickerParts.IsSet) != ColorPickerParts.None)
            {
                Value = picker.IsSet ? _brush : EmptyBrush;
            }
        }
    }
}
