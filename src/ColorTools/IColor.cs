namespace Koda.ColorTools
{
    public interface IColor
    {
        RgbColor ToRgb();

        HsbColor ToHsb();

        HslColor ToHsl();
    }
}
