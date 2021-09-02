using System.Windows;

namespace ColorTools
{
    public class ColorState : DependencyObject, IColorState
    {
        public static readonly IColorState Empty = new ColorState(new RgbColor(255, 255, 255));

        public static readonly DependencyProperty SubscribeProperty =
            DependencyProperty.RegisterAttached("Subscribe", typeof(IColorState), typeof(ColorState),
                new PropertyMetadata(null, OnSubscribePropertyChanged));

        public ColorState(IColor color)
        {
            Update(color);
        }

        public RgbColor Rgb { get; private set; }

        public HsbColor Hsb { get; private set; }

        public HslColor Hsl { get; private set; }

        public void Update(IColor color)
        {
            Rgb = color.ToRgb();
            Hsb = color.ToHsb();
            Hsl = color.ToHsl();

            Changed?.Invoke(this);
        }

        public static IColorState? GetSubscribe(DependencyObject obj)
        {
            return (IColorState?)obj.GetValue(SubscribeProperty);
        }

        public static void SetSubscribe(DependencyObject obj, IColorState? value)
        {
            obj.SetValue(SubscribeProperty, value);
        }

        private static void OnSubscribePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is IColorStateSubscriber subscriber)
            {
                if (args.OldValue is IColorState oldColorState)
                {
                    oldColorState.Changed -= subscriber.HandleColorStateChanged;
                }

                if (args.NewValue is IColorState newColorState)
                {
                    subscriber.HandleColorStateChanged(newColorState);
                    newColorState.Changed += subscriber.HandleColorStateChanged;
                }
            }
        }

        public event ColorStateChanged? Changed;
    }
}
