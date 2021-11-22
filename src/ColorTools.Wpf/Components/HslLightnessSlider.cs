namespace Koda.ColorTools.Components
{
    public class HslLightnessSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Hsl.L / 100);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HslColor(picker.Hsl.H, picker.Hsl.S, Value * 100);
        }
    }
}
