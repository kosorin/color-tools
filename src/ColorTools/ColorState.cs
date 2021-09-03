using System.Windows;

namespace ColorTools
{
    public class ColorState : DependencyObject
    {
        public static readonly ColorState Empty = new ColorState();

        public static readonly DependencyProperty SubscribeProperty =
            DependencyProperty.RegisterAttached("Subscribe", typeof(ColorState), typeof(ColorState),
                new PropertyMetadata(null, OnSubscribePropertyChanged));

        public ColorState()
        {
            Update(new RgbColor(255, 255, 255));
        }

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

        public static ColorState? GetSubscribe(DependencyObject obj)
        {
            return (ColorState?)obj.GetValue(SubscribeProperty);
        }

        public static void SetSubscribe(DependencyObject obj, ColorState? value)
        {
            obj.SetValue(SubscribeProperty, value);
        }

        private static void OnSubscribePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is IColorStateSubscriber subscriber)
            {
                if (args.OldValue is ColorState oldColorState)
                {
                    oldColorState.Changed -= subscriber.HandleColorStateChanged;
                }

                if (args.NewValue is ColorState newColorState)
                {
                    subscriber.HandleColorStateChanged(newColorState);
                    newColorState.Changed += subscriber.HandleColorStateChanged;
                }
            }
        }

        public event ColorStateChanged? Changed;
    }
}
