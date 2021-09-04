namespace ColorTools.Components
{
    public class RgbGreenSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Rgb.G / 255);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new RgbColor(picker.Rgb.R, Value * 255, picker.Rgb.B);
        }
    }
}
