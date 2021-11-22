namespace Koda.ColorTools.Components
{
    public class HsbHueSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Hsb.H / 360);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HsbColor(Value * 360, picker.Hsb.S, picker.Hsb.B);
        }
    }
}
