using System.Windows.Media;

namespace Koda.ColorTools.Wpf.BrushSources
{
    public class HslHueGradientBrushSource : GradientBrushSource
    {
        public HslHueGradientBrushSource() : base(7)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new HslColor(value * 360, state.Hsl.S, state.Hsl.L).ToColor();
        }
    }
}
