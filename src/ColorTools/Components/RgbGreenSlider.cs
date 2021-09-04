namespace ColorTools.Components
{
    public class RgbGreenSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Rgb.G / 255);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new RgbColor(picker.Rgb.R, Value * 255, picker.Rgb.B);
        }
    }
}
