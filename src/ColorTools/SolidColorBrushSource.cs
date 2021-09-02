using System.Windows.Media;

namespace ColorTools
{
    public class SolidColorBrushSource : BrushSource
    {
        private readonly SolidColorBrush _brush;

        public SolidColorBrushSource()
        {
            _brush = new SolidColorBrush(DefaultColor);

            Value = _brush;
        }

        protected override void Update(IColorState colorState)
        {
            _brush.Color = colorState.Rgb.ToColor();
        }
    }
}
