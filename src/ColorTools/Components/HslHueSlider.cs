namespace Koda.ColorTools.Components
{
    public class HslHueSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Hsl.H / 360);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HslColor(Value * 360, picker.Hsl.S, picker.Hsl.L);
        }
    }
}
