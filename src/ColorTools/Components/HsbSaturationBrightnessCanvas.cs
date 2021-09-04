using System.Windows;

namespace ColorTools.Components
{
    public class HsbSaturationBrightnessCanvas : ColorCanvas
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, new Point(picker.Hsb.S / 100, picker.Hsb.B / 100));
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HsbColor(picker.Hsb.H, Value.X * 100, Value.Y * 100);
        }
    }
}
