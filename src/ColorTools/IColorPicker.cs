namespace ColorTools
{
    public interface IColorPicker : IColorState
    {
        bool IsSet { get; set; }

        double Alpha { get; set; }

        IColor Color { get; set; }

        event IsSetChanged? IsSetChanged;

        event AlphaChanged? AlphaChanged;

        event ColorChanged? ColorChanged;
    }
}
