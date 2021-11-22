namespace Koda.ColorTools.Wpf
{
    public interface IColorState
    {
        RgbColor Rgb { get; }

        HsbColor Hsb { get; }

        HslColor Hsl { get; }
    }
}
