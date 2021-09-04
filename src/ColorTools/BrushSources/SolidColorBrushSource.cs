using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public class SolidColorBrushSource : BrushSource
    {
        private readonly SolidColorBrush _brush;

        public SolidColorBrushSource()
        {
            _brush = new SolidColorBrush(DefaultColor);

            Value = _brush;
        }

        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            _brush.Color = color.ToColor();
        }
    }
}
