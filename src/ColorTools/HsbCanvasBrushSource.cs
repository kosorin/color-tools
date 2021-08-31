using System.Windows;
using System.Windows.Media;

namespace ColorTools
{
    public class HsbCanvasBrushSource : ComponentBrushSource
    {
        private readonly DrawingBrush _brush;
        private readonly SolidColorBrush _accentBrush;

        public HsbCanvasBrushSource(IColorState colorState)
            : base(colorState)
        {
            _accentBrush = new SolidColorBrush(DefaultColor);
            _brush = CreateBrush(_accentBrush);
        }

        public override Brush Brush => _brush;

        public override void Update()
        {
            _accentBrush.Color = new HsbColor(ColorState.Hsb.H, 100, 100).ToColor();
        }

        private static DrawingBrush CreateBrush(Brush accentBrush)
        {
            var viewport = new Rect(new Size(1, 1));

            var drawing = new DrawingGroup
            {
                Children = new DrawingCollection(new Drawing[]
                {
                    new GeometryDrawing
                    {
                        Brush = accentBrush,
                        Geometry = new RectangleGeometry(viewport),
                    },
                    new GeometryDrawing
                    {
                        Brush = new LinearGradientBrush(Color.FromArgb(255, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), new Point(0, 0), new Point(1, 0)),
                        Geometry = new RectangleGeometry(viewport),
                    },
                    new GeometryDrawing
                    {
                        Brush = new LinearGradientBrush(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(255, 0, 0, 0), new Point(0, 0), new Point(0, 1)),
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
