using System.Windows;
using System.Windows.Media;

namespace ColorTools
{
    public abstract class BrushSource : FrameworkElement, IColorStateSubscriber
    {
        protected static readonly Color DefaultColor = Colors.Transparent;

        private static readonly DependencyPropertyKey ValuePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Value), typeof(Brush), typeof(BrushSource), new PropertyMetadata(new SolidColorBrush(DefaultColor)));

        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.RegisterAttached("Background", typeof(BrushSource), typeof(BrushSource), new PropertyMetadata(null));

        public Brush Value
        {
            get => (Brush)GetValue(ValueProperty);
            protected set => SetValue(ValuePropertyKey, value);
        }

        public static BrushSource GetBackground(DependencyObject obj)
        {
            return (BrushSource)obj.GetValue(BackgroundProperty);
        }

        public static void SetBackground(DependencyObject obj, BrushSource? value)
        {
            obj.SetValue(BackgroundProperty, value);
        }

        protected abstract void Update(IColorState colorState);

        void IColorStateSubscriber.HandleColorStateChanged(IColorState colorState)
        {
            Update(colorState);
        }
    }
}
