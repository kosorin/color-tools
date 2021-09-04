namespace ColorTools.Components
{
    public class RgbBlueSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Rgb.B / 255);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new RgbColor(picker.Rgb.R, picker.Rgb.G, Value * 255);
        }
    }
}
