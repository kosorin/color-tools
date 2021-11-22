using System.Windows.Media;

namespace Koda.ColorTools
{
    public interface IColorPicker : IColorState
    {
        bool IsSet { get; set; }

        double Alpha { get; set; }

        IColor Color { get; set; }

        void ReplaceSelectedColor(Color? newColor);

        void BeginUpdate();

        void EndUpdate();

        event ColorPickerChanged? Changed;
    }
}
