namespace ColorTools.Components
{
    public class HsbHueSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Hsb.H / 360);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HsbColor(Value * 360, picker.Hsb.S, picker.Hsb.B);
        }
    }
}
