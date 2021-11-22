namespace Koda.ColorTools.Wpf.Components
{
    public class HsbSaturationSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Hsb.S / 100);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HsbColor(picker.Hsb.H, Value * 100, picker.Hsb.B);
        }
    }
}
