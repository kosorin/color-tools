using System.Windows;

namespace ColorTools.Components
{
    public class HsbSaturationBrightnessCanvas : ColorCanvas
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, new Point(picker.Hsb.S / 100, picker.Hsb.B / 100));
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HsbColor(picker.Hsb.H, Value.X * 100, Value.Y * 100);
        }
    }
}
