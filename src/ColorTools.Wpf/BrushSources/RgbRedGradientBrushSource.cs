using System.Windows.Media;

namespace Koda.ColorTools.Wpf.BrushSources
{
    public class RgbRedGradientBrushSource : GradientBrushSource
    {
        public RgbRedGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new RgbColor(value * 255, state.Rgb.G, state.Rgb.B).ToColor();
        }
    }
}
