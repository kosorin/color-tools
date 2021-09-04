using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public class HsbBrightnessGradientBrushSource : GradientBrushSource
    {
        public HsbBrightnessGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new HsbColor(state.Hsb.H, state.Hsb.S, value * 100).ToColor();
        }
    }
}
