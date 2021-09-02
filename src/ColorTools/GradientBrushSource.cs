using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorTools
{
    public abstract class GradientBrushSource : BrushSource
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

        protected override void Update(IColorState colorState)
        {
            var stops = _brush.GradientStops;
            var stopCount = stops.Count;

            for (var i = 0; i < stopCount; i++)
            {
                var value = i / (double)(stopCount - 1);
                stops[i].Color = GetColor(value, colorState);
            }
        }

        protected abstract Color GetColor(double value, IColorState colorState);

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

    public class AlphaGradientBrushSource : GradientBrushSource
    {
        public AlphaGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return colorState.Rgb.ToColor((byte)(value * 255));
        }
    }

    public class RgbRedGradientBrushSource : GradientBrushSource
    {
        public RgbRedGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new RgbColor(value * 255, colorState.Rgb.G, colorState.Rgb.B).ToColor();
        }
    }

    public class RgbGreenGradientBrushSource : GradientBrushSource
    {
        public RgbGreenGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new RgbColor(colorState.Rgb.R, value * 255, colorState.Rgb.B).ToColor();
        }
    }

    public class RgbBlueGradientBrushSource : GradientBrushSource
    {
        public RgbBlueGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new RgbColor(colorState.Rgb.R, colorState.Rgb.G, value * 255).ToColor();
        }
    }

    public class HsbHueGradientBrushSource : GradientBrushSource
    {
        public HsbHueGradientBrushSource() : base(7)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new HsbColor(value * 360, colorState.Hsb.S, colorState.Hsb.B).ToColor();
        }
    }

    public class HsbSaturationGradientBrushSource : GradientBrushSource
    {
        public HsbSaturationGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new HsbColor(colorState.Hsb.H, value * 100, colorState.Hsb.B).ToColor();
        }
    }

    public class HsbBrightnessGradientBrushSource : GradientBrushSource
    {
        public HsbBrightnessGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new HsbColor(colorState.Hsb.H, colorState.Hsb.S, value * 100).ToColor();
        }
    }

    public class HslHueGradientBrushSource : GradientBrushSource
    {
        public HslHueGradientBrushSource() : base(7)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new HslColor(value * 360, colorState.Hsl.S, colorState.Hsl.L).ToColor();
        }
    }

    public class HslSaturationGradientBrushSource : GradientBrushSource
    {
        public HslSaturationGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new HslColor(colorState.Hsl.H, value * 100, colorState.Hsl.L).ToColor();
        }
    }

    public class HslLightnessGradientBrushSource : GradientBrushSource
    {
        public HslLightnessGradientBrushSource() : base(3)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new HslColor(colorState.Hsl.H, colorState.Hsl.S, value * 100).ToColor();
        }
    }

    public class HueGradientBrushSource : GradientBrushSource
    {
        public HueGradientBrushSource() : base(7)
        {
        }

        protected override Color GetColor(double value, IColorState colorState)
        {
            return new HsbColor(value * 360, 100, 100).ToColor();
        }
    }
}