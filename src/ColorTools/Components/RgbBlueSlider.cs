namespace ColorTools.Components
{
    public class RgbBlueSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Rgb.B / 255);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new RgbColor(picker.Rgb.R, picker.Rgb.G, Value * 255);
        }
    }
}
