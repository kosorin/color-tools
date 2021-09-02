namespace ColorTools
{
    public interface IColorState
    {
        RgbColor Rgb { get; }

        HsbColor Hsb { get; }

        HslColor Hsl { get; }

        event ColorStateChanged? Changed;
    }
}
