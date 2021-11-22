using System.Windows.Media;

namespace Koda.ColorTools.Wpf.BrushSources
{
    public class RgbBlueGradientBrushSource : GradientBrushSource
    {
        public RgbBlueGradientBrushSource() : base(2)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new RgbColor(state.Rgb.R, state.Rgb.G, value * 255).ToColor();
        }
    }
}
