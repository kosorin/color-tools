namespace ColorTools.Components
{
    public class HslSaturationSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Hsl.S / 100);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HslColor(picker.Hsl.H, Value * 100, picker.Hsl.L);
        }
    }
}
