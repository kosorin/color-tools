using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public class HslSaturationGradientBrushSource : GradientBrushSource
    {
        public HslSaturationGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new HslColor(state.Hsl.H, value * 100, state.Hsl.L).ToColor();
        }
    }
}
