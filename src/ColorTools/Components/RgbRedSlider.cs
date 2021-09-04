namespace ColorTools.Components
{
    public class RgbRedSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Rgb.R / 255);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new RgbColor(Value * 255, picker.Rgb.G, picker.Rgb.B);
        }
    }
}
