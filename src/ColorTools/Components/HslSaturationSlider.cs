namespace ColorTools.Components
{
    public class HslSaturationSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Hsl.S / 100);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HslColor(picker.Hsl.H, Value * 100, picker.Hsl.L);
        }
    }
}
