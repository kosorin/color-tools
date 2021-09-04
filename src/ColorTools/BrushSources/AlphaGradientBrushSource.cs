using System.Windows.Media;

namespace ColorTools.BrushSources
{
    public class AlphaGradientBrushSource : GradientBrushSource
    {
        public AlphaGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return state.Rgb.ToColor(value);
        }
    }
}
