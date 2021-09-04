using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public class RgbGreenGradientBrushSource : GradientBrushSource
    {
        public RgbGreenGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new RgbColor(state.Rgb.R, value * 255, state.Rgb.B).ToColor();
        }
    }
}
