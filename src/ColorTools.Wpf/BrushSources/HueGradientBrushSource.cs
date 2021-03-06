using System.Windows.Media;

namespace Koda.ColorTools.Wpf.BrushSources
{
    public class HueGradientBrushSource : GradientBrushSource
    {
        public HueGradientBrushSource() : base(7)
        {
        }

        protected override Color GetColor(double value, IColorState state)
        {
            return new HsbColor(value * 360, 100, 100).ToColor();
        }
    }
}
