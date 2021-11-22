using System.Windows.Media;

namespace Koda.ColorTools.BrushSources
{
    public class HsbHueGradientBrushSource : GradientBrushSource
    {
        public HsbHueGradientBrushSource() : base(7)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new HsbColor(value * 360, state.Hsb.S, state.Hsb.B).ToColor();
        }
    }
}
