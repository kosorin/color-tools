using System.Windows;
using System.Windows.Media;

namespace ColorTools
{
    public class HsbCanvasBrushSource : BrushSource
    {
        private readonly SolidColorBrush _accentBrush;

        public HsbCanvasBrushSource()
        {
            _accentBrush = new SolidColorBrush(DefaultColor);

            Value = CreateBrush(_accentBrush);
        }

        protected override void Update(ColorState colorState)
        {
            _accentBrush.Color = new HsbColor(colorState.Hsb.H, 100, 100).ToColor();
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
