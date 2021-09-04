namespace ColorTools.Components
{
    public class HsbBrightnessSlider : ColorSlider
    {
        protected override void OnColorChanged(IColorPicker picker, IColor color)
        {
            base.OnColorChanged(picker, color);

            SetCurrentValue(ValueProperty, picker.Hsb.B / 100);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HsbColor(picker.Hsb.H, picker.Hsb.S, Value * 100);
        }
    }
}
