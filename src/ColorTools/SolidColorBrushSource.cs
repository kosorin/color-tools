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

        protected override void OnStateChanged(ColorState state)
        {
            _brush.Color = state.Rgb.ToColor();
        }
    }
}
