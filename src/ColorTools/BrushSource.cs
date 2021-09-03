using System.Windows;
using System.Windows.Media;

namespace ColorTools
{
    public abstract class BrushSource : FrameworkElement
    {
        protected static readonly Color DefaultColor = Colors.Transparent;

        private static readonly DependencyPropertyKey ValuePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Value), typeof(Brush), typeof(BrushSource), new PropertyMetadata(new SolidColorBrush(DefaultColor)));

        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;
        
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(ColorState), typeof(BrushSource),
                new PropertyMetadata(new ColorState(), OnStatePropertyChanged));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.RegisterAttached("Background", typeof(BrushSource), typeof(BrushSource), new PropertyMetadata(null));

        public Brush Value
        {
            get => (Brush)GetValue(ValueProperty);
            protected set => SetValue(ValuePropertyKey, value);
        }
        
        public ColorState State
        {
            get => (ColorState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public static BrushSource GetBackground(DependencyObject obj)
        {
            return (BrushSource)obj.GetValue(BackgroundProperty);
        }

        public static void SetBackground(DependencyObject obj, BrushSource? value)
        {
            obj.SetValue(BackgroundProperty, value);
        }

        private static void OnStatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is BrushSource brushSource)
            {
                if (args.OldValue is ColorState oldState)
                {
                    oldState.Changed -= brushSource.OnStateChanged;
                }

                if (args.NewValue is ColorState newState)
                {
                    brushSource.OnStateChanged(newState);
                    newState.Changed += brushSource.OnStateChanged;
                }
            }
        }

        protected abstract void OnStateChanged(ColorState state);
    }
}
