using System.Windows;
using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public class HslCanvasBrushSource : BrushSource
    {
        private readonly GradientStop _accentGradientStop;

        public HslCanvasBrushSource()
        {
            _accentGradientStop = new GradientStop(DefaultColor, 1);

            Value = CreateBrush(_accentGradientStop);
        }

        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            _accentGradientStop.Color = new HslColor(picker.Hsl.H, 100, 50).ToColor();
        }

        private static DrawingBrush CreateBrush(GradientStop accentGradientStop)
        {
            var viewport = new Rect(new Size(1, 1));

            var drawing = new DrawingGroup
            {
                Children = new DrawingCollection(new Drawing[]
                {
                    new GeometryDrawing
                    {
                        Brush = new LinearGradientBrush(new GradientStopCollection(new[]
                        {
                            new GradientStop(Color.FromArgb(255, 127, 127, 127), 0),
                            accentGradientStop,
                        }), new Point(0, 0), new Point(1, 0)),
                        Geometry = new RectangleGeometry(viewport),
                    },
                    new GeometryDrawing
                    {
                        Brush = new LinearGradientBrush(new GradientStopCollection(new[]
                        {
                            new GradientStop(Color.FromArgb(255, 255, 255, 255), 0),
                            new GradientStop(Color.FromArgb(0, 255, 255, 255), 0.5),
                            new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.5),
                            new GradientStop(Color.FromArgb(255, 0, 0, 0), 1),
                        }), new Point(0, 0), new Point(0, 1)),
                        Geometry = new RectangleGeometry(viewport),
                    },
                }),
            };

            return new DrawingBrush(drawing)
            {
                Viewport = viewport,
            };
        }
    }
}
