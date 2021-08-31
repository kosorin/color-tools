using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ColorTools
{
    public class SliderBrushSource : BrushSource
    {
        private readonly LinearGradientBrush _brush;
        private readonly ColorFactory<double> _stopColorFactory;

        public SliderBrushSource(IColorState colorState, int stopCount, ColorFactory<double> stopColorFactory)
            : base(colorState)
        {
            _stopColorFactory = stopColorFactory;

            _brush = CreateBrush(stopCount);
        }

        public override Brush Brush => _brush;

        public override void Update()
        {
            var stops = _brush.GradientStops;
            var stopCount = stops.Count;

            for (var i = 0; i < stopCount; i++)
            {
                var value = i / (double)(stopCount - 1);
                stops[i].Color = _stopColorFactory.Invoke(value, ColorState);
            }
        }

        private static LinearGradientBrush CreateBrush(int stopCount)
        {
            var stops = Enumerable.Range(0, stopCount).Select(x => new GradientStop(DefaultColor, x / (double)(stopCount - 1)));
            return new LinearGradientBrush(new GradientStopCollection(stops), new Point(0, 0), new Point(1, 0));
        }
    }
}
