using System.Windows.Media;

namespace Koda.ColorTools.Wpf.BrushSources
{
    public class HsbSaturationGradientBrushSource : GradientBrushSource
    {
        public HsbSaturationGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new HsbColor(state.Hsb.H, value * 100, state.Hsb.B).ToColor();
        }
    }
}
