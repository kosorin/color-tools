namespace ColorTools.Components
{
    public class HsbSaturationSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Hsb.S / 100);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HsbColor(picker.Hsb.H, Value * 100, picker.Hsb.B);
        }
    }
}
