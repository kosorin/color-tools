namespace ColorTools.Components
{
    public class HslLightnessSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Hsl.L / 100);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HslColor(picker.Hsl.H, picker.Hsl.S, Value * 100);
        }
    }
}
