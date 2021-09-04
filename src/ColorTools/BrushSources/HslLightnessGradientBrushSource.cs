using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public class HslLightnessGradientBrushSource : GradientBrushSource
    {
        public HslLightnessGradientBrushSource() : base(3)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new HslColor(state.Hsl.H, state.Hsl.S, value * 100).ToColor();
        }
    }
}
