namespace ColorTools
{
    public class ColorState : IColorState
    {
        public ColorState(IColor color)
        {
            Update(color);
        }

        public RgbColor Rgb { get; private set; }

        public HsbColor Hsb { get; private set; }

        public HslColor Hsl { get; private set; }

        public void Update(IColor color)
        {
            Rgb = color.ToRgb();
            Hsb = color.ToHsb();
            Hsl = color.ToHsl();
        }
    }
}
