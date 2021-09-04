namespace ColorTools.Components
{
    public class HslHueSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Hsl.H / 360);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HslColor(Value * 360, picker.Hsl.S, picker.Hsl.L);
        }
    }
}
