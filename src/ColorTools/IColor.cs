using System.Windows.Media;

namespace ColorTools
{
    public interface IColor
    {
        Color ToColor();

        Color ToColor(double alpha);

        RgbColor ToRgb();

        HsbColor ToHsb();

        HslColor ToHsl();
    }
}
