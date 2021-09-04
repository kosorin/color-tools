namespace ColorTools
{
    public interface IColorPicker : IColorState
    {
        bool IsSet { get; set; }

        double Alpha { get; set; }

        IColor Color { get; set; }

        void BeginUpdate();

        void EndUpdate();

        event ColorPickerChanged? Changed;
    }
}
