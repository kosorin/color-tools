using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Koda.ColorTools.BrushSources
{
    public abstract class GradientBrushSource : PickerBrushSource
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(GradientBrushSource),
                new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        private readonly LinearGradientBrush _brush;

        protected GradientBrushSource(int stopCount)
        {
            _brush = CreateBrush(stopCount);

            OnOrientationChanged();

            Value = _brush;
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            var stops = _brush.GradientStops;
            var stopCount = stops.Count;

            for (var i = 0; i < stopCount; i++)
            {
                var value = i / (double)(stopCount - 1);
                stops[i].Color = GetColor(value, picker);
            }
        }

        protected abstract Color GetColor(double value, IColorState state);

        private static void OnOrientationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is GradientBrushSource gradientBrushSource)
            {
                gradientBrushSource.OnOrientationChanged();
            }
        }

        private static LinearGradientBrush CreateBrush(int stopCount)
        {
            var stops = Enumerable.Range(0, stopCount).Select(x => new GradientStop(DefaultColor, x / (double)(stopCount - 1)));
            return new LinearGradientBrush(new GradientStopCollection(stops));
        }

        private void OnOrientationChanged()
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    // Left to right
                    _brush.StartPoint = new Point(0, 0);
                    _brush.EndPoint = new Point(1, 0);
                    break;
                case Orientation.Vertical:
                    // Bottom to top
                    _brush.StartPoint = new Point(0, 1);
                    _brush.EndPoint = new Point(0, 0);
                    break;
            }
        }
    }
}
